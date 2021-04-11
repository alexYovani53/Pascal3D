using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using Pascal3D;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.definicion
{

    /**
     * @class        definicion
     * @comentario   Clase que representa la definicion de una variable, la cual puede estar inicializada.
     *          
     * La variable que sera declarada y posiblemente inicializada con la expresion puede ser de tipo 
     * primitivo un entero, caracter, objeto etc. 
     */


    public class Declaracion : Instruccion
    {


        public bool declaraParametro { get; set; }

        public string TemporalCambioEntorno { get; set; }

        public int tamanoPadre { get; set; }
        /**
         * @propiedad       valorInicializacion   
         * @comentario      este representa la expresion con valor incial para la declaracion
         */
        private Expresion valorInicializacion { get; set; }

        /**
         * @propiedad       tipo_implicito   
         * @comentario      guarda el valor implicito de la declaracion
         */
        private TipoDatos tipo_variables { get; set; }

        /**
         * @propiedad       variables   
         * @comentario      Lista de las variables a declarar
         */
        private LinkedList<Simbolo> variables { get; set; }

        private Simbolo ideUnico { get; set; }


        public int linea { get; set; }
        public int columna { get; set; }

        public result3D valor { get; set; }

        /**
         *  @funcion        Constructor     
         *  @comentario     Declaración de variables con un valor inicial. 
         *                  El lenguaje PASCAL solo puede asignar valor inicial a una variable a la vez 
         *                  en el momento de la delcaración. por lo que "variables" solo puede tener un simbolo
         *  
         *  @parametro      tipo            tipo implicito para los simbolos
         *  @parametro      variables       lista de los simbolos a declarar
         *  @parametro      inicializado    valor con el que se guardaran los simbolos
         */

        public Declaracion( LinkedList<Simbolo> variables, TipoDatos tipo, Expresion inicializador)
        {

            //inicialmente se le pasa el Tipo de dato  (ose la palabra string, integer ...)
            //a cada una de las variables de la lista
            //luego se almacenan los datos

            foreach (Simbolo variable in variables)
            {
                variable.Tipo = tipo;
            }
            this.tipo_variables = tipo;
            this.variables = variables;
            this.valorInicializacion = inicializador;
            this.declaraParametro = false;
        }

        /**
         *  @funcion        Constructor     
         *  @comentario     Declaración de variables sin valor inicial
         *                  Aca si se puede declarar una lista de variables de una sola vez.
         *                  por lo que se recibe una lista de variables 
         *                  
         *  @parametro      tipo            tipo implicito para los simbolos
         *  @parametro      variables       lista de los simbolos a declarar
         *  @parametro      inicializado    valor con el que se guardaran los simbolos
         */
        public Declaracion( LinkedList<Simbolo> variables, TipoDatos tipo)
        {
            //inicialmente se le pasa el Tipo de dato implicito (ose la palabra string, integer ...)
            //a cada una de las variables de la lista
            //luego se almacenan los datos

            foreach (Simbolo variable in variables)
            {
                variable.Tipo = tipo;
            }
            this.tipo_variables = tipo;
            this.variables = variables;
            this.valorInicializacion = null;
            this.declaraParametro = false;

        }

        /**
         *  @funcion        Constructor     
         *  @comentario     Declaración de constantes con valor inicial
         *                  
         *  @parametro      variable        simbolo de la constante
         *  @parametro      inicial         valor con el que se guardara la constante
         */
        public Declaracion(Simbolo variable_,Expresion inicial)
        {
            //inicialmente se le pasa el Tipo de dato implicito (ose la palabra string, integer ...)
            //a cada una de las variables de la lista
            //luego se almacenan los datos

            this.ideUnico = variable_;
            this.valorInicializacion = inicial;
            this.declaraParametro = false;

        }

        /*  ESTE METODO SE HIZO PARA LA DECLARACIÓN DE PARAMETROS EN FUNCIONES */
        public Declaracion(Simbolo variable,result3D valor)
        {
            this.variables = new LinkedList<Simbolo>();
            this.variables.AddLast(variable);
            this.valor = valor;
            this.tipo_variables = variable.Tipo; 
            this.declaraParametro = true;
        }



        private object valorDefecto(TipoDatos tipo)
        {

            if (tipo == TipoDatos.String)
            {
                return "";
            }
            else if (tipo == TipoDatos.Char)
            {
                return 0;
            }
            else if (tipo == TipoDatos.Integer)
            {
                return 0;
            }
            else if (tipo == TipoDatos.Real)
            {
                return 0.0;
            }
            else if (tipo == TipoDatos.Boolean)
            {
                return 0;
            }
            else if (tipo == TipoDatos.Void)
            {
                return "";
            }

            return "";

        }


        public LinkedList<Simbolo> varNoramles()
        {
            return variables;
        }

        public string getC3(Entorno ent)
        {
            string puntero_SP_TEMPORAL = "SP";
            if (declaraParametro)
            {
                /* CUANDO LA DECLARACIÓN ES UN PARAMETRO DE UNA FUNCIÓN, ESTA VARIABLE GUARDA LA ETIQUETA
                 * QUE CONTIENE EL VALOR DEL NUEVO ENTORNO DONDE SE REQUIERE DECLARAR LOS PARAMETROS */
                puntero_SP_TEMPORAL = TemporalCambioEntorno;
            }


            //DECLARACION DE CONSTANTES

            if (ideUnico != null)
            {

                result3D valAsignacion = valorInicializacion.obtener3D(ent);
                TipoDatos tipo = valAsignacion.TipoResultado;

                string nombre = ideUnico.Identificador;

                //ERROR PORQUE NO SE ENCONTRO EL SIMBOLO
                if (ent.existeSimbolo(nombre))
                {
                    Program.getIntefaz().agregarError("El simobolo a declarar ya existe en el entorno actual", linea, columna);
                    return "";
                }

                string declaracionConstante = "";
                int posicionRelativa = ent.tamano;             //CUANDO SE DECLARA, LA POSICION RELATIVA ES JUSTAMENTE EL TAMAÑO DEL ENTORNO ACTUAL

                declaracionConstante += valAsignacion.Codigo;

                string temporalConst = Generador.pedirTemporal();

                declaracionConstante += $"/*Declaracion de la constante {ideUnico.Identificador}*/\n";
                declaracionConstante += $"{temporalConst}= {puntero_SP_TEMPORAL} + {ent.tamano}; \n";
                declaracionConstante += $"Stack[(int){temporalConst}] = {valAsignacion.Temporal}; \n";

                Simbolo constanteNueva = new Simbolo(tipo, ideUnico.Identificador, true, 1, posicionRelativa, ideUnico.linea, ideUnico.columna);
                ent.agregarSimbolo(ideUnico.Identificador, constanteNueva);

                ent.tamano++;

                return declaracionConstante;
            }
            


            if(esInicializado() && variables.Count > 1)
            {
                Program.getIntefaz().agregarError("Pascal solo permite la declaracion inicializada de una variable a la vez", linea, columna);
                return "";
            }

            string codigoSalida = "";
            if (!esInicializado())
            {
                object def = valorDefecto(tipo_variables);

                int posicionRelativa = ent.tamano;
                foreach (Simbolo item in variables)
                {
                    string temp = Generador.pedirTemporal();

                    codigoSalida += $"/* declaracion de variable {item.Identificador}*/\n";
                    codigoSalida += $"{temp} = {puntero_SP_TEMPORAL} + {posicionRelativa};\n";
                    codigoSalida += $"Stack[(int){temp}] = {def} ; \n";
                    
                    Simbolo simboloNuevo = new Simbolo(tipo_variables,item.Identificador, false, 1,posicionRelativa,item.linea,item.columna);
                    ent.agregarSimbolo(item.Identificador, simboloNuevo);


                    posicionRelativa ++;
                    ent.tamano++;
                }

                return codigoSalida+"\n";
            }

            else {

                /* ESTA VALIDACIÓN SE HACE YA QUE HAY UN CONSTRUCTOR QUE RECIBE UN           result3D como valor inicial  
                 * ESTE YA CONTIENE UNA ETIQUETA TEMPORAL QUE GUARDA EL VALOR (SI ES UN PRIMITIVO), DIRECCIÓN A HEAP(STRING O OBJETO) O DIRECCION
                 * AL MISMO STACK (UNA REFERENCIA)
                 */

                result3D valAsignacion;
                if (valor == null) valAsignacion = valorInicializacion.obtener3D(ent);
                else valAsignacion = valor;

                /* ESTAS VALIDACIONES SON PARA VER QUE LOS TIPOS COICIDAN, SON LAS POSIBILIDADES QUE PASCAL PERMITE */
                if (tipo_variables == TipoDatos.Integer)
                { 
                    if (valAsignacion.TipoResultado != TipoDatos.Integer && valAsignacion.TipoResultado != TipoDatos.Real)
                    {
                        Program.getIntefaz().agregarError("Error de tipos, declaracion", linea, columna);
                        return "";
                    }
                }
                else if (tipo_variables == TipoDatos.Real)
                {
                    if (valAsignacion.TipoResultado != TipoDatos.Integer && valAsignacion.TipoResultado != TipoDatos.Real)
                    {
                        Program.getIntefaz().agregarError("Error de tipos, declaracion", linea, columna);
                        return "";
                    }
                }
                else
                {
                    if(tipo_variables != valAsignacion.TipoResultado)
                    {
                        Program.getIntefaz().agregarError("Error de tipos, declaracion", linea, columna);
                        return "";
                    }

                }

                //COMO SOLO SE PUEDE INICIALIZAR UNA VARIABLE A LA VEZ (CUANDO SE DECLARA) SE CAPTURA EL UNICO ELEMENTO EN LA LISTA DE VARIABLES
                Simbolo variableUniInicializada = variables.ElementAt(0);

                codigoSalida += valAsignacion.Codigo;

                // posicionRelativa lleva el conteo de variables dentro del entorno actual
                int posicionRelativa = ent.tamano;
                string temp = Generador.pedirTemporal();

                codigoSalida += $"/* declaracion de variable {variableUniInicializada.Identificador}*/\n";
                codigoSalida += $"{temp} = {puntero_SP_TEMPORAL} + {ent.tamano}; \n";
                codigoSalida += $"Stack[(int){temp}] = {valAsignacion.Temporal};\n";
                

               
                Simbolo simboloNuevo = new Simbolo(tipo_variables, variableUniInicializada.Identificador,false, 1, posicionRelativa, variableUniInicializada.linea, variableUniInicializada.columna);

                ent.agregarSimbolo(variableUniInicializada.Identificador, simboloNuevo);

                ent.tamano++;

            }

            return codigoSalida+"\n";
        }

        public bool esInicializado()
        {
            return this.valorInicializacion != null || this.valor != null;
        }

    }


}
