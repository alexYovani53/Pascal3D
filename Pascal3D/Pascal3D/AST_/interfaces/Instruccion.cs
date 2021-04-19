using CompiPascal.entorno_;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.AST_.interfaces
{

    /*
      * @comentario          Esta interfaz es la base para todas las instruccions que no devuelven
      *                      ningun valor, pero si ejecutan sentencias
      *                      
      * Sentencias como: IF, WHILE, FOR, REPEAT,....
      * 
      *
      */
    public interface Instruccion:NodoAST
    {

        /*
        *
        * @param   ent     Un entorno actual de ejecucion el cual gestiona todo lo relacionado con 
        *                  la tabla de simbolos.
        *
        * @return  Devuelve objetos resultado de sintetizar valores implicitos de una Expresion
        *          por ejemplo: En una Instruccion While puede haber una Expresion Return
        *          que retorna un valor.
        */


        public string getC3(Entorno ent,AST arbol);
    }
}
