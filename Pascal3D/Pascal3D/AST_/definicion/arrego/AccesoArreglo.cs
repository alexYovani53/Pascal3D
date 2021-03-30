using CompiPascal.AST_.interfaces;
using CompiPascal.AST_.valoreImplicito;
using CompiPascal.entorno_;
using CompiPascal.entorno_.simbolos;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.definicion.arrego
{
    public class AccesoArreglo : Expresion
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
        public int linea { get; set; }
        public int columna { get; set ; }

        private string nombreAcceso { get; set; }

        private LinkedList<Expresion> indices { get; set; }

        private LinkedList<string> acceso { get; set; }
        private LinkedList<string> acceso2 { get; set; }

        public bool esObjeto { get; set; }

        public AccesoArreglo(string ide,LinkedList<Expresion> indices,int linea, int columna)
        {
            this.nombreAcceso = ide;
            this.indices = indices;
            this.linea = linea;
            this.columna = columna;
            this.esObjeto = false;
            this.acceso2 = null;
        }

        public AccesoArreglo(string ide, LinkedList<Expresion> indices, LinkedList<string> acceso, int linea, int columna)
        {
            this.nombreAcceso = ide;
            this.indices = indices;
            this.linea = linea;
            this.columna = columna;
            this.esObjeto = false;
            this.acceso2 = acceso;
        }

        public AccesoArreglo(string ide, LinkedList<string> acceso, LinkedList<Expresion> indices, int linea, int columna)
        {
            this.nombreAcceso = ide;
            this.indices = indices;
            this.linea = linea;
            this.columna = columna;
            this.acceso = acceso;
            this.esObjeto = true;
            this.acceso2 = null;
        }
        public AccesoArreglo(string ide, LinkedList<string> acceso, LinkedList<string> acceso2, LinkedList<Expresion> indices, int linea, int columna)
        {
            this.nombreAcceso = ide;
            this.indices = indices;
            this.linea = linea;
            this.columna = columna;
            this.acceso = acceso;
            this.acceso2 = acceso2;
            this.esObjeto = true;
        }
        public string getC3()
        {
            throw new NotImplementedException();
        }

        public TipoDatos getTipo(Entorno entorno, AST ast)
        {

            bool existeObjetoArreglo = entorno.existeSimbolo(nombreAcceso);

            if (!existeObjetoArreglo)
            {
                return TipoDatos.NULL;
            }

            Simbolo obtener = entorno.obtenerSimbolo(nombreAcceso);


            return obtener.Tipo;
        }


        public string nombre()
        {
            return nombreAcceso;
        }

        public LinkedList<string> accesoArr()
        {
            return this.acceso;
        }


        public LinkedList<Expresion> indicesVal()
        {
            return this.indices;
        }

        public result3D obtener3D(Entorno ent)
        {
            throw new NotImplementedException();
        }
    }

}
