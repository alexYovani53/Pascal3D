using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using System;
using System.Collections.Generic;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.valoreImplicito
{

    /*
     * @class               Operacion
     * @Comentario          Esta clase sera util para construir todo tipo de operaciones en el lenguaje
     *                      ya sea: aritmeticas, logicas y/o relacionales. 
     */

    public class Operacion : Expresion
    {

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

        private object val;
        public int linea { get; set; }
        public int columna { get; set; }

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
                case "DIV":
                    return Operador.DIVISION;
                case "%":
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

        public TipoDatos getTipo(Entorno entorno, AST ast)
        {
            object valor;
            if(val != null)
            {
                valor = val;
            }
            else
            {
                valor = getValorImplicito(entorno, ast);
            }


            if(valor is string)
            {
                return TipoDatos.String;
            }
            else if (valor is char)
            {
                return TipoDatos.Char;
            }
            else if (valor is bool)
            {
                return TipoDatos.Boolean;
            }
            else if (valor is int)
            {
                return TipoDatos.Integer;
            }
            else if (valor is double)
            {
                return TipoDatos.Real;
            }

            return TipoDatos.String;

        }


        public object getValorImplicito(Entorno ent, AST arbol)
        {


            object op1 = new object(), op2 = new object(), opU = new object();
           


            switch (operador)
            {

                case Operador.MAS:

                    if (op1 is int && op2 is double)         val = (int)op1 + (double)op2;
                    else if (op1 is double && op2 is int)    val = (double)op1 + (int)op2;
                    else if (op1 is int && op2 is int) val = (int)op1 + (int)op2;
                    else if (op1 is double && op2 is double) val = (double)op1 + (double)op2;
                    else if (op1 is string || op2 is string) val = op1.ToString() + op2.ToString();
                    else
                    {
                       
                    }

                    break;

                case Operador.MENOS:

                    if(operandoU != null)
                    {
                        if (opU is double)  val = -1 * (double)opU;
                        else if (opU is int) val = -1 * (int)opU;
                        else
                        {
                           
                        }
                        break;
                    }

                    if (op1 is int && op2 is double) val = (int)op1 - (double)op2;
                    else if (op1 is double && op2 is int) val = (double)op1 - (int)op2;
                    else if (op1 is int && op2 is int) val = (int)op1 - (int)op2;
                    else if (op1 is double && op2 is double) val = (double)op1 - (double)op2;
                    else
                    {
                       
                    }

                    break;

                case Operador.MULTIPLICACION:

                    if (op1 is int && op2 is double) val = (int)op1 * (double)op2;
                    else if (op1 is double && op2 is int) val = (double)op1 * (int)op2;
                    else if (op1 is int && op2 is int) val = (int)op1 * (int)op2;
                    else if (op1 is double && op2 is double) val = (double)op1 * (double)op2;
                    else
                    {
                        
                    }

                    break;

                case Operador.DIVISION:


                    if (op1 is int && op2 is double)
                    {
                        if((double)op2 !=0.0) val = (int)op1 / (double)op2;
                        else
                        {
                         
                        }
                    }
                    else if (op1 is double && op2 is int)
                    {
                        if ((int)op2 != 0) val = (double)op1 / (int)op2;
                        else
                        {
                        }
                    }
                    else if (op1 is int && op2 is int)
                    {
                        if ((int)op2 != 0)
                        {
                            double p = 0.0;
                            p += (int)op1; 
                            double pivote = p / (int)op2;
                            val = pivote;
                        }
                        else
                        {
                        }
                    }
                    else if (op1 is double && op2 is double)
                    {
                        if ((double)op2 != 0.0) val = (double)op1 / (double)op2;
                        else
                        {
                        }
                    }
                    else
                    {
                    }

                    break;

                case Operador.MODULO:

                    if (op1 is int && op2 is double) val = (int)op1 % (double)op2;
                    else if (op1 is double && op2 is int) val = (double)op1 % (int)op2;
                    else if (op1 is int && op2 is int) val = (int)op1 % (int)op2;
                    else if (op1 is double && op2 is double) val = (double)op1 % (double)op2;
                    else
                    {
                    }

                    break;


                case Operador.IGUAL:
                    if (op1 is int && op2 is double)            val = (int)op1 == (double)op2;
                    else if (op1 is double && op2 is int)       val = (double)op1 == (int)op2;
                    else if (op1 is int && op2 is int)          val = (int)op1 == (int)op2;
                    else if (op1 is double && op2 is double)    val = (double)op1 == (double)op2;
                    else if (op1 is string && op2 is string)    val = op1.ToString().Equals(op2.ToString());
                    else if (op1 is char && op2 is char)        val = op1.ToString().Equals(op2.ToString());
                    else if (op1 is bool && op2 is bool)        val = (bool)op1 == (bool)op2;
                    break;


                case Operador.MENOR:
                    if (op1 is int && op2 is double)            val = (int)op1 < (double)op2;
                    else if (op1 is double && op2 is int)       val = (double)op1 < (int)op2;
                    else if (op1 is int && op2 is int)          val = (int)op1 < (int)op2;
                    else if (op1 is double && op2 is double)    val = (double)op1 < (double)op2;

                    break;


                case Operador.MAYOR:
                    if      (op1 is int && op2 is double)       val = (int)op1 > (double)op2;
                    else if (op1 is double && op2 is int)       val = (double)op1 > (int)op2;
                    else if (op1 is int && op2 is int)          val = (int)op1 > (int)op2;
                    else if (op1 is double && op2 is double)    val = (double)op1 > (double)op2;

                    break;

                case Operador.MAYOR_QUE:
                    if      (op1 is int && op2 is double)       val = (int)op1 >= (double)op2;
                    else if (op1 is double && op2 is int)       val = (double)op1 >= (int)op2;
                    else if (op1 is int && op2 is int)          val = (int)op1 >= (int)op2;
                    else if (op1 is double && op2 is double)    val = (double)op1 >= (double)op2;
                    break;

                case Operador.MENOR_QUE:
                    if (op1 is int && op2 is double)            val = (int)op1 <= (double)op2;
                    else if (op1 is double && op2 is int)    val = (double)op1 <= (int)op2;
                    else if (op1 is int && op2 is int)          val = (int)op1 <= (int)op2;
                    else if (op1 is double && op2 is double)    val = (double)op1 <= (double)op2;
                    break;


                case Operador.DIFERENTE:
                    if (op1 is int && op2 is double)            val = (int)op1 != (double)op2;
                    else if (op1 is double && op2 is int)       val = (double)op1 != (int)op2;
                    else if (op1 is int && op2 is int)          val = (int)op1 != (int)op2;
                    else if (op1 is double && op2 is double)    val = (double)op1 != (double)op2;
                    break;


                case Operador.AND:
                    if (op1 is bool && op2 is bool) val = (bool)op1 && (bool)op2;
                    else
                    {
                        return null;
                    }break;

                case Operador.OR:
                    if (op1 is bool && op2 is bool) val = (bool)op1 || (bool)op2;
                    else
                    {
                        return null;
                    }break;

                case Operador.NOT:
                    if (opU is bool)
                    {
                        val = !((bool)opU);
                    }
                    else
                    {
                        return null;
                    }
                    break;



                default:
                    break;

            }


            return val;


        }

        string NodoAST.getC3()
        {
            throw new NotImplementedException();
        }
    }
}
