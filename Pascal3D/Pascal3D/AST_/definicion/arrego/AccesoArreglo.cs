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
    public class AccesoArreglo : Expresion
    {

        // ESTE SERVIRA CUANDO EN UN ARREGLO, EL TIPO DE DATOS ES UN OBJETO, DEBÍDO A QUE EN 3D NO SE CREA UNA MATRIZ CON LOS OBJETOS
        // SE GUARDA UN UNIDO OBJETO QUE IDENTIFICA A TODOS LOS DEMAS EN EL ARREGLO, ESO PARA PODER ACCEDER A LOS PARAMETROS DEL MISMO EN UN ACCESO
        public Objeto objetoAuxiliar { get; set; }

        /*
         * @param   string      etiquetaFalsa              Guarda la siguiente etiqueta para una instrucción donde se 
         *                                                  evalua una expresión condicional
         */
        public string etiquetaFalsa { get; set; }
        /*
         * @param   string      etiquetaVerdadera           Guarda la etiqueta verdadera para una instrucción donde se 
         *                                                  evalua una expresión condicional
         */
        public string etiquetaVerdadera { get; set; }
        public int linea { get; set; }
        public int columna { get; set ; }

        private string nombreAcceso { get; set; }

        private LinkedList<Expresion> indices { get; set; }

        private LinkedList<string> acceso { get; set; }
        private LinkedList<string> acceso2 { get; set; }

        public bool arregloEn_Struct { get; set; }

        public AccesoArreglo(string ide,LinkedList<Expresion> indices,int linea, int columna)
        {
            this.nombreAcceso = ide;
            this.indices = indices;
            this.linea = linea;
            this.columna = columna;
            this.arregloEn_Struct = false;
            this.acceso2 = null;
        }

        public AccesoArreglo(string ide, LinkedList<Expresion> indices, LinkedList<string> acceso, int linea, int columna)
        {
            this.nombreAcceso = ide;
            this.indices = indices;
            this.linea = linea;
            this.columna = columna;
            this.arregloEn_Struct = false;
            this.acceso2 = acceso;
        }

        public AccesoArreglo(string ide, LinkedList<string> acceso, LinkedList<Expresion> indices, int linea, int columna)
        {
            this.nombreAcceso = ide;
            this.indices = indices;
            this.linea = linea;
            this.columna = columna;
            this.acceso = acceso;
            this.arregloEn_Struct = true;
            this.acceso2 = null;
        }
        public AccesoArreglo(string ide, LinkedList<string> acceso, LinkedList<string> acceso2, LinkedList<Expresion> indices, int linea, int columna)
        {
            this.nombreAcceso = ide;
            this.indices = indices;
            this.linea = linea;
            this.columna = columna;
            this.acceso = acceso;
            this.acceso2 = acceso2;
            this.arregloEn_Struct = true;
        }



        public result3D obtener3D(Entorno ent)
        {

            string codigo = "";
            //BUSCAMOS LA VARIABLE DE TIPO ARREGLO EN EL ENTORNO 
            bool existeObjetoArreglo = ent.existeSimbolo(nombreAcceso);

            if (!existeObjetoArreglo)
            {
                Program.getIntefaz().agregarError($"No se encontro la variable {nombreAcceso}", linea, columna);
                return new result3D();
            }

            //Se tiene una variable de tipo objeto o arreglo en el entorno global, al obtener el valor de esa variable obtenemos
            // una dirección hacia el heap donde comienza el objeto
            Identificador buscarDireccion = new Identificador(nombreAcceso, 0, 0);
            result3D direccion = buscarDireccion.obtener3D(ent);        //Este lleva la dirección que apunta al heap donde esta el objeto
            Simbolo variableArreglo = ent.obtenerSimbolo(nombreAcceso);


            codigo += direccion.Codigo;

            //  ACCESO A UN ARREGLO EN UN OBJETO
            //  Sería como        nombreStruct.arreglo[0][0]
            if (arregloEn_Struct)
            {
                result3D resultado = new result3D();
                resultado.Codigo += codigo;
                result3D val_ =obtenerValor_parametro((Objeto)variableArreglo, acceso, 0,direccion.Temporal, ((Objeto)variableArreglo).getPropiedades(), null);
                resultado.Codigo += val_.Codigo;
                resultado.Temporal = val_.Temporal;
                resultado.TipoResultado = val_.TipoResultado;
                return resultado;
            }

            // Si el arreglo no esta dentro de un objeto llega hasta aca
            // Sería como  arreglo[0][0]

            // ahora comprobamos que si sea un arreglo
            if(!(variableArreglo is ObjetoArray))
            {
                Program.getIntefaz().agregarError($"El simbolo {variableArreglo.Identificador} no es un arreglo", linea, columna);
                return new result3D();
            }

            ObjetoArray instanciaArreglo = (ObjetoArray)variableArreglo;

            result3D valorFinal = new result3D();
            result3D valor_ = obtenerValorDirecto(instanciaArreglo, direccion.Temporal, ent, null);
            valorFinal.Codigo += codigo;
            valorFinal.Codigo += valor_.Codigo;
            valorFinal.Temporal = valor_.Temporal;
            valorFinal.TipoResultado = valor_.TipoResultado;

            return valorFinal;
        }

        public result3D obtenerValor_parametro(Objeto variable, LinkedList<string> acceso, int indice,string direccionObjeto, Entorno ent, AST arbol)
        {
            result3D codigo = new result3D();
            string temp1 = Generador.pedirTemporal();

            bool buscandoAcceso = variable.getPropiedades().existeEnEntornoActual(acceso.ElementAt(indice));
            if (!buscandoAcceso)
            {
                Program.getIntefaz().agregarError($"La propiedad {acceso.ElementAt(indice)} no existe en el objeto {variable.Identificador}", linea, columna);
                return codigo;
            }

            // CAPTURA DEL SIMBOLO Y BUSQUEDA DE SU DIRECCIÓN EN HEAP, YA QUE ES UN PROPIEDAD DE UN OBJETO, YA NO RESIDE EN EL STACK SI NO EN EL HEAP
            Simbolo propiedad = variable.getPropiedades().obtenerSimbolo(acceso.ElementAt(indice));
            Identificador variableArreglo = new Identificador(propiedad.Identificador, 0, 0)
            {
                etiquetaEntornoHeap = direccionObjeto
            };
            result3D busqueda = variableArreglo.obtener3D(ent);
            codigo.Codigo += busqueda.Codigo;
            codigo.Codigo += $"{temp1} =  Heap[(int){busqueda.Temporal}];";

            if (acceso.Count -1 == indice)
            {
                // comprovamos que el ultimo parametro sea de tipo arreglo para acceder asi    propiedad[x]...[x]
                if(!(propiedad is ObjetoArray))
                {
                    Program.getIntefaz().agregarError($"El objeto {propiedad.Identificador} no es un arreglo",linea,columna);
                    return codigo;
                }

                ObjetoArray propiedad_ = (ObjetoArray)propiedad;

                result3D resultado = obtenerValorDirecto(propiedad_, temp1, ent, arbol);

                codigo.Codigo += resultado.Codigo;
                codigo.Temporal = resultado.Temporal;
                codigo.TipoResultado = resultado.TipoResultado;
                if (codigo.TipoResultado == TipoDatos.Object) this.objetoAuxiliar = propiedad_.objetoParaAcceso;
                return codigo;

            }

            if(propiedad.Tipo != TipoDatos.Object)
            {
                Program.getIntefaz().agregarError($"El simbolo {propiedad.Identificador} no es un objeto",linea,columna);
                return codigo;
            }


            result3D siguiente =  obtenerValor_parametro((Objeto)propiedad, acceso, indice + 1, temp1,((Objeto)propiedad).getPropiedades(),arbol);
            codigo.Codigo += Generador.tabular(siguiente.Codigo);
            codigo.Temporal = siguiente.Temporal;
            codigo.TipoResultado = siguiente.TipoResultado;

            return codigo;
        }

        //ACCESODIRECTO OBTIENE EL VALOR DEL ARREGLO EN LA POSICION DADA. PASANDO COMO PARAMETRO EL OBJETO ARRAY EN SI 
        public result3D obtenerValorDirecto(ObjetoArray variable,string direccion, Entorno ent, AST arbol)
        {
            //CODIGO DEL ACCESO
            string codigo = "";


            //RECUPERAMOS LOS INIDCES DE ACCESO
            List<string> accesos = new List<string>();

            // REcorremos los indices de acceso del arreglo
            foreach (Expresion item in indices)
            {
                result3D resultado = item.obtener3D(ent);

                if (resultado.TipoResultado != TipoDatos.Integer)
                {
                    Program.getIntefaz().agregarError("La expresion de acceso no es un integer", linea, columna);
                    return null;
                }
                codigo += resultado.Codigo;
                accesos.Add(resultado.Temporal);
            }

            //ANTES DE IR A OBTENER EL VALOR VALIDAMOS QUE EL ACCESO NO SOBREPASE LOS LIMITES DEL ARREGLO

            if(accesos.Count > variable.getNiveles().Count)
            {
                Program.getIntefaz().agregarError("Acceso fuera de rango", linea, columna);
                return new result3D();
            }

            // OBTENEMOS EL VALOR EN EL OBJETO ARRAY 
            result3D valorEncontrado = variable.valor3D(accesos,ent,0,direccion);

            // RECOPILAMOS LA INFORMACIÓN 
            result3D final = new result3D();
            final.Codigo += codigo;
            final.Codigo += valorEncontrado.Codigo;


            final.Temporal = valorEncontrado.Temporal;
            final.TipoResultado = valorEncontrado.TipoResultado;

            if (final.TipoResultado == TipoDatos.Object) this.objetoAuxiliar = variable.objetoParaAcceso;

            return final;
        }


        public result3D obtenerPosicionVar(Entorno ent, string identificador)
        {

            result3D regresos = new result3D();
            string tempora1 = Generador.pedirTemporal();

            regresos.Codigo += $"\n/*BUSCANDO DIRECCION DE UN IDENTIFICADOR*/\n";
            regresos.Codigo += $"{tempora1} = SP; \n";

            for (Entorno actual = ent; actual != null; actual = actual.entAnterior())
            {

                foreach (Simbolo item in actual.TablaSimbolos())
                {
                    if (item.Identificador.Equals(identificador))
                    {

                        regresos.Codigo += $"{tempora1} = {tempora1} + {item.direccion};           /*Capturamos la direccion donde se encuentra el ide, tomado de la tabla de simbolos*/\n";

                        /* CUANDO LA ASIGNACIÓN ES A UNA VARIABLE POR REFERENCIA EN UNA FUNCION, LA VARIABLE GUARDA LA REFERENCIA
                         * HACIA EL STACK 
                         */

                        if (item.porReferencia)
                        {
                            string temporal2 = Generador.pedirTemporal();
                            regresos.Codigo += $"{temporal2} = Stack[(int) {tempora1}];             /* Variable por referencia, puntero*/ \n";
                            regresos.Temporal = temporal2;
                        }
                        else regresos.Temporal = tempora1;


                        regresos.Codigo += $"/*BUSCANDO DIRECCION DE UN IDENTIFICADOR -> ENCONTRADO*/\n\n";


                        regresos.TipoResultado = item.Tipo;
                        return regresos;
                    }
                }

                if (actual.entAnterior() != null)
                {
                    regresos.Codigo += $"{tempora1} = 0;             /*Retrocedemos entre los entornos*/\n";
                }
            }

            return regresos;
        }


        public void obtenerListasAnidadas(LinkedList<string> variablesUsadas)
        {
            variablesUsadas.AddLast(this.nombreAcceso);
        }
    }

}
