using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.entorno_
{
    public class reporteVar
    {


        public string nombre { get; set; }
        public string Tipo { get; set; }
        public string Ambito { get; set; }

        public string Fila { get; set; }

        public string Columna { get; set; }
        public string Nivel { get; set; }


        public reporteVar(string nombre, string tipo, string ambito, string fila, string columna, string nivel)
        {
            this.nombre = nombre;
            this.Tipo = tipo;
            this.Ambito = ambito;
            this.Fila = fila;
            this.Columna = columna;
            this.Nivel = nivel;
        }



    }
}
