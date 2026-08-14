namespace principal
{
    partial class productorNatural
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(productorNatural));
            this.label1 = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.cbLetra = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCedula = new System.Windows.Forms.TextBox();
            this.s = new System.Windows.Forms.Label();
            this.cbAsociaciones = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnEditarAsociacion = new System.Windows.Forms.Button();
            this.btnAgregarAsociacion = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.lbNombre = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(44, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Nombre:";
            // 
            // txtNombre
            // 
            this.txtNombre.AccessibleDescription = "validar";
            this.txtNombre.Location = new System.Drawing.Point(94, 43);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(253, 20);
            this.txtNombre.TabIndex = 1;
            // 
            // cbLetra
            // 
            this.cbLetra.AccessibleDescription = "validar";
            this.cbLetra.FormattingEnabled = true;
            this.cbLetra.Location = new System.Drawing.Point(94, 69);
            this.cbLetra.Name = "cbLetra";
            this.cbLetra.Size = new System.Drawing.Size(53, 21);
            this.cbLetra.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(57, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Letra:";
            // 
            // txtCedula
            // 
            this.txtCedula.AccessibleDescription = "validar";
            this.txtCedula.Location = new System.Drawing.Point(94, 96);
            this.txtCedula.MaxLength = 10;
            this.txtCedula.Name = "txtCedula";
            this.txtCedula.Size = new System.Drawing.Size(253, 20);
            this.txtCedula.TabIndex = 3;
            // 
            // s
            // 
            this.s.AutoSize = true;
            this.s.Location = new System.Drawing.Point(48, 99);
            this.s.Name = "s";
            this.s.Size = new System.Drawing.Size(43, 13);
            this.s.TabIndex = 4;
            this.s.Text = "Cédula:";
            // 
            // cbAsociaciones
            // 
            this.cbAsociaciones.AccessibleDescription = "validar";
            this.cbAsociaciones.FormattingEnabled = true;
            this.cbAsociaciones.Location = new System.Drawing.Point(94, 122);
            this.cbAsociaciones.Name = "cbAsociaciones";
            this.cbAsociaciones.Size = new System.Drawing.Size(253, 21);
            this.cbAsociaciones.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 125);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Asociaciones:";
            // 
            // btnEliminar
            // 
            this.btnEliminar.Image = ((System.Drawing.Image)(resources.GetObject("btnEliminar.Image")));
            this.btnEliminar.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnEliminar.Location = new System.Drawing.Point(183, 147);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(75, 53);
            this.btnEliminar.TabIndex = 8;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnEliminar.UseVisualStyleBackColor = true;
            this.btnEliminar.Visible = false;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Image = ((System.Drawing.Image)(resources.GetObject("btnCancelar.Image")));
            this.btnCancelar.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCancelar.Location = new System.Drawing.Point(275, 147);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(75, 53);
            this.btnCancelar.TabIndex = 9;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnEditarAsociacion
            // 
            this.btnEditarAsociacion.Image = ((System.Drawing.Image)(resources.GetObject("btnEditarAsociacion.Image")));
            this.btnEditarAsociacion.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnEditarAsociacion.Location = new System.Drawing.Point(414, 111);
            this.btnEditarAsociacion.Name = "btnEditarAsociacion";
            this.btnEditarAsociacion.Size = new System.Drawing.Size(49, 40);
            this.btnEditarAsociacion.TabIndex = 6;
            this.btnEditarAsociacion.Text = "Editar";
            this.btnEditarAsociacion.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnEditarAsociacion.UseVisualStyleBackColor = true;
            this.btnEditarAsociacion.Click += new System.EventHandler(this.btnEditarAsociacion_Click);
            // 
            // btnAgregarAsociacion
            // 
            this.btnAgregarAsociacion.Image = ((System.Drawing.Image)(resources.GetObject("btnAgregarAsociacion.Image")));
            this.btnAgregarAsociacion.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAgregarAsociacion.Location = new System.Drawing.Point(356, 111);
            this.btnAgregarAsociacion.Name = "btnAgregarAsociacion";
            this.btnAgregarAsociacion.Size = new System.Drawing.Size(52, 40);
            this.btnAgregarAsociacion.TabIndex = 5;
            this.btnAgregarAsociacion.Text = "Agregar";
            this.btnAgregarAsociacion.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnAgregarAsociacion.UseVisualStyleBackColor = true;
            this.btnAgregarAsociacion.Click += new System.EventHandler(this.btnAgregarAsociacion_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Image = ((System.Drawing.Image)(resources.GetObject("btnGuardar.Image")));
            this.btnGuardar.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnGuardar.Location = new System.Drawing.Point(95, 147);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(75, 53);
            this.btnGuardar.TabIndex = 7;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // lbNombre
            // 
            this.lbNombre.AutoSize = true;
            this.lbNombre.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lbNombre.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbNombre.Location = new System.Drawing.Point(68, 9);
            this.lbNombre.Name = "lbNombre";
            this.lbNombre.Size = new System.Drawing.Size(197, 25);
            this.lbNombre.TabIndex = 10;
            this.lbNombre.Text = "Productor Natural";
            // 
            // productorNatural
            // 
            this.AcceptButton = this.btnGuardar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(525, 205);
            this.ControlBox = false;
            this.Controls.Add(this.lbNombre);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnEditarAsociacion);
            this.Controls.Add(this.btnAgregarAsociacion);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.s);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbAsociaciones);
            this.Controls.Add(this.cbLetra);
            this.Controls.Add(this.txtCedula);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "productorNatural";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "  ";
            this.Load += new System.EventHandler(this.productorNatural_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label s;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox txtNombre;
        public System.Windows.Forms.ComboBox cbLetra;
        public System.Windows.Forms.TextBox txtCedula;
        public System.Windows.Forms.ComboBox cbAsociaciones;
        public System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnEditarAsociacion;
        private System.Windows.Forms.Button btnAgregarAsociacion;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Label lbNombre;
    }
}