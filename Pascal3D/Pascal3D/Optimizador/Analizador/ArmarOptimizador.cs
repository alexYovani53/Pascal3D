using Irony.Parsing;
using Pascal3D.Optimizador.AsignacionOp;
using Pascal3D.Optimizador.ControlOp;
using Pascal3D.Optimizador.EtiquetasOp;
using Pascal3D.Optimizador.Funcion;
using Pascal3D.Optimizador.FuncionesPrimitivasOp;
using Pascal3D.Optimizador.Heap_Stack;
using Pascal3D.Optimizador.InterfacesOp;
using Pascal3D.Optimizador.PrimitivosOp;
using Pascal3D.Optimizador.SaltosOp;
using Pascal3D.Optimizador.ValorImplicitoOp;
using System;
using System.Collections.Generic;
using System.Text;
using static Pascal3D.Optimizador.ControlOp.IfOp;
using static Pascal3D.Optimizador.PrimitivosOp.OperacionOp;

namespace Pascal3D.Optimizador.Analizador
{
    public class ArmarOptimizador
    {

        public ArmarOptimizador()
        {

        }

        public AST_OP armarArbolOptimizar(ParseTree raiz)
        {
            return (AST_OP)recorrerArbol(raiz.Root);
        }

        public object recorrerArbol(ParseTreeNode nodo)
        {

            if (EstoyEnEsteNodo("INI", nodo)) {


                //            INI.Rule = ENCABEZADOS + TEMPORALES + FUNCIONES;
                EncabezadoOp encabezado = (EncabezadoOp)recorrerArbol(nodo.ChildNodes[0]);
                LinkedList<Temp_SP_HP> temps = (LinkedList<Temp_SP_HP>)recorrerArbol(nodo.ChildNodes[1]); 
                LinkedList<FuncionOp> listaFunciones = (LinkedList<FuncionOp>)recorrerArbol(nodo.ChildNodes[2]);


                return new AST_OP(encabezado, temps,listaFunciones);
            }
            else if (EstoyEnEsteNodo("ENCABEZADOS",nodo))
            {
                string codigoEncabezado = "";

                if (nodo.ChildNodes.Count != 24) return new EncabezadoOp("");

                int[,] lista = new int[5, 2] { { 0, 1 }, { 2, 7 }, { 8, 13 }, { 14, 18 }, { 19, 23 } };
                for (int i = 0; i < 5; i++)
                {
                    int y = lista[i,0];
                    int x = lista[i,1];
                    for (int j = y; j <= x; j++)
                    {
                        codigoEncabezado += obtenerLexema(nodo.ChildNodes[j])+" ";
                    }
                    codigoEncabezado += "\n";
                }

                return new EncabezadoOp(codigoEncabezado);

            }
            else if (EstoyEnEsteNodo("TEMPORALES", nodo))
            {
                //  TEMPORALES.Rule = pr_float  + LISTA_TEMPS + ptcoma;


                LinkedList<Temp_SP_HP> temps2 = (LinkedList<Temp_SP_HP>)recorrerArbol(nodo.ChildNodes[1]);

                return temps2;
            }
            else if (EstoyEnEsteNodo("LISTA_TEMPS", nodo))
            {
                LinkedList<Temp_SP_HP> tempsFinales = new LinkedList<Temp_SP_HP>();

                foreach (ParseTreeNode item in nodo.ChildNodes)
                {
                    Temp_SP_HP temps = new Temp_SP_HP(obtenerLexema(item),
                    getLinea(item), getColumna(item));
                    tempsFinales.AddLast(temps);
                }


                return tempsFinales;
            }

            else if (EstoyEnEsteNodo("FUNCIONES", nodo))
            {
                if (nodo.ChildNodes.Count == 0) return new LinkedList<FuncionOp>();
                FuncionOp funcion_ = (FuncionOp)recorrerArbol(nodo.ChildNodes[0]);

                LinkedList<FuncionOp> funciones = (LinkedList<FuncionOp>)recorrerArbol(nodo.ChildNodes[1]);
                funciones.AddFirst(funcion_);

                return funciones;
            }

            else if (EstoyEnEsteNodo("FUNCION", nodo))
            {
                string nombre = obtenerLexema(nodo.ChildNodes[1]);

                /* FUNCION.Rule
                        = pr_void + Id + par_abierto + par_cerrado + cor_abierto + INSTRUCCIONES + cor_cerrado
                        | pr_void + Id + par_abierto + par_cerrado + cor_abierto +  cor_cerrado;
                 */

                LinkedList<InstruccionOp> instrucciones;
                if (nodo.ChildNodes.Count == 7)
                {
                    instrucciones = (LinkedList<InstruccionOp>)recorrerArbol(nodo.ChildNodes[5]);
                }
                else
                {
                    instrucciones = new LinkedList<InstruccionOp>();
                }

                return new FuncionOp(nombre, instrucciones, getLinea(nodo.ChildNodes[0]), getColumna(nodo.ChildNodes[0]));

            }
            else if (EstoyEnEsteNodo("INSTRUCCIONES", nodo))
            {
                LinkedList<InstruccionOp> instrucciones = new LinkedList<InstruccionOp>();
                foreach (ParseTreeNode item in nodo.ChildNodes)
                {
                    instrucciones.AddLast((InstruccionOp)recorrerArbol(item));
                }
                return instrucciones;
            }
            else if (EstoyEnEsteNodo("INSTRUCCION", nodo))
            {
                return recorrerArbol(nodo.ChildNodes[0]);
            }
            else if (EstoyEnEsteNodo("IF",nodo))
            {
                /*  IF.Rule
                        = pr_if + par_abierto + TERMINAL + OPERACION_IF + TERMINAL + par_cerrado + pr_goto + Label + ptcoma;
                */
                ExpresionOp opiz = (ExpresionOp)recorrerArbol(nodo.ChildNodes[2]);
                ExpresionOp opder = (ExpresionOp)recorrerArbol(nodo.ChildNodes[4]);
                tipoComparacion comparacion = (tipoComparacion)recorrerArbol(nodo.ChildNodes[3]);
                Label salto = new Label(obtenerLexema(nodo.ChildNodes[7]));

                return new IfOp(opiz,comparacion,opder, salto,getLinea(nodo),getColumna(nodo));
            }


            else if (EstoyEnEsteNodo("OPERACION_IF", nodo))
            {
                /*  OPERACION_IF.Rule
                    = signo_iguales
                    | menor
                    | menor_igual
                    | mayor
                    | mayor_igual
                    | diferente_a;
                 */
                return getTipoCompara(nodo.ChildNodes[0]);
            }

            else if (EstoyEnEsteNodo("GOTO", nodo))
            {
                /*
                    GOTO.Rule = pr_goto + Label + ptcoma;
                 */
                return new GotoOp(obtenerLexema(nodo.ChildNodes[1]), getLinea(nodo), getColumna(nodo));
            }
            else if (EstoyEnEsteNodo("EXPR",nodo))
            {
                /*   EXPR = TERMINAL + signo_mas + TERMINAL
                        | TERMINAL + signo_menos + TERMINAL
                        | TERMINAL + signo_mod + TERMINAL
                        | TERMINAL + signo_por + TERMINAL
                        | TERMINAL + signo_div + TERMINAL
                        | TERMINAL
                        | ACCESO;
                 */
                if (nodo.ChildNodes.Count == 1)
                {
                    return recorrerArbol(nodo.ChildNodes[0]);
                }


                tipoOperacionOpti operacion = getTipoOp(nodo.ChildNodes[1]);
                ExpresionOp izq = (ExpresionOp)recorrerArbol(nodo.ChildNodes[0]);
                ExpresionOp der = (ExpresionOp)recorrerArbol(nodo.ChildNodes[2]);

                OperacionOp aritmetica = new OperacionOp(izq, operacion, der, getLinea(nodo.ChildNodes[1]), getColumna(nodo.ChildNodes[1]));

                return aritmetica;
            }


            else if (EstoyEnEsteNodo("TERMINAL", nodo))
            {
                /*      TERMINAL.Rule
                        = Real
                        | entero
                        | Temporal
                        | pr_SP
                        | pr_HP
                        | signo_menos + entero
                        | signo_menos + Real;
                 */
                return obtenerPrimitivos(nodo.ChildNodes[0]);
            }
            
            else if (EstoyEnEsteNodo("ACCESO", nodo))
            {
                /*
                 *             ACCESO.Rule
                        = pr_heap + lla_abierto + CONVERSION + Temporal + lla_cerrado
                        | pr_heap + lla_abierto + Temporal + lla_cerrado
                        | pr_stack + lla_abierto + CONVERSION + Temporal + lla_cerrado
                        | pr_stack + lla_abierto + Temporal + lla_cerrado;
                 */


                string heap_stack = obtenerLexema(nodo.ChildNodes[0]);
                bool casteo = nodo.ChildNodes.Count > 4 ? true : false;
                Temp_SP_HP temp;

                if(nodo.ChildNodes.Count > 4)
                {
                    temp = new Temp_SP_HP(obtenerLexema(nodo.ChildNodes[3]), getLinea(nodo.ChildNodes[3]), getColumna(nodo.ChildNodes[3]));
                }
                else
                {
                    temp = new Temp_SP_HP(obtenerLexema(nodo.ChildNodes[3]), getLinea(nodo.ChildNodes[3]), getColumna(nodo.ChildNodes[3]));
                }

                return new AccesoOp(heap_stack, temp, casteo, getLinea(nodo), getColumna(nodo));

            }
            else if (EstoyEnEsteNodo("CONVERSION", nodo))
            {
                /* 
                    CONVERSION.Rule 
                        = par_abierto + pr_entero + par_cerrado
                        | par_abierto + pr_float + par_cerrado
                        | par_abierto + pr_char + par_cerrado;*/

                return obtenerLexema(nodo.ChildNodes[1]);
            }
            else if (EstoyEnEsteNodo("RETURN", nodo))
            {
                if(nodo.ChildNodes.Count == 3)
                {
                    ExpresionOp exprecion = (ExpresionOp)recorrerArbol(nodo.ChildNodes[1]);
                    return new ReturnOp(exprecion, getLinea(nodo), getColumna(nodo));
                }
                else
                {
                    return new ReturnOp(getLinea(nodo), getColumna(nodo));
                }
            }
            else if (EstoyEnEsteNodo("DEF_LABEL", nodo))
            {
                /*
                 *   DEF_LABEL.Rule
                         = Label + doble_pt;
                 */

                Label etiqueta = new Label(obtenerLexema(nodo.ChildNodes[0]));

                return new DefLabel(etiqueta, getLinea(nodo), getColumna(nodo));
            }
            else if (EstoyEnEsteNodo("PRINTF", nodo))
            {
                /*
                 *  PRINTF.Rule
                        = pr_printf + par_abierto + cadena + coma + TERM_PRINT + par_cerrado + ptcoma;
                 */
                string cadena = obtenerLexema(nodo.ChildNodes[2]);
                object[] valores = (object[])recorrerArbol(nodo.ChildNodes[4]);

                return new PrintfOp(cadena, (string)valores[0], (ExpresionOp)valores[1], getLinea(nodo), getColumna(nodo));
            
            }
            else if (EstoyEnEsteNodo("TERM_PRINT",nodo))
            {
                /*
                 *  TERM_PRINT.Rule
                 *       =  CONVERSION + Temporal
                 *      | CONVERSION + entero; 
                 */

                string conversion = (string)recorrerArbol(nodo.ChildNodes[0]);
                ExpresionOp valorImpresion;
                valorImpresion = (ExpresionOp)obtenerPrimitivos(nodo.ChildNodes[1]);

                return new object[] { conversion, valorImpresion };

            }

            else if (EstoyEnEsteNodo("ASIGNACION", nodo))
            {
                /*
                 *      ASIGNACION.Rule = EXPR_ASIG + signo_igual + EXPR + ptcoma;
                 */
                ExpresionOp expr_asig = (ExpresionOp)recorrerArbol(nodo.ChildNodes[0]);
                ExpresionOp valor = (ExpresionOp)recorrerArbol(nodo.ChildNodes[2]);

                return new AsignacionOpti(expr_asig, valor, getLinea(nodo), getColumna(nodo));
            }

            else if (EstoyEnEsteNodo("EXPR_ASIG", nodo))
            {
                if (EstoyEnEsteNodo("Temporal", nodo.ChildNodes[0]))
                {
                    return new Temp_SP_HP(obtenerLexema(nodo.ChildNodes[0]), getLinea(nodo.ChildNodes[0]), getColumna(nodo.ChildNodes[0]));
                }
                else if (EstoyEnEsteNodo("SP", nodo.ChildNodes[0]))
                {
                    return new Temp_SP_HP(obtenerLexema(nodo.ChildNodes[0]), getLinea(nodo.ChildNodes[0]), getColumna(nodo.ChildNodes[0]));
                }
                else if (EstoyEnEsteNodo("HP", nodo.ChildNodes[0]))
                {
                    return new Temp_SP_HP(obtenerLexema(nodo.ChildNodes[0]), getLinea(nodo.ChildNodes[0]), getColumna(nodo.ChildNodes[0]));
                }
            }
            else if (EstoyEnEsteNodo("LLAMADA", nodo))
            {
                /*     
                 *     LLAMADA.Rule = Id + par_abierto + par_cerrado + ptcoma;
                 *         */
                string id = obtenerLexema(nodo.ChildNodes[0]);
                return new LlamadaOp(id, getLinea(nodo.ChildNodes[0]), getColumna(nodo.ChildNodes[0]));
            }
            else if (EstoyEnEsteNodo("ASIGNACION_HEAP", nodo))
            {
                /*
                 *  ASIGNACION_HEAP.Rule
                        = pr_heap + lla_abierto + TERMINAL +  lla_cerrado + signo_igual + EXPR + ptcoma
                        | pr_heap + lla_abierto + CONVERSION + TERMINAL + lla_cerrado + signo_igual + EXPR + ptcoma;
                 */
                string Arreglo = obtenerLexema(nodo.ChildNodes[0]);

                bool conversion = nodo.ChildNodes.Count == 8 ? true : false;

                int lugarTerminal;
                if (nodo.ChildNodes.Count == 8) lugarTerminal = 3;
                else lugarTerminal = 2;

                ExpresionOp indice = (ExpresionOp)recorrerArbol(nodo.ChildNodes[lugarTerminal]);

                ExpresionOp valor = (ExpresionOp)recorrerArbol(nodo.ChildNodes[lugarTerminal + 3]);

                return new AsignacionEstructurasOP(Arreglo, indice, valor, conversion, getLinea(nodo.ChildNodes[0]), getColumna(nodo.ChildNodes[0]));
            }
            else if (EstoyEnEsteNodo("ASIGNACION_STACK", nodo))
            {
                /*
                 *  ASIGNACION_STACK.Rule
                        = pr_stack + lla_abierto + TERMINAL +  lla_cerrado + signo_igual + EXPR + ptcoma
                        | pr_stack + lla_abierto + CONVERSION + TERMINAL + lla_cerrado + signo_igual + EXPR + ptcoma;
                 */
                string Arreglo = obtenerLexema(nodo.ChildNodes[0]);

                bool conversion = nodo.ChildNodes.Count == 8 ? true : false;

                int lugarTerminal;
                if (nodo.ChildNodes.Count == 8) lugarTerminal = 3;
                else lugarTerminal = 2;

                ExpresionOp indice = (ExpresionOp)recorrerArbol(nodo.ChildNodes[lugarTerminal]);

                ExpresionOp valor = (ExpresionOp)recorrerArbol(nodo.ChildNodes[lugarTerminal + 3]);

                return new AsignacionEstructurasOP(Arreglo, indice, valor, conversion, getLinea(nodo.ChildNodes[0]), getColumna(nodo.ChildNodes[0]));
            }

            return null;




        }












        public object obtenerPrimitivos(ParseTreeNode nodo)
        {
            if (EstoyEnEsteNodo("real", nodo))
            {
                return new NumeroOp(Double.Parse(obtenerLexema(nodo)), getLinea(nodo), getColumna(nodo));
            }
            else if (EstoyEnEsteNodo("entero", nodo))
            {
                return new NumeroOp(Int32.Parse(obtenerLexema(nodo)), getLinea(nodo), getColumna(nodo));
            }
            else if (EstoyEnEsteNodo("Temporal", nodo))
            {
                return new Temp_SP_HP(obtenerLexema(nodo), getLinea(nodo), getColumna(nodo));

            }else if (EstoyEnEsteNodo("SP",nodo))
            {
                return new Temp_SP_HP(obtenerLexema(nodo), getLinea(nodo), getColumna(nodo));
            }
            else if(EstoyEnEsteNodo("HP", nodo))
            {
                return new Temp_SP_HP(obtenerLexema(nodo), getLinea(nodo), getColumna(nodo));
            }
            else if(nodo.ChildNodes.Count == 2)
            {
                ParseTreeNode nodoValor = nodo.ChildNodes[1];
                if (EstoyEnEsteNodo("real", nodoValor))
                {
                    double valor = Double.Parse(obtenerLexema(nodoValor))*-1;
                    return new NumeroOp(valor, getLinea(nodoValor), getColumna(nodoValor));
                }
                else if (EstoyEnEsteNodo("entero", nodoValor))
                {
                    int valor = Int32.Parse(obtenerLexema(nodoValor));
                    return new NumeroOp(valor, getLinea(nodoValor), getColumna(nodoValor));
                }
            }
            return new NumeroOp(0, 0, 0);
        }

        public tipoComparacion getTipoCompara(ParseTreeNode nodo)
        {
            string signo = obtenerLexema(nodo);
            switch (signo)
            {
                case "==":
                    return tipoComparacion.igual;
                case "!=":
                    return tipoComparacion.diferente;
                case "<":
                    return tipoComparacion.menor;
                case "<=":
                    return tipoComparacion.menorque;
                case ">":
                    return tipoComparacion.mayor;
                case ">=":
                    return tipoComparacion.mayorque;
                default:
                    return tipoComparacion.igual;
            }
        }

        public tipoOperacionOpti getTipoOp(ParseTreeNode nodo)
        {
            string signo = obtenerLexema(nodo);
            switch (signo)
            {
                case "+":
                    return tipoOperacionOpti.suma;
                case "-":
                    return tipoOperacionOpti.resta;
                case "%":
                    return tipoOperacionOpti.modulo;
                case "/":
                    return tipoOperacionOpti.division;
                case "*":
                    return tipoOperacionOpti.multiplicacion;
                default:
                    return tipoOperacionOpti.suma;
            }
        }

        public bool EstoyEnEsteNodo(string nodo,ParseTreeNode evaluar)
        {
            return evaluar.Term.Name.Equals(nodo, System.StringComparison.InvariantCultureIgnoreCase);
        }

        public string obtenerLexema(ParseTreeNode hijo)
        {
            return hijo.Token.Text;
        }

        public int getLinea(ParseTreeNode hijo)
        {
            return hijo.Span.Location.Line +1;
        }

        public int getColumna(ParseTreeNode hijo)
        {
            return hijo.Span.Location.Column;
        }

    }
}
