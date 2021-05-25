using CompiPascal.AST_.definicion;
using CompiPascal.AST_.definicion.arrego;
using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using CompiPascal.entorno_.simbolos;
using Pascal3D;
using Pascal3D.entorno_.simbolos;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.valoreImplicito
{
    public class Llamada : Expresion, Instruccion
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




        public int linea { get; set ;}
        public int columna { get; set; }



        /* @parametro   string      nombreLlamada       nombre del la funcion o  procedimiento al que se llama
         **/
        public  string nombreLlamada { get; set; }

        /* @parametro   LinkedList<Expresion>      expresionesValor       lista de las expresiones a asignar en los parametros de la funcion
         */
        public LinkedList<Expresion> expresionesValor { get; set; }


        public Llamada(string nombreLlamada, LinkedList<Expresion> expresiones,int linea, int columna)
        {
            this.nombreLlamada = nombreLlamada;
            this.expresionesValor = expresiones;
            this.linea = linea;
            this.columna = columna;
        }


        public result3D obtener3D(Entorno ent)
        {

            Funcion funcionLlamada = ent.obtenerFuncion(nombreLlamada);

            if (funcionLlamada == null)
            {
                Program.getIntefaz().agregarError("La funcion no se encontro", linea, columna);
                return new result3D();
            }

            result3D retorno = new result3D();
            retorno.Codigo = getC3(ent,null);


            if (funcionLlamada.Tipo == TipoDatos.Void)
            {
                retorno.TipoResultado = TipoDatos.Void;
            }

            //ACÁ OBTENEMOS EL RESULTADO DE LA FUNCION SI ESTE ES DE ALGUN TIPO
            string temp1 = Generador.pedirTemporal();   //GUARDARA la direccion para entrar al entorno de la funcion
            string temp2 = Generador.pedirTemporal();
            string temp3 = Generador.pedirTemporal();



            retorno.Codigo += $"{temp1} = SP + {ent.tamano};                    /*ENTRAMOS AL ENTORNO DE LA FUNCION*/\n";
            retorno.Codigo += $"{temp2} = {temp1} + 0;                          /*NOS TRASLADAMOS AL PRIMER PARAMETRO DE LA FUNCION (AQUI ESTA EL RETURN)*/\n";
            retorno.Codigo += $"{temp3} = Stack[(int){temp2}];                  /*CAPTURAMOS EL VALOR DEL RETUR*/\n";

            retorno.Temporal = temp3;
            retorno.TipoResultado = funcionLlamada.Tipo;

            return retorno;
        }



        public string getC3(Entorno ent, AST arbol)
        {
            Funcion funcionLLamada = ent.obtenerFuncion(nombreLlamada);
            Entorno entornoFuncion = new Entorno(ent, nombreLlamada);
            entornoFuncion.tamano = 1; //HACEMOS ESTO PARA RESERVAR EL ESPACIO DEL RETORNO AL INICIO DE LAS VARIABLES 
            // HEAP_FUNCION [ --retorno--  , -- param1--, -- param2--, -- paramN--, -- VariableFuncion ( PASCAL )--  ]

            if (funcionLLamada == null)
            {
                Program.getIntefaz().agregarError("La funcion no se encontro", linea, columna);
                return "";
            }

            string codigo = $"/* PREPARACION DE VARIABLES PARA LA FUNCION {funcionLLamada.Identificador}*/\n";

            /*  OBTENEMOS EL 3D QUE REPRESENTAN LOS VALORES PASADOS EN LOS PARAMETROS
             *  
             *  PERO ANTES, BUSCAMOS ENTRE LAS EXPRESIONES QUE SERAN PASADAS A LA FUNCIÓN, UN IDENTIFICADOR QUE VAYA COMO REFERENCIA
             *  YA QUE AL ENVIARLA A LA FUNCIÓN NO SE ENVIA SU VALOR SI NO SU DIRECCIÓN
             */
            buscarReferencia(funcionLLamada);

            LinkedList<result3D> parametros = new LinkedList<result3D>();
            int i = 0;
            foreach (Expresion item in expresionesValor)
            {
                result3D valorExpr = item.obtener3D(ent);

                
                if ((item is Operacion || item is Primitivo) &&  funcionLLamada.ListaParametros.ElementAt(i).porReferencia){
                    Program.getIntefaz().agregarError("La expresión por referencia debe ser un identificador", linea, columna); return "";
                }

                parametros.AddLast(valorExpr);
                i++;
            }


            //  GENERAMOS UN TEMPORAL PARA QUE LAS DECLARACIONES SE HAGAN DESPUES DEL ENTORNO ACTUAL 
            //  esto porque SP esta en el entorno actual y para no moverlo, se crea un temporal con la nueva posicion (stacK) del entorno
            //  de la funcion a llamar
            string temporalEntorno = Generador.pedirTemporal();
            codigo += Generador.tabular($"{temporalEntorno} = SP + {ent.tamano}; \n\n");

            string codigoParams = verificarParametros(parametros,  funcionLLamada.ListaParametros, entornoFuncion,temporalEntorno,arbol);
            codigo += Generador.tabular(codigoParams);


            codigo += $"SP = SP + {ent.tamano};  /*Estamos sumando el tamaño del entorno actual para pasar al siguiente*/\n";
            codigo += $"{funcionLLamada.Identificador}();\n";
            codigo += $"SP = SP -{ent.tamano};  /*Estamos restando el tamño sumando previamente para regresar al entorno actual*/\n";

            return codigo;
        }





        private string  verificarParametros(LinkedList<result3D> valores, LinkedList<Simbolo> parametrosFuncProc, Entorno ent,string temporalEntorno,AST arbol)
        {
            string codigoParams = "";
            int numeroParametros = parametrosFuncProc.Count;
            int numeroRecibe = valores.Count;

            if (numeroParametros != numeroRecibe)
            {
                Program.getIntefaz().agregarError("Se esperaban " + numeroParametros + " y solo se encontraron " + numeroRecibe, linea, columna);
                return "";
            }

            /*  ESTAS VARIABLES GUARDARAN TODOS LOS VALORES NECESARIOS PARA VALIDAR LA CONSISTENCIA DE LOS DATOS*/
            Simbolo parametroActual;
            string nombreParam;
            TipoDatos tipoParametro;

            result3D valorActual;
            TipoDatos tipoExpresion;


            /*  RECORREMOS TODOS LOS PARAMETROS QUE LA FUNCION TIENE PARA DEFINIRLOS*/
            for (int i = 0; i < parametrosFuncProc.Count; i++)
            {
                //CAPTURA DEL SIMBOLO QUE CONTIENE EL NOMBRE DEL PARAMETRO ACTUAL
                parametroActual = parametrosFuncProc.ElementAt(i);
                tipoParametro = parametroActual.Tipo;
                nombreParam = parametroActual.Identificador;

                //CAPTURAMOS LOS VALORES 
                valorActual = valores.ElementAt(i);
                tipoExpresion = valorActual.TipoResultado;


                //COMPARAMOS LOS TIPOS

                if (parametroActual.structGenerador !=null && !parametroActual.structGenerador.Equals(""))
                {
                    Struct aux1 = arbol.retornarEstructura(parametroActual.structGenerador);
                    Arreglo aux2 = arbol.retornarArreglo(parametroActual.structGenerador);

                    tipoParametro = aux1 != null ? TipoDatos.Object : TipoDatos.Array;
                }

                if(tipoParametro != tipoExpresion ) {
                    Program.getIntefaz().agregarError($"Error de tipos {tipoParametro} -> {tipoExpresion} " + numeroRecibe, linea, columna);
                    return "";
                }


                //COMPROBAMOS SI EL VALOR ES POR REFERENCIA O POR VALOR
                if (parametroActual.porReferencia)
                {
                    codigoParams += valorActual.Codigo;
                    codigoParams += valoresReferencia(parametroActual, tipoParametro, nombreParam, valorActual, ent, temporalEntorno,i,arbol);
                }
                else
                {

                    //VALIDAMOS EL TIPO DEL PARAMETRO
                    if(tipoParametro == TipoDatos.Object || tipoParametro == TipoDatos.Array )
                    {
                        string temp2 = Generador.pedirTemporal();
                        codigoParams += $"/*Declaración de parametro {parametroActual.Identificador} ---------<>>>> POR VALOR*/\n";
                        codigoParams += $"    {temp2} = {temporalEntorno} + {ent.tamano};  /* El parametro va en la ultima posicion del entorno de la funcion*/\n";

                        if ( valorActual.Referencia is Objeto )
                        {
                            hacerCopia nuevaCopia = new hacerCopia((Objeto)valorActual.Referencia, null, valorActual.Temporal);
                            result3D copia = nuevaCopia.obtener3D(ent);

                            codigoParams += valorActual.Codigo;
                            codigoParams += "\n\n";
                            codigoParams += Generador.tabular(copia.Codigo);
                            codigoParams += $"    Stack[(int){temp2}] = {copia.Temporal};\n";
                        }
                        else if(valorActual.Referencia is ObjetoArray)
                        {
                            hacerCopia nuevaCopia = new hacerCopia(null,(ObjetoArray)valorActual.Referencia, valorActual.Temporal);
                            result3D copia = nuevaCopia.obtener3D(ent);

                            codigoParams += valorActual.Codigo;
                            codigoParams += "\n\n";
                            codigoParams += Generador.tabular(copia.Codigo);
                            codigoParams += $"    Stack[(int){temp2}] = {copia.Temporal};\n";

                        }
                        else 
                        {

                            codigoParams += valorActual.Codigo;
                            codigoParams += "\n\n";
                            codigoParams += $"    Stack[(int){temp2}] ={valorActual.Temporal};\n";
                        }


                        codigoParams += $"/* FIN Declaración de parametro {parametroActual.Identificador} ---------<>>>> POR VALOR*/\n";
                        ent.tamano++;

                    }
                    else
                    {
                        Simbolo simboloParam = new Simbolo(tipoParametro, nombreParam, false,0,0, parametroActual.linea, parametroActual.columna);
                        Declaracion parametro = new Declaracion(simboloParam,valorActual);
                        parametro.TemporalCambioEntorno = temporalEntorno;
                        codigoParams+=parametro.getC3(ent,null);
                    }

                }

            }

            return codigoParams;
        }



        /* Esta funcion se encarga unicamente de las validaciones correspondientes para poder trabajar con variables por referencia
         * se separa el codigo para no tener un metodo demasiado extenso*/

        private string valoresReferencia(Simbolo parametroActual, TipoDatos tipoParametro, string nombreParam, result3D ValorRef, Entorno ent,string temporalCambio,int posicionExpresionValor,AST arbol)
        {                    // SI EL PARAMETRO ESPERADO ES POR REFERENCIA, SE DEVE VALIDAR QUE ESTE SEA UN IDENTIFICADOR Y NO UN VALOR PRIMITIVO

            string codigo = "";

            if (tipoParametro == TipoDatos.Object)
            {

                /*  Esta VALIDACIÓN  se hace ya que cuando se pasa un valor por referencia, en el caso de pascal, que existen
                     *  funciones anidadas, es decir, definición de funciones dentro de una funcion. 
                     *  
                     *  sería algo como    public void funcionpadre(){
                     *                          void funcionHija(){
                     *                          }
                     *                     }
                     *            
                     *  se puede que el parametro del padre sea una referencía, y al pasarsela al hijo se pasa la referencía también, entonces
                     *  se debe validar que se pase correctamente. 
                     * 
                     */
                if (expresionesValor.ElementAt(posicionExpresionValor) is Identificador)
                {

                    string nombreRef_ref = ((Identificador)expresionesValor.ElementAt(posicionExpresionValor)).nombre();
                    Simbolo referencia = ent.obtenerSimbolo(nombreRef_ref);

                    // validar que la variable referenciada ya sea una referencia.
                    if (referencia != null && referencia.porReferencia)
                    {
                        // Previamente se tenía que cuando se pasa una variable por referencia se pasa solo su dirección
                        // pero en este caso, cuando se vuelve a pasar la misma referencía necesitamos acceder al Stack en la posicion que se 
                        // devuelve al obtener el codigo 3D 
                        string temporal = Generador.pedirTemporal();
                        ValorRef.Codigo += $"{temporal} = Stack[(int){ValorRef.Temporal}];\n";
                        ValorRef.Temporal = temporal;
                    }
                }

                string temp2 = Generador.pedirTemporal();
                string temp1 = Generador.pedirTemporal();

                codigo += $"/*Declaración de parametro {parametroActual.Identificador} ---------<>>>> POR REFERENCIA*/\n";

                codigo += $"    {temp1} = Stack[(int){ValorRef.Temporal}];\n\n";

                codigo += $"    {temp2} = {temporalCambio} + {ent.tamano};\n";
                codigo += $"    Stack[(int){temp2}] = {temp1};\n";
                codigo += $"/* FIN Declaración de parametro {parametroActual.Identificador} ---------<>>>> POR REFERENCIA*/\n";
                ent.tamano++;

            }
            else if(tipoParametro == TipoDatos.Array)
            {

                string temp1 = Generador.pedirTemporal();
                string temp2 = Generador.pedirTemporal();

                codigo += $"/*Declaración de parametro {parametroActual.Identificador} ---------<>>>> POR REFERENCIA*/\n";
                
                //codigo += $"    {temp1} = Stack[(int){ValorRef.Temporal}];\n\n";  esta linea no es necesaria debido a que si se hace esto al pasar la referencia no funciona correctamente
                codigo += $"    {temp2} = {temporalCambio} + {ent.tamano};\n";
                codigo += $"    Stack[(int){temp2}] = {ValorRef.Temporal};\n";

                codigo += $"/* FIN Declaración de parametro {parametroActual.Identificador} ---------<>>>> POR REFERENCIA*/\n";
                ent.tamano++;
            }
            else
            {

                /*  Esta VALIDACIÓN  se hace ya que cuando se pasa un valor por referencia, en el caso de pascal, que existen
                 *  funciones anidadas, es decir, definición de funciones dentro de una funcion. 
                 *  
                 *  sería algo como    public void funcionpadre(){
                 *                          void funcionHija(){
                 *                          }
                 *                     }
                 *            
                 *  se puede que el parametro del padre sea una referencía, y al pasarsela al hijo se pasa la referencía también, entonces
                 *  se debe validar que se pase correctamente. 
                 * 
                 */
                if (expresionesValor.ElementAt(posicionExpresionValor) is Identificador)
                {

                    string nombreRef_ref = ((Identificador)expresionesValor.ElementAt(posicionExpresionValor)).nombre();
                    Simbolo referencia = ent.obtenerSimbolo(nombreRef_ref);
                    
                    // validar que la variable referenciada ya sea una referencia.
                    if(referencia!=null && referencia.porReferencia)
                    {
                        // Previamente se tenía que cuando se pasa una variable por referencia se pasa solo su dirección
                        // pero en este caso, cuando se vuelve a pasar la misma referencía necesitamos acceder al Stack en la posicion que se 
                        // devuelve al obtener el codigo 3D 
                        string temporal = Generador.pedirTemporal();
                        ValorRef.Codigo += $"{temporal} = Stack[(int){ValorRef.Temporal}];\n";
                        ValorRef.Temporal = temporal;
                    }
                }
                

                /* hacemos una declaración, de un nuevo simbolo para obtener el codigo 3D, aca no lo agregamos al entorno porque
                 * las declarcaciones se hacen en la @clase Funcion.getC3(.....);
                 *
                 */
                Simbolo simboloParam = new Simbolo(tipoParametro, nombreParam, false, 0, 0, parametroActual.linea, parametroActual.columna);
                Declaracion nuevaVarRef = new Declaracion(simboloParam, ValorRef);
                nuevaVarRef.TemporalCambioEntorno = temporalCambio;
                codigo += nuevaVarRef.getC3(ent,null);

            }

            return codigo;

        }



        public void buscarReferencia(Funcion llamada) 
        {
            //  comprobar cuantos parametros tiene la llamada
            if (expresionesValor.Count != llamada.ListaParametros.Count) return;

            for (int i = 0; i < llamada.ListaParametros.Count; i++)
            {
                /*  VERIFICAMOS A QUE PARAMETROS DE LA LLAMADA SE MANIPULARAN COMO REFERENCIA,
                *       1. Primero tomamos los parametros de la funcion esqueleto (plantilla, o base) 
                *          ya que esta tiene la definicion de toda la función
                *       2. Combrovar si el valor que lleva la llamada en ese indice es un ide, ya que 
                *          no se puede pasar un primitivo como referencia. 
                */
                if(llamada.ListaParametros.ElementAt(i).porReferencia && expresionesValor.ElementAt(i) is Identificador)
                {
                    ((Identificador)expresionesValor.ElementAt(i)).buscarSoloDireccion = true;

                }
            }
        }


        public void obtenerListasAnidadas(LinkedList<string> variablesUsadas)
        {
            foreach (Expresion item in expresionesValor)
            {
                item.obtenerListasAnidadas(variablesUsadas);
            }
        }

    }



}
