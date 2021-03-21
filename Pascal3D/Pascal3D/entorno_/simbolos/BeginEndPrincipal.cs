using CompiPascal.AST_.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.entorno_.simbolos
{


    public class BeginEndPrincipal:Funcion
    {


        /* @class               BeginEndPrincipal
         * @parametro           TipoDatos               retorno                 retorno ->en este caso sería void
         * @parametro           string                  nombre                  nombre -> sería principal o cualquiera 
         * @parametro           LinkedList<Simbolo>     parametros              parametros -> no tiene
         * @parametro           LinkedList<Instruccion> instruccion             Lista de instrucciones del metodo main ( begin end.  -> para pascal)
         *
         * @comentario          Esta clase hace referencia a la sección principal del codigo fuente, donde inicia toda la ejecución. 
         *                      es el main de pascal. 
         */

        public BeginEndPrincipal(TipoDatos retorno, string nombre, LinkedList<Simbolo> parametros, LinkedList<Instruccion> instrucciones,int linea, int columna) :
            base(retorno, nombre, parametros, instrucciones,new LinkedList<Instruccion>(),linea,columna)
        {

        }



    }
}
