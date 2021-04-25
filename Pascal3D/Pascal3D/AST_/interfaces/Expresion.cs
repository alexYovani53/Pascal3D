using CompiPascal.entorno_;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.interfaces
{

    /**
     * @comentario          Esta interfaz se implementa en todas las sentencias en donde se devuelve
     *                      un valor 
     *                      
     * @Valor               El valor puede ser el resultado de una comparacion (true o false?)
     *                      El resultado de una operacion aritmetica,....
     * 
     * **/


    public interface Expresion:NodoAST
    {

        public string etiquetaFalsa { get; set; }
        public string etiquetaVerdadera { get; set; }

        public result3D obtener3D(Entorno ent);

        public void obtenerListasAnidadas(LinkedList<string> variablesUsadas);
    }
}
