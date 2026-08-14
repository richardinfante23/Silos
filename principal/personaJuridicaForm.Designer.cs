namespace principal
{
    partial class personaJuridicaForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(personaJuridicaForm));
            this.s = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbAsociaciones = new System.Windows.Forms.ComboBox();
            this.cbLetra = new System.Windows.Forms.ComboBox();
            this.txtRif = new System.Windows.Forms.TextBox();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnEditarAsociacion = new System.Windows.Forms.Button();
            this.btnAgregarAsociacion = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.lbNombre = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // s
            // 
            this.s.AutoSize = true;
            this.s.Location = new System.Drawing.Point(-39, 116);
            this.s.Name = "s";
            this.s.Size = new System.Drawing.Size(43, 13);
            this.s.TabIndex = 15;
            this.s.Text = "Cédula:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(-68, 142);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Asociaciones:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(-30, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Letra:";
            // 
            // cbAsociaciones
            // 
            this.cbAsociaciones.AccessibleDescription = "validar";
            this.cbAsociaciones.FormattingEnabled = true;
            this.cbAsociaciones.Location = new System.Drawing.Point(98, 97);
            this.cbAsociaciones.Name = "cbAsociaciones";
            this.cbAsociaciones.Size = new System.Drawing.Size(253, 21);
            this.cbAsociaciones.TabIndex = 4;
            // 
            // cbLetra
            // 
            this.cbLetra.AccessibleDescription = "validar";
            this.cbLetra.FormattingEnabled = true;
            this.cbLetra.Location = new System.Drawing.Point(98, 67);
            this.cbLetra.Name = "cbLetra";
            this.cbLetra.Size = new System.Drawing.Size(53, 21);
            this.cbLetra.TabIndex = 2;
            // 
            // txtRif
            // 
            this.txtRif.AccessibleDescription = "validar";
            this.txtRif.Location = new System.Drawing.Point(190, 67);
            this.txtRif.MaxLength = 10;
            this.txtRif.Name = "txtRif";
            this.txtRif.Size = new System.Drawing.Size(161, 20);
            this.txtRif.TabIndex = 3;
            // 
            // txtNombre
            // 
            this.txtNombre.AccessibleDescription = "validar";
            this.txtNombre.Location = new System.Drawing.Point(98, 41);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(253, 20);
            this.txtNombre.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-43, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Nombre:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(157, 71);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "RIF:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 97);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Asociaciones:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(58, 70);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 21;
            this.label6.Text = "Letra:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(45, 44);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Nombre:";
            // 
            // btnEliminar
            // 
            this.btnEliminar.Image = ((System.Drawing.Image)(resources.GetObject("btnEliminar.Image")));
            this.btnEliminar.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnEliminar.Location = new System.Drawing.Point(160, 124);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(60, 48);
            this.btnEliminar.TabIndex = 9;
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
            this.btnCancelar.Location = new System.Drawing.Point(226, 124);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(60, 48);
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
            this.btnEditarAsociacion.Location = new System.Drawing.Point(417, 85);
            this.btnEditarAsociacion.Name = "btnEditarAsociacion";
            this.btnEditarAsociacion.Size = new System.Drawing.Size(47, 48);
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
            this.btnAgregarAsociacion.Location = new System.Drawing.Point(357, 85);
            this.btnAgregarAsociacion.Name = "btnAgregarAsociacion";
            this.btnAgregarAsociacion.Size = new System.Drawing.Size(54, 48);
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
            this.btnGuardar.Location = new System.Drawing.Point(98, 124);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(53, 48);
            this.btnGuardar.TabIndex = 8;
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
            this.lbNombre.Location = new System.Drawing.Point(65, 9);
            this.lbNombre.Name = "lbNombre";
            this.lbNombre.Size = new System.Drawing.Size(189, 25);
            this.lbNombre.TabIndex = 27;
            this.lbNombre.Text = "Persona Jurídica";
            // 
            // personaJuridicaForm
            // 
            this.AcceptButton = this.btnGuardar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(476, 210);
            this.ControlBox = false;
            this.Controls.Add(this.lbNombre);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnEditarAsociacion);
            this.Controls.Add(this.btnAgregarAsociacion);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.s);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbAsociaciones);
            this.Controls.Add(this.cbLetra);
            this.Controls.Add(this.txtRif);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "personaJuridicaForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "  ";
            this.Load += new System.EventHandler(this.personaJuridicaForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label s;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.ComboBox cbAsociaciones;
        public System.Windows.Forms.ComboBox cbLetra;
        public System.Windows.Forms.TextBox txtRif;
        public System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnEditarAsociacion;
        private System.Windows.Forms.Button btnAgregarAsociacion;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Label lbNombre;
    }
}