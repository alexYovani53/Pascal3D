using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_.simbolos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.AST_
{
    public class AST
    {
        // @class AST -> Representará el arbol abstracto que se generara luego de que IRONY devuelva el árbol
        // @autor Alex Yovani Jérónimo Tomás 

        /* 
         * @propiedad           instrucciones
         * @propiedad           main
         * @comentario          instrucciones guardara la lista de instrucciones Raiz para el arbol
         *                      es decir. esta la raiz y luego esta lista de instrucciones
         *                      se recibe dos parametros de mismo tipo. ya que puede haber muchas
         *                      definiciones de variables o tipos, pero solo un main. 
         *                      y todos son instrucciones;
         */


        private LinkedList<Instruccion> instrucciones;
        public LinkedList<Struct> Estructuras;

        public AST(LinkedList<Instruccion> instruccion,Instruccion main)
        {
            this.Estructuras = new LinkedList<Struct>();
            this.instrucciones = instruccion;
            this.instrucciones.AddLast(main);
        }

        public void agregarEstructura(Struct Estructura)
        {
            Estructuras.AddLast(Estructura);
        }

        public bool existeEstructura(string identificador)
        {
            identificador = identificador.ToLower();
            foreach (Struct item in Estructuras)
            {
                if (item.identificador.Equals(identificador)) return true;
            }
            return false;
        }

        public Struct retornarEstructura(string identificador)
        {
            identificador = identificador.ToLower();
            foreach (Struct item in Estructuras)
            {
                if (item.identificador.Equals(identificador)) return item;
            }
            return null;
        }

        public LinkedList<Instruccion> obtenerInstrucciones()
        {
            return instrucciones;
        }

    }
}
