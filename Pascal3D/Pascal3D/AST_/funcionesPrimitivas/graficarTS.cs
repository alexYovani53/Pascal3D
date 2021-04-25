using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using CompiPascal.entorno_.simbolos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CompiPascal.AST_.funcionesPrimitivas
{
    public class graficarTS : Instruccion
    {


        public int tamanoPadre { get; set; }
        public int linea { get; set ; }
        public int columna { get; set; }



        public graficarTS(int fila, int columna)
        {
            this.linea = fila;
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
