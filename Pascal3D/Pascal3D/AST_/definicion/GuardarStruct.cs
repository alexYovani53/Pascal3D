using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using Pascal3D;
using System;
using System.Collections.Generic;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.definicion
{
    public class GuardarStruct : Instruccion
    {
        public int tamanoPadre { get; set; }
        public int linea { get ; set ; }
        public int columna { get ; set ; }

        private string identificador { get; set; }

        private LinkedList<Instruccion> instruccionesDef { get; set; }


        public GuardarStruct(string identificador, LinkedList<Instruccion> instruccionesDef, int linea, int columna)
        {
            this.identificador = identificador;
            this.instruccionesDef = instruccionesDef;
            this.linea = linea;
            this.columna = columna;
        }

        public string getC3(Entorno ent, AST arbol)
        {

            bool existe = ent.existeSimbolo(identificador);

            if (existe)
            {
                Program.getIntefaz().agregarError("El identificador" + identificador + " ya tiene una definicion en el entorno actual", linea, columna);
                return null;
            }

            Struct estructuraNuev = new Struct(identificador, instruccionesDef);
            arbol.agregarEstructura(estructuraNuev);

            return "";
        }

        public void obtenerListasAnidadas(LinkedList<string> variablesUsadas)
        {

        }
    }
}
