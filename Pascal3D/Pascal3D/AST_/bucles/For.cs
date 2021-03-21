using CompiPascal.AST_.cambioFlujo;
using CompiPascal.AST_.definicion;
using CompiPascal.AST_.interfaces;
using CompiPascal.AST_.valoreImplicito;
using CompiPascal.entorno_;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.AST_.bucles
{
    public class For : Instruccion
    {
        /*      esto si el for fuese uno de alto nivel   for (int i = 0; i< a ; i++)
         *      
                Expresion condicionPaso;
                Instruccion actualizacion;
        */


        public int linea { get; set; }
        public int columna { get; set; }


        /* @propiedad       valorInicial
         * @comentario      Esta variable almacena una instruccion de asignacion que se ejecuta al ejecutar esta clase
         */
        Instruccion valorInicial;

        /* @propiedad       valFinal
         * @comentario      Esta variable almacena el valor final del ciclo
         */
        Expresion valfinal;

        /* @propiedad       instrucciones
         * @comentario      instrucciones que se ejecutaran dentro del ciclo for
         */
        LinkedList<Instruccion> instrucciones;


        bool aumentar { get; set; }

        
        public For(Instruccion valInicial, Expresion final,LinkedList<Instruccion> instrucciones,bool aumenta, int linea, int columna)
        {
            this.valorInicial = valInicial;
            this.valfinal = final;
            this.instrucciones = instrucciones;
            this.linea = linea;
            this.columna = columna;
            this.aumentar = aumenta;

        }

        public string getC3()
        {
            throw new NotImplementedException();
        }
    }
}
