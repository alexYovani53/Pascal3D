using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.cambioFlujo
{
    public class Exit : Expresion,Instruccion
    {

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
        public int linea { get ; set ; }
        public int columna { get; set ; }

        /* @parametro       bool          salidaVoid        
         * @comentario      Esta variabe guarda una bandera que indica si e exit esta en una funcion o metodo
         */
        private TipoDatos tipoRet { get; set; }

        /* @parametro       Expresion     valorSalida       
         * @comentario      Esta variabe guarda el valor de salida que se devuelve por el exit
         */
        private Expresion valorSalida { get; set; }



        public Exit(Expresion expr, TipoDatos salidaVoid, int linea, int columna)
        {
            this.valorSalida = expr;
            this.tipoRet = salidaVoid;

        }
        public Exit(TipoDatos salidavoid, int linea, int columna)
        {
            this.tipoRet = salidavoid;
        }

        public TipoDatos esVoid()
        {
            return tipoRet;
        }




        public string getC3()
        {
            throw new NotImplementedException();
        }

        public result3D obtener3D(Entorno ent)
        {
            throw new NotImplementedException();
        }
    }
}
