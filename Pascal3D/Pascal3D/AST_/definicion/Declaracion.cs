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

        public bool declara_EN_Objeto { get; set; }

        public bool cambiar_Ambito { get; set; }

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
            this.cambiar_Ambito = false;
            this.declara_EN_Objeto = false;
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
            this.cambiar_Ambito = false;
            this.declara_EN_Objeto = false;

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
            this.cambiar_Ambito = false;
            this.declara_EN_Objeto = false;

        }

        /*  ESTE METODO SE HIZO PARA LA DECLARACIÓN DE PARAMETROS EN FUNCIONES */
        public Declaracion(Simbolo variable,result3D valor)
        {
            this.variables = new LinkedList<Simbolo>();
            this.variables.AddLast(variable);
            this.valor = valor;
            this.tipo_variables = variable.Tipo; 
            this.cambiar_Ambito = true;
            this.declara_EN_Objeto = false;
        }



        private result3D valorDefecto(TipoDatos tipo)
        {
            result3D codigoDef = new result3D();

            if (tipo == TipoDatos.String)
            {
                string temp1 = Generador.pedirTemporal();

                codigoDef.Codigo += $"{temp1}= HP; \n";
                codigoDef.Codigo += $"Heap[(int){temp1}] = 0; \n";
                codigoDef.Codigo += $"HP = HP + 1; \n";

                codigoDef.Temporal = temp1;
                codigoDef.TipoResultado = TipoDatos.String;
                return codigoDef;
            }
            else if (tipo == TipoDatos.Char)
            {
                codigoDef.Temporal = ""+0;
                return codigoDef;
            }
            else if (tipo == TipoDatos.Integer)
            {
                codigoDef.Temporal = "" + 0;
                return codigoDef;
            }
            else if (tipo == TipoDatos.Real)
            {
                codigoDef.Temporal = "" + 0.0;
                return codigoDef;
            }
            else if (tipo == TipoDatos.Boolean)
            {
                codigoDef.Temporal = "" + 0;
                return codigoDef;
            }
            else if (tipo == TipoDatos.Void)
            {
                return codigoDef;
            }

            return codigoDef;

        }


        public LinkedList<Simbolo> varNoramles()
        {
            return variables;
        }

        public string getC3(Entorno ent, AST arbol)
        {
            string puntero_Ambito = "SP";
            string stack_heap = "Stack";

            if (cambiar_Ambito)
            {
                /* *****Cuando la Declaración es un PARAMETRO de una FUNCION, --> TemporalCambioEntorno <-- guarda
                 *      el temporal 
                 *      que contiene el valor del nuevo entorno donde se requiere declarar los parametros
                 *
                 * *****Al igual que cuando se declaran parametros de un STRUCT, estos se guardan en un nuevo entorno 
                 *      El cual se encuentra en EL HEAP y no en EL STACK, por lo que el apuntador al HEAP se pasa 
                 *      en la declaración en la propiedad  ->TemporalCambioEntorno <--- que ya lleva la referencia al 
                 *      HEAP y solo hace falta usarla. 
                 */
                puntero_Ambito = TemporalCambioEntorno;
            }
            if (declara_EN_Objeto)
            {
                /*      declara_EN_Objeto se modifica en la clase  @DeclaraStruct.cs
                 */

                /*      Esto se hace por la razón de que cuando se hace una instancia de un STRUCT, este contiene
                 *      propiedades que se almacenan en el HEAP y no en el Stack
                 *
                 */
                stack_heap = "Heap";

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
                declaracionConstante += $"{temporalConst}= {puntero_Ambito} + {ent.tamano}; \n";
                declaracionConstante += $"{stack_heap}[(int){temporalConst}] = {valAsignacion.Temporal}; \n";

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
                result3D def = valorDefecto(tipo_variables);

                int posicionRelativa = ent.tamano;
                foreach (Simbolo item in variables)
                {
                    string temp = Generador.pedirTemporal();

                    codigoSalida += $"/* declaracion de variable {item.Identificador}*/\n";
                    codigoSalida += def.Codigo;
                    codigoSalida += $"{temp} = {puntero_Ambito} + {posicionRelativa};\n";
                    codigoSalida += $"{stack_heap}[(int){temp}] = {def.Temporal} ; \n";
                    
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
                codigoSalida += $"{temp} = {puntero_Ambito} + {ent.tamano}; \n";
                codigoSalida += $"{stack_heap}[(int){temp}] = {valAsignacion.Temporal};\n";
                

               
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

        public void obtenerListasAnidadas(LinkedList<string> variablesUsadas)
        {
            if (valorInicializacion != null)
            {
                valorInicializacion.obtenerListasAnidadas(variablesUsadas);
            }
        }

        public void obtenerIdes(LinkedList<string> declaraciones)
        {

            if (ideUnico != null)
            {
                declaraciones.AddLast(this.ideUnico.Identificador.ToLower());
            }

            if (variables != null)
            {
                foreach (Simbolo item in this.variables)
                {
                    declaraciones.AddLast(item.Identificador.ToLower());
                }
            }
        
        }

        public int obtenerTamano()
        {
            int numero = 0;
            if (ideUnico != null)
            {
                return 1;
            }

            if (variables != null)
            {
                foreach (Simbolo item in this.variables)
                {
                    numero++;
                }
            }
            return numero;

        }

        public TipoDatos tipoVars()
        {
            if (ideUnico != null)
            {
                return ideUnico.Tipo;
            }

            if(variables != null)
            {
                foreach (Simbolo item in variables)
                {
                    return item.Tipo;
                }
            }

            return TipoDatos.NULL;
        }
        
    }


}
