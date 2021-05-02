using CompiPascal.AST_;
using CompiPascal.AST_.definicion;
using CompiPascal.AST_.definicion.arrego;
using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using CompiPascal.entorno_.simbolos;
using System;
using System.Collections.Generic;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace Pascal3D.entorno_.simbolos
{
    public class PlantillasObjetos:Instruccion
    {
        public int linea { get; set; }
        public int columna { get; set; }

        string ideEstructura { get; set; }

        string ideSimboloRetorno { get; set; }

        bool porReferencia { get; set; }        


        public PlantillasObjetos(string ideSimboloRetorno,string nombreEstructura, int linea, int columna)
        {
            this.ideEstructura = nombreEstructura;
            this.linea = linea;
            this.columna = columna;
            this.ideSimboloRetorno = ideSimboloRetorno;
        }

        public string getC3(Entorno ent, AST arbol)
        {
            Struct generadorStruct = arbol.retornarEstructura(ideEstructura);
            Arreglo generadorArreglo = arbol.retornarArreglo(ideEstructura);
            string codigo = "";

            // SE VERIFICA SI ES UN ESTRUCT
            if (generadorStruct != null)
            {
                Entorno nuevo = new Entorno(ent, "Objeto");
                foreach (Instruccion declaracionesEnStruct in generadorStruct.GetInstruccions())
                {
                    if (declaracionesEnStruct is Declaracion)
                    {
                        // HACEMOS UNA DELCARACIÓN  SIMPLE DE VARIABLES DE TIPOS PRIMITIVOS
                        if (((Declaracion)declaracionesEnStruct).variables != null)
                        {
                            foreach (Simbolo varAux in ((Declaracion)declaracionesEnStruct).variables)
                            {
                                nuevo.agregarSimbolo(varAux.Identificador, new Simbolo(varAux.Tipo, varAux.Identificador, varAux.Constante, 1, nuevo.tamano, linea, columna));
                                nuevo.tamano++;
                            }
                        }
                        else if (((Declaracion)declaracionesEnStruct).ideUnico != null)
                        {
                            Simbolo varAux = ((Declaracion)declaracionesEnStruct).ideUnico;
                            nuevo.agregarSimbolo(varAux.Identificador, new Simbolo(varAux.Tipo, varAux.Identificador, varAux.Constante, 1, nuevo.tamano, linea, columna));
                            nuevo.tamano++;
                        }
                    }
                    else if (declaracionesEnStruct is DeclararStruct)
                    {
                        LinkedList<Simbolo> subStructs = ((DeclararStruct)declaracionesEnStruct).variables;
                        string nombrePlantilla = ((DeclararStruct)declaracionesEnStruct).structuraNombre;

                        foreach (Simbolo auxVar in subStructs)
                        {
                            PlantillasObjetos siguienteEstructura = new PlantillasObjetos(auxVar.Identificador, nombrePlantilla, declaracionesEnStruct.linea, declaracionesEnStruct.columna);
                            codigo += siguienteEstructura.getC3(nuevo, arbol);
                        }
                    }
                    else if (declaracionesEnStruct is DeclaraArray2)
                    {
                        codigo += ((DeclaraArray2)declaracionesEnStruct).getC3(ent, arbol);
                    }

                }
                Objeto resultado = new Objeto(ideSimboloRetorno, generadorStruct.identificador, nuevo, ent.tamano, linea, columna);
                ent.tamano++;
                ent.agregarSimbolo(ideSimboloRetorno, resultado);

            }
            else if (generadorArreglo != null)
            {

                // LA ESTRUCTURA ES UN ARREGLO
                TipoDatos tipoFinal = obtenerTipoArregloRecursivo(generadorArreglo, arbol);
                List<string[]> niveles = obtenerNivelesRecursivos(generadorArreglo, arbol);
                ObjetoArray nuevoArreglo = new ObjetoArray(ideSimboloRetorno, ideEstructura, tipoFinal, ent.tamano, niveles, linea, columna);
                ent.tamano++;
                ent.agregarSimbolo(ideSimboloRetorno, nuevoArreglo);
            
            }

            return codigo;
        }

        

        public TipoDatos obtenerTipoArregloRecursivo(Arreglo arreglo,AST arbol)
        {
            // Validamos si el arreglo es de tipo arreglo, recursivamente, 
            // buscamos llegar al ultimo tipo ya sea primitivo o un struct
            if (arreglo.nombreObjeto_arrTipoObject != null)
            {
                if(arbol.existeArreglo(arreglo.nombreObjeto_arrTipoObject))
                {
                    return obtenerTipoArregloRecursivo((Arreglo)arbol.retornarArreglo(arreglo.nombreObjeto_arrTipoObject),arbol);
                }
                
                if(arbol.existeEstructura(arreglo.nombreObjeto_arrTipoObject))
                {
                    return TipoDatos.Object;
                }

                return TipoDatos.NULL;
            }
            else
            {
                return arreglo.tipoArreglo;
            }

        }


        public List<string[]> obtenerNivelesRecursivos(Arreglo arreglo, AST arbol)
        {
            List<string[]> aux =  new List<string[]>();

            foreach (string[] item in arreglo.niveles)
            {
                aux.Add(item);
            }

            // Validamos si el arreglo es de tipo arreglo, recursivamente, 
            // buscamos llegar al ultimo tipo ya sea primitivo o un struct
            if (arreglo.nombreObjeto_arrTipoObject != null)
            {
                // Comprobamos que la estrucutra generadora exista y sea un arreglo
                if (arbol.existeArreglo(arreglo.nombreObjeto_arrTipoObject))
                {
                    // obtenemos el arreglo y agregamos los niveles al nuevo arreglo
                    Arreglo siguiente = arbol.retornarArreglo(arreglo.nombreObjeto_arrTipoObject);
                    foreach (string[] interno in obtenerNivelesRecursivos(siguiente,arbol))
                    {
                        aux.Add(interno);
                    }
                }
            }

            return aux;
        }

        public void obtenerListasAnidadas(LinkedList<string> variablesUsadas)
        {

        }
    }
}
