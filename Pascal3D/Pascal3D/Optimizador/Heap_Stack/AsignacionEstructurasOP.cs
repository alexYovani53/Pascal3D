using Pascal3D.Optimizador.InterfacesOp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pascal3D.Optimizador.Heap_Stack
{
    public class AsignacionEstructurasOP : InstruccionOp
    {
        public int linea { get;set; }
        public int columna { get; set; }

        public string arreglo { get; set; }

        public ExpresionOp index { get; set;}

        public bool casteo { get; set;} 

        public ExpresionOp valor { get; set; }


        public AsignacionEstructurasOP(string arreglo, ExpresionOp index, ExpresionOp valor, bool casteo, int linea, int columna)
        {
            this.arreglo = arreglo;
            this.index = index;
            this.casteo = casteo;
            this.linea = linea;
            this.columna = columna;
            this.valor = valor;
        }

        public string pedirCodigo()
        {
            if (casteo)
            {
                return $"{arreglo}[(int){index.getValor()}] = {valor.getValor()};";
            }
            else
            {
                return $"{arreglo}[{index.getValor()}] = {valor.getValor()};";
            }
        }

        public object ejecutarOptimizacion()
        {
            return "";
        }
    }
}
