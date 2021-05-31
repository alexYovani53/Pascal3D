using Pascal3D.Optimizador.InterfacesOp;
using Pascal3D.Optimizador.PrimitivosOp;
using System;
using System.Collections.Generic;
using System.Text;
using static Pascal3D.Optimizador.PrimitivosOp.OperacionOp;

namespace Pascal3D.Optimizador.AsignacionOp
{
    public class AsignacionOpti:InstruccionOp
    {

        public int linea { get; set; }
        public int columna { get; set; }

        public ExpresionOp valorAsig { get; set; }

        public ExpresionOp temp { get; set; }

        public bool instruccionEliminada { get; set; }

        public AsignacionOpti(ExpresionOp temp,ExpresionOp valor, int linea, int columna)
        {
            this.temp = temp;
            this.valorAsig = valor;
            this.linea = linea;
            this.columna = columna;
        }

        public string pedirCodigo()
        {
            if (!instruccionEliminada)
            {
                return $"{temp.getValor()} = {valorAsig.getValor()};";
            }
            return "";
        }

        public object ejecutarOptimizacion()
        {

            if (!(valorAsig is OperacionOp)) return 0;

            OperacionOp operacionAsign = (OperacionOp)valorAsig;
            ExpresionOp izquierda = operacionAsign.izq;
            ExpresionOp derecha = operacionAsign.der;

            // COMPARACION DEL TIPO
            //  EJE:   T1 = T1 + <VALOR>
            if (temp.getValor().Equals(izquierda.getValor()))
            {
                // REGLA 6
                // T1 = T1 + 0;  -> ELIMINAR INSTRUCCION
                if (derecha.getValor().Equals("0") && operacionAsign.operador == tipoOperacionOpti.suma)
                {
                    RegistroOptimizacion.agregarOptimizacion("REGLA 6", pedirCodigo(), "", linea, columna, "----");
                    instruccionEliminada = true;
                    return 0;
                }
                // REGLA 7
                // T1 = T1 - 0;  -> ELIMINAR INSTRUCCION
                else if (derecha.getValor().Equals("0") && operacionAsign.operador == tipoOperacionOpti.resta)
                {
                    RegistroOptimizacion.agregarOptimizacion("REGLA 7", pedirCodigo(), "", linea, columna, "----");
                    instruccionEliminada = true;
                    return 0;
                }

                // REGLA 8
                // T1 = T1 * 1;  -> ELIMINAR INSTRUCCION
                else if (derecha.getValor().Equals("1") && operacionAsign.operador == tipoOperacionOpti.multiplicacion)
                {
                    RegistroOptimizacion.agregarOptimizacion("REGLA 8", pedirCodigo(), "", linea, columna, "----");
                    instruccionEliminada = true;
                    return 0;
                }
                // REGLA 9
                // T1 = T1 / 1;  -> ELIMINAR INSTRUCCION
                else if (derecha.getValor().Equals("1") && operacionAsign.operador == tipoOperacionOpti.division)
                {
                    RegistroOptimizacion.agregarOptimizacion("REGLA 9", pedirCodigo(), "", linea, columna, "----");
                    instruccionEliminada = true;
                    return 0;
                }
            }

            // COMPARACION DEL TIPO
            //  EJE:   T1 = <VALOR> + T1;
            else if (temp.getValor().Equals(derecha.getValor()))
            {
                // REGLA 6
                // T1 = 0 + T1;  -> ELIMINAR INSTRUCCION
                if (izquierda.getValor().Equals("0") && operacionAsign.operador == tipoOperacionOpti.suma)
                {
                    RegistroOptimizacion.agregarOptimizacion("REGLA 6", pedirCodigo(), "", linea, columna, "----");
                    instruccionEliminada = true;
                    return 0;
                }
                // REGLA 8
                // T1 = 1 * T1;  -> ELIMINAR INSTRUCCION
                else if (izquierda.getValor().Equals("1") && operacionAsign.operador == tipoOperacionOpti.multiplicacion)
                {
                    RegistroOptimizacion.agregarOptimizacion("REGLA 8", pedirCodigo(), "", linea, columna, "----");
                    instruccionEliminada = true;
                    return 0;
                }
            }

            /*
             * 
             *                  REGLAS DE 10,11,12,13,14,15,16 
             * 
             * 
             */
            else if(izquierda is NumeroOp && derecha is NumeroOp)
            {
                double izq_ = Double.Parse(izquierda.getValor());
                double der_ = Double.Parse(derecha.getValor());

                double resultado;
                switch (operacionAsign.operador)
                {
                    case tipoOperacionOpti.suma:
                        resultado = izq_ + der_;
                        break;
                    case tipoOperacionOpti.resta:
                        resultado = izq_ - der_;
                        break;
                    case tipoOperacionOpti.multiplicacion:
                        resultado = izq_ * der_;
                        break;
                    case tipoOperacionOpti.division:
                        resultado = izq_ / der_;
                        break;
                    case tipoOperacionOpti.modulo:
                        resultado = izq_ % der_;
                        break;
                    default:
                        resultado = 0;
                        break;
                }

                string codigoAntes = pedirCodigo();
                valorAsig = new NumeroOp(resultado, valorAsig.linea, valorAsig.columna);
                string codigoDespues = pedirCodigo();
                RegistroOptimizacion.agregarOptimizacion("REGLA 6-16", codigoAntes, codigoDespues, linea, columna, "----");
                return 0;

            }
            else if (izquierda is NumeroOp)
            {
                string codigoAntes;
                string codigoDespues;

                // REGLA 10
                // T1 =  0 + T2;  -> T1 = T2;
                if (izquierda.getValor().Equals("0") && operacionAsign.operador == tipoOperacionOpti.suma)
                {
                    codigoAntes = pedirCodigo();
                    valorAsig = derecha;
                    codigoDespues = pedirCodigo();
                    RegistroOptimizacion.agregarOptimizacion("REGA 10", codigoAntes, codigoDespues, linea, columna, "----");
                    return 0;
                }

                // REGLA 11
                // T1 = 0 - T2;  ->  NO APLICA;


                // REGLA 12
                // T1 = 1 * T2;  -> T1 = T2;
                if (izquierda.getValor().Equals("1") && operacionAsign.operador == tipoOperacionOpti.multiplicacion)
                {
                    codigoAntes = pedirCodigo();
                    valorAsig = derecha;
                    codigoDespues = pedirCodigo();
                    RegistroOptimizacion.agregarOptimizacion("REGA 12", codigoAntes, codigoDespues, linea, columna, "----");
                    return 0;
                }

                // REGLA 13
                // T1 = 1 / T2;  -> NO APLICA
                

                // REGLA 14
                // T1 = 2 * T2 ;  -> T1 = T2 + T2;
                if (izquierda.getValor().Equals("2") && operacionAsign.operador == tipoOperacionOpti.multiplicacion)
                {
                    codigoAntes = pedirCodigo();
                    valorAsig = new OperacionOp(derecha, tipoOperacionOpti.suma, derecha, operacionAsign.linea, operacionAsign.columna);
                    codigoDespues = pedirCodigo();
                    RegistroOptimizacion.agregarOptimizacion("REGA 14", codigoAntes, codigoDespues, linea, columna, "----");
                    return 0;
                }


                // REGLA 15
                // T1 = 0 * T2;  -> T1 = 0;
                if (izquierda.getValor().Equals("0") && operacionAsign.operador == tipoOperacionOpti.multiplicacion)
                {
                    codigoAntes = pedirCodigo();
                    valorAsig = new NumeroOp(0, operacionAsign.linea, operacionAsign.columna);
                    codigoDespues = pedirCodigo();
                    RegistroOptimizacion.agregarOptimizacion("REGA 15", codigoAntes, codigoDespues, linea, columna, "----");
                    return 0;
                }


                // REGLA 16
                // T1 = 0 / T2;  -> T1 = 0;
                else if (izquierda.getValor().Equals("0") && operacionAsign.operador == tipoOperacionOpti.division)
                {
                    codigoAntes = pedirCodigo();
                    valorAsig = new NumeroOp(0, operacionAsign.linea, operacionAsign.columna);
                    codigoDespues = pedirCodigo();
                    RegistroOptimizacion.agregarOptimizacion("REGA 16", codigoAntes, codigoDespues, linea, columna, "----");
                    return 0;
                }
            }

            else if(derecha is NumeroOp)
            {
                string codigoAntes;
                string codigoDespues;

                // REGLA 10
                // T1 = T2 + 0;  -> T1 = T2;
                if (derecha.getValor().Equals("0") && operacionAsign.operador == tipoOperacionOpti.suma)
                {
                    codigoAntes = pedirCodigo();
                    valorAsig = izquierda;
                    codigoDespues = pedirCodigo();
                    RegistroOptimizacion.agregarOptimizacion("REGA 10", codigoAntes, codigoDespues, linea, columna, "----");
                    return 0;
                }

                // REGLA 11
                // T1 = T2 - 0;  -> T1 = T2;
                else if (derecha.getValor().Equals("0") && operacionAsign.operador == tipoOperacionOpti.resta)
                {
                    codigoAntes = pedirCodigo();
                    valorAsig = izquierda;
                    codigoDespues = pedirCodigo();
                    RegistroOptimizacion.agregarOptimizacion("REGA 11", codigoAntes, codigoDespues, linea, columna, "----");
                    return 0;
                }

                // REGLA 12
                // T1 = T2 * 1;  -> T1 = T2;
                if (derecha.getValor().Equals("1") && operacionAsign.operador == tipoOperacionOpti.multiplicacion)
                {
                    codigoAntes = pedirCodigo();
                    valorAsig = izquierda;
                    codigoDespues = pedirCodigo();
                    RegistroOptimizacion.agregarOptimizacion("REGA 12", codigoAntes, codigoDespues, linea, columna, "----");
                    return 0;
                }

                // REGLA 13
                // T1 = T2 / 1;  -> T1 = T2;
                if (derecha.getValor().Equals("0") && operacionAsign.operador == tipoOperacionOpti.division)
                {
                    codigoAntes = pedirCodigo();
                    valorAsig = izquierda;
                    codigoDespues = pedirCodigo();
                    RegistroOptimizacion.agregarOptimizacion("REGA 13", codigoAntes, codigoDespues, linea, columna, "----");
                    return 0;
                }

                // REGLA 14
                // T1 = T2 * 2 ;  -> T1 = T2 + T2;
                if (derecha.getValor().Equals("2") && operacionAsign.operador == tipoOperacionOpti.multiplicacion)
                {
                    codigoAntes = pedirCodigo();
                    valorAsig = new OperacionOp(izquierda,tipoOperacionOpti.suma,izquierda,operacionAsign.linea,operacionAsign.columna);
                    codigoDespues = pedirCodigo();
                    RegistroOptimizacion.agregarOptimizacion("REGA 14", codigoAntes, codigoDespues, linea, columna, "----");
                    return 0;
                }


                // REGLA 15
                // T1 = T2 * 0 ;  -> T1 = 0;
                if (derecha.getValor().Equals("0") && operacionAsign.operador == tipoOperacionOpti.multiplicacion)
                {
                    codigoAntes = pedirCodigo();
                    valorAsig = new NumeroOp(0, operacionAsign.linea, operacionAsign.columna);
                    codigoDespues = pedirCodigo();
                    RegistroOptimizacion.agregarOptimizacion("REGA 15", codigoAntes, codigoDespues, linea, columna, "----");
                    return 0;
                }
            }




            return "";
        }
    }

}
