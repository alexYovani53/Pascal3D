
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Text;
using CompiPascal.AST_;
using CompiPascal.AST_.interfaces;
using CompiPascal.AST_.definicion;
using CompiPascal.entorno_;
using static CompiPascal.entorno_.Simbolo;
using CompiPascal.AST_.valoreImplicito;
using CompiPascal.AST_.funcionesPrimitivas;
using CompiPascal.entorno_.simbolos;
using CompiPascal.AST_.control;
using CompiPascal.AST_.bucles;
using CompiPascal.AST_.cambioFlujo;
using System.IO;
using System.Linq;
using System.Collections;
using CompiPascal.AST_.definicion.arrego;

namespace CompiPascal.Traductor
{
    public class ArmarAST
    {

        Stack<bool> bucles = new Stack<bool>();     //SE UTILIZA PARA IR ABILANDO LOS CICLOS Y ASI PODER USAR LOS BREAK Y CONTINUE
        Stack<TipoDatos> tiposRet = new Stack<TipoDatos>();

        /**
         * @fn  public AST Analizar(ParseTreeNode raiz)
         * @descripcion   Metodo que devuelve un AST construido
         * @return  El AST construido.
         */

        public ArmarAST()
        {


        }

        public AST generarAST(ParseTree raizPrincipal)
        {
            using (StreamWriter ar = new StreamWriter("dicho.txt"))
            {
                ar.Write("hola");
            }
            return (AST) recorrerArbol(raizPrincipal.Root);
        }


        public object recorrerArbol(ParseTreeNode nodo)
        {

            if (estoyEnEsteNodo(nodo, "INI"))
            {
                /* INI.Rule = pr_program + identificador + ptcoma + ENCABEZADOS + PRINCIPAL;
                 * nodo.ChildNodes[2] =  ENCABEZADOS.
                 * esto porque ptcoma se quita del arbol al hacer uso de la funcion
                 *            -------- MarkPunctuation --- en la parte final de la gramatica
                 */
                LinkedList<Instruccion> primero = (LinkedList<Instruccion>)recorrerArbol(nodo.ChildNodes[2]);
                Instruccion segundo = (Instruccion)recorrerArbol(nodo.ChildNodes[3]);

                return new AST(primero, segundo);

            }
            else if (estoyEnEsteNodo(nodo, "ENCABEZADOS"))
            {
                /* ENCABEZADOS.Rule = MakeStarRule(ENCABEZADOS, _ENCABEZADO);    
                 * se debe crear una lista para guardar las instrucciones de encabezado
                 */

                LinkedList<Instruccion> instrucciones = new LinkedList<Instruccion>();
                foreach (ParseTreeNode item in nodo.ChildNodes)
                {
                    // recorrerArbol(item) entra a uno de los encabezados posibles, VAR_SEC, CONST_SEC
                    // y este devuelve una instruccion (clase) dependiendo el tipo

                    object resultado = recorrerArbol(item);

                        //RESULTADO DE LISTA
                    if (resultado is LinkedList<Instruccion>)
                    {
                        foreach (Instruccion insTemp in (LinkedList<Instruccion>)resultado)
                        {
                            instrucciones.AddLast(insTemp);
                        }
                    }
                    else
                    {
                        //RESULTADO DE UNI CLASE. 
                        instrucciones.AddLast((Instruccion)resultado);
                    }

                }

                return instrucciones;

            }

            else if (estoyEnEsteNodo(nodo, "_ENCABEZADO"))
            {
                /* _ENCABEZADO.Rule = VAR_SEC
                                    | CONST_SEC
                                    | SEC_TYPES
                                    | FUNCION;*/
                return recorrerArbol(nodo.ChildNodes[0]);
            }

            else if (estoyEnEsteNodo(nodo, "VAR_SEC"))
            {
                /*            VAR_SEC.Rule = pr_var + DECLARACIONES;
                 */

                //return new Seccion((LinkedList<Instruccion>)recorrerArbol(nodo.ChildNodes[1]));

                return recorrerArbol(nodo.ChildNodes[1]);

            }

            else if (estoyEnEsteNodo(nodo, "DECLARACIONES"))
            {
                /*   DECLARACIONES.Rule = MakePlusRule(DECLARACIONES, _DECLARACION);
                 */
                LinkedList<Instruccion> declaraciones = new LinkedList<Instruccion>();
                foreach (ParseTreeNode itemNodo in nodo.ChildNodes)
                {
                    declaraciones.AddLast((Instruccion)recorrerArbol(itemNodo));
                }
                return declaraciones;
            }

            else if (estoyEnEsteNodo(nodo, "_DECLARACION"))
            {
                /* _DECLARACION.Rule = DECLARACION_SIN_INICIAR + ptcoma
                                       | DECLARACION_INICIALIZACION + ptcoma;
                */
                return recorrerArbol(nodo.ChildNodes[0]);
            }

            else if (estoyEnEsteNodo(nodo, "DECLARACION_SIN_INICIAR"))
            {
                /* DECLARACION_SIN_INICIAR.Rule = LIST_NOMBRES + doble_pt + TIPO_DATO
                 *                              | LIST_NOMBRES + doble_pt + pr_array + CUERPO_ARRAY
                                                | LIST_NOMBRES + doble_pt + identificador;
                 *   
                 *   acá de nuevo. el doble_pt ya no aparece por lo que solo hay dos hijos LIST_NOMBRES y TIPO_DATO
                 */

                if(nodo.ChildNodes.Count == 3)
                {
                    object resultado = recorrerArbol(nodo.ChildNodes[2]);
                    GuardaArray dec = (GuardaArray)resultado;
                    return new DeclaraArray2((LinkedList<Simbolo>)recorrerArbol(nodo.ChildNodes[0]), 
                                            dec.tipoObjeto, dec.tipo, dec.niveles, dec.linea, dec.columna);

                }


                /* declaracion de variables de tipos primitivos y de tipo Objeto */

                if (estoyEnEsteNodo(nodo.ChildNodes[1], "TIPO_DATO"))
                {

                    return new Declaracion((LinkedList<Simbolo>)recorrerArbol(nodo.ChildNodes[0]),
                                           (TipoDatos)recorrerArbol(nodo.ChildNodes[1]));
                }
                else
                {


                    string nombreStruct = obtenerLexema(nodo, 1);
                    return new DeclararStruct((LinkedList<Simbolo>)recorrerArbol(nodo.ChildNodes[0]),
                                                nombreStruct,
                                                getLinea(nodo, 0),
                                                getColumna(nodo, 0));
                }


            }

            else if (estoyEnEsteNodo(nodo, "DECLARACION_INICIALIZACION"))
            {
                /*   DECLARACION_INICIALIZACION.Rule = LIST_NOMBRES + doble_pt + TIPO_DATO + VAL_INICIAL;
                 *   de nuevo, doble_pt no aparece en el arbol de IRONY por lo que en esta produción solo se tienen 3 no terminales
                 */
                return new Declaracion((LinkedList<Simbolo>)recorrerArbol(nodo.ChildNodes[0]),
                                        (TipoDatos)recorrerArbol(nodo.ChildNodes[1]),
                                        (Expresion)recorrerArbol(nodo.ChildNodes[2]));
            }

            else if (estoyEnEsteNodo(nodo, "LIST_NOMBRES"))
            {
                /* LIST_NOMBRES.Rule = MakePlusRule(LIST_NOMBRES,coma, identificador);
                 */

                LinkedList<Simbolo> variables = new LinkedList<Simbolo>();
                foreach (ParseTreeNode nodoVar in nodo.ChildNodes)
                {
                    variables.AddLast(new Simbolo(nodoVar.Token.Text, nodoVar.Token.Location.Line, nodoVar.Token.Location.Column));
                }
                return variables;
            }


            else if (estoyEnEsteNodo(nodo, "CONST_SEC"))
            {

                /*  CONST_SEC.Rule = pr_constante + CONSTANTES;
                 */
                //return new Seccion((LinkedList<Instruccion>)recorrerArbol(nodo.ChildNodes[1]));
                return recorrerArbol(nodo.ChildNodes[1]);
            }
            else if (estoyEnEsteNodo(nodo, "CONSTANTES"))
            {
                /*  CONSTANTES.Rule = MakePlusRule(CONSTANTES, _CONSTANTES);
                 */
                LinkedList<Instruccion> dec_Constantes = new LinkedList<Instruccion>();
                foreach (ParseTreeNode nodo_pivote in nodo.ChildNodes)
                {
                    dec_Constantes.AddLast((Instruccion)recorrerArbol(nodo_pivote));
                }
                return dec_Constantes;
            }
            else if (estoyEnEsteNodo(nodo, "_CONSTANTE"))
            {
                /*  _CONSTANTES.Rule = identificador + VAL_INICIAL + ptcoma;
                 */

                Declaracion f = new Declaracion(new Simbolo(nodo.ChildNodes[0].Token.Text, getLinea(nodo, 0),
                                            getColumna(nodo, 0)), (Expresion)recorrerArbol(nodo.ChildNodes[1]));

                return f;
            }


            /*  ════════════════════════════════════════════════════════════════════════════════════════════════
             *  ════════════════════════════════                    ════════════════════════════════════════════
             *  ════════════════════════════════ AREA DE TYPES      ════════════════════════════════════════════
             *  ════════════════════════════════                    ════════════════════════════════════════════
             *  ════════════════════════════════════════════════════════════════════════════════════════════════
             */


            else if (estoyEnEsteNodo(nodo, "SEC_TYPES"))
            {
                /* SEC_TYPES.Rule = pr_type + identificador + signo_igual + pr_object + CUERPO_STRUCT
                                   |pr_type + identificador + signo_igual + pr_array + CUERPO_ARRAY; */

                string nombreType = obtenerLexema(nodo, 1);


                if (estoyEnEsteNodo(nodo.ChildNodes[4], "CUERPO_STRUCT"))
                {
                    LinkedList<Instruccion> declaraciones = (LinkedList<Instruccion>)recorrerArbol(nodo.ChildNodes[4]);
                    return new GuardarStruct(nombreType, declaraciones, getLinea(nodo, 0), getColumna(nodo, 0));
                }
                else if (estoyEnEsteNodo(nodo.ChildNodes[4], "CUERPO_ARRAY"))
                {

                    object resultado = recorrerArbol(nodo.ChildNodes[4]);
                    GuardaArray dec = (GuardaArray)resultado;
                    dec.identificador = nombreType;
                    return dec;
                }

            }

            else if (estoyEnEsteNodo(nodo, "CUERPO_STRUCT"))
            {

                /* CUERPO_STRUCT.Rule = ENCABEZADOS + pr_end + ptcoma
                                        | pr_end + ptcoma;*/
                return recorrerArbol(nodo.ChildNodes[0]);

            }
            else if (estoyEnEsteNodo(nodo, "CUERPO_ARRAY"))
            {
                /*  CUERPO_ARRAY.Rule = lla_abierto + NIVEL_ARRAY + lla_cerrado + pr_of + TIPO_DATO + ptcoma
                                   |lla_abierto + NIVEL_ARRAY + lla_cerrado + pr_of + identificador + ptcoma;*/

                TipoDatos tipo;
                LinkedList<Expresion[]> niveles = (LinkedList<Expresion[]>)recorrerArbol(nodo.ChildNodes[0]);

                if (estoyEnEsteNodo(nodo.ChildNodes[2], "TIPO_DATO"))
                {
                    tipo = (TipoDatos)recorrerArbol(nodo.ChildNodes[2]);
                    return new GuardaArray("", null, tipo, niveles, getLinea(nodo, 0), getColumna(nodo, 0));
                }
                else
                {
                    tipo = TipoDatos.Object;
                    string nombreTipoIde = obtenerLexema(nodo, 2);
                    return new GuardaArray("", nombreTipoIde, tipo, niveles, getLinea(nodo, 0), getColumna(nodo, 0));
                }

            }
            else if (estoyEnEsteNodo(nodo, "NIVEL_ARRAY"))
            {
                /* NIVEL_ARRAY.Rule = MakePlusRule(NIVEL_ARRAY,coma,NIVEL);*/

                LinkedList<Expresion[]> niveles = new LinkedList<Expresion[]>();
                foreach (ParseTreeNode item in nodo.ChildNodes)
                {
                    Expresion[] dato = new Expresion[2];
                    dato = (Expresion[])recorrerArbol(item);
                    niveles.AddLast(dato);
                }

                return niveles;

            }
            else if (estoyEnEsteNodo(nodo, "NIVEL"))
            {
                /*            NIVEL.Rule = EXPR + pt + pt + EXPR;*/

                Expresion[] niv = new Expresion[2];
                niv[0] = (Expresion)recorrerArbol(nodo.ChildNodes[0]);
                niv[1] = (Expresion)recorrerArbol(nodo.ChildNodes[3]);
                return niv;
            }

            else if (estoyEnEsteNodo(nodo, "ACCESO_ARR"))
            {
                /*ACCESO_ARR.Rule = SENTENCIA_ACCESO + INDICES + pt  + SENTENCIA_ACCESO
                                    |SENTENCIA_ACCESO + INDICES;*/

                LinkedList<Expresion> indices = (LinkedList<Expresion>)recorrerArbol(nodo.ChildNodes[1]);
                object resultado = recorrerArbol(nodo.ChildNodes[0]);


                if (nodo.ChildNodes.Count == 4)
                {
                    object resultado2 = recorrerArbol(nodo.ChildNodes[3]);
                    LinkedList<string> buscando =  new LinkedList<string>(); 
                    if (resultado2 is Identificador)
                    {
                        buscando.AddLast(((Identificador)resultado2).nombre());
                    }
                    else
                    {
                        buscando = new LinkedList<string>(((Acceso)resultado2).listaParametros);
                        buscando.AddLast(((Acceso)resultado2).idObjeto);
                    }


                    if (resultado is Acceso)
                    {
                        Acceso encontrado = (Acceso)resultado;
                        AccesoArreglo nuevoAcceso = new AccesoArreglo(encontrado.idObjeto, encontrado.listaParametros, buscando, indices, getLinea(nodo, 0), getColumna(nodo, 0));
                        return nuevoAcceso;
                    }
                    else
                    {
                        string ide = ((Identificador)resultado).nombre();
                        AccesoArreglo nuevoAcceso = new AccesoArreglo(ide, indices,buscando, getLinea(nodo, 0), getColumna(nodo, 0));
                        return nuevoAcceso;
                    }
                }



                if (resultado is Acceso)
                {
                    Acceso encontrado = (Acceso)resultado;
                    AccesoArreglo nuevoAcceso = new AccesoArreglo(encontrado.idObjeto, encontrado.listaParametros, indices, getLinea(nodo, 0), getColumna(nodo, 0));
                    return nuevoAcceso;
                }
                else
                {
                    string ide = ((Identificador)resultado).nombre();
                    AccesoArreglo nuevoAcceso = new AccesoArreglo(ide, indices, getLinea(nodo, 0), getColumna(nodo, 0));
                    return nuevoAcceso;
                }

            }

            else if (estoyEnEsteNodo(nodo, "INDICES"))
            {
                /* INDICES.Rule = lla_abierto + PARAMS + lla_cerrado
                                | INDICE2*/

                if (estoyEnEsteNodo(nodo.ChildNodes[0], "PARAMS"))
                {
                    return recorrerArbol(nodo.ChildNodes[0]);
                }
                else
                {
                    return recorrerArbol(nodo.ChildNodes[0]);
                }

            }

            else if (estoyEnEsteNodo(nodo, "INDICE2"))
            {
                /* INDICES.Rule = MakePlusRule(INDICES, INDICE);*/

                LinkedList<Expresion> indices = new LinkedList<Expresion>();
                foreach (ParseTreeNode item in nodo.ChildNodes)
                {
                    Expresion nuevaExpr = (Expresion)recorrerArbol(item);
                    indices.AddLast(nuevaExpr);
                }
                return indices;
            }
            else if (estoyEnEsteNodo(nodo, "INDICE"))
            {
                /*            INDICE.Rule = cor_abierto + EXPR + cor_cerrado;*/
                Expresion index = (Expresion)recorrerArbol(nodo.ChildNodes[0]);
                return index;
            }

            else if (estoyEnEsteNodo(nodo, "SENTENCIA_ASIG_ARR"))
            {
                /* SENTENCIA_ASIG_ARR.Rule = SENTENCIA_ACCESO  + INDICES + doble_pt + signo_igual + EXPR ;*/

                LinkedList<Expresion> indices = (LinkedList<Expresion>)recorrerArbol(nodo.ChildNodes[1]);
                Expresion valNuevo = (Expresion)recorrerArbol(nodo.ChildNodes[3]);

                object resultado = recorrerArbol(nodo.ChildNodes[0]);
                if ( resultado is Acceso)
                {
                    Acceso encontrado = (Acceso)resultado;
                    return new AsignarArray(encontrado.idObjeto, encontrado.listaParametros, indices, valNuevo, getLinea(nodo, 0), getColumna(nodo, 0));
                }
                else
                {
                    string nombre = ((Identificador)resultado).nombre();
                    return new AsignarArray(nombre, indices, valNuevo, getLinea(nodo, 0), getColumna(nodo, 0));
                }

            }




            /*  ════════════════════════════════════════════════════════════════════════════════════════════════
             *  ════════════════════════════════                    ════════════════════════════════════════════
             *  ════════════════════════════════ AREA DE VAL INICIAL════════════════════════════════════════════
             *  ════════════════════════════════                    ════════════════════════════════════════════
             *  ════════════════════════════════════════════════════════════════════════════════════════════════
             */




            else if (estoyEnEsteNodo(nodo, "VAL_INICIAL"))
            {
                /*  VAL_INICIAL.Rule = signo_igual + EXPR;
                 *
                 */

                return recorrerArbol(nodo.ChildNodes[1]); ;

            }
            else if (estoyEnEsteNodo(nodo, "EXPR"))
            {
                /* EXPR.Rule = EXPR_ARITMETICA
                        | EXPR_LOGICA
                        | EXPR_RELACIONAL
                        | PRIMITIVO
                        | SENTENCIA_ACCESO
                        | SENTENCIA_LLAMADA
                        | par_abierto + EXPR + par_cerrado;
                */
                return recorrerArbol(nodo.ChildNodes[0]);
            }

            else if (estoyEnEsteNodo(nodo, "EXPR_ARITMETICA"))
            {
                /*EXPR_ARITMETICA.Rule = EXPR + signo_mas + EXPR
                        | EXPR + signo_menos + EXPR
                        | EXPR + signo_por + EXPR
                        | EXPR + signo_div + EXPR
                        | EXPR + signo_mod + EXPR
                        | signo_menos + EXPR;
                 * 
                 * op1 en la posicion 0, op2 en la posicion 2 y operador en la posicion 1
                 */

                if(nodo.ChildNodes.Count == 3)
                {
                    return new Operacion((Expresion)recorrerArbol(nodo.ChildNodes[0]),
                     (Expresion)recorrerArbol(nodo.ChildNodes[2]),
                     Operacion.GetOperador(obtenerLexema(nodo, 1)), getLinea(nodo, 0), getColumna(nodo, 0));

                }
                else
                {
                    return new Operacion((Expresion)recorrerArbol(nodo.ChildNodes[1]),
                                        Operacion.GetOperador(obtenerLexema(nodo, 0)), getLinea(nodo, 0), getColumna(nodo, 0));
                }


            }

            else if (estoyEnEsteNodo(nodo, "EXPR_LOGICA"))
            {
                /*            EXPR_LOGICA.Rule = EXPR + pr_or + EXPR
                                | EXPR + pr_and + EXPR
                                | pr_not + EXPR;
                */

                if (nodo.ChildNodes.Count == 3)
                {

                    return new Operacion((Expresion)recorrerArbol(nodo.ChildNodes[0]),
                                         (Expresion)recorrerArbol(nodo.ChildNodes[2]),
                                         Operacion.GetOperador(obtenerLexema(nodo, 1)), getLinea(nodo, 0), getColumna(nodo, 0));
                }
                else
                {
                    return new Operacion((Expresion)recorrerArbol(nodo.ChildNodes[1]),
                                         Operacion.GetOperador(obtenerLexema(nodo, 0)), getLinea(nodo, 0), getColumna(nodo, 0));
                }

            }

            else if (estoyEnEsteNodo(nodo, "EXPR_RELACIONAL"))
            {
                /*EXPR_RELACIONAL.Rule = EXPR + mayor_igual + EXPR
                        | EXPR + menor_igual + EXPR
                        | EXPR + mayor + EXPR
                        | EXPR + menor + EXPR
                        | EXPR + signo_igual + EXPR
                        | EXPR + diferente_a + EXPR;*/

                return new Operacion((Expresion)recorrerArbol(nodo.ChildNodes[0]),
                                     (Expresion)recorrerArbol(nodo.ChildNodes[2]),
                                     Operacion.GetOperador(obtenerLexema(nodo, 1)), getLinea(nodo, 0), getColumna(nodo, 0));
            }



            else if (estoyEnEsteNodo(nodo, "SENTENCIAS"))
            {
                /*      SENTENCIAS.Rule = MakeStarRule(SENTENCIAS, _SENTENCIA);
                 */
                LinkedList<Instruccion> sentencias = new LinkedList<Instruccion>();

                foreach (ParseTreeNode statemen in nodo.ChildNodes)
                {

                    sentencias.AddLast((Instruccion)recorrerArbol(statemen));
                }

                return sentencias;
            }

            else if (estoyEnEsteNodo(nodo, "_SENTENCIA"))
            {
                /*_SENTENCIA.Rule = SENTENCIA_IF
                                | SENTENCIA_CASE
                                | SENTENCIA_WHILE
                                | SENTENCIA_REPEAT
                                | SENTENCIA_FOR
                                | SENTENCIA_ACCESO
                                | SENTENCIA_LLAMADA
                                | SENTENCIA_BREAK
                                | SENTENCIA_CONTINUE
                                | SENTENCIA_ASIGNAR
                                | SENTENCIA_WRITE
                                | SENTENCIA_GRAFICAR_TS;
                */
                return recorrerArbol(nodo.ChildNodes[0]);
            }
            else if (estoyEnEsteNodo(nodo, "_SENTENCIA2"))
            {
                /*_SENTENCIA.Rule =  SENTENCIA_CASE
                                | SENTENCIA_WHILE
                                | SENTENCIA_REPEAT
                                | SENTENCIA_FOR
                                | SENTENCIA_ACCESO
                                | SENTENCIA_LLAMADA
                                | SENTENCIA_BREAK
                                | SENTENCIA_CONTINUE
                                | SENTENCIA_ASIGNAR
                                | SENTENCIA_WRITE
                                | SENTENCIA_GRAFICAR_TS;
                */
                return recorrerArbol(nodo.ChildNodes[0]);
            }

            /*  ════════════════════════════════════════════════════════════════════════════════════════════════
             *  ════════════════════════════════                    ════════════════════════════════════════════
             *  ════════════════════════════════ AREA DE SENTENCIAS ════════════════════════════════════════════
             *  ════════════════════════════════                    ════════════════════════════════════════════
             *  ════════════════════════════════════════════════════════════════════════════════════════════════
             */

            else if (estoyEnEsteNodo(nodo, "SENTENCIA_GRAFICAR_TS"))
            {
                /*            SENTENCIA_GRAFICAR_TS.Rule = pr_graficar;*/

                return new graficarTS(getLinea(nodo, 0), getColumna(nodo, 0));

            }
            else if (estoyEnEsteNodo(nodo, "SENTENCIA_WRITE"))
            {
                /* SENTENCIA_WRITE.Rule = pr_writeln + par_abierto + EXPR_IMPRIMIR + par_cerrado + ptcoma
                                            | pr_writeln
                                            | pr_write + par_abierto + EXPR_IMPRIMIR + par_cerrado + ptcoma;
                */
                // el parentecis abierto y cerrado se omiten por la gramatica, por eso la posicion de la 
                // expresion es 1 no 2

                string lexema = obtenerLexema(nodo, 0);
                lexema = lexema.ToLower();

                if (lexema.Equals("writeln"))
                {
                    if(nodo.ChildNodes.Count == 1)
                    {
                        return new Write(true, nodo.ChildNodes[0].Token.Location.Line, nodo.ChildNodes[0].Token.Location.Column);
                    }
                    return new Write((LinkedList<Expresion>)recorrerArbol(nodo.ChildNodes[1]), true, nodo.ChildNodes[0].Token.Location.Line, nodo.ChildNodes[0].Token.Location.Column);
                
                }
                else
                {
                    return new Write((LinkedList<Expresion>)recorrerArbol(nodo.ChildNodes[1]), false, nodo.ChildNodes[0].Token.Location.Line, nodo.ChildNodes[0].Token.Location.Column);
                }

            }
            else if (estoyEnEsteNodo(nodo, "EXPR_IMPRIMIR"))
            {
                /*  EXPR_IMPRIMIR.Rule = MakePlusRule(EXPR_IMPRIMIR, coma, EXPR_FORMATEO);
                 */

                LinkedList<Expresion> expr_imprimir = new LinkedList<Expresion>();

                foreach (ParseTreeNode nodo_impresion in nodo.ChildNodes)
                {
                    expr_imprimir.AddLast((Expresion)recorrerArbol(nodo_impresion));
                }
                return expr_imprimir;

            }

            else if (estoyEnEsteNodo(nodo, "EXPR_FORMATEO"))
            {
                /* EXPR_FORMATEO.Rule = EXPR + doble_pt + EXPR + doble_pt + EXPR
                                        | EXPR + doble_pt + EXPR
                                        | EXPR;
                */

                if (nodo.ChildNodes.Count == 1)
                {
                    return recorrerArbol(nodo.ChildNodes[0]);
                }
                else if (nodo.ChildNodes.Count == 2)
                {
                    return new Formateo((Expresion)recorrerArbol(nodo.ChildNodes[0]),
                                        (Expresion)recorrerArbol(nodo.ChildNodes[1]), getLinea(nodo, 0), getColumna(nodo, 0));
                }
                else if (nodo.ChildNodes.Count == 3)
                {
                    return new Formateo((Expresion)recorrerArbol(nodo.ChildNodes[0]),
                                        (Expresion)recorrerArbol(nodo.ChildNodes[1]),
                                        (Expresion)recorrerArbol(nodo.ChildNodes[2]), getLinea(nodo, 0), getColumna(nodo, 0));
                }
            }

            else if (estoyEnEsteNodo(nodo, "TIPO_DATO"))
            {
                /*TIPO_DATO.Rule = pr_string
                    | pr_char
                    | pr_entero
                    | pr_real
                    | pr_boolean
                    | pr_void
                    | identificador;*/

                if (estoyEnEsteNodo(nodo.ChildNodes[0], "String"))
                {
                    return TipoDatos.String;
                }
                else if (estoyEnEsteNodo(nodo.ChildNodes[0], "integer"))
                {
                    return TipoDatos.Integer;
                }
                else if (estoyEnEsteNodo(nodo.ChildNodes[0], "boolean"))
                {
                    return TipoDatos.Boolean;
                }
                else if (estoyEnEsteNodo(nodo.ChildNodes[0], "char"))
                {
                    return TipoDatos.Char;
                }
                else if (estoyEnEsteNodo(nodo.ChildNodes[0], "real"))
                {
                    return TipoDatos.Real;
                }
                else if (estoyEnEsteNodo(nodo.ChildNodes[0], "identificador"))
                {
                    return TipoDatos.Object;
                }

            }

            else if (estoyEnEsteNodo(nodo, "SENTENCIA_ASIGNAR"))
            {

                /*    SENTENCIA_ASIGNAR.Rule = VARIABLE_ASIGNAR + doble_pt + signo_igual + EXPR + ptcoma;
                 */
                object resultado = recorrerArbol(nodo.ChildNodes[0]);

                if (resultado is Identificador)     //PRODUCCION   VARIABLE_ASIGNA.RULE -> identificador
                {
                    string nombre = ((Identificador)resultado).nombre();
                    return new Asignacion(new Simbolo(nombre, getLinea(nodo, 0), getColumna(nodo, 0)),
                                         (Expresion)recorrerArbol(nodo.ChildNodes[2]), false, 0, 0);
                }
                else
                {
                    Acceso encontrado = (Acceso)resultado;
                    return new Asignacion(encontrado.idObjeto, encontrado.listaParametros, (Expresion)recorrerArbol(nodo.ChildNodes[2]),
                                            getLinea(nodo, 0), getColumna(nodo, 0));

                }//FIN ELSE

            }
            else if (estoyEnEsteNodo(nodo, "VARIABLE_ASIGNAR"))
            {
                /*            VARIABLE_ASIGNAR.Rule = SENTENCIA_ACCESO;
                */

                return recorrerArbol(nodo.ChildNodes[0]);
            }
            else if (estoyEnEsteNodo(nodo, "SENTENCIA_ACCESO"))
            {
                /*                SENTENCIA_ACCESO.Rule = identificador + pt + LISTA_ACCESO;
                 */


                LinkedList<string> listaIdes = (LinkedList<string>)recorrerArbol(nodo.ChildNodes[0]);


                if (listaIdes.Count == 1) return new Identificador(listaIdes.ElementAt(0), getLinea(nodo, 0), getColumna(nodo, 0));


                IList<string> listaIdesCopia = new List<string>(listaIdes);
                string idPrincipal = listaIdesCopia[0];
                listaIdesCopia.RemoveAt(0);
                LinkedList<string> nuevo = new LinkedList<string>(listaIdesCopia);
                Acceso nuevoAcceso = new Acceso(idPrincipal, nuevo, getLinea(nodo, 0), getColumna(nodo, 0));
                return nuevoAcceso;
            }
            else if (estoyEnEsteNodo(nodo, "LISTA_ACCESO"))
            {
                /*            LISTA_ACCESO.Rule = MakeStarRule(LISTA_ACCESO, pt, identificador);
                */
                LinkedList<string> listaIdes = new LinkedList<string>();
                foreach (ParseTreeNode item in nodo.ChildNodes)
                {
                    listaIdes.AddLast(item.Token.Text);
                }

                return listaIdes;

            }



            else if (estoyEnEsteNodo(nodo, "SENTENCIA_IF"))
            {
                /*    SENTENCIA_IF.Rule = pr_if + EXPR + pr_then + BLOQUE_ELSE
                                        | pr_if + EXPR + pr_then + BLOQUE_ELSE + pr_else + ELSE_IF
                                        | pr_if + EXPR + pr_then + BLOQUE_ELSE + pr_else + BLOQUE_ELSE;*/


                //LA EXPRESION SIEMPRE ESTA EN EL NODO 2
                Expresion exprCondicional = (Expresion)recorrerArbol(nodo.ChildNodes[1]);

                //LAS INSTRUCCIONES ESTAN EL EL NODO 4
                LinkedList<Instruccion> instrucionesPrincipales = (LinkedList<Instruccion>)recorrerArbol(nodo.ChildNodes[3]);

                LinkedList<Instruccion> instruccionesElse = new LinkedList<Instruccion>();
                LinkedList<Instruccion> instrucionesElse_if = new LinkedList<Instruccion>();

                if (nodo.ChildNodes.Count > 4)
                {

                    if (estoyEnEsteNodo(nodo.ChildNodes[5], "BLOQUE_ELSE"))
                    {

                        instruccionesElse = (LinkedList<Instruccion>)recorrerArbol(nodo.ChildNodes[5]);
                    }
                    else
                    {
                        LinkedList<Instruccion> instruccionesElse_auxiliar = new LinkedList<Instruccion>();
                        LinkedList<Instruccion> pivote = (LinkedList<Instruccion>)recorrerArbol(nodo.ChildNodes[5]);
                        IList pivotes = new List<Instruccion>(pivote);

                        for (int i = pivotes.Count - 1; i >= 0; i--)
                        {
                            Instruccion item = (Instruccion)pivotes[i];
                            if (item is If) instrucionesElse_if.AddLast(item);
                            else instruccionesElse_auxiliar.AddLast(item);
                        }

                        IList auxiliar2 = new List<Instruccion>(instruccionesElse_auxiliar);
                        for( int i = instruccionesElse_auxiliar.Count-1; i>= 0; i--)
                        {
                            instruccionesElse.AddLast((Instruccion)auxiliar2[i]);
                        }

                    }

                }
                If final = new If(exprCondicional, instrucionesPrincipales, instruccionesElse, instrucionesElse_if, getLinea(nodo, 0), getColumna(nodo, 0));

                return final;


            }
            else if (estoyEnEsteNodo(nodo, "ELSE_IF"))
            {
                /*    ELSE_IF.Rule = pr_if + EXPR + pr_then + BLOQUE_ELSE
                                        | pr_if + EXPR + pr_then + BLOQUE_ELSE + pr_else + ELSE_IF
                                        | pr_if + EXPR + pr_then + BLOQUE_ELSE + pr_else + BLOQUE_ELSE;*/

                LinkedList<Instruccion> listaIfFinal = new LinkedList<Instruccion>();

                //LA EXPRESION SIEMPRE ESTA EN EL NODO 2
                Expresion exprCondicional = (Expresion)recorrerArbol(nodo.ChildNodes[1]);

                //LAS INSTRUCCIONES ESTAN EL EL NODO 4
                LinkedList<Instruccion> instrucionesPrincipales = (LinkedList<Instruccion>)recorrerArbol(nodo.ChildNodes[3]);


                if (nodo.ChildNodes.Count == 4)
                {
                    listaIfFinal.AddLast(new If(exprCondicional, instrucionesPrincipales, new LinkedList<Instruccion>(), new LinkedList<Instruccion>(), getLinea(nodo, 0), getColumna(nodo, 0)));
                    return listaIfFinal;
                }

                LinkedList<Instruccion> pivote = (LinkedList<Instruccion>)recorrerArbol(nodo.ChildNodes[5]);

                foreach (Instruccion instrsElse in pivote)
                {
                    listaIfFinal.AddLast(instrsElse);
                }

                listaIfFinal.AddLast(new If(exprCondicional, instrucionesPrincipales, new LinkedList<Instruccion>(), new LinkedList<Instruccion>(), getLinea(nodo, 0), getColumna(nodo, 0)));
                return listaIfFinal;


            }



            else if (estoyEnEsteNodo(nodo, "BLOQUE")) {

                /* BLOQUE.Rule = pr_begin + SENTENCIAS + pr_end
                 *                | pr_begin + _SENTENCIA2 + pr_end
                                  | pr_begin + pr_end 
                                  | _SENTENCIA2;
                */

                //Todos deben retornar una lista

                if (nodo.ChildNodes.Count == 3)      // begin sentencias end    punto y coma no se cuenta
                {
                    if (estoyEnEsteNodo(nodo.ChildNodes[1], "SENTENCIAS"))
                    {
                        return recorrerArbol(nodo.ChildNodes[1]);   //Lista con varias instrucciones
                    }
                    else
                    {
                        LinkedList<Instruccion> listaInstru = new LinkedList<Instruccion>();
                        listaInstru.AddLast((Instruccion)recorrerArbol(nodo.ChildNodes[1]));
                        return listaInstru;                        // lista con 1 instruccion
                    }


                } else if (nodo.ChildNodes.Count == 2)// begin end        punto y como no se cuenta
                {
                    return new LinkedList<Instruccion>();       //Lista vacia
                } else
                {
                    LinkedList<Instruccion> listaInstru = new LinkedList<Instruccion>();
                    listaInstru.AddLast((Instruccion)recorrerArbol(nodo.ChildNodes[0]));
                    return listaInstru;                        // lista con 1 instruccion
                }


            }
            else if (estoyEnEsteNodo(nodo, "BLOQUE_ELSE"))
            {

                /* BLOQUE.Rule = pr_begin + SENTENCIAS + pr_end
                 *                | pr_begin + _SENTENCIA2 + pr_end
                                  | pr_begin + pr_end 
                                  | _SENTENCIA2;
                */

                //Todos deben retornar una lista

                if (nodo.ChildNodes.Count == 3)      // begin sentencias end    punto y coma no se cuenta
                {
                    if (estoyEnEsteNodo(nodo.ChildNodes[1], "SENTENCIAS"))
                    {
                        return recorrerArbol(nodo.ChildNodes[1]);   //Lista con varias instrucciones
                    }
                    else
                    {
                        LinkedList<Instruccion> listaInstru = new LinkedList<Instruccion>();
                        listaInstru.AddLast((Instruccion)recorrerArbol(nodo.ChildNodes[1]));
                        return listaInstru;                        // lista con 1 instruccion
                    }


                }
                else if (nodo.ChildNodes.Count == 2)// begin end        punto y como no se cuenta
                {
                    return new LinkedList<Instruccion>();       //Lista vacia
                }
                else
                {
                    LinkedList<Instruccion> listaInstru = new LinkedList<Instruccion>();
                    listaInstru.AddLast((Instruccion)recorrerArbol(nodo.ChildNodes[0]));
                    return listaInstru;                        // lista con 1 instruccion
                }


            }




            else if (estoyEnEsteNodo(nodo, "SENTENCIA_FOR"))
            {
                /*    SENTENCIA_FOR.Rule = pr_for + SENTENCIA_ASIGNAR + pr_to + EXPR + pr_do + BLOQUE
                 *                        |pr_for + SENTENCIA_ASIGNAR + pr_down + EXPR + pr_do + BLOQUE;
                 */

                bool boolAumentoDec = false;
                string cadena = obtenerLexema(nodo, 2);
                if (cadena == "to") boolAumentoDec = true;
                else if(cadena == "downto") boolAumentoDec = false;

                Instruccion inicio = (Instruccion)recorrerArbol(nodo.ChildNodes[1]);
                Expresion expresionFinal = (Expresion)recorrerArbol(nodo.ChildNodes[3]);

                bucles.Push(true);
                LinkedList<Instruccion> listaInstrucciones = (LinkedList<Instruccion>)recorrerArbol(nodo.ChildNodes[5]);
                bucles.Pop();

                return new For(inicio, expresionFinal, listaInstrucciones, boolAumentoDec, getLinea(nodo, 0), getColumna(nodo, 0));


            }
            else if (estoyEnEsteNodo(nodo, "SENTENCIA_WHILE"))
            {
                /*SENTENCIA_WHILE.Rule = pr_while + EXPR_RELACIONAL + pr_do + BLOQUE;*/

                bucles.Push(true);
                Expresion condicion = (Expresion)recorrerArbol(nodo.ChildNodes[1]);
                LinkedList<Instruccion> instrucciones = (LinkedList<Instruccion>)recorrerArbol(nodo.ChildNodes[3]);
                bucles.Pop();
                return new While(condicion, instrucciones, getLinea(nodo, 0), getColumna(nodo, 0));

            }

            else if (estoyEnEsteNodo(nodo, "SENTENCIA_REPEAT"))
            {
                /*            SENTENCIA_REPEAT.Rule = pr_repeat + SENTENCIAS + pr_until + EXPR 
                 */
                bucles.Push(true);
                LinkedList<Instruccion> instrucciones = (LinkedList<Instruccion>)recorrerArbol(nodo.ChildNodes[1]);
                Expresion condicion = (Expresion)recorrerArbol(nodo.ChildNodes[3]);
                bucles.Pop();

                return new Repeat(condicion, instrucciones, getLinea(nodo, 0), getColumna(nodo, 0));

            }

            else if (estoyEnEsteNodo(nodo, "SENTENCIA_BREAK"))
            {

                /*SENTENCIA_BREAK.Rule = pr_break;*/

                Break nuevaBreak = new Break(getLinea(nodo, 0), getColumna(nodo, 0));

                if (bucles.Count > 0)
                {
                    nuevaBreak.siPuedeRetornar = true;
                }

                return nuevaBreak;
            }
            else if (estoyEnEsteNodo(nodo, "SENTENCIA_CONTINUE"))
            {
                /*      SENTENCIA_CONTINUE.Rule = pr_continue;
                */
                Continue nuevaContinue = new Continue(getLinea(nodo, 0), getColumna(nodo, 0));
                if (bucles.Count > 0)
                {
                    nuevaContinue.siPuedeRetornar = true;
                }
                return nuevaContinue;
            }
            else if (estoyEnEsteNodo(nodo, "SENTENCIA_EXIT"))
            {
                /* SENTENCIA_EXIT.Rule = pr_exit + par_abierto + EXPR + par_cerrado
                                  | pr_exit+ par_abierto + par_cerrado
                                  | pr_exit;*/

                if (nodo.ChildNodes.Count == 2)
                {
                    Expresion retorno = (Expresion)recorrerArbol(nodo.ChildNodes[1]);
                    return new Exit(retorno, tiposRet.Peek(), getLinea(nodo, 0), getColumna(nodo, 0));
                }
                else
                {
                    return new Exit(tiposRet.Peek(), getLinea(nodo, 0), getColumna(nodo, 0));

                }



            }

            else if (estoyEnEsteNodo(nodo, "SENTENCIA_CASE"))
            {
                /*   SENTENCIA_CASE.Rule = pr_case + EXPR + pr_of + CASOS + pr_end
                                         | pr_case + EXPR + pr_of + CASOS + pr_else +SENTENCIA_CASOS +  pr_end + ptcoma;*/

                Expresion validacion = (Expresion)recorrerArbol(nodo.ChildNodes[1]);
                LinkedList<Case> casos = (LinkedList<Case>)recorrerArbol(nodo.ChildNodes[3]);

                if (nodo.ChildNodes.Count == 5)
                {

                    return new SwitchCase(validacion,casos,getLinea(nodo,0),getColumna(nodo,0));
                }
                else {

                    LinkedList<Instruccion> insElse = (LinkedList<Instruccion>)recorrerArbol(nodo.ChildNodes[5]);
                    Case casoElse = new Case(null, insElse, getLinea(nodo, 0), getLinea(nodo, 4));
                    return new SwitchCase(validacion, casos, casoElse, getLinea(nodo, 0), getColumna(nodo, 0));

                }


            }
            else if (estoyEnEsteNodo(nodo, "CASOS"))
            {
                /*            CASOS.Rule = MakePlusRule(CASOS, _CASO);*/

                LinkedList<Case> casos = new LinkedList<Case>();
                foreach (ParseTreeNode item in nodo.ChildNodes)
                {
                    Case caso = (Case)recorrerArbol(item);
                    casos.AddLast(caso);
                }

                return casos;

            }
            else if (estoyEnEsteNodo(nodo, "_CASO"))
            {
                /*            _CASO.Rule = PRIMITIVO + doble_pt + SENTENCIA_CASOS;*/

                // EN LA GRAMATICA, SE HIZO QUE EL DOBLE PUNTO NO APARECIERA 
                Expresion valor_Caso = (Expresion)recorrerArbol(nodo.ChildNodes[0]);
                LinkedList<Instruccion> sentencias = (LinkedList<Instruccion>)recorrerArbol(nodo.ChildNodes[1]);
                return new Case(valor_Caso, sentencias, getLinea(nodo, 0), getColumna(nodo, 0));

            }
            else if (estoyEnEsteNodo(nodo, "SENTENCIA_CASOS"))
            {
                /*            SENTENCIA_CASOS.Rule = _SENTENCIA 
                                    | pr_begin + SENTENCIAS + pr_end;*/

                LinkedList<Instruccion> instrucciones = new LinkedList<Instruccion>();
                if (estoyEnEsteNodo(nodo.ChildNodes[0], "_SENTENCIA"))
                {
                    Instruccion unico = (Instruccion)recorrerArbol(nodo.ChildNodes[0]);
                    instrucciones.AddLast(unico);
                    return instrucciones;
                }
                else
                {
                    return recorrerArbol(nodo.ChildNodes[1]);
                }

            }



            /*  ═══════════════════════════════════════════════════════════════════════════════════════════════════
             *  ══════════════════════════════                         ════════════════════════════════════════════
             *  ══════════════════════════════  FIN AREA DE SENTENCIAS ════════════════════════════════════════════
             *  ══════════════════════════════                         ════════════════════════════════════════════
             *  ═══════════════════════════════════════════════════════════════════════════════════════════════════
             */

                    /*  ═══════════════════════════════════════════════════════════════════════════════════════════════════
                     *  ══════════════════════════════                         ════════════════════════════════════════════
                     *  ══════════════════════════════   AREA DE FUNCIONES Y   ════════════════════════════════════════════
                     *  ══════════════════════════════      PROCEDIMIENTOS     ════════════════════════════════════════════
                     *  ═══════════════════════════════════════════════════════════════════════════════════════════════════
                     */

            else if (estoyEnEsteNodo(nodo, "FUNCION"))
            {
                /*  FUNCION.Rule = pr_function + identificador + par_abierto + LIST_PARAMS_FUNCION_PROC + par_cerrado + doble_pt + TIPO_DATO + ptcoma + ENCABEZADOS + BLOQUE + ptcoma
                            | pr_function + identificador + par_abierto + LIST_PARAMS_FUNCION_PROC + par_cerrado + doble_pt + identificador + ptcoma + ENCABEZADOS + BLOQUE + ptcoma;*/

                //no se cuentan los parentecis y puntos - comas

                string nombre = obtenerLexema(nodo, 1);
                nombre = nombre.ToLower();
                LinkedList<Simbolo> parametros = (LinkedList<Simbolo>)recorrerArbol(nodo.ChildNodes[2]);
                LinkedList<Instruccion> encabezados = (LinkedList<Instruccion>)recorrerArbol(nodo.ChildNodes[4]);
                LinkedList<Instruccion> instrucciones;

                if (estoyEnEsteNodo(nodo.ChildNodes[3], "TIPO_DATO"))
                {
                    TipoDatos TipoFuncion = (TipoDatos)recorrerArbol(nodo.ChildNodes[3]);

                        tiposRet.Push(TipoFuncion);     //agregamos el tipo a la pila y luego obtenemos las instrucciones
                                                        //esto para guardar el valor de retorno por si viene un exit

                    instrucciones = (LinkedList<Instruccion>)recorrerArbol(nodo.ChildNodes[5]);

                        tiposRet.Pop();

                    return new Funcion(TipoFuncion, nombre, parametros, instrucciones, encabezados, getLinea(nodo, 0), getColumna(nodo, 0));                //FUNCION PRIMITIVO
                }
                else
                {
                    string retornoFuncionStruct = obtenerLexema(nodo, 3);
                    tiposRet.Push(TipoDatos.Object);
                    instrucciones = (LinkedList<Instruccion>)recorrerArbol(nodo.ChildNodes[5]);
                    tiposRet.Pop();
                    return new Funcion(retornoFuncionStruct, nombre, parametros, instrucciones, encabezados, getLinea(nodo, 0), getColumna(nodo, 0));       //FUNCION OBJETO
                }


            }

            else if (estoyEnEsteNodo(nodo, "_PROCEDIMIENTO"))
            {
                /*            _PROCEDIMIENTO.Rule = pr_procedure + identificador + par_abierto + LIST_PARAMS_FUNCION_PROC + par_cerrado + ptcoma + ENCABEZADOS + BLOQUE + ptcoma;*/

                string nombre = obtenerLexema(nodo, 1);
                nombre = nombre.ToLower();
                LinkedList<Simbolo> parametros = (LinkedList<Simbolo>)recorrerArbol(nodo.ChildNodes[2]);
                LinkedList<Instruccion> encabezados = (LinkedList<Instruccion>)recorrerArbol(nodo.ChildNodes[3]);
                tiposRet.Push(TipoDatos.Void);
                LinkedList<Instruccion> instrucciones = (LinkedList<Instruccion>)recorrerArbol(nodo.ChildNodes[4]);
                tiposRet.Push(TipoDatos.Void);
                return new Funcion(TipoDatos.Void, nombre, parametros, instrucciones, encabezados, getLinea(nodo, 0), getColumna(nodo, 0));

            }

            else if (estoyEnEsteNodo(nodo, "LIST_PARAMS_FUNCION_PROC"))
            {
                /*LIST_PARAMS_FUNCION_PROC.Rule = MakePlusRule(LIST_PARAMS_FUNCION_PROC, ptcoma, PARAMETRO);*/

                LinkedList<Simbolo> parametros = new LinkedList<Simbolo>();

                foreach (ParseTreeNode item in nodo.ChildNodes)
                {
                    //PARAMETRO PUEDE RETORNAR UNA LISTA DE PARAMETROS SI SE DEFINE UNA LISTA DE IDES CON UN TIPO
                    LinkedList<Simbolo> temporal =  (LinkedList<Simbolo>)recorrerArbol(item);
                    foreach (Simbolo parametro in temporal)
                    {
                        parametros.AddLast(parametro);
                    }
                }

                return parametros;
            }
            else if (estoyEnEsteNodo(nodo, "PARAMETRO"))
            {
                /*  PARAMETRO.Rule = LIST_NOMBRES + doble_pt + TIPO_DATO
                             | LIST_NOMBRES + doble_pt + identificador
                             | pr_var + LIST_NOMBRES + doble_pt + TIPO_DATO
                             | pr_var + LIST_NOMBRES + doble_pt + identificador;*/

                int indiceTipo; 
                int indiceLista;
                bool porReferencia;
                
                //VERFICAMOS EN QUE INODO SE BUSCARA EL TIPO DEL DATO, O TIPO STRUCT ASI COMO SI ES POR REFERENCIA O POR VALOR
                if (nodo.ChildNodes.Count == 2) {
                    indiceTipo = 1;
                    indiceLista = 0;
                    porReferencia = false;
                }
                else {
                    indiceLista = 1;
                    indiceTipo = 2;
                    porReferencia = true;
                }

                LinkedList<Simbolo> parametros = new LinkedList<Simbolo>();

                //RECUPERACION DE LOS PARAMETROS DENTRO DE LA LISTA, YA QUE PUEDE VENIR EN LISTAS
                if (estoyEnEsteNodo(nodo.ChildNodes[indiceTipo], "TIPO_DATO"))
                {
                    //CAPTURAMOS EL TIPO DE LOS PARAMETROS, Y LOS AGREGAMOS A LA LISTA PARAMETROS
                    //SE DEFINE SI ES POR REFERENCIA O VALOR
                    TipoDatos tipoParametros = (TipoDatos)recorrerArbol(nodo.ChildNodes[indiceTipo]);
                    LinkedList<Simbolo> temporal = (LinkedList<Simbolo>)recorrerArbol(nodo.ChildNodes[indiceLista]);
                    foreach (Simbolo item in temporal)
                    {
                        parametros.AddLast(new Simbolo(tipoParametros, item.Identificador,porReferencia,item.linea,item.columna));
                    }
                }
                else
                {
                    //CAPTURAMOS EL TIPO DEL STRUCT GENERADOR Y LOS AGREGAMOS A LA LISTA DE PARAMETROS
                    //DEFINIENDO SI ES POR REFERENCIA O VALOR
                    string generador = obtenerLexema(nodo, indiceTipo);
                    LinkedList<Simbolo> temporal = (LinkedList<Simbolo>)recorrerArbol(nodo.ChildNodes[indiceLista]);
                    foreach (Simbolo item in temporal)
                    {
                        parametros.AddLast(new Simbolo(generador, item.Identificador, porReferencia, item.linea, item.columna));
                    }

                }

                return parametros;
            }
            else if (estoyEnEsteNodo(nodo, "SENTENCIA_LLAMADA"))
            {
                /*  SENTENCIA_LLAMADA.Rule = identificador + par_abierto + PARAMS + par_cerrado;*/

                LinkedList<Expresion> valores = (LinkedList<Expresion>)recorrerArbol(nodo.ChildNodes[1]);
                
                string nombreLlamada = obtenerLexema(nodo, 0);

                return new Llamada(nombreLlamada, valores,getLinea(nodo,0), getColumna(nodo,0));


            }
            else if (estoyEnEsteNodo(nodo, "PARAMS"))
            {
                /*  PARAMS.Rule = MakeStarRule(PARAMS, EXPR);*/

                LinkedList<Expresion> valores = new LinkedList<Expresion>();
                foreach (ParseTreeNode item in nodo.ChildNodes)
                {
                    valores.AddLast((Expresion)recorrerArbol(item));
                }

                return valores;

            }


            /*  ═══════════════════════════════════════════════════════════════════════════════════════════════════
             *  ══════════════════════════════                         ════════════════════════════════════════════
             *  ══════════════════════════════   FIN DE FUNCIONES Y   ════════════════════════════════════════════
             *  ══════════════════════════════      PROCEDIMIENTOS     ════════════════════════════════════════════
             *  ═══════════════════════════════════════════════════════════════════════════════════════════════════
             */

            else if (estoyEnEsteNodo(nodo, "PRIMITIVO"))
            {
                /*PRIMITIVO.Rule = numero
                | cadena
                | cadena_1
                | caracter
                | pr_true
                | pr_false
                | identificador;
                */
                return retornoDeDatosPrimitivos(nodo);
            }

            else if (estoyEnEsteNodo(nodo, "PRINCIPAL"))
            {
                /*            PRINCIPAL.Rule = pr_begin + SENTENCIAS + pr_end + pt;
                 */

                return new BeginEndPrincipal(TipoDatos.Void,"main",new LinkedList<Simbolo>(),(LinkedList<Instruccion>)recorrerArbol(nodo.ChildNodes[1]), getLinea(nodo, 0), getColumna(nodo, 0));
            }

            return null;
        }


        /** @funcion        object retornoDeDatosPrimitivos(ParseTreeNode nodo)
         *  @param          nodo
         *  @comentario     Esta función retornara un dato primitivo contenido en el parametro nodo
         */
        public object retornoDeDatosPrimitivos(ParseTreeNode nodo)
        {

            if (estoyEnEsteNodo(nodo.ChildNodes[0], "numero"))
            {
                double resultado = Convert.ToDouble(obtenerLexema(nodo, 0));

                try
                {
                    int resultado2 = Convert.ToInt32(obtenerLexema(nodo, 0));
                    return new Primitivo(resultado2,getLinea(nodo,0),getColumna(nodo,0));
                }
                catch (Exception)
                {
                    return new Primitivo(resultado, getLinea(nodo, 0), getColumna(nodo, 0));
                }
            }
            else if (estoyEnEsteNodo(nodo.ChildNodes[0], "cadena"))
            {
                string aux = obtenerLexema(nodo, 0).ToString();
                aux = aux.Replace("\\n", "\n");
                aux = aux.Replace("\\t", "\t");
                aux = aux.Replace("\\r", "\r");
                aux = aux.Substring(1, aux.Length - 2);  //esto para quitarle las comillas simples en los extremos
                return new Primitivo(aux, getLinea(nodo, 0), getColumna(nodo, 0));
            }
            else if (estoyEnEsteNodo(nodo.ChildNodes[0], "caracter"))
            {
                try
                {
                    string aux = obtenerLexema(nodo, 0);
                    char result = Convert.ToChar(aux.Substring(1, 1));
                    return new Primitivo(result, getLinea(nodo, 0), getColumna(nodo, 0));
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else if (estoyEnEsteNodo(nodo.ChildNodes[0], "true"))
            {
                return new Primitivo(true, getLinea(nodo, 0), getColumna(nodo, 0));
            }
            else if (estoyEnEsteNodo(nodo.ChildNodes[0], "false"))
            {
                return new Primitivo(false, getLinea(nodo, 0), getColumna(nodo, 0));
            }
            else if (estoyEnEsteNodo(nodo.ChildNodes[0], "identificador"))
            {
                return new Identificador(obtenerLexema(nodo, 0), getLinea(nodo, 0), getColumna(nodo, 0));
            }

            return null;
        }


        /**
         * 
         * @nodo        Nodo del arbol donde se quiere saber su nombre
         * @textoEvaluar    Es el nombre que se esta buscando en este nodo
         * @devuelve        Devuelve true -> si el nombre se encontro y false si no 
         *
         */

         public bool estoyEnEsteNodo(ParseTreeNode nodo, String textoEvaluar)
        {
            return nodo.Term.Name.Equals(textoEvaluar, System.StringComparison.InvariantCultureIgnoreCase);

        }


        /**
         *@nodo      -> nodo padre de donde se quiere obtener un lexema 
         * @posicion -> ubicación del hijo que tiene el lexeman en el nodo padre
         * @comen    -> obtiene el lexema de un token, que es un nodo hoja. 
         * 
         **/
        public string obtenerLexema(ParseTreeNode nodo, int posicion) {
            return nodo.ChildNodes[posicion].Token.Text;
        }


        public int getLinea(ParseTreeNode nodo, int posicion)
        {
            return nodo.ChildNodes[posicion].Span.Location.Line+1;
        }

        public int getLineaProduccion(ParseTreeNode nodo, int posicion)
        {
            return nodo.ChildNodes[posicion].Span.Location.Line + 1;
        }
        public int getColumnaProduccion(ParseTreeNode nodo, int posicion)
        {
            return nodo.ChildNodes[posicion].Span.Location.Column;
        }

        public int getColumna(ParseTreeNode nodo, int posicion)
        {
            return nodo.ChildNodes[posicion].Span.Location.Column;
        }

    }
}
