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

        /**
        * @funcion  object ejecutar(entorno_ ent, AST arbol);
        *
        * @coment   El nivel de abstraccion del Arbol Abstracto de Analisis Sintactico requiere de 
        *          una forma de recorrerlo, ya que todo forma parte de un NodoAST se define el metodo 
        *          ejecutar() en todos aquellos nodos que sean Instrucciones, al hacer llamadas 
        *          al mismo se recorre de una forma abstracta nuestro arbol.
        *
        * @param   ent     Un entorno actual de ejecucion el cual gestiona todo lo relacionado con 
        *                  la tabla de simbolos.
        * @param   arbol   Un Arbol Abstracto de Analisis Sintactico, el cual esta compuesto por NodosAST
        *                  los cuales pueden ser Expresiones o Instrucciones.
        *
        * @return  Devuelve objetos resultado de sintetizar valores implicitos de una Expresion
        *          por ejemplo: En una Instruccion While puede haber una Expresion Return
        *          que retorna un valor.
        */


        public string getC3();
    }
}
