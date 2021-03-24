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

    }
}
