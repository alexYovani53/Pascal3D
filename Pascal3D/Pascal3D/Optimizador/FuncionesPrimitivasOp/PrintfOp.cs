using Pascal3D.Optimizador.InterfacesOp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pascal3D.Optimizador.FuncionesPrimitivasOp
{
    public class PrintfOp : InstruccionOp
    {
        public int linea { get; set; }
        public int columna { get; set; }

        private string cadena { get; set; }

        private string tipoCasteo { get; set; }

        private ExpresionOp valor { get; set; }

        public PrintfOp(string cadena,string tipoCasteo, ExpresionOp valor, int linea, int columna)
        {
            this.cadena = cadena;
            this.tipoCasteo = tipoCasteo;
            this.linea = linea;
            this.columna = columna;
            this.valor = valor;
        }

        public object ejecutarOptimizacion()
        {
            return "";
        }
        public string pedirCodigo()
        {
            if(cadena.Length > 3)
            {
                cadena = cadena.Substring(1, cadena.Length - 2);
            }
            return $"printf(\"{cadena}\",({tipoCasteo}){valor.getValor()});\n";
        }
    }
}
