using CompiPascal.AST_;
using CompiPascal.AST_.cambioFlujo;
using CompiPascal.AST_.definicion;
using CompiPascal.AST_.definicion.arrego;
using CompiPascal.AST_.interfaces;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.entorno_.simbolos
{
    public class Funcion : Simbolo, Instruccion
    {


        public int tamanoPadre { get; set; }


        public int tamaFuncion { get; set; }

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

            Entorno nuevo = new Entorno(ent,"Funcion_"+this.Identificador);
            string codigoFuncion = "";

            codigoFuncion += $"void {this.Identificador} () "+"{";
            agregarParametros(nuevo);

            foreach (Instruccion item in ENCABEZADOS)
            {
                if(item is Declaracion)
                {
                    string declaraVar = item.getC3(nuevo);
                    codigoFuncion += Generador.tabular(declaraVar);
                }
            }

            codigoFuncion += agregarRetorno(nuevo);

            foreach (Instruccion item in instrucciones)
            {

                string codigo = item.getC3(nuevo);
                codigoFuncion += Generador.tabular(codigo);

            }

            codigoFuncion += Generador.tabularLinea("return;", 1);
            codigoFuncion += "}";

            tamaFuncion = ent.tamano;

            return codigoFuncion;
        }


        public void agregarParametros(Entorno ent)
        {

            int posRelativa = 0;


            if (ListaParametros != null)
            {
                foreach (Simbolo item in ListaParametros)
                {
                    Simbolo parametro = new Simbolo(item.Tipo, item.Identificador, false, 1, posRelativa, item.linea, item.columna);
                    ent.agregarSimbolo(item.Identificador, parametro);
                    ent.tamano++;
                }
            }

        }

        public string agregarRetorno(Entorno ent)
        {

            if (Tipo != TipoDatos.Void)
            {
                LinkedList<Simbolo> param = new LinkedList<Simbolo>();
                param.AddLast(new Simbolo(Identificador, linea, columna));
                Declaracion retornoPascal = new Declaracion(param, Tipo);
                string codigo = retornoPascal.getC3(ent);
                return codigo;
            
            }

            return "";
        }

    } 
}
