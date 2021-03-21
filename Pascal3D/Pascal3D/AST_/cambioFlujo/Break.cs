using CompiPascal.AST_.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.AST_.cambioFlujo
{
    public class Break : Instruccion
    {

        public bool siPuedeRetornar { get; set; }
        public int linea { get ; set; }
        public int columna { get;set; }

        public Break(int linea, int columna)
        {
            this.linea = linea;
            this.columna = columna;
        }



        public string getC3()
        {
            throw new NotImplementedException();
        }
    }
}
