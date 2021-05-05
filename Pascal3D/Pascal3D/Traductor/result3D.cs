using System;
using System.Collections.Generic;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace Pascal3D.Traductor
{
    public class result3D
    {


        /* @param       TipoDatos       tipoResultado
         * @comentario                  Esta propiedad guarada el tipo del resultado de una expresion 3D
         */
        private TipoDatos tipoResultado { get; set; }

        /* @param       string          codigo
         * @comentario                  Esta propiedad guarada el codigo 3D de una expresion u operacion
         */
        private string codigo { get; set; }

        /* @param       string          temporal
         * @comentario                  Esta propiedad guarada la etiqueta temporal donde se guarda el valor
         *                              temporal de una expresion
         */
        private string temporal { get; set; }

        /* @param       string          etiquetaV
         * @comentario                  Esta propiedad guarada la etiqueta verdadera de una expresion logica o relacional
         */
        private string etiquetaV { get; set; }

        /* @param       string          etiquetaF
         * @comentario                  Esta propiedad guarada la etiqueta falsa de una expresion logica o relacional
         */
        private string etiquetaF { get; set; }

        private object referencia { get; set; }
        public result3D()
        {
            this.tipoResultado = TipoDatos.NULL;
            this.codigo = "";
            this.temporal = "";
            this.etiquetaF = "";
            this.etiquetaV = "";
            this.referencia = 0;
        }


        /*
         * 
         *              METODS PARA ASIGNACIÓN Y OBTENCIÓN
         * 
         */
        public string Temporal
        {
            get
            {
                return temporal;
            }

            set
            {
                this.temporal = value;
            }

        }


        public string Codigo
        {
            get
            {
                return codigo;
            }

            set
            {
                this.codigo = value;
            }

        }

        public TipoDatos TipoResultado
        {
            get
            {
                return tipoResultado;
            }

            set
            {
                this.tipoResultado = value;
            }

        }

        public string EtiquetaV
        {
            get
            {
                return etiquetaV;
            }

            set
            {
                this.etiquetaV = value;
            }

        }

        public string EtiquetaF
        {
            get
            {
                return etiquetaF;
            }

            set
            {
                this.etiquetaF = value;
            }

        }

        public object Referencia
        {
            get
            {
                return referencia;
            }

            set
            {
                this.referencia = value;
            }

        }


    }
}
