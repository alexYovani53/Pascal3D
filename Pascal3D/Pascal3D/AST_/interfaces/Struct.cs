using CompiPascal.AST_.definicion;
using CompiPascal.AST_.definicion.arrego;
using CompiPascal.entorno_;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.AST_.interfaces
{


    /* @brief   Clase Struct, base para trabajar con clases.        la clase solo almacena la DEFINICION. NO LA GUARDA EN LA TABLA DE SIMBOLOS
    *
    * Un Struct es un conjunto de atributos asociados a un identificador.    *
    * Diferencias entre una clase y un Struct:
    * 
    * 1. Un Struct genera automáticamente un inicializador, mientras que en las clases no lo hacen.
    * 2. Los Struct son "por valor" y las clases son “por referencia”.
    * 3. Un Struct no puede manejar herencia, las clases si.
    * 4. Una clase tiene acciones (metodos o funciones) asociadas, un Struct no.
    * 
    * Si se quiere trabajar con clases. se debe agregar propiedades de metodos o funciones, una visibilidad y otros.
    * 
    */
    public class Struct
    {


        public int linea { get; set; }
        public int columna { get; set; }


        /* @propiedad    String      identificador
         * @comentario   esta propiedad guardara el identificador con el que se denominara el struct
         */

        public string identificador { get; set; }


        /* @propiedad    String      identificador
         * @comentario   esta propiedad guardara el identificador con el que se denominara el struct
         */

        private LinkedList<Instruccion> declaraciones { get; set; }



        /* @constructor     Este es el principal constructor para la declaracion de objetos en pascal
         */
        public Struct(string identificador, LinkedList<Instruccion> declaraciones)
        {
            this.identificador = identificador;
            this.declaraciones = declaraciones;
        }


        public LinkedList<Instruccion> GetInstruccions()
        {
            return declaraciones;
        }

        public int tamano()
        {
            int declaracionesNumero = 0;
            foreach (Instruccion item in declaraciones)
            {
                if(item is Declaracion || item is DeclararStruct || item is DeclaraArray  || item is DeclaraArray2)
                {
                    declaracionesNumero++;
                }

            }

            return declaracionesNumero;
        }

        public string getC3()
        {
            return "";
        }
    }
}
