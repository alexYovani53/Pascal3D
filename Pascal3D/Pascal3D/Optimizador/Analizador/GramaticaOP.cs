using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pascal3D.Optimizador.Analizador
{
    class GramaticaOP: Irony.Parsing.Grammar
    {

        public GramaticaOP(): base(caseSensitive:false)
        {
            #region Palabras reservadas

            KeyTerm pr_entero = ToTerm("int"),
                pr_float = ToTerm("float"),
                pr_char = ToTerm("char"),
                pr_void = ToTerm("void"),
                reservada_include = ToTerm("#include"),
                reservada_lib = ToTerm("<stdio.h>"),
                pr_heap = ToTerm("Heap"),
                pr_stack = ToTerm("Stack"),
                pr_if = ToTerm("if"),
                pr_else = ToTerm("else"),
                pr_return = ToTerm("return"),
                pr_HP = ToTerm("HP"),
                pr_SP = ToTerm("SP"),
                pr_goto = ToTerm("goto"),
                pr_printf = ToTerm("printf");

            /* El metodo "MarkReservedWords" le dice al parser que Terminales seran palabras reservadas,
             * esto para que tengan prioridad sobre los identificadores, de lo contrario las palabras reservadas,
             * se tomarian como identificadores.            
            */
            MarkReservedWords("int", "float","void","if", "else","char","#include","<stdio.h>","Heap","Stack","return",
                "HP","SP","goto","printf");

            #endregion


            #region Operadores y simbolos

            Terminal ptcoma = ToTerm(";"),
                coma = ToTerm(","),
                pt = ToTerm("."),
                doble_pt = ToTerm(":"),
                par_abierto = ToTerm("("),
                par_cerrado = ToTerm(")"),
                cor_abierto = ToTerm("{"),
                cor_cerrado = ToTerm("}"),
                lla_abierto = ToTerm("["),
                lla_cerrado = ToTerm("]"),
                signo_mas = ToTerm("+"),
                signo_menos = ToTerm("-"),
                umenos = ToTerm("-"),
                signo_por = ToTerm("*"),
                signo_mod = ToTerm("%"),
                signo_div = ToTerm("/"),
                diferente_a = ToTerm("!="),
                menor_igual = ToTerm("<="),
                menor = ToTerm("<"),
                mayor_igual = ToTerm(">="),
                mayor = ToTerm(">"),
                signo_iguales = ToTerm("=="),
                signo_igual = ToTerm("=");

            #endregion


            #region Terminales definidos con una expresión regular

            
            RegexBasedTerminal Real = new RegexBasedTerminal("real", "[0-9]+(.[0-9]+)");
            NumberLiteral entero = new NumberLiteral("entero");

            RegexBasedTerminal Temporal = new RegexBasedTerminal("Temporal", "T[0-9]+");
            RegexBasedTerminal Label = new RegexBasedTerminal("Label", "L[0-9]+");
            Temporal.Priority = TerminalPriority.High;
            Temporal.Priority = TerminalPriority.High;

            IdentifierTerminal Id = new IdentifierTerminal("Id");
            Id.Priority = TerminalPriority.Low;

            StringLiteral cadena = new StringLiteral("cadena", "\"");


            #endregion


            #region Comentarios

            CommentTerminal ComentarioUniLinea = new CommentTerminal("comentario_uni_linea", "//", "\n", "\r\n","\r");
            CommentTerminal ComentarioMulti = new CommentTerminal("comentario_multilinea", "/*", "*/");

            NonGrammarTerminals.Add(ComentarioMulti);
            NonGrammarTerminals.Add(ComentarioUniLinea);

            #endregion

            NonTerminal INI = new NonTerminal("INI"); 
            NonTerminal ENCABEZADOS = new NonTerminal("ENCABEZADOS");
            NonTerminal TEMPORALES = new NonTerminal("TEMPORALES");
            NonTerminal LISTA_TEMPS = new NonTerminal("LISTA_TEMPS");
            NonTerminal FUNCIONES = new NonTerminal("FUNCIONES");
            NonTerminal FUNCION = new NonTerminal("FUNCION");
            NonTerminal INSTRUCCIONES = new NonTerminal("INSTRUCCIONES");
            NonTerminal INSTRUCCION = new NonTerminal("INSTRUCCION");

            NonTerminal ASIGNACION_STACK = new NonTerminal("ASIGNACION_STACK");
            NonTerminal ASIGNACION_HEAP = new NonTerminal("ASIGNACION_HEAP");

            NonTerminal ASIGNACION = new NonTerminal("ASIGNACION");
            NonTerminal EXPR_ASIG = new NonTerminal("EXPR_ASIG");

            NonTerminal GOTO = new NonTerminal("GOTO");
            NonTerminal IF = new NonTerminal("IF");
            NonTerminal RETURN = new NonTerminal("RETURN");
            NonTerminal LLAMADA = new NonTerminal("LLAMADA");
            NonTerminal PRINTF = new NonTerminal("PRINTF");
            NonTerminal TERM_PRINT = new NonTerminal("TERM_PRINT");
            NonTerminal DEF_LABEL = new NonTerminal("DEF_LABEL");

            NonTerminal OPERACION_IF = new NonTerminal("OPERACION_IF");

            NonTerminal EXPR = new NonTerminal("EXPR");
            NonTerminal TERMINAL = new NonTerminal("TERMINAL");
            NonTerminal ACCESO = new NonTerminal("ACCESO");

            NonTerminal CONVERSION = new NonTerminal("CONVERSION");



            #region Gramatica

            INI.Rule = ENCABEZADOS + TEMPORALES + FUNCIONES;

            ENCABEZADOS.Rule 
                = 
                reservada_include + reservada_lib + 
                pr_float + pr_heap + lla_abierto + entero + lla_cerrado + ptcoma +
                pr_float + pr_stack + lla_abierto + entero + lla_cerrado + ptcoma + 
                pr_entero + pr_SP + signo_igual + entero + ptcoma+ 
                pr_entero + pr_HP + signo_igual + entero + ptcoma;

            ENCABEZADOS.ErrorRule = SyntaxError + ptcoma;


            TEMPORALES.Rule = pr_float + LISTA_TEMPS+ptcoma;
            //TEMPORALES.Rule = pr_float + Temporal + LISTA_TEMPS + ptcoma;

            TEMPORALES.ErrorRule = SyntaxError + ptcoma;

            LISTA_TEMPS.Rule = MakePlusRule(LISTA_TEMPS, coma, Temporal);
            //LISTA_TEMPS.Rule
            //    =
            //    coma + Temporal + LISTA_TEMPS
            //    | Empty;


            FUNCIONES.Rule
                =
                FUNCION + FUNCIONES
                | Empty;

            FUNCION.Rule
                = pr_void + Id + par_abierto + par_cerrado + cor_abierto + INSTRUCCIONES + cor_cerrado
                | pr_void + Id + par_abierto + par_cerrado + cor_abierto +  cor_cerrado;

            INSTRUCCIONES.Rule = MakeStarRule (INSTRUCCIONES, INSTRUCCION);

            INSTRUCCION.Rule
                = IF
                | GOTO
                | RETURN
                | DEF_LABEL
                | LLAMADA
                | ASIGNACION_HEAP
                | ASIGNACION_STACK
                | ASIGNACION
                | PRINTF;

            INSTRUCCION.ErrorRule = SyntaxError + ptcoma;

            //      INSTRUCCION IF
            IF.Rule
                = pr_if + par_abierto + TERMINAL + OPERACION_IF + TERMINAL + par_cerrado + pr_goto + Label + ptcoma;

            OPERACION_IF.Rule
                = signo_iguales
                | menor_igual
                | menor
                | mayor_igual
                | mayor
                | diferente_a;

            //      INSTRUCCION GOTO    
            GOTO.Rule = pr_goto + Label + ptcoma;



            //      EXPRESION
            EXPR.Rule
                = TERMINAL + signo_mas + TERMINAL
                | TERMINAL + signo_menos + TERMINAL
                | TERMINAL + signo_mod + TERMINAL
                | TERMINAL + signo_por + TERMINAL
                | TERMINAL + signo_div + TERMINAL
                | TERMINAL
                | ACCESO;

            //      TERMINALES
            TERMINAL.Rule
                = Real
                | entero
                | Temporal
                | pr_SP
                | pr_HP
                | signo_menos + entero
                | signo_menos + Real;


            //      ACCESO A HEAP O STACK
            ACCESO.Rule
                = pr_heap + lla_abierto + CONVERSION + Temporal + lla_cerrado
                | pr_heap + lla_abierto + Temporal + lla_cerrado
                | pr_stack + lla_abierto + CONVERSION + Temporal + lla_cerrado
                | pr_stack + lla_abierto + Temporal + lla_cerrado;


            //      CASTEO 
            CONVERSION.Rule 
                = par_abierto + pr_entero + par_cerrado
                | par_abierto + pr_float + par_cerrado
                | par_abierto + pr_char + par_cerrado;


            //      INSTRUCCION RETURN
            RETURN.Rule
                = pr_return + EXPR + ptcoma
                | pr_return + ptcoma;

            //      DEFINICION DE ETIQUETA
            DEF_LABEL.Rule
                = Label + doble_pt;

            //      INSTRUCCION PRINT
            PRINTF.Rule
                = pr_printf + par_abierto + cadena + coma + TERM_PRINT + par_cerrado + ptcoma;

            //      TERMINO IMPRESION
            TERM_PRINT.Rule
                =  CONVERSION + Temporal
                | CONVERSION + entero;

            //      ASIGNACION TEMPS
            ASIGNACION.Rule = EXPR_ASIG + signo_igual + EXPR + ptcoma;

            //      LLAMADA
            LLAMADA.Rule = Id + par_abierto + par_cerrado + ptcoma;



            EXPR_ASIG.Rule
                = pr_SP
                | pr_HP
                | Temporal;


            //      ASIGNACION HEAP

            ASIGNACION_HEAP.Rule
                = pr_heap + lla_abierto + TERMINAL +  lla_cerrado + signo_igual + EXPR + ptcoma
                | pr_heap + lla_abierto + CONVERSION + TERMINAL + lla_cerrado + signo_igual + EXPR + ptcoma;

            ASIGNACION_STACK.Rule
                = pr_stack + lla_abierto + TERMINAL + lla_cerrado + signo_igual + EXPR + ptcoma
                | pr_stack + lla_abierto + CONVERSION+ TERMINAL + lla_cerrado + signo_igual + EXPR + ptcoma;

            #endregion


            //#region operadores 

            //RegisterOperators(1, Associativity.Left, signo_mas, signo_menos);
            //RegisterOperators(2, Associativity.Left, signo_por, signo_div);
            //RegisterOperators(3, Associativity.Left, signo_mod);
            //RegisterOperators(4, Associativity.Left, mayor, mayor_igual, menor, menor_igual);
            //RegisterOperators(5, Associativity.Left, signo_iguales, diferente_a);
            //RegisterOperators(6, Associativity.Left, par_abierto, par_cerrado);

            //#endregion


            #region precedencia
            /*
             * En esta region se define la precedencia y asociatividad para remover la ambiguedad
             * de la gramatica de expresiones.             
            */


            RegisterOperators(1, Associativity.Left, signo_igual, diferente_a);
            RegisterOperators(2, Associativity.Neutral, mayor, menor, mayor_igual, menor_igual);
            RegisterOperators(3, Associativity.Left, signo_mas, signo_menos);
            RegisterOperators(4, Associativity.Left, signo_por, signo_div, signo_mod);
            RegisterOperators(5, Associativity.Right, umenos);
            RegisterOperators(6, Associativity.Neutral, par_abierto, par_cerrado);
            #endregion


            this.Root = INI;

        }
    }
}
 