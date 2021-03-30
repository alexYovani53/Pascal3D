﻿using CompiPascal.AST_.interfaces;
using CompiPascal.entorno_;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompiPascal.AST_.cambioFlujo
{
    public class Continue : Instruccion
    {


        public int tamanoPadre { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }

        public bool siPuedeRetornar { get; set; }
        public Continue(int linea, int columna)
        {
            this.linea = linea;
            this.columna = columna;
        }


        public string getC3(Entorno ent)
        {
            throw new NotImplementedException();
        }
    }
}