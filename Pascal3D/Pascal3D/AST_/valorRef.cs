using CompiPascal.AST_.definicion;
using CompiPascal.AST_.definicion.arrego;
using CompiPascal.AST_.interfaces;
using CompiPascal.AST_.valoreImplicito;
using CompiPascal.entorno_;
using CompiPascal.entorno_.simbolos;
using System;
using System.Collections.Generic;
using System.Text;
using static CompiPascal.entorno_.Simbolo;

namespace CompiPascal.AST_
{
    public class valorRef:Instruccion
    {


        public int tamanoPadre { get; set; }
        public int linea { get; set; }
        public int columna { get; set  ; }

        Expresion referencia { get; set; }

        object referenciaOb { get; set; }

        Entorno entornoRef { get; set; }

        Expresion valor { get; set; }

        TipoDatos tipoRef { get; set; }

        object ObjetoValor { get; set; }

        public valorRef(Expresion referencia, Entorno entornoRef,TipoDatos tipoRef )
        {
            this.referencia = referencia;
            this.entornoRef = entornoRef;
            this.linea = referencia.linea;
            this.columna = referencia.columna;
            this.tipoRef = tipoRef;
            this.valor = null;
            referenciaOb = null;
        }
        public valorRef(object referencia, Entorno entornoRef, TipoDatos tipoRef,int linea, int columan)
        {
            this.referencia = null;
            this.entornoRef = entornoRef;
            this.linea = linea;
            this.columna = columna;
            this.tipoRef = tipoRef;
            this.valor = null;
            referenciaOb = referencia;
        }

  



        public string getC3(Entorno ent, AST arbol)
        {
            throw new NotImplementedException();
        }
    }
}
