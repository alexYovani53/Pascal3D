
namespace Pascal3D
{
    partial class Optimizar
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.AreaTexto = new ScintillaNET.Scintilla();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnOptimizar = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.SalidaTexto = new ScintillaNET.Scintilla();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tablaErrores = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tablaErrores)).BeginInit();
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
            // panel1
            // 
            this.panel1.Controls.Add(this.btnOptimizar);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(175, 198);
            this.panel1.TabIndex = 2;
            // 
            // btnOptimizar
            // 
            this.btnOptimizar.Location = new System.Drawing.Point(30, 71);
            this.btnOptimizar.Name = "btnOptimizar";
            this.btnOptimizar.Size = new System.Drawing.Size(121, 44);
            this.btnOptimizar.TabIndex = 0;
            this.btnOptimizar.Text = "Optimizar";
            this.btnOptimizar.UseVisualStyleBackColor = true;
            this.btnOptimizar.Click += new System.EventHandler(this.btnOptimizar_Click);
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
            this.splitContainer2.Panel2.Controls.Add(this.panel2);
            this.splitContainer2.Panel2.Controls.Add(this.panel1);
            this.splitContainer2.Size = new System.Drawing.Size(1197, 627);
            this.splitContainer2.SplitterDistance = 425;
            this.splitContainer2.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tabControl1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(175, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1022, 198);
            this.panel2.TabIndex = 4;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1022, 198);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Desktop;
            this.tabPage1.Controls.Add(this.tablaErrores);
            this.tabPage1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1014, 170);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "REGISTROS OPTIMIZACION";
            // 
            // tablaErrores
            // 
            this.tablaErrores.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.tablaErrores.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tablaErrores.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablaErrores.Location = new System.Drawing.Point(3, 3);
            this.tablaErrores.Name = "tablaErrores";
            this.tablaErrores.RowHeadersVisible = false;
            this.tablaErrores.RowTemplate.Height = 25;
            this.tablaErrores.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.tablaErrores.Size = new System.Drawing.Size(1008, 164);
            this.tablaErrores.TabIndex = 0;
            // 
            // Optimizar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1197, 627);
            this.Controls.Add(this.splitContainer2);
            this.Name = "Optimizar";
            this.Text = "Optimizar";
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tablaErrores)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ScintillaNET.Scintilla AreaTexto;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOptimizar;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private ScintillaNET.Scintilla SalidaTexto;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView tablaErrores;
    }
}