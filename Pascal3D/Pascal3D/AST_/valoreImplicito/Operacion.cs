using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using Pascal3D;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Text;
using static CompiPascal.entorno_.Simbolo;
using CompiPascal.Traductor;

namespace CompiPascal.AST_.valoreImplicito
{

    /*
     * @class               Operacion
     * @Comentario          Esta clase sera util para construir todo tipo de operaciones en el lenguaje
     *                      ya sea: aritmeticas, logicas y/o relacionales. 
     */

    public class Operacion : Expresion
    {

        public int tamanoPadre { get; set; }
        /**
         * @propiedad    Operador
         * @comentario   Este enumerador servira para contener el tipo de la operacion. 
         */

        public enum Operador
        {
            MAS,
            MENOS,
            DIVISION,
            MODULO,
            MULTIPLICACION,
            MENOS_UNARIO,
            MAYOR,
            MENOR,
            MAYOR_QUE,
            MENOR_QUE,
            IGUAL,
            DIFERENTE,
            AND,
            OR,
            NOT,
            DESCONOCIDO
        }

        private Expresion operando1;
        private Expresion operando2;
        private Expresion operandoU;
        private Operador operador;

        public int linea { get; set; }
        public int columna { get; set; }

        /*
         * @param   string      etiquetaFalsa              Guarda la siguiente etiqueta para una instrucción donde se 
         *                                                  evalua una expresión condicional
         */
        public string etiquetaFalsa { get; set; }
        /*
         * @param   string      etiquetaVerdadera           Guarda la etiqueta verdadera para una instrucción donde se 
         *                                                  evalua una expresión condicional
         */
        public string etiquetaVerdadera { get; set; }

        /**
         * @constructor  public Operacion(Expresion operando1, Expresion operando2, Operador operador)
         *
         * @comentario   Constructor para operaciones binarias.
         * 
         * @param   operando1   Expresion izquierda de la operacion.
         * @param   operando2   Expresion derecha de la operacion.
         * @param   operador    Tipo de operación que se esta realizando.
         */


        public Operacion(Expresion op1, Expresion op2, Operador operador_ , int linea, int columna)
        {
            this.operando1 = op1;
            this.operando2 = op2;
            this.operador = operador_;
            this.linea = linea;
            this.columna = columna;
            this.etiquetaFalsa = "";
        }


        /**
         * @constructor  public Operacion(Expresion OpUnario, Operador operador_)
         *
         * @comentario   Constructor para operaciones unarias (un operando).
         * 
         * @param   OpUnario    Unica expresion para la operacion unaria (ej:!expr)
         * @param   operador    Tipo de operación que se esta realizando.
         */

        public Operacion(Expresion OpUnario, Operador operador_, int linea, int columna)
        {
            this.operandoU = OpUnario;
            this.operador = operador_;
            this.linea = linea;
            this.columna = columna;
            this.etiquetaFalsa = "";
        }


        /**
         * @ param          op
         * @ comentario     op es una cadena con el operador, para recuperar un valor del enum
         *                  operador.
         */

        public static Operador GetOperador(string op)
        {
            op = op.ToLower();
            switch (op)
            {
                case "+":
                    return Operador.MAS;
                case "-":
                    return Operador.MENOS;
                case "*":
                    return Operador.MULTIPLICACION;
                case "/":
                    return Operador.DIVISION;
                case "div":
                    return Operador.DIVISION;
                case "mod":
                    return Operador.MODULO;
                case "<":
                    return Operador.MENOR;
                case "<=":
                    return Operador.MENOR_QUE;
                case ">":
                    return Operador.MAYOR;
                case ">=":
                    return Operador.MAYOR_QUE;
                case "=":
                    return Operador.IGUAL;
                case "and":
                    return Operador.AND;
                case "or":
                    return Operador.OR;
                case "not":
                    return Operador.NOT;
                case "<>":
                    return Operador.DIFERENTE;

                default:
                    return Operador.DESCONOCIDO;
            }

        }

        public result3D obtener3D(Entorno ent)
        {

            result3D resultado = null;

            switch (operador)
            {
                case Operador.MAS:
                    resultado = SUMA(operando1, operando2, ent);
                    break;
                case Operador.MENOS:
                    if(operando1 != null && operando2 != null)
                    {
                        resultado = RESTA(operando1, operando2, ent);
                    }
                    else
                    {
                        resultado= RESTA_UNARIA(operandoU,ent);
                    }
                    break;
                case Operador.MULTIPLICACION:
                    resultado = MULTIPLICACION(operando1, operando2, ent);
                    break;
                case Operador.DIVISION:
                    resultado = DIVISION(operando1, operando2, ent);
                    break;
                case Operador.MODULO:
                    resultado = MODULO(operando1, operando2, ent);
                    break;
                case Operador.AND:
                    resultado = LOGIC_AND(operando1, operando2, ent);
                    break;
                case Operador.OR:
                    resultado = LOGIC_OR(operando1, operando2, ent);
                    break;
                case Operador.NOT:
                    resultado = LOGIC_NOT(operandoU, ent);
                    break;


                case Operador.MAYOR:
                case Operador.MENOR:
                case Operador.MAYOR_QUE:
                case Operador.MENOR_QUE:
                case Operador.DIFERENTE:
                case Operador.IGUAL:
                    resultado = RELACION(operando1, operando2, stringOp(), ent);
                    break;

                default:
                    break;
            }



            return resultado;

        }

        public result3D SUMA(Expresion opIzq, Expresion opDer,Entorno ent)
        {

            result3D expreIzq = opIzq.obtener3D(ent);
            result3D expreDer = opDer.obtener3D(ent);

            result3D resultado = new result3D(); 
            string er = "El tipo " + expreIzq.TipoResultado + " no se puede sumar con " + expreDer.TipoResultado;

            /****************************************************************************
             * ══════════════════════════════════════════════════════════════════════════
             *                         IZQUIERDO ->     INTEGER  
             * ══════════════════════════════════════════════════════════════════════════
             ****************************************************************************/

            if (expreIzq.TipoResultado == TipoDatos.Integer)
            {

                /*
                 *          TIPOS DEL SEGUNDO OPERANDO      INTEGER ----  X TIPO
                 */
                switch (expreDer.TipoResultado)
                {

                    case TipoDatos.Integer:

                        //PEDIMOS UN TEMPORAL QUE GUARDARA EL RESULTADO
                        resultado.Temporal = Generador.pedirTemporal();
                        resultado.TipoResultado = TipoDatos.Integer;

                        resultado.Codigo = expreIzq.Codigo + expreDer.Codigo + "\n";
                        resultado.Codigo += resultado.Temporal + " = " + expreIzq.Temporal + " + " + expreDer.Temporal + "; \n";
                        break;

                    case TipoDatos.Real:

                        //PEDIMOS UN TEMPORAL QUE GUARDARA EL RESULTADO
                        resultado.Temporal = Generador.pedirTemporal();
                        resultado.TipoResultado = TipoDatos.Real;

                        resultado.Codigo = expreIzq.Codigo + expreDer.Codigo + "\n";
                        resultado.Codigo += resultado.Temporal + " = " + expreIzq.Temporal + " + " + expreDer.Temporal + "; \n";
                        break;


                    default:
                        Program.getIntefaz().agregarError(er, operando1.linea, operando1.columna);
                        break;
                }

            }
            /****************************************************************************
             * ══════════════════════════════════════════════════════════════════════════
             *                         IZQUIERDO ->     REAL  
             * ══════════════════════════════════════════════════════════════════════════
             ****************************************************************************/
            else if (expreIzq.TipoResultado == TipoDatos.Real)
            {

                /*
                *          TIPOS DEL SEGUNDO OPERANDO      REAL ----  X TIPO
                */
                switch (expreDer.TipoResultado)
                {

                    case TipoDatos.Integer:

                        //PEDIMOS UN TEMPORAL QUE GUARDARA EL RESULTADO
                        resultado.Temporal = Generador.pedirTemporal();
                        resultado.TipoResultado = TipoDatos.Real;

                        resultado.Codigo = expreIzq.Codigo + expreDer.Codigo + "\n";
                        resultado.Codigo += resultado.Temporal + " = " + expreIzq.Temporal + " + " + expreDer.Temporal + "; \n";
                        break;

                    case TipoDatos.Real:

                        //PEDIMOS UN TEMPORAL QUE GUARDARA EL RESULTADO
                        resultado.Temporal = Generador.pedirTemporal();
                        resultado.TipoResultado = TipoDatos.Real;

                        resultado.Codigo = expreIzq.Codigo + expreDer.Codigo + "\n";
                        resultado.Codigo += resultado.Temporal + " = " + expreIzq.Temporal + " + " + expreDer.Temporal + "; \n";
                        break;


                    default:
                        Program.getIntefaz().agregarError(er, operando1.linea, operando1.columna);
                        break;
                }



            }
            /****************************************************************************
             * ══════════════════════════════════════════════════════════════════════════
             *                         IZQUIERDO ->     STRING  
             * ══════════════════════════════════════════════════════════════════════════
             ****************************************************************************/
            else if (expreIzq.TipoResultado == TipoDatos.String)
            {
                resultado.Temporal = Generador.pedirTemporal();
                resultado.TipoResultado = TipoDatos.String;

                resultado.Codigo += expreIzq.Codigo;
                resultado.Codigo += expreDer.Codigo;

                resultado.Codigo += $"{resultado.Temporal} = HP; /*Guardamos el nuevo inicio de la cadena*/;";
                
                string codigo = SumarCadena(operando1, expreIzq);
                string codigo2 = SumarCadena(operando2, expreDer);

                resultado.Codigo += $"Heap[HP] = 0; \n";
                resultado.Codigo += $"HP = HP + 1; \n";
                resultado.Codigo += codigo + codigo2;


            }            
            /****************************************************************************
             * ══════════════════════════════════════════════════════════════════════════
             *                         IZQUIERDO ->     CHAR  
             * ══════════════════════════════════════════════════════════════════════════
             ****************************************************************************/
            else if (expreIzq.TipoResultado == TipoDatos.Char )
            {

                if (expreDer.TipoResultado != TipoDatos.Char)
                {
                    Program.getIntefaz().agregarError("Char solo puede sumarse con char",operando2.linea,operando2.columna);
                    return new result3D();
                }

                resultado.Temporal = Generador.pedirTemporal();
                resultado.TipoResultado = TipoDatos.String;

                resultado.Codigo = expreIzq.Codigo + expreDer.Codigo + "\n";
                resultado.Codigo += resultado.Temporal + " = " + expreIzq.Temporal + " + " + expreDer.Temporal + "; \n";
            }
            else
            {
                //LOS TIPOS NO SON OPERABLES EN PASCAL
                Program.getIntefaz().agregarError(er, operando1.linea, operando1.columna);
            }


            return resultado;
        }

        public result3D RESTA_UNARIA(Expresion unario,Entorno ent)
        {

            result3D resultUnario = unario.obtener3D(ent);

            if(resultUnario.TipoResultado != TipoDatos.Integer && resultUnario.TipoResultado != TipoDatos.Real)
            {
                Program.getIntefaz().agregarError("El operador unario \"-\" solo puede operarse con un valor numerico", unario.linea, unario.columna);
                return new result3D();
            }

            result3D restaUnario = new result3D();
            string temp1 = Generador.pedirTemporal();

            restaUnario.Codigo += resultUnario.Codigo;
            restaUnario.Codigo += $"{temp1} = 0 - {resultUnario.Temporal};\n";

            resultUnario.Temporal = temp1;
            resultUnario.TipoResultado = resultUnario.TipoResultado;

            return resultUnario;
        }

        public result3D RESTA(Expresion opIzq, Expresion opDer, Entorno ent)
        {

            result3D expreIzq = opIzq.obtener3D(ent);
            result3D expreDer = opDer.obtener3D(ent);

            result3D resultado = new result3D();
            string er = "El tipo " + expreIzq.TipoResultado + " no se puede RESTAR con " + expreDer.TipoResultado;

            /****************************************************************************
             * ══════════════════════════════════════════════════════════════════════════
             *                         IZQUIERDO ->     INTEGER  
             * ══════════════════════════════════════════════════════════════════════════
             ****************************************************************************/

            if (expreIzq.TipoResultado == TipoDatos.Integer)
            {

                /*
                 *          TIPOS DEL SEGUNDO OPERANDO      INTEGER ----  X TIPO
                 */
                switch (expreDer.TipoResultado)
                {

                    case TipoDatos.Integer:

                        //PEDIMOS UN TEMPORAL QUE GUARDARA EL RESULTADO
                        resultado.Temporal = Generador.pedirTemporal();
                        resultado.TipoResultado = TipoDatos.Integer;

                        resultado.Codigo = expreIzq.Codigo + expreDer.Codigo + "\n";
                        resultado.Codigo += resultado.Temporal + " = " + expreIzq.Temporal + " - " + expreDer.Temporal + "; \n";
                        break;

                    case TipoDatos.Real:

                        //PEDIMOS UN TEMPORAL QUE GUARDARA EL RESULTADO
                        resultado.Temporal = Generador.pedirTemporal();
                        resultado.TipoResultado = TipoDatos.Real;

                        resultado.Codigo = expreIzq.Codigo + expreDer.Codigo + "\n";
                        resultado.Codigo += resultado.Temporal + " = " + expreIzq.Temporal + " - " + expreDer.Temporal + "; \n";
                        break;


                    default:
                        Program.getIntefaz().agregarError(er, operando1.linea, operando1.columna);
                        break;
                }

            }
            /****************************************************************************
             * ══════════════════════════════════════════════════════════════════════════
             *                         IZQUIERDO ->     REAL  
             * ══════════════════════════════════════════════════════════════════════════
             ****************************************************************************/
            else if (expreIzq.TipoResultado == TipoDatos.Real)
            {

                /*
                *          TIPOS DEL SEGUNDO OPERANDO      REAL ----  X TIPO
                */
                switch (expreDer.TipoResultado)
                {

                    case TipoDatos.Integer:

                        //PEDIMOS UN TEMPORAL QUE GUARDARA EL RESULTADO
                        resultado.Temporal = Generador.pedirTemporal();
                        resultado.TipoResultado = TipoDatos.Real;

                        resultado.Codigo = expreIzq.Codigo + expreDer.Codigo + "\n";
                        resultado.Codigo += resultado.Temporal + " = " + expreIzq.Temporal + " - " + expreDer.Temporal + "; \n";
                        break;

                    case TipoDatos.Real:

                        //PEDIMOS UN TEMPORAL QUE GUARDARA EL RESULTADO
                        resultado.Temporal = Generador.pedirTemporal();
                        resultado.TipoResultado = TipoDatos.Real;

                        resultado.Codigo = expreIzq.Codigo + expreDer.Codigo + "\n";
                        resultado.Codigo += resultado.Temporal + " = " + expreIzq.Temporal + " - " + expreDer.Temporal + "; \n";
                        break;


                    default:
                        Program.getIntefaz().agregarError(er, operando1.linea, operando1.columna);
                        break;
                }



            }
            /****************************************************************************
             * ══════════════════════════════════════════════════════════════════════════
             *                        OPERANDOS NO SE PUEDEN RESTAR
             * ══════════════════════════════════════════════════════════════════════════
             ****************************************************************************/
            else
            {
                //LOS TIPOS NO SON OPERABLES EN PASCAL
                Program.getIntefaz().agregarError(er, operando1.linea, operando1.columna);
            }


            return resultado;
        }
        public result3D MULTIPLICACION(Expresion opIzq, Expresion opDer, Entorno ent)
        {

            result3D expreIzq = opIzq.obtener3D(ent);
            result3D expreDer = opDer.obtener3D(ent);

            result3D resultado = new result3D();
            string er = "El tipo " + expreIzq.TipoResultado + " no se puede MULTIPLICAR con " + expreDer.TipoResultado;

            /****************************************************************************
             * ══════════════════════════════════════════════════════════════════════════
             *                         IZQUIERDO ->     INTEGER  
             * ══════════════════════════════════════════════════════════════════════════
             ****************************************************************************/

            if (expreIzq.TipoResultado == TipoDatos.Integer)
            {

                /*
                 *          TIPOS DEL SEGUNDO OPERANDO      INTEGER ----  X TIPO
                 */
                switch (expreDer.TipoResultado)
                {

                    case TipoDatos.Integer:

                        //PEDIMOS UN TEMPORAL QUE GUARDARA EL RESULTADO
                        resultado.Temporal = Generador.pedirTemporal();
                        resultado.TipoResultado = TipoDatos.Integer;

                        resultado.Codigo = expreIzq.Codigo + expreDer.Codigo + "\n";
                        resultado.Codigo += resultado.Temporal + " = " + expreIzq.Temporal + " * " + expreDer.Temporal + "; \n";
                        break;

                    case TipoDatos.Real:

                        //PEDIMOS UN TEMPORAL QUE GUARDARA EL RESULTADO
                        resultado.Temporal = Generador.pedirTemporal();
                        resultado.TipoResultado = TipoDatos.Real;

                        resultado.Codigo = expreIzq.Codigo + expreDer.Codigo + "\n";
                        resultado.Codigo += resultado.Temporal + " = " + expreIzq.Temporal + " * " + expreDer.Temporal + "; \n";
                        break;


                    default:
                        Program.getIntefaz().agregarError(er, operando1.linea, operando1.columna);
                        break;
                }

            }
            /****************************************************************************
             * ══════════════════════════════════════════════════════════════════════════
             *                         IZQUIERDO ->     REAL  
             * ══════════════════════════════════════════════════════════════════════════
             ****************************************************************************/
            else if (expreIzq.TipoResultado == TipoDatos.Real)
            {

                /*
                *          TIPOS DEL SEGUNDO OPERANDO      REAL ----  X TIPO
                */
                switch (expreDer.TipoResultado)
                {

                    case TipoDatos.Integer:

                        //PEDIMOS UN TEMPORAL QUE GUARDARA EL RESULTADO
                        resultado.Temporal = Generador.pedirTemporal();
                        resultado.TipoResultado = TipoDatos.Real;

                        resultado.Codigo = expreIzq.Codigo + expreDer.Codigo + "\n";
                        resultado.Codigo += resultado.Temporal + " = " + expreIzq.Temporal + " * " + expreDer.Temporal + "; \n";
                        break;

                    case TipoDatos.Real:

                        //PEDIMOS UN TEMPORAL QUE GUARDARA EL RESULTADO
                        resultado.Temporal = Generador.pedirTemporal();
                        resultado.TipoResultado = TipoDatos.Real;

                        resultado.Codigo = expreIzq.Codigo + expreDer.Codigo + "\n";
                        resultado.Codigo += resultado.Temporal + " = " + expreIzq.Temporal + " * " + expreDer.Temporal + "; \n";
                        break;


                    default:
                        Program.getIntefaz().agregarError(er, operando1.linea, operando1.columna);
                        break;
                }



            }
            /****************************************************************************
             * ══════════════════════════════════════════════════════════════════════════
             *                        OPERANDOS NO SE PUEDEN RESTAR
             * ══════════════════════════════════════════════════════════════════════════
             ****************************************************************************/
            else
            {
                //LOS TIPOS NO SON OPERABLES EN PASCAL
                Program.getIntefaz().agregarError(er, operando1.linea, operando1.columna);
            }


            return resultado;
        }
        public result3D DIVISION(Expresion opIzq, Expresion opDer, Entorno ent)
        {

            result3D expreIzq = opIzq.obtener3D(ent);
            result3D expreDer = opDer.obtener3D(ent);

            result3D resultado = new result3D();
            string er = "El tipo " + expreIzq.TipoResultado + " no se puede RESTAR con " + expreDer.TipoResultado;

            if (expreDer.Temporal.Equals('0'))
            {
                Program.getIntefaz().agregarError("Division entre 0 ", operando1.linea, operando1.columna);
                return new result3D();
            }

            /****************************************************************************
             * ══════════════════════════════════════════════════════════════════════════
             *                         IZQUIERDO ->     INTEGER  
             * ══════════════════════════════════════════════════════════════════════════
             ****************************************************************************/

            if (expreIzq.TipoResultado == TipoDatos.Integer)
            {

                /*
                 *          TIPOS DEL SEGUNDO OPERANDO      INTEGER ----  X TIPO
                 */
                switch (expreDer.TipoResultado)
                {

                    case TipoDatos.Integer:

                        //PEDIMOS UN TEMPORAL QUE GUARDARA EL RESULTADO
                        resultado.Temporal = Generador.pedirTemporal();
                        resultado.TipoResultado = TipoDatos.Integer;

                        resultado.Codigo = expreIzq.Codigo + expreDer.Codigo + "\n";
                        resultado.Codigo += resultado.Temporal + " = " + expreIzq.Temporal + " / " + expreDer.Temporal + "; \n";
                        break;

                    case TipoDatos.Real:

                        //PEDIMOS UN TEMPORAL QUE GUARDARA EL RESULTADO
                        resultado.Temporal = Generador.pedirTemporal();
                        resultado.TipoResultado = TipoDatos.Real;

                        resultado.Codigo = expreIzq.Codigo + expreDer.Codigo + "\n";
                        resultado.Codigo += resultado.Temporal + " = " + expreIzq.Temporal + " / " + expreDer.Temporal + "; \n";
                        break;


                    default:
                        Program.getIntefaz().agregarError(er, operando1.linea, operando1.columna);
                        break;
                }

            }
            /****************************************************************************
             * ══════════════════════════════════════════════════════════════════════════
             *                         IZQUIERDO ->     REAL  
             * ══════════════════════════════════════════════════════════════════════════
             ****************************************************************************/
            else if (expreIzq.TipoResultado == TipoDatos.Real)
            {

                /*
                *          TIPOS DEL SEGUNDO OPERANDO      REAL ----  X TIPO
                */
                switch (expreDer.TipoResultado)
                {

                    case TipoDatos.Integer:

                        //PEDIMOS UN TEMPORAL QUE GUARDARA EL RESULTADO
                        resultado.Temporal = Generador.pedirTemporal();
                        resultado.TipoResultado = TipoDatos.Real;

                        resultado.Codigo = expreIzq.Codigo + expreDer.Codigo + "\n";
                        resultado.Codigo += resultado.Temporal + " = " + expreIzq.Temporal + " / " + expreDer.Temporal + "; \n";
                        break;

                    case TipoDatos.Real:

                        //PEDIMOS UN TEMPORAL QUE GUARDARA EL RESULTADO
                        resultado.Temporal = Generador.pedirTemporal();
                        resultado.TipoResultado = TipoDatos.Real;

                        resultado.Codigo = expreIzq.Codigo + expreDer.Codigo + "\n";
                        resultado.Codigo += resultado.Temporal + " = " + expreIzq.Temporal + " / " + expreDer.Temporal + "; \n";
                        break;


                    default:
                        Program.getIntefaz().agregarError(er, operando1.linea, operando1.columna);
                        break;
                }



            }
            /****************************************************************************
             * ══════════════════════════════════════════════════════════════════════════
             *                        OPERANDOS NO SE PUEDEN RESTAR
             * ══════════════════════════════════════════════════════════════════════════
             ****************************************************************************/
            else
            {
                //LOS TIPOS NO SON OPERABLES EN PASCAL
                Program.getIntefaz().agregarError(er, operando1.linea, operando1.columna);
            }


            return resultado;
        }
        public result3D MODULO(Expresion opIzq, Expresion opDer, Entorno ent)
        {

            result3D expreIzq = opIzq.obtener3D(ent);
            result3D expreDer = opDer.obtener3D(ent);

            result3D resultado = new result3D();
            string er = "El tipo " + expreIzq.TipoResultado + " no se puede MODULAR con " + expreDer.TipoResultado;


            /****************************************************************************
             * ══════════════════════════════════════════════════════════════════════════
             *                         IZQUIERDO ->     INTEGER  
             * ══════════════════════════════════════════════════════════════════════════
             ****************************************************************************/

            if (expreIzq.TipoResultado == TipoDatos.Integer)
            {

                /*
                 *          TIPOS DEL SEGUNDO OPERANDO      INTEGER ----  X TIPO
                 */
                switch (expreDer.TipoResultado)
                {

                    case TipoDatos.Integer:

                        //PEDIMOS UN TEMPORAL QUE GUARDARA EL RESULTADO
                        resultado.Temporal = Generador.pedirTemporal();
                        resultado.TipoResultado = TipoDatos.Integer;

                        resultado.Codigo = expreIzq.Codigo + expreDer.Codigo + "\n";
                        resultado.Codigo += resultado.Temporal + " = " + expreIzq.Temporal + " % " + expreDer.Temporal + "; \n";
                        break;

                    case TipoDatos.Real:

                        //PEDIMOS UN TEMPORAL QUE GUARDARA EL RESULTADO
                        resultado.Temporal = Generador.pedirTemporal();
                        resultado.TipoResultado = TipoDatos.Real;

                        resultado.Codigo = expreIzq.Codigo + expreDer.Codigo + "\n";
                        resultado.Codigo += resultado.Temporal + " = " + expreIzq.Temporal + " % " + expreDer.Temporal + "; \n";
                        break;


                    default:
                        Program.getIntefaz().agregarError(er, operando1.linea, operando1.columna);
                        break;
                }

            }
            /****************************************************************************
             * ══════════════════════════════════════════════════════════════════════════
             *                         IZQUIERDO ->     REAL  
             * ══════════════════════════════════════════════════════════════════════════
             ****************************************************************************/
            else if (expreIzq.TipoResultado == TipoDatos.Real)
            {

                /*
                *          TIPOS DEL SEGUNDO OPERANDO      REAL ----  X TIPO
                */
                switch (expreDer.TipoResultado)
                {

                    case TipoDatos.Integer:

                        //PEDIMOS UN TEMPORAL QUE GUARDARA EL RESULTADO
                        resultado.Temporal = Generador.pedirTemporal();
                        resultado.TipoResultado = TipoDatos.Real;

                        resultado.Codigo = expreIzq.Codigo + expreDer.Codigo + "\n";
                        resultado.Codigo += resultado.Temporal + " = " + expreIzq.Temporal + " % " + expreDer.Temporal + "; \n";
                        break;

                    case TipoDatos.Real:

                        //PEDIMOS UN TEMPORAL QUE GUARDARA EL RESULTADO
                        resultado.Temporal = Generador.pedirTemporal();
                        resultado.TipoResultado = TipoDatos.Real;

                        resultado.Codigo = expreIzq.Codigo + expreDer.Codigo + "\n";
                        resultado.Codigo += resultado.Temporal + " = " + expreIzq.Temporal + " % " + expreDer.Temporal + "; \n";
                        break;


                    default:
                        Program.getIntefaz().agregarError(er, operando1.linea, operando1.columna);
                        break;
                }



            }
            /****************************************************************************
             * ══════════════════════════════════════════════════════════════════════════
             *                        OPERANDOS NO SE PUEDEN RESTAR
             * ══════════════════════════════════════════════════════════════════════════
             ****************************************************************************/
            else
            {
                //LOS TIPOS NO SON OPERABLES EN PASCAL
                Program.getIntefaz().agregarError(er, operando1.linea, operando1.columna);
            }


            return resultado;
        }
        public result3D RELACION(Expresion opIzq, Expresion opDer, string relacion, Entorno ent)
        {

            /*
             * if a < b goto B.true
             * goto B.false
             * 
             */
            result3D resultado = new result3D();
            result3D resultadoIz = opIzq.obtener3D(ent);
            result3D resultadoDe = opDer.obtener3D(ent);


            string er = "El tipo " + resultadoIz.TipoResultado + " no se puede OPERAR con " + resultadoDe.TipoResultado;

            if (resultadoIz.TipoResultado == TipoDatos.Integer && resultadoDe.TipoResultado == TipoDatos.Integer ||
               resultadoIz.TipoResultado == TipoDatos.Real    && resultadoDe.TipoResultado == TipoDatos.Integer ||
               resultadoIz.TipoResultado == TipoDatos.Integer && resultadoDe.TipoResultado == TipoDatos.Real    ||
               resultadoIz.TipoResultado == TipoDatos.Real    && resultadoDe.TipoResultado == TipoDatos.Real      )
            {
                //OBTENEMOS LAS ETIQUETAS DE SALIDA Y CONTINUACION
                string etiquetaV;
                etiquetaV  = (etiquetaVerdadera == null || etiquetaVerdadera.Equals("")) ? Generador.pedirEtiqueta() : etiquetaVerdadera;

                string etiquetaF;
                etiquetaF  = (etiquetaFalsa== null || etiquetaFalsa.Equals("")) ? Generador.pedirEtiqueta() :etiquetaFalsa;


                resultado.Codigo =  resultadoIz.Codigo + resultadoDe.Codigo +"\n";
                resultado.Codigo += "if (" + resultadoIz.Temporal + relacion + resultadoDe.Temporal + ") goto "+ etiquetaV + "; \n";
                resultado.Codigo += Generador.tabularLinea("goto " + etiquetaF + ";\n",2);

                resultado.EtiquetaV = etiquetaV;
                resultado.EtiquetaF = etiquetaF;

                resultado.TipoResultado = TipoDatos.Boolean;
            }

            else if(resultadoIz.TipoResultado == TipoDatos.Boolean && resultadoDe.TipoResultado == TipoDatos.Boolean 
                && (relacion.Equals("==") || relacion.Equals("!=")))
            {
                //OBTENEMOS LAS ETIQUETAS DE SALIDA Y CONTINUACION
                string etiquetaV;
                etiquetaV = (etiquetaVerdadera == null || etiquetaVerdadera.Equals("")) ? Generador.pedirEtiqueta() : etiquetaVerdadera;

                string etiquetaF;
                etiquetaF = (etiquetaFalsa == null || etiquetaFalsa.Equals("")) ? Generador.pedirEtiqueta() : etiquetaFalsa;


                resultado.Codigo = resultadoIz.Codigo + resultadoDe.Codigo + "\n";
                resultado.Codigo += "if (" + resultadoIz.Temporal + relacion + resultadoDe.Temporal + ") goto " + etiquetaV + "; \n";
                resultado.Codigo += Generador.tabularLinea("goto " + etiquetaF + ";\n", 2);

                resultado.EtiquetaV = etiquetaV;
                resultado.EtiquetaF = etiquetaF;

                resultado.TipoResultado = TipoDatos.Boolean;
            }
            else
            {
                Program.getIntefaz().agregarError(er, operando1.linea, operando1.columna);
            }


            return resultado;
        }
        public result3D LOGIC_AND(Expresion opIzq, Expresion opDer, Entorno ent)
        {
            /*
             * if ( x < 100 || x > 200 && x != y ) x = 0;
             * 
             *          if x < 100  goto L2
             *          goto L3
             *  L3:     if x > 200  goto L4
             *          goto L1
             *  L4:     if x!= y    goto L2
             *          goto L1
             *  L2:     x=0
             *  L1:     
             *   
             */


            // OPERADOR IZQUIERDO, EN LA ETIQUETA VERDADERA SI SE GENERA UNA NUEVA ETIQUETA
            // EN LA FALSA NO

            opIzq.etiquetaFalsa = etiquetaFalsa;
            opIzq.etiquetaVerdadera = Generador.pedirEtiqueta();
            result3D resultIzq = opIzq.obtener3D(ent);

            // OPERADOR FALSO, LA ETIQUETA FALSA Y VERDADERA DE ESTE OPERANDO SON LOS MISMOS DEL PADRES
            // POR ESO QUE SOLO SE COPIAN DE LA OPERACIÓN ACTUAL
            opDer.etiquetaFalsa = etiquetaFalsa;
            opDer.etiquetaVerdadera = etiquetaVerdadera;
            result3D resultDer = opDer.obtener3D(ent);

            result3D resultado = new result3D();
            string er = "Eror tipo " + resultIzq.TipoResultado + " and " + resultDer.TipoResultado;

            if (resultIzq.TipoResultado != TipoDatos.Boolean || resultDer.TipoResultado != TipoDatos.Boolean)
            {
                Program.getIntefaz().agregarError(er, opIzq.linea, opIzq.columna);
                return resultado;
            }

            /*  VERIFICAMOS EL TIPO DE EXPRESION, DEL OPERANDO IZQUIERDO. ESTO PARA SABER SI EL CODIGO QUE TRAE
             *  ESTO PARA SABER SI ESA EXPRESION ES SOL UN TRUE O FALSE     -> O ES UNA OPERACION LOGICA O ARITMETICA QUE 
             *  LO GENERO. 
             *  CUANDO ES UN TRU O FALSE EL VALOR LO GUARDAMOS EN EL TEMPORAL DE ESA EXPRESION (result3D.temporal)
             */

            if (resultIzq.EtiquetaF.Equals("") && resultIzq.EtiquetaV.Equals(""))
            {
                string etiquetaV = Generador.pedirEtiqueta();
                string etiquetaF;
                if (etiquetaFalsa.Equals("")) etiquetaF = Generador.pedirEtiqueta();
                else etiquetaF = etiquetaFalsa;

                resultado.Codigo = resultIzq.Codigo; 
                resultado.Codigo += "if (" + resultIzq.Temporal + " == 1)  goto " + etiquetaV+ "; \n";
                resultado.Codigo += "goto " + etiquetaF + "; \n";

                resultIzq.EtiquetaV = etiquetaV;
                resultIzq.EtiquetaF = etiquetaF;
            }
            else
            {
                // SI ES UNA RELACION QUE GENERO EL BOOLEANO, SOLO SE PEGA EL CODIGO
                resultado.Codigo = resultIzq.Codigo + "\n";
            }

            //PEGAMOS LA ETIQUETA VERDADERA PARA EVALUAR EL SIGUIENTE OPERANDO DEL AND
            resultado.Codigo += resultIzq.EtiquetaV+ ": ";

            if(resultDer.EtiquetaF.Equals("") && resultDer.EtiquetaV.Equals(""))
            {
                string etiquetaV = Generador.pedirEtiqueta();
                resultado.Codigo += resultDer.Codigo;
                resultado.Codigo += "if (" + resultDer.Temporal + " == 1) goto " + etiquetaV +"; \n";
                resultado.Codigo += "goto " + resultIzq.EtiquetaF + "; \n";

                resultDer.EtiquetaV = etiquetaV;
                resultDer.EtiquetaF = resultIzq.EtiquetaF;
            }
            else
            {
                resultado.Codigo += resultDer.Codigo +"\n";
            }

            resultado.EtiquetaV =  resultDer.EtiquetaV;
            resultado.EtiquetaF =  resultDer.EtiquetaF;
            resultado.TipoResultado = TipoDatos.Boolean;

            return resultado;
        }
        public result3D LOGIC_OR(Expresion opIzq, Expresion opDer, Entorno ent)
        {
            /*
             * if ( x < 100 || x > 200 && x != y ) x = 0;
             * 
             *          if x < 100  goto L2
             *          goto L3
             *  L3:     if x > 200  goto L4
             *          goto L1
             *  L4:     if x!= y    goto L2
             *          goto L1
             *  L2:     x=0
             *  L1:     
             *   
             */



            // OPERADOR IZQUIERDO, EN LA ETIQUETA FALSA SI SE GENERA UNA NUEVA ETIQUETA
            // EN LA VERDADERA NO
            opIzq.etiquetaVerdadera = etiquetaVerdadera;
            opIzq.etiquetaFalsa = Generador.pedirEtiqueta();
            result3D resultIzq = opIzq.obtener3D(ent);

            // OPERADOR FALSO, LA ETIQUETA FALSA Y VERDADERA DE ESTE OPERANDO SON LOS MISMOS DEL PADRES
            // POR ESO QUE SOLO SE COPIAN DE LA OPERACIÓN ACTUAL
            opDer.etiquetaVerdadera = etiquetaVerdadera;
            opDer.etiquetaFalsa = etiquetaFalsa;
            result3D resultDer = opDer.obtener3D(ent);

            result3D resultado = new result3D();
            string er = "Eror tipo " + resultIzq.TipoResultado + " or " + resultDer.TipoResultado;

            if (resultIzq.TipoResultado != TipoDatos.Boolean || resultDer.TipoResultado != TipoDatos.Boolean)
            {
                Program.getIntefaz().agregarError(er, opIzq.linea, opIzq.columna);
                return resultado;
            }

            /*  VERIFICAMOS EL TIPO DE EXPRESION, DEL OPERANDO IZQUIERDO. ESTO PARA SABER SI EL CODIGO QUE TRAE
             *  ESTO PARA SABER SI ESA EXPRESION ES SOL UN TRUE O FALSE     -> O ES UNA OPERACION LOGICA O ARITMETICA QUE 
             *  LO GENERO. 
             *  CUANDO ES UN TRU O FALSE EL VALOR LO GUARDAMOS EN EL TEMPORAL DE ESA EXPRESION (result3D.temporal)
             */

            if (resultIzq.EtiquetaF.Equals("") && resultIzq.EtiquetaV.Equals(""))
            {
                string etiquetaV = opIzq.etiquetaVerdadera;
                string etiquetaF = opIzq.etiquetaFalsa;

                resultado.Codigo += resultIzq.Codigo;
                resultado.Codigo += "if (" + resultIzq.Temporal + " == 1 ) goto " + etiquetaV + "; \n";
                resultado.Codigo += "goto " + etiquetaF + "; \n";

                resultIzq.EtiquetaV = etiquetaV;
                resultIzq.EtiquetaF = etiquetaF;
            }
            else
            {
                // SI ES UNA RELACION QUE GENERO EL BOOLEANO, SOLO SE PEGA EL CODIGO
                resultado.Codigo = resultIzq.Codigo + "\n";
            }

            //PEGAMOS LA ETIQUETA VERDADERA PARA EVALUAR EL SIGUIENTE OPERANDO DEL AND
            resultado.Codigo += resultIzq.EtiquetaF + ": ";

            if (resultDer.EtiquetaF.Equals("") && resultDer.EtiquetaV.Equals(""))
            {
                string etiquetaV = opDer.etiquetaVerdadera;
                string etiquetaF = opDer.etiquetaFalsa;

                resultado.Codigo += resultDer.Codigo;
                resultado.Codigo += "if (" + resultDer.Temporal + " == 1) goto " + etiquetaV + "; \n";
                resultado.Codigo += "goto " + resultIzq.EtiquetaF + "; \n";

                resultDer.EtiquetaV = etiquetaV;
                resultDer.EtiquetaF = etiquetaF;
            }
            else
            {
                resultado.Codigo += resultDer.Codigo + "\n";
            }

            resultado.EtiquetaV = resultDer.EtiquetaV;
            resultado.EtiquetaF = resultDer.EtiquetaF;
            resultado.TipoResultado = TipoDatos.Boolean;

            return resultado;
        }

        public result3D LOGIC_NOT(Expresion opUnico, Entorno ent)
        {


            // OPERADOR FALSO, LA ETIQUETA FALSA Y VERDADERA DE ESTE OPERANDO SON LOS MISMOS DEL PADRES
            // POR ESO QUE SOLO SE COPIAN DE LA OPERACIÓN ACTUAL
            opUnico.etiquetaVerdadera = etiquetaFalsa; 
            opUnico.etiquetaFalsa = etiquetaVerdadera;
            result3D resultUnico = opUnico.obtener3D(ent);

            result3D resultado = new result3D();
            string er = "Eror tipo  not " + resultUnico.TipoResultado;

            if (resultUnico.TipoResultado != TipoDatos.Boolean )
            {
                Program.getIntefaz().agregarError(er, opUnico.linea, opUnico.columna);
                return resultado;
            }



            if (resultUnico.EtiquetaF.Equals("") && resultUnico.EtiquetaV.Equals(""))
            {
                string etiquetaV = opUnico.etiquetaVerdadera;
                string etiquetaF = opUnico.etiquetaFalsa;

                resultado.Codigo = resultUnico.Codigo; 
                resultado.Codigo += "if (" + resultUnico.Temporal + " == 1 ) goto " + etiquetaV + "; \n";
                resultado.Codigo += "goto " + etiquetaF + "; \n";

                resultUnico.EtiquetaV = etiquetaV;
                resultUnico.EtiquetaF = etiquetaF;
            }
            else
            {
                // SI ES UNA RELACION QUE GENERO EL BOOLEANO, SOLO SE PEGA EL CODIGO
                resultado.Codigo = resultUnico.Codigo + "\n";
            }


            resultado.EtiquetaV = resultUnico.EtiquetaF;
            resultado.EtiquetaF = resultUnico.EtiquetaV;
            resultado.TipoResultado = TipoDatos.Boolean;

            return resultado;
        }

        public string stringOp()
        {

            switch (this.operador)
            {
                case Operador.MAS:
                    return "+";
                case Operador.MENOS:
                    return "-";
                case Operador.MULTIPLICACION:
                    return "*";
                case Operador.DIVISION:
                    return "/";
                case Operador.MODULO:
                    return "mod";
                case Operador.MENOR:
                    return "<";
                case Operador.MENOR_QUE:
                    return "<=";
                case Operador.MAYOR:
                    return ">";
                case Operador.MAYOR_QUE:
                    return ">=";
                case Operador.IGUAL :
                    return "==";
                case Operador.AND:
                    return "&&";
                case Operador.OR:
                    return " || ";
                case Operador.NOT:
                    return "!";
                case Operador.DIFERENTE:
                    return "!=";

                default:
                    return "";
            }

        }


        private string SumarCadena(Expresion expr, result3D resultExpr)
        {
            string codigo = "";

            if(expr is Identificador || expr is Primitivo)
            {

                if(resultExpr.TipoResultado == TipoDatos.String)
                {
                    //EL TEMPORAL DE "resulExpr" CONTIENE LA POSICION EN EL HEAP DONDE COMIENZA EL STRING

                    string EtiquetaCiclo = Generador.pedirEtiqueta();
                    string EtiquetaSalida = Generador.pedirEtiqueta();
                    string CARACTER = Generador.pedirTemporal();


                    codigo += $"{EtiquetaCiclo}: /*** Etiqueta para ciclado de lectura ***/ \n\n";
                    codigo += $"    {CARACTER} = Heap[(int){resultExpr.Temporal}];   /*Capturamos el caracter a copiar*/\n\n";

                    codigo += $"    if({CARACTER}==0) goto {EtiquetaSalida}; /*Comparamos si ya se a llegado al final de la cadena */\n\n";
                    codigo += $"        Heap[HP] = {CARACTER}; /* Copiamos el caracter en la ultima posicion del HEAP, donde vamos*/\n";

                    codigo += $"            {resultExpr.Temporal} = {resultExpr.Temporal}+1 ;  /*Aumentamos el contador para seguir leyendo los caracteres*/\n";
                    codigo += $"            HP = HP + 1;\n";

                    codigo += $"            goto {EtiquetaCiclo}; /*Regresamos al inicio del ciclo para seguir leyendo*/\n";
                    codigo += $"{EtiquetaSalida}: \n\n";

                }
                else if( resultExpr.TipoResultado == TipoDatos.Integer)
                {

                }
            
            }


            return codigo;
        }


        public Expresion comprobarTipo(Expresion op )
        {

            if (op is Identificador)
            {
                return new Operacion(op, new Primitivo(1, op.linea, op.columna), Operador.IGUAL, op.linea, op.columna);
            }

            return op;
        }

        public void obtenerListasAnidadas(LinkedList<string> variablesUsadas)
        {
            if (operandoU != null)
            {
                operandoU.obtenerListasAnidadas(variablesUsadas);
            }
            else
            {

                operando1.obtenerListasAnidadas(variablesUsadas);
                operando2.obtenerListasAnidadas(variablesUsadas);
            }
        }
    }
}
