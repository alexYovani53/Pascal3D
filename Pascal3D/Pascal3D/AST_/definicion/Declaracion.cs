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

        }

        public Declaracion(Simbolo variable,result3D valor)
        {
            this.variables = new LinkedList<Simbolo>();
            this.variables.AddLast(variable);
            this.valor = valor;
        }

        public bool esInicializado()
        {
            return this.valorInicializacion != null || this.valor != null;
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

            //DECLARACION DE CONSTANTES

            if (ideUnico != null)
            {

                result3D valAsignacion = valorInicializacion.obtener3D(ent);
                TipoDatos tipo = valAsignacion.TipoResultado;

                string nombre = ideUnico.Identificador;

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
                declaracionConstante += $"{temporalConst}= SP + {ent.tamano}; \n";
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
                    codigoSalida += temp + " = SP +" + posicionRelativa + ";\n";
                    codigoSalida += $"Stack[(int){temp}] = {def} ; \n";
                    
                    Simbolo simboloNuevo = new Simbolo(tipo_variables,item.Identificador, false, 1,posicionRelativa,item.linea,item.columna);
                    ent.agregarSimbolo(item.Identificador, simboloNuevo);


                    posicionRelativa ++;
                    ent.tamano++;
                }

                return codigoSalida+"\n";
            }

            else {

                result3D valAsignacion;
                if (valor == null) valAsignacion = valorInicializacion.obtener3D(ent);
                else valAsignacion = valor;

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

                Simbolo variableUniInicializada = variables.ElementAt(0);

                codigoSalida += valAsignacion.Codigo;

                int posicionRelativa = ent.tamano;
                string temp = Generador.pedirTemporal();

                codigoSalida += $"/* declaracion de variable {variableUniInicializada.Identificador}*/\n";
                codigoSalida += $"{temp} = SP + {ent.tamano}; \n";
                codigoSalida += $"Stack[(int){temp}] = {valAsignacion.Temporal};\n";
                
               
                Simbolo simboloNuevo = new Simbolo(tipo_variables, variableUniInicializada.Identificador,false, 1, posicionRelativa, variableUniInicializada.linea, variableUniInicializada.columna);

                ent.agregarSimbolo(variableUniInicializada.Identificador, simboloNuevo);

                ent.tamano++;

            }

            return codigoSalida+"\n";
        }

          
    }
}
