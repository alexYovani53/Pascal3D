using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Pascal3D.GUI
{
    public class ConsolaSalida
    {

        #region Numbers, Bookmarks, Code Folding

        /// <summary>
        /// change this to whatever margin you want the line numbers to show in
        /// </summary>
        private const int NUMBER_MARGIN = 1;

        /// <summary>
        /// change this to whatever margin you want the bookmarks/breakpoints to show in
        /// </summary>
        private const int BOOKMARK_MARGIN = 2;
        private const int BOOKMARK_MARKER = 2;


        #endregion

        public static void iniciar(Scintilla consola)
        {
            //CREACION DEL AREA DE TEXTOV 
            //this = new ScintillaNET.Scintilla();
            //panel1.Controls.Add(this);

            //AJUSTE
            //Dock = System.Windows.Forms.DockStyle.Fill;
            //AreaTexto.TextChanged += (this.OnTextChanged);

            consola.WrapMode = WrapMode.None;
            consola.IndentationGuides = IndentView.LookBoth;

            // ESTILO
            InitColors(consola);
            InitSyntaxColoring(consola);

            // MARGEN DE NUMEROS
            InitNumberMargin(consola);

            // BOOKMARK MARGIN
            InitBookmarkMargin(consola);

            // CODE FOLDING MARGIN
            //InitCodeFolding();

            // DRAG DROP
            //InitDragDropFile();

            // INIT HOTKEYS
            //InitHotkeys();

        }

        private static void InitBookmarkMargin(Scintilla consola)
        {
            var margin = consola.Margins[BOOKMARK_MARGIN];
            margin.Width = 20;
            margin.Sensitive = true;
            margin.Type = MarginType.Symbol;
            margin.Mask = (1 << BOOKMARK_MARKER);
            //margin.Cursor = MarginCursor.Arrow;

            var marker = consola.Markers[BOOKMARK_MARKER];
            marker.Symbol = MarkerSymbol.Circle;
            marker.SetBackColor(IntToColor(0xFF003B));
            marker.SetForeColor(IntToColor(0x000000));
            marker.SetAlpha(100);
        }

        private static void InitNumberMargin(Scintilla consola)
        {
            consola.Styles[Style.LineNumber].BackColor = IntToColor(0x2A211C);
            consola.Styles[Style.LineNumber].ForeColor = IntToColor(0xB7B7B7);
            consola.Styles[Style.IndentGuide].ForeColor = IntToColor(0xB7B7B7);
            consola.Styles[Style.IndentGuide].BackColor = IntToColor(0x2A211C);
           
            var nums = consola.Margins[1];
            nums.Width = 30;
            nums.Type = MarginType.Number;
            nums.Sensitive = true;
            nums.Mask = 0;

            //this.MarginClick += TextArea_MarginClick;
        }

        private static void InitSyntaxColoring(Scintilla consola)
        {
            // Configure the default style
            consola.StyleResetDefault();
            consola.Styles[Style.Default].Font = "Consolas";
            consola.Styles[Style.Default].Size = 10;
            consola.Styles[Style.Default].BackColor = IntToColor(0x252526);
            consola.Styles[Style.Default].ForeColor = IntToColor(0xFFFFFF);

            consola.StyleClearAll();

            // Configure the CPP (C#) lexer styles
            consola.Styles[Style.Cpp.Identifier].ForeColor = IntToColor(0xD0DAE2);
            consola.Styles[Style.Cpp.Comment].ForeColor = IntToColor(0xBD758B);
            consola.Styles[Style.Cpp.CommentLine].ForeColor = IntToColor(0x40BF57);
            consola.Styles[Style.Cpp.CommentDoc].ForeColor = IntToColor(0x2FAE35);
            consola.Styles[Style.Cpp.Number].ForeColor = IntToColor(0xFFFF00);
            consola.Styles[Style.Cpp.String].ForeColor = IntToColor(0xFFFF00);
            consola.Styles[Style.Cpp.Character].ForeColor = IntToColor(0xE95454);
            consola.Styles[Style.Cpp.Preprocessor].ForeColor = IntToColor(0x8AAFEE);
            consola.Styles[Style.Cpp.Operator].ForeColor = IntToColor(0xE0E0E0);
            consola.Styles[Style.Cpp.Regex].ForeColor = IntToColor(0xff00ff);
            consola.Styles[Style.Cpp.CommentLineDoc].ForeColor = IntToColor(0x77A7DB);
            consola.Styles[Style.Cpp.Word].ForeColor = IntToColor(0x48A8EE);
            consola.Styles[Style.Cpp.Word2].ForeColor = IntToColor(0xF98906);
            consola.Styles[Style.Cpp.CommentDocKeyword].ForeColor = IntToColor(0xB3D991);
            consola.Styles[Style.Cpp.CommentDocKeywordError].ForeColor = IntToColor(0xFF0000);
            consola.Styles[Style.Cpp.GlobalClass].ForeColor = IntToColor(0x48A8EE);



            consola.Lexer = Lexer.Cpp;
            consola.SetKeywords(0, " #include void main int float printf  ");
            consola.SetKeywords(1, " #include void main printf goto if Heap Stack  ");
  

        }

        private static Color IntToColor(int rgb)
        {
            return Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }

        private static  void InitColors(Scintilla consola)
        {
            consola.SetSelectionBackColor(true, IntToColor(0x114D9C));
        }

    }
}
