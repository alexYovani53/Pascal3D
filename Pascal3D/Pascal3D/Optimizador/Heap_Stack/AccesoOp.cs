using Pascal3D.Optimizador.InterfacesOp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pascal3D.Optimizador.Heap_Stack
{
    class AccesoOp : ExpresionOp
    {
        public int linea { get; set; }
        public int columna { get; set; }
        public object valor { get; set; }
  

        public string puntero { get; set; }

        public ExpresionOp index { get; set; }

        public bool casteo { get; set; }

        public AccesoOp(string puntero, ExpresionOp index,bool casteo , int linea, int columna)
        {
            this.puntero = puntero;
            this.index = index;
            this.linea = linea;
            this.columna = columna;
            this.casteo = casteo;
        }

        public string getValor()
        {
            if (casteo)
            {
                return $"{puntero}[(int){index.getValor()}]";
            }
            else
            {
                return $"{puntero}[{index.getValor()}]";
            }
        }
    }
}
