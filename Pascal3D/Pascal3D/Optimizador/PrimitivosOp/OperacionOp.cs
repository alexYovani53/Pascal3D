using Pascal3D.Optimizador.InterfacesOp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pascal3D.Optimizador.PrimitivosOp
{
    public class OperacionOp : ExpresionOp
    {

        public int linea { get; set; }
        public int columna { get; set; }

        public enum tipoOperacionOpti
        {
            suma,
            resta,
            multiplicacion,
            division,
            modulo
        }

        public ExpresionOp izq { get; set; }

        public ExpresionOp der { get; set; }

        public tipoOperacionOpti operador { get; set; }

        public object valor { get; set; }
        public OperacionOp(ExpresionOp izq, tipoOperacionOpti operador, ExpresionOp der, int linea, int columna)
        {
            this.izq = izq;
            this.operador = operador;
            this.der = der;
            this.linea = linea;
            this.columna = columna;

        }
        
        public string convertirOperador()
        {
            switch (operador)
            {
                case tipoOperacionOpti.suma:return "+";
                case tipoOperacionOpti.resta:return "-";
                case tipoOperacionOpti.multiplicacion:return "*";
                case tipoOperacionOpti.division:return "/";
                case tipoOperacionOpti.modulo:return "%";
                default: return "*";
            }
        }

        public string getValor()
        {
            return $"{izq.getValor()} {convertirOperador()} {der.getValor()}";
        }
    }
}
