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

        public int linea { get; set; }
        public int columna { get; set; }
        public Repeat(Expresion condicional, LinkedList<Instruccion> instrucciones, int linea, int columna):
            base(condicional,instrucciones)
        {
            this.linea = linea;
            this.columna = columna;
        }

        public string getC3() 
        {
            
            string repeatInicio = Generador.pedirEtiqueta();


            string codigo = "";
            //SE CONCATENAN LAS INSTRUCCIONES DEL REPEAT

            codigo += repeatInicio + ":\n";
            codigo += " /* codigo de instrucciones en repeat*/ \n";


            ((Expresion)exprCondicional).etiquetaFalsa = Generador.pedirEtiqueta();
            ((Expresion)exprCondicional).etiquetaVerdadera = repeatInicio;
            result3D resultadoPrueba = exprCondicional.obtener3D(null);

            codigo += resultadoPrueba.Codigo;
            codigo += resultadoPrueba.EtiquetaF + ": \n";


            Program.getIntefaz().agregarTexto(codigo);
            return "";
        }
    }
}
