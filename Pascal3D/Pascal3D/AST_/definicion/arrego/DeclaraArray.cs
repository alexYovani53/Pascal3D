using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using CompiPascal.entorno_.simbolos;
using Pascal3D;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.definicion.arrego
{
    public class DeclaraArray : Instruccion
    {


        Objeto defaultTipo { get; set; }

        public int linea { get; set; }
        public int columna { get; set; }

        public string ide;

        private string nombreStruct;

        private string nombreStructArreglo;

        private TipoDatos tipo;

        List<string[]> arrayPropDimension;

        public bool objetoInterno;
        
        public string temporalCambioEntorno;


        public DeclaraArray(string ide, string nombreStructArreglo, string tipoObjeto, TipoDatos tipo, List<string[]> niveles, int linea, int columna)
        {
            arrayPropDimension = new List<string[]>(niveles);      //SI NO SE HACE ESTO LA REFERENCIA SEGUIRA Y DARA PROBLEMAS
            this.ide = ide;
            this.nombreStructArreglo = nombreStructArreglo;
            this.nombreStruct = tipoObjeto;
            this.linea = linea;
            this.columna = columna;
            this.tipo = tipo;
            this.defaultTipo = null;
        }




        public string getC3(Entorno ent, AST arbol)
        {
            string codigo = $"/*           ----------------------  Declaración de arreglo {ide} <<<<<<<<<<<<<<<<<<<<<<< */\n";

            List<string[]> copiaDimensiones = new List<string[]>(arrayPropDimension);

            //VEMOS SI EXISTE EL IDE DE ESTE ARREGLO
            bool existeId = ent.existeEnEntornoActual(this.ide);
            if (existeId)
            {
                Program.getIntefaz().agregarError("El ide " + this.ide + " ya esta definido con otro valor", linea, columna);
                return "";
            }

            //OBTENEMOS EL ARREGLO DE VALORES

            string direccionArray = Generador.pedirTemporal();
            string direccionHeap = Generador.pedirTemporal();

            // Declaración de la forma    estructura.arreglo[][]    aqui estamos declarando arreglo[][] dentro del entorno en heap de estructura
            if (objetoInterno)
            {

                codigo += $"{direccionArray} = {temporalCambioEntorno};\n";
                codigo += $"{direccionArray} = {direccionArray} + {ent.tamano};\n";
                codigo += $"{direccionHeap} = HP;\n";
                codigo += $"    Heap[(int){direccionArray}] = {direccionHeap};\n\n";

            }
            else
            {
                codigo += $"{direccionArray} = SP;\n";
                codigo += $"{direccionArray} = {direccionArray} + {ent.tamano};\n";
                codigo += $"{direccionHeap} = HP;\n";
                codigo += $"    Stack[(int){direccionArray}] = {direccionHeap};\n\n";

            }

            codigo += Generador.tabular(arregloValores(tipo, nombreStruct, copiaDimensiones, direccionHeap, ent, arbol));

            int posicionRelativa = ent.tamano;

            ObjetoArray instancia = new ObjetoArray(ide, nombreStructArreglo, tipo, posicionRelativa, arrayPropDimension, linea, columna);

            if (defaultTipo != null) instancia.objetoParaAcceso = defaultTipo;

            ent.agregarSimbolo(ide,instancia);
            ent.tamano++;

            codigo += $"/*           ---------------------- FIN Declaración de arreglo {ide} <<<<<<<<<<<<<<<<<<<<<<< */\n";
            return codigo;
        }



        private string arregloValores(TipoDatos tipoDatos, string tipoObjeto, List<string[]> lista,string direccionHeap,Entorno ent, AST arbol)
        {
            string codigo = "";

            List<string[]> copiaLista = new List<string[]>(lista);
            string[] nivel = copiaLista[0];
            copiaLista.RemoveAt(0);                             //Removemos la dimension actual leida

            string inicio = nivel[0]; 
            string ancho = nivel[1];


            if (copiaLista.Count > 0)
            {

                    string siguientePosicion = Generador.pedirTemporal();
                    string posiciones = Generador.pedirTemporal();
                    string contador = Generador.pedirTemporal();
                    string ancho_mas_1 = Generador.pedirTemporal();

                    //RECURSIVAMENTE AGREGAMOS LAS DIMENSIONES EN CADA POSICION
                    // CREANDO UN FOR 
                    string etiquetaInicio = Generador.pedirEtiqueta();
                    string etiquetaFinal = Generador.pedirEtiqueta();



                    codigo += $"{ancho_mas_1} = {ancho} + 1;  /*Ancho del arreglo mas una posicion para almacenar el tamano*/\n";
                    codigo += $"{posiciones} = {direccionHeap}; /*Capturamos el inico del arreglo*/\n";
                    codigo += $"Heap[(int){posiciones}] = {ancho};  /*El la primera posicion colocamos el tamaño del arreglo*/ \n";
                    codigo += $"HP = HP + {ancho_mas_1}; /*Reservamos el espacio para la dimension*/\n";

                    codigo += $"{posiciones} = {posiciones} + 1;  /*Pasamos a la primera posicion donde iran los valores*/\n";
                    codigo += $"{contador} = 1 ; \n";
                    codigo += $"{etiquetaInicio}: \n";

                    codigo += $"{siguientePosicion} = HP;\n";
                    codigo += $"Heap[(int){posiciones}] = {siguientePosicion};\n";
                    codigo += Generador.tabular(arregloValores(tipoDatos, tipoObjeto, copiaLista, siguientePosicion, ent, arbol));


                    codigo += $"if ( {contador} >= {ancho} ) goto {etiquetaFinal};\n";
                    codigo += $"    {posiciones} = {posiciones} + 1; \n";
                    codigo += $"    {contador} = {contador} + 1; \n";

                    codigo += $"        goto {etiquetaInicio};\n";
                    codigo += $"{etiquetaFinal}:\n\n";



            }
            else
            {

   
                // CREANDO UN FOR 
                string etiquetaInicio = Generador.pedirEtiqueta();
                string etiquetaFinal = Generador.pedirEtiqueta();

                string contador = Generador.pedirTemporal();
                string posiciones = Generador.pedirTemporal();
                string ancho_mas_1 = Generador.pedirTemporal();


                codigo += $"{ancho_mas_1} = {ancho} + 1;\n";
                codigo += $"{posiciones} = {direccionHeap}; /*Capturamos el inico del arreglo*/\n";
                codigo += $"Heap[(int){posiciones}] = {ancho};  /*El la primera posicion colocamos el tamaño del arreglo*/\n";
                codigo += $"HP = HP + {ancho_mas_1}; /*Reservamos el espacio para la dimension*/\n";

                codigo += $"{posiciones} = {posiciones} + 1;  /*Pasamos a la primera posicion donde iran los valores*/\n";
                codigo += $"{contador} = 1; \n\n";
                codigo += $"{etiquetaInicio}: \n";

                result3D valpor_defecto = valorDefecto(tipoDatos, tipoObjeto, ent, arbol);

                codigo += Generador.tabular(valpor_defecto.Codigo);

                codigo += $"\n\nHeap[(int){posiciones}] = {valpor_defecto.Temporal};\n";

                codigo += $"if ( {contador} >= {ancho} ) goto {etiquetaFinal};\n";
                codigo += $"    {posiciones} = {posiciones} + 1; \n";
                codigo += $"    {contador} = {contador} + 1; \n";

                codigo += $"        goto {etiquetaInicio};\n";
                codigo += $"{etiquetaFinal}:\n";

            }

            return codigo;

        }


        private result3D valorDefecto(TipoDatos tipo, string nombreObjeto, Entorno ent, AST arbol)
        {
            result3D codigoDef = new result3D();

            if (tipo == TipoDatos.String)
            {
                string temp1 = Generador.pedirTemporal();

                codigoDef.Codigo += $"{temp1}= HP; \n";
                codigoDef.Codigo += $"Heap[(int){temp1}] = 0; \n";
                codigoDef.Codigo += $"HP = HP + 1; \n";

                codigoDef.Temporal = temp1;
                codigoDef.TipoResultado = TipoDatos.String;
                return codigoDef;
            }
            else if (tipo == TipoDatos.Char)
            {
                codigoDef.Temporal = "" + 0;
                return codigoDef;
            }
            else if (tipo == TipoDatos.Integer)
            {
                codigoDef.Temporal = "" + 0;
                return codigoDef;
            }
            else if (tipo == TipoDatos.Real)
            {
                codigoDef.Temporal = "" + 0.0;
                return codigoDef;
            }
            else if (tipo == TipoDatos.Boolean)
            {
                codigoDef.Temporal = "" + 0;
                return codigoDef;
            }
            else if (tipo == TipoDatos.Void)
            {
                return codigoDef;
            }
            else if (tipo == TipoDatos.Object)
            {
                return valorObjeto(nombreObjeto, ent, arbol);
            }

            return codigoDef;

        }

        public result3D valorObjeto(string nombreOjbeto, Entorno ent, AST arbol)
        {

            result3D final = new result3D();

            //SE VALIDA SI EL NOMBRE DE LA ESTRUCTURA QUE GUARDARA EL ARREGLO EXISTE O NO 
            if (nombreOjbeto == null)
            {
                Program.getIntefaz().agregarError("NO SE A DEFINIDO UN TIPO PARA LA ESTRUCTURA A ALMACENAR", linea, columna);
                return null;
            }


            Struct estructura = arbol.retornarEstructura(nombreOjbeto);
            Arreglo arreglo = arbol.retornarArreglo(nombreOjbeto);

            if(arreglo!= null)
            {
                //CAPTURAMOS EL TIPO DEL ARREGLO Y SE LO PASAMOS COMO TIPO GENERAL DEL ARREGLO
                tipo = arreglo.tipoArreglo;

                //AGREGAMOS LOS NIVELES A LOS NIVELES SUPERIORES 
                foreach (string[] item in arreglo.niveles)
                {
                    arrayPropDimension.Add(item);
                }

                string direccionHeap = Generador.pedirTemporal();
                final.Codigo += $"{direccionHeap} = HP; \n";
                final.Codigo += arregloValores(arreglo.tipoArreglo, arreglo.nombreObjeto_arrTipoObject, arreglo.niveles, direccionHeap, ent, arbol);
                final.Temporal = direccionHeap;
                final.TipoResultado = arreglo.tipoArreglo;   // esto esta de mas, pero se hace para llevar el tipo 


            }
            else if(estructura != null)
            {
                tipo = TipoDatos.Object;

                Entorno entornoStruct = new Entorno(ent, "Objeto");
                string tempDireccionHeap = Generador.pedirTemporal();
                LinkedList<Simbolo> LISTA = new LinkedList<Simbolo>();


                // UBICACION DEL OBJETO TIPO STRUCT
                final.Codigo += $"{tempDireccionHeap} = HP; \n";
                LISTA.AddLast(new Simbolo("--", 0, 0));

                // DECLARAMOS EL OBJETO 
                DeclararStruct structEnArray = new DeclararStruct(LISTA, estructura.identificador, linea, columna)
                {
                    objetoInterno = true
                };

                // GENERAMOS EL OBJETO
                final.Codigo += structEnArray.getC3(entornoStruct, arbol);
                final.Temporal = tempDireccionHeap;
                final.TipoResultado = TipoDatos.Object;

                defaultTipo = (Objeto)entornoStruct.obtenerSimbolo("--");


            }

            return final;
        }



        public void obtenerListasAnidadas(LinkedList<string> variablesUsadas)
        {

        }
    }
}
