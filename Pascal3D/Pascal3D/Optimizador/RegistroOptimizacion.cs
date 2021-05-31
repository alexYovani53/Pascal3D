using System;
using System.Collections.Generic;
using System.Text;

namespace Pascal3D.Optimizador
{
    public class RegistroOptimizacion
    {

        public string regla { get; set; }
        public string codigoRemplazado { get; set; }
        public string codigoNuevo { get; set; }
        public int linea { get; set; }
        public int columna {get;set;}
        public string descripcion { get; set; }

        public static LinkedList<RegistroOptimizacion> registros = new LinkedList<RegistroOptimizacion>();

        public RegistroOptimizacion(string regla, string CodogioRemplazado, string codigoNuevo,int linea, int columna, string descripcion)
        {
            this.regla = regla;
            this.codigoRemplazado = CodogioRemplazado;
            this.codigoNuevo = codigoNuevo;
            this.linea = linea;
            this.columna = columna;
            this.descripcion = descripcion;
        }

        public static void agregarOptimizacion(string regla, string CodogioRemplazado, string codigoNuevo, int linea, int columna, string descripcion)
        {
            registros.AddLast(new RegistroOptimizacion(regla, CodogioRemplazado, codigoNuevo, linea, columna, descripcion));
        }

    }
}
