
using System;
using System.Collections.Generic;
using System.Text;
using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;

namespace CompiPascal.AST_.funcionesPrimitivas
{
    public class Write : Instruccion
    {

        /*
         * @param           linea
         * @comentario      contendra la linea donde aparecio la instruccion
         */
        public int linea { get; set; }

        /*
         * @param           columna
         * @comentario      contendra la columna donde aparecio la instruccion
         */
        public int columna { get; set; }

        /*
         * @param           expr
         * @comentario      contendra la expresion que se va a imprimir
         */
        public LinkedList<Expresion> expr_imprimir;


        public bool saltoLinea { get; set; }


        public Write(LinkedList<Expresion> expr,bool saltoLinea, int linea, int columna)
        {
            this.expr_imprimir = expr;
            this.linea = linea;
            this.columna = columna;
            this.saltoLinea = saltoLinea;

        }

        public Write(bool saltolinea, int linea, int columna)
        {
            this.linea = linea;
            this.columna = columna;
            this.saltoLinea = saltolinea;
            this.expr_imprimir = null;
        }


        string NodoAST.getC3()
        {
            throw new NotImplementedException();
        }
    }
}
