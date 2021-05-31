using Pascal3D.Optimizador.InterfacesOp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pascal3D.Optimizador
{
    public class EncabezadoOp : InstruccionOp
    {
        public int linea { get; set; }
        public int columna { get; set; }
        public string valor { get; set; }


        public EncabezadoOp(string encabezado)
        {
            this.valor = encabezado;
        }

        public object ejecutarOptimizacion()
        {
            return "";
        }
        public string pedirCodigo()
        {
            return valor;
        }
    }
}
