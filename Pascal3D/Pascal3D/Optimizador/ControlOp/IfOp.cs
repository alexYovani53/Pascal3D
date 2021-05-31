using Pascal3D.Optimizador.InterfacesOp;
using Pascal3D.Optimizador.PrimitivosOp;
using System;
using System.Collections.Generic;
using System.Text;
using static Pascal3D.Optimizador.PrimitivosOp.OperacionOp;

namespace Pascal3D.Optimizador.ControlOp
{
    public class IfOp : InstruccionOp
    {

        public int linea { get; set; }
        public int columna { get; set; }

        private ExpresionOp izq { get; set; }

        private ExpresionOp derecha { get; set; }

        public enum tipoComparacion
        {
            igual = 1,
            diferente = 2,
            menor = 3,
            menorque = 4,
            mayor = 5,
            mayorque = 6
        }

        public tipoComparacion comparacion { get; set; }

        public Label salto { get; set; }
        public IfOp(ExpresionOp izq, tipoComparacion comparacion, ExpresionOp der, Label salto, int linea, int columna)
        {
            this.izq = izq;
            this.derecha = der;
            this.comparacion = comparacion;
            this.linea = linea;
            this.columna = columna;
            this.salto = salto;
        }

        public object ejecutarOptimizacion()
        {
            return "";
        }

        public bool OperandosNumericos()
        {
            if (izq is NumeroOp && derecha is NumeroOp) return true;
            return false;
        }

        public bool evaluarCondicion()
        {
            double valorIzq = Double.Parse(izq.getValor());
            double valorDer = Double.Parse(derecha.getValor());

            switch (comparacion)
            {
                case tipoComparacion.diferente:
                    return valorIzq != valorDer;

                case tipoComparacion.igual:
                    return valorIzq == valorDer;

                case tipoComparacion.mayor:
                    return valorIzq > valorDer;

                case tipoComparacion.mayorque:
                    return valorIzq >= valorDer;

                case tipoComparacion.menor:
                    return valorIzq < valorDer;

                case tipoComparacion.menorque:
                    return valorIzq <= valorDer;

                default:
                    return false;
            }

        }

        public tipoComparacion operacionInversa()
        {
            switch (comparacion)
            {
                case tipoComparacion.igual:
                    return tipoComparacion.diferente;

                case tipoComparacion.diferente:
                    return tipoComparacion.igual;

                case tipoComparacion.menor:
                    return tipoComparacion.mayorque;

                case tipoComparacion.menorque:
                    return tipoComparacion.mayor;

                case tipoComparacion.mayor:
                    return tipoComparacion.menorque;

                case tipoComparacion.mayorque:
                    return tipoComparacion.menor;

                default:
                    return tipoComparacion.diferente;
            }
        }

        public string pedirCodigo()
        {
            return $"if ( {izq.getValor()} {convertirOperador()} {derecha.getValor()} ) goto {salto.nombre};\n";
        }

        public string convertirOperador()
        {

            switch (comparacion)
            {
                case tipoComparacion.igual:return "==";
                case tipoComparacion.diferente:return "!=";
                case tipoComparacion.menor:return "<";
                case tipoComparacion.menorque:return "<=";
                case tipoComparacion.mayor:return ">";
                case tipoComparacion.mayorque:return ">=";
                default:return "==";
            }
        }
    }
}
