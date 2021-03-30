using CompiPascal.AST_;
using CompiPascal.AST_.cambioFlujo;
using CompiPascal.AST_.definicion;
using CompiPascal.AST_.definicion.arrego;
using CompiPascal.AST_.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.entorno_.simbolos
{
    public class Funcion : Simbolo, Instruccion
    {


        public int tamanoPadre { get; set; }

        /*@propiedad    int     linea
         *@comentario           guardara la linea de inicio de la funcion o procedimiento*/
        public int linea { get; set; }

        /*@propiedad    int     columna
         *@comentario           guardara la columna de inicio de la funcion o procedimiento*/
        public int columna { get; set; }

        /*@propiedad    string      nombreStruct
        *@comentario                guardara el nombre del struct que debe retornar*/
        public string nombreStruct { get; set; }


        /*@propiedad        LinkedList<Instruccion>     instrucciones
         *@comentario           guardara la lista de instrucciones de la funcion o método*/
        public LinkedList<Instruccion> instrucciones { get; set; }



        /*ESTO ES PARA EL LENGUAJE PASCAL, YA QUE SOLO PERMITE DECLARACIONES EN EL ENCABEZADO DE LA FUNCION*/
        /*@propiedad        LinkedList<Instruccion>     ENCABEZADOS
        *@comentario           guardara la lista de declaraciones de la funcion*/
        LinkedList<Instruccion> ENCABEZADOS { get; set; }






        /**
         * @comentario   Constructor de funcion de tipo PRIMITIVO.

         * @param   tipo            tipo primitivo de retorno.
         * @param   nombre          Identificador de a funcion.
         * @param   parametros      La lista de parametros que definen la funcion.
         * @param   instrucciones   Lista de las instrucciones que se encuentran dentro del método.
         * @param   encabezado      Lista de instrucciones de declaracion
         */

        public Funcion(TipoDatos tipo ,string nombre, LinkedList<Simbolo> parametros,LinkedList<Instruccion> instrucciones,
                        LinkedList<Instruccion> encabezados,int linea, int columna) : base(tipo,nombre,parametros,linea, columna)
        {
            this.instrucciones = instrucciones;
            this.ENCABEZADOS = encabezados;
            this.nombreStruct = string.Empty;
        }

        /**
         * @comentario   Constructor de funcion de tipo STRUCT

         * @param   structTipo      El identificador del Struct que generara el objeto a retornar.
         * @param   nombre          Nombre de la funcion o procedimiento.
         * @param   parametros      La lista de parametros que definen la funcion.
         * @param   instrucciones   Lista de las instrucciones que se encuentran dentro del método.
         * @param   encabezado      Lista de instrucciones de declaracion
         */

        public Funcion(string structTipo, string nombre, LinkedList<Simbolo> parametros, LinkedList<Instruccion> instrucciones,
                        LinkedList<Instruccion> encabezados, int linea, int columna) : base(TipoDatos.Object, nombre, parametros,linea,columna)
        {
            this.instrucciones = instrucciones;
            this.ENCABEZADOS = encabezados;
            this.nombreStruct = structTipo;
        }


        private bool verificarTipo(TipoDatos tipo, object result)
        {
            if (tipo == TipoDatos.Integer && result is int)
            {
                return true;
            }
            if (tipo == TipoDatos.String && result is string)
            {
                return true;
            }
            else if (tipo == TipoDatos.Char && result is char)
            {
                return true;
            }
            else if (tipo == TipoDatos.Real && result is double)
            {
                return true;
            }
            else if (tipo == TipoDatos.Boolean && result is bool)
            {
                return true;
            }
            else if (tipo == TipoDatos.Object && result is Objeto)
            {
                return true;
            }
            else if (tipo == TipoDatos.Object && result is ObjetoArray)
            {
                return true;
            }
            else if (tipo==TipoDatos.Void && result is Exit)
            {
                return true;
            }
            else if (tipo == TipoDatos.Void && (TipoDatos)result == TipoDatos.Void)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public string getC3(Entorno ent)
        {
            throw new NotImplementedException();
        }
    }
}
