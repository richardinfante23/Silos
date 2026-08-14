namespace principal
{
    partial class MovimientoArticulosForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MovimientoArticulosForm));
            this.lbcantidadtotal = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.txtObservacion = new System.Windows.Forms.TextBox();
            this.txtCodigo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.lbprovcli = new System.Windows.Forms.Label();
            this.cbProvCli = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnBuscarConductor = new System.Windows.Forms.Button();
            this.txtNombreConductor = new System.Windows.Forms.TextBox();
            this.txtConductor = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this.cbArticulo = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.txtCantidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtPRecio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cbEstante = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.txtestante = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cbSeccion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtExistencia = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.filanueva = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_seccion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.eliminado = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.btnAgregarDetalle = new System.Windows.Forms.Button();
            this.btnQuitarDetalle = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.btnExportar = new System.Windows.Forms.Button();
            this.btnImprimir = new System.Windows.Forms.Button();
            this.btnAnular = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnAgregarSilos = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            this.dgvMovimientos = new System.Windows.Forms.DataGridView();
            this.lbfechahasta = new System.Windows.Forms.Label();
            this.dtpfechahasta = new System.Windows.Forms.DateTimePicker();
            this.lbanulado = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMovimientos)).BeginInit();
            this.SuspendLayout();
            // 
            // lbcantidadtotal
            // 
            this.lbcantidadtotal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbcantidadtotal.AutoSize = true;
            this.lbcantidadtotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbcantidadtotal.Location = new System.Drawing.Point(1060, 177);
            this.lbcantidadtotal.Name = "lbcantidadtotal";
            this.lbcantidadtotal.Size = new System.Drawing.Size(0, 25);
            this.lbcantidadtotal.TabIndex = 81;
            this.lbcantidadtotal.Visible = false;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(542, 243);
            this.label27.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(103, 17);
            this.label27.TabIndex = 80;
            this.label27.Text = "Observaciones";
            // 
            // txtObservacion
            // 
            this.txtObservacion.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtObservacion.Location = new System.Drawing.Point(653, 231);
            this.txtObservacion.Margin = new System.Windows.Forms.Padding(4);
            this.txtObservacion.Multiline = true;
            this.txtObservacion.Name = "txtObservacion";
            this.txtObservacion.Size = new System.Drawing.Size(577, 63);
            this.txtObservacion.TabIndex = 79;
            // 
            // txtCodigo
            // 
            this.txtCodigo.Enabled = false;
            this.txtCodigo.Location = new System.Drawing.Point(114, 166);
            this.txtCodigo.Margin = new System.Windows.Forms.Padding(4);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.Size = new System.Drawing.Size(95, 22);
            this.txtCodigo.TabIndex = 75;
            this.txtCodigo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.soloNumeros_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(55, 273);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 17);
            this.label2.TabIndex = 78;
            this.label2.Text = "Fecha:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(53, 166);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 17);
            this.label1.TabIndex = 76;
            this.label1.Text = "Código:";
            // 
            // dtpFecha
            // 
            this.dtpFecha.Location = new System.Drawing.Point(114, 268);
            this.dtpFecha.Margin = new System.Windows.Forms.Padding(4);
            this.dtpFecha.Name = "dtpFecha";
            this.dtpFecha.Size = new System.Drawing.Size(287, 22);
            this.dtpFecha.TabIndex = 77;
            this.dtpFecha.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpFecha_KeyDown);
            this.dtpFecha.MouseCaptureChanged += new System.EventHandler(this.dtpFecha_DropDown);
            // 
            // lbprovcli
            // 
            this.lbprovcli.AutoSize = true;
            this.lbprovcli.Location = new System.Drawing.Point(573, 184);
            this.lbprovcli.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbprovcli.Name = "lbprovcli";
            this.lbprovcli.Size = new System.Drawing.Size(57, 17);
            this.lbprovcli.TabIndex = 83;
            this.lbprovcli.Text = "Usuario";
            this.lbprovcli.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbProvCli
            // 
            this.cbProvCli.AccessibleDescription = "validar";
            this.cbProvCli.FormattingEnabled = true;
            this.cbProvCli.Location = new System.Drawing.Point(653, 181);
            this.cbProvCli.Margin = new System.Windows.Forms.Padding(4);
            this.cbProvCli.Name = "cbProvCli";
            this.cbProvCli.Size = new System.Drawing.Size(423, 24);
            this.cbProvCli.TabIndex = 82;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(47, 241);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 17);
            this.label3.TabIndex = 88;
            this.label3.Text = "Nombre:";
            // 
            // btnBuscarConductor
            // 
            this.btnBuscarConductor.Location = new System.Drawing.Point(309, 196);
            this.btnBuscarConductor.Margin = new System.Windows.Forms.Padding(4);
            this.btnBuscarConductor.Name = "btnBuscarConductor";
            this.btnBuscarConductor.Size = new System.Drawing.Size(99, 28);
            this.btnBuscarConductor.TabIndex = 85;
            this.btnBuscarConductor.Text = "Buscar";
            this.btnBuscarConductor.UseVisualStyleBackColor = true;
            this.btnBuscarConductor.Click += new System.EventHandler(this.btnBuscarConductor_Click);
            // 
            // txtNombreConductor
            // 
            this.txtNombreConductor.Location = new System.Drawing.Point(114, 238);
            this.txtNombreConductor.Margin = new System.Windows.Forms.Padding(4);
            this.txtNombreConductor.Name = "txtNombreConductor";
            this.txtNombreConductor.Size = new System.Drawing.Size(287, 22);
            this.txtNombreConductor.TabIndex = 86;
            // 
            // txtConductor
            // 
            this.txtConductor.AccessibleDescription = "validarconductor";
            this.txtConductor.Location = new System.Drawing.Point(114, 202);
            this.txtConductor.Margin = new System.Windows.Forms.Padding(4);
            this.txtConductor.MaxLength = 10;
            this.txtConductor.Name = "txtConductor";
            this.txtConductor.Size = new System.Drawing.Size(180, 22);
            this.txtConductor.TabIndex = 84;
            this.txtConductor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtConductor_KeyDown);
            this.txtConductor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.soloNumeros_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 202);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 17);
            this.label4.TabIndex = 87;
            this.label4.Text = "C.I Conductor:";
            // 
            // dgvDetalle
            // 
            this.dgvDetalle.AllowUserToAddRows = false;
            this.dgvDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDetalle.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDetalle.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetalle.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cbArticulo,
            this.txtCantidad,
            this.txtPRecio,
            this.cbEstante,
            this.txtestante,
            this.cbSeccion,
            this.txtExistencia,
            this.txtTotal,
            this.filanueva,
            this.id,
            this.id_seccion,
            this.eliminado});
            this.dgvDetalle.Location = new System.Drawing.Point(-5, 360);
            this.dgvDetalle.Name = "dgvDetalle";
            this.dgvDetalle.RowTemplate.Height = 24;
            this.dgvDetalle.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDetalle.Size = new System.Drawing.Size(1278, 302);
            this.dgvDetalle.TabIndex = 89;
            this.dgvDetalle.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvDetalle_CellBeginEdit);
            this.dgvDetalle.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDetalle_CellEndEdit);
            this.dgvDetalle.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvDetalle_CellMouseClick);
            this.dgvDetalle.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvDetalle_EditingControlShowing);
            this.dgvDetalle.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgvUserDetails_RowPostPaint);
            this.dgvDetalle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvDetalle_KeyDown);
            // 
            // cbArticulo
            // 
            this.cbArticulo.FillWeight = 245.7817F;
            this.cbArticulo.HeaderText = "Atículo";
            this.cbArticulo.Name = "cbArticulo";
            this.cbArticulo.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.cbArticulo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // txtCantidad
            // 
            this.txtCantidad.FillWeight = 96.2437F;
            this.txtCantidad.HeaderText = "Cantidad";
            this.txtCantidad.Name = "txtCantidad";
            // 
            // txtPRecio
            // 
            this.txtPRecio.FillWeight = 96.2437F;
            this.txtPRecio.HeaderText = "Precio";
            this.txtPRecio.Name = "txtPRecio";
            // 
            // cbEstante
            // 
            this.cbEstante.FillWeight = 96.2437F;
            this.cbEstante.HeaderText = "Estante";
            this.cbEstante.Name = "cbEstante";
            this.cbEstante.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.cbEstante.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // txtestante
            // 
            this.txtestante.HeaderText = "Estante";
            this.txtestante.Name = "txtestante";
            this.txtestante.ReadOnly = true;
            this.txtestante.Visible = false;
            // 
            // cbSeccion
            // 
            this.cbSeccion.FillWeight = 96.2437F;
            this.cbSeccion.HeaderText = "Sección";
            this.cbSeccion.Name = "cbSeccion";
            this.cbSeccion.ReadOnly = true;
            this.cbSeccion.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // txtExistencia
            // 
            this.txtExistencia.HeaderText = "Existencia";
            this.txtExistencia.Name = "txtExistencia";
            this.txtExistencia.Visible = false;
            // 
            // txtTotal
            // 
            this.txtTotal.FillWeight = 96.2437F;
            this.txtTotal.HeaderText = "Total";
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.ReadOnly = true;
            // 
            // filanueva
            // 
            this.filanueva.HeaderText = "nuevo";
            this.filanueva.Name = "filanueva";
            this.filanueva.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.filanueva.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.filanueva.Visible = false;
            // 
            // id
            // 
            this.id.HeaderText = "id";
            this.id.Name = "id";
            this.id.Visible = false;
            // 
            // id_seccion
            // 
            this.id_seccion.HeaderText = "seccion";
            this.id_seccion.Name = "id_seccion";
            this.id_seccion.Visible = false;
            // 
            // eliminado
            // 
            this.eliminado.HeaderText = "eliminado";
            this.eliminado.Name = "eliminado";
            this.eliminado.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.eliminado.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.eliminado.Visible = false;
            // 
            // btnAgregarDetalle
            // 
            this.btnAgregarDetalle.Enabled = false;
            this.btnAgregarDetalle.Location = new System.Drawing.Point(576, 325);
            this.btnAgregarDetalle.Margin = new System.Windows.Forms.Padding(4);
            this.btnAgregarDetalle.Name = "btnAgregarDetalle";
            this.btnAgregarDetalle.Size = new System.Drawing.Size(99, 28);
            this.btnAgregarDetalle.TabIndex = 90;
            this.btnAgregarDetalle.Text = "Agregar";
            this.btnAgregarDetalle.UseVisualStyleBackColor = true;
            this.btnAgregarDetalle.Click += new System.EventHandler(this.btnAgregarDetalle_Click);
            // 
            // btnQuitarDetalle
            // 
            this.btnQuitarDetalle.Enabled = false;
            this.btnQuitarDetalle.Location = new System.Drawing.Point(706, 325);
            this.btnQuitarDetalle.Margin = new System.Windows.Forms.Padding(4);
            this.btnQuitarDetalle.Name = "btnQuitarDetalle";
            this.btnQuitarDetalle.Size = new System.Drawing.Size(99, 28);
            this.btnQuitarDetalle.TabIndex = 90;
            this.btnQuitarDetalle.Text = "Quitar";
            this.btnQuitarDetalle.UseVisualStyleBackColor = true;
            this.btnQuitarDetalle.Click += new System.EventHandler(this.btnQuitarDetalle_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(437, 328);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(90, 25);
            this.label5.TabIndex = 91;
            this.label5.Text = "Detalles";
            // 
            // btnExportar
            // 
            this.btnExportar.Enabled = false;
            this.btnExportar.Image = ((System.Drawing.Image)(resources.GetObject("btnExportar.Image")));
            this.btnExportar.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnExportar.Location = new System.Drawing.Point(684, 671);
            this.btnExportar.Margin = new System.Windows.Forms.Padding(4);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(100, 63);
            this.btnExportar.TabIndex = 96;
            this.btnExportar.Tag = "";
            this.btnExportar.Text = "Exportar";
            this.btnExportar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnExportar.UseVisualStyleBackColor = true;
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);
            // 
            // btnImprimir
            // 
            this.btnImprimir.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnImprimir.Image = ((System.Drawing.Image)(resources.GetObject("btnImprimir.Image")));
            this.btnImprimir.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnImprimir.Location = new System.Drawing.Point(523, 670);
            this.btnImprimir.Margin = new System.Windows.Forms.Padding(4);
            this.btnImprimir.Name = "btnImprimir";
            this.btnImprimir.Size = new System.Drawing.Size(100, 63);
            this.btnImprimir.TabIndex = 95;
            this.btnImprimir.Text = "Imprimir ";
            this.btnImprimir.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnImprimir.UseVisualStyleBackColor = true;
            this.btnImprimir.Visible = false;
            this.btnImprimir.Click += new System.EventHandler(this.btnImprimir_Click);
            // 
            // btnAnular
            // 
            this.btnAnular.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnAnular.Image = ((System.Drawing.Image)(resources.GetObject("btnAnular.Image")));
            this.btnAnular.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAnular.Location = new System.Drawing.Point(362, 670);
            this.btnAnular.Margin = new System.Windows.Forms.Padding(4);
            this.btnAnular.Name = "btnAnular";
            this.btnAnular.Size = new System.Drawing.Size(100, 63);
            this.btnAnular.TabIndex = 94;
            this.btnAnular.Text = "Anular";
            this.btnAnular.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnAnular.UseVisualStyleBackColor = true;
            this.btnAnular.Visible = false;
            this.btnAnular.Click += new System.EventHandler(this.btnAnular_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.Image = ((System.Drawing.Image)(resources.GetObject("btnCancelar.Image")));
            this.btnCancelar.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCancelar.Location = new System.Drawing.Point(198, 669);
            this.btnCancelar.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(103, 64);
            this.btnCancelar.TabIndex = 93;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardar.Image = ((System.Drawing.Image)(resources.GetObject("btnGuardar.Image")));
            this.btnGuardar.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnGuardar.Location = new System.Drawing.Point(32, 671);
            this.btnGuardar.Margin = new System.Windows.Forms.Padding(4);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(105, 63);
            this.btnGuardar.TabIndex = 92;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnAgregarSilos
            // 
            this.btnAgregarSilos.Image = ((System.Drawing.Image)(resources.GetObject("btnAgregarSilos.Image")));
            this.btnAgregarSilos.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAgregarSilos.Location = new System.Drawing.Point(1084, 168);
            this.btnAgregarSilos.Margin = new System.Windows.Forms.Padding(4);
            this.btnAgregarSilos.Name = "btnAgregarSilos";
            this.btnAgregarSilos.Size = new System.Drawing.Size(69, 55);
            this.btnAgregarSilos.TabIndex = 97;
            this.btnAgregarSilos.Text = "Agregar";
            this.btnAgregarSilos.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnAgregarSilos.UseVisualStyleBackColor = true;
            this.btnAgregarSilos.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            // btnEditar
            // 
            this.btnEditar.Image = ((System.Drawing.Image)(resources.GetObject("btnEditar.Image")));
            this.btnEditar.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnEditar.Location = new System.Drawing.Point(1161, 166);
            this.btnEditar.Margin = new System.Windows.Forms.Padding(4);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(69, 57);
            this.btnEditar.TabIndex = 98;
            this.btnEditar.Text = "Editar";
            this.btnEditar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnEditar.UseVisualStyleBackColor = true;
            this.btnEditar.Click += new System.EventHandler(this.btnEditarSilos_Click);
            // 
            // dgvMovimientos
            // 
            this.dgvMovimientos.AllowUserToAddRows = false;
            this.dgvMovimientos.AllowUserToDeleteRows = false;
            this.dgvMovimientos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvMovimientos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMovimientos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMovimientos.Location = new System.Drawing.Point(1, -1);
            this.dgvMovimientos.Name = "dgvMovimientos";
            this.dgvMovimientos.ReadOnly = true;
            this.dgvMovimientos.RowTemplate.Height = 24;
            this.dgvMovimientos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMovimientos.Size = new System.Drawing.Size(1283, 150);
            this.dgvMovimientos.TabIndex = 99;
            this.dgvMovimientos.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgvUserDetails_RowPostPaint);
            this.dgvMovimientos.DoubleClick += new System.EventHandler(this.dgvMovimientos_CellDoubleClick);
            // 
            // lbfechahasta
            // 
            this.lbfechahasta.AutoSize = true;
            this.lbfechahasta.Location = new System.Drawing.Point(17, 303);
            this.lbfechahasta.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbfechahasta.Name = "lbfechahasta";
            this.lbfechahasta.Size = new System.Drawing.Size(92, 17);
            this.lbfechahasta.TabIndex = 101;
            this.lbfechahasta.Text = "Fecha Hasta:";
            // 
            // dtpfechahasta
            // 
            this.dtpfechahasta.Location = new System.Drawing.Point(114, 298);
            this.dtpfechahasta.Margin = new System.Windows.Forms.Padding(4);
            this.dtpfechahasta.Name = "dtpfechahasta";
            this.dtpfechahasta.Size = new System.Drawing.Size(287, 22);
            this.dtpfechahasta.TabIndex = 100;
            this.dtpfechahasta.DropDown += new System.EventHandler(this.dtpFecha_DropDown);
            this.dtpfechahasta.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpFecha_KeyDown);
            // 
            // lbanulado
            // 
            this.lbanulado.AutoSize = true;
            this.lbanulado.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbanulado.Location = new System.Drawing.Point(301, 153);
            this.lbanulado.Name = "lbanulado";
            this.lbanulado.Size = new System.Drawing.Size(0, 24);
            this.lbanulado.TabIndex = 102;
            // 
            // MovimientoArticulosForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1285, 747);
            this.Controls.Add(this.lbanulado);
            this.Controls.Add(this.lbfechahasta);
            this.Controls.Add(this.dtpfechahasta);
            this.Controls.Add(this.dgvMovimientos);
            this.Controls.Add(this.btnAgregarSilos);
            this.Controls.Add(this.btnEditar);
            this.Controls.Add(this.btnExportar);
            this.Controls.Add(this.btnImprimir);
            this.Controls.Add(this.btnAnular);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnQuitarDetalle);
            this.Controls.Add(this.btnAgregarDetalle);
            this.Controls.Add(this.dgvDetalle);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnBuscarConductor);
            this.Controls.Add(this.txtNombreConductor);
            this.Controls.Add(this.txtConductor);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lbprovcli);
            this.Controls.Add(this.cbProvCli);
            this.Controls.Add(this.lbcantidadtotal);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.txtObservacion);
            this.Controls.Add(this.txtCodigo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dtpFecha);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MovimientoArticulosForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.MovimientoArticulosForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMovimientos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbcantidadtotal;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.TextBox txtObservacion;
        private System.Windows.Forms.TextBox txtCodigo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.Label lbprovcli;
        private System.Windows.Forms.ComboBox cbProvCli;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnBuscarConductor;
        private System.Windows.Forms.TextBox txtNombreConductor;
        private System.Windows.Forms.TextBox txtConductor;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView dgvDetalle;
        private System.Windows.Forms.Button btnAgregarDetalle;
        private System.Windows.Forms.Button btnQuitarDetalle;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.Button btnImprimir;
        private System.Windows.Forms.Button btnAnular;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnAgregarSilos;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.DataGridView dgvMovimientos;
        private System.Windows.Forms.Label lbfechahasta;
        private System.Windows.Forms.DateTimePicker dtpfechahasta;
        private System.Windows.Forms.Label lbanulado;
        private System.Windows.Forms.DataGridViewComboBoxColumn cbArticulo;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtCantidad;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtPRecio;
        private System.Windows.Forms.DataGridViewComboBoxColumn cbEstante;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtestante;
        private System.Windows.Forms.DataGridViewTextBoxColumn cbSeccion;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtExistencia;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtTotal;
        private System.Windows.Forms.DataGridViewCheckBoxColumn filanueva;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn id_seccion;
        private System.Windows.Forms.DataGridViewCheckBoxColumn eliminado;
    }
}