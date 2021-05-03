using CompiPascal.AST_;
using CompiPascal.AST_.definicion;
using CompiPascal.AST_.definicion.arrego;
using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using CompiPascal.entorno_.simbolos;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace Pascal3D.entorno_.simbolos
{
    public class hacerCopia : Expresion
    {
        public string etiquetaFalsa { get ; set; }
        public string etiquetaVerdadera { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }

        Objeto objetoStructCopia { get; set; }

        ObjetoArray objetoArrayCopia { get; set; }


        string etiquetaHeap { get; set; }

        public hacerCopia(Objeto objeto, ObjetoArray arreglo,string etiquetaHeap)
        {
            this.objetoStructCopia = objeto;
            this.objetoArrayCopia = arreglo;
            this.etiquetaHeap = etiquetaHeap;
        }


        public result3D obtener3D(Entorno ent)
        {

            if (objetoStructCopia!=null)
            {
             
                return copiarObjeto(objetoStructCopia, etiquetaHeap);
            }
            else if(objetoArrayCopia !=null)
            {
                return arreglo(objetoArrayCopia, etiquetaHeap);
            }

            return new result3D();
        }


        /*
         *  string      entornoUbicacion            Esta cadena representa la ubicación del objeto en el heap  
         */
        public result3D copiarObjeto(Objeto instancia,string entornoUbicacion)
        {
            string codigo = "";
            string entornoCopia = Generador.pedirTemporal();
            codigo += $"{entornoCopia} = HP;\n";
            codigo += $"HP = HP + {instancia.getPropiedades().tamano}; \n";

            foreach (Simbolo item in instancia.getPropiedades().TablaSimbolos())
            {

                if(item is Objeto objeto)           // Sintaxis C#, coincidencia de patrones
                {
                    string nuevaUbicacionParam = Generador.pedirTemporal();
                    string direccionAnterior = Generador.pedirTemporal();
                    string valorAnterior = Generador.pedirTemporal();

                    codigo += $"/*Declaración de parametro  {item.Identificador} del objeto tipo {instancia.nombreStructura}*/\n";
                    codigo += $"{nuevaUbicacionParam} = {entornoCopia} + {item.direccion}; /*Esta es la nueva ubicacion del parametro {item.Identificador}*/\n";
                    codigo += $"{direccionAnterior} = {entornoUbicacion} + {item.direccion}; /*Esta es la antigua ubicacion del parametro {item.Identificador}*/\n";
                    codigo += $"{valorAnterior} = Heap[(int){direccionAnterior}]; /*Captura del valor anterior*/\n";

                    result3D subObjeto = copiarObjeto(objeto, valorAnterior);
                    codigo += Generador.tabular(subObjeto.Codigo);
                    codigo += $"Heap[(int){nuevaUbicacionParam}] = {subObjeto.Temporal};  /*Cambio de valor*/\n";
                    codigo += $"/*Fin declaración de parametro  {item.Identificador} del objeto tipo {instancia.nombreStructura}*/\n";
                }
                else if (item is ObjetoArray referencia)
                {
                    string nuevaUbicacionParam = Generador.pedirTemporal();
                    string ubicacionArreglo = Generador.pedirTemporal();
                    string comienzoArreglo = Generador.pedirTemporal();

                    codigo += $"/*Declaración de parametro  {item.Identificador} del objeto tipo {instancia.nombreStructura}*/\n";
                    codigo += $"{nuevaUbicacionParam} = {entornoCopia} + {item.direccion}; /*Esta es la nueva ubicacion del parametro {item.Identificador}*/\n";
                    codigo += $"{ubicacionArreglo} = {entornoUbicacion} + {item.direccion}; /*Esta es la antigua ubicacion del parametro {item.Identificador}*/\n";
                    codigo += $"{comienzoArreglo} = Heap[(int){ubicacionArreglo}]; /*Captura del valor anterior*/\n";

                    result3D resultado = arreglo(referencia, comienzoArreglo);

                    codigo += Generador.tabular( resultado.Codigo);
                    codigo += $"Heap[(int){nuevaUbicacionParam}] = {resultado.Temporal};  /*Cambio de valor*/\n";
                    codigo += $"/*Fin declaración de parametro  {item.Identificador} del objeto tipo {instancia.nombreStructura}*/\n";

                }
                else
                {
                    string nuevaUbicacionParam = Generador.pedirTemporal();
                    string direccionAnterior = Generador.pedirTemporal();
                    string valorAnterior = Generador.pedirTemporal();

                    codigo += $"/*Declaración de parametro  {item.Identificador} del objeto tipo {instancia.nombreStructura}*/\n";
                    codigo += $"{nuevaUbicacionParam} = {entornoCopia} + {item.direccion}; /*Esta es la nueva ubicacion del parametro {item.Identificador}*/\n";
                    codigo += $"{direccionAnterior} = {entornoUbicacion} + {item.direccion}; /*Esta es la antigua ubicacion del parametro {item.Identificador}*/\n";
                    codigo += $"{valorAnterior} = Heap[(int){direccionAnterior}]; /*Captura del valor anterior*/\n";

                    result3D val = copiarValoresFinales(item.Tipo, null, direccionAnterior);

                    codigo += Generador.tabular(val.Codigo);
                    codigo += $"Heap[(int){nuevaUbicacionParam}] = {val.Temporal};  /*Cambio de valor*/\n";
                    codigo += $"/*Fin declaración de parametro  {item.Identificador} del objeto tipo {instancia.nombreStructura}*/\n";
                }

            }

            result3D final = new result3D();
            final.Codigo += codigo;
            final.Temporal = entornoCopia;

            return final;
        }

        public result3D arreglo(ObjetoArray referencia,string entornoArreglo)
        {
            string codigo = "";
            string nuevaCopia = Generador.pedirTemporal();
            
            codigo += $"{nuevaCopia} = HP;    /*Esta es la ubicación de la copia del arreglo*/\n";
            codigo += copiarArreglo(referencia.getNiveles(), nuevaCopia, entornoArreglo, referencia.objetoParaAcceso, referencia.tipoValores);

            result3D final = new result3D();
            final.Codigo += codigo;
            final.TipoResultado = TipoDatos.Array;
            final.Temporal = nuevaCopia;

            return final;
        }

        private string copiarArreglo(List<string[]> lista, string direccionHeap, string direccionAnterior, Objeto valorObjeto, TipoDatos tipoPrimitivo)
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

                string direAnterior = Generador.pedirTemporal();
                string posicionesAnteriores = Generador.pedirTemporal();
                string valorAnterior = Generador.pedirTemporal();

                codigo += $"{direAnterior} = {direccionAnterior}; /*Capturamos la vijea dirección del nivel actual*/\n";
                codigo += $"{posicionesAnteriores} = {direAnterior} + 1; /*Nos pasamos a la primera dirección del arreglo*/\n";

                codigo += $"{ancho_mas_1} = {ancho} + 1;  /*Ancho del arreglo mas una posicion para almacenar el tamano*/\n";
                codigo += $"{posiciones} = {direccionHeap}; /*Capturamos el inico del arreglo*/\n";
                codigo += $"Heap[(int){posiciones}] = {ancho};  /*El la primera posicion colocamos el tamaño del arreglo*/ \n";
                codigo += $"HP = HP + {ancho_mas_1}; /*Reservamos el espacio para todos los datos de esta dimencion incluyendo la posicion para guardar el tamano*/\n";



                codigo += $"{posiciones} = {posiciones} + 1;  /*Pasamos a la primera posicion donde iran los valores*/\n";
                codigo += $"{contador} = 1 ;        /*Contador para asignar los valores de esta dimension*/\n";
                codigo += $"{etiquetaInicio}:       /*Etiqueta de ciclo for para asignar valores*/\n";

                codigo += $"{siguientePosicion} = HP;    /* Esta dimension, tiene valores de tipo arreglo, por lo que se toma una nueva dirección de heap */\n";
                codigo += $"Heap[(int){posiciones}] = {siguientePosicion};   /*Asignamos la nueva dirección de Heap al arreglo en la posicion actual*/\n";


                codigo += $"{valorAnterior} = Heap[(int){posicionesAnteriores}];     /*El arreglo anterior también tiene direccion a un arreglo en esta posicion, la tomamos y pasamos al siguiente nivel*/\n";

                codigo += Generador.tabular(copiarArreglo( copiaLista, siguientePosicion, valorAnterior, valorObjeto, tipoPrimitivo));


                codigo += $"if ( {contador} >= {ancho} ) goto {etiquetaFinal};\n";
                codigo += $"    {posiciones} = {posiciones} + 1; \n";
                codigo += $"    {posicionesAnteriores} = {posicionesAnteriores} + 1; \n";
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


                string direAnterior = Generador.pedirTemporal();
                string posicionesAnteriores = Generador.pedirTemporal();
                string valorAnterior = Generador.pedirTemporal();

                codigo += $"{direAnterior} = {direccionAnterior};\n";
                codigo += $"{posicionesAnteriores} = {direAnterior} + 1; /*Nos pasamos a la primera dirección del arreglo*/\n";


                codigo += $"{ancho_mas_1} = {ancho} + 1;  /*Ancho del arreglo pas posicion del tamaño*/\n";
                codigo += $"{posiciones} = {direccionHeap}; /*Capturamos el inico del arreglo*/\n";
                codigo += $"Heap[(int){posiciones}] = {ancho};  /*El la primera posicion colocamos el tamaño del arreglo*/\n";
                codigo += $"HP = HP + {ancho_mas_1}; /*Reservamos el espacio para la dimension*/\n";

                codigo += $"{posiciones} = {posiciones} + 1;  /*Pasamos a la primera posicion donde iran los valores*/\n";
                codigo += $"{contador} = 1; \n\n";
                codigo += $"{etiquetaInicio}: \n";


                //RECUPERAMOS EL VALOR DEPENDIENDO DEL TIPO QUE SEA
                result3D valpor_defecto = copiarValoresFinales(tipoPrimitivo, valorObjeto, posicionesAnteriores);
                codigo += Generador.tabular(valpor_defecto.Codigo + "\n\n");

                codigo += $"Heap[(int){posiciones}] = {valpor_defecto.Temporal};\n";

                codigo += $"if ( {contador} >= {ancho} ) goto {etiquetaFinal};\n";
                codigo += $"    {posiciones} = {posiciones} + 1; \n";
                codigo += $"    {posicionesAnteriores} = {posicionesAnteriores} + 1; \n";
                codigo += $"    {contador} = {contador} + 1; \n";

                codigo += $"        goto {etiquetaInicio};\n";
                codigo += $"{etiquetaFinal}:\n";

            }

            return codigo;

        }

        public result3D copiarValoresFinales(TipoDatos tipo,Objeto arregloObjeto, string inicio)
        {
            result3D codigo = new result3D();
            switch (tipo)
            {

                case TipoDatos.Integer:
                case TipoDatos.Char:
                case TipoDatos.Real:
                case TipoDatos.Boolean:

                    string anterior = Generador.pedirTemporal();
                    string valor = Generador.pedirTemporal();

                    codigo.Codigo += $"{anterior} = {inicio}; /*Ubicacion del valor*/\n";
                    codigo.Codigo += $"{valor} = Heap[(int){anterior}]; /*Ubicacion del valor*/;\n";

                    codigo.Temporal = valor;
                    codigo.TipoResultado = tipo;

                    break;
                case TipoDatos.String:

                    string direccionString = Generador.pedirTemporal();                   
                    string EtiquetaCiclo = Generador.pedirEtiqueta();
                    string EtiquetaSalida = Generador.pedirEtiqueta();
                    string CARACTER = Generador.pedirTemporal();

                    codigo.Temporal = Generador.pedirTemporal();

                    codigo.Codigo += $"{direccionString} = {inicio}; /*Ubicacion del valor*/;\n";
                    codigo.Codigo += $"{codigo.Temporal} = HP; /*Capturamos el inicio de la cadena nueva*/\n";
                    codigo.Codigo += $"{EtiquetaCiclo}: /*** Etiqueta para ciclado de lectura ***/ \n\n";
                    codigo.Codigo += $"    {CARACTER} = Heap[(int){direccionString}];   /*Capturamos el caracter a copiar*/\n\n";

                    codigo.Codigo += $"    if({CARACTER}==0) goto {EtiquetaSalida}; /*Comparamos si ya se a llegado al final de la cadena */\n\n";
                    codigo.Codigo += $"        Heap[HP] = {CARACTER}; /* Copiamos el caracter en la ultima posicion del HEAP, donde vamos*/\n";

                    codigo.Codigo += $"            {direccionString} = {direccionString}+1 ;  /*Aumentamos el contador para seguir leyendo los caracteres*/\n";
                    codigo.Codigo += $"            HP = HP + 1;\n";

                    codigo.Codigo += $"            goto {EtiquetaCiclo}; /*Regresamos al inicio del ciclo para seguir leyendo*/\n";
                    codigo.Codigo += $"{EtiquetaSalida}: \n\n";


                    codigo.Codigo += $"Heap[HP] = 0; \n";
                    codigo.Codigo += $"HP = HP + 1; \n";
                    break;

                case TipoDatos.Object:
                    string direccion = Generador.pedirTemporal();
                    codigo.Codigo += $"{direccion} = Heap[(int){inicio}];   /*Entrar al entorno del objeto*/\n";

                    result3D result = copiarObjeto(arregloObjeto, inicio);

                    codigo.Codigo += result.Codigo;
                    codigo.Temporal = result.Temporal;

                    break;

                case TipoDatos.NULL:
                    break;
                default:
                    break;
            }

            return codigo;
        }



        public void obtenerListasAnidadas(LinkedList<string> variablesUsadas)
        {

        }
    }
}
