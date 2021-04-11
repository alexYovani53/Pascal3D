using CompiPascal.AST_.definicion;
using CompiPascal.AST_.definicion.arrego;
using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using CompiPascal.entorno_.simbolos;
using Pascal3D;
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
        public int tamanoPadre { get; set; }
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
        private string nombreLlamada { get; set; }

        /* @parametro   LinkedList<Expresion>      expresionesValor       lista de las expresiones a asignar en los parametros de la funcion
         */
        private LinkedList<Expresion> expresionesValor { get; set; }


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
            retorno.Codigo = getC3(ent);

            if (funcionLlamada.Tipo == TipoDatos.Void)
            {
                retorno.TipoResultado = TipoDatos.Void;
            }

            //ACÁ OBTENEMOS EL RESULTADO DE LA FUNCION SI ESTE ES DE ALGUN TIPO
            string temp1 = Generador.pedirTemporal();   //GUARDARA la direccion para entrar al entorno de la funcion
            string temp2 = Generador.pedirTemporal();
            string temp3 = Generador.pedirTemporal();



            retorno.Codigo += $"{temp1} = SP + {ent.tamano};                    /*ENTRAMOS AL ENTORNO DE LA FUNCION*/\n";
            retorno.Codigo += $"{temp2} = {temp1} + {funcionLlamada.tamaFuncion};/*NOS TRASLADAMOS AL ULTIMO PARAMETRO DE LA FUNCION (AQUI ESTA EL RETURN)*/\n";
            retorno.Codigo += $"{temp3} = Stack[(int){temp2}];                  /*CAPTURAMOS EL VALOR DEL RETUR*/\n";

            retorno.Temporal = temp3;
            retorno.TipoResultado = funcionLlamada.Tipo;

            return retorno;
        }



        public string getC3(Entorno ent)
        {
            Funcion existeFuncion = ent.obtenerFuncion(nombreLlamada);
            Entorno entornoFuncion = new Entorno(ent, nombreLlamada);
            entornoFuncion.tamano = 1; //HACEMOS ESTO PARA RECERBAR EL ESPACIO DEL RETORNO AL INICIO DE LAS VARIABLES 

            if (existeFuncion == null)
            {
                Program.getIntefaz().agregarError("La funcion no se encontro", linea, columna);
                return "";
            }

            string codigo = $"/* PREPARACION DE VARIABLES PARA LA FUNCION {existeFuncion.Identificador}*/\n";

            /*  OBTENEMOS EL 3D QUE REPRESENTAN LOS VALORES PASADOS EN LOS PARAMETROS
             *  
             *  PERO ANTES, BUSCAMOS ENTRE LAS EXPRESIONES QUE SERAN PASADAS A LA FUNCIÓN, UN IDENTIFICADOR QUE VAYA COMO REFERENCIA
             *  YA QUE AL ENVIARLA A LA FUNCIÓN NO SE ENVIA SU VALOR SI NO SU DIRECCIÓN
             */
            buscarReferencia(existeFuncion);

            LinkedList<result3D> parametros = new LinkedList<result3D>();
            int i = 0;
            foreach (Expresion item in expresionesValor)
            {
                if(!(item is Identificador) && existeFuncion.ListaParametros.ElementAt(i).porReferencia)
                {
                    Program.getIntefaz().agregarError("La expresión por referencia debe ser un identificador", linea, columna);
                    return "";
                }
                parametros.AddLast(item.obtener3D(ent));
                i++;
            }

            //GENERAMOS UN TEMPORAL PARA QUE LAS DECLARACIONES SE HAGAN DESPUES DEL ENTORNO ACTUAL 
            string temporalEntorno = Generador.pedirTemporal();
            codigo += Generador.tabular($"{temporalEntorno} = SP + {ent.tamano}; \n\n");

            string codigoParams = verificarParametros(parametros,  existeFuncion.ListaParametros, entornoFuncion,temporalEntorno);
            codigo += Generador.tabular(codigoParams);


            codigo += $"SP = SP + {ent.tamano};  /*Estamos sumando el tamaño del entorno actual para pasar al siguiente*/\n";
            codigo += $"{existeFuncion.Identificador}();\n";
            codigo += $"SP = SP -{ent.tamano};  /*Estamos restando el tamño sumando previamente para regresar al entorno actual*/\n";

            return codigo;
        }



        private string  verificarParametros(LinkedList<result3D> valores, LinkedList<Simbolo> parametrosFuncProc, Entorno ent,string temporalEntorno)
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

                if(tipoParametro != tipoExpresion)
                {
                    Program.getIntefaz().agregarError($"Error de tipos {tipoParametro} -> {tipoExpresion} " + numeroRecibe, linea, columna);
                    return "";
                }


                //COMPROBAMOS SI EL VALOR ES POR REFERENCIA O POR VALOR

                if (parametroActual.porReferencia)
                {
                    codigoParams += valoresReferencia(parametroActual, parametroActual.Tipo, nombreParam, valorActual, ent, temporalEntorno);
                }
                else
                {


                    //VALIDAMOS EL TIPO DEL PARAMETRO
                    if(tipoParametro == TipoDatos.Object)
                    {

                    }
                    else
                    {
                        Simbolo simboloParam = new Simbolo(tipoParametro, nombreParam, false,0,0, parametroActual.linea, parametroActual.columna);
                        Declaracion parametro = new Declaracion(simboloParam,valorActual);
                        parametro.TemporalCambioEntorno = temporalEntorno;
                        codigoParams+=parametro.getC3(ent);

                    }



                }

            }


            return codigoParams;
        }



        /* Esta funcion se encarga unicamente de las validaciones correspondientes para poder trabajar con variables por referencia
         * se separa el codigo para no tener un metodo demasiado extenso*/

        private string valoresReferencia(Simbolo parametroActual, TipoDatos tipoParametro, string nombreParam, result3D ValorRef, Entorno ent,string temporalCambio)
        {                    // SI EL PARAMETRO ESPERADO ES POR REFERENCIA, SE DEVE VALIDAR QUE ESTE SEA UN IDENTIFICADOR Y NO UN VALOR PRIMITIVO

            string codigo = "";

                if (tipoParametro == TipoDatos.Object)
                {
                    /*Simbolo structura = ent.obtenerSimbolo(parametroActual.structGenerador);
                    if (structura == null && structura.Tipo != TipoDatos.Struct && structura.Tipo != TipoDatos.Array)
                    {
                        Program.getIntefaz().agregarError("El identificador " + parametroActual.structGenerador + " no tiene una definicion struct asociada", linea, columna);
                        return false;
                    }

                    if (structura.Tipo == TipoDatos.Array)
                    {
                        if (!(valorExpresion is ObjetoArray))
                        {
                            Program.getIntefaz().agregarError("El identificador " + ((Identificador)valorActual).nombre() + " no tiene representa un arreglo", linea, columna);
                            return false;
                        }

                        ObjetoArray valorCopia = (ObjetoArray)valorExpresion;

                        List<int[]> nivels = new List<int[]>(valorCopia.getNiveles());
                        object[] valores = valorCopia.valores;
                        ObjetoArray nuevo = new ObjetoArray(nombreParam, parametroActual.structGenerador, valorCopia.Tipo, valores, nivels, parametroActual.linea, parametroActual.columna)
                        {
                            porReferencia = true,
                            punteroRef = new valorRef(valorCopia, ent.entAnterior(), tipoParametro, parametroActual.linea, parametroActual.columna)
                        };
                        ent.agregarSimbolo(nombreParam, nuevo);
                    }
                    else
                    {
                        Objeto valorACopiar = (Objeto)valorExpresion;
                        Objeto nuevo = new Objeto(nombreParam, parametroActual.structGenerador, valorACopiar.getPropiedades().copiarEntorno(), parametroActual.linea, parametroActual.columna)
                        {
                            porReferencia = true,
                            punteroRef = new valorRef(valorActual, ent.entAnterior(), tipoParametro)
                        };

                        ent.agregarSimbolo(nombreParam, nuevo);
                    }*/


                }
                else
                {

                    Simbolo simboloParam = new Simbolo(tipoParametro, nombreParam, false, 0, 0, parametroActual.linea, parametroActual.columna);
                    Declaracion nuevaVarRef = new Declaracion(simboloParam, ValorRef);
                    nuevaVarRef.TemporalCambioEntorno = temporalCambio;
                    codigo += nuevaVarRef.getC3(ent);

                }

            return codigo;

        }

        public void reservandoRetorno(Entorno ent)
        {
            if (ent.tamano == 0) ent.tamano++;
        }



        public void buscarReferencia(Funcion llamada)
        {

            if (expresionesValor.Count != llamada.ListaParametros.Count) return;

            for (int i = 0; i < llamada.ListaParametros.Count; i++)
            {
                if(llamada.ListaParametros.ElementAt(i).porReferencia && expresionesValor.ElementAt(i) is Identificador)
                {
                    ((Identificador)expresionesValor.ElementAt(i)).buscar_puntero = true;
                }

            }

        }


    }
}
