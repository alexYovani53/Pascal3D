using CompiPascal.AST_.interfaces;
using CompiPascal.AST_.valoreImplicito;
using CompiPascal.entorno_;
using CompiPascal.entorno_.simbolos;
using Pascal3D;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.definicion.arrego
{
    public class AsignarArray : Instruccion
    {
        public int tamanoPadre { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }


        private string nombreAcceso { get; set; }

        private LinkedList<Expresion> indices { get; set; }

        private Expresion valorAsignar { get; set; }


        private LinkedList<string> acceso { get; set; }

        private bool esObjeto { get; set; }
        public AsignarArray(string nombreAcceso, LinkedList<Expresion> indices, Expresion valAsignar, int linea, int columna)
        {

            this.nombreAcceso = nombreAcceso;
            this.indices = indices;
            this.valorAsignar = valAsignar;
            this.linea = linea;
            this.columna = columna;
            this.esObjeto = false;
        }

        public AsignarArray(string nombreAcceso, LinkedList<string> acceso, LinkedList<Expresion> indices, Expresion valAsignar, int linea, int columna)
        {

            this.nombreAcceso = nombreAcceso;
            this.indices = indices;
            this.valorAsignar = valAsignar;
            this.linea = linea;
            this.columna = columna;
            this.acceso = acceso;
            this.esObjeto = true;

        }

   

        public string getC3(Entorno ent, AST arbol)
        {
            string codigo = "";
            Simbolo var = ent.obtenerSimbolo(nombreAcceso);
            Identificador buscando = new Identificador(nombreAcceso, linea, columna);

            if (var == null)
            {
                Program.getIntefaz().agregarError($"La variable no se encuentra {nombreAcceso}", linea, columna);
                return "";
            }

            //    Objeto.parametro1[X][x][...]
            if (esObjeto)
            {
                if(!(var is Objeto))
                {
                    Program.getIntefaz().agregarError("el simbolo con el identificador" + nombreAcceso + " no es un objeto", linea, columna);
                    return null;
                }
                result3D valorNuevo = valorAsignar.obtener3D(ent);
                result3D direccionVar = buscando.obtener3D(ent);

                codigo += valorNuevo.Codigo;
                codigo += direccionVar.Codigo;
                codigo += Generador.tabular(cambiarValorRecursivo((Objeto)var, acceso, indices, 0, valorNuevo, direccionVar.Temporal, ent));
            }
            //    arreglo[X][x][...]
            else
            {
                result3D valorNuevo = valorAsignar.obtener3D(ent);
                result3D direccionVar = buscando.obtener3D(ent);
                codigo += valorNuevo.Codigo;
                codigo += direccionVar.Codigo;
                codigo += Generador.tabular( cambioDirecto(var, indices, valorNuevo, direccionVar.Temporal, ent));
            }



            return codigo;
        }


        public string cambiarValorRecursivo(Objeto instancia, LinkedList<string> subPropiedades, LinkedList<Expresion> indices, int indice, result3D valor,string direccion, Entorno ent)
        {
            string codigo = "";
            string temp1 = Generador.pedirTemporal();

            Entorno entornoNoModificado = instancia.getPropiedades();

            // CAPTURAMOS LA PROPIEDAD DEPENDIENDO DEL INDICE     instancia.prop"0".prop"1"......
            string propiedadBuscada = subPropiedades.ElementAt(indice);          //nombre de la propiedad buscada en este nivel
            bool existeParametro = entornoNoModificado.existeEnEntornoActual(propiedadBuscada);
            Simbolo variableEncontrada = entornoNoModificado.obtenerSimbolo(propiedadBuscada);
            Identificador direccionVariable = new Identificador(variableEncontrada.Identificador, 0, 0)
            {
                etiquetaEntornoHeap = direccion
            };
            result3D resultDireccion = direccionVariable.obtener3D(entornoNoModificado);

            codigo += resultDireccion.Codigo;
            codigo += $"{temp1} = Heap[(int){resultDireccion.Temporal}];  /*Capturamos la dirección del la instancia */\n";


            if (!existeParametro) //SI EN ALGUN NIVEL NO SE ENCUENTRA EL PARAMETRO SE RETORNA NULL
            {
                Program.getIntefaz().agregarError($"la variable {propiedadBuscada} no se encuentra en el objeto {instancia.Identificador}", linea, columna);
                return "";                
            }



            // PARA HACER EL CAMBIO DEBEMOS ESTAR EN LA ULTIMA VARIABLE DE LA LISTA 
            // ES DECIR   Objeto.param1.param2      ->  debemos estar leyendo  >>" param2 "<< para poder hacer la asignación
            if (indice == subPropiedades.Count - 1)
            {
                if (!(variableEncontrada is ObjetoArray))
                {
                    Program.getIntefaz().agregarError("No se puede asignar al parametro " + propiedadBuscada + " este no es un arreglo", variableEncontrada.linea, variableEncontrada.columna);
                    return null;
                }

                ObjetoArray arr = (ObjetoArray)variableEncontrada;

                //RECUPERAMOS LOS INIDCES DE ACCESO
                List<string> accesos = new List<string>();

                foreach (Expresion item in indices)
                {
                    result3D resultado = item.obtener3D(ent);
                    if (resultado.TipoResultado != TipoDatos.Integer)
                    {
                        Program.getIntefaz().agregarError("La expresion de acceso no es un integer", linea, columna);
                        return "";
                    }
                    accesos.Add(resultado.Temporal);
                    codigo += resultado.Codigo;
                }

                // COMPROBAMOS LO SIGUIENTE    arr[0][0]   --    accesos [x][x]    ambos deben tener los mismos niveles  
                if (accesos.Count > arr.getNiveles().Count)
                {
                    Program.getIntefaz().agregarError("El acceso ocasiona un desbordamiento por un indice no accesible", linea, columna);
                    return ""; 
                }

                if (valor.TipoResultado != arr.tipoValores)
                {
                    Program.getIntefaz().agregarError("Se intenta setear un valor tipo " + valor.TipoResultado + " en un tipo " + arr.Tipo.ToString(), linea, columna);
                    return null;
                }

                //ESTABLECEMOS EL VALOR Y AGREGAMOS AL ENTORNO
                codigo += arr.establecerValor(accesos, valor,temp1, 0, linea, columna);
              
            }
            else
            {
                //SI EL PARAMETRO SIGUE SIENDO UN OBJETO SEGUIMOS RECORRIENDO
                if (variableEncontrada.Tipo != TipoDatos.Object)
                {
                    Program.getIntefaz().agregarError(" La propiedad " + propiedadBuscada + " no es un objeto", variableEncontrada.linea, variableEncontrada.columna);
                    return null;
                }


                //OBTENEMOS EL OBJECTO CAMBIADO EN LA RECURSIVA
                codigo += cambiarValorRecursivo((Objeto)variableEncontrada, subPropiedades, indices, indice + 1, valor,temp1, ent);

                               
            }

            return codigo;
        }

        public string cambioDirecto(Simbolo arreglo, LinkedList<Expresion> indices,result3D valorNuevo, string inicioArreglo, Entorno ent)
        {
            string codigo = "";

            if (!(arreglo is ObjetoArray))
            {
                Program.getIntefaz().agregarError("El identificador " + nombreAcceso + " no representa un arreglo ", linea, columna);
                return codigo;
            }

            ObjetoArray ob = (ObjetoArray)arreglo;


            //RECUPERAMOS LOS INIDCES DE ACCESO
            List<string> accesos = new List<string>();

            foreach (Expresion item in indices)
            {
                result3D resultado = item.obtener3D(ent);
                if (resultado.TipoResultado != TipoDatos.Integer)
                {
                    Program.getIntefaz().agregarError("La expresion de acceso no es un integer", linea, columna);
                    return codigo;
                }
                accesos.Add(resultado.Temporal);
                codigo += resultado.Codigo;
            }

            // COMPROBAMOS LO SIGUIENTE    arr[0][0]   --    accesos [x][x]    ambos deben tener los mismos niveles  
            if (accesos.Count > ob.getNiveles().Count)
            {
                Program.getIntefaz().agregarError("El acceso ocasiona un desbordamiento por un indice no accesible", linea, columna);
                return codigo;
            }

            if (valorNuevo.TipoResultado != ob.tipoValores)
            {
                Program.getIntefaz().agregarError("Se intenta setear un valor tipo " + valorNuevo.TipoResultado + " en un tipo " + ob.Tipo.ToString(), linea, columna);
                return codigo;
            }

            //ESTABLECEMOS EL VALOR Y AGREGAMOS AL ENTORNO

            codigo += ob.establecerValor(accesos, valorNuevo,inicioArreglo , 0, linea, columna);

            return codigo;

        }

        public void obtenerListasAnidadas(LinkedList<string> variablesUsadas)
        {
            throw new NotImplementedException();
        }
    }
}
