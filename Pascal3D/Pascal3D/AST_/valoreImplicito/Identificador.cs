using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using CompiPascal.entorno_.simbolos;
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

        /**
         * @propiedad       ide
         * @comentario      Esta variable almacenara la letra del identificador que aparece en el
         *                  codigo fuente de la entrada.
         */

        private string ide;
        public int linea { get; set; }
        public int columna { get; set; }

        public Identificador(String letra, int linea, int columna)
        {
            this.ide = letra;
            this.linea = linea;
            this.columna = columna;
        }


        public TipoDatos getTipo(Entorno entorno, AST ast)
        {

            object valor = null;

            if (valor == null) return TipoDatos.NULL;

            if (valor is string)
            {
                return TipoDatos.String;
            }
            else if (valor is char)
            {
                return TipoDatos.Char;
            }
            else if (valor is bool)
            {
                return TipoDatos.Boolean;
            }
            else if (valor is int)
            {
                return TipoDatos.Integer;
            }
            else if (valor is double)
            {
                return TipoDatos.Real;
            }
            else if (valor is Objeto)
            {
                return TipoDatos.Object;
            }
            else if( valor is ObjetoArray)
            {
                return TipoDatos.Object;
            }
            else
            {
                try
                {
                    if((TipoDatos)valor == TipoDatos.Struct)
                    {
                        return TipoDatos.Struct;
                    }
                }
                catch (Exception)
                {
                    return TipoDatos.Object;
                }
            }


            return TipoDatos.Object;

        }



        public string nombre()
        {
            return ide;
        }

        public string getC3()
        {
            throw new NotImplementedException();
        }
    }
}
