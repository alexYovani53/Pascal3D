using System;
using System.Collections.Generic;
using System.Text;

namespace Pascal3D.Optimizador.InterfacesOp
{
    public interface ExpresionOp
    {

        public int linea { get; set; }

        public int columna { get; set; }

        public object valor { get; set; }

        public string getValor();

    }
}
