using CompiPascal.AST_.definicion.arrego;
using CompiPascal.AST_.interfaces;
using CompiPascal.AST_.valoreImplicito;
using CompiPascal.entorno_;
using CompiPascal.entorno_.simbolos;
using Pascal3D;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.definicion
{
    public class Asignacion : Instruccion
    {

        public int linea { get; set; }
        public int columna { get; set; }

        /**
         * @propiedad       bool        esObjeto
         * @comentario      esta bandera dira si la asignación es hacia una variable o a la propiedad 
         *                  de un objeto 
         */
        private bool estaEnObjeto;


        /**
         * @propiedad       string      idObjeto    
         * @comentario      Este almacenara el nombre del objeto al cual se decea acceder (en su momento)
         */
        private string idObjeto;


        /**
         * @propiedad       string      propiedad
         * @comentario      Este almacenara el nombre de una propiedad a la cual se desea acceder
         */
        private LinkedList<string> propiedades;

        /**
         * @propiedad       variable
         * @comentario      Este almacenara el simbolo al cual se le va a asignar el valor
         */
        private Simbolo variable_;

        /**
         * @propiedad       valor
         * @comentario      es la expresión que contiene el valor a asignar a la variable dentro 
         *                  de la tabla de simbolos
         */
        private Expresion valor { get; set; }


        private string enHeap { get; set; }

        /*
         *  Asignación a variables de cualquier tipo pero que no cuente con acceso 
         *  es decir    variable.param1.param2...   esto va en el otro constructor
         */

        public Asignacion(Simbolo variable, Expresion valor, bool objeto, int linea, int columna)
        {
            this.variable = variable;
            this.valor = valor;
            this.estaEnObjeto = objeto;
            this.linea = linea;
            this.columna = columna;

        }

        /*
         * Este constructor es para crear asignaciones hacia parametros de un objeto
         * es decir
         * objeto.param1.param2 := X (valor);   donde idObjeto =  objeto,   idPropiedad =  lista (param1,param2) 
         * 
         */

        public Asignacion(string idObjeto, LinkedList<string> idPropiedad , Expresion valor, int linea, int columna)
        {
            this.idObjeto = idObjeto;
            this.propiedades = idPropiedad;
            this.valor = valor;
            this.estaEnObjeto = true;
            this.linea = linea;
            this.columna = columna;

        }

   




        public string getC3(Entorno ent, AST arbol)
        {


            string codigo = "/*************************************** INICIO ASIGNACION *********/\n\n";
            result3D final = valor.obtener3D(ent);
            verificarTipo_Boolean(final);

            //      Se valida que la asignación en el primer caso sea        OBJETO.param1.param2..paramn := valor;
            //      Se comprueba que haya una secuencia de accesos delante de la asignación
            if (estaEnObjeto)
            {
                // BUSCAMOS EL OBJETO en el entorno. 
                Simbolo objetoEncontrado = ent.obtenerSimbolo(idObjeto);

                if(objetoEncontrado != null)
                {
                    if (objetoEncontrado is Objeto objeto)
                    {
                        string temp1 = Generador.pedirTemporal();
                        string temp2 = Generador.pedirTemporal();

                        codigo += $"{temp1} = SP + {objeto.direccion}; /* Capturamos la direccion de la instancia de objeto */\n";
                        codigo += $"{temp2} = Stack[(int){temp1}]; \n";
                        codigo += cambiarValorRecursivo(objeto, propiedades, 0, final, temp2);
                        //
                        // EJ:  casa.tamano = 10;
                    }
                    else 
                    {
                        Program.getIntefaz().agregarError($"La variable {idObjeto} no es un objeto ", variable.linea, variable.columna);
                        return "";
                    }   
                }
                else
                {
                    Program.getIntefaz().agregarError($"La variable {idObjeto} no se encontro instanciada", variable.linea, variable.columna);
                    return "";
                }

                return codigo;

            }
            // Declaración del tipo      IDE:= X valor ;   ya sea un acceso, un primitivo o un ide
            else
            {

                bool existeSimbolo = ent.existeSimbolo(variable.Identificador);
                if (!existeSimbolo)
                {
                    Program.getIntefaz().agregarError("La variable a asignar \"" + variable.Identificador + "\" no se encuentra en ningun entorno", variable.linea,variable.columna);
                    return "";
                }

                //CAPTURAMOS EL SIMBOLO
                Simbolo simboloVar = ent.obtenerSimbolo(variable.Identificador);

                if (simboloVar.Constante)
                {
                    Program.getIntefaz().agregarError($"La variable {variable.Identificador} es una CONSTANTE, no se puede asignar", variable.linea, variable.columna);
                    return "";
                }


                // VERIFICAMOS LOS TIPOS DE LA VARIABLE A ASIGNAR Y SU VALOR
                if (!igualdadTipos(simboloVar, final.TipoResultado)) return "";

                //   1. ASIGNACIÓN DEL TIPO IDE := IDE ; 
                //   2. ASIGNACIÓN DEL TIPO IDE := accesoArreglo[x]..[x]   donde el arreglo "accesoArreglo" tiene un tipo de datos OBJETO, en este 
                //          caso debemos validar que ambas estructuras sean del mismo tipo


                codigo += final.Codigo;

                //PARA ASIGNAR UNA VARIABLE DEBEMOS OBTENER LA POSICION EN EL STACK DE ESTE
                result3D varAsignar = obtenerPosicionVar(ent, simboloVar.Identificador);
                codigo += varAsignar.Codigo;

                // COMPROBAMOS DE QUE TIPO ES LA VARIBALE QUE ASIGNAREMOS
                if (simboloVar.Tipo == TipoDatos.Object)
                {
                    // SI ENTRO ACÁ, LA VARIABLE ES UN STRUCT Y DEBE SER ASIGNADO POR UN VALOR STRUCT
                    if(valor is AccesoArreglo)
                    {
                        // UN ACCESO A UN ARREGLO PUEDE RETORNAR UN STRUCT SI EL TIPO ES UN STRUCT
                        string tipo1 = ((Objeto)simboloVar).nombreStructura.ToLower();
                        string tipo2 = ((Objeto)final.Referencia).nombreStructura.ToLower();
                        // ACÁ VALIDAMOS QUE EL TIPO DEL OBJETO QUE RETORNA EL ARREGLO SEA IGUAL AL DE LA VARIABLE
                        // 
                        //      casa =  arreglo[x][x];
                        //      (struct) = (struct) 

                        if (tipo1.Equals(tipo2))
                        {
                            if(enHeap == null)  codigo += $"Stack[(int){varAsignar.Temporal}] = {final.Temporal}; \n";
                            else
                            {
                                string etiq1 = Generador.pedirEtiqueta();
                                string etiq2 = Generador.pedirEtiqueta();
                                codigo += $"if( {enHeap} == 1) goto {etiq1};\n";
                                codigo += $"    Stack[(int){varAsignar.Temporal}] = {final.Temporal};\n";
                                codigo += $"    goto {etiq2};\n";
                                codigo += $"{etiq1}:\n";
                                codigo += $"    Heap[(int){varAsignar.Temporal}] = {final.Temporal};\n";
                                codigo += $"{etiq2}:\n";
                            }
                        }
                    }
                    else if(valor is Identificador)
                    {
                        if (enHeap == null) codigo += $"Stack[(int){varAsignar.Temporal}] = {final.Temporal}; \n";
                        else
                        {
                            string etiq1 = Generador.pedirEtiqueta();
                            string etiq2 = Generador.pedirEtiqueta();
                            codigo += $"if( {enHeap} == 1) goto {etiq1};\n";
                            codigo += $"    Stack[(int){varAsignar.Temporal}] = {final.Temporal};\n";
                            codigo += $"    goto {etiq2};\n";
                            codigo += $"{etiq1}:\n";
                            codigo += $"    Heap[(int){varAsignar.Temporal}] = {final.Temporal};\n";
                            codigo += $"{etiq2}:\n";
                        }

                    }
                    else if(valor is Acceso)
                    {
                        // casa =  condominio.casa;
                        if (enHeap == null) codigo += $"Stack[(int){varAsignar.Temporal}] = {final.Temporal}; \n";
                        else
                        {
                            string etiq1 = Generador.pedirEtiqueta();
                            string etiq2 = Generador.pedirEtiqueta();
                            codigo += $"if( {enHeap} == 1) goto {etiq1};\n";
                            codigo += $"    Stack[(int){varAsignar.Temporal}] = {final.Temporal};\n";
                            codigo += $"    goto {etiq2};\n";
                            codigo += $"{etiq1}:\n";
                            codigo += $"    Heap[(int){varAsignar.Temporal}] = {final.Temporal};\n";
                            codigo += $"{etiq2}:\n";
                        }
                    }

                }
                else //ASIGNACIÓN DEL TIPO IDE:= PRIMITIVO
                {

                    if (enHeap == null) codigo += $"Stack[(int){varAsignar.Temporal}] = {final.Temporal}; \n";
                    else
                    {
                        string etiq1 = Generador.pedirEtiqueta();
                        string etiq2 = Generador.pedirEtiqueta();
                        codigo += $"if( {enHeap} == 1) goto {etiq1};\n";
                        codigo += $"    Stack[(int){varAsignar.Temporal}] = {final.Temporal};\n";
                        codigo += $"    goto {etiq2};\n";
                        codigo += $"{etiq1}:\n";
                        codigo += $"    Heap[(int){varAsignar.Temporal}] = {final.Temporal};\n";
                        codigo += $"{etiq2}:\n";
                    }
                }

            }

            codigo += "/*************************************** FIN ASIGNACION *********/\n\n";

            return codigo;
        }

        public result3D obtenerPosicionVar(Entorno ent,string identificador)
        {

            result3D codigo = new result3D();
            string tempora1 = Generador.pedirTemporal();

            codigo.Codigo += $"\n/*BUSCANDO DIRECCION DE UN IDENTIFICADOR   >>>>------ {identificador} <<<<----*/\n";
            codigo.Codigo += $"{tempora1} = SP; \n";

            for (Entorno actual = ent; actual != null; actual = actual.entAnterior())
            {

                foreach (Simbolo item in actual.TablaSimbolos())
                {
                    if (item.Identificador.Equals(identificador))
                    {
                                            
                        codigo.Codigo += $"{tempora1} = {tempora1} + {item.direccion};           /*Capturamos la direccion donde se encuentra el ide, tomado de la tabla de simbolos*/\n" ;

                        /* CUANDO LA ASIGNACIÓN ES A UNA VARIABLE POR REFERENCIA EN UNA FUNCION, LA VARIABLE GUARDA LA REFERENCIA
                         * HACIA EL STACK 
                         */

                        if (item.porReferencia)
                        {
                            string temporal2 = Generador.pedirTemporal();
                            codigo.Codigo += $"{temporal2} = Stack[(int) {tempora1}];             /* Variable por referencia, puntero*/ \n";
                            codigo.Temporal = temporal2;

                            // CUANDO UNA VARIABLE ES POR REFERENCIA, EL SIMBOLO CONTIENE UN TEMPORAL QUE IDENTIFICA SI LA VARIABLE ESTA EN 
                            // EL HEAP temp = 1 o Stack  temp=0;
                            // ESTA ETIQUETA SE GENERA AL GENERAR EL CODIGO DE LOS PARAMETROS DE UNA FUNCION
                            this.enHeap = item.Temp_auxiliar_referencias;
                        }
                        else codigo.Temporal = tempora1;


                        codigo.Codigo += $"/*BUSCANDO DIRECCION DE UN IDENTIFICADOR -> ENCONTRADO*/\n\n";


                        codigo.TipoResultado = item.Tipo;
                        return codigo;
                    }
                }

                if (actual.entAnterior() != null)
                {
                    codigo.Codigo += $"{tempora1} = 0;             /*Retrocedemos entre los entornos*/\n";
                }
            }

            return codigo;
        }

        public void verificarTipo_Boolean(result3D reBooleano)
        {

            // SE VALIDA UNA CONDICION DE LA FORMA
            //  
            //      var = true and false;   -> lo cual genera una condicion if y no retorna un termporal
            //      por lo tanto generamos el codigo para obtener el valor true(1) o false(0)

            if (reBooleano.Temporal.Equals("") && reBooleano.TipoResultado == TipoDatos.Boolean)
            {
                reBooleano.Temporal = Generador.pedirTemporal();

                reBooleano.Codigo += $"{reBooleano.EtiquetaV}: \n";
                reBooleano.Codigo += Generador.tabularLinea($"{reBooleano.Temporal} = 1; \n", 1);
                reBooleano.Codigo += $"{reBooleano.EtiquetaF}: \n";
                reBooleano.Codigo += Generador.tabularLinea($"{reBooleano.Temporal} = 0; \n", 1);
            }

        }



        string cambiarValorRecursivo(Objeto instancia, LinkedList<string> propiedad, int indice, result3D val,string etiquetaDireccion)
        {
            string codigo = "";

            string propiedadBuscada = propiedad.ElementAt(indice);          //nombre de la propiedad buscada en este nivel
            Entorno ENTORNO_OBJETO = instancia.getPropiedades();
            bool existeParametro = ENTORNO_OBJETO.existeEnEntornoActual(propiedadBuscada);

            if (!existeParametro) return "";                                                      //SI EN ALGUN NIVEL NO SE ENCUENTRA EL PARAMETRO SE RETORNA NULL

            //TIPO DE DATOS DEL PARAMETRO BUSCADO
            TipoDatos tipoParametro = ENTORNO_OBJETO.obtenerSimbolo(propiedadBuscada).Tipo;    
       
            //CAPTURAMOS EL SIMBOLO QUE BUSCAMOS
            Simbolo encontrado = ENTORNO_OBJETO.obtenerSimbolo(propiedadBuscada);
            codigo += $"{etiquetaDireccion} = {etiquetaDireccion} + {encontrado.direccion}; \n";

            // ESTA VALIDACION SE HACE PARA VALIDAR QUE LLEGAMOS AL ULTIMO ACCESO QUE QUEREMOS EJ:   casa.habitacion.escritorio
            if (indice == propiedad.Count - 1)
            {
                // VERIFICAMOS LOS TIPOS DE LA VARIABLE A ASIGNAR Y SU VALOR
                if (!igualdadTipos(encontrado, val.TipoResultado)) return "";

                // OPERACIONES DEPENDIENDO DEL TIPO DE LA VARIABLE
                if (tipoParametro == TipoDatos.Object || tipoParametro == TipoDatos.Array)
                {

                    if (val.TipoResultado == TipoDatos.Array)
                    {
                        codigo += val.Codigo;
                        codigo += $"Heap[(int){etiquetaDireccion}] = {val.Temporal};";
                    }
                    else if (val.TipoResultado == TipoDatos.Object)
                    {
                        codigo += val.Codigo;
                        codigo += $"Heap[(int){etiquetaDireccion}] = {val.Temporal};";
                    }

                }
                else
                {

                    codigo += val.Codigo;
                    codigo += $"Heap[(int){etiquetaDireccion}] = {val.Temporal};";
                }

                return codigo;

            }
            else
            {
                // MIENTRAS NO ESTEMOS EN EL FINAL, GENERAMOS CODIGO RECURSIVAMENTE SI Y SOLO SI
                // ESTAMOS EN UN OBJETO
                if (tipoParametro != TipoDatos.Object)
                {

                    Program.getIntefaz().agregarError(" La propiedad " + propiedadBuscada + " no es un objeto", encontrado.linea, encontrado.columna);
                    return null;

                }
                string nuevaDireccion = Generador.pedirTemporal();
                codigo += $"{nuevaDireccion} = Heap[(int){etiquetaDireccion}];";
                codigo += cambiarValorRecursivo((Objeto)encontrado, propiedad, indice + 1, val, nuevaDireccion);

                return codigo ; //MODIFICAR RECURSIVO
            }


        }

        public Simbolo variable
        {
            get
            {
                return variable_;
            }
            set
            {
                variable_ = value;
            }
        }

        public bool igualdadTipos(Simbolo parametro, TipoDatos TipoResultado)
        {
            // VERIFICAMOS LOS TIPOS DE LA VARIABLE A ASIGNAR Y SU VALOR
            // LOS TIMPOS INTEGER Y REAL SE PUEDEN CONVERTIR IMPLICITAMENTE (sin casteo) ENTONCES SE VALIDA ESO 
            if (parametro.Tipo == TipoDatos.Integer)
            {
                if (TipoResultado != TipoDatos.Integer && TipoResultado != TipoDatos.Real)
                {
                    Program.getIntefaz().agregarError("Error de tipos, Asignacion", linea, columna);
                    return false;
                }
            }
            else if (parametro.Tipo == TipoDatos.Real)
            {
                if (TipoResultado != TipoDatos.Integer && TipoResultado != TipoDatos.Real)
                {
                    Program.getIntefaz().agregarError("Error de tipos, Asignacion", linea, columna);
                    return false;
                }
            }
            // SI EL TIPO DEL PARAMETRO QUE SERA ASIGNADO NO ES INTEGER O REAL SE VALIDAN LAS SIGUIENTES CONDICIONES
            else
            {
                // SI SE INTENTA SETEAR UN OBJETO STRUCT A UN OBJETO STRUCT     IDE (struct):= IDE (struct);
                if (parametro.Tipo !=  TipoResultado)
                {
                    Program.getIntefaz().agregarError($"Error de tipos, Asignacion   {TipoDatos.Object} intenta setear un tipo {TipoResultado}", linea, columna);
                    return false;
                }

            }
            return true;
        }



        public void obtenerListasAnidadas(LinkedList<string> nombres)
        {


            if(variable != null)
            {
                if(!nombres.Contains(variable.Identificador.ToLower())) nombres.AddLast(variable.Identificador);
            }
            else
            {
                if(!nombres.Contains(idObjeto.ToLower())) nombres.AddLast(idObjeto);
            }

            valor.obtenerListasAnidadas(nombres);

        }
    
    }
}
