using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.entorno_.simbolos
{
    public class ObjetoArray : Simbolo,ICloneable
    {



        /* @propiedad    string      nombre
         * @comentario   esta propiedad guardara el nombre de la estructura tipo arreglo que la genero
         */
        string nombreStructArray { get; set; }

        public object[] valores {get;set;}

        private List<int[]> niveles { get; set; }

        public ObjetoArray(string nombreObjeto, string nombreStructArray,TipoDatos tipoArr, object []vals,List<int[]> niveles,int linea, int columna)
            : base(tipoArr, nombreObjeto,linea,columna)
        {
            this.nombreStructArray = nombreStructArray;
            this.valores = vals;
            this.niveles = niveles;
        }




        public List<int[]> getNiveles()
        {
            return niveles;
        }


        /* funcion que retorna el nombre (propiedad no accesible desde fuera)
         */
        public string getNombreGenerador()
        {
            return nombreStructArray;
        }


        internal object[] getArreglo()
        {
            return this.valores;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
