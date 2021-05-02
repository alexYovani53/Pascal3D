using CompiPascal.AST_.definicion.arrego;
using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using CompiPascal.entorno_.simbolos;
using Pascal3D;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.definicion
{
    public class DeclararStruct : Instruccion
    {

        public int linea { get; set; }
        public int columna { get; set; }
        
        public bool objetoInterno { get; set; }

        public string TemporalCambioEntorno { get;  set; }

        /**
         * @propiedad       variables   
         * @comentario      Lista de las variables a declarar
         */
        public LinkedList<Simbolo> variables { get; set; }

        /* @propiedad       string      structuraNombre
         * @comentario      esta propiedad guarda el identificador con el que se ha guardado la definicion de un struct
         */
        public string structuraNombre { get; set; }



        public DeclararStruct(LinkedList<Simbolo> variables, string structuraNombre, int linea, int columna)
        {
            this.variables = variables;
            this.structuraNombre = structuraNombre;
            this.linea = linea;
            this.columna = columna;
            this.objetoInterno = false;
        }



        public LinkedList<Simbolo> varTipoStruct()
        {
            return variables;
        }

        public string getC3(Entorno ent, AST arbol)
        {

            string codigoDeclaraStruct = "" ;

            Struct encontrarStructPlantilla = arbol.retornarEstructura(this.structuraNombre);
            Arreglo encontrarArreglo = arbol.retornarArreglo(this.structuraNombre);

            if(encontrarStructPlantilla == null && encontrarArreglo == null)
            {
                Program.getIntefaz().agregarError($"No se encontro la estructura {structuraNombre}.",linea,columna);
                return "";
            }

            if (encontrarStructPlantilla != null) codigoDeclaraStruct += declararStructOBJETO(encontrarStructPlantilla, ent, arbol);
            else if (encontrarArreglo != null) codigoDeclaraStruct += declararArregloOBJETO(encontrarArreglo,ent,arbol);

            return codigoDeclaraStruct;
        }

        public string declararArregloOBJETO(Arreglo plantilla, Entorno ent, AST arbol)
        {
            string codigo = "";

            foreach (Simbolo item in variables)
            {
                DeclaraArray declaracionArreglo = new DeclaraArray(item.Identificador, structuraNombre, plantilla.nombreObjeto_arrTipoObject, plantilla.tipoArreglo, plantilla.niveles, item.linea, item.columna)
                {
                    objetoInterno = this.objetoInterno,
                    temporalCambioEntorno = TemporalCambioEntorno
                };
                codigo += declaracionArreglo.getC3(ent, arbol);
            }

            return codigo;
        }

        public string declararStructOBJETO(Struct estructura, Entorno ent, AST arbol)
        {
            //CAPTURAMOS EL STRUCT
            Struct objetoStruct = estructura;
            string codigo = "";

            foreach (Simbolo item in variables)
            {
                string nombreVar = item.Identificador;

                if (ent.existeSimbolo(nombreVar))
                {
                    Program.getIntefaz().agregarError($"La variable {nombreVar} ya tiene una declaracion",item.linea,item.columna);
                    return "";
                }
                else
                {
                    codigo += $"/***************************************** DECLARAR OBJETO STRUCT -> {item.Identificador}*/\n";
                    string tempDireccionStack = Generador.pedirTemporal();
                    string tempDireccionHeap = Generador.pedirTemporal();

                    //CAPTURAMOS LA DIRECCIÓN DONDE ESTARA EL OBJETO EN EL HEAP
                    codigo += $"{tempDireccionHeap} =  HP; /*Capturamos la direccion del heap*/\n";

                    //SUMAMOS EL TAMAÑO DE LA ESTRUCTURA AL PUNTERO "HP" PARA APARTAR EL TAMAÑO DEL OBJETO
                    codigo += $"HP = HP + {objetoStruct.tamano()};\n";


                    if (!objetoInterno)
                    {
                        //OBTENEMOS LA DIRECCIÓN DONDE EL OBJETO ESTARA EN EL STACK
                        codigo += $"{tempDireccionStack} = SP + {ent.tamano};\n";

                        //ASIGNAMOS EL OBJETO AL STACK EN LA POSICION GUARDADA EN "tempDireccionStack"
                        codigo += $"Stack[(int){tempDireccionStack}] = {tempDireccionHeap};\n";

                    }


                    Entorno nuevoObjeto = new Entorno(null,$"Objeto {this.structuraNombre}");
                    foreach (Instruccion declaracionesInternas in objetoStruct.GetInstruccions())
                    {
                        if(declaracionesInternas is Declaracion)
                        {
                            ((Declaracion)declaracionesInternas).cambiar_Ambito = true;
                            ((Declaracion)declaracionesInternas).declara_EN_Objeto = true;
                            ((Declaracion)declaracionesInternas).TemporalCambioEntorno = tempDireccionHeap;
                        }
                        else if(declaracionesInternas is DeclararStruct)
                        {
                            ((DeclararStruct)declaracionesInternas).objetoInterno = true;
                            ((DeclararStruct)declaracionesInternas).TemporalCambioEntorno = tempDireccionHeap;
                        }
                        else if(declaracionesInternas is DeclaraArray2)
                        {
                            ((DeclaraArray2)declaracionesInternas).objetoInterno = true;
                            ((DeclaraArray2)declaracionesInternas).temporalCambioEntorno = tempDireccionHeap;
                        }
                        codigo += Generador.tabular(declaracionesInternas.getC3(nuevoObjeto,arbol));
                    }

                    int posicionRelativa = ent.tamano;
                    ent.agregarSimbolo(nombreVar, new Objeto(nombreVar, this.structuraNombre, nuevoObjeto, posicionRelativa,item.linea, item.columna));
                    ent.tamano++;

                    codigo += $"/***************************************** FIN DECLARAR OBJETO STRUCT -> {item.Identificador}*/\n";
                }

            }


            return codigo;
        }

        public void obtenerListasAnidadas(LinkedList<string> variablesUsadas)
        {
           
        }

        public int obtenerTamano()
        {
            int numero = 0;

            if (variables != null)
            {
                foreach (Simbolo item in this.variables)
                {
                    numero++;
                }
            }
            return numero;

        }
    }
}
