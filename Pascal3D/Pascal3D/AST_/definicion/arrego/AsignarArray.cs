using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using CompiPascal.entorno_.simbolos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.definicion.arrego
{
    public class AsignarArray : Instruccion
    {
        public int linea { get; set; }
        public int columna { get; set; }


        private string nombreAcceso { get; set; }

        private LinkedList<Expresion> indices { get; set; }

        private Expresion valorAsignar { get; set; }


        private LinkedList<string> acceso { get; set; }

        private bool esObjeto { get; set; }
        public AsignarArray(string nombreAcceso, LinkedList<Expresion> indices, Expresion valAsignar, int linea, int columna)
        {

            this.nombreAcceso = nombreAcceso;
            this.indices = indices;
            this.valorAsignar = valAsignar;
            this.linea = linea;
            this.columna = columna;
            this.esObjeto = false;
        }

        public AsignarArray(string nombreAcceso, LinkedList<string> acceso, LinkedList<Expresion> indices, Expresion valAsignar, int linea, int columna)
        {

            this.nombreAcceso = nombreAcceso;
            this.indices = indices;
            this.valorAsignar = valAsignar;
            this.linea = linea;
            this.columna = columna;
            this.acceso = acceso;
            this.esObjeto = true;

        }

   

        public string getC3()
        {
            throw new NotImplementedException();
        }
    }
}
