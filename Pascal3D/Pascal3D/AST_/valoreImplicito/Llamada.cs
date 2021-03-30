using CompiPascal.AST_.definicion;
using CompiPascal.AST_.definicion.arrego;
using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using CompiPascal.entorno_.simbolos;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.valoreImplicito
{
    public class Llamada : Expresion, Instruccion
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
        public int linea { get; set ;}
        public int columna { get; set; }

        /* @parametro   string      nombreLlamada       nombre del la funcion o  procedimiento al que se llama
         **/
        private string nombreLlamada { get; set; }

        /* @parametro   LinkedList<Expresion>      expresionesValor       lista de las expresiones a asignar en los parametros de la funcion
         */
        private LinkedList<Expresion> expresionesValor { get; set; }


        public Llamada(string nombreLlamada, LinkedList<Expresion> expresiones,int linea, int columna)
        {
            this.nombreLlamada = nombreLlamada;
            this.expresionesValor = expresiones;
            this.linea = linea;
            this.columna = columna;
        }


        public TipoDatos getTipo(Entorno entorno, AST ast) {

            Funcion existeFuncion = ast.getFuncion(nombreLlamada);

            if (existeFuncion==null)
            {
                return TipoDatos.NULL;
            }

    
            return existeFuncion.Tipo;
        }

        public string getC3(Entorno ent)
        {
            return "";
        }

        public result3D obtener3D(Entorno ent)
        {
            throw new NotImplementedException();
        }
    }
}
