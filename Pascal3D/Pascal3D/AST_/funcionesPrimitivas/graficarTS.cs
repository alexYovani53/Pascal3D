using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using CompiPascal.entorno_.simbolos;
using Pascal3D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CompiPascal.AST_.funcionesPrimitivas
{
    public class graficarTS : Instruccion
    {


        public int tamanoPadre { get; set; }
        public int linea { get; set ; }
        public int columna { get; set; }



        public graficarTS(int fila, int columna)
        {
            this.linea = fila;
            this.columna = columna;
        }


        public string getC3(Entorno ent, AST arbol)
        {
            LinkedList<reporteVar> vars = new LinkedList<reporteVar>();


            Entorno actual = ent;
            while (actual.entAnterior() != null)
            {
                actual = actual.entAnterior();
            }
            agregarVariables(vars, actual, 1);

            Program.getIntefaz().agregarLocales(vars);

            return "";
        }

        private void agregarVariables(LinkedList<reporteVar> vars, Entorno entActual, int nivel)
        {

            foreach (Simbolo item in entActual.TablaSimbolos())
            {

                if (item.Tipo != Simbolo.TipoDatos.Array && item.Tipo != Simbolo.TipoDatos.Object && item.Tipo != Simbolo.TipoDatos.NULL &&
                    item.Tipo != Simbolo.TipoDatos.Struct)
                {
                    vars.AddLast(new reporteVar(item.Identificador, item.Tipo.ToString(), entActual.nombre, item.linea.ToString(), item.columna.ToString(), nivel.ToString()));
                }

                if (item is Funcion)
                {

                    vars.AddLast(new reporteVar(item.Identificador, "FUNCION", entActual.nombre, item.linea.ToString(), item.columna.ToString(), nivel.ToString()));
                }
                else if (item is Objeto)
                {
                    vars.AddLast(new reporteVar(item.Identificador, "OBJETO", entActual.nombre, item.linea.ToString(), item.columna.ToString(), nivel.ToString()));
                }
                else if (item is ObjetoArray)
                {
                    vars.AddLast(new reporteVar(item.Identificador, "OBJETO-Array", entActual.nombre, item.linea.ToString(), item.columna.ToString(), nivel.ToString()));
                }



            }

            foreach (Entorno item in entActual.getHijos())
            {
                agregarVariables(vars, item, nivel + 1);
            }


        }


        public void obtenerListasAnidadas(LinkedList<string> variablesUsadas)
        {

        }
    }
}
