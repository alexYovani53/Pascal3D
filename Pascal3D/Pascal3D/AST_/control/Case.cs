using CompiPascal.AST_.cambioFlujo;
using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.AST_.control
{
    public class Case:Instruccion
    {



        public Expresion expresionCase { get; set; }

        LinkedList<Instruccion> instrucciones;
        public int linea { get; set; }
        public int columna { get; set; }

        public Case(Expresion caso, LinkedList<Instruccion> instrucciones,int linea, int columa)
        {
            this.expresionCase = caso;
            this.instrucciones = instrucciones;
            this.linea = linea;
            this.columna = columna;
        }

        public string getC3()
        {
            throw new NotImplementedException();
        }
    }
}
