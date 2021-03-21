using CompiPascal.AST_.cambioFlujo;
using CompiPascal.AST_.interfaces;
using CompiPascal.AST_.valoreImplicito;
using CompiPascal.entorno_;
using System;
using System.Collections.Generic;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.bucles
{
    public class Repeat : Condicional, Instruccion
    {

        public int linea { get; set; }
        public int columna { get; set; }
        public Repeat(Expresion condicional, LinkedList<Instruccion> instrucciones, int linea, int columna):
            base(condicional,instrucciones)
        {
            this.linea = linea;
            this.columna = columna;
        }

        public string getC3() 
        {
            return "";
        }
    }
}
