
using System;
using System.Collections.Generic;
using System.Text;
using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using Pascal3D.Traductor;

namespace CompiPascal.AST_.funcionesPrimitivas
{
    public class Write : Instruccion
    {

        public int tamanoPadre { get; set; }
        /*
         * @param           linea
         * @comentario      contendra la linea donde aparecio la instruccion
         */
        public int linea { get; set; }

        /*
         * @param           columna
         * @comentario      contendra la columna donde aparecio la instruccion
         */
        public int columna { get; set; }

        /*
         * @param           expr
         * @comentario      contendra la expresion que se va a imprimir
         */
        public LinkedList<Expresion> expr_imprimir;


        public bool saltoLinea { get; set; }


        public Write(LinkedList<Expresion> expr,bool saltoLinea, int linea, int columna)
        {
            this.expr_imprimir = expr;
            this.linea = linea;
            this.columna = columna;
            this.saltoLinea = saltoLinea;

        }

        public Write(bool saltolinea, int linea, int columna)
        {
            this.linea = linea;
            this.columna = columna;
            this.saltoLinea = saltolinea;
            this.expr_imprimir = null;
        }


        public string getC3(Entorno ent)
        {

            string codigoWrite = "";

            if (saltoLinea)
            {
                codigoWrite += "printf(\"%c\", (char)10); /*imprime salto de linea*/ \n";
            }

            foreach (Expresion item in expr_imprimir)
            {
                result3D resultExpr = item.obtener3D(ent);

                if(resultExpr.TipoResultado == Simbolo.TipoDatos.Integer)
                {
                    codigoWrite += $"printf(\"%d\", (int){resultExpr.Temporal}); \n";
                }
                else if(resultExpr.TipoResultado == Simbolo.TipoDatos.Real)
                {
                    codigoWrite += $"printf(\"%f\",(float){resultExpr.Temporal});\n";
                }
                else if(resultExpr.TipoResultado == Simbolo.TipoDatos.Char)
                {
                    codigoWrite += $"printf(\"%c\",(char){resultExpr.Temporal});\n";
                }
                else if(resultExpr.TipoResultado == Simbolo.TipoDatos.String)
                {
                    string indice = Generador.pedirTemporal();
                    string caracter = Generador.pedirTemporal();

                    string etq1 = Generador.pedirEtiqueta();
                    string finCad = Generador.pedirEtiqueta();

                    codigoWrite += resultExpr.Codigo;
                    codigoWrite += $"{indice} = {resultExpr.Temporal}; /*Guardamos el inicio de la cadena */ \n";

                    //CAPTURAMOS EL PRIMER CARACTER 
                    codigoWrite += $"{etq1}: /*Inicio de ciclo para impresión*/ \n";
                    codigoWrite += $"{caracter} = Heap[{indice}]; /*Captura del caracter*/ \n";

                    //ESTE IF SIMULA UN CICLO PARA EL RECORRDIO DE LA EXPRESION
                    codigoWrite += $"if ({caracter}== 0) goto {finCad}; \n";

                    //IMPRESION DE LOS ASCII
                    codigoWrite += $"printf(\"%c\", (char){caracter}); \n";
                    codigoWrite += $"{indice} = {indice} + 1 \n";
                    codigoWrite += $"goto {etq1}; \n";

                    codigoWrite += $"{finCad}: /*Fin de cadena*/ \n";
                }

            }

            return codigoWrite;
        }
    }
}
