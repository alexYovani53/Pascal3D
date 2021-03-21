using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.AST_
{
    public class Error
    {

        public enum tipoError
        {
            Semantico,
            Sintactico,
            Lexico
        }


        public tipoError TipoE;

        public string descripcion;

        public int fila;

        public int Columna;

        public Error(tipoError tipo, string descripcion, int fila, int columna)
        {
            this.TipoE = tipo;
            this.descripcion = descripcion;
            this.fila = fila;
            this.Columna = columna;
        }


    }
}
