using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using System;
using System.Collections.Generic;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.valoreImplicito
{
    public class Formateo : Expresion
    {
        
        private Expresion valor;
        private Expresion tamano;
        private Expresion decimales;


        public int linea { get; set; }
        public int columna { get; set; }

        /**
         * @constructor  public Formateo(Expresion valor, Expresion tamano, Expresion decimales)
         *
         * @comentario   Constructor para formateo de texto de tres valores
         * 
         * @param   valor       Expresion que indica el valor del formato
         * @param   tamano      Expresion que indica el tamano de la expresion (espacios) al momento de imprimir
         * @param   decimales   Este es el numero de decimales que se van a mostrar
         */
        public Formateo(Expresion valor, Expresion tamano, Expresion decimales,int linea, int columna)
        {
            this.valor = valor;
            this.tamano = tamano;
            this.decimales = decimales;
            this.linea = linea;
            this.columna = columna;
        }

        /**
         * @constructor  public Formateo(Expresion valor, Expresion tamano)
         *
         * @comentario   Constructor para formateo de texto de 2 valores, sin especificacion de decimales
         * 
         * @param   valor       Expresion que indica el valor del formato
         * @param   tamano      Expresion que indica el tamano de la expresion (espacios) al momento de imprimir
         * @param   decimales   Este es el numero de decimales que se van a mostrar
         */
        public Formateo(Expresion valor, Expresion tamano,int linea, int columna)
        {
            this.valor = valor;
            this.tamano = tamano;
            this.decimales = null;
            this.linea = linea;
            this.columna = columna;
        }

        public TipoDatos getTipo(Entorno entorno, AST ast)
        {

            return TipoDatos.String;
        }

        string NodoAST.getC3()
        {
            throw new NotImplementedException();
        }


        /* @funcion     agregarEspacios
         * @parametro   espacios                numero de espacios a agregar del lado izquierdo de la cadena
         * @parametro   cadena                  cadena que se concatenara con los espacios especificados
         */
        public string agregarEspacios(int espacios, string cadena)
        {
            string cadenaFormateada="";
            
            for (int i = 0; i < espacios; i++)
            {
                cadenaFormateada += " ";
            }
            cadenaFormateada += cadena;
            return cadenaFormateada;

        }

    }
}
