using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.AST_.cambioFlujo
{
    public class Break : Instruccion
    {

        public int tamanoPadre { get; set; }
        public bool siPuedeRetornar { get; set; }
        public int linea { get ; set; }
        public int columna { get;set; }

        public Break(int linea, int columna)
        {
            this.linea = linea;
            this.columna = columna;
        }



        public string getC3(Entorno ent, AST arbol)
        {
            string codigo = "";
            if (siPuedeRetornar)
            {
                codigo = "#BREAK#\n";
            }

            return Generador.tabular(codigo);
        }
    }
}
