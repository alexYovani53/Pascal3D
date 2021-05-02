using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using Pascal3D;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.definicion.arrego
{
    public class DeclaraArray2:Instruccion
    {
        public int tamanoPadre { get; set; }
        public int linea { get ; set; }
        public int columna { get ; set; }

        LinkedList<Simbolo> variables { get; set; }
        
        string nombreObjeto { get; set; }

        LinkedList<Expresion[]> niveles { get; set; }

        TipoDatos tipo { get; set; }

        public bool objetoInterno;

        public string temporalCambioEntorno;

        public DeclaraArray2(LinkedList<Simbolo> variables, string tipoObjeto, TipoDatos tipo, LinkedList<Expresion[]> niveles, int linea, int columna)
        {
            this.variables = variables;
            this.linea = linea;
            this.columna = columna;
            this.nombreObjeto = tipoObjeto;
            this.niveles = niveles;
            this.tipo = tipo;
        }



        public string getC3(Entorno ent, AST arbol)
        {
            string codigo = "";

            List<string[]> listaNiveles = new List<string[]>();

            List<result3D[]> auxiliar = dimensiones(ent, arbol);

            foreach (result3D[] item in auxiliar)
            {
                codigo += "/* Determinando dimension de un arreglo */\n";
                codigo += Generador.tabular(item[0].Codigo);
                codigo += Generador.tabular(item[1].Codigo);
                codigo += "/* Fin determinando dimension de un arreglo */\n\n";

                listaNiveles.Add(new string[] { item[0].Temporal, item[1].Temporal });
            }

            foreach (Simbolo item in variables)
            {
                DeclaraArray declarNuevo = new DeclaraArray(item.Identificador, "", nombreObjeto, tipo, listaNiveles, item.linea, item.columna)
                {
                    objetoInterno = this.objetoInterno,
                    temporalCambioEntorno = this.temporalCambioEntorno
                };
                
                
                codigo += Generador.tabular(declarNuevo.getC3(ent,arbol));
            }

            return codigo;
        }



        /* @funcion         dimension               se encarga de obtener el inicio y el tamano de cada dimension del arreglo
         * @parametro       Entorno         ent     es el entorno donde se efectua la operacion
         * @parametro       AST             arbol   arrastre del arbol AST*/
        public List<result3D[]> dimensiones(Entorno ent, AST arbol)
        {

            List<result3D[]> valsFinales = new List<result3D[]>();
            foreach (Expresion[] item in niveles)
            {
                result3D tamano = obtenerTamano(item, ent, arbol);
                result3D inicio = inicioDimension(item[0], ent);

                valsFinales.Add(new result3D[] { inicio, tamano });

            }

            return valsFinales;

        }

        /* @funcion         obtenerTamano           se encarga de calcular el tamano de las expresiones del arreglo (incluyendo extremos)
         * @parametro       Expresion[]     dimension   esta contiene dos expresiones, la expresion de inicio y la de fin, las cuales deben ser ejecutadas y calculadas
         * @parametro       Entorno         ent         es el entorno donde se efectua la operacion
         * @parametro       AST             arbol       arrastre del arbol AST*/
        public result3D obtenerTamano(Expresion[] dimension, Entorno ent, AST arbol)
        {
            Expresion inicio = dimension[0];
            Expresion final = dimension[1];


            result3D val1 = inicio.obtener3D(ent);
            result3D val2 = final.obtener3D(ent);

            if (val1.TipoResultado != TipoDatos.Integer || val2.TipoResultado != TipoDatos.Integer)
            {
                Program.getIntefaz().agregarError(" La expresion para definir la dimension no es de tipo int", inicio.linea, inicio.columna);
                return new result3D();
            }

            string temporalTamano = Generador.pedirTemporal();
            result3D valFinal = new result3D();
            valFinal.Codigo += val1.Codigo;
            valFinal.Codigo += val2.Codigo;
            valFinal.Codigo += $"{temporalTamano} = {val2.Temporal} - {val1.Temporal};\n";
            valFinal.Codigo += $"{temporalTamano} = {temporalTamano} + 1;\n";

            valFinal.Temporal = temporalTamano;
            valFinal.TipoResultado = TipoDatos.Integer;

            return valFinal;
        }

        /* @funcion     inicioDimension         calcula el inicio de una de las dimensiones del arreglo
         * @parametro   Expresion           inicio          es la expresion inicio de la dimension actual obtenida del arrelo EXpresion[]
         * @parametro   Entorno             ent             entorno sobre la que se ejecuta la expresion
         * @parametro   AST                 arbol           es el AST que se esta armando*/
        public result3D inicioDimension(Expresion inicio, Entorno ent)
        {

            result3D valor = inicio.obtener3D(ent);

            if (valor.TipoResultado != TipoDatos.Integer)
            {
                Program.getIntefaz().agregarError("La expresion no retorna un valor entero", inicio.linea, inicio.columna);
                return new result3D();
            }

            return valor;

        }


        public void obtenerListasAnidadas(LinkedList<string> variablesUsadas)
        {


        }
    }
}
