using CompiPascal.AST_.cambioFlujo;
using CompiPascal.AST_.interfaces;
using CompiPascal.AST_.valoreImplicito;
using CompiPascal.entorno_;
using CompiPascal.entorno_.simbolos;
using Pascal3D;
using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_.control
{
    public class SwitchCase : Instruccion
    {

        public int tamanoPadre { get; set; }
        private Expresion exprValidar { get; set; }

        private LinkedList<Case> casos { get; set; }

        private Case casoDefault { get; set; }

        public int linea { get; set; }
        public int columna { get; set; }

        public SwitchCase(Expresion exprValidar,  LinkedList<Case> casos, int linea, int columna )
        {
            this.exprValidar = exprValidar;
            this.casos = casos;
            this.linea = linea;
            this.columna = columna;

        }

        public SwitchCase(Expresion exprValidar, LinkedList<Case> casos, Case casoElse, int linea, int columna)
        {
            this.exprValidar = exprValidar;
            this.casoDefault = casoElse;
            this.casos = casos;
            this.linea = linea;
            this.columna = columna;

        }



        public string getC3(Entorno ent, AST arbol)
        {

            string codigo = "/********************* SWITCH CASE ***************************************/\n\n";

            string etiquetaSalida = Generador.pedirEtiqueta();
            string etiqueta1 = Generador.pedirEtiqueta();

            foreach (Case item in casos)
            {
                // HAREMOS UNA CONDICION PARA COMPARAR SI LA EXPRESION A VALIDAR ES IGUAL A LA DEL CASE ACTUAL
                Operacion condicion = new Operacion(exprValidar,item.expresionCase,Operacion.Operador.IGUAL,item.linea,item.columna);

                codigo += $"{etiqueta1}: \n";
                result3D expresion = condicion.obtener3D(ent);

                if(expresion.TipoResultado != TipoDatos.Boolean)
                {
                    Program.getIntefaz().agregarError("La expresion para un Case debe ser de tipo booleano", item.linea, item.columna);
                    return "";
                }


                codigo += expresion.Codigo;
                codigo += $"{expresion.EtiquetaV}: \n";

                codigo += item.getC3(ent,arbol);

                codigo += Generador.tabularLinea($"goto {etiquetaSalida}; \n", 1);

                etiqueta1 = expresion.EtiquetaF;
            }

            codigo += $"{etiqueta1}: "; 

            if(casoDefault != null)
            {
                codigo += "\n";
                codigo += casoDefault.getC3(ent,arbol);
            }


            codigo += $"{etiquetaSalida}: /*    Etiqueta de salida switch   */ \n\n";



            return Generador.tabular(codigo);
        }

        public void obtenerListasAnidadas(LinkedList<string> variablesUsadas)
        {
            if (casoDefault != null) casoDefault.obtenerListasAnidadas(variablesUsadas);
            foreach (Instruccion item in casos)
            {
                item.obtenerListasAnidadas(variablesUsadas);
            }
        }
    }
}
