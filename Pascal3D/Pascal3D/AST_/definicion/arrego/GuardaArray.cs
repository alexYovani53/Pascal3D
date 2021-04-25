using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using System;
using System.Collections.Generic;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.definicion.arrego
{
    public class GuardaArray : Instruccion
    {
        public int tamanoPadre { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }

        public string identificador { get; set; }

        public string tipoObjeto { get; set; }

        public TipoDatos  tipo { get; set; }

        public  LinkedList<Expresion[]> niveles { get; set; }

        public GuardaArray(string ide, string tipoObjeto, TipoDatos tipo, LinkedList<Expresion[]> niveles, int linea, int columna)
        {
            this.identificador = ide;
            this.tipoObjeto = tipoObjeto;
            this.tipo = tipo;
            this.niveles = niveles;
            this.linea = linea;
            this.columna = columna;
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
