using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
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
            object valor = this.getValorImplicito(entorno,ast);

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

        public object getValorImplicito(Entorno ent, AST arbol)
        {
            return valor;
        }


        string NodoAST.getC3()
        {
            throw new NotImplementedException();
        }
    }
}
