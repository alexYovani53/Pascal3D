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

        public bool buscarSoloDireccion { get; set; }

        public string etiquetaEntornoHeap { get; set; }

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

            // OBTENER SOLO LA DIRECCIÓN DE UN IDE ES PARA USARLA COMO REFERENCIA Y NO COMO VALOR
            if (buscarSoloDireccion)
            {
                ide_buscando = buscando_Direccion(ent, ide);
            }
            // TAMBIEN CABE LA POSIBILIDAD DE QUE UNA VARIABLE NO ESTE EN EL STACK SI NO EN EL HEAP
            // ES EL CASO DE UNA VARIABLE DENTRO DE UN STRUCT EJ:     OBJETO.variable1 
            else if (etiquetaEntornoHeap !=null)
            {
                ide_buscando = Buscando_Direccion_Heap(ent, ide);
            }
            // ULTIMO CASO, BUSCAR EN EL STACK
            else 
            {
                ide_buscando = buscandoId(ent, ide);
            }
            
            return ide_buscando;
        }

        private result3D buscandoId(Entorno ent,string identificador)
        {

            result3D VALOR_ID = new result3D();
            string tempora1 = Generador.pedirTemporal();

            VALOR_ID.Codigo += $"/*BUSCANDO UN IDENTIFICADOR  >>>----- {identificador}----<<<*/\n";
            VALOR_ID.Codigo += $"{tempora1} = SP;\n";

            /* RECORREMOS TODOS LOS ENTORNOS POR EL CASO DE QUE LA VARIABLE SEA EXTERNA
             * ES POR ESO QUE SI EN EL ENTORNO ACTUAL NO ESTA LA VARIABLE, EL UNICO ENTORNO ACCESIBLE 
             * DESDE CUALQUIER FUNCION O PROCEDIMIENTO ES EL ENTORNO GLOBAL
             */
            for (Entorno actual = ent; actual != null; actual = actual.entAnterior())
            {

                foreach (Simbolo item in actual.TablaSimbolos())
                {
                    //COMPARAMOS CADA VARIABLE PARA COMPROBAR SI ESTA EN EL ENTORNO ACTUAL
                    if (item.Identificador.Equals(identificador.ToLower()))
                    {
                        string tempora2 = Generador.pedirTemporal();
                        VALOR_ID.Codigo += $"{tempora1} = {tempora1} + {item.direccion};           /*CAPTURAMOS LA DIRECCION RELATIVA DEL PARAMETRO*/\n";
                        VALOR_ID.Codigo += $"{tempora2} = Stack[(int){tempora1}];                    /*CAPTURAMOS EL VALOR ALMACENADO EN STACK*/\n";


                        /* CUANDO ES UNA REFERENCIA (PASADO EN UN PARAMETRO DENTRO DE UNA FUNCION O PROCEDIMIENTO)
                         * EL VALOR DE LA VARIABLE SE ENCUENTRA EN LA POSICION DEL STACK CONTENIDA EN EL PRIMER ACCESO AL STACK
                         * SERIA ASÍ:   
                         *              i =  a(referencia en funcion) ;    i = Stack[Stack[a.posicion]];
                         */
                        if (item.porReferencia)
                        {
                            string tempora3 = Generador.pedirTemporal();
                            if (item.Temp_auxiliar_referencias == null)
                            {
                                VALOR_ID.Codigo += $"{tempora3} = Stack[(int){tempora2}]; /* variable por referencia, ahora si tenemos el valor*/\n";
                            }
                            else
                            {
                                string etiq1 = Generador.pedirEtiqueta();
                                string etiq2 = Generador.pedirEtiqueta();

                                // SI LA ETIQUETA ES IGUAL A 1, EL VALOR ESTA EN EL HEAP, DE LO CONTRARIO ESTA EN EL sTACK
                                VALOR_ID.Codigo += $"if( {item.Temp_auxiliar_referencias} == 1) goto {etiq1};\n";
                                VALOR_ID.Codigo += $"   {tempora3} = Stack[(int){tempora2}]; /* variable por referencia, ahora si tenemos el valor*/\n";
                                VALOR_ID.Codigo += $"    goto {etiq2};\n";
                                VALOR_ID.Codigo += $"{etiq1}:\n";
                                VALOR_ID.Codigo += $"   {tempora3} = Heap[(int){tempora2}]; /* variable por referencia, ahora si tenemos el valor*/\n";
                                VALOR_ID.Codigo += $"{etiq2}:\n";
                            }

                          
                            VALOR_ID.Temporal = tempora3;
                        }
                        else VALOR_ID.Temporal = tempora2;

                        VALOR_ID.Codigo += "/*IDENTIFICADOR ENCONTRADO*/\n\n\n";
                        VALOR_ID.TipoResultado = item.Tipo;

                        return VALOR_ID;
                    }
                }

                // SI NO SE  ENCONTRADO LA VARIABLE, SOLO CUANDO LLEGAMOS AL ENTORNO GLOBAL (cuando este no tiene un entorno anterior)
                // ES QUE IGUALAMOS A 0 LA VARIABLE QUE CONTIENE EL PUNTERO AL ENTORNO ACTUAL 
                if (actual.entAnterior() != null)
                {
                    VALOR_ID.Codigo += $"{tempora1} = 0;             /*Retrocedemos al entorno global*/\n";
                }
            }

            return new result3D();
        }



        private result3D buscando_Direccion(Entorno ent, string identificador)
        {

            result3D DIRECCION = new result3D();

            string tempora1 = Generador.pedirTemporal();

            DIRECCION.Codigo += $"/***********************BUSCANDO UN IDENTIFICADOR  >>>----- {identificador}----<<<*/\n";
            DIRECCION.Codigo += $"{tempora1} = SP;\n";

            /* RECORREMOS TODOS LOS ENTORNOS POR EL CASO DE QUE LA VARIABLE SEA EXTERNA...........
             * ES POR ESO QUE SI EN EL ENTORNO ACTUAL NO ESTA LA VARIABLE, SETEAMOS EN 0 EL TEMPORAL QUE CONTIENE LA REFERENCIA 
             * DEL ENTORNO ACTUAL
             *                     { temporal1} ver arriba;
             */
            for (Entorno actual = ent; actual != null; actual = actual.entAnterior())
            {

                foreach (Simbolo item in actual.TablaSimbolos())
                {
                    //COMPARAMOS CADA VARIABLE PARA COMPROBAR SI ESTA EN EL ENTORNO ACTUAL
                    if (item.Identificador.Equals(identificador.ToLower()))
                    {
                        string tempora2 = Generador.pedirTemporal();

                        // SE PUEDE VER QUE NO SE ACCEDE AL STACK SI NO SOLO A SU POSICIÓN QUE APUNTA A UNA POSICION EN EL STACK
                        DIRECCION.Codigo += $"{tempora1} = {tempora1} + {item.direccion};           /*CAPTURAMOS LA DIRECCION RELATIVA DEL PARAMETRO*/\n";
                        DIRECCION.Temporal = tempora1;
       
                        DIRECCION.Codigo += "/***********************IDENTIFICADOR ENCONTRADO*/\n\n\n";

                        DIRECCION.TipoResultado = item.Tipo;
                        return DIRECCION;
                    }
                }

                // SI NO SE  ENCONTRADO LA VARIABLE, SOLO CUANDO LLEGAMOS AL ENTORNO GLOBAL (cuando este no tiene un entorno anterior)
                // ES QUE IGUALAMOS A 0 LA VARIABLE QUE CONTIENE EL PUNTERO AL ENTORNO ACTUAL 
                if (actual.entAnterior() != null)
                {
                    DIRECCION.Codigo += $"{tempora1} = 0;             /*Retrocedemos entre los entornos*/\n";
                }
            }

            return new result3D();
        }

        private result3D Buscando_Direccion_Heap(Entorno ent, string identificador)
        {

            result3D DIRECCION_EN_HEAP = new result3D();

            // DEVEMOS TENER UN APUNTADOR AL ENTORNO DE UN OBJETO 
            if (etiquetaEntornoHeap == null) return DIRECCION_EN_HEAP;
            string tempora1 = Generador.pedirTemporal();

            DIRECCION_EN_HEAP.Codigo += $"/***********************BUSCANDO UN IDENTIFICADOR  >>>----- {identificador}----<<<*/\n";
            DIRECCION_EN_HEAP.Codigo += $"{tempora1} = {etiquetaEntornoHeap};\n";


            foreach (Simbolo item in ent.TablaSimbolos())
            {
                //COMPARAMOS CADA VARIABLE PARA COMPROBAR SI ESTA EN EL ENTORNO ACTUAL
                if (item.Identificador.Equals(identificador.ToLower()))
                {

                    DIRECCION_EN_HEAP.Codigo += $"{tempora1} = {tempora1} + {item.direccion};           /*CAPTURAMOS LA DIRECCION RELATIVA DEL PARAMETRO*/\n";
                    DIRECCION_EN_HEAP.Temporal = tempora1;

                    DIRECCION_EN_HEAP.Codigo += "/***********************IDENTIFICADOR ENCONTRADO*/\n\n\n";

                    DIRECCION_EN_HEAP.TipoResultado = item.Tipo;
                    return DIRECCION_EN_HEAP;
                }
            }
            
            return new result3D();
        }


        public void obtenerListasAnidadas(LinkedList<string> variablesUsadas)
        {
            if (!variablesUsadas.Contains(ide))
            {
                variablesUsadas.AddLast(ide);
            }

        }


    }
}
