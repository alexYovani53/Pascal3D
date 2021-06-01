using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using CompiPascal.entorno_.simbolos;
using Pascal3D;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.valoreImplicito
{
    public class Acceso : Expresion
    {
        public int tamanoPadre { get; set; }
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
        public int linea { get ;set; }
        public int columna { get ; set ; }

        /**
         * @propiedad       string      idObjeto   
         * @comentario      nombre de la instancia del objeto al cual se accedera a una propiedad de el
         */
        public string idObjeto { get; set; }

        /**
         * @propiedad       string      idPropiedad   
         * @comentario      nombre de la propiedad a la cual se quiere acceder
         */
        public LinkedList<string> listaParametros { get; set; }

        /* @class       Acceso
         * @param       string                  acceso              nombre del objeto
         * @param       LinkedList<string>      parametros          ista de parametros
         */


        public bool retornarSoloDireccion { get; set; }

        public Acceso(string acceso, LinkedList<string> parametro, int linea, int columna)
        {
            this.idObjeto = acceso;
            this.listaParametros = parametro;
            this.linea = linea;
            this.columna = columna;
        }



        public result3D obtener3D(Entorno ent)
        {


            Simbolo objetoInstancia = ent.obtenerSimbolo(idObjeto);

            if(objetoInstancia == null)
            {
                Program.getIntefaz().agregarError($"El identificador {idObjeto} no se encontro declarado", linea, columna);
                return new result3D();
            }

            if(!(objetoInstancia is Objeto))
            {
                Program.getIntefaz().agregarError($"El identificador {idObjeto} no es una instancia de clase", linea, columna);
                return new result3D();
            }

            Objeto instancia = (Objeto)objetoInstancia;

            string codigoFinal = "";
            string temp1 = Generador.pedirTemporal();
            string temp2 = Generador.pedirTemporal();

            codigoFinal += $"{temp1} = SP + {instancia.direccion}; /*Capturamos objeto*/\n";
            codigoFinal += $"{temp2} = Stack[(int){temp1}];\n";
            result3D valor =  accesoRecursivo(instancia, listaParametros, 0,temp2);

            result3D final = new result3D();
            final.Codigo += codigoFinal;
            final.Codigo += valor.Codigo;
            final.Temporal = valor.Temporal;
            final.TipoResultado = valor.TipoResultado;
            final.Referencia = valor.Referencia;

            return final;
        }

        public result3D accesoRecursivo(Objeto instancia,LinkedList<string> listaParams, int indiceParametro, string temporalDireccion)
        {
            result3D final = new result3D();
            string primerVariable = listaParams.ElementAt(indiceParametro);
            bool existeParametro = instancia.getPropiedades().existeEnEntornoActual(primerVariable);

            // revisamos si si existe la variable del indice buscado, en el entorno del objeto actual.
            if (!existeParametro)
            {
                Program.getIntefaz().agregarError($"La variable {primerVariable} no se encontro en {instancia.Identificador}",linea,columna);
                return new result3D();
            }

            // La variable si se encontro y ahora la recuperamos
            Simbolo variableEncontrada = instancia.getPropiedades().obtenerSimbolo(primerVariable);

            final.Codigo = $"{temporalDireccion} = {temporalDireccion} + {variableEncontrada.direccion};  \n";


            // Verificamos que estemos en el indice ultimo de la listaParams, es decir   objeto.param1.param2 := ?  (estar en param2)
            if (indiceParametro == listaParams.Count - 1)
            {
                string temp1 = Generador.pedirTemporal();

                if(variableEncontrada is Objeto || variableEncontrada is ObjetoArray)
                {
                    if (retornarSoloDireccion)
                    {
                        final.Temporal = temporalDireccion;
                    }
                    else
                    {

                        final.Codigo += $"{temp1} = Heap[(int){temporalDireccion}];\n";
                        final.Temporal = temp1;
                    }
                    final.TipoResultado = variableEncontrada.Tipo;
                    final.Referencia = variableEncontrada;
                }
                else
                {
                    if (retornarSoloDireccion)
                    {
                        final.Temporal = temporalDireccion;
                    }
                    else
                    {

                        final.Codigo += $"{temp1} = Heap[(int){temporalDireccion}];\n";
                        final.Temporal = temp1;
                    }
                    final.TipoResultado = variableEncontrada.Tipo;
                }

                return final;
            }
         

            // Si aun no estamos en la ultima variable, verificamos que la actual sea un objeto para seguir buscando
            if((variableEncontrada.Tipo != TipoDatos.Object)){
                Program.getIntefaz().agregarError($"La variable {primerVariable} no es un objeto", linea, columna);
                return new result3D();
            }


            string siguienteDireccion = Generador.pedirTemporal();
            final.Codigo += $"{siguienteDireccion} = Heap[(int){temporalDireccion}];\n";

            result3D codigoSiguiente = accesoRecursivo((Objeto)variableEncontrada, listaParams, indiceParametro + 1,siguienteDireccion);
            final.Codigo += codigoSiguiente.Codigo;
            final.TipoResultado = codigoSiguiente.TipoResultado;
            final.Temporal = codigoSiguiente.Temporal;
            final.Referencia = codigoSiguiente.Referencia;
            return final;
        }

        public void obtenerListasAnidadas(LinkedList<string> variablesUsadas)
        {
            if (!variablesUsadas.Contains(idObjeto))
            {
                variablesUsadas.AddLast(this.idObjeto);
            }
        }
    }
}
