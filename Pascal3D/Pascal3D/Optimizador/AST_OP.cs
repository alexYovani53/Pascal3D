using Pascal3D.Optimizador.Funcion;
using Pascal3D.Optimizador.InterfacesOp;
using Pascal3D.Optimizador.ValorImplicitoOp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pascal3D.Optimizador
{
    public class AST_OP
    {

        public EncabezadoOp encabezado_ = null;
        public LinkedList<Temp_SP_HP> temporales = null;
        public LinkedList<FuncionOp> Funciones = null;

        public AST_OP(EncabezadoOp instrucciones,LinkedList<Temp_SP_HP> temporales,LinkedList<FuncionOp> Funs)
        {
            this.encabezado_ = instrucciones;
            this.temporales = temporales;
            this.Funciones = Funs;
        }


    }
}
