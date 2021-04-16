using System;
using System.Collections.Generic;
using System.Text;

namespace Pascal3D.Traductor
{
    public class Generador
    {

        //ESTAS VARIABLES LLEVAN EL CONTEO DE LAS ETIQUETAS Y DE LOS TEMPORALES
        public static int etiquetas = 0;
        public static int temporales = 0;

        public static string cadenaFinal = "";

        public static int ptrHeap = 0;
        public static int ptrStack = 0;


        public static string pedirTemporal()
        {
            string temp = "";
            temp += "t" + temporales.ToString();
            temporales++;
            return temp;
        }

        public static string pedirEtiqueta()
        {
            string etiqueta = "";
            etiqueta += "L" + etiquetas.ToString();
            etiquetas++;
            return etiqueta;
        }


        public static void reiniciar()
        {
            temporales = 0;
            etiquetas = 0;
        }

        public static string tabular(string codigo )
        {
            string nuevoCodigo = "";

            foreach (string item in codigo.Split('\n'))
            {
                nuevoCodigo += "\t"+item+"\n";
            }
            return nuevoCodigo;

        }

        public static string tabularLinea(string codigo,int tabs)
        {
            string salida = "";
            for (int i = 0; i < tabs; i++)
            {
                salida += '\t';
            }
            salida += codigo + "\n" ;
            return salida;
        }




        public static string cabezera()
        {

            string codigo = "#include <stdio.h> \n";

            codigo += "\t\t float Heap[100000]; //Estructura heap \n";
            codigo += "\t\t float Stack[100000]; //Estructura stack \n";

            codigo += "int SP=0; //Puntero al stack  \n  ";
            codigo += "int HP=0; //Puntero al heap  \n  ";

            if (temporales > 0)
            {
                codigo += "float ";
                for (int i = 0; i < temporales; i++)
                {
                    if (i % 15 == 0 && i>0) codigo += '\n';
                    codigo += "t" + i;
                    if (i < temporales - 1) codigo += ", ";
                }

                codigo += "; \n\n";
            }


            return codigo;
        }

    }
}
