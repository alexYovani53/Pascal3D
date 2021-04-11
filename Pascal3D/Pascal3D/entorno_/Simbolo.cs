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
        private string generador;
        private TipoDatos tipoParametros;

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

        public int direccion { get; set; }

        public int tamano { get; set; }
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


        public valorRef punteroRef { get; set; }

   
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

        /*
         * @param           ListaParametros
         * @comentario      contendra la lista de parametros para una funcion
         */
        public LinkedList<Simbolo> ListaParametros { get; set; }


        /**   
         * @comentario  Constructor que se utiliza solo para guardar un ide en un simbolo
         * @param       identificador   Identificador del Simbolo.
         */



        public Simbolo(string Identificador,int linea, int columna)
        {
            this.Identificador = Identificador;

            this.Constante = false;
            this.linea = linea;
            this.columna = columna;
        }

        /* 
         *  DECLARACION DE VARIABLES O CONSTANTES 
         */
        public Simbolo(TipoDatos tipo,  string Identificador,bool esConstante, int tamano, int direccion, int linea, int columna)
        {
            this.Identificador = Identificador;
            this.Tipo = tipo;
            this.Constante = esConstante;
            this.linea = linea;
            this.columna = columna;
            this.tamano = tamano;
            this.direccion = direccion;
        }



        public Simbolo(TipoDatos tipoD, string nameObjeto, int linea, int columna)
        {
            this.linea = linea;
            this.columna = columna;
        }

        public Simbolo(TipoDatos tipoD, string nambeObjeto, LinkedList<Simbolo> parametros, int linea, int columna)
        {
            this.Tipo = tipoD;
            this.Identificador = nambeObjeto;
            this.ListaParametros = parametros;
            this.linea = linea;
            this.columna = columna;
        }

        /*Parametros en funciones del tipo STRUCTU y por referencia o no */
        public Simbolo(string generador, string identificador, bool porReferencia, int linea, int columna)
        {
            this.generador = generador;
            Identificador = identificador;
            this.porReferencia = porReferencia;
            this.linea = linea;
            this.columna = columna;
        }

        /*Parametros de tipo primitivo y por referencia o no */
        public Simbolo(TipoDatos tipoParametros, string identificador, bool porReferencia, int linea, int columna)
        {
            this.Tipo = tipoParametros;
            Identificador = identificador;
            this.porReferencia = porReferencia;
            this.linea = linea;
            this.columna = columna;
        }
    }
}
