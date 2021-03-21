using CompiPascal.AST_.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.AST_.valoreImplicito
{


    public class Condicional
    {


        /** @class      Condicional         
         *  @comentario Esta clase almacenara una condicion y una lista de instrucciones 
         *              siendo heredada en las sentencias que usan una condicional 
         */

        /** @propiedad      Expresion       expresion       
         *  @comentario     expresion condicional de la sentencia
         */
        Expresion exprCondicional_;

        /** @propiedad      LinkedList<Instruccion>       instrucciones       
         *  @comentario     Lista de instrucciones de la sentencia que hereda esta clase
         */
        LinkedList<Instruccion> instrucciones_;

        public Condicional(Expresion condicion, LinkedList<Instruccion> instrucciones)
        {
            this.exprCondicional = condicion;
            this.instrucciones = instrucciones;
        }

        public Expresion exprCondicional
        {
            get
            {
                return exprCondicional_;
            }
            set
            {
                exprCondicional_ = value;
            }
        }



        public LinkedList<Instruccion> instrucciones
        {
            get
            {
                return instrucciones_;
            }
            set
            {
                instrucciones_ = value;
            }
        }

        
    }
}
