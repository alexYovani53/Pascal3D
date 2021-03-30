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
        public Acceso(string acceso, LinkedList<string> parametro, int linea, int columna)
        {
            this.idObjeto = acceso;
            this.listaParametros = parametro;
            this.linea = linea;
            this.columna = columna;
        }



        public result3D obtener3D(Entorno ent)
        {
            throw new NotImplementedException();
        }
    }
}
