using CompiPascal.AST_.interfaces;
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


        public int tamanoPadre { get; set; }
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



        public int linea { get; set; }
        public int columna { get; set; }

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




        public string getC3(Entorno ent)
        {
            string codigo = "";
            result3D final =   valor.obtener3D(ent);


            if(esobjeto)
            {



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
                if (simboloVar.Tipo == TipoDatos.Integer)
                {
                    if (final.TipoResultado != TipoDatos.Integer && final.TipoResultado != TipoDatos.Real)
                    {
                        Program.getIntefaz().agregarError("Error de tipos, declaracion", linea, columna);
                        return "";
                    }
                }
                else if (simboloVar.Tipo == TipoDatos.Real)
                {
                    if (final.TipoResultado != TipoDatos.Integer && final.TipoResultado != TipoDatos.Real)
                    {
                        Program.getIntefaz().agregarError("Error de tipos, declaracion", linea, columna);
                        return "";
                    }
                }
                else
                {
                    if (simboloVar.Tipo != final.TipoResultado)
                    {
                        Program.getIntefaz().agregarError("Error de tipos, declaracion", linea, columna);
                        return "";
                    }

                }

                //ASIGNACIÓN DEL TIPO IDE := IDE ; 
                if(simboloVar.Tipo == TipoDatos.Object)
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

            return codigo;
        }

        public result3D obtenerPosicionVar(Entorno ent,string identificador)
        {

            result3D regresos = new result3D();
            string tempora1 = Generador.pedirTemporal();

            regresos.Codigo += $" /*BUSCAMOS EL IDE QUE SERA ASIGNADA EN EL ENTORNO ACTUAL O EN LOS ANTERIORES*/\n";
            regresos.Codigo += $"{tempora1} = SP; \n";

            for (Entorno actual = ent; actual != null; actual = actual.entAnterior())
            {

                foreach (Simbolo item in actual.TablaSimbolos())
                {
                    if (item.Identificador.Equals(identificador))
                    {
                        regresos.Codigo += $"{tempora1} = {tempora1} + {item.direccion};           /*CAPTURAMOS LA DIRECCION RELATIVA DEL PARAMETRO*/\n\n" ;
                        regresos.Codigo += "/*ENCONTRAMOS LA POSICION ABSOLUTA EN EL STACK DEL IDE QUE SERA ASIGNADO*/\n";
                        
                        regresos.Temporal = tempora1;
                        regresos.TipoResultado = item.Tipo;
                        return regresos;
                    }
                }

                regresos.Codigo += $"{tempora1} = {tempora1} - {actual.tamano};             /*Retrocedemos entre los entornos*/";
            }

            return regresos;
        }
    }
}
