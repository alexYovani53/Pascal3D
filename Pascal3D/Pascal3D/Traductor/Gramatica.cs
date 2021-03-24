using System;
using System.Collections.Generic;
using System.Text;
using Irony.Parsing;
//esta es la referencia para la herramienta de irony




namespace CompiPascal.compilador
{


    class Gramatica : Irony.Parsing.Grammar

    {

        public Gramatica() : base(false)
        {

            #region Declaracion de terminales
            /* En esta region se declararan los terminales, es decir, los nodos hoja del AST,
             * esta región contendrá:
             *  1. Palabras reservadas
             *  2. Operadores y simbolos
             *  3. Terminales definidos con una expresion regular
             *  4. Comentarios
            */

            #region Palabras reservadas

            KeyTerm pr_entero = ToTerm("integer"),
                pr_real = ToTerm("real"),
                pr_string = ToTerm("string"),
                pr_char = ToTerm("char"),
                pr_boolean = ToTerm("boolean"),
                pr_void = ToTerm("void"),
                pr_type = ToTerm("type"),
                pr_end = ToTerm("end"),
                pr_begin = ToTerm("begin"),
                pr_object = ToTerm("object"),
                pr_program = ToTerm("program"),
                pr_var = ToTerm("var"),
                pr_constante = ToTerm("const"),
                pr_if = ToTerm("if"),
                pr_else = ToTerm("else"),
                pr_then = ToTerm("then"),
                pr_for = ToTerm("for"),
                pr_to = ToTerm("to"),
                pr_down = ToTerm("downto"),
                pr_do = ToTerm("do"),
                pr_while = ToTerm("while"),
                pr_case = ToTerm("case"),
                pr_array = ToTerm("array"),
                pr_of = ToTerm("of"),
                pr_repeat = ToTerm("repeat"),
                pr_until = ToTerm("until"),
                pr_break = ToTerm("break"),
                pr_continue = ToTerm("continue"),
                pr_function = ToTerm("function"),
                pr_procedure = ToTerm("procedure"),
                pr_writeln = ToTerm("writeln"),
                pr_write = ToTerm("write"),
                pr_exit = ToTerm("exit"),
                pr_graficar = ToTerm("graficar_ts");

            /* El metodo "MarkReservedWords" le dice al parser que Terminales seran palabras reservadas,
             * esto para que tengan prioridad sobre los identificadores, de lo contrario las palabras reservadas,
             * se tomarian como identificadores.            
            */
            MarkReservedWords("integer", "real", "string", "const", "boolean", "void", "type", "end", "begin", "object",
                "program", "var", "if", "else", "then", "for", "do", "while", "case", "array", "of", "repeat",
                "until", "break", "continue", "function", "procedure", "writeln", "write", "exit", "graficar_ts");

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
                signo_mod = ToTerm("mod"),
                signo_div = ToTerm("/"),
                div_div = ToTerm("div"),
                diferente_a = ToTerm("<>"),
                menor_igual = ToTerm("<="),
                menor = ToTerm("<"),
                mayor_igual = ToTerm(">="),
                mayor = ToTerm(">"),
                pr_or = ToTerm("and"),
                pr_and = ToTerm("or"),
                pr_not = ToTerm("not"),
                pr_true = ToTerm("true", "true"),
                pr_false = ToTerm("false", "false"),
                signo_igual = ToTerm("=");


            #endregion

            #region Terminales definidos con una expresion regular
            /* Tipo de literal que reconoce numeros equivalente a 
             * RegexBasedTerminal entero = new RegexBasedTerminal("entero", "[0-9]+");
             * RegexBasedTerminal decimal = new RegexBasedTerminal("decimal", "[0-9]+(.[0-9]+)?");
             * Pero mas completo porque reconoce gran variedad de tipos de números, 
             * desde enteros simples (por ejemplo, 1) hasta decimales (por ejemplo, 1.0) 
             * hasta números expresados ​​en notación científica (por ejemplo, 1.1e2).
             */

            NumberLiteral numero = new NumberLiteral("numero");
            /*
             *Identifica los nombres de variables, permitiendo al inicio el uso de _, de intermedio algun numero
             *y caracteres
            */
            IdentifierTerminal identificador = new IdentifierTerminal("identificador");


            //RegexBasedTerminal cadena = new RegexBasedTerminal("cadena", "\'[^\']*\'");
            StringLiteral cadena = new StringLiteral("cadena", "\'", StringOptions.IsTemplate);
            RegexBasedTerminal caracter = new RegexBasedTerminal("caracter", "\'.\'");

            #endregion

            #region Comentarios
            /* Se procede a definir los comentarios, los mismos seran ignorados por el analizador.
             * Constructor: CommentTerminal(<nombre>,<SimboloInicial>,<Simbolo(s)Final(es)>)
             * Un comentario podria terminar con uno o mas simbolos.  
             * Luego de definirlo es necesario agregar a la lista de terminales que seran ignorados, 
             * es decir, que no formaran parte de la gramatica con el metodo: NonGrammarTerminals.Add(<terminal>)
            */

            CommentTerminal comentarioUnaLinea = new CommentTerminal("comentario_una_linea", "//", "\n", "\r\n");
            CommentTerminal comentarioMultilinea_1 = new CommentTerminal("comentarioMultiLinea_1", "(*", "*)");
            CommentTerminal comentarioMultilinea_2 = new CommentTerminal("comentarioMultiLinea_2", "{", "}");

            base.NonGrammarTerminals.Add(comentarioUnaLinea);
            base.NonGrammarTerminals.Add(comentarioMultilinea_1);
            base.NonGrammarTerminals.Add(comentarioMultilinea_2);
            #endregion

            #endregion

            #region Declaracion de NO TERMINALES
            // En esta region se declararan los no terminales, es decir, los nodos intermedios del AST,

            NonTerminal INI = new NonTerminal("INI");
            NonTerminal ENCABEZADOS = new NonTerminal("ENCABEZADOS");
            NonTerminal _ENCABEZADO = new NonTerminal("_ENCABEZADO");
            NonTerminal VAR_SEC = new NonTerminal("VAR_SEC");
            NonTerminal DECLARACIONES = new NonTerminal("DECLARACIONES");
            NonTerminal _DECLARACION = new NonTerminal("_DECLARACION");
            NonTerminal DECLARACION_SIN_INICIAR = new NonTerminal("DECLARACION_SIN_INICIAR");
            NonTerminal DECLARACION_INICIALIZACION = new NonTerminal("DECLARACION_INICIALIZACION");
            NonTerminal LIST_NOMBRES = new NonTerminal("LIST_NOMBRES");
            NonTerminal CONST_SEC = new NonTerminal("CONST_SEC");
            NonTerminal CONSTANTES = new NonTerminal("CONSTANTES");
            NonTerminal _CONSTANTE = new NonTerminal("_CONSTANTE");
            NonTerminal SEC_TYPES = new NonTerminal("SEC_TYPES");
            NonTerminal CUERPO_STRUCT = new NonTerminal("CUERPO_STRUCT");
            NonTerminal CUERPO_ARRAY = new NonTerminal("CUERPO_ARRAY");
            NonTerminal NIVEL_ARRAY = new NonTerminal("NIVEL_ARRAY");
            NonTerminal NIVEL = new NonTerminal("NIVEL");
            NonTerminal ACCESO_ARR = new NonTerminal("ACCESO_ARR");
            NonTerminal INDICE2 = new NonTerminal("INDICE2");
            NonTerminal INDICES = new NonTerminal("INDICES");
            NonTerminal INDICE = new NonTerminal("INDICE");
            NonTerminal SENTENCIAS = new NonTerminal("SENTENCIAS");
            NonTerminal _SENTENCIA = new NonTerminal("_SENTENCIA");
            NonTerminal _SENTENCIA2 = new NonTerminal("_SENTENCIA2");
            NonTerminal SENTENCIA_ACCESO = new NonTerminal("SENTENCIA_ACCESO");
            NonTerminal LISTA_ACCESO = new NonTerminal("LISTA_ACCESO");
            NonTerminal SENTENCIA_LLAMADA = new NonTerminal("SENTENCIA_LLAMADA");
            NonTerminal PARAMS = new NonTerminal("PARAMS");
            NonTerminal SENTENCIA_BREAK = new NonTerminal("SENTENCIA_BREAK");
            NonTerminal SENTENCIA_CONTINUE = new NonTerminal("SENTENCIA_CONTINUE");
            NonTerminal SENTENCIA_ASIGNAR = new NonTerminal("SENTENCIA_ASIGNAR");
            NonTerminal VARIABLE_ASIGNAR = new NonTerminal("VARIABLE_ASIGNAR");
            NonTerminal SENTENCIA_WRITE = new NonTerminal("SENTENCIA_WRITE");
            NonTerminal SENTENCIA_ASIG_ARR = new NonTerminal("SENTENCIA_ASIG_ARR");
            NonTerminal EXPR_IMPRIMIR = new NonTerminal("EXPR_IMPRIMIR");
            NonTerminal EXPR_FORMATEO = new NonTerminal("EXPR_FORMATEO");
            NonTerminal SENTENCIA_GRAFICAR_TS = new NonTerminal("SENTENCIA_GRAFICAR_TS");
            NonTerminal SENTENCIA_IF = new NonTerminal("SENTENCIA_IF");
            NonTerminal SENTENCIA_EXIT = new NonTerminal("SENTENCIA_EXIT");
            NonTerminal ELSE_IF = new NonTerminal("ELSE_IF");
            NonTerminal SENTENCIA_CASE = new NonTerminal("SENTENCIA_CASE");
            NonTerminal CASOS = new NonTerminal("CASOS");
            NonTerminal _CASO = new NonTerminal("_CASO");
            NonTerminal SENTENCIA_CASOS = new NonTerminal("SENTENCIA_CASOS");
            NonTerminal SENTENCIA_FOR = new NonTerminal("SENTENCIA_FOR");
            NonTerminal SENTENCIA_REPEAT = new NonTerminal("SENTENCIA_REPEAT");
            NonTerminal SENTENCIA_WHILE = new NonTerminal("SENTENCIA_WHILE");
            NonTerminal EXPR = new NonTerminal("EXPR");
            NonTerminal EXPR_RELACIONAL = new NonTerminal("EXPR_RELACIONAL");
            NonTerminal EXPR_LOGICA = new NonTerminal("EXPR_LOGICA");
            NonTerminal EXPR_ARITMETICA = new NonTerminal("EXPR_ARITMETICA");
            NonTerminal PRIMITIVO = new NonTerminal("PRIMITIVO");
            NonTerminal TIPO_DATO = new NonTerminal("TIPO_DATO");
            NonTerminal VAL_INICIAL = new NonTerminal("VAL_INICIAL");
            NonTerminal FUNCION = new NonTerminal("FUNCION");
            NonTerminal _PROCEDIMIENTO = new NonTerminal("_PROCEDIMIENTO");
            NonTerminal LIST_PARAMS_FUNCION_PROC = new NonTerminal("LIST_PARAMS_FUNCION_PROC");
            NonTerminal PARAMETRO = new NonTerminal("PARAMETRO");
            NonTerminal PRINCIPAL = new NonTerminal("PRINCIPAL");
            NonTerminal BLOQUE = new NonTerminal("BLOQUE");
            NonTerminal BLOQUE_ELSE = new NonTerminal("BLOQUE_ELSE");

            #endregion



            #region Gramatica

            /*             ═══════════════ IMPORTANTE══════════════════
             * En la notación BNF tradicional, los caracteres "?", "+" Y "*" 
             * se utilizan para indicar "0 o 1 vez", "1 o más veces" y "0 o más veces", respectivamente. 
             * En Irony, se hace de forma ligeramente diferente. Utilice los métodos 
             * MakePlusRule y MakeStarRule de la clase Grammar base para "+" y "*" o 
             * puede utilizar los métodos Q (), Plus () y Star () directamente en el término dentro de la regla.
             */


            INI.Rule = pr_program + identificador + ptcoma + ENCABEZADOS + PRINCIPAL;
                      /* |pr_program + identificador + ptcoma  + PRINCIPAL;
                      * al utilizar makeStarRule se le dice al compilador que puede venir 0 o más meves
                      * el encabezado. ya que no siempre se necesita definir variables
                      */
            ENCABEZADOS.Rule = MakeStarRule(ENCABEZADOS, _ENCABEZADO);

            _ENCABEZADO.Rule =  VAR_SEC
                               | CONST_SEC
                               | SEC_TYPES
                               | FUNCION
                               | _PROCEDIMIENTO;

            //SECCION DE VARIABLES
            VAR_SEC.Rule = pr_var + DECLARACIONES;
                                    
            DECLARACIONES.Rule = MakePlusRule(DECLARACIONES, _DECLARACION);

            _DECLARACION.Rule = DECLARACION_SIN_INICIAR + ptcoma
                                | DECLARACION_INICIALIZACION + ptcoma;
                                


            DECLARACION_SIN_INICIAR.Rule = LIST_NOMBRES + doble_pt + TIPO_DATO
                                            | LIST_NOMBRES + doble_pt + pr_array + CUERPO_ARRAY
                                            | LIST_NOMBRES + doble_pt + identificador;
                                            
            DECLARACION_INICIALIZACION.Rule = LIST_NOMBRES + doble_pt + TIPO_DATO + VAL_INICIAL;

            LIST_NOMBRES.Rule = MakePlusRule(LIST_NOMBRES,coma, identificador);

            //SECCION DE CONSTANTES
            CONST_SEC.Rule = pr_constante + CONSTANTES;

            CONSTANTES.Rule = MakePlusRule(CONSTANTES, _CONSTANTE);

            _CONSTANTE.Rule = identificador + VAL_INICIAL + ptcoma;

            //SECCION DE TYPES
            SEC_TYPES.Rule = pr_type + identificador + signo_igual + pr_object + CUERPO_STRUCT
                            |pr_type + identificador + signo_igual + pr_array + CUERPO_ARRAY + ptcoma;

            CUERPO_STRUCT.Rule = ENCABEZADOS + pr_end + ptcoma
                                | pr_end + ptcoma;

            //ARREGLOS 
            CUERPO_ARRAY.Rule = lla_abierto + NIVEL_ARRAY + lla_cerrado + pr_of + TIPO_DATO 
                                |lla_abierto + NIVEL_ARRAY + lla_cerrado + pr_of + identificador ;

            NIVEL_ARRAY.Rule = MakePlusRule(NIVEL_ARRAY,coma,NIVEL);

            NIVEL.Rule = EXPR + pt + pt + EXPR;

            //ACCESO ARREGLO
            ACCESO_ARR.Rule = SENTENCIA_ACCESO + INDICES + pt  + SENTENCIA_ACCESO
                              |SENTENCIA_ACCESO + INDICES;

            INDICES.Rule = lla_abierto + PARAMS + lla_cerrado
                          | INDICE2;

            INDICE2.Rule = MakePlusRule(INDICE2, INDICE);

            INDICE.Rule = lla_abierto + EXPR + lla_cerrado;



            //SENTENCIAS
            SENTENCIAS.Rule = MakeStarRule(SENTENCIAS, _SENTENCIA);

            _SENTENCIA.Rule = SENTENCIA_IF + ptcoma
                                | SENTENCIA_CASE  + ptcoma
                                | SENTENCIA_WHILE   + ptcoma
                                | SENTENCIA_REPEAT + ptcoma
                                | SENTENCIA_FOR + ptcoma
                                | SENTENCIA_ACCESO + ptcoma
                                | SENTENCIA_LLAMADA + ptcoma
                                | SENTENCIA_BREAK + ptcoma
                                | SENTENCIA_CONTINUE + ptcoma
                                | SENTENCIA_ASIGNAR + ptcoma
                                | SENTENCIA_WRITE + ptcoma
                                | SENTENCIA_EXIT + ptcoma
                                | SENTENCIA_ASIG_ARR + ptcoma
                                | SENTENCIA_GRAFICAR_TS + ptcoma;

            _SENTENCIA.ErrorRule = SyntaxError + ";";


            /*BLOQUE.Rule = pr_begin + _SENTENCIA2 + pr_end
                              | pr_begin + SENTENCIAS + pr_end
                              | pr_begin + pr_end
                              | _SENTENCIA2;*/

            BLOQUE.Rule = pr_begin + _SENTENCIA2 + pr_end
                          | pr_begin + SENTENCIAS + pr_end
                          | pr_begin + pr_end
                          | SENTENCIA_IF
                          | _SENTENCIA2;


            BLOQUE_ELSE.Rule = pr_begin + _SENTENCIA2 + pr_end
                              | pr_begin + SENTENCIAS + pr_end
                              | pr_begin + pr_end
                              | _SENTENCIA2;






            _SENTENCIA2.Rule =  SENTENCIA_CASE
                                | SENTENCIA_WHILE 
                                | SENTENCIA_REPEAT
                                | SENTENCIA_FOR
                                | SENTENCIA_ACCESO
                                | SENTENCIA_LLAMADA
                                | SENTENCIA_BREAK 
                                | SENTENCIA_CONTINUE 
                                | SENTENCIA_ASIGNAR
                                | SENTENCIA_EXIT
                                | SENTENCIA_WRITE
                                | SENTENCIA_ASIG_ARR
                                | SENTENCIA_GRAFICAR_TS;

            _SENTENCIA2.ErrorRule = SyntaxError + ";";

            //SENTENCIA DE ACCESO
            /*SENTENCIA_ACCESO.Rule = identificador + pt + LISTA_ACCESO;

            LISTA_ACCESO.Rule = MakeStarRule(LISTA_ACCESO, pt, identificador);*/

            SENTENCIA_ACCESO.Rule = LISTA_ACCESO;

            LISTA_ACCESO.Rule = MakePlusRule(LISTA_ACCESO, pt, identificador);

            //SENTENCIA LLAMDA
            SENTENCIA_LLAMADA.Rule = identificador + par_abierto + PARAMS + par_cerrado ;
            PARAMS.Rule = MakeStarRule(PARAMS,coma, EXPR);

            //SENTENCIA BREAK
            SENTENCIA_BREAK.Rule = pr_break ;

            //SENTENCIA CONTINUE
            SENTENCIA_CONTINUE.Rule = pr_continue;

            //SENTENCIA ASIGNAR
            SENTENCIA_ASIGNAR.Rule = VARIABLE_ASIGNAR + doble_pt + signo_igual + EXPR ;

            VARIABLE_ASIGNAR.Rule = SENTENCIA_ACCESO;

            /*VARIABLE_ASIGNAR.Rule = SENTENCIA_ACCESO
                        | identificador;*/

            //SENTENCIA ASIGNAR ARREGLO
            SENTENCIA_ASIG_ARR.Rule = SENTENCIA_ACCESO + INDICES + doble_pt + signo_igual + EXPR;

            //SENTENCIA WRITE
            SENTENCIA_WRITE.Rule = pr_writeln + par_abierto + EXPR_IMPRIMIR + par_cerrado
                                    | pr_writeln
                                    | pr_write + par_abierto + EXPR_IMPRIMIR + par_cerrado;



            EXPR_IMPRIMIR.Rule = MakePlusRule(EXPR_IMPRIMIR, coma, EXPR_FORMATEO);

            EXPR_FORMATEO.Rule = EXPR + doble_pt + EXPR + doble_pt + EXPR
                                | EXPR + doble_pt + EXPR
                                | EXPR;

            //SENTENCIA GRAFICAR
            SENTENCIA_GRAFICAR_TS.Rule = pr_graficar;


            //SENTENCIA IF
            SENTENCIA_IF.Rule =   pr_if + EXPR + pr_then + BLOQUE_ELSE
                                | pr_if + EXPR + pr_then + BLOQUE_ELSE + pr_else + ELSE_IF
                                | pr_if + EXPR + pr_then + BLOQUE_ELSE + pr_else + BLOQUE_ELSE;

            ELSE_IF.Rule =       pr_if + EXPR + pr_then + BLOQUE_ELSE
                                | pr_if + EXPR + pr_then + BLOQUE_ELSE + pr_else + ELSE_IF
                                | pr_if + EXPR + pr_then + BLOQUE_ELSE + pr_else + BLOQUE_ELSE;

            //SENTENCIA EXIT
            SENTENCIA_EXIT.Rule = pr_exit + par_abierto + EXPR + par_cerrado
                                  | pr_exit+ par_abierto + par_cerrado
                                  | pr_exit;
    

            //SENTENCIA CASE

            SENTENCIA_CASE.Rule = pr_case + EXPR + pr_of + CASOS + pr_end
                                | pr_case + EXPR + pr_of + CASOS + pr_else +SENTENCIA_CASOS +  pr_end ;


            CASOS.Rule = MakePlusRule(CASOS, _CASO);

            _CASO.Rule = PRIMITIVO + doble_pt + SENTENCIA_CASOS;

            SENTENCIA_CASOS.Rule = _SENTENCIA 
                                    | pr_begin + SENTENCIAS + pr_end + ptcoma;

            //SENTENCIA FOR
            SENTENCIA_FOR.Rule = pr_for + SENTENCIA_ASIGNAR + pr_down   + EXPR + pr_do + BLOQUE
                                |pr_for + SENTENCIA_ASIGNAR + pr_to + EXPR + pr_do + BLOQUE;
            //SENTENCIA REPEAT
            SENTENCIA_REPEAT.Rule = pr_repeat + SENTENCIAS + pr_until + EXPR ;

            //SENTENCIA WHILE
            SENTENCIA_WHILE.Rule = pr_while + EXPR + pr_do + BLOQUE ;


            EXPR.Rule = EXPR_LOGICA
                        | EXPR_ARITMETICA
                        | EXPR_RELACIONAL
                        | SENTENCIA_ACCESO
                        | PRIMITIVO
                        | SENTENCIA_LLAMADA
                        | ACCESO_ARR 
                        | par_abierto + EXPR + par_cerrado;


            EXPR_RELACIONAL.Rule = EXPR + mayor_igual + EXPR
                                    | EXPR + menor_igual + EXPR
                                    | EXPR + mayor + EXPR
                                    | EXPR + menor + EXPR
                                    | EXPR + signo_igual + EXPR
                                    | EXPR + diferente_a + EXPR;

            EXPR_ARITMETICA.Rule =  umenos + EXPR 
                                    | EXPR + signo_mas + EXPR
                                    | EXPR + signo_menos + EXPR
                                    | EXPR + signo_por + EXPR
                                    | EXPR + signo_div + EXPR
                                    | EXPR + div_div + EXPR
                                    | EXPR + signo_mod + EXPR;
                                    


            EXPR_LOGICA.Rule = EXPR + pr_or + EXPR
                                | EXPR + pr_and + EXPR
                                | pr_not + EXPR;


            PRIMITIVO.Rule = numero
                            | cadena
                            | caracter
                            | pr_true
                            | pr_false
                            | identificador;


            TIPO_DATO.Rule = pr_string
                                | pr_char
                                | pr_entero
                                | pr_real
                                | pr_boolean;

            //VALOR INICIAL
            VAL_INICIAL.Rule = signo_igual + EXPR;

            //FUNCIONES

            FUNCION.Rule = pr_function + identificador + par_abierto + LIST_PARAMS_FUNCION_PROC + par_cerrado + doble_pt + TIPO_DATO + ptcoma + ENCABEZADOS + BLOQUE + ptcoma
                            | pr_function + identificador + par_abierto + LIST_PARAMS_FUNCION_PROC + par_cerrado + doble_pt + identificador + ptcoma + ENCABEZADOS + BLOQUE + ptcoma;

            //PROCEDIMIENTOS
            //PROCEDIMIENTOS.Rule = MakePlusRule(PROCEDIMIENTOS, _PROCEDIMIENTO);

            _PROCEDIMIENTO.Rule = pr_procedure + identificador + par_abierto + LIST_PARAMS_FUNCION_PROC + par_cerrado + ptcoma + ENCABEZADOS + BLOQUE + ptcoma;

            LIST_PARAMS_FUNCION_PROC.Rule = MakeStarRule(LIST_PARAMS_FUNCION_PROC, ptcoma, PARAMETRO);
                               
            PARAMETRO.Rule = LIST_NOMBRES + doble_pt + TIPO_DATO
                             | LIST_NOMBRES + doble_pt + identificador
                             | pr_var + LIST_NOMBRES + doble_pt + TIPO_DATO
                             | pr_var + LIST_NOMBRES + doble_pt + identificador;



            //************* ACA ESTA LA PRODUCCIÓN PARA EL CUERPO PRINCIPAL DEL PROGRAMA


            PRINCIPAL.Rule = pr_begin + SENTENCIAS + pr_end + pt;







            #endregion

            #region precedencia
            /*
             * En esta region se define la precedencia y asociatividad para remover la ambiguedad
             * de la gramatica de expresiones.             
            */  

            RegisterOperators(1, Associativity.Left, pr_and);
            RegisterOperators(2, Associativity.Left, pr_or);
            RegisterOperators(3, Associativity.Left, signo_igual, diferente_a);
            RegisterOperators(4, Associativity.Neutral, mayor, menor,mayor_igual,menor_igual);
            RegisterOperators(5, Associativity.Left, signo_mas, signo_menos);
            RegisterOperators(6, Associativity.Left, signo_por, signo_div,signo_mod);
            RegisterOperators(7, Associativity.Right, pr_not);
            RegisterOperators(8, Associativity.Right, umenos);
            RegisterOperators(9, Associativity.Neutral, par_abierto, par_cerrado);
            #endregion




            MarkPunctuation(par_abierto, par_cerrado, lla_abierto, lla_cerrado, ptcoma, doble_pt, cor_abierto, cor_cerrado);

            this.Root = INI;

        }

    }
}
