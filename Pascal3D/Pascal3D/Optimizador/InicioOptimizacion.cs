using Pascal3D.Optimizador.Funcion;
using Pascal3D.Optimizador.InterfacesOp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pascal3D.Optimizador
{
    public class InicioOptimizacion
    {
        AST_OP arbolOptimizar = null;
        public InicioOptimizacion(AST_OP ARBOL)
        {
            this.arbolOptimizar = ARBOL;
        }

        public void optimizar()
        {
            foreach (FuncionOp item in arbolOptimizar.Funciones)
            {

            }
        }



    }
}
