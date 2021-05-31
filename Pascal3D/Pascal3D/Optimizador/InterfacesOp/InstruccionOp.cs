using System;
using System.Collections.Generic;
using System.Text;

namespace Pascal3D.Optimizador.InterfacesOp
{
    public interface InstruccionOp
    {
        public int linea { get; set; }

        public int columna { get; set; }
 
        public string pedirCodigo();

        public object ejecutarOptimizacion();

    }


}
