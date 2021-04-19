using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using CompiPascal.entorno_.simbolos;
using System;
using System.Collections.Generic;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.definicion.arrego
{
    public class DeclaraArray : Instruccion
    {
        public int tamanoPadre { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }

        public string ide;

        private string nombreStruct;

        private string nombreStructArreglo;

        private TipoDatos tipo;

        List<int[]> arrayPropDimension;

        public DeclaraArray(string ide, string nombreStructArreglo, string tipoObjeto, TipoDatos tipo, List<int[]> niveles, int linea, int columna)
        {
            arrayPropDimension = new List<int[]>(niveles);      //SI NO SE HACE ESTO LA REFERENCIA SEGUIRA Y DARA PROBLEMAS
            this.ide = ide;
            this.nombreStructArreglo = nombreStructArreglo;
            this.nombreStruct = tipoObjeto;
            this.linea = linea;
            this.columna = columna;
            this.tipo = tipo;
        }



        private object valorDefecto(TipoDatos tipo, string nombreObjeto,Entorno ent, AST arbol)
        {

            if (tipo == TipoDatos.String)
            {
                return "";
            }
            else if (tipo == TipoDatos.Char)
            {
                return '\0';
            }
            else if (tipo == TipoDatos.Boolean)
            {
                return false;
            }
            else if (tipo == TipoDatos.Integer)
            {
                return 0;
            }
            else if (tipo == TipoDatos.Real)
            {
                return 0.0;
            }
            else if (tipo == TipoDatos.Struct)
            {
                return null;
            }
            else if (tipo == TipoDatos.Object)
            {
                return null;
            }

            return null;
        }




        private object[] arregloValores(List<int[]> lista, object valorDefecto)
        {
            List<int[]> copiaLista = new List<int[]>(lista);
            int[] nivel = copiaLista[0];
            copiaLista.RemoveAt(0);                             //Removemos la dimension actual leida

            int inicio = nivel[0];
            int ancho = nivel[1];


            object[] dimen = new object[ancho];

            if (copiaLista.Count > 0)
            {
                //RECURSIVAMENTE AGREGAMOS LAS DIMENSIONES EN CADA POSICION
                for (int i = 0; i < (int)ancho; i++)
                {
                    dimen[i] = arregloValores(copiaLista, valorDefecto);
                }

            }
            else
            {
                //EN CADA POSICION A LO ANCHO DEL ARREGLO
                for (int i = 0; i < (int)ancho; i++)
                {
                    if (valorDefecto is Objeto)
                    {
                        dimen[i] = ((Objeto)valorDefecto).Clone();
                    }
                    else
                    {
                        dimen[i] = valorDefecto;
                    }
                }
            }

            return dimen;

        }


        public string getC3(Entorno ent, AST arbol)
        {
            throw new NotImplementedException();
        }
    }
}
