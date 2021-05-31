using Pascal3D.Optimizador.InterfacesOp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pascal3D.Optimizador.Funcion
{
    public class ReturnOp:InstruccionOp
    {
        public int linea { get; set; }
        public int columna { get; set; }

        public ExpresionOp expr { get; set; }

        public ReturnOp(ExpresionOp expresionRetur, int linea, int columna)
        {
            this.expr = expresionRetur;
            this.linea = linea;
            this.columna = columna;
        }

        public ReturnOp(int linea, int columna)
        {
            this.linea = linea;
            this.columna = columna;
            this.expr = null;
        }

        public string pedirCodigo()
        {
            if (expr != null)
                return $"return {expr.getValor()};\n";
            else
                return "return;\n";
        }
        public object ejecutarOptimizacion()
        {
            return "";
        }
    }
}
