using Pascal3D.Optimizador.AsignacionOp;
using Pascal3D.Optimizador.ControlOp;
using Pascal3D.Optimizador.EtiquetasOp;
using Pascal3D.Optimizador.Heap_Stack;
using Pascal3D.Optimizador.InterfacesOp;
using Pascal3D.Optimizador.PrimitivosOp;
using Pascal3D.Optimizador.SaltosOp;
using Pascal3D.Optimizador.ValorImplicitoOp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pascal3D.Optimizador.Funcion
{
    public class FuncionOp : InstruccionOp

    {
        public int linea { get; set; }
        public int columna { get; set; }

        private string nombre { get; set; }

        private LinkedList<InstruccionOp> instrucciones { get;set;}

        public FuncionOp(string idFuncion,LinkedList<InstruccionOp> instrucciones, int linea, int columna)
        {
            this.nombre = idFuncion;
            this.instrucciones = instrucciones;
            this.linea = linea;
            this.columna = columna;
        }


        public object ejecutarOptimizacion()
        {
            string codigoEliminado = "";
            string codigoNuevo = "";


            List<InstruccionOp> copia;
            LinkedList<InstruccionOp> nuevasInstrucciones;


            #region regla 2,3,4
            /**********************************************
 *  
 *          REGLA NO 2,3,4. ----- 
 *  
 **********************************************/
            copia = new List<InstruccionOp>(instrucciones);
            nuevasInstrucciones = new LinkedList<InstruccionOp>();
            for (int i = 0; i < instrucciones.Count; i++)
            {
                // RECORREMOS LA LISTA DE INSTRUCCIONES UNA POR UNA
                var pivoteInstruccion = copia[i];

                // BUSCAMOS UN IF, DE NO SER ESTE LA INSTRUCCIÓN PIVOTE, SOLO LA AGREGAMOS A LA LISTA
                if (!(pivoteInstruccion is IfOp)) nuevasInstrucciones.AddLast(pivoteInstruccion);
                else
                {
                    IfOp if_pivote = (IfOp)pivoteInstruccion;
                    int i_mas_1 = i + 1;

                    // SI ESTAMOS EN LA ULTIMA POSICION, AGREGAMOS EL IF DIRECTAMENTE
                    if (i_mas_1 > copia.Count - 1)
                    {
                        nuevasInstrucciones.AddLast(if_pivote);
                        continue;
                    }

                    var pivote_mas_1 = copia[i_mas_1];

                    // TANTO LA REGLA 2,3 Y 4 necesitan UN GOTO  despues del IF
                    if (!(pivote_mas_1 is GotoOp))
                    {
                        // SI LA SIGUIENTE INSTRUCCIÓN NO ES UN GOTO, ENTONCES AGREGAMOS EL IF Y CONTINUAMOS LA BUSQUEDA
                        nuevasInstrucciones.AddLast(if_pivote);
                        continue;
                    }


                    // IF CON OPERANDO NUMERICOS EJ:        if( 1 == 1 )  goto L2;
                    if (if_pivote.OperandosNumericos())
                    {
                        /**********************************************
                         *  
                         *          REGLA NO 3,4. ----- 
                         *  
                         **********************************************/

                        GotoOp goto_pivote = (GotoOp)pivote_mas_1;
                        codigoEliminado += if_pivote.pedirCodigo() + "\n" + goto_pivote.pedirCodigo();


                        string regla;

                        GotoOp gotoOptimo;                  // este goto reemplazara el if ya existente en conjunto con el goto que le sigue
                        if (if_pivote.evaluarCondicion()) // condicion verdadera ej: if ( 1==1 )  esto es verdadero
                        {
                            regla = "REGLA 3";
                            gotoOptimo = new GotoOp(if_pivote.salto.nombre, if_pivote.linea, if_pivote.columna);
                            codigoNuevo += gotoOptimo.pedirCodigo();
                        }
                        else
                        {
                            regla = "REGLA 4";
                            gotoOptimo = new GotoOp(goto_pivote.etiquetaSalto, if_pivote.linea, if_pivote.columna);
                            codigoNuevo += gotoOptimo.pedirCodigo();
                        }

                        RegistroOptimizacion.agregarOptimizacion(regla, codigoEliminado, codigoNuevo, if_pivote.linea, if_pivote.columna, "FUNCION " + nombre);

                        nuevasInstrucciones.AddLast(gotoOptimo);

                        i = i_mas_1;
                    }

                    // SUPRIMIR INSTRUCCIONES Y CAMBIO OPERACION EN IF
                    else
                    {
                        /**********************************************
                         *  
                         *          REGLA NO 2 ----- 
                         *  
                         **********************************************/

                        /*      PATRON
                         *      if ( x [!=,>,<,>=...] y ) goto L1:
                         *      goto L2;
                         *      L1;
                         */

                        int i_mas_2 = i_mas_1 + 1;

                        var pivote_mas_2 = copia[i_mas_2];

                        if (!(pivote_mas_2 is DefLabel)) nuevasInstrucciones.AddLast(if_pivote);
                        else
                        {

                            DefLabel etiquetaDespuesGoto = (DefLabel)pivote_mas_2;

                            // SI LA ETIQUETA NO ES A LA QUE SALTA EL IF CUANDO ES VERDADERA, NO ES UNA OPTIMIZACION
                            if (!etiquetaDespuesGoto.etiqueta.nombre.Equals(if_pivote.salto.nombre)) nuevasInstrucciones.AddLast(if_pivote);
                            else
                            {
                                GotoOp gotoOptimizar = (GotoOp)pivote_mas_1;

                                //ANTES DE OPTIMIZAR DEBEMOS VALIDAR SI NO EXISTE ALGUNA INSTRUCCIÓN QUE SALTE A ESTA ETIQUETA QUE SE ELIMINARA
                                // UN if o un Goto pueden alcanzar la etiqueta, asi que con estas comparamos. 

                                bool etiquetaUsadaPreviamente = false;

                                // EL IF ESTA EN i, DEBEMOS COMPROBAR QUE i no sea 0 YA QUE INTENTARIAMOS ACCEDER A UN INDICE NEGATIVO
                                if (i - 1 == 0) etiquetaUsadaPreviamente = false;
                                else
                                {
                                    for (int y = i - 1; y >= 0; y--)
                                    {

                                        //INSTRUCCION POR INSTRUCCION BUSCAMOS UN IF O UN GOTO. 
                                        var instruccionPrevia = copia[y];

                                        // UN IF ENCONTRADO, ANTES DEL IF QUE ESTAMOS VALIDANDO
                                        if (instruccionPrevia is IfOp)
                                        {
                                            IfOp auxiliarIf = (IfOp)instruccionPrevia;
                                            if (auxiliarIf.salto.nombre.Equals(etiquetaDespuesGoto.etiqueta.nombre))
                                            {
                                                etiquetaUsadaPreviamente = true;
                                                break;
                                            }
                                        }

                                        // UN GOTO ENCONTRADO, ANTES DEL IF QUE ESTAMOS VALIDANDO
                                        else if (instruccionPrevia is GotoOp)
                                        {
                                            GotoOp auxiliarGoto = (GotoOp)instruccionPrevia;
                                            if (auxiliarGoto.etiquetaSalto.Equals(etiquetaDespuesGoto.etiqueta.nombre))
                                            {
                                                etiquetaUsadaPreviamente = true;
                                                break;
                                            }
                                        }


                                    }

                                    if (etiquetaUsadaPreviamente) nuevasInstrucciones.AddLast(if_pivote);
                                    else
                                    {
                                        codigoEliminado += if_pivote.pedirCodigo() + "\n" + gotoOptimizar.pedirCodigo() + "\n" + etiquetaDespuesGoto.pedirCodigo();

                                        //  1. LA REGLA DOS INDICA CAMBIAR EL SIGNO DE COMPARACIÓN, PARA EJECUTAR LA OPERACION INVERSA
                                        //  2. LA REGLA 2 TAMBIÉN INDICA QUE EL IF NUEVO, SALTARA A LA ETIQUETA QUE EL GOTO DESPUES DEL IF QUE EVALUAMOS, USA PARA SALTAR. 
                                        if_pivote.comparacion = if_pivote.operacionInversa();
                                        if_pivote.salto = new Label(gotoOptimizar.etiquetaSalto);

                                        // AHORA CAPTURAMOS EL CODIGO DEL NUEVO IF
                                        codigoNuevo += if_pivote.pedirCodigo();

                                        RegistroOptimizacion.agregarOptimizacion("REGLA 2", codigoEliminado, codigoNuevo, if_pivote.linea, if_pivote.columna, "FUNCION " + nombre);

                                        codigoNuevo = "";
                                        codigoEliminado = "";

                                        nuevasInstrucciones.AddLast(if_pivote);

                                        i = i + 2;

                                    }
                                }

                            }
                        }
                    }



                }

            }

            this.instrucciones = nuevasInstrucciones;
            #endregion

            #region regla 1
            /**********************************************
             *  
             *          REGLA NO 1. ----- 
             *  
             **********************************************/
            copia = new List<InstruccionOp>(instrucciones);
            nuevasInstrucciones = new LinkedList<InstruccionOp>();
            for (int i = 0; i < copia.Count; i++)
            {
                //TOMAMOS INSTRUCCIÓN POR INSTRUCCIÓN EN BUSCA DE UN GOTO

                var pivote = copia[i];

                //MIENTRAS NO SEA UN GOTO SOLO AGREGAMOS LA INSTRUCCIÓN A LA LISTA
                if (!(pivote is GotoOp))
                {
                    nuevasInstrucciones.AddLast(pivote);
                }
                else
                {
                    // SE ENCONTRO UN GOTO
                    GotoOp gotoPivote = (GotoOp)pivote;
                    nuevasInstrucciones.AddLast(gotoPivote);

                    //SI ESTAMOS EN UNA POSICION MAYOR A LA ULTIMA POSICION DEL ARREGLO, NO EJECUTAMOS NADA
                    if (i + 1 > copia.Count - 1) continue;

                    int inicioValidacion = i + 1;
                    var instruccionPivote = copia[inicioValidacion];

                    //SI DESPUES DE UN GOTO BIENE UNA ETIQUETA, NO SE EJECUTA LA REGLA 1 YA QUE NO HAY INSTRUCCIONES ENTRE EL GOTO Y LA ETIQUETA
                    //   GOTO L1:   No es posible optimizar
                    //      LX:
                    // 
                    //   GOTO L1:   Es posible optimizar si LX = L1
                    //        --- INSTRUCCIONES--- 
                    //      LX:
                    if (instruccionPivote is DefLabel)
                    {
                        nuevasInstrucciones.AddLast(instruccionPivote);
                        i++;
                        continue;
                    }

                    //SI LLEGA A ESTE PUNTO, HAY MAS DE UNA INSTRUCCION DESPUES DEL GOTO. 
                    //AHORA BUSACMOS UNA ETIQUETA

                    for (int puntoFinal = inicioValidacion; puntoFinal < copia.Count; puntoFinal++)
                    {
                        var instruccionPivote2 = copia[puntoFinal];

                        if (instruccionPivote2 is DefLabel)
                        {
                            DefLabel labelPivote = (DefLabel)instruccionPivote2;
                            if (labelPivote.etiqueta.nombre.Equals(gotoPivote.etiquetaSalto))
                            {
                                nuevasInstrucciones.AddLast(labelPivote);
                                // SE ENCONTRO COINCIDENCIA ENTRE LA ETIQUETA DEL SALTO Y LA ETIQUETA PIVOTE

                                // CAPTURAMOS EL CODIGO QUE SE OPTIMIZO
                                codigoEliminado += gotoPivote.pedirCodigo();
                                for (int eliminar = inicioValidacion; eliminar < puntoFinal; eliminar++)
                                {
                                    codigoEliminado += copia[eliminar].pedirCodigo() + "\n";
                                    //ELIMINAMOS LAS INSTRUCCIONES
                                    //copia.RemoveAt(i + 1 );   // SE ELIMINA ASI, PARA IR ELIMINANDO LA INSTRUCCION SIGUIENTE AL GOTO, QUE A SU VEZ
                                    //                          // GENERA UN CORRIMIENTO HACIA LA IZQUIERDA CADA QUE SE ELIMINA UNA INSTRUCCION
                                }
                                codigoEliminado += labelPivote.pedirCodigo();

                                codigoNuevo += gotoPivote.pedirCodigo() + "\n" + labelPivote.pedirCodigo() + "\n";

                                RegistroOptimizacion.agregarOptimizacion("REGLA 1", codigoEliminado, codigoNuevo, gotoPivote.linea, gotoPivote.columna, $"FUNCION {nombre}");

                                codigoEliminado = "";
                                codigoNuevo = "";
                                i = puntoFinal;                  // NOS POSICIONAMOS EN EL PUNTO DONDE SE ENCONTRO LA ETIQUETA VALIDA. 
                                break;
                            }
                            else
                            {
                                // SI SE ENCONTRO UNA ETIQUETA PERO NO ES A LA QUE SALTA EL GOTO, LA REGLA 1 NO SE CUMPLE
                                // YA QUE HAY UNA ETIQUETA DE INTERMEDIO ENTRE LA ETIQUETA A LA QUE SALTA LA INSTRUCCIÓN GOTO Y LA INSTRUCCIÓN GOTO COMO TAL 
                                break;
                            }
                        }
                    }


                }

            }

            this.instrucciones = nuevasInstrucciones;
            #endregion


            /**********************************************
             *  
             *          REGLA NO 5. ----- 
             *  
             **********************************************/


            nuevasInstrucciones = new LinkedList<InstruccionOp>();
            copia = new List<InstruccionOp>(this.instrucciones);


            for (int i = 0; i < copia.Count; i++)
            {

                // CAPTURAMOS CADA INSTRUCCION
                var instruccionPivote = copia[i];

                // ANTES DE VALIDAR LA REGLA 5 YA DEBE HABER MAS DE UNA INSTRUCCION en la LISTA
                // para poder retroceder. 

                if (nuevasInstrucciones.Count == 0) nuevasInstrucciones.AddLast(instruccionPivote);
                else
                {
                    
                    // EN ESTE PUNTO DEBEMOS BUSCAR UNA ASIGNACION. 

                    if(instruccionPivote is AsignacionOpti)
                    {
                        bool redundanciaEncontrada = false;
                        AsignacionOpti asignacionPivote = (AsignacionOpti)instruccionPivote;

                        // SI LA ASIGNACION ES DEL TIPO OPERACION O TIPO NUMERICO NO APLICA LA REGLA  5
                        // EJE:  T1 = T2 + 7;   no valido 
                        //
                        //  EJE:  T1 = 8 ;      no valido 
                        //
                        //  EJE:  T1 = T3;      Valido
                        //        <instrucciones>   
                        //        T3 = T1;
                        if( asignacionPivote.valorAsig is Temp_SP_HP) { 


                            Temp_SP_HP temporalPivote1 = (Temp_SP_HP)asignacionPivote.valorAsig;
                            Temp_SP_HP temporalAsignacion1 = (Temp_SP_HP)asignacionPivote.temp;

                            for (int j = i-1; j >=0; j--)
                            {
                                // AHORA RETROCEDEMOS ENTRE LAS INSTRUCCIONES
                                var instruccionPrevia = copia[j];

                                if(instruccionPrevia is AsignacionOpti)
                                {

                                    // AL ENCONTRAR UNA ASIGNACION COMPROBAMOS QUE NO TENGA UNA ASIGNACION NUMERICA U OPERACIÓN
                                    AsignacionOpti asignacionPrevia = (AsignacionOpti)instruccionPrevia;

                                    if (asignacionPrevia.valorAsig is Temp_SP_HP)
                                    {

                                        Temp_SP_HP temporalPivote2 = (Temp_SP_HP)asignacionPrevia.valorAsig;
                                        Temp_SP_HP temporalAsignacion2 = (Temp_SP_HP)asignacionPrevia.temp;


                                        // SE VALIDA LO SIGUIENTE    (----------ejemplo referencia--------)                     
                                        //  EJE:  T1 = T3;          (temporalAsignacion2) =  (temporalPivote2);
                                        //        <instrucciones>   
                                        //        T3 = T1;          (temporalAsignacion1) =  (temporalPivote1);

                                        if (temporalPivote1.temp_sp_hp.Equals(temporalAsignacion2.temp_sp_hp) &&
                                            temporalPivote2.temp_sp_hp.Equals(temporalAsignacion1.temp_sp_hp))
                                        {


                                            // AHORA DEVEMOS COMPROBAR QUE EL VALOR NO HAYA CAMBIADO ENTRE LAS INSTRUCCIONES 
                                            // DE AMBAS ASIGNACIONES 

                                            bool seEncontroCambio = false;
                                            for (int n = j + 1; n < i; n++)
                                            {
                                                var instruccionBusqueda = copia[n];
                                                if(instruccionBusqueda is AsignacionOpti)
                                                {
                                                    // DEL ----------ejemplo referencia-------- 
                                                    // VALIDAMOS QUE T1 NO HAYA CAMBIADO EN ALGUA DE LAS INSTRUCCIONES 
                                                    // ENTRE LA PRIMERA INSTRUCCION Y LA ULTIMA INSTRUCCION PARA VALIDAR LA REGLA 5

                                                    AsignacionOpti auxiliar = (AsignacionOpti)instruccionBusqueda;
                                                    Temp_SP_HP auxiliarVar = (Temp_SP_HP)auxiliar.temp;
                                                    if (auxiliarVar.temp_sp_hp.Equals(temporalAsignacion2.temp_sp_hp))
                                                    {
                                                        seEncontroCambio = true;
                                                        redundanciaEncontrada = false;
                                                        break;
                                                    }
                                                }

                                            }
                                            if (!seEncontroCambio)
                                            {
                                                redundanciaEncontrada = true;
                                            }


                                        }

                                    }
                                }

                            }// FIN FOR


                        }

                        if (!redundanciaEncontrada)
                        {
                            nuevasInstrucciones.AddLast(asignacionPivote);
                        }
                        else
                        {
                            RegistroOptimizacion.agregarOptimizacion("REGLA 5", instruccionPivote.pedirCodigo(), "", instruccionPivote.linea, instruccionPivote.columna, "FUNCION " + nombre);
                        }

                    }
                    else
                    {
                        nuevasInstrucciones.AddLast(instruccionPivote);
                    }
                }



            }//FIN FOR .... REGLA 5


            this.instrucciones = nuevasInstrucciones;


            foreach (InstruccionOp item in instrucciones)
            {
                item.ejecutarOptimizacion();
            }


            return this;
        }

        public string pedirCodigo()
        {
            string codigo = "";

            codigo += "void "+nombre+"(){\n";

            foreach (InstruccionOp item in this.instrucciones)
            {
                codigo +="\t"+item.pedirCodigo()+"\n";
            }

            codigo += "}\n";

            return codigo;
        }
    }
}
