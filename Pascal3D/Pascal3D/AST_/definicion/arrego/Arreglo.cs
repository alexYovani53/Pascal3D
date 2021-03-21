using CompiPascal.AST_.interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.definicion.arrego
{
    public class Arreglo
    {


        public int linea { get; set; }
        public int columna { get; set; }

        public  TipoDatos tipoArreglo;


        public List<int[]> niveles { get; set; }

        /* @propiedad    String      identificador
         * @comentario   esta propiedad guardara el identificador con el que se denominara el arreglo
         */

        public  string nombreArreglo { get; set; }
        public string nombreObjeto { get; set; }

        public Arreglo(string nombreStuct, TipoDatos tipoArray, List<int[]> niveles)
        {
            this.nombreArreglo = nombreStuct;
            this.tipoArreglo = tipoArray;
            this.niveles = niveles;
        }

        public Arreglo(string nombreStuct, string objeto, TipoDatos tipoArray, List<int[]> niveles)
        {
            this.nombreArreglo = nombreStuct;
            this.tipoArreglo = tipoArray;
            this.nombreObjeto = objeto;
            this.niveles = niveles;
        }

        

    }
}
