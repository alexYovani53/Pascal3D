using CompiPascal.AST_.interfaces;
using CompiPascal.AST_.valoreImplicito;
using CompiPascal.entorno_;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.cambioFlujo
{
    public class Exit : Expresion,Instruccion
    {


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
        public int linea { get ; set ; }
        public int columna { get; set ; }

        /* @parametro       bool          salidaVoid        
         * @comentario      Esta variabe guarda una bandera que indica si e exit esta en una funcion o metodo
         */
        private TipoDatos tipoRet { get; set; }

        /* @parametro       Expresion     valorSalida       
         * @comentario      Esta variabe guarda el valor de salida que se devuelve por el exit
         */
        private Expresion valorSalida { get; set; }



        public Exit(Expresion expr, TipoDatos salidaVoid, int linea, int columna)
        {
            this.valorSalida = expr;
            this.tipoRet = salidaVoid;
            this.linea = linea;
            this.columna = columna;

        }
        public Exit(TipoDatos salidavoid, int linea, int columna)
        {
            this.tipoRet = salidavoid;
            this.linea = linea;
            this.columna = columna;
        }

        public string getC3(Entorno ent, AST arbol)
        {

            string codigo = "/*CODIGO PARA INSTRUCCIÓN RETURN*/\n";
            string temp1 = Generador.pedirTemporal();

            //CAPTURAMOS EL TAMAÑO DEL ENTORNO ACTUAL
            codigo += $"{temp1} = SP + 0; /*En el entorno actual el retorno esta en SP + 0 (inicio) del entorno*/\n";
            
            if(valorSalida == null)
            {
                codigo += $"Stack[(int){temp1}]=-1; /* Retorno tipo void*/ \n";
                codigo += "#EXIT#\n";
            }
            else
            {
                result3D resultExpresion = validarRetorno(ent);
                codigo += resultExpresion.Codigo;
                codigo += $"Stack[(int){temp1}] = {resultExpresion.Temporal}; /* Asiganción de valor de retorno */\n";
                codigo += "#EXIT# \n";
            }
            codigo += "/*FINAL CODIGO INSTRUCCION RETURN*/\n";


            return Generador.tabular( codigo);
        }

        public result3D obtener3D(Entorno ent)
        {
            result3D retorno = new result3D();
            retorno.Codigo = getC3(ent,null);
            retorno.Temporal = Generador.pedirTemporal();
            retorno.TipoResultado = tipoRet;

            return retorno;
        }

        /* MODIFICA EL RESULTADO CUANDO LA EXPRESIÓN A RETORNAR ES UNA OPERACIÓN QUE DEVUELVE UN IF (COMPARACIÓN --> TIPO BOOLEANO)  */
        public result3D validarRetorno(Entorno ent)
        {
            result3D resultExpresion = valorSalida.obtener3D(ent);

            // COMPROBAMOS QUE LA OPERACIÓN HAYA GENERADO CODIGO DE UN IF Y "NO DEVUELVE NADA EN EL TEMPORAL"
            if(valorSalida is Operacion && resultExpresion.TipoResultado == TipoDatos.Boolean && resultExpresion.Temporal.Equals(""))
            {
                string temporal = Generador.pedirTemporal();
                string salida = Generador.pedirEtiqueta();
                result3D Nuevo = new result3D();
                Nuevo.Codigo = resultExpresion.Codigo;


                Nuevo.Codigo += $"{resultExpresion.EtiquetaV}: \n";
                Nuevo.Codigo += Generador.tabularLinea($"{temporal} = 1;",2);
                Nuevo.Codigo += Generador.tabularLinea($"goto {salida};",2);
                Nuevo.Codigo += $"{resultExpresion.EtiquetaF}:\n"; 
                Nuevo.Codigo += Generador.tabularLinea($"{temporal} = 0;",2);
                Nuevo.Codigo += $"{salida}: \n";

                Nuevo.Temporal = temporal;
                Nuevo.TipoResultado = TipoDatos.Boolean;
                return Nuevo;
            }

            return resultExpresion;
        }

        public void obtenerListasAnidadas(LinkedList<string> variablesUsadas)
        {
            valorSalida.obtenerListasAnidadas(variablesUsadas);
        }
    }
}
