using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using System;
using System.Collections.Generic;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.definicion.arrego
{
    public class DeclaraArray2:Instruccion
    {
        public int tamanoPadre { get; set; }
        public int linea { get ; set; }
        public int columna { get ; set; }

        LinkedList<Simbolo> identificadores { get; set; }
        
        string nombreObjeto { get; set; }

        LinkedList<Expresion[]> niveles { get; set; }

        TipoDatos tipo { get; set; }


        public DeclaraArray2(LinkedList<Simbolo> variables, string tipoObjeto, TipoDatos tipo, LinkedList<Expresion[]> niveles, int linea, int columna)
        {
            this.identificadores = variables;
            this.linea = linea;
            this.columna = columna;
            this.nombreObjeto = tipoObjeto;
            this.niveles = niveles;
            this.tipo = tipo;
        }



        public string getC3(Entorno ent, AST arbol)
        {
            throw new NotImplementedException();
        }

        public void obtenerListasAnidadas(LinkedList<string> variablesUsadas)
        {
            throw new NotImplementedException();
        }
    }
}
