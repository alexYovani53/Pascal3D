using CompiPascal.AST_.cambioFlujo;
using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.AST_.control
{
    public class Case:Instruccion
    {


        public int tamanoPadre { get; set; }
        public Expresion expresionCase { get; set; }

        LinkedList<Instruccion> instrucciones;
        public int linea { get; set; }
        public int columna { get; set; }

        public Case(Expresion caso, LinkedList<Instruccion> instrucciones,int linea, int columa)
        {
            this.expresionCase = caso;
            this.instrucciones = instrucciones;
            this.linea = linea;
            this.columna = columna;
        }

        public string getC3(Entorno ent, AST arbol)
        {


            string codigo = "";

            foreach (Instruccion item in instrucciones)
            {
                codigo += item.getC3(ent,arbol);
            }



            return Generador.tabular(codigo);
        }

        public void obtenerListasAnidadas(LinkedList<string> variablesUsadas)
        {
            foreach (Instruccion item in instrucciones)
            {
                item.obtenerListasAnidadas(variablesUsadas);
            }

            expresionCase.obtenerListasAnidadas(variablesUsadas);
        }
    }
}
