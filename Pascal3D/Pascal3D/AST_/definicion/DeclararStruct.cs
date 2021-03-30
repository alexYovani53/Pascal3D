using CompiPascal.AST_.definicion.arrego;
using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using CompiPascal.entorno_.simbolos;
using System;
using System.Collections.Generic;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.definicion
{
    public class DeclararStruct : Instruccion
    {


        public int tamanoPadre { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }


        /**
         * @propiedad       variables   
         * @comentario      Lista de las variables a declarar
         */
        private LinkedList<Simbolo> variables { get; set; }

        /* @propiedad       string      structuraNombre
         * @comentario      esta propiedad guarda el identificador con el que se ha guardado la definicion de un struct
         */
        private string structuraNombre { get; set; }


        public DeclararStruct(LinkedList<Simbolo> variables, string structuraNombre, int linea, int columna)
        {
            this.variables = variables;
            this.structuraNombre = structuraNombre;
            this.linea = linea;
            this.columna = columna;
        }



        public LinkedList<Simbolo> varTipoStruct()
        {
            return variables;
        }

        public string getC3(Entorno ent)
        {
            throw new NotImplementedException();
        }
    }
}
