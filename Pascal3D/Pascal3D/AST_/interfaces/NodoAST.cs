using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.AST_.interfaces
{

    /**
     * @interface   NodoAST
     *
     * @comentario  La interfaz NodoAST define el elemento basico que conformara nuestro Arbol Abstracto
     *              de Analisis Sintactico.
     *
     */


    public interface NodoAST
    {

        public int linea { get; set; }

        public int columna { get; set; }


    }
}
