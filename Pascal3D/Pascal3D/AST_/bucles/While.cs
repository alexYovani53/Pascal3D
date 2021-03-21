using CompiPascal.AST_.cambioFlujo;
using CompiPascal.AST_.interfaces;
using CompiPascal.AST_.valoreImplicito;
using CompiPascal.entorno_;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.AST_.bucles
{
    public class While:Condicional,Instruccion
    {


        /*@class            while
         *@comentario       Instruccion while, la condicion e instrucion se guardan en la clase de la que hereda
         */

        public int linea { get; set; }
        public int columna { get; set; }

        public While(Expresion condicion, LinkedList<Instruccion> instrucciones , int linea, int columna):base(condicion, instrucciones)
        {
            this.linea = linea;
            this.columna = columna;

        }


        string NodoAST.getC3()
        {
            throw new NotImplementedException();
        }
    }
}
