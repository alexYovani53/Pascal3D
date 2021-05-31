
using System;
using System.Collections.Generic;
using System.Text;
using CompiPascal.AST_.interfaces;
using CompiPascal.AST_.valoreImplicito;
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


        public string getC3(Entorno ent, AST arbol)
        {

            string codigoWrite = "";



            if(expr_imprimir != null)
            {
                foreach (Expresion item in expr_imprimir)
                {
                    if (item is Llamada) ((Llamada)item).arbolAST = arbol;
                    result3D resultExpr = item.obtener3D(ent);

                    codigoWrite += resultExpr.Codigo;
                    if (resultExpr.TipoResultado == Simbolo.TipoDatos.Integer)
                    {
                        codigoWrite += $"printf(\"%d\", (int){resultExpr.Temporal}); \n";
                    }
                    else if (resultExpr.TipoResultado == Simbolo.TipoDatos.Real)
                    {
                        codigoWrite += $"printf(\"%f\",(float){resultExpr.Temporal});\n";
                    }
                    else if (resultExpr.TipoResultado == Simbolo.TipoDatos.Char)
                    {
                        codigoWrite += $"printf(\"%c\",(char){resultExpr.Temporal});\n";
                    }
                    else if (resultExpr.TipoResultado == Simbolo.TipoDatos.String)
                    {
                        string indice = Generador.pedirTemporal();
                        string caracter = Generador.pedirTemporal();

                        string etq1 = Generador.pedirEtiqueta();
                        string finCad = Generador.pedirEtiqueta();


                        codigoWrite += "/*IMPRIMIENDO UNA CADENA*/\n";
                        codigoWrite += $"{indice} = {resultExpr.Temporal};                  /*Guardamos el inicio de la cadena */ \n";

                        //CAPTURAMOS EL PRIMER CARACTER 
                        codigoWrite += $"{etq1}:                           /*Inicio de ciclo para impresión*/ \n";

                        codigoWrite += $"   {caracter} = Heap[(int){indice}]; /*Captura del caracter*/ \n";

                        //ESTE IF SIMULA UN CICLO PARA EL RECORRDIO DE LA EXPRESION
                        codigoWrite += $"   if ({caracter}== 0) goto {finCad}; \n";

                        //IMPRESION DE LOS ASCII
                        codigoWrite += $"   printf(\"%c\", (char){caracter}); \n\n";
                        codigoWrite += $"       {indice} = {indice} + 1 ; \n";

                        codigoWrite += $"       goto {etq1}; \n";

                        codigoWrite += $"{finCad}:      \n/*FIN IMPRESION DE CADENA*/ \n\n";
                    }
                    else if (resultExpr.TipoResultado == Simbolo.TipoDatos.Boolean)
                    {

                        if (item is Primitivo)
                        {
                            string true_false = resultExpr.Temporal;
                            string VALOR = true_false == "1" ? "TRUE" : "FALSE";            //OPERADOR TERNARIO
                            codigoWrite += imprimirTRUE_FALSE(VALOR);
                        }
                        else if (item is Identificador)
                        {
                            string etiqTRUE = Generador.pedirEtiqueta();
                            string etiqFALSE = Generador.pedirEtiqueta();
                            string etiquetaSalida = Generador.pedirEtiqueta();

                            codigoWrite += $"if({resultExpr.Temporal}==1) goto {etiqTRUE};\n";
                            codigoWrite += $"goto {etiqFALSE};\n";
                            codigoWrite += $"{etiqTRUE}: \n";
                            codigoWrite += imprimirTRUE_FALSE("TRUE");
                            codigoWrite += $"goto {etiquetaSalida};\n";
                            codigoWrite += $"{etiqFALSE}:\n";
                            codigoWrite += imprimirTRUE_FALSE("FALSE");
                            codigoWrite += $"{etiquetaSalida}:\n\n";

                        }
                        else
                        {
                            string tempFinal = Generador.pedirEtiqueta();
                            codigoWrite += $"{resultExpr.EtiquetaV}: \n";
                            codigoWrite += imprimirTRUE_FALSE("TRUE");

                            codigoWrite += $"goto {tempFinal}; \n";
                            codigoWrite += $"{resultExpr.EtiquetaF}: \n";
                            codigoWrite += imprimirTRUE_FALSE("FALSE");
                            codigoWrite += $"goto {tempFinal}; \n";
                            codigoWrite += $"{tempFinal}:\n";

                        }
                    }

                }

            }


            if (saltoLinea)
            {
                codigoWrite += "printf(\"%c\", (char)10); /*imprime salto de linea*/ \n";
            }

            return codigoWrite;
        }


        public string imprimirTRUE_FALSE(string booleano)
        {

            string cadenaImpresion = "";

            string temp1 = Generador.pedirTemporal();

            cadenaImpresion += "/*INICIO DE CADENA VALOR BOOLEAN*/ \n";
            cadenaImpresion += $"{temp1} = HP; \n";          //CAPTURAMOS EL INICO DE LA CADENA 

            for (int i = 0; i < booleano.Length; i++)
            {
                cadenaImpresion += $"Heap[HP] = {(int)booleano[i]}; \n";
                cadenaImpresion += $"HP = HP + 1; \n";
            }
            cadenaImpresion += $"Heap[HP] = 0 ; \n";
            cadenaImpresion += "HP = HP +1 ; \n";

            cadenaImpresion += "/*FIN DE CADENA VALOR BOOLEANO*/ \n";

            string indice = Generador.pedirTemporal();
            string caracter = Generador.pedirTemporal();

            string inicioB = Generador.pedirEtiqueta();
            string finalB = Generador.pedirEtiqueta();

            cadenaImpresion += $"{indice} = {temp1}; \n";

            cadenaImpresion += $"{inicioB}: \n";        //ETIQUETA DE INICO IMPRESION
            cadenaImpresion += $"{caracter} = Heap[(int){indice}]; \n";  //CAPTURAMOS EL CARACTER

            cadenaImpresion += $"if( {caracter} == 0 ) goto {finalB};\n";
            cadenaImpresion += $"printf(\"%c\", (char){caracter}); \n";
            cadenaImpresion += $"{indice} = {indice}+1; \n";
            cadenaImpresion += $"goto {inicioB};\n";

            cadenaImpresion += $"{finalB}: /* FIN IMPRESION BOOLEANO*/\n\n";

            cadenaImpresion = Generador.tabular(cadenaImpresion);

            return cadenaImpresion;
        }

        public void obtenerListasAnidadas(LinkedList<string> variablesUsadas)
        {
            foreach (Expresion item in this.expr_imprimir)
            {
                item.obtenerListasAnidadas(variablesUsadas);
            }
        }
    }
}
