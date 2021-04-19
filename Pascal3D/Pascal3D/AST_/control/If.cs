using CompiPascal.AST_.cambioFlujo;
using CompiPascal.AST_.interfaces;
using CompiPascal.AST_.valoreImplicito;
using CompiPascal.entorno_;
using Pascal3D;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.AST_.control
{


    class If:Condicional, Instruccion
    {

        public int tamanoPadre { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }

        /* @propiedad       instruccionesElse
         * @comentario      Esta propiedad almacena las instrucciones que se ejecutan condo el if principal no se cumple
         */
        LinkedList<Instruccion> instruccionesElse;

        /* @propiedad       instruccionesElse_If
         * @comentario      Esta propiedad almacena las instrucciones que se ejecutan para los demas if en la secuencia -> if, else if, else if...... else 
         */
        LinkedList<Instruccion> instruccionesElse_if;


        /* @construcro          Este constructor se utiliza para cuando la sentencia if viene acompañada de 
         *                      sentencias else-if 
         *                      
         * @parametro       condicion               expresion que representa la condicion
         * @parametro       instrucciones           instrucciones del if principal
         * @parametro       instrucciones_else      instrucciones del bloque else
         * @parametro       instrucciones_else_if   lista de sentencias if
         */

        public If(Expresion condicion, LinkedList<Instruccion> instrucciones, LinkedList<Instruccion> instrucciones_else, LinkedList<Instruccion> instrucciones_else_if,
                            int linea, int columna) : base(condicion, instrucciones)
        {
            this.instruccionesElse = instrucciones_else;
            this.instruccionesElse_if = instrucciones_else_if;
            this.linea = linea;
            this.columna = columna;
        }


        /* @construcro          Este constructor se utiliza para cuando la sentencia if viene sola
         *
         * @parametro       condicion       expresion que representa la condicion
         * @parametro       instrucciones   instrucciones del if principal
         */

        public If(Expresion condicion, LinkedList<Instruccion> instrucciones, int linea, int columna) : base(condicion, instrucciones)
        {
            this.instruccionesElse = new LinkedList<Instruccion>();
            this.instruccionesElse_if = new LinkedList<Instruccion>();
            this.linea = linea;
            this.columna = columna;
        }

        public string getC3(Entorno ent, AST arbol)
        {

            result3D result = new result3D();
            result3D condicion = new result3D();
            string etiquetaSalida = Generador.pedirEtiqueta();

            if (exprCondicional is Operacion)
            {
                ((Operacion)exprCondicional).etiquetaFalsa = Generador.pedirEtiqueta();
                ((Operacion)exprCondicional).etiquetaVerdadera = Generador.pedirEtiqueta();
                condicion = exprCondicional.obtener3D(ent);

            }
            else if (exprCondicional is Llamada)
            {
                Operacion comparacion = new Operacion(exprCondicional, new Primitivo(true, linea, columna), Operacion.Operador.IGUAL, linea, columna);
                comparacion.etiquetaFalsa = Generador.pedirEtiqueta();
                comparacion.etiquetaVerdadera = Generador.pedirEtiqueta();
                condicion = comparacion.obtener3D(ent);
            }


            result.Codigo += "/****************************  INSTRUCCION IF *************/\n";
            result.Codigo += condicion.Codigo;
            result.Codigo += condicion.EtiquetaV+" :               /*Continua el codigo de las instrucciones*/\n";
            result.Codigo += generarCodigoInstrucciones(instrucciones, ent,arbol);

            result.Codigo += $" goto {etiquetaSalida} ;             /*etiqueta salida*/ \n\n\n";


            result.Codigo += condicion.EtiquetaF+ " :                  /*Etiqueta falsa*/ \n";

            foreach (Instruccion item in instruccionesElse_if)
            {
                If pivote = (If)item;
                pivote.exprCondicional.etiquetaFalsa = Generador.pedirEtiqueta();
                pivote.exprCondicional.etiquetaVerdadera = Generador.pedirEtiqueta();
                result3D ifElse_result = pivote.exprCondicional.obtener3D(ent);

                result.Codigo += ifElse_result.Codigo;
                result.Codigo += ifElse_result.EtiquetaV + " :\n";

                result.Codigo += generarCodigoInstrucciones(pivote.instrucciones, ent,arbol);

                result.Codigo += $" goto {etiquetaSalida} ; /*etiqueta salida*/ \n\n\n";           


                result.Codigo += ifElse_result.EtiquetaF + " : \t";

            }

            /* SI HAY ALGUNA INSTRUCCIÓN PARA EL ELSE DEL IF, SE RECUPERAN  */
            if (instruccionesElse.Count > 0)
            {
                result.Codigo += "//INSTRUCCIONES DEL ELSE \n";
                result.Codigo += generarCodigoInstrucciones(instruccionesElse, ent,arbol);        
            }


            result.Codigo += etiquetaSalida + ": \n";
            result.Codigo += "/****************************  FIN INSTRUCCION IF *************/";

            return result.Codigo;
        }

        public string generarCodigoInstrucciones(LinkedList<Instruccion> instrucciones,Entorno ent, AST arbol)
        {

            string codigo = "";

            foreach (Instruccion item in instrucciones)
            {
                codigo += item.getC3(ent,arbol);
            }

            return Generador.tabular(codigo);
        }
    }
}


