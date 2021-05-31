using Pascal3D.Optimizador.InterfacesOp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pascal3D.Optimizador.Funcion
{
    public class LlamadaOp:InstruccionOp
    {

        public int linea { get; set; }
        public int columna { get; set; }

        public string llamada { get; set; }
        public LlamadaOp(string llamada, int linea, int columna)
        {
            this.linea = linea;
            this.columna = columna;
            this.llamada = llamada;
        }


        public string pedirCodigo()
        {
            return $"{llamada}();\n";
        }

        public object ejecutarOptimizacion()
        {
            return "";
        }
    }
}
