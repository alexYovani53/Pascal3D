using Pascal3D.Optimizador.InterfacesOp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pascal3D.Optimizador.SaltosOp
{
    public class GotoOp : InstruccionOp
    {
        public int linea { get; set; }
        public int columna { get ; set ; }

        public string etiquetaSalto { get; set; }

        public GotoOp(string etiqueta,int linea, int columna)
        {
            this.etiquetaSalto = etiqueta;
            this.linea = linea;
            this.columna = columna;
        }

        public string pedirCodigo()
        {
            return $"goto {etiquetaSalto};\n";
        }

        public object ejecutarOptimizacion()
        {
            return "";
        }
    }
}
