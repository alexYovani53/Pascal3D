using CompiPascal.AST_;
using CompiPascal.AST_.definicion.arrego;
using CompiPascal.AST_.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.entorno_
{

    /**
     *@comentario           La clase simbolo es un nodo de la tabla de simbolos
     *
     *          Estos simbolos pueden ser. Un identificador, valor y tipo
     *     
     */

    public class Simbolo
    {

        /**
         * @enum            TipoDatos 
         * @comentario      esta enumeración representa los tipos de datos que tendra el lenguaje
         */

        public enum TipoDatos{
            String,
            Integer,
            Char,
            Real,
            Boolean,
            Struct,
            Object,
            Void,
            Array,
            NULL
        }

        public int linea { get; set; }

        public int columna { get; set; }


        /*
         * @param           porReferencia
         * @comentario      para las funciones y o procedimientos, se valida si es un parametro por referencia o por valor
         */
        public bool porReferencia { get; set; }


        /*
         * @param           identificador
         * @comentario      contendra el nombre del simbolo creado
         */
        public string Identificador { get; set; }


        /*
         * @param           valor
         * @comentario      contendra el valor del identificador, primitivo u objeto
         */
        public object Valor { get; set; }


        /*
         * @param           Tipo
         * @comentario      contendra el tipo del simbolo: Integer, String, real, ...
         */
        public TipoDatos Tipo { get; set; }


        /*
         * @param           ListaParametros
         * @comentario      contendra la lista de parametros para una funcion
         */
        public LinkedList<Simbolo> ListaParametros { get; set; }

        public valorRef punteroRef { get; set; }

        /*
         * @param           esFuncion
         * @comentario      true si es funcion y false si no 
         */
        private bool esFuncion { get; set; }

        /*
         * @param           Struct
         * @comentario      almacena la definicion de un struct 
         */
        public  Struct estructura { get; set; }

        /*
         * @param           arreglo
         * @comentario      almacena la definicion de un struct 
         */
        public Arreglo arregloStruct { get; set; }

        /*
         * @param           Consante
         * @comentario      contendra el valor true o false si el simbolo es una constante o no 
         */
        public bool Constante { get; set; }


        /*
         * @param           structGenerador
         * @comentario      guardara el nombre de la estructura con la que se instanciara el valor
         */
        public string structGenerador { get; set; }

        /**   
         * @comentario  Constructor que se utiliza solo para guardar un ide en un simbolo
         * @param       identificador   Identificador del Simbolo.
         */

        public Simbolo(string Identificador,int linea, int columna)
        {
            this.Identificador = Identificador;
            this.esFuncion = false;
            this.Constante = false;
            this.linea = linea;
            this.columna = columna;
        }



        /* 
         * @comentario              Constructor que se utiliza para definicion de estructuras (declarar un objeto)
         *                          y procedimientos
         * @param   identificador   Identificador del Simbolo.
         */

        public Simbolo(TipoDatos Tipo, string Identificador, int linea , int columans)
        {
            this.Tipo = Tipo;
            this.Identificador = Identificador;
            esFuncion = false;
            Constante = false;
            this.linea = linea;
            this.columna = columna;
        }

        /* 
         * @comentario              Constructor que se utiliza para definicion de parametros en funciones y procedimientos
         * @param   identificador   Identificador del Simbolo.
         */

        public Simbolo(TipoDatos Tipo, string Identificador,bool porReferencia, int linea, int columans)
        {
            this.Tipo = Tipo;
            this.Identificador = Identificador;
            this.porReferencia = porReferencia;
            esFuncion = false;
            Constante = false;
            this.linea = linea;
            this.columna = columna;
        }


        /* 
         * @comentario              Constructor que se utiliza para definicion de parametros en funciones
         *                          y procedimientos, del tipo objeto. EN LA CONSTRUCCION DEL AST
         * @param   identificador   Identificador del Simbolo.
         */

        public Simbolo(string Generador, string Identificador, bool porReferencia, int linea, int columans)
        {
            this.Tipo = TipoDatos.Object;
            this.Identificador = Identificador;
            this.porReferencia = porReferencia;
            this.structGenerador = Generador;
            this.linea = linea;
            this.columna = columna;
        }

        /* 
         * @comentario  Constructor que se utiliza pripalmente para inicializacion de 
         *              parámetros en las llamadas a funciones y procedimientos.
         *              
         * @param   tipo            Tipo específico de la variable, integer, real,etc.
         * @param   identificador   nombre del Simbolo.
         * @param   valor           El valor específico de la variable, ya sea que se
         *                          trate de un valor primitivo o de un objeto.
         */

        public Simbolo(TipoDatos Tipo, string Identificador, object Valor, int linea, int columans)
        {
            this.Tipo = Tipo;
            this.Identificador = Identificador;
            this.Valor = Valor;
            esFuncion = false;
            Constante = false;
            this.linea = linea;
            this.columna = columna;
        }



        /* 
         * @comentario  Constructor que se utiliza pripalmente para inicializacion de 
         *              parámetros en las llamadas a funciones y procedimientos, por referencia o no
         *              
         * @param   tipo            Tipo específico de la variable, integer, real,etc.
         * @param   identificador   nombre del Simbolo.
         * @param   valor           El valor específico de la variable, ya sea que se
         *                          trate de un valor primitivo o de un objeto.
         * @param   valorRef        Esta estructura guarda la definicion de referencia al parametro que se guardo en una funcion
         */

        public Simbolo(TipoDatos Tipo, string Identificador, object Valor, valorRef referencia, int linea, int columans)
        {
            this.Tipo = Tipo;
            this.Identificador = Identificador;
            this.Valor = Valor;
            this.porReferencia = true;
            this.punteroRef = referencia;
            this.linea = linea;
            this.columna = columna;
        }

                
        /* @comentario   Constructor que se utiliza para definición de funciones. UNA FUNCION ES UN SIMBOLO YA QUE 
         *               PUEDE SER LLAMADO MUCHAS VECES
        *
        * @param   Identificador   Nombre para el Simbolo.
        * @param   Tipo            Tipo de retorno de la funcion: Real, integer, string....
        * @param   ListaParametros La lista parametros de la funcion.
        */

        public Simbolo(TipoDatos Tipo, string Identificador,  LinkedList<Simbolo> ListaParametros, int linea, int columans)
        {
            this.Identificador = Identificador;
            this.Tipo = Tipo;
            this.ListaParametros = ListaParametros;
            esFuncion = true;
            Constante = false;
            this.linea = linea;
            this.columna = columna;
        }



        /* @comentario   Constructor que se utiliza para la declaracion de estrucs.
        *
        * @param   Identificador   Nombre para el Simbolo struct
        * @param   Tipo            Tipo de retorno de la funcion: Real, integer, string....
        * @param   ListaParametros La lista parametros de la funcion.
        */

        public Simbolo(string Identificador, TipoDatos Tipo, Struct estructura, int linea, int columans)
        {
            this.Identificador = Identificador;
            this.Tipo = Tipo;
            this.estructura = estructura;
            esFuncion = false;
            Constante = false;
            this.linea = linea;
            this.columna = columna;
        }


       /* @comentario   Constructor que se utiliza para la declaracion de estrucs arreglos.
        *
        * @param   Identificador   Nombre para el Simbolo array
        * @param   nombreTipoOb    Nombre del tipo de dato objeto (este puede o no venir)
        * @param   Tipo            Tipo para cada posicion del objeto..
        * @param   arr             es la estructura arreglo que contiene las definicioes de los limites e indices de inicio.
        */

        public Simbolo(string Identificador, TipoDatos Tipo, Arreglo arr, int linea, int columans)
        {
            this.Identificador = Identificador;
            this.Tipo = Tipo;
            arregloStruct = arr;
            esFuncion = false;
            Constante = false;
            this.linea = linea;
            this.columna = columna;
        }

    }
}
