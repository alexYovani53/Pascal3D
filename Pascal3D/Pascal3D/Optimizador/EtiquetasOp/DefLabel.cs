using Pascal3D.Optimizador.InterfacesOp;
using Pascal3D.Optimizador.PrimitivosOp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pascal3D.Optimizador.EtiquetasOp
{
    public class DefLabel : InstruccionOp
    {
        public int linea { get; set; }
        public int columna { get; set; }

        public Label etiqueta { get; set; }

        public DefLabel(Label etiqueta,int linea, int columna )
        {
            this.etiqueta = etiqueta;
            this.linea = linea;
            this.columna = columna;
        }

        public object ejecutarOptimizacion()
        {
            return "";
        }

        public string pedirCodigo()
        {
            return $"{etiqueta.nombre}:\n";
        }
    }
}
