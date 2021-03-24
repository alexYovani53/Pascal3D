using CompiPascal.AST_.interfaces;
using CompiPascal.AST_.valoreImplicito;
using CompiPascal.entorno_;
using CompiPascal.entorno_.simbolos;
using Pascal3D;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.definicion
{
    public class Asignacion : Instruccion
    {


        /**
         * @propiedad       bool        esObjeto
         * @comentario      esta bandera dira si la asignación es hacia una variable o a la propiedad 
         *                  de un objeto 
         */
        private bool esobjeto;


        /**
         * @propiedad       string      idObjeto    
         * @comentario      Este almacenara el nombre del objeto al cual se decea acceder (en su momento)
         */
        private string idObjeto;


        /**
         * @propiedad       string      propiedad
         * @comentario      Este almacenara el nombre de una propiedad a la cual se desea acceder
         */
        private LinkedList<string> oropiedades;

        /**
         * @propiedad       variable
         * @comentario      Este almacenara el simbolo al cual se le va a asignar el valor
         */
        private Simbolo variable_;

        /**
         * @propiedad       valor
         * @comentario      es la expresión que contiene el valor a asignar a la variable dentro 
         *                  de la tabla de simbolos
         */
        private Expresion valor { get; set; }



        public int linea { get; set; }
        public int columna { get; set; }

        public Asignacion(Simbolo variable, Expresion valor, bool objeto, int linea, int columna)
        {
            this.variable = variable;
            this.valor = valor;
            this.esobjeto = objeto;
            this.linea = linea;
            this.columna = columna;

        }

        public Asignacion(string idObjeto, LinkedList<string> idPropiedad , Expresion valor, int linea, int columna)
        {
            this.idObjeto = idObjeto;
            this.oropiedades = idPropiedad;
            this.valor = valor;
            this.esobjeto = true;
            this.linea = linea;
            this.columna = columna;

        }

   


        public Simbolo variable
        {
            get
            {
                return variable_;
            }
            set
            {
                variable_ = value;
            }
        }




        public string getC3()
        {

            result3D final =   valor.obtener3D(null);
            Program.getIntefaz().agregarTexto(final.Codigo);
            return "";
        }
    }
}
