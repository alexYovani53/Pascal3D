using CompiPascal.AST_;
using CompiPascal.AST_.bucles;
using CompiPascal.AST_.control;
using CompiPascal.AST_.definicion;
using CompiPascal.AST_.funcionesPrimitivas;
using CompiPascal.AST_.interfaces;
using CompiPascal.AST_.valoreImplicito;
using CompiPascal.entorno_;
using CompiPascal.entorno_.simbolos;
using CompiPascal.GUI.Archivo;
using CompiPascal.Traductor;
using Irony.Parsing;
using Pascal3D.GUI;
using Pascal3D.Traductor;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pascal3D
{
    public partial class Pascal : Form
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

        public static LinkedList<Error> Errores= null;

        DataTable modeloErrores = null;

        public Pascal()
        {
            InitializeComponent();
            
            iniciar();
            iniciarModeloErrores();
            Errores = new LinkedList<Error>();

            ConsolaSalida.iniciar(SalidaTexto);
        }

        public void iniciarModeloErrores()
        {


            modeloErrores = new DataTable();
            modeloErrores.Columns.Add("Tipo");
            modeloErrores.Columns.Add("Descripcion");
            modeloErrores.Columns.Add("Linea");
            modeloErrores.Columns.Add("Columna");

            this.tablaErrores.DataSource=modeloErrores;
        }

        private void iniciar()
        {
            //CREACION DEL AREA DE TEXTOV 
            //AreaTexto = new ScintillaNET.Scintilla();
            //panel1.Controls.Add(AreaTexto);

            //AJUSTE
            AreaTexto.Dock = System.Windows.Forms.DockStyle.Fill;
            //AreaTexto.TextChanged += (this.OnTextChanged);

            AreaTexto.WrapMode = WrapMode.None;
            AreaTexto.IndentationGuides = IndentView.LookBoth;

            // ESTILO
            InitColors();
            InitSyntaxColoring();

            // MARGEN DE NUMEROS
            InitNumberMargin();

            // BOOKMARK MARGIN
            InitBookmarkMargin();

            // CODE FOLDING MARGIN
            //InitCodeFolding();

            // DRAG DROP
            //InitDragDropFile();

            // INIT HOTKEYS
            //InitHotkeys();

        }

        private void InitBookmarkMargin()
        {
            var margin = AreaTexto.Margins[BOOKMARK_MARGIN];
            margin.Width = 20;
            margin.Sensitive = true;
            margin.Type = MarginType.Symbol;
            margin.Mask = (1 << BOOKMARK_MARKER);
            //margin.Cursor = MarginCursor.Arrow;

            var marker = AreaTexto.Markers[BOOKMARK_MARKER];
            marker.Symbol = MarkerSymbol.Circle;
            marker.SetBackColor(IntToColor(0xFF003B));
            marker.SetForeColor(IntToColor(0x000000));
            marker.SetAlpha(100);
        }

        private void InitNumberMargin()
        {
            AreaTexto.Styles[Style.LineNumber].BackColor = IntToColor(0x2A211C);
            AreaTexto.Styles[Style.LineNumber].ForeColor = IntToColor(0xB7B7B7);
            AreaTexto.Styles[Style.IndentGuide].ForeColor = IntToColor(0xB7B7B7);
            AreaTexto.Styles[Style.IndentGuide].BackColor = IntToColor(0x2A211C);

            var nums = AreaTexto.Margins[1];
            nums.Width = 30;
            nums.Type = MarginType.Number;
            nums.Sensitive = true;
            nums.Mask = 0;


            //AreaTexto.MarginClick += TextArea_MarginClick;
        }

        private void InitSyntaxColoring()
        {
            // Configure the default style
            AreaTexto.StyleResetDefault();
            AreaTexto.Styles[Style.Default].Font = "Consolas";
            AreaTexto.Styles[Style.Default].Size = 10;
            AreaTexto.Styles[Style.Default].BackColor = IntToColor(0x252526);
            AreaTexto.Styles[Style.Default].ForeColor = IntToColor(0xFFFFFF);

            AreaTexto.StyleClearAll();

            // Configure the CPP (C#) lexer styles
            AreaTexto.Styles[Style.Pascal.Identifier].ForeColor = IntToColor(0xD0DAE2);
            AreaTexto.Styles[Style.Pascal.Comment].ForeColor = IntToColor(0xBD758B);
            AreaTexto.Styles[Style.Pascal.CommentLine].ForeColor = IntToColor(0x40BF57);
            AreaTexto.Styles[Style.Pascal.Comment2].ForeColor = IntToColor(0x40BF57);
            AreaTexto.Styles[Style.Pascal.Number].ForeColor = IntToColor(0x00FF00);
            AreaTexto.Styles[Style.Pascal.String].ForeColor = IntToColor(0xFFFF00);
            AreaTexto.Styles[Style.Pascal.Character].ForeColor = IntToColor(0xFFD700);
            AreaTexto.Styles[Style.Pascal.Preprocessor].ForeColor = IntToColor(0x8AAFEE);
            AreaTexto.Styles[Style.Pascal.Operator].ForeColor = IntToColor(0xE0E0E0);
            AreaTexto.Styles[Style.Pascal.Word].ForeColor = IntToColor(0x48A8EE);


            AreaTexto.Lexer = Lexer.Pascal;

            AreaTexto.SetKeywords(0, " begin end program type of writeln write exit case do while else if for  switch function var while break continue return const default true false bool case char const continue do else false for  if  ");
            AreaTexto.SetKeywords(1, "void Null ArgumentError arguments Array Boolean Class Date DefinitionError Error EvalError Function int Math Namespace Number Object RangeError ReferenceError RegExp SecurityError String SyntaxError TypeError uint XML XMLList Boolean Byte Char DateTime Decimal Double Int16 Int32 Int64 IntPtr SByte Single UInt16 UInt32 UInt64 UIntPtr Void Path File System Windows Forms ScintillaNET");

        }

        private Color IntToColor(int rgb)
        {
            return Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }

        private void InitColors()
        {
            AreaTexto.SetSelectionBackColor(true, IntToColor(0x114D9C));
        }

        private void process1_Exited(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string texto = Abrir.cargarArchivo();
            if (texto.Equals(""))return;

            this.AreaTexto.Text = texto;
        }

        private void traducir_Click(object sender, EventArgs e)
        {

            Errores = new LinkedList<Error>();
            Generador.reiniciar();
            SalidaTexto.Text = "";

            if(!AreaTexto.Text.Equals(""))
            {

                //INSTANCIA DE LA GRAMATICA
                Gramatica grama3D = new Gramatica();
                LanguageData lenguaje = new LanguageData(grama3D);

                //INSTANCIA DEL PARSER
                Parser parseo = new Parser(lenguaje);
                ParseTree arbolIrony = parseo.Parse(AreaTexto.Text);


                if (!arbolIrony.HasErrors())
                {

                    ArmarAST armado = new ArmarAST();
                    AST ARBOL =  armado.generarAST(arbolIrony);

                    Entorno GLOBAL = new Entorno(null, "GLOBAL");
                    string codigoDeclaraciones = "";
                    string codigoMain = "";
                    string codigoFunciones = "";
                    if(ARBOL != null)
                    {
                        foreach (Instruccion item in ARBOL.obtenerInstrucciones())
                        {
                            if (item is Funcion && !(item is BeginEndPrincipal))
                            {
                                //AGREGAMOS LA FUNCION AL ENTORNO GLOBAL
                                Funcion funcionDeclarado = (Funcion)item;
                                GLOBAL.agregarSimbolo(funcionDeclarado.Identificador, funcionDeclarado);
                                codigoFunciones +=funcionDeclarado.getC3(GLOBAL, ARBOL);

                            }
                            else if (item is Declaracion)
                            {

                                string codTemp = item.getC3(GLOBAL, ARBOL);
                                codTemp = Generador.tabular(codTemp);
                                codigoDeclaraciones += codTemp;

                            }
                            else if(item is DeclararStruct)
                            {
                                string codTemp = item.getC3(GLOBAL, ARBOL);
                                codTemp = Generador.tabular(codTemp);
                                codigoDeclaraciones += codTemp;
                            }
                            else if(item is GuardarStruct)
                            {
                                string codTemp = item.getC3(GLOBAL, ARBOL);
                                codTemp = Generador.tabular(codTemp);
                                codigoDeclaraciones += codTemp;
                            }

                            else if(item is BeginEndPrincipal)
                            {


                                foreach (Instruccion interna in ((BeginEndPrincipal)item).instrucciones)
                                {
                                    if (interna is If)
                                    {
                                        string codTemp = ((If)interna).getC3(GLOBAL, ARBOL);
                                        codTemp = Generador.tabular(codTemp);
                                        codigoMain += codTemp;
                                    }

                                    else if (interna is While)
                                    {
                                        string codTemp = ((While)interna).getC3(GLOBAL, ARBOL);
                                        codTemp = Generador.tabular(codTemp);
                                        codigoMain += codTemp;
                                    }
                                    else if (interna is Repeat)
                                    {
                                        string codTemp = ((Repeat)interna).getC3(GLOBAL, ARBOL);
                                        codTemp = Generador.tabular(codTemp);
                                        codigoMain += codTemp;

                                    }
                                    else if (interna is Write)
                                    {
                                        string codTemp = ((Write)interna).getC3(GLOBAL, ARBOL);
                                        codTemp = Generador.tabular(codTemp);
                                        codigoMain += codTemp;

                                    }
                                    else if(interna is Llamada)
                                    {
                                        string codTemp = ((Llamada)interna).getC3(GLOBAL, ARBOL);
                                        codTemp = Generador.tabular(codTemp);
                                        codigoMain += codTemp;
                                    }
                                    else if (interna is Asignacion)
                                    {
                                        string codTemp = ((Asignacion)interna).getC3(GLOBAL, ARBOL);
                                        codTemp = Generador.tabular(codTemp);
                                        codigoMain += codTemp;
                                    }
                                    else if (interna is For)
                                    {
                                        string codTemp = ((For)interna).getC3(GLOBAL, ARBOL);
                                        codTemp = Generador.tabular(codTemp);
                                        codigoMain += codTemp;
                                    }
                                    else if (interna is SwitchCase)
                                    {
                                        string codTemp = ((SwitchCase)interna).getC3(GLOBAL,ARBOL);
                                        codTemp = Generador.tabular(codTemp);
                                        codigoMain += codTemp;
                                    }



                                }


                            }

                        }

                        SalidaTexto.Text = Generador.cabezera();
                        SalidaTexto.Text += codigoFunciones;
                        SalidaTexto.Text += "void main() {\n\n";
                        SalidaTexto.Text += codigoDeclaraciones + codigoMain;
                        SalidaTexto.Text += "\treturn 0; \n";
                        SalidaTexto.Text += "}\n";


                    }

                }
                else
                {

                    foreach (var item in arbolIrony.ParserMessages)
                    {
                        Error nuevo = new Error(Error.tipoError.Sintactico, item.Message, item.Location.Line, item.Location.Column);
                        Errores.AddLast(nuevo);
                    }

                }//FIN HAY ERRORES

            }//FIN DE TRADUCCIÓN
            

            if(Errores != null && Errores.Count > 0)
            {

                DialogResult mostrarErrores = MessageBox.Show("Existen errores, Quiere verlos?","Errores",MessageBoxButtons.YesNo);
                if(mostrarErrores == DialogResult.Yes)
                {
                    tabControl1.SelectedIndex = 0;
                    generarTablaErrores();
                }
            }

        }


        public void agregarError(string descripcion, int linea, int columna)
        {
            Errores.AddLast(new Error(Error.tipoError.Semantico, descripcion, linea, columna));
        }

        public void agregarTexto(string salida)
        {
            SalidaTexto.AppendText(salida);
        }

        private void generarTablaErrores()
        {
            modeloErrores.Rows.Clear();
            foreach (Error item in Errores)
            {
                modeloErrores.Rows.Add(item.TipoE, item.descripcion, item.fila, item.Columna);
            }
        }

    }
}
