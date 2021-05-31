using Pascal3D.Optimizador.InterfacesOp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pascal3D.Optimizador.ValorImplicitoOp
{
    public class Temp_SP_HP : ExpresionOp
    {
        public int linea { get; set; }
        public int columna { get; set; }

        public string temp_sp_hp { get; set; }

        public object valor { get; set; }

        public Temp_SP_HP(string temp, int linea, int columna)
        {
            this.temp_sp_hp = temp;
            this.linea = linea;
            this.columna = columna;
            this.valor = temp;
        }

        public string getValor()
        {
            return temp_sp_hp;
        }
    }
}
