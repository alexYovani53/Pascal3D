using Pascal3D.Optimizador.InterfacesOp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pascal3D.Optimizador.PrimitivosOp
{
    public class NumeroOp : ExpresionOp
    {
        public int linea { get; set; }
        public int columna { get; set; }

        public object valor { get; set; }

        public NumeroOp(object valor, int linea, int columna)
        {
            this.valor = valor;
            this.linea = linea;
            this.columna = columna;
        }

        public string getValor()
        {
            return valor.ToString();
        }
    }
}
