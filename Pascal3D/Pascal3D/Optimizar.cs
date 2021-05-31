
using Irony.Parsing;
using Pascal3D.GUI;
using Pascal3D.Optimizador;
using Pascal3D.Optimizador.Analizador;
using Pascal3D.Optimizador.ValorImplicitoOp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Pascal3D
{
    public partial class Optimizar : Form
    {


        #region Numbers, Bookmarks, Code Folding

        /// <summary>
        /// the background color of the text area
        /// </summary>
        private const int BACK_COLOR = 0x1E1E1E;



        /// <summary>
        /// default text color of the text area
        /// </summary>
        private const int FORE_COLOR = 0xF0CA95;

        /// <summary>
        /// change this to whatever margin you want the line numbers to show in
        /// </summary>
        private const int NUMBER_MARGIN = 1;

        /// <summary>
        /// change this to whatever margin you want the bookmarks/breakpoints to show in
        /// </summary>
        private const int BOOKMARK_MARGIN = 2;
        private const int BOOKMARK_MARKER = 2;

        /// <summary>
        /// change this to whatever margin you want the code folding tree (+/-) to show in
        /// </summary>
        private const int FOLDING_MARGIN = 3;

        /// <summary>
        /// set this true to show circular buttons for code folding (the [+] and [-] buttons on the margin)
        /// </summary>
        private const bool CODEFOLDING_CIRCULAR = true;

        #endregion

        DataTable modeloOptimizacion = null;
        public Optimizar(string texto)
        {
            InitializeComponent();

            ConsolaSalida.iniciar(SalidaTexto);
            ConsolaSalida.iniciar(AreaTexto);

            AreaTexto.Text = texto;
            iniciarModeloOptimizacion();
        }
        public void iniciarModeloOptimizacion()
        {


            modeloOptimizacion = new DataTable();
            modeloOptimizacion.Columns.Add("REGLA");
            modeloOptimizacion.Columns.Add("CODIGO ELIMINADO");
            modeloOptimizacion.Columns.Add("CODIGO NUEVO");
            modeloOptimizacion.Columns.Add("LINEA");
            modeloOptimizacion.Columns.Add("COLUMNA");
            modeloOptimizacion.Columns.Add("DESCRIPCION");

            this.tablaErrores.DataSource = modeloOptimizacion;

            tablaErrores.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            tablaErrores.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
        }


        private void btnOptimizar_Click(object sender, EventArgs e)
        {
            SalidaTexto.Text = "";
            RegistroOptimizacion.registros = new LinkedList<RegistroOptimizacion>();
            if (AreaTexto.Text.Equals("")) return;

            GramaticaOP grama = new GramaticaOP();
            LanguageData lengua = new LanguageData(grama);

            //INSTANCIA DEL PARSER
            Parser parseador = new Parser(lengua);
            ParseTree ARBOL_IRONY = parseador.Parse(AreaTexto.Text);

            if (!ARBOL_IRONY.HasErrors())
            {
                ArmarOptimizador armador = new ArmarOptimizador();
                AST_OP arbolOptimizar =  armador.armarArbolOptimizar(ARBOL_IRONY);


                List<Temp_SP_HP> copiaTemps = new List<Temp_SP_HP>(arbolOptimizar.temporales);

                SalidaTexto.Text += arbolOptimizar.encabezado_.pedirCodigo();
                SalidaTexto.Text += "\n";
                SalidaTexto.Text += $"float {copiaTemps[0].temp_sp_hp}";
                int lineaTemps = 0;

                for (int i = 1; i < arbolOptimizar.temporales.Count; i++)
                {
                    var item = copiaTemps[i];   
                    SalidaTexto.Text += ", " + item.temp_sp_hp;
                    if (lineaTemps == 20)
                    {
                        SalidaTexto.Text += "\n";
                        lineaTemps = 0;
                    }
                    else lineaTemps++;
                }

                SalidaTexto.Text += ";\n";

                foreach (var item in arbolOptimizar.Funciones)
                {
                    item.ejecutarOptimizacion();
                }

                foreach (var item in arbolOptimizar.Funciones)
                {
                    SalidaTexto.Text += item.pedirCodigo();
                }

                generarReporteOptimizacion();

            }
            else
            {
                foreach (var item in ARBOL_IRONY.ParserMessages)
                {
                    SalidaTexto.Text += item.Message +" --> linea: " + item.Location.Line + "--> columna: " + item.Location.Column +"\n";
                }

            }
            

        }

        public void generarReporteOptimizacion()
        {
            modeloOptimizacion.Clear();
            foreach (var item in RegistroOptimizacion.registros)
            {
                modeloOptimizacion.Rows.Add(item.regla, item.codigoRemplazado, item.codigoNuevo, item.linea, item.columna, item.descripcion);
            }
        }


    }
}
