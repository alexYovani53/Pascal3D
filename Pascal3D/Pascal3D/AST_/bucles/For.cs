using CompiPascal.AST_.cambioFlujo;
using CompiPascal.AST_.definicion;
using CompiPascal.AST_.interfaces;
using CompiPascal.AST_.valoreImplicito;
using CompiPascal.entorno_;
using Pascal3D;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.bucles
{
    public class For : Instruccion
    {
        /*      esto si el for fuese uno de alto nivel   for (int i = 0; i< a ; i++)
         *      
                Expresion condicionPaso;
                Instruccion actualizacion;
        */


        public int linea { get; set; }
        public int columna { get; set; }

        public int tamanoPadre { get; set; }

        /* @propiedad       valorInicial
         * @comentario      Esta variable almacena una instruccion de asignacion que se ejecuta al ejecutar esta clase
         */
        Instruccion valorInicial;

        /* @propiedad       valFinal
         * @comentario      Esta variable almacena el valor final del ciclo
         */
        Expresion valfinal;

        /* @propiedad       instrucciones
         * @comentario      instrucciones que se ejecutaran dentro del ciclo for
         */
        LinkedList<Instruccion> instrucciones;


        bool aumentar { get; set; }

        
        public For(Instruccion valInicial, Expresion final,LinkedList<Instruccion> instrucciones,bool aumenta, int linea, int columna)
        {
            this.valorInicial = valInicial;
            this.valfinal = final;
            this.instrucciones = instrucciones;
            this.linea = linea;
            this.columna = columna;
            this.aumentar = aumenta;

        }

        public string getC3(Entorno ent)
        {

            string nombreContador = ((Asignacion)valorInicial).variable.Identificador;
            Simbolo asignador = ent.obtenerSimbolo(nombreContador);
            Identificador contador = new Identificador(nombreContador, linea, columna);

            if(asignador == null)
            {
                Program.getIntefaz().agregarError($"La variable {nombreContador} no existe en ningun entorno",linea,columna);
                return "";
            }

            string codigoFor = "";                  /* GUARDARA EL CODIGO DE LA ESTRUCTURA DEL FOR*/
            string contenidoFor = "";               /* GUARDARA EL CODIGO DE LAS INTRUCCIONES DENTRO DEL FOR*/

            //ESTA ES LA SECCIÓN DONDE SE INICIA EL CONTADOR DEL FOR
            codigoFor += "/*ASIGNACIÓN DEL CONTADOR*/ \n";
            codigoFor += valorInicial.getC3(ent);   


            //CAPTURANDO VALOR FINAL  Y VALIDACIÓN DEL TIPO DEL PARAMETRO FINAL
            result3D finalConteo = valfinal.obtener3D(ent);
            if (finalConteo.TipoResultado != TipoDatos.Integer)
            {
                Program.getIntefaz().agregarError($"La expresion final no es un integer", linea, columna);
                return "";
            }


            codigoFor += finalConteo.Codigo;

            //DEFINIMOS LA ETIQUETA A LA QUE VUELTE CUANDO TERMINA UN CICLO
            string etiquetaCiclo = Generador.pedirEtiqueta();
            string etiquetaSalida = Generador.pedirEtiqueta();
            string etiquetaAumento = Generador.pedirEtiqueta();

            codigoFor += $"{etiquetaCiclo}:   /*INICIO DEL CICLO*/ \n\n";

            result3D buscarContador = contador.obtener3D(ent);
            

            //COMIENZO DE VALIDACIÓN DEL CONTADOR
            codigoFor += buscarContador.Codigo;


            if (aumentar)
            {
                codigoFor += $"if ({buscarContador.Temporal} > {finalConteo.Temporal}) goto {etiquetaSalida}; /*LA CONDICION DEL FOR SE A CUMPLIDO*/\n\n ";
            }
            else
            {
                codigoFor += $"if ({buscarContador.Temporal} < {finalConteo.Temporal}) goto {etiquetaSalida}; /*LA CONDICION DEL FOR SE A CUMPLIDO*/\n\n ";
            }



            //AQUI ESCRIBIMOS EL CODIGO DE TODAS LAS INSTRUCCIONES DENTRO DEL FOR

            foreach (Instruccion item in instrucciones)
            {
                contenidoFor += item.getC3(ent);
            }

            /*  ANTES DE COPIAR EL CODIGO FOR LO TABULAMOS */
            codigoFor += Generador.tabular(contenidoFor);

            contenidoFor += $"{etiquetaAumento}: \n";

            /* ANTES DE REGRESAR AL INICIO DEL CODIGO HAY QUE AUMENTAR O DECREMENTAR EL CONTADOR*/
            Operacion.Operador aumentoDecremento;
            if (aumentar) aumentoDecremento = Operacion.Operador.MAS;
            else aumentoDecremento = Operacion.Operador.MENOS;

            Expresion aumentando = new Operacion(new Identificador(nombreContador, valfinal.linea, valfinal.columna), new Primitivo(1, valfinal.linea, valfinal.columna), aumentoDecremento, linea, columna);
            Asignacion asignacion = new Asignacion(new Simbolo(nombreContador, linea, columna), aumentando, false, linea, columna);
            codigoFor += asignacion.getC3(ent);

            // REGRESAR AL INICIO Y SALIR DEL FOR
            codigoFor += $"goto {etiquetaCiclo}; /* CICLO CUMPLIDO, REGRESAMOS AL INICIO DE LA VALIDACIÓN*/\n\n";
            codigoFor += $"{etiquetaSalida}: /*FIN DEL CICLO FOR*/ \n\n";


            codigoFor =  codigoFor.Replace("#BREAK#", $"goto {etiquetaSalida};"); 
            codigoFor =  codigoFor.Replace("#CONTINUE#", $"goto {etiquetaAumento};");

            return codigoFor;
        }

    }
}
