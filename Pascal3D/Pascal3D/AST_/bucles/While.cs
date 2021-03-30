using CompiPascal.AST_.cambioFlujo;
using CompiPascal.AST_.interfaces;
using CompiPascal.AST_.valoreImplicito;
using CompiPascal.entorno_;
using Pascal3D;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.AST_.bucles
{
    public class While:Condicional,Instruccion
    {

        public int tamanoPadre { get; set; }
        /*@class            while
         *@comentario       Instruccion while, la condicion e instrucion se guardan en la clase de la que hereda
         */

        public int linea { get; set; }
        public int columna { get; set; }

        public While(Expresion condicion, LinkedList<Instruccion> instrucciones , int linea, int columna):base(condicion, instrucciones)
        {
            this.linea = linea;
            this.columna = columna;

        }


        public string getC3(Entorno ent)
        {


            string etiquetaInicio = Generador.pedirEtiqueta();        //ETIQUETA DE INICIO

            ((Operacion)exprCondicional).etiquetaFalsa = Generador.pedirEtiqueta();
            ((Operacion)exprCondicional).etiquetaVerdadera = Generador.pedirEtiqueta();

            result3D result = exprCondicional.obtener3D(ent);

            string whileCadena = etiquetaInicio + ": \n";

            whileCadena += result.Codigo;
            whileCadena += result.EtiquetaV +":\n";

            whileCadena += "goto " + etiquetaInicio + ";\n";

            whileCadena += result.EtiquetaF+ ":\n";


            Program.getIntefaz().agregarTexto(whileCadena);

            return whileCadena;

        }
    }
}
