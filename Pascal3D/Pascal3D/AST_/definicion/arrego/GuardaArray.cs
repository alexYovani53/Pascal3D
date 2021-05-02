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
    public class GuardaArray : Instruccion
    {
        public int tamanoPadre { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }

        public string identificador { get; set; }

        public string tipoObjeto { get; set; }

        public TipoDatos  tipo { get; set; }

        public  LinkedList<Expresion[]> niveles { get; set; }

        public GuardaArray(string ide, string tipoObjeto, TipoDatos tipo, LinkedList<Expresion[]> niveles, int linea, int columna)
        {
            this.identificador = ide;
            this.tipoObjeto = tipoObjeto;
            this.tipo = tipo;
            this.niveles = niveles;
            this.linea = linea;
            this.columna = columna;
        }


        public string getC3(Entorno ent, AST arbol)
        {
            string codigo = "";
            bool existe = arbol.existeArreglo(identificador);

            if (existe)
            {
                Program.getIntefaz().agregarError("El identificador" + identificador + " ya tiene una definicion en el entorno actual", 0, 0);
                return codigo;
            }

            //GENERAMOS LAS OPERACIONES QUE DEFINEN EL INICIO Y FIN DE LOS ARREGLOS         -> [1*9+3..3*6]  resultado inicio = 15   final 18 tamano-> 18-15 (contando extremos)  = 4 
            List<result3D[]> valsNiveles = dimensiones(ent, arbol);

            List<string[]> valsNivles_TEmporales = new List<string[]>();
            foreach (result3D[] item in valsNiveles)
            {
                codigo += "/* Determinando dimension de un arreglo */\n";
                codigo += Generador.tabular(item[0].Codigo);
                codigo += Generador.tabular(item[1].Codigo);
                codigo += "/* Fin determinando dimension de un arreglo */\n";

                valsNivles_TEmporales.Add(new string[] {item[0].Temporal,item[1].Temporal});
            }


            Arreglo estructuraNuev;
            if (tipoObjeto == null)
            {
                //SI EL TIPO DE DATO ES UN PRIMITIVO ENTONCES GENERAMOS UN ARREGLO SIN  "TIPOOBJETO"
                estructuraNuev = new Arreglo(identificador, tipo, valsNivles_TEmporales);
            }
            else
            {
                estructuraNuev = new Arreglo(identificador, tipoObjeto, tipo, valsNivles_TEmporales);
            }

            arbol.agregarArreglo(estructuraNuev);

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

                valsFinales.Add(new result3D[] { inicio,tamano});

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

            if (val1.TipoResultado != TipoDatos.Integer || val2.TipoResultado!=TipoDatos.Integer)
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
