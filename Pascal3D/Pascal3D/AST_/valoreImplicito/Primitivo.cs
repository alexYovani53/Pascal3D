using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.valoreImplicito
{

    /*
     * @class           Primitivo
     * @comentario      esta clase representara valores tales como: Numero, cadena, caracter, true or false, real. 
     * 
     */

    public class Primitivo : Expresion
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

        /**
         * @propiedad   valor
         * @comentario  es el valor de la instancia de primitivo
         */
        private object valor { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }


        public Primitivo(object valor, int linea, int columna)
        {
            this.valor = valor;
            this.linea = linea;
            this.columna = columna;
        }




        public TipoDatos getTipo(Entorno entorno, AST ast)
        {
            

            if (valor is bool)
            {
                return TipoDatos.Boolean;
            }
            else if (valor is string)
            {
                return TipoDatos.String;
            }
            else if (valor is char)
            {
                return TipoDatos.Char;
            }
            else if (valor is int)
            {
                return TipoDatos.Integer;
            }
            else if (valor is double)
            {
                return TipoDatos.Real;
            }
            else
            {
                return TipoDatos.Object;
            }
            
        }

  

        public result3D obtener3D(Entorno ent)
        {

            result3D nuevo = new result3D();
            //string temporal = Generador.pedirTemporal();

            if (valor is int)
            {
                //nuevo.Codigo += temporal + " = " + valor.ToString() + ";";
                //nuevo.Temporal = temporal;

                nuevo.Codigo = "";
                nuevo.Temporal = valor.ToString();
                nuevo.TipoResultado = TipoDatos.Integer;
            }
            else if( valor is double)
            {
                //nuevo.Codigo += temporal + " = " + valor.ToString() + ";";
                //nuevo.Temporal = temporal;

                nuevo.Codigo = "";
                nuevo.Temporal = valor.ToString();
                nuevo.TipoResultado = TipoDatos.Real;
            }
            else if(valor is bool)
            {
                //CONVERTIMOS EL VALOR TRUE O FALSO EN  1 o 0  RESPECTIVAMENTE
                int val = (bool)valor == true ? 1 : 0;
                nuevo.Codigo = "";
                nuevo.Temporal = val.ToString();
                nuevo.TipoResultado = TipoDatos.Boolean;
            }

            return nuevo;
        }

        public string getC3()
        {
            throw new NotImplementedException();
        }
    }
}
