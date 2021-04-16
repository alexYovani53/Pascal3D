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

            //Etiqueta de inicio para el ciclo 
            string etiquetaInicio = Generador.pedirEtiqueta();        //ETIQUETA DE INICIO

            /* DEFINIMOS LAS ETIQUETAS FALSA Y VERDADERA DE LA CONDICIÓN, ESTO GUIARA PARA DONDE DEBER SALTAR 
             * DEPENDIENDO DEL RESULTADO DEL CODIGO 3D*/ 
            ((Operacion)exprCondicional).etiquetaFalsa = Generador.pedirEtiqueta();
            ((Operacion)exprCondicional).etiquetaVerdadera = Generador.pedirEtiqueta();

            /* RECUPERAMOS EL CODIGO 3D DE LA EXPRESIÓN */
            result3D result = exprCondicional.obtener3D(ent);

            /* CODIGO DEL WHILE */
            string whileCadena = "/************************************************************ INICIO CICLO WHILE **************************/\n\n";
            whileCadena += etiquetaInicio + ": \n";
            whileCadena += result.Codigo;
            whileCadena += result.EtiquetaV +":\n";

            foreach (Instruccion item in instrucciones)
            {
                whileCadena += Generador.tabular(item.getC3(ent));
            }

            /* SALTO Y ETIQUETA DE SALIDA*/
            whileCadena += "goto " + etiquetaInicio + ";\n";
            whileCadena += result.EtiquetaF+ ":\n";
            whileCadena += "/************************************************************ FIN CICLO WHILE **************************/\n\n";

            whileCadena.Replace("#RETORNAR#", $"goto {result.EtiquetaF};");

            whileCadena = whileCadena.Replace("#BREAK#", $"goto {result.EtiquetaF};");
            whileCadena = whileCadena.Replace("#CONTINUE#", $"goto {etiquetaInicio};");

            return whileCadena;

        }
    }
}
