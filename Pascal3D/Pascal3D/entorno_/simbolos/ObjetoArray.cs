using Pascal3D.Traductor;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.entorno_.simbolos
{
    public class ObjetoArray : Simbolo,ICloneable
    {



        /* @propiedad    string      nombre
         * @comentario   esta propiedad guardara el nombre de la estructura tipo arreglo que la genero
         */
        string nombreStructArray { get; set; }

        public object[] valores {get;set;}

        private List<string[]> niveles { get; set; }

        public TipoDatos tipoValores { get; set; }


        public Objeto objetoParaAcceso { get; set; }

        public ObjetoArray(string nombreObjeto, string nombreStructArray,TipoDatos tipoArr,int direccion, List<string[]> niveles,int linea, int columna)
            : base(TipoDatos.Array, nombreObjeto,direccion,linea,columna)
        {
            this.nombreStructArray = nombreStructArray;
            this.niveles = niveles;
            this.tipoValores = tipoArr;
            this.objetoParaAcceso = null;

        }


        public result3D valor3D(List<string> accesos, Entorno entorno ,  int indiceNivel, string inicioArreglo)
        {
            result3D final = new result3D(); 
            string codigo = "";
            string iniNivel = Generador.pedirTemporal();
            string tamano = Generador.pedirTemporal();
            string valor = Generador.pedirTemporal();
            string posicion = Generador.pedirTemporal();


            List<string> clonado = new List<string>(accesos);
            string index = clonado[0];
            clonado.RemoveAt(0);

            if(clonado.Count > 0)
            {
                codigo += $"{iniNivel} = {niveles[indiceNivel][0]};\n";
                codigo += $"{tamano} = Heap[(int){inicioArreglo}]; /*Capturamos el tamaño almacenado del nivel*/ \n";

                codigo += $"{posicion} = {inicioArreglo};\n";
                codigo += $"{posicion} = {posicion} + {index}; /* Capturamos el indice*/ \n";
                codigo += $"{posicion} = {posicion} - {iniNivel}; /*Restamos el inicio del nivel, ya que pascal permite no Iniciar en 0*/\n";
                codigo += $"{posicion} = {posicion} + 1; /*Sumamos 1 porque el la posicion 0 esta el tamaño del arreglo*/\n";
                codigo += $"{valor} = Heap[(int){posicion}];\n";

                result3D subnivel = valor3D(clonado, entorno, indiceNivel + 1, valor);
                codigo += Generador.tabular(subnivel.Codigo);

                final.Codigo = codigo;
                final.Temporal = subnivel.Temporal;
                final.TipoResultado = subnivel.TipoResultado;

            }
            else
            {


                codigo += $"{iniNivel} = {niveles[indiceNivel][0]};\n";
                codigo += $"{tamano} = Heap[(int){inicioArreglo}]; /*Capturamos el tamaño almacenado del nivel*/ \n";

                codigo += $"{posicion} = {inicioArreglo};\n";
                codigo += $"{posicion} = {posicion} + {index}; /* Capturamos el indice*/ \n";
                codigo += $"{posicion} = {posicion} - {iniNivel}; /*Restamos el inicio del nivel, ya que pascal permite no Iniciar en 0*/\n";
                codigo += $"{posicion} = {posicion} + 1; /*Sumamos 1 porque el la posicion 0 esta el tamaño del arreglo*/\n";
                codigo += $"{valor} = Heap[(int){posicion}];\n";

                final.Codigo = codigo;
                final.Temporal = valor;
                final.TipoResultado = tipoValores;

            }

            return final;
        }

        public List<string[]> getNiveles()
        {
            return niveles;
        }




        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public string establecerValor(List<string> accesos,  result3D valorAsignar, string direccionArreglo, int indiceNivel, int linea, int columna)
        {

            string codigo = "";
            string iniNivel = Generador.pedirTemporal();
            string tamano = Generador.pedirTemporal();
            string valor = Generador.pedirTemporal();
            string posicion = Generador.pedirTemporal();


            List<string> clonado = new List<string>(accesos);
            string index = clonado[0];
            clonado.RemoveAt(0);

            if (clonado.Count > 0)
            {
                codigo += $"{iniNivel} = {niveles[indiceNivel][0]};  /*Posicion de inicio en el arreglo*/\n";
                codigo += $"{tamano} = Heap[(int){direccionArreglo}]; /*Capturamos el tamaño almacenado del nivel*/ \n";

                codigo += $"{posicion} = {direccionArreglo};\n";
                codigo += $"{posicion} = {posicion} + {index}; /* Capturamos el indice*/ \n";
                codigo += $"{posicion} = {posicion} - {iniNivel}; /*Restamos el inicio del nivel, ya que pascal permite no Iniciar en 0*/\n";
                codigo += $"{posicion} = {posicion} + 1; /*Sumamos 1 porque el la posicion 0 esta el tamaño del arreglo*/\n";
                codigo += $"{valor} = Heap[(int){posicion}];\n";

                string resultado =  establecerValor(clonado, valorAsignar,valor, indiceNivel + 1, linea,columna);
                codigo += Generador.tabular(resultado);
                
            }
            else
            {


                codigo += $"{iniNivel} = {niveles[indiceNivel][0]};\n";
                codigo += $"{tamano} = Heap[(int){direccionArreglo}]; /*Capturamos el tamaño almacenado del nivel*/ \n";

                codigo += $"{posicion} = {direccionArreglo};\n";
                codigo += $"{posicion} = {posicion} + {index}; /* Capturamos el indice*/ \n";
                codigo += $"{posicion} = {posicion} - {iniNivel}; /*Restamos el inicio del nivel, ya que pascal permite no Iniciar en 0*/\n";
                codigo += $"{posicion} = {posicion} + 1; /*Sumamos 1 porque el la posicion 0 esta el tamaño del arreglo*/\n";
                codigo += $"Heap[(int){posicion}] = {valorAsignar.Temporal};\n";

              }

            return codigo;

        }



    }
}
