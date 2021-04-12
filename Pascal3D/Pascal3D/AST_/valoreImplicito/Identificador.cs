using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using CompiPascal.entorno_.simbolos;
using Pascal3D;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.valoreImplicito
{


    /**
     * @class       identificador
     * @comentario  Clase que representa la instancia de un identificador en el lenguaje
     *              Un identificador puede tener implicito un objeto, una variable o incluso un arreglo.
     */

    public class Identificador:Expresion
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

        /**
         * @propiedad       ide
         * @comentario      Esta variable almacenara la letra del identificador que aparece en el
         *                  codigo fuente de la entrada.
         */
        private string ide { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }
        public int tamanoPadre { get ; set; }

        public bool buscar_puntero { get; set; }

        public Identificador(string letra, int linea, int columna)
        {
            this.ide = letra;
            this.linea = linea;
            this.columna = columna;
        }



        public string nombre()
        {
            return ide;
        }

        public result3D obtener3D(Entorno ent)
        {

            result3D ide_buscando; 

            if (buscar_puntero)
            {
                ide_buscando = buscando_Direccion(ent, ide);
            }
            else
            {
                ide_buscando = buscandoId(ent, ide);
            }
            
            return ide_buscando;
        }

        private result3D buscandoId(Entorno ent,string identificador)
        {

            result3D regresos = new result3D();
            string tempora1 = Generador.pedirTemporal();

            regresos.Codigo += $"/*BUSCANDO UN IDENTIFICADOR*/\n";
            regresos.Codigo += $"{tempora1} = SP;\n";

            /* RECORREMOS TODOS LOS ENTORNOS POR EL CASO DE QUE LA VARIABLE SEA EXTERNA
             * ES POR ESO QUE SI EN EL ENTORNO ACTUAL NO ESTA LA VARIABLE, RESTAMOS EL TAMAÑO DEL ENTORNO ACTUAL 
             * PARA OBTENER CORRECTAMENTE LOS VALORES*/
            for (Entorno actual = ent; actual != null; actual = actual.entAnterior())
            {

                foreach (Simbolo item in actual.TablaSimbolos())
                {
                    //COMPARAMOS CADA VARIABLE PARA COMPROBAR SI ESTA EN EL ENTORNO ACTUAL
                    if (item.Identificador.Equals(identificador.ToLower()))
                    {
                        string tempora2 = Generador.pedirTemporal();
                        regresos.Codigo += $"{tempora1} = {tempora1} + {item.direccion};           /*CAPTURAMOS LA DIRECCION RELATIVA DEL PARAMETRO*/\n";
                        regresos.Codigo += $"{tempora2} = Stack[(int){tempora1}];                    /*CAPTURAMOS EL VALOR ALMACENADO EN STACK*/\n";


                        /* CUANDO ES UNA REFERENCIA (PASADO EN UN PARAMETRO DENTRO DE UNA FUNCION O PROCEDIMIENTO)
                         * EN ESTE CASO NO SE NECESITA EL VALOR SI NO LA DIRECCIÓN DEL STACK DONDE ESTA*/
                        if (item.porReferencia)
                        {
                            string tempora3 = Generador.pedirTemporal();
                            regresos.Codigo += $"{tempora3} = Stack[(int){tempora2}]; /* variable por referencia, ahora si tenemos el valor*/\n";
                            regresos.Temporal = tempora3;
                        }
                        else regresos.Temporal = tempora2;

                        regresos.Codigo += "/*IDENTIFICADOR ENCONTRADO*/\n\n\n";

                        regresos.TipoResultado = item.Tipo;
                        return regresos;
                    }
                }

                if (actual.entAnterior() != null)
                {
                    regresos.Codigo += $"{tempora1} = {tempora1} - {actual.entAnterior().tamano};             /*Retrocedemos entre los entornos*/\n";
                }
            }

            return new result3D();
        }



        private result3D buscando_Direccion(Entorno ent, string identificador)
        {

            result3D regresos = new result3D();
            string tempora1 = Generador.pedirTemporal();

            regresos.Codigo += $"/***********************BUSCANDO UN IDENTIFICADOR*/\n";
            regresos.Codigo += $"{tempora1} = SP;\n";

            /* RECORREMOS TODOS LOS ENTORNOS POR EL CASO DE QUE LA VARIABLE SEA EXTERNA
             * ES POR ESO QUE SI EN EL ENTORNO ACTUAL NO ESTA LA VARIABLE, RESTAMOS EL TAMAÑO DEL ENTORNO ACTUAL 
             * PARA OBTENER CORRECTAMENTE LOS VALORES*/
            for (Entorno actual = ent; actual != null; actual = actual.entAnterior())
            {

                foreach (Simbolo item in actual.TablaSimbolos())
                {
                    //COMPARAMOS CADA VARIABLE PARA COMPROBAR SI ESTA EN EL ENTORNO ACTUAL
                    if (item.Identificador.Equals(identificador.ToLower()))
                    {
                        string tempora2 = Generador.pedirTemporal();

                        regresos.Codigo += $"{tempora1} = {tempora1} + {item.direccion};           /*CAPTURAMOS LA DIRECCION RELATIVA DEL PARAMETRO*/\n";
                        regresos.Temporal = tempora1;
       
                        regresos.Codigo += "/***********************IDENTIFICADOR ENCONTRADO*/\n\n\n";

                        regresos.TipoResultado = item.Tipo;
                        return regresos;
                    }
                }

                if (actual.entAnterior() != null)
                {
                    regresos.Codigo += $"{tempora1} = {tempora1} - {actual.entAnterior().tamano};             /*Retrocedemos entre los entornos*/\n";
                }
            }

            return new result3D();
        }



    }
}
