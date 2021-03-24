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

            Simbolo encontrar = entorno.obtenerSimbolo(this.ide);

            if (encontrar == null) return TipoDatos.NULL;

            return encontrar.Tipo;

        }



        public string nombre()
        {
            return ide;
        }

        public result3D obtener3D(Entorno ent)
        {
            //OBTENEMOS EL SIMBOLO DEL ENTORNO ACTUAL
            Simbolo encontrado = ent.obtenerSimbolo(this.ide);

            //SI EL SIMBOLO ES NULL ES PORQUE NO SE ENCONTRO 
            if(encontrado == null)
            {
                Program.getIntefaz().agregarError("No se encontro el identificador " + this.ide, linea, columna);
                return null;
            }

            //RETORNAMOS UNA CADENA 3D PARA EL IDENTIFICADOR 
            result3D nuevo = new result3D
            {
                Temporal = this.ide,
                Codigo = "",
                TipoResultado = encontrado.Tipo
            };

            return nuevo;
        }
    }
}
