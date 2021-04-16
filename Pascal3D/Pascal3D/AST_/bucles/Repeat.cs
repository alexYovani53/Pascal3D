using CompiPascal.AST_.cambioFlujo;
using CompiPascal.AST_.interfaces;
using CompiPascal.AST_.valoreImplicito;
using CompiPascal.entorno_;
using Pascal3D;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.bucles
{
    public class Repeat : Condicional, Instruccion
    {

        public int tamanoPadre { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }
        public Repeat(Expresion condicional, LinkedList<Instruccion> instrucciones, int linea, int columna):
            base(condicional,instrucciones)
        {
            this.linea = linea;
            this.columna = columna;
        }

        public string getC3(Entorno ent) 
        {
            
            string repeatInicio = Generador.pedirEtiqueta();


            string codigo = "/************************************************************ INICIO CICLO REPEAT - UNTIL **************************/ \n";
            //SE CONCATENAN LAS INSTRUCCIONES DEL REPEAT

            codigo += repeatInicio + ":\n";
            codigo += " /* codigo de instrucciones en repeat*/ \n";

            foreach (Instruccion item in instrucciones)
            {
                codigo += Generador.tabular(item.getC3(ent));
            }

            /* LE ASIGNAMOS LAS ETIQUETAS, YA QUE LA CONDICION ES UNA EXPRESIÓN BOOLEANA
             * Y EN EL CASO QUE SEA UNA EXPRESIÓN COMPUESTA, LAS ETIQUETAS 
             * VERDADERAS Y FALSAS SE DEBEN HABER GENERADO ANTES DE COMENZAR CON LA TRADUCCIÓN DE ESTA EXPRESION
             */

            ((Expresion)exprCondicional).etiquetaFalsa = repeatInicio;
            ((Expresion)exprCondicional).etiquetaVerdadera = Generador.pedirEtiqueta(); 

            /* GENERAMOS EL CODIGO DE LA EXPRESIÓN CONDICION DEL REPEAT*/
            result3D resultadoPrueba = exprCondicional.obtener3D(ent);

            /* COPIAMOS EL CODIGO Y LA ETIQUETA DE SALIDA PARA LA EXPRESIÓN */
            codigo += resultadoPrueba.Codigo;  

            codigo += resultadoPrueba.EtiquetaV + ": \n";
            codigo += "/************************************************************ FIN CICLO REPEAT - UNTIL **************************/ \n\n";


            codigo = codigo.Replace("#BREAK#", $"goto {resultadoPrueba.EtiquetaF};");
            codigo = codigo.Replace("#CONTINUE#", $"goto {repeatInicio};");

            return codigo;
        }
    }
}
