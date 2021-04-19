﻿using CompiPascal.AST_.interfaces;
using CompiPascal.AST_.valoreImplicito;
using CompiPascal.entorno_;
using CompiPascal.entorno_.simbolos;
using Pascal3D;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.definicion
{
    public class Asignacion : Instruccion
    {

        public int linea { get; set; }
        public int columna { get; set; }

        /**
         * @propiedad       bool        esObjeto
         * @comentario      esta bandera dira si la asignación es hacia una variable o a la propiedad 
         *                  de un objeto 
         */
        private bool esobjeto;


        /**
         * @propiedad       string      idObjeto    
         * @comentario      Este almacenara el nombre del objeto al cual se decea acceder (en su momento)
         */
        private string idObjeto;


        /**
         * @propiedad       string      propiedad
         * @comentario      Este almacenara el nombre de una propiedad a la cual se desea acceder
         */
        private LinkedList<string> propiedades;

        /**
         * @propiedad       variable
         * @comentario      Este almacenara el simbolo al cual se le va a asignar el valor
         */
        private Simbolo variable_;

        /**
         * @propiedad       valor
         * @comentario      es la expresión que contiene el valor a asignar a la variable dentro 
         *                  de la tabla de simbolos
         */
        private Expresion valor { get; set; }


        public Asignacion(Simbolo variable, Expresion valor, bool objeto, int linea, int columna)
        {
            this.variable = variable;
            this.valor = valor;
            this.esobjeto = objeto;
            this.linea = linea;
            this.columna = columna;

        }

        public Asignacion(string idObjeto, LinkedList<string> idPropiedad , Expresion valor, int linea, int columna)
        {
            this.idObjeto = idObjeto;
            this.propiedades = idPropiedad;
            this.valor = valor;
            this.esobjeto = true;
            this.linea = linea;
            this.columna = columna;

        }

   




        public string getC3(Entorno ent, AST arbol)
        {
            string codigo = "/*************************************** INICIO ASIGNACION *********/\n\n";
            result3D final = valor.obtener3D(ent);
            verificarTipo_Boolean(final);


            if (esobjeto)
            {

                Simbolo objetoEncontrado = ent.obtenerSimbolo(idObjeto);

                if(objetoEncontrado == null)
                {
                    Program.getIntefaz().agregarError($"La variable {idObjeto} no se encontro instanciada", variable.linea, variable.columna);
                    return "";
                }
                
                if(!(objetoEncontrado is Objeto))
                {
                    Program.getIntefaz().agregarError($"La variable {idObjeto} no es un objeto ", variable.linea, variable.columna);
                    return "";
                }

                string temp1 = Generador.pedirTemporal();
                string temp2 = Generador.pedirTemporal();

                codigo += $"{temp1} = SP + {((Objeto)objetoEncontrado).direccion}; /*Capturamos la direccion de la instancia de objeto*/  \n";
                codigo += $"{temp2} = Stack[(int){temp1}]; \n";

                codigo += cambiarValorRecursivo((Objeto)objetoEncontrado, propiedades, 0, final, temp2 );

                return codigo;

            }
            else
            {

                bool existeSimbolo = ent.existeSimbolo(variable.Identificador);
                if (!existeSimbolo)
                {
                    Program.getIntefaz().agregarError("La variable a asignar \"" + variable.Identificador + "\" no se encuentra en ningun entorno", variable.linea,variable.columna);
                    return "";
                }

                //CAPTURAMOS EL SIMBOLO
                Simbolo simboloVar = ent.obtenerSimbolo(variable.Identificador);

                if (simboloVar.Constante)
                {
                    Program.getIntefaz().agregarError($"La variable {variable.Identificador} es una CONSTANTE, no se puede asignar", variable.linea, variable.columna);
                    return "";
                }


                // VERIFICAMOS LOS TIPOS DE LA VARIABLE A ASIGNAR Y SU VALOR
                if (!verificarTipos(simboloVar.Tipo, final.TipoResultado)) return "";

                //ASIGNACIÓN DEL TIPO IDE := IDE ; 
                if (simboloVar.Tipo == TipoDatos.Object)
                {

                }
                else //ASIGNACIÓN DEL TIPO IDE:= PRIMITIVO
                {

                    codigo += final.Codigo;
                    result3D varAsignar = obtenerPosicionVar(ent, simboloVar.Identificador);

                    codigo += varAsignar.Codigo;
                    codigo += $"Stack[{varAsignar.Temporal}] = {final.Temporal}; \n";
                }

            }

            codigo += "/*************************************** FIN ASIGNACION *********/\n\n";

            return codigo;
        }

        public result3D obtenerPosicionVar(Entorno ent,string identificador)
        {

            result3D regresos = new result3D();
            string tempora1 = Generador.pedirTemporal();

            regresos.Codigo += $"\n/*BUSCANDO DIRECCION DE UN IDENTIFICADOR*/\n";
            regresos.Codigo += $"{tempora1} = SP; \n";

            for (Entorno actual = ent; actual != null; actual = actual.entAnterior())
            {

                foreach (Simbolo item in actual.TablaSimbolos())
                {
                    if (item.Identificador.Equals(identificador))
                    {
                                            
                        regresos.Codigo += $"{tempora1} = {tempora1} + {item.direccion};           /*Capturamos la direccion donde se encuentra el ide, tomado de la tabla de simbolos*/\n" ;

                        /* CUANDO LA ASIGNACIÓN ES A UNA VARIABLE POR REFERENCIA EN UNA FUNCION, LA VARIABLE GUARDA LA REFERENCIA
                         * HACIA EL STACK 
                         */

                        if (item.porReferencia)
                        {
                            string temporal2 = Generador.pedirTemporal();
                            regresos.Codigo += $"{temporal2} = Stack[(int) {tempora1}];             /* Variable por referencia, puntero*/ \n";
                            regresos.Temporal = temporal2;
                        }
                        else regresos.Temporal = tempora1;


                        regresos.Codigo += $"/*BUSCANDO DIRECCION DE UN IDENTIFICADOR -> ENCONTRADO*/\n\n";


                        regresos.TipoResultado = item.Tipo;
                        return regresos;
                    }
                }

                if (actual.entAnterior() != null)
                {
                    regresos.Codigo += $"{tempora1} = 0;             /*Retrocedemos entre los entornos*/\n";
                }
            }

            return regresos;
        }

        public void verificarTipo_Boolean(result3D reBooleano)
        {

            if (reBooleano.Temporal.Equals("") && reBooleano.TipoResultado == TipoDatos.Boolean)
            {
                reBooleano.Temporal = Generador.pedirTemporal();

                reBooleano.Codigo += $"{reBooleano.EtiquetaV}: \n";
                reBooleano.Codigo += Generador.tabularLinea($"{reBooleano.Temporal} = 1; \n", 1);
                reBooleano.Codigo += $"{reBooleano.EtiquetaF}: \n";
                reBooleano.Codigo += Generador.tabularLinea($"{reBooleano.Temporal} = 0; \n", 1);
            }

        }

        string cambiarValorRecursivo(Objeto instancia, LinkedList<string> propiedad, int indice, result3D val,string etiquetaDireccion)
        {
            string codigo = "";

            string propiedadBuscada = propiedad.ElementAt(indice);          //nombre de la propiedad buscada en este nivel
            Entorno entornoNoModificado = instancia.getPropiedades();
            bool existeParametro = entornoNoModificado.existeEnEntornoActual(propiedadBuscada);

            if (!existeParametro) return "";                                                      //SI EN ALGUN NIVEL NO SE ENCUENTRA EL PARAMETRO SE RETORNA NULL

            //TIPO DE DATOS DEL PARAMETRO BUSCADO
            TipoDatos tipoParametro = entornoNoModificado.obtenerSimbolo(propiedadBuscada).Tipo;    
       
            //CAPTURAMOS EL SIMBOLO QUE BUSCAMOS
            Simbolo encontrado = entornoNoModificado.obtenerSimbolo(propiedadBuscada);
            codigo += $"{etiquetaDireccion} = {etiquetaDireccion} + {encontrado.direccion}; \n";

            if (indice == propiedad.Count - 1)
            {
                // VERIFICAMOS LOS TIPOS DE LA VARIABLE A ASIGNAR Y SU VALOR
                if (!verificarTipos(tipoParametro, val.TipoResultado)) return "";

                //YA QUE EL BUSCADO NO FUE AGREGADO, AHORA CREAMOS UN NUEVO SIMBOLO QUE SUSTITUIRA AL NO AGREGADO
                if (tipoParametro == TipoDatos.Object)
                {

                    if (val.TipoResultado == TipoDatos.Array)
                    {
                        //nuevoModificado.agregarSimbolo(propiedadBuscada, (ObjetoArray)val);
                    }
                    else if (val.TipoResultado == TipoDatos.Object)
                    {
                        //nuevoModificado.agregarSimbolo(propiedadBuscada, (Objeto)val);
                    }

                }
                else
                {

                    codigo += val.Codigo;
                    codigo += $"Heap[(int){etiquetaDireccion}] = {val.Temporal};";
                }

                return codigo;

            }
            else
            {
                if (tipoParametro != TipoDatos.Object)
                {

                    Program.getIntefaz().agregarError(" La propiedad " + propiedadBuscada + " no es un objeto", encontrado.linea, encontrado.columna);
                    return null;

                }
                string nuevaDireccion = Generador.pedirTemporal();
                codigo += $"{nuevaDireccion} = Heap[(int){etiquetaDireccion}];";
                codigo += cambiarValorRecursivo((Objeto)encontrado, propiedad, indice + 1, val, nuevaDireccion);

                return codigo ; //MODIFICAR RECURSIVO
            }


        }


        public Simbolo variable
        {
            get
            {
                return variable_;
            }
            set
            {
                variable_ = value;
            }
        }

        public bool verificarTipos(TipoDatos tipoParametro, TipoDatos TipoResultado)
        {
            // VERIFICAMOS LOS TIPOS DE LA VARIABLE A ASIGNAR Y SU VALOR
            if (tipoParametro == TipoDatos.Integer)
            {
                if (TipoResultado != TipoDatos.Integer && TipoResultado != TipoDatos.Real)
                {
                    Program.getIntefaz().agregarError("Error de tipos, Asignacion", linea, columna);
                    return false;
                }
            }
            else if (tipoParametro == TipoDatos.Real)
            {
                if (TipoResultado != TipoDatos.Integer && TipoResultado != TipoDatos.Real)
                {
                    Program.getIntefaz().agregarError("Error de tipos, Asignacion", linea, columna);
                    return false;
                }
            }
            else
            {
                if (tipoParametro !=  TipoResultado)
                {
                    Program.getIntefaz().agregarError("Error de tipos, Asignacion", linea, columna);
                    return false;
                }

            }
            return true;
        }
    }
}
