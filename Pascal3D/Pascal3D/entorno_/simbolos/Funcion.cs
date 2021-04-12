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


        private bool verificarTipo(TipoDatos tipo, object result)
        {
            if (tipo == TipoDatos.Integer && result is int)
            {
                return true;
            }
            if (tipo == TipoDatos.String && result is string)
            {
                return true;
            }
            else if (tipo == TipoDatos.Char && result is char)
            {
                return true;
            }
            else if (tipo == TipoDatos.Real && result is double)
            {
                return true;
            }
            else if (tipo == TipoDatos.Boolean && result is bool)
            {
                return true;
            }
            else if (tipo == TipoDatos.Object && result is Objeto)
            {
                return true;
            }
            else if (tipo == TipoDatos.Object && result is ObjetoArray)
            {
                return true;
            }
            else if (tipo==TipoDatos.Void && result is Exit)
            {
                return true;
            }
            else if (tipo == TipoDatos.Void && (TipoDatos)result == TipoDatos.Void)
            {
                return true;
            }
            else
            {
                return false;
            }

        }


        public string getC3(Entorno ent)
        {
            string etiquetaRetorno = Generador.pedirEtiqueta(); 

            Entorno nuevo = new Entorno(ent,"Funcion_"+this.Identificador);
            string codigoFuncion = "";

            codigoFuncion += $"void {this.Identificador} () "+"{";
            agregarParametros(nuevo);
            agregarRetorno(nuevo);

            foreach (Instruccion item in ENCABEZADOS)
            {
                if(item is Declaracion)
                {
                    string declaraVar = item.getC3(nuevo);
                    codigoFuncion += Generador.tabular(declaraVar);
                }
            }



            foreach (Instruccion item in instrucciones)
            {

                string codigo = item.getC3(nuevo);
                codigoFuncion += Generador.tabular(codigo);

            }

            codigoFuncion += RealizarCambioVariable(nuevo);

            codigoFuncion += Generador.tabularLinea($"{etiquetaRetorno}: \n",1);
            codigoFuncion += Generador.tabularLinea("return;\n", 1);
            codigoFuncion += "}";

            tamaFuncion = ent.tamano;


            return codigoFuncion;
        }


        public void agregarParametros(Entorno ent)
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

        public string agregarRetorno(Entorno ent)
        {
            if (Tipo != TipoDatos.Void)
            {
                LinkedList<Simbolo> param = new LinkedList<Simbolo>();
                param.AddLast(new Simbolo(Identificador, linea, columna));
                Declaracion retornoPascal = new Declaracion(param, Tipo);
                string codigo = retornoPascal.getC3(ent);
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



    }
}
