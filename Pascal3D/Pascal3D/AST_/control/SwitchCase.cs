using CompiPascal.AST_.cambioFlujo;
using CompiPascal.AST_.interfaces;
using CompiPascal.AST_.valoreImplicito;
using CompiPascal.entorno_;
using CompiPascal.entorno_.simbolos;
using System;
using System.Collections.Generic;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.control
{
    public class SwitchCase : Instruccion
    {

        private Expresion exprValidar { get; set; }

        private LinkedList<Case> casos { get; set; }

        private Case casoDefault { get; set; }

        public int linea { get; set; }
        public int columna { get; set; }

        public SwitchCase(Expresion exprValidar,  LinkedList<Case> casos, int linea, int columna )
        {
            this.exprValidar = exprValidar;
            this.casos = casos;
            this.linea = linea;
            this.columna = columna;

        }

        public SwitchCase(Expresion exprValidar, LinkedList<Case> casos, Case casoElse, int linea, int columna)
        {
            this.exprValidar = exprValidar;
            this.casoDefault = casoElse;
            this.casos = casos;
            this.linea = linea;
            this.columna = columna;

        }



        public string getC3()
        {
            throw new NotImplementedException();
        }
    }
}
