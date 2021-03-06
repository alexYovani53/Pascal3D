using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.entorno_.simbolos
{
    public class Objeto:Simbolo , ICloneable
    {

        /* @propiedad    Entorno      entornoPropio
         * @comentario   esta propiedad guardara en un entorno (tabla de simbolos) las definiciones dentro del struct que lo genera
         */

        Entorno entornoPropio { get; set; }


        /* @propiedad    string      nombre
         * @comentario   esta propiedad guardara el nombre de la estructura generadora
         */
        public string nombreStructura { get; set; }


        public Objeto(string nombreObjeto,string nombreStructura, Entorno entornoPropio,int posicionRelativa,int linea, int columna):base(TipoDatos.Object,nombreObjeto,posicionRelativa,linea,columna)
        {
            this.nombreStructura = nombreStructura;
            this.entornoPropio = entornoPropio;
        }

               
        public Entorno getPropiedades()
        {
            return entornoPropio;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
