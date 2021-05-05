using CompiPascal.entorno_.simbolos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.entorno_
{

    /**
     * @class          Entorno
     *
     * @comentario     Clase Entorno, se encarga de gestionar todo lo referente a 
     *                 la(s) tabla(s) de simbolos durante la ejecucion del programa.
     */
    public class Entorno
    {

        public string nombre { get; set; }

        public int tamano { get; set; }

        private LinkedList<Entorno> hijos;

        /**
         * @property    tabla      
         * @comentario  Esta es la estructura de datos que guardara los objetos Simbolos
         */
        private ArrayList tabla { get; set; }

        /**
         * @property    ent_Anterior      
         * @comentario  Esta es una instancia para un entorno superior. El actual sería en este caso
         *              un entorno dentro del anterior.
         */
        private Entorno ent_Anterior { get; set; }


        
        /**
         * @constructor  public Entorno(Entorno anterior)
         *
         * @comentario   Constructor de la clase Entorno, se construye un nuevo entorno 
         *               en base a un entorno anterior.
         *
         * La clase Entorno posee una tabla hash que almacenara los simbolos, es decir 
         * la tabla de simbolos actual.
         * 
         * @param   anterior    Sera el entorno "padre".
         */
        public Entorno(Entorno anterior,string nombre)
        {
            this.nombre = nombre;
            this.hijos = new LinkedList<Entorno>();
            this.tabla = new ArrayList();
            this.ent_Anterior = anterior;
            this.tamano = 0;
            if(this.ent_Anterior != null)
            {
                this.ent_Anterior.hijos.AddLast(this);
            }
        }

        internal IEnumerable<Entorno> getHijos()
        {
            return hijos;
        }


        /**
         * @funcion  void agregarSimbolo(string identificador, Simbolo simbolo)
         * 
         * @comentario  Esta función agregara un simbolo a la tabla con el IDENTIFICADOR especificado
         * 
         * @param   identificador    nombre del simbolo
         * @param   simbolo          Este sera una instancia a la clase simbolo que contendra
         *                           el tipo, valor y demas propiedades del simbolo
         */

        public void agregarSimbolo(string identificador, Simbolo simbolo)
        {

            simbolo.Identificador = identificador.ToLower();
            tabla.Add(simbolo);

        }



        /**
         * @funcion  bool existeSimbolo(string identificador)
         * 
         * @comentario  Esta función busca el identificador en la tabla del entorno actual y 
         *              recorre los entornos anteriores en busqueda del simbolo si no aparece en 
         *              uno de ellos.
         * 
         * @param   identificador    nombre del simbolo a buscar
         * @return  devuelve    true -> si se encontro  y false-> si no se encontro
         */

        public bool existeSimbolo(string identificador)
        {
            identificador = identificador.ToLower();

            for(Entorno actual =  this; actual!=null;actual = actual.ent_Anterior)
            {

                foreach (Simbolo item in actual.tabla)
                {
                    if (item.Identificador.Equals(identificador)) return true;
                }

            }
            return false;

        }



        /**
         * @funcion  bool existeSimbolo(string identificador)
         * 
         * @comentario  Esta función busca el identificador en la tabla del entorno actual y 
         *              recorre los entornos anteriores en busqueda del simbolo si no aparece en 
         *              uno de ellos.
         * 
         * @param   identificador    nombre del simbolo a buscar
         * @return  devuelve    true -> si se encontro  y false-> si no se encontro
         */

        public bool existeSimbolo_Menos_EN_GLOBAL(string identificador)
        {
            identificador = identificador.ToLower();

            Entorno pivote = this;
            bool salida = false;

            while(!salida && pivote != null)
            {
                foreach (Simbolo item in pivote.tabla)
                {
                    if (item.Identificador.Equals(identificador)) return true;
                }

                if (pivote.ent_Anterior != null && !pivote.ent_Anterior.nombre.Equals("GLOBAL")) pivote = pivote.ent_Anterior;
                else salida = true;

            }
            return false;
        }



        /*
         *  @funcion   bool existeEnEntornoActual(string identificador)
         *
         *  @comentario     esta función devuelve true o false, si encontro el simbolo en la tabla 
         *                  del entorno actual, respectivamente. 
         *                  
         *  @identificador  es el nombre dle simbolo a buscar
         */

        public bool existeEnEntornoActual(string identificador)
        {
            identificador = identificador.ToLower();

            foreach (Simbolo item in this.tabla)
            {
                if (item.Identificador.Equals(identificador)) return true;
            }

            return false;
        }



        /**
         *  @funcion    Simbolo obtenerSimbolo()
         *  
         *  @comentario esta funcion devuelve el simbolo correspondiente al identificador pasado
         *              como parametro. Buscara desde el entorno actual hacia el entorno mas 
         *              externo. 
         *              
         *  @param  identificador     nombre del simbolo a buscar   
         *  @return Simbolo           simbolo encontrado respecto al identificador
         */

        public Simbolo obtenerSimbolo(string identificador)
        {
            identificador = identificador.ToLower();

            for (Entorno actual = this; actual!=null;actual =  actual.ent_Anterior)
            {

                foreach (Simbolo item in actual.tabla)
                {
                    if (item.Identificador.Equals(identificador))
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        /**
          *  @funcion    Simbolo obtenerFuncion()
          *  
          *  @comentario esta funcion devuelve la funcion correspondiente al identificador pasado
          *              como parametro. Buscara desde el entorno actual hacia el entorno mas 
          *              externo. 
          *              
          *  @param  identificador     nombre de la funcion a buscar   
          *  @return Simbolo           simbolo encontrado respecto al identificador
          */

        public Funcion obtenerFuncion(string identificador)
        {
            identificador = identificador.ToLower();

            for (Entorno actual = this; actual != null; actual = actual.ent_Anterior)
            {

                foreach (Simbolo item in actual.tabla)
                {
                    if (item.Identificador.Equals(identificador) && item is Funcion)
                    {
                        return (Funcion)item;
                    }
                }
            }
            return null;
        }




        public ArrayList TablaSimbolos()
        {
            return tabla;
        }

        public Entorno entAnterior()
        {
            return ent_Anterior;
        }
      

    }
}
