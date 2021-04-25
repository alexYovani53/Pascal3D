using CompiPascal.AST_;
using CompiPascal.AST_.cambioFlujo;
using CompiPascal.AST_.definicion;
using CompiPascal.AST_.definicion.arrego;
using CompiPascal.AST_.interfaces;
using CompiPascal.AST_.valoreImplicito;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.entorno_.simbolos
{
    public class Funcion : Simbolo, Instruccion
    {


        public int tamanoPadre { get; set; }


        public int tamaFuncion { get; set; }

        /*@propiedad    string      nombreStruct
        *@comentario                guardara el nombre del struct que debe retornar*/
        public string nombreStruct { get; set; }


        /*@propiedad        LinkedList<Instruccion>     instrucciones
         *@comentario           guardara la lista de instrucciones de la funcion o método*/
        public LinkedList<Instruccion> instrucciones { get; set; }



        /*ESTO ES PARA EL LENGUAJE PASCAL, YA QUE SOLO PERMITE DECLARACIONES EN EL ENCABEZADO DE LA FUNCION*/
        /*@propiedad        LinkedList<Instruccion>     ENCABEZADOS
        *@comentario           guardara la lista de declaraciones de la funcion*/
        LinkedList<Instruccion> ENCABEZADOS { get; set; }






        /**
         * @comentario   Constructor de funcion de tipo PRIMITIVO.

         * @param   tipo            tipo primitivo de retorno.
         * @param   nombre          Identificador de a funcion.
         * @param   parametros      La lista de parametros que definen la funcion.
         * @param   instrucciones   Lista de las instrucciones que se encuentran dentro del método.
         * @param   encabezado      Lista de instrucciones de declaracion
         */

        public Funcion(TipoDatos tipo ,string nombre, LinkedList<Simbolo> parametros,LinkedList<Instruccion> instrucciones,
                        LinkedList<Instruccion> encabezados,int linea, int columna) : base(tipo,nombre,parametros,linea, columna)
        {
            this.instrucciones = instrucciones;
            this.ENCABEZADOS = encabezados;
            this.nombreStruct = string.Empty;
        }

        /**
         * @comentario   Constructor de funcion de tipo STRUCT

         * @param   structTipo      El identificador del Struct que generara el objeto a retornar.
         * @param   nombre          Nombre de la funcion o procedimiento.
         * @param   parametros      La lista de parametros que definen la funcion.
         * @param   instrucciones   Lista de las instrucciones que se encuentran dentro del método.
         * @param   encabezado      Lista de instrucciones de declaracion
         */

        public Funcion(string structTipo, string nombre, LinkedList<Simbolo> parametros, LinkedList<Instruccion> instrucciones,
                        LinkedList<Instruccion> encabezados, int linea, int columna) : base(TipoDatos.Object, nombre, parametros,linea,columna)
        {
            this.instrucciones = instrucciones;
            this.ENCABEZADOS = encabezados;
            this.nombreStruct = structTipo;
        }

        public string getC3(Entorno ent, AST arbol)
        {
            string etiquetaRetorno = Generador.pedirEtiqueta(); 

            Entorno nuevo = new Entorno(ent,"Funcion_"+this.Identificador);

            string codigoAnidadas = "";
            string codigoFuncion = "";

            codigoFuncion += $"void {this.Identificador} () "+"{\n\n";
            agregarParametros(nuevo,arbol);
            agregarRetorno(nuevo,arbol);

            foreach (Instruccion item in ENCABEZADOS)
            {
                if(item is Declaracion)
                {
                    string declaraVar = item.getC3(nuevo,arbol);
                    codigoFuncion += Generador.tabular(declaraVar);
                }
                else if(item is Funcion)
                {
                    anidada((Funcion)item,ent);
                    ent.agregarSimbolo(((Funcion)item).Identificador,(Funcion)item);
                    codigoAnidadas += ((Funcion)item).getC3(ent, arbol) + codigoAnidadas;
                }
            }




            foreach (Instruccion item in instrucciones)
            {

                string codigo = item.getC3(nuevo,arbol);
                codigoFuncion += Generador.tabular(codigo);

            }

            codigoFuncion += RealizarCambioVariable(nuevo);

            codigoFuncion += Generador.tabularLinea($"{etiquetaRetorno}: \n",1);
            codigoFuncion += Generador.tabularLinea("return;\n", 1);
            codigoFuncion += "}\n\n";

            tamaFuncion = ent.tamano;

            codigoFuncion = codigoFuncion.Replace("#EXIT#", $"goto {etiquetaRetorno};");

            return codigoAnidadas + codigoFuncion;
        }

        public void agregarParametros(Entorno ent,AST arbol)
        {
            /*PARA EL MANEJO DE LAS FUNCIONES QUE TIENEN UN RETORNO
              SE MANEJA QUE EL PRIMER ESPACIO EN EL ENTORNO DE LA FUNCION SEA EL QUE GUARDARA EL VALOR A RETORNAR
              POR LO QUE LA PRIMERA DECLARACION EN LA FUNCION COMIENZA EN  
              P = P + 1      Y NO EN         P = P + 0  
             */
            int posRelativa = 1;
            ent.tamano++;


            if (ListaParametros != null)
            {
                foreach (Simbolo item in ListaParametros)
                {
                    Simbolo parametro = new Simbolo(item.Tipo, item.Identificador, false, 1, posRelativa, item.linea, item.columna);

                    /* HACEMOS ESTA VALIDACIÓN PARA CUANDO VIENE UN PARAMETRO POR REFERENCIA, EN EL ENTORNO TAMBIEN APARESCA UNA BANDERA QUE LO INDIQUE
                     * ESTA BANDERA SERA USADA EN LA ASIGNACIÓN O AL MOMENTO DE ACCEDER A LA VARIABLE */
                    if (item.porReferencia) parametro.porReferencia = true;

                    ent.agregarSimbolo(item.Identificador, parametro);
                    ent.tamano++;
                    posRelativa++;
                }
            }

        }

        public string agregarRetorno(Entorno ent,AST ARBOL)
        {
            if (Tipo != TipoDatos.Void)
            {
                LinkedList<Simbolo> param = new LinkedList<Simbolo>();
                param.AddLast(new Simbolo(Identificador, linea, columna));
                Declaracion retornoPascal = new Declaracion(param, Tipo);
                string codigo = retornoPascal.getC3(ent, ARBOL);
                return codigo;            
            }
            return "";
        }

        public string RealizarCambioVariable(Entorno ent)
        {
            string codigo = "/*************************************************** CONFIGURANDO RETORNO*/\n";
            string temporal = Generador.pedirTemporal();
            if (Tipo != TipoDatos.Void)
            {

                Identificador ide_var_funcion = new Identificador(Identificador, linea, columna);
                result3D codigo_cambio_valor = ide_var_funcion.obtener3D(ent);

                codigo += codigo_cambio_valor.Codigo;
                codigo += $"{temporal} = SP + 0;                    /* lo que hacemos es cambiar el valor en la variable que se llama igual que la funcion, a la primera posicion del entorno actual*/ \n";
                codigo += $"Stack[(int){temporal}] = {codigo_cambio_valor.Temporal}; \n";

            }
            else
            {
                codigo += $"{temporal} = SP + 0; /*Retorno void, colocamos -1 en la variable de retorno en el entorno*/ \n\n";
                codigo += $"Stack[(int){temporal}] = 0 - 1 ; \n";
            }


            codigo += "/*************************************************** CONFIGURANDO RETORNO*/\n";

            return Generador.tabular(codigo);
        }

        public void anidada(Funcion funcion, Entorno ent)
        {
            LinkedList<string> vars = new LinkedList<string>();
            buscarAnidadas(vars);

            LinkedList<TipoDatos> tipos = new LinkedList<TipoDatos>();
            foreach (string item in vars)
            {
                tipos.AddLast(obtenerTipoAnidada(item, ent));
            }

            LinkedList<bool> referencias = new LinkedList<bool>();
            foreach (string item in vars)
            {
                referencias.AddLast(porReferenciaAnidada(item, ent));
            }


            IList<string> vars2 = new List<string>(vars);
            IList<TipoDatos> tipo2 = new List<TipoDatos>(tipos);
            IList<bool> referencias2 = new List<bool>(referencias);

            if (vars2.Count != tipo2.Count || vars2.Count != referencias2.Count || tipo2.Count != referencias2.Count) return;

            for (int i = 0; i < vars.Count; i++)
            {
                funcion.ListaParametros.AddLast(new Simbolo(tipo2[i], vars2[i], referencias2[i], 0, 0));         
            }

            foreach (string item in vars2)
            {
                cambiarLLamada(funcion.Identificador, item);
            }

        }

        public void buscarAnidadas(LinkedList<string> vars)
        {

            foreach (Instruccion item in ENCABEZADOS)
            {
                if(item is Funcion)
                {
                    ((Funcion)item).buscarAnidadas(vars);
                    ((Funcion)item).obtenerListasAnidadas(vars);
                }
            }
                        
        }

        public void obtenerListasAnidadas(LinkedList<string> variablesUsadas)
        {
            /**
             * En este lenguaje no se validan los encabezados porque pascal da error, que 
             * fue comprobado para sustentar esto 
             */

            foreach (Instruccion item in instrucciones)
            {
                item.obtenerListasAnidadas(variablesUsadas);
            }

            foreach (Instruccion item in ENCABEZADOS)
            {
                if(item is Declaracion)
                {
                    LinkedList<string> declaras = new LinkedList<string>();
                    ((Declaracion)item).obtenerIdes(declaras);

                    foreach (string pivote in declaras)
                    {
                        if (variablesUsadas.Contains(pivote))
                        {
                            variablesUsadas.Remove(pivote.ToLower());
                        }
                    }
                }
            }

            foreach (Simbolo item in ListaParametros)
            {
                if (variablesUsadas.Contains(item.Identificador.ToLower()))
                {
                    variablesUsadas.Remove(item.Identificador.ToLower());
                }
            }

        }
    
        public TipoDatos obtenerTipoAnidada(string ide, Entorno ent)
        {
            foreach (Simbolo item in this.ListaParametros)
            {
                if (item.Identificador.Equals(ide)){
                    return item.Tipo;
                }
            }

            foreach (Instruccion item in ENCABEZADOS)
            {
                if(item is Declaracion)
                {
                    LinkedList<string> vars = new LinkedList<string>();
                    if (vars.Contains(ide.ToLower())) return ((Declaracion)item).tipoVars();
                }
            }

            Simbolo buscando = ent.obtenerSimbolo(ide);
            if(buscando!=null)return buscando.Tipo;

            return TipoDatos.NULL;

        }
        
        public bool porReferenciaAnidada(string ide, Entorno ent)
        {
            foreach (Simbolo item in this.ListaParametros)
            {
                if (item.Identificador.Equals(ide))
                {
                    return item.porReferencia;
                }
            }

            return false;
        }

        public void cambiarLLamada(string llamada,string parametro)
        {

            foreach (Instruccion item in instrucciones)
            {
                if(item is Llamada)
                {
                    if (((Llamada)item).nombreLlamada.Equals(llamada))
                    {
                        ((Llamada)item).expresionesValor.AddLast(new Identificador(parametro,0,0));
                    }
                }
            }

        }

    }
}
