
namespace Pascal3D
{
    partial class Pascal
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.AreaTexto = new ScintillaNET.Scintilla();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.SalidaTexto = new ScintillaNET.Scintilla();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.traducir = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // AreaTexto
            // 
            this.AreaTexto.AutoCMaxHeight = 9;
            this.AreaTexto.CaretForeColor = System.Drawing.Color.White;
            this.AreaTexto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AreaTexto.Location = new System.Drawing.Point(0, 0);
            this.AreaTexto.Name = "AreaTexto";
            this.AreaTexto.Size = new System.Drawing.Size(743, 425);
            this.AreaTexto.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.AreaTexto);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AllowDrop = true;
            this.splitContainer1.Panel2.Controls.Add(this.SalidaTexto);
            this.splitContainer1.Size = new System.Drawing.Size(1197, 425);
            this.splitContainer1.SplitterDistance = 743;
            this.splitContainer1.TabIndex = 1;
            // 
            // SalidaTexto
            // 
            this.SalidaTexto.AutoCMaxHeight = 9;
            this.SalidaTexto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SalidaTexto.Location = new System.Drawing.Point(0, 0);
            this.SalidaTexto.Name = "SalidaTexto";
            this.SalidaTexto.Size = new System.Drawing.Size(450, 425);
            this.SalidaTexto.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.traducir);
            this.splitContainer2.Panel2.Controls.Add(this.button1);
            this.splitContainer2.Size = new System.Drawing.Size(1197, 627);
            this.splitContainer2.SplitterDistance = 425;
            this.splitContainer2.TabIndex = 2;
            // 
            // traducir
            // 
            this.traducir.Location = new System.Drawing.Point(63, 121);
            this.traducir.Name = "traducir";
            this.traducir.Size = new System.Drawing.Size(75, 23);
            this.traducir.TabIndex = 1;
            this.traducir.Text = "Traducir";
            this.traducir.UseVisualStyleBackColor = true;
            this.traducir.Click += new System.EventHandler(this.traducir_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(63, 40);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(83, 26);
            this.button1.TabIndex = 0;
            this.button1.Text = "Abrir";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Pascal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1197, 627);
            this.Controls.Add(this.splitContainer2);
            this.Name = "Pascal";
            this.Text = "Pascal  - 3D";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ScintillaNET.Scintilla AreaTexto;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private ScintillaNET.Scintilla SalidaTexto;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button traducir;
    }
}

