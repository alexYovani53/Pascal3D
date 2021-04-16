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
            this.linea = linea;
            this.columna = columna;

        }
        public Exit(TipoDatos salidavoid, int linea, int columna)
        {
            this.tipoRet = salidavoid;
            this.linea = linea;
            this.columna = columna;
        }

        public string getC3(Entorno ent)
        {

            string codigo = "/*CODIGO PARA INSTRUCCIÓN RETURN*/\n";
            string temp1 = Generador.pedirTemporal();

            //CAPTURAMOS EL TAMAÑO DEL ENTORNO ACTUAL
            codigo += $"{temp1} = SP + 0; /*En el entorno actual el retorno esta en SP + 0 (inicio) del entorno*/\n";
            
            if(valorSalida == null)
            {
                codigo += $"Stack[(int){temp1}]=-1; /* Retorno tipo void*/ \n";
            }
            else
            {
                result3D resultExpresion = valorSalida.obtener3D(ent);
                codigo += resultExpresion.Codigo;
                codigo += $"Stack[(int){temp1}] = {resultExpresion.Temporal}; /* Asiganción de valor de retorno */\n";
                codigo += "#EXIT# \n";
            }
            codigo += "/*FINAL CODIGO INSTRUCCION RETURN*/\n";


            return Generador.tabular( codigo);
        }

        public result3D obtener3D(Entorno ent)
        {
            result3D retorno = new result3D();
            retorno.Codigo = getC3(ent);
            retorno.Temporal = Generador.pedirTemporal();
            retorno.TipoResultado = tipoRet;

            return retorno;
        }
    }
}
