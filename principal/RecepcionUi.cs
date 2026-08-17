using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace principal
{
    public partial class recepcionForm
    {
        private bool _uiModernizada;
        private Panel _panelMetaRecepcion;
        private FlowLayoutPanel _panelAccionesRecepcion;
        private TabPage _tpCaptura;
        private TabPage _tpListado;
        private TabControl _tcPasosCaptura;
        private TableLayoutPanel _hostDatosRecepcion;
        private TableLayoutPanel _hostListado;
        private Panel _panelGrillaRecepcion;
        private Panel _panelFormularioRecepcion;
        private Panel _panelFiltrosListado;
        private Button _btnBuscarListado;
        private GridHeaderFilter _filtroRecepciones;
        private bool _modoListado;

        // Filtros dedicados del modo Listado (no reutilizan el formulario de captura).
        private TextBox _txtFiltroTicket;
        private DateTimePicker _dtpFiltroDesde;
        private DateTimePicker _dtpFiltroHasta;
        private TextBox _txtFiltroConductor;
        private ComboBox _cbFiltroLetraCliente;
        private TextBox _txtFiltroCliente;
        private ComboBox _cbFiltroProducto;
        private ComboBox _cbFiltroEstatus;
        private const float AlturaFiltrosListado = 156F;
        private const int AlturaCardInicio = 182;
        private const int AlturaCardGuia = 182;
        private const int AlturaCardMeta = 140;
        private const int AlturaFilaCampo = 36;
        private const int AlturaFilaFiltro = 34;
        private const int AlturaControlCampo = 30;

        private void AplicarUiRecepcionModerna()
        {
            if (_uiModernizada) return;
            _uiModernizada = true;

            this.Text = "Recepción";
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.MinimumSize = new Size(1100, 720);
            this.BackColor = UiTheme.Surface;
            this.Font = UiTheme.FontRegular;
            this.DoubleBuffered = true;

            tcRecepcion.Dock = DockStyle.Fill;
            tcRecepcion.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            tcRecepcion.Padding = new Point(12, 6);
            label1.Text = "Nro ticket (auto)";
            label2.Text = "Fecha";
            label5.Text = "Producto";
            label3.Text = "Cédula conductor";
            label27.Text = "Nombre conductor";
            label4.Text = "Placa";
            label6.Text = "Datos iniciales";
            label7.Text = "Datos guía";
            label8.Text = "Nro guía";
            label9.Text = "Peso guía (Kg)";
            label11.Text = "Estatus";
            label23.Text = "Silo";
            label24.Text = "Observaciones";
            label28.Text = "Análisis:";

            ConstruirJerarquiaPestanas();

            _panelFormularioRecepcion = CrearPanelFormulario();
            _panelFiltrosListado = CrearPanelFiltrosListado();
            _panelGrillaRecepcion = CrearPanelGrilla();
            _panelAccionesRecepcion = CrearPanelAcciones();

            ConstruirLayoutDatosIniciales();
            ConstruirLayoutListado();
            ConstruirLayoutAnalisis();
            ConstruirLayoutRomana();
            EstilizarControlesRecepcion();
            ConfigurarTicketAutogenerado();
            ConfigurarAnalisisSinUso();
            ActualizarPasosRecepcion();
            AplicarModoRecepcion(!nuevo);
        }

        private void ConstruirJerarquiaPestanas()
        {
            tcRecepcion.SuspendLayout();

            if (tcRecepcion.TabPages.Contains(dtpHasta))
                tcRecepcion.TabPages.Remove(dtpHasta);
            if (tcRecepcion.TabPages.Contains(tpAnalisis))
                tcRecepcion.TabPages.Remove(tpAnalisis);
            if (tcRecepcion.TabPages.Contains(tpRomana))
                tcRecepcion.TabPages.Remove(tpRomana);
            tcRecepcion.Controls.Clear();

            _tpCaptura = new TabPage
            {
                Text = "Captura",
                BackColor = UiTheme.Surface,
                Padding = new Padding(4)
            };
            _tpListado = new TabPage
            {
                Text = "Listado",
                BackColor = UiTheme.Surface,
                Padding = new Padding(4)
            };

            _tcPasosCaptura = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Padding = new Point(8, 4)
            };

            dtpHasta.Text = "1. Datos";
            tpAnalisis.Text = "2. Análisis";
            tpRomana.Text = "3. Pesaje";
            dtpHasta.BackColor = UiTheme.Surface;
            tpAnalisis.BackColor = UiTheme.Surface;
            tpRomana.BackColor = UiTheme.Surface;

            _tcPasosCaptura.TabPages.Add(dtpHasta);
            _tcPasosCaptura.TabPages.Add(tpAnalisis);
            _tcPasosCaptura.TabPages.Add(tpRomana);

            _tpCaptura.Controls.Add(_tcPasosCaptura);
            tcRecepcion.Controls.Add(_tpCaptura);
            tcRecepcion.Controls.Add(_tpListado);

            tcRecepcion.SelectedIndexChanged += (s, e) =>
            {
                if (_tpCaptura == null || _tpListado == null) return;
                if (tcRecepcion.SelectedTab == _tpListado && !_modoListado)
                    AplicarModoRecepcion(true);
                else if (tcRecepcion.SelectedTab == _tpCaptura && _modoListado)
                    AplicarModoRecepcion(false);
            };

            tcRecepcion.ResumeLayout(true);
        }

        private void ConstruirLayoutDatosIniciales()
        {
            dtpHasta.SuspendLayout();
            dtpHasta.Controls.Clear();

            _hostDatosRecepcion = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(10, 10, 16, 10),
                BackColor = UiTheme.Surface
            };
            _hostDatosRecepcion.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _hostDatosRecepcion.RowStyles.Add(new RowStyle(SizeType.Absolute, 74F));

            _panelFormularioRecepcion.Dock = DockStyle.Fill;
            _hostDatosRecepcion.Controls.Add(_panelFormularioRecepcion, 0, 0);
            _hostDatosRecepcion.Controls.Add(_panelAccionesRecepcion, 0, 1);

            dtpHasta.Controls.Add(_hostDatosRecepcion);
            dtpHasta.ResumeLayout(true);
        }

        private void ConstruirLayoutListado()
        {
            _tpListado.SuspendLayout();
            _tpListado.Controls.Clear();

            _hostListado = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(10, 10, 16, 10),
                BackColor = UiTheme.Surface
            };
            _hostListado.RowStyles.Add(new RowStyle(SizeType.Absolute, AlturaFiltrosListado));
            _hostListado.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _hostListado.RowStyles.Add(new RowStyle(SizeType.Absolute, 74F));

            _panelFiltrosListado.Dock = DockStyle.Fill;
            _panelFiltrosListado.Visible = true;
            _panelGrillaRecepcion.Dock = DockStyle.Fill;
            _panelGrillaRecepcion.Visible = true;

            _hostListado.Controls.Add(_panelFiltrosListado, 0, 0);
            _hostListado.Controls.Add(_panelGrillaRecepcion, 0, 1);

            _tpListado.Controls.Add(_hostListado);
            _tpListado.ResumeLayout(true);
        }

        public void IrAPasoCapturaDatos()
        {
            AplicarModoRecepcion(false);
            if (_tcPasosCaptura != null && _tcPasosCaptura.TabPages.Contains(dtpHasta))
                _tcPasosCaptura.SelectedTab = dtpHasta;
        }

        private Panel CrearPanelFormulario()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UiTheme.Surface,
                AutoScroll = true,
                Padding = new Padding(0, 0, 4, 14)
            };

            var stack = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = UiTheme.Surface,
                Padding = new Padding(0)
            };
            stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            ReconstruirPanelInicio();
            ReconstruirPanelGuia();
            _panelMetaRecepcion = CrearPanelMeta();

            pInicio.MinimumSize = new Size(0, AlturaCardInicio);
            pInicio.Height = AlturaCardInicio;
            pInicio.Dock = DockStyle.Top;
            pGuia.MinimumSize = new Size(0, AlturaCardGuia);
            pGuia.Height = AlturaCardGuia;
            pGuia.Dock = DockStyle.Top;
            _panelMetaRecepcion.MinimumSize = new Size(0, AlturaCardMeta);
            _panelMetaRecepcion.Height = AlturaCardMeta;
            _panelMetaRecepcion.Dock = DockStyle.Top;
            pInicio.Margin = new Padding(0, 0, 0, 10);
            pGuia.Margin = new Padding(0, 0, 0, 10);

            stack.Controls.Add(pInicio, 0, 0);
            stack.Controls.Add(pGuia, 0, 1);
            stack.Controls.Add(_panelMetaRecepcion, 0, 2);
            panel.Controls.Add(stack);
            return panel;
        }

        public void AplicarModoRecepcion(bool listado)
        {
            _modoListado = listado;
            if (_tpCaptura == null || _tpListado == null) return;

            var destino = listado ? _tpListado : _tpCaptura;
            if (tcRecepcion.SelectedTab != destino)
                tcRecepcion.SelectedTab = destino;

            if (listado)
            {
                PrepararFiltrosListado();
                AnclarPanelAcciones(_hostListado, 2);
            }
            else
            {
                RestaurarFormularioCaptura();
                if (rbtFechaHasta != null) rbtFechaHasta.Visible = false;
                if (dtpFechaHasta != null) dtpFechaHasta.Visible = false;
                AnclarPanelAcciones(_hostDatosRecepcion, 1);
                if (lbTotal != null) lbTotal.Visible = false;
            }

            ActualizarBotonesAccionModo();
        }

        private void ActualizarBotonesAccionModo()
        {
            if (btnCancelar != null) btnCancelar.Visible = true;

            if (_modoListado)
            {
                if (btnGuardar != null) btnGuardar.Visible = false;
                if (btnAnular != null) btnAnular.Visible = false;
                if (btnImprimir != null) btnImprimir.Visible = false;
                if (btnExportar != null) btnExportar.Visible = true;
                return;
            }

            if (btnExportar != null) btnExportar.Visible = false;
            if (btnGuardar != null)
            {
                btnGuardar.Visible = true;
                btnGuardar.Text = "Guardar";
                UiTheme.StylePrimaryButton(btnGuardar);
                btnGuardar.Size = new Size(120, 38);
            }

            bool tieneRegistro = objRecepcion != null && objRecepcion.id > 0;
            if (btnAnular != null)
                btnAnular.Visible = tieneRegistro;
            if (btnImprimir != null)
                btnImprimir.Visible = tieneRegistro
                    && objRecepcion.id_estatus >= 4
                    && objRecepcion.id_estatus <= 6;
        }

        private void AnclarPanelAcciones(TableLayoutPanel host, int fila)
        {
            if (_panelAccionesRecepcion == null || host == null) return;
            if (_panelAccionesRecepcion.Parent == host)
            {
                host.SetCellPosition(_panelAccionesRecepcion, new TableLayoutPanelCellPosition(0, fila));
                return;
            }
            _panelAccionesRecepcion.Parent?.Controls.Remove(_panelAccionesRecepcion);
            host.Controls.Add(_panelAccionesRecepcion, 0, fila);
        }

        private void RestaurarFormularioCaptura()
        {
            if (pInicio == null || pGuia == null || _panelMetaRecepcion == null) return;
            pInicio.MinimumSize = new Size(0, AlturaCardInicio);
            pInicio.Height = AlturaCardInicio;
            pInicio.Visible = true;
            pGuia.Visible = true;
            pGuia.MinimumSize = new Size(0, AlturaCardGuia);
            pGuia.Height = AlturaCardGuia;
            _panelMetaRecepcion.Visible = true;
            _panelMetaRecepcion.MinimumSize = new Size(0, AlturaCardMeta);
            _panelMetaRecepcion.Height = AlturaCardMeta;
            if (label3 != null) label3.Visible = true;
            if (txtConductor != null) txtConductor.Visible = true;
            if (btnBuscarConductor != null) btnBuscarConductor.Visible = true;
            if (label27 != null) label27.Visible = true;
            if (txtNombreConductor != null) txtNombreConductor.Visible = true;
            if (label4 != null) label4.Visible = true;
            if (txtPlaca != null) txtPlaca.Visible = true;
            if (label24 != null) label24.Visible = true;
            if (txtObservaciones != null) txtObservaciones.Visible = true;
            if (btnAgregarSilos != null) btnAgregarSilos.Visible = true;
            if (btnEditarSilos != null) btnEditarSilos.Visible = true;
            ConfigurarTicketAutogenerado();
        }

        public void EstilarBotonCampo(Button btn, string texto)
        {
            if (btn == null) return;
            bool cambiar = string.Equals(texto, "Cambiar", StringComparison.OrdinalIgnoreCase);
            int ancho = cambiar ? 100 : 88;
            if (cambiar)
                UiTheme.StyleCompactPrimaryButton(btn, texto, ancho, 30);
            else
                UiTheme.StyleCompactSecondaryButton(btn, texto, ancho, 30);
        }

        private Panel CrearPanelFiltrosListado()
        {
            // Host con padding externo: el borde del strip nunca queda flush al borde del tab.
            var host = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UiTheme.Surface,
                Padding = new Padding(12, 8, 16, 8),
                Visible = true
            };

            // FilterStrip + borde fino. Padding derecho 14 = aire visible dentro del borde.
            // Importante: NUNCA usar Margin en celdas Percent (Margin queda fuera del % y desborda).
            var strip = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UiTheme.FilterStrip,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(12, 8, 14, 16)
            };

            // Columna fija Absolute para Buscar: no puede clippearse por percent+margin de los filtros.
            var stripLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = UiTheme.FilterStrip,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            stripLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            stripLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 12F)); // gap filtros→Buscar
            stripLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F)); // botón 100px

            var filtersRoot = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = UiTheme.FilterStrip,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            filtersRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            filtersRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            _txtFiltroTicket = new TextBox();
            _dtpFiltroDesde = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = " " };
            _dtpFiltroHasta = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = " " };
            _txtFiltroConductor = new TextBox();
            _cbFiltroLetraCliente = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            _txtFiltroCliente = new TextBox();
            _cbFiltroProducto = new ComboBox { DropDownStyle = ComboBoxStyle.DropDown };
            _cbFiltroEstatus = new ComboBox { DropDownStyle = ComboBoxStyle.DropDown };
            _btnBuscarListado = new Button
            {
                Name = "btnBuscarListado",
                Text = "Buscar",
                Size = new Size(100, 32)
            };

            UiTheme.StyleTextBox(_txtFiltroTicket);
            UiTheme.StyleTextBox(_txtFiltroConductor);
            UiTheme.StyleTextBox(_txtFiltroCliente);
            UiTheme.StyleComboBox(_cbFiltroLetraCliente);
            UiTheme.StyleComboBox(_cbFiltroProducto);
            UiTheme.StyleComboBox(_cbFiltroEstatus);
            UiTheme.StyleDateTimePicker(_dtpFiltroDesde);
            UiTheme.StyleDateTimePicker(_dtpFiltroHasta);

            foreach (Control c in new Control[] {
                _txtFiltroTicket, _dtpFiltroDesde, _dtpFiltroHasta, _txtFiltroConductor,
                _txtFiltroCliente, _cbFiltroProducto, _cbFiltroEstatus, _cbFiltroLetraCliente })
            {
                c.Dock = DockStyle.Fill;
                c.Font = UiTheme.FontRegular;
            }

            _dtpFiltroDesde.ValueChanged += (s, e) =>
            {
                _dtpFiltroDesde.Format = DateTimePickerFormat.Custom;
                _dtpFiltroDesde.CustomFormat = "dd/MM/yyyy";
            };
            _dtpFiltroHasta.ValueChanged += (s, e) =>
            {
                _dtpFiltroHasta.Format = DateTimePickerFormat.Custom;
                _dtpFiltroHasta.CustomFormat = "dd/MM/yyyy";
            };

            KeyEventHandler enterBuscar = (s, e) =>
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    EjecutarBusquedaListado();
                }
            };
            _txtFiltroTicket.KeyDown += enterBuscar;
            _txtFiltroConductor.KeyDown += enterBuscar;
            _txtFiltroCliente.KeyDown += enterBuscar;

            // CompactActionTag: ApplyActionButtons no reaplica icono/min-width ni borra el texto.
            UiTheme.StyleCompactPrimaryButton(_btnBuscarListado, "Buscar", 100, 32);
            _btnBuscarListado.Click += (s, e) => EjecutarBusquedaListado();

            var lblTicket = CrearLabelFiltro("Nro ticket");
            var lblDesde = CrearLabelFiltro("Fecha desde");
            var lblHasta = CrearLabelFiltro("Fecha hasta");
            var lblConductor = CrearLabelFiltro("Conductor (cédula)");
            var lblCliente = CrearLabelFiltro("Cliente / Productor");
            var lblProducto = CrearLabelFiltro("Producto");
            var lblEstatus = CrearLabelFiltro("Estatus");

            // Texto: FixedSingle blanco sobre Surface. Combo/DTP: marco 1px (no tienen BorderStyle fiable).
            Control campoTicket = _txtFiltroTicket;
            Control campoConductor = _txtFiltroConductor;
            Control campoDesde = UiTheme.WrapInputFrame(_dtpFiltroDesde);
            Control campoHasta = UiTheme.WrapInputFrame(_dtpFiltroHasta);
            Control campoProducto = UiTheme.WrapInputFrame(_cbFiltroProducto);
            Control campoEstatus = UiTheme.WrapInputFrame(_cbFiltroEstatus);

            var clienteBox = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0),
                BackColor = UiTheme.FilterStrip
            };
            clienteBox.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 52F));
            clienteBox.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            clienteBox.Controls.Add(UiTheme.WrapInputFrame(_cbFiltroLetraCliente), 0, 0);
            clienteBox.Controls.Add(UiTheme.WrapInputFrame(_txtFiltroCliente), 1, 0);

            // Solo campos de filtro; huecos = Absolute spacers (Margin=0 en todas las celdas).
            var row1 = CrearFilaFiltrosListado(
                new Control[] { lblTicket, campoTicket },
                new Control[] { lblDesde, campoDesde },
                new Control[] { lblHasta, campoHasta },
                new Control[] { lblConductor, campoConductor });

            var row2 = CrearFilaFiltrosListado(
                new Control[] { lblCliente, clienteBox },
                new Control[] { lblProducto, campoProducto },
                new Control[] { lblEstatus, campoEstatus });

            filtersRoot.Controls.Add(row1, 0, 0);
            filtersRoot.Controls.Add(row2, 0, 1);

            // Buscar anclado abajo de la columna fija (alineado con inputs de fila 2).
            var btnHost = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = new Padding(0),
                Padding = new Padding(0),
                BackColor = UiTheme.FilterStrip
            };
            btnHost.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            btnHost.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            _btnBuscarListado.Dock = DockStyle.Fill;
            _btnBuscarListado.Margin = new Padding(0);
            btnHost.Controls.Add(_btnBuscarListado, 0, 1);

            stripLayout.Controls.Add(filtersRoot, 0, 0);
            stripLayout.Controls.Add(btnHost, 2, 0);

            strip.Controls.Add(stripLayout);
            host.Controls.Add(strip);
            return host;
        }

        /// <summary>
        /// Fila de filtros: columnas Percent (suman 100%) intercaladas con Absolute spacers.
        /// Margin de celdas siempre Empty — evita overflow WinForms (Margin fuera del %).
        /// </summary>
        private TableLayoutPanel CrearFilaFiltrosListado(params Control[][] campos)
        {
            if (campos == null || campos.Length == 0)
                throw new ArgumentException("Se requiere al menos un campo de filtro.", nameof(campos));

            int n = campos.Length;
            int colCount = n + (n - 1); // campo | spacer | campo | ...
            var row = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = colCount,
                RowCount = 1,
                BackColor = UiTheme.FilterStrip,
                Margin = new Padding(0, 2, 0, 2),
                Padding = new Padding(0)
            };

            float pct = 100F / n;
            for (int i = 0; i < n; i++)
            {
                if (i > 0)
                    row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 12F));
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, pct));
            }

            int col = 0;
            for (int i = 0; i < n; i++)
            {
                if (i > 0)
                    col++; // saltar columna spacer vacía
                AgregarCeldaFiltroListado(row, col, campos[i]);
                col++;
            }
            return row;
        }

        private void AgregarCeldaFiltroListado(TableLayoutPanel row, int col, Control[] pair)
        {
            var cell = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty,
                BackColor = UiTheme.FilterStrip,
                Padding = new Padding(0)
            };
            cell.RowStyles.Add(new RowStyle(SizeType.Absolute, 18F));
            cell.RowStyles.Add(new RowStyle(SizeType.Absolute, AlturaFilaFiltro));

            if (pair == null || pair.Length == 0)
            {
                row.Controls.Add(cell, col, 0);
                return;
            }

            var label = pair[0];
            label.Dock = DockStyle.Fill;
            if (label is Label lbl)
            {
                lbl.AutoSize = false;
                lbl.TextAlign = ContentAlignment.MiddleLeft;
                lbl.ForeColor = UiTheme.TextMuted;
                lbl.BackColor = UiTheme.FilterStrip;
            }

            cell.Controls.Add(label, 0, 0);
            if (pair.Length > 1 && pair[1] != null)
            {
                pair[1].Dock = DockStyle.Fill;
                cell.Controls.Add(pair[1], 0, 1);
            }
            row.Controls.Add(cell, col, 0);
        }

        private static Label CrearLabelFiltro(string texto)
        {
            return new Label
            {
                Text = texto,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = UiTheme.TextMuted
            };
        }

        public void SincronizarCombosFiltroListado()
        {
            if (_cbFiltroProducto == null || _cbFiltroEstatus == null) return;

            try
            {
                if (_cbFiltroProducto.DataSource == null && cbProducto.DataSource is DataTable dtProd)
                {
                    DataTable copia = dtProd.Copy();
                    _cbFiltroProducto.DisplayMember = "nombre";
                    _cbFiltroProducto.ValueMember = "id";
                    _cbFiltroProducto.DataSource = copia;
                    _cbFiltroProducto.SelectedIndex = -1;
                    _cbFiltroProducto.Text = string.Empty;
                }

                if (_cbFiltroEstatus.DataSource == null && cbEstatus.DataSource is DataTable dtEst)
                {
                    DataTable copia = dtEst.Copy();
                    _cbFiltroEstatus.DisplayMember = "nombre";
                    _cbFiltroEstatus.ValueMember = "id";
                    _cbFiltroEstatus.DataSource = copia;
                    _cbFiltroEstatus.SelectedIndex = -1;
                    _cbFiltroEstatus.Text = string.Empty;
                }

                if (_cbFiltroLetraCliente.DataSource == null)
                {
                    var letras = new System.Collections.Generic.List<string>();
                    if (!string.IsNullOrWhiteSpace(letrasNaturales))
                        letras.AddRange(letrasNaturales.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    if (!string.IsNullOrWhiteSpace(letrasJuridicas))
                    {
                        foreach (string l in letrasJuridicas.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            string t = l.Trim();
                            if (!letras.Contains(t))
                                letras.Add(t);
                        }
                    }
                    _cbFiltroLetraCliente.DataSource = letras;
                    if (letras.Count > 0)
                        _cbFiltroLetraCliente.SelectedIndex = 0;
                }
            }
            catch
            {
                // Combos aún no cargados; se reintenta al entrar a listado.
            }
        }

        private void PrepararFiltrosListado()
        {
            SincronizarCombosFiltroListado();
            if (_dtpFiltroDesde != null && string.IsNullOrWhiteSpace(_dtpFiltroDesde.Text))
            {
                _dtpFiltroDesde.Format = DateTimePickerFormat.Custom;
                _dtpFiltroDesde.CustomFormat = " ";
            }
            if (_dtpFiltroHasta != null && string.IsNullOrWhiteSpace(_dtpFiltroHasta.Text))
            {
                _dtpFiltroHasta.Format = DateTimePickerFormat.Custom;
                _dtpFiltroHasta.CustomFormat = " ";
            }
        }

        private void ReconstruirPanelInicio()
        {
            pInicio.SuspendLayout();
            pInicio.Controls.Clear();
            pInicio.Dock = DockStyle.Top;
            pInicio.BorderStyle = BorderStyle.None;
            pInicio.BackColor = UiTheme.Card;
            pInicio.Padding = new Padding(14, 10, 14, 10);

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = UiTheme.Card
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            label6.Text = "Datos iniciales";
            label6.AutoSize = true;
            label6.Font = UiTheme.FontBold;
            label6.ForeColor = UiTheme.Primary;

            var header = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = UiTheme.Card
            };
            header.Controls.Add(label6);

            // Textos explícitos: el Designer tenía label3/label4 cruzados.
            label1.Text = "Nro ticket (auto)";
            label2.Text = "Fecha";
            label5.Text = "Producto";
            label3.Text = "Cédula conductor";
            label27.Text = "Nombre conductor";
            label4.Text = "Placa";
            rbtFechaHasta.Text = "Hasta";

            EstilarBotonCampo(btnBuscarConductor, "Buscar");
            var conductorBox = CrearCampoConBoton(txtConductor, btnBuscarConductor);

            // Hasta solo útil en listado; oculto en captura.
            rbtFechaHasta.Visible = false;
            dtpFechaHasta.Visible = false;

            var row1 = CrearFilaCampos(
                new Control[] { label1, txtNroTcicket },
                new Control[] { label2, dtpFecha },
                new Control[] { label5, cbProducto },
                new Control[] { rbtFechaHasta, dtpFechaHasta });

            var row2 = CrearFilaCampos(
                new Control[] { label3, conductorBox },
                new Control[] { label27, txtNombreConductor },
                new Control[] { label4, txtPlaca },
                null);

            foreach (Control c in new Control[] { txtNroTcicket, txtConductor, txtNombreConductor, txtPlaca, cbProducto, dtpFecha, dtpFechaHasta })
                c.Dock = DockStyle.Fill;

            root.Controls.Add(header, 0, 0);
            root.Controls.Add(row1, 0, 1);
            root.Controls.Add(row2, 0, 2);
            pInicio.Controls.Add(root);
            pInicio.ResumeLayout(true);
        }

        private void ReconstruirPanelGuia()
        {
            pGuia.SuspendLayout();
            pGuia.Controls.Clear();
            pGuia.Dock = DockStyle.Top;
            pGuia.BorderStyle = BorderStyle.None;
            pGuia.BackColor = UiTheme.Card;
            pGuia.Padding = new Padding(14, 10, 14, 10);

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = UiTheme.Card
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));

            var header = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 6, 0, 0),
                BackColor = UiTheme.Card
            };
            label7.Text = "Datos guía";
            label7.AutoSize = true;
            label7.Font = UiTheme.FontBold;
            label7.ForeColor = UiTheme.Primary;
            label7.Margin = new Padding(0, 2, 16, 0);
            rbTipoNatural.AutoSize = true;
            rbTipoNatural.Margin = new Padding(0, 2, 12, 0);
            rbJuridica.AutoSize = true;
            rbJuridica.Margin = new Padding(0, 2, 0, 0);
            header.Controls.Add(label7);
            header.Controls.Add(rbTipoNatural);
            header.Controls.Add(rbJuridica);

            label8.Text = "Nro guía";
            label9.Text = "Peso guía (Kg)";
            var rowGuia = CrearFilaCampos(
                new Control[] { label8, txtNroguia },
                new Control[] { label9, txtPesoGuia },
                null,
                null);

            var productorId = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0)
            };
            productorId.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 48F));
            productorId.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            cbLetras.Dock = DockStyle.Fill;
            txtDIProductor.Dock = DockStyle.Fill;
            productorId.Controls.Add(cbLetras, 0, 0);
            productorId.Controls.Add(txtDIProductor, 1, 0);

            EstilarBotonCampo(btnBuscarProductor, "Buscar");
            var productorNombre = CrearCampoConBoton(txtNombreProductor, btnBuscarProductor);

            lbPersona.Text = "Documento";
            var lblNombreProductor = new Label
            {
                Text = "Nombre productor",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };
            var rowProd = CrearFilaCampos(
                new Control[] { lbPersona, productorId },
                new Control[] { lblNombreProductor, productorNombre },
                null,
                null);

            foreach (Control c in new Control[] { txtNroguia, txtPesoGuia, txtDIProductor, txtNombreProductor })
                c.Dock = DockStyle.Fill;

            root.Controls.Add(header, 0, 0);
            root.Controls.Add(rowGuia, 0, 1);
            root.Controls.Add(rowProd, 0, 2);
            pGuia.Controls.Add(root);
            pGuia.ResumeLayout(true);
        }

        private Control CrearCampoConBoton(Control campo, Button btn)
        {
            var box = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0)
            };
            box.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            box.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 108F));
            campo.Dock = DockStyle.Fill;
            btn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn.Margin = new Padding(6, 3, 0, 3);
            box.Controls.Add(campo, 0, 0);
            box.Controls.Add(btn, 1, 0);
            return box;
        }

        private TableLayoutPanel CrearFilaCampos(Control[] c1, Control[] c2, Control[] c3, Control[] c4)
        {
            var row = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                BackColor = UiTheme.Card,
                Margin = new Padding(0, 2, 0, 2)
            };
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            AgregarCeldaCampo(row, 0, c1);
            AgregarCeldaCampo(row, 1, c2);
            AgregarCeldaCampo(row, 2, c3);
            AgregarCeldaCampo(row, 3, c4);
            return row;
        }

        private void AgregarCeldaCampo(TableLayoutPanel row, int col, Control[] pair)
        {
            var cell = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = new Padding(0, 0, 10, 0),
                BackColor = UiTheme.Card
            };
            cell.RowStyles.Add(new RowStyle(SizeType.Absolute, 18F));
            cell.RowStyles.Add(new RowStyle(SizeType.Absolute, AlturaFilaCampo));

            if (pair == null || pair.Length == 0)
            {
                row.Controls.Add(cell, col, 0);
                return;
            }

            var label = pair[0];
            label.Dock = DockStyle.Fill;
            if (label is Label lbl)
            {
                lbl.AutoSize = false;
                lbl.TextAlign = ContentAlignment.MiddleLeft;
                lbl.ForeColor = UiTheme.TextMuted;
            }
            else if (label is RadioButton rb)
            {
                rb.AutoSize = true;
                label.Dock = DockStyle.Left;
            }

            cell.Controls.Add(label, 0, 0);
            if (pair.Length > 1 && pair[1] != null)
            {
                Control campo = pair[1] is DateTimePicker
                    ? UiTheme.WrapInputFrame(pair[1])
                    : pair[1];
                campo.Dock = DockStyle.Fill;
                campo.Margin = new Padding(0);
                cell.Controls.Add(campo, 0, 1);
            }
            row.Controls.Add(cell, col, 0);
        }

        private Panel CrearPanelMeta()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                BackColor = UiTheme.Card,
                Padding = new Padding(14, 10, 14, 14),
                Height = AlturaCardMeta
            };

            var grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = UiTheme.Card
            };
            grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var top = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 6,
                RowCount = 1,
                BackColor = UiTheme.Card
            };
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 48F));
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 118F));
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));

            label23.Text = "Silo";
            label23.AutoSize = true;
            label23.Anchor = AnchorStyles.Left;
            label23.ForeColor = UiTheme.TextMuted;
            cbSilos.Dock = DockStyle.Fill;

            UiTheme.StyleAddButton(btnAgregarSilos);
            UiTheme.StyleEditButton(btnEditarSilos);
            btnAgregarSilos.Text = "Agregar";
            btnEditarSilos.Text = "Editar";
            btnAgregarSilos.Dock = DockStyle.Fill;
            btnEditarSilos.Dock = DockStyle.Fill;
            btnAgregarSilos.Margin = new Padding(8, 4, 4, 4);
            btnEditarSilos.Margin = new Padding(4, 4, 0, 4);

            label11.Text = "Estatus";
            label11.AutoSize = true;
            label11.Anchor = AnchorStyles.Left;
            label11.ForeColor = UiTheme.TextMuted;
            cbEstatus.Dock = DockStyle.Fill;

            top.Controls.Add(label23, 0, 0);
            top.Controls.Add(cbSilos, 1, 0);
            top.Controls.Add(btnAgregarSilos, 2, 0);
            top.Controls.Add(btnEditarSilos, 3, 0);
            top.Controls.Add(label11, 4, 0);
            top.Controls.Add(cbEstatus, 5, 0);

            var obs = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = UiTheme.Card
            };
            obs.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            obs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            obs.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            label24.Text = "Observaciones";
            label24.AutoSize = true;
            label24.Anchor = AnchorStyles.Left;
            label24.ForeColor = UiTheme.TextMuted;
            txtObservaciones.Dock = DockStyle.Fill;
            txtObservaciones.Multiline = true;
            txtObservaciones.Margin = new Padding(0, 2, 0, 4);
            txtObservaciones.MinimumSize = new Size(0, 52);
            lbAnulado.AutoSize = true;
            lbAnulado.Anchor = AnchorStyles.Left;
            obs.Controls.Add(label24, 0, 0);
            obs.Controls.Add(txtObservaciones, 1, 0);
            obs.Controls.Add(lbAnulado, 2, 0);

            grid.Controls.Add(top, 0, 0);
            grid.Controls.Add(obs, 0, 1);
            panel.Controls.Add(grid);
            return panel;
        }

        private Panel CrearPanelGrilla()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UiTheme.Card,
                Padding = new Padding(8)
            };

            var headerBar = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 28,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = UiTheme.Card,
                Padding = new Padding(0, 0, 4, 4)
            };
            headerBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            headerBar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var title = new Label
            {
                Text = "Resultados  ·  clic en encabezado para filtrar  ·  doble clic para abrir",
                Font = UiTheme.FontBold,
                ForeColor = UiTheme.Primary,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            lbTotal.AutoSize = true;
            lbTotal.Font = UiTheme.FontBold;
            lbTotal.ForeColor = UiTheme.Accent;
            lbTotal.TextAlign = ContentAlignment.MiddleRight;
            lbTotal.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lbTotal.Visible = false;
            lbTotal.Margin = new Padding(8, 4, 0, 0);

            headerBar.Controls.Add(title, 0, 0);
            headerBar.Controls.Add(lbTotal, 1, 0);

            dgvRecepciones.Dock = DockStyle.Fill;
            dgvRecepciones.BorderStyle = BorderStyle.None;
            dgvRecepciones.BackgroundColor = UiTheme.Card;
            dgvRecepciones.EnableHeadersVisualStyles = false;
            dgvRecepciones.ColumnHeadersDefaultCellStyle.BackColor = UiTheme.Primary;
            dgvRecepciones.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvRecepciones.ColumnHeadersDefaultCellStyle.Font = UiTheme.FontBold;
            dgvRecepciones.DefaultCellStyle.SelectionBackColor = UiTheme.Accent;
            dgvRecepciones.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvRecepciones.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(236, 242, 245);
            dgvRecepciones.RowHeadersVisible = false;
            dgvRecepciones.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (_filtroRecepciones == null)
                _filtroRecepciones = new GridHeaderFilter(dgvRecepciones);

            panel.Controls.Add(dgvRecepciones);
            panel.Controls.Add(headerBar);
            return panel;
        }

        public void ActualizarTotalesListado(decimal totalNeto, decimal totalAcondicionado, int cantidad)
        {
            if (lbTotal == null) return;
            if (cantidad == 0)
            {
                lbTotal.Visible = false;
                lbTotal.Text = string.Empty;
                return;
            }

            lbTotal.Visible = _modoListado;
            lbTotal.Text = string.Format(
                "Peso N. = {0:N0}   ·   Peso Acond. = {1:N2}   ·   {2} registro(s)",
                totalNeto,
                totalAcondicionado,
                cantidad);
        }

        private FlowLayoutPanel CrearPanelAcciones()
        {
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(8, 10, 8, 14),
                BackColor = UiTheme.Surface
            };

            foreach (var btn in new[] { btnGuardar, btnCancelar, btnAnular, btnImprimir, btnExportar })
            {
                btn.Size = new Size(120, 38);
                btn.Margin = new Padding(0, 0, 10, 0);
                panel.Controls.Add(btn);
            }

            UiTheme.ApplyActionButtons(panel);
            return panel;
        }

        public void ActualizarEspacioGrilla()
        {
            // La grilla vive solo en la pestaña Listado; no requiere ajuste dinámico de filas.
        }

        public void ActualizarPasosRecepcion()
        {
            if (_tcPasosCaptura == null) return;

            if (!_tcPasosCaptura.TabPages.Contains(tpAnalisis))
                _tcPasosCaptura.TabPages.Add(tpAnalisis);
            if (!_tcPasosCaptura.TabPages.Contains(tpRomana))
                _tcPasosCaptura.TabPages.Add(tpRomana);

            bool tieneRegistro = objRecepcion != null && objRecepcion.id > 0;
            bool anulado = tieneRegistro && objRecepcion.id_estatus == 6;
            bool analisisNoAplica = rbAnalisisNo != null && rbAnalisisNo.Checked;
            tpAnalisis.Enabled = tieneRegistro && !anulado;
            // Sin análisis, el trámite pasa a pesaje al existir el ticket (no espera estatus 2).
            tpRomana.Enabled = tieneRegistro && !anulado
                && (objRecepcion.id_estatus >= 2 || analisisNoAplica);

            if (!tpAnalisis.Enabled && _tcPasosCaptura.SelectedTab == tpAnalisis)
                _tcPasosCaptura.SelectedTab = dtpHasta;
            if (!tpRomana.Enabled && _tcPasosCaptura.SelectedTab == tpRomana)
                _tcPasosCaptura.SelectedTab = tieneRegistro ? tpAnalisis : dtpHasta;
        }

        public void ConfigurarColumnasRecepcionVisibles()
        {
            if (dgvRecepciones.DataSource == null || dgvRecepciones.Columns.Count == 0) return;

            string[] ocultar =
            {
                "cedulaproductor","id_estatus","id_producto","nro_guia","peso_guia",
                "rif","id_silos","hora_entrada","hora_salida","cedulaconductor","nombre_asociacion",
                "nombre_transporte","letranatural","letrajuridica","id_conductor","id","id_productor",
                "id_productor_juridico","observaciones","resumen","peso_bruto","peso_tara"
            };
            foreach (string col in ocultar)
                CatalogGridHelper.HideColumn(dgvRecepciones, col);

            CatalogGridHelper.SetHeader(dgvRecepciones, "Nro_Ticket", "Ticket");
            CatalogGridHelper.SetHeader(dgvRecepciones, "fecha", "Fecha");
            CatalogGridHelper.SetHeader(dgvRecepciones, "nombreconductor", "Conductor");
            CatalogGridHelper.SetHeader(dgvRecepciones, "nro_placa", "Placa");
            CatalogGridHelper.SetHeader(dgvRecepciones, "nombre_producto", "Producto");
            CatalogGridHelper.SetHeader(dgvRecepciones, "nombreproductornatural", "Productor");
            CatalogGridHelper.SetHeader(dgvRecepciones, "nombreproductorjuridico", "Productor jur.");
            CatalogGridHelper.SetHeader(dgvRecepciones, "nombresilo", "Silo");
            CatalogGridHelper.SetHeader(dgvRecepciones, "nombre_estatus", "Estatus");
            CatalogGridHelper.SetHeader(dgvRecepciones, "peso_neto", "Peso neto");
            CatalogGridHelper.SetHeader(dgvRecepciones, "peso_acondicionado", "Peso acond.");

            if (_filtroRecepciones != null)
                _filtroRecepciones.SyncHeaders();
        }

        private void ConstruirLayoutAnalisis()
        {
            tpAnalisis.SuspendLayout();
            tpAnalisis.Controls.Clear();

            var host = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(12),
                BackColor = UiTheme.Surface
            };
            host.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            host.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            host.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));

            var top = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(10, 8, 10, 8),
                BackColor = UiTheme.Card
            };
            label28.AutoSize = true;
            label28.Font = UiTheme.FontBold;
            label28.ForeColor = UiTheme.Primary;
            label28.Margin = new Padding(0, 6, 16, 0);
            rbAnalisis.Margin = new Padding(0, 6, 12, 0);
            rbAnalisisNo.Margin = new Padding(0, 6, 16, 0);
            var notaAnalisis = new Label
            {
                Text = "Los trámites de recepción no llevan análisis",
                AutoSize = true,
                ForeColor = UiTheme.TextMuted,
                Font = UiTheme.FontRegular,
                Margin = new Padding(8, 8, 0, 0)
            };
            top.Controls.Add(label28);
            top.Controls.Add(rbAnalisis);
            top.Controls.Add(rbAnalisisNo);
            top.Controls.Add(notaAnalisis);

            panel2.Dock = DockStyle.Fill;
            panel2.BorderStyle = BorderStyle.None;
            panel2.BackColor = UiTheme.Card;
            panel2.Padding = new Padding(10);
            panel2.AutoScroll = true;
            label33.Font = UiTheme.FontBold;
            label33.ForeColor = UiTheme.Primary;
            lbAdvertencia.ForeColor = UiTheme.Danger;
            dgvAnalisis.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvAnalisis.BorderStyle = BorderStyle.None;
            dgvAnalisis.BackgroundColor = UiTheme.Card;
            dgvAnalisis.EnableHeadersVisualStyles = false;
            dgvAnalisis.ColumnHeadersDefaultCellStyle.BackColor = UiTheme.Primary;
            dgvAnalisis.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAnalisis.ColumnHeadersDefaultCellStyle.Font = UiTheme.FontBold;
            dgvAnalisis.ColumnHeadersHeight = 32;
            dgvAnalisis.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            var actions = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(2, 8, 2, 2),
                BackColor = UiTheme.Surface
            };
            foreach (var btn in new[] { btnGuardarAnalisis, btnCancelarAnalisis, btnEliminarAnalisis })
            {
                btn.Size = new Size(130, 38);
                btn.Margin = new Padding(0, 0, 10, 0);
                actions.Controls.Add(btn);
            }
            UiTheme.ApplyActionButtons(actions);

            host.Controls.Add(top, 0, 0);
            host.Controls.Add(panel2, 0, 1);
            host.Controls.Add(actions, 0, 2);
            tpAnalisis.Controls.Add(host);
            tpAnalisis.ResumeLayout(true);

            panel2.Resize -= panel2_ResizeAnalisis;
            panel2.Resize += panel2_ResizeAnalisis;
            panel2_ResizeAnalisis(panel2, EventArgs.Empty);
        }

        private void panel2_ResizeAnalisis(object sender, EventArgs e)
        {
            if (dgvAnalisis == null || panel2 == null) return;
            int top = label33 != null ? label33.Bottom + 8 : 36;
            dgvAnalisis.Location = new Point(10, top);
            dgvAnalisis.Size = new Size(
                Math.Max(200, panel2.ClientSize.Width - 20),
                Math.Max(120, panel2.ClientSize.Height - top - 48));
        }

        private void ConstruirLayoutRomana()
        {
            tpRomana.SuspendLayout();
            tpRomana.Controls.Clear();

            var host = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(12),
                BackColor = UiTheme.Surface
            };
            host.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            host.RowStyles.Add(new RowStyle(SizeType.Absolute, 74F));

            var scroll = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = UiTheme.Surface,
                Padding = new Padding(0, 0, 8, 8)
            };

            var stack = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = UiTheme.Surface
            };
            stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            panel1.Controls.Clear();
            panel1.Dock = DockStyle.Top;
            panel1.AutoSize = true;
            panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel1.BorderStyle = BorderStyle.None;
            panel1.BackColor = UiTheme.Card;
            panel1.Padding = new Padding(16, 12, 16, 16);
            panel1.Margin = new Padding(0, 0, 0, 10);

            ReconstruirPanelPesaje();

            stack.Controls.Add(panel1, 0, 0);
            scroll.Controls.Add(stack);

            var actions = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(8, 10, 8, 14),
                BackColor = UiTheme.Surface
            };
            foreach (var btn in new[] { btnGuardarRomana, btnCancelarRomana })
            {
                btn.Size = new Size(130, 38);
                btn.Margin = new Padding(0, 0, 10, 0);
                actions.Controls.Add(btn);
            }
            UiTheme.ApplyActionButtons(actions);

            host.Controls.Add(scroll, 0, 0);
            host.Controls.Add(actions, 0, 1);
            tpRomana.Controls.Add(host);
            tpRomana.ResumeLayout(true);
        }

        private void ReconstruirPanelPesaje()
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 5,
                BackColor = UiTheme.Card
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));

            label13.Text = "Pesaje";
            label13.AutoSize = true;
            label13.Font = UiTheme.FontBold;
            label13.ForeColor = UiTheme.Primary;
            label13.Margin = new Padding(0, 8, 16, 0);
            label25.Visible = false;
            rbAutomatic.AutoSize = true;
            rbAutomatic.ForeColor = UiTheme.TextPrimary;
            rbAutomatic.Margin = new Padding(0, 8, 16, 0);
            rbManual.AutoSize = true;
            rbManual.ForeColor = UiTheme.TextPrimary;
            rbManual.Margin = new Padding(0, 8, 0, 0);

            var header = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = UiTheme.Card
            };
            header.Controls.Add(label13);
            header.Controls.Add(rbAutomatic);
            header.Controls.Add(rbManual);

            EstilarEtiquetaPesaje(label12, "Peso entrada (Kg)");
            EstilarEtiquetaPesaje(label15, "Hora entrada");
            EstilarEtiquetaPesaje(label18, "Peso salida (Kg)");
            EstilarEtiquetaPesaje(label16, "Hora salida");
            EstilarEtiquetaPesaje(label20, "Peso neto (Kg)");
            EstilarEtiquetaPesaje(label22, "Peso acondicionado (Kg)");
            OcultarEtiquetaUnidad(label10);
            OcultarEtiquetaUnidad(label14);
            OcultarEtiquetaUnidad(label17);
            OcultarEtiquetaUnidad(label19);
            OcultarEtiquetaUnidad(label21);

            UiTheme.StyleDateTimePicker(dtpHoraentrada);
            UiTheme.StyleDateTimePicker(dtpHoraSalida);
            UiTheme.StyleCompactPrimaryButton(btnPesoEntrada, "Tomar peso", 128, 32);
            UiTheme.StyleCompactPrimaryButton(btnPesoSalida, "Tomar peso", 128, 32);
            btnPesoEntrada.ForeColor = Color.White;
            btnPesoSalida.ForeColor = Color.White;

            var rowEntrada = CrearFilaPesaje(
                CrearCeldaPesaje(label12, txtPesoEntrada),
                CrearCeldaAccionPesaje(btnPesoEntrada),
                CrearCeldaPesaje(label15, dtpHoraentrada));

            var rowSalida = CrearFilaPesaje(
                CrearCeldaPesaje(label18, txtPesoSalida),
                CrearCeldaAccionPesaje(btnPesoSalida),
                CrearCeldaPesaje(label16, dtpHoraSalida));

            label26.Text = "Resultado";
            label26.AutoSize = true;
            label26.Font = UiTheme.FontBold;
            label26.ForeColor = UiTheme.Primary;
            label26.Dock = DockStyle.Fill;
            label26.TextAlign = ContentAlignment.MiddleLeft;

            txtPesoNeto.ReadOnly = true;
            txtPesoAcondicionado.ReadOnly = true;
            txtPesoNeto.BackColor = UiTheme.FilterStrip;
            txtPesoAcondicionado.BackColor = UiTheme.FilterStrip;
            txtPesoNeto.TabStop = false;
            txtPesoAcondicionado.TabStop = false;

            var rowResultado = CrearFilaPesaje(
                CrearCeldaPesaje(label20, txtPesoNeto),
                CrearCeldaPesaje(label22, txtPesoAcondicionado),
                null);

            foreach (Control c in new Control[] {
                txtPesoEntrada, txtPesoSalida, txtPesoNeto, txtPesoAcondicionado,
                dtpHoraentrada, dtpHoraSalida })
                c.Dock = DockStyle.Fill;

            root.Controls.Add(header, 0, 0);
            root.Controls.Add(rowEntrada, 0, 1);
            root.Controls.Add(rowSalida, 0, 2);
            root.Controls.Add(label26, 0, 3);
            root.Controls.Add(rowResultado, 0, 4);
            panel1.Controls.Add(root);
            ActualizarModoPesaje();
        }

        private static void EstilarEtiquetaPesaje(Label etiqueta, string texto)
        {
            etiqueta.Text = texto;
            etiqueta.AutoSize = false;
            etiqueta.Visible = true;
            etiqueta.Dock = DockStyle.Fill;
            etiqueta.TextAlign = ContentAlignment.BottomLeft;
            etiqueta.ForeColor = UiTheme.TextMuted;
            etiqueta.Font = UiTheme.FontRegular;
            etiqueta.BackColor = UiTheme.Card;
        }

        private static void OcultarEtiquetaUnidad(Label unidad)
        {
            if (unidad == null) return;
            unidad.Visible = false;
            unidad.Text = string.Empty;
            unidad.Parent?.Controls.Remove(unidad);
        }

        private TableLayoutPanel CrearFilaPesaje(Control c1, Control c2, Control c3)
        {
            var row = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = UiTheme.Card,
                Margin = new Padding(0, 2, 0, 2)
            };
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24F));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            if (c1 != null) row.Controls.Add(c1, 0, 0);
            if (c2 != null) row.Controls.Add(c2, 1, 0);
            if (c3 != null) row.Controls.Add(c3, 2, 0);
            return row;
        }

        private Control CrearCeldaPesaje(Label etiqueta, Control campo)
        {
            var cell = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = new Padding(0, 0, 12, 0),
                BackColor = UiTheme.Card
            };
            cell.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            cell.RowStyles.Add(new RowStyle(SizeType.Absolute, AlturaControlCampo));
            etiqueta.Visible = true;
            etiqueta.AutoSize = false;
            etiqueta.Dock = DockStyle.Fill;
            etiqueta.TextAlign = ContentAlignment.BottomLeft;
            etiqueta.ForeColor = UiTheme.TextMuted;
            etiqueta.Font = UiTheme.FontRegular;
            etiqueta.BackColor = UiTheme.Card;
            Control hosted = (campo is DateTimePicker || campo is TextBox)
                ? UiTheme.WrapInputFrame(campo)
                : campo;
            hosted.Dock = DockStyle.Fill;
            hosted.Margin = new Padding(0);
            cell.Controls.Add(etiqueta, 0, 0);
            cell.Controls.Add(hosted, 0, 1);
            return cell;
        }

        private Control CrearCeldaAccionPesaje(Button btn)
        {
            var cell = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = new Padding(0, 0, 12, 0),
                BackColor = UiTheme.Card
            };
            cell.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
            cell.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            var spacer = new Label { Text = " ", Dock = DockStyle.Fill, BackColor = UiTheme.Card };
            btn.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            btn.Margin = new Padding(0, 0, 0, 0);
            cell.Controls.Add(spacer, 0, 0);
            cell.Controls.Add(btn, 0, 1);
            return cell;
        }

        private void EstilizarControlesRecepcion()
        {
            foreach (Control c in new Control[]
            {
                txtNroTcicket, txtPlaca, txtConductor, txtNombreConductor, txtNroguia, txtPesoGuia,
                txtDIProductor, txtNombreProductor, txtObservaciones, txtPesoEntrada, txtPesoSalida,
                txtPesoNeto, txtPesoAcondicionado
            })
            {
                if (c is TextBox tb) UiTheme.StyleTextBox(tb);
            }

            UiTheme.StyleAddButton(btnAgregarSilos);
            UiTheme.StyleEditButton(btnEditarSilos);
            btnAgregarSilos.Text = "Agregar";
            btnEditarSilos.Text = "Editar";
            btnAgregarSilos.Width = 110;
            btnEditarSilos.Width = 100;
            btnAgregarSilos.Height = 32;
            btnEditarSilos.Height = 32;
            btnAgregarSilos.Tag = UiTheme.CompactActionTag;
            btnEditarSilos.Tag = UiTheme.CompactActionTag;

            if (txtPesoNeto != null)
            {
                txtPesoNeto.ReadOnly = true;
                txtPesoNeto.BackColor = UiTheme.FilterStrip;
            }
            if (txtPesoAcondicionado != null)
            {
                txtPesoAcondicionado.ReadOnly = true;
                txtPesoAcondicionado.BackColor = UiTheme.FilterStrip;
            }
        }

        public void ConfigurarColumnasAnalisis()
        {
            if (dgvAnalisis == null || dgvAnalisis.Columns.Count == 0) return;

            CatalogGridHelper.HideColumn(dgvAnalisis, "id");
            CatalogGridHelper.HideColumn(dgvAnalisis, "id_item");
            CatalogGridHelper.SetHeader(dgvAnalisis, "nombre", "Parámetro");
            CatalogGridHelper.SetHeader(dgvAnalisis, "menor", "Mínimo");
            CatalogGridHelper.SetHeader(dgvAnalisis, "mayor", "Máximo");
            CatalogGridHelper.SetHeader(dgvAnalisis, "valor_resultado", "Resultado");
        }

        public void ConfigurarTicketAutogenerado()
        {
            if (txtNroTcicket == null) return;
            txtNroTcicket.ReadOnly = true;
            txtNroTcicket.TabStop = false;
            txtNroTcicket.BackColor = UiTheme.FilterStrip;
            txtNroTcicket.ForeColor = UiTheme.TextMuted;
            txtNroTcicket.Cursor = Cursors.Default;
        }

        public void ConfigurarAnalisisSinUso()
        {
            if (rbAnalisisNo != null)
            {
                this.rbAnalisis.CheckedChanged -= rbResumen_CheckedChanged;
                rbAnalisisNo.Checked = true;
                if (rbAnalisis != null) rbAnalisis.Checked = false;
            }

            if (rbAnalisis != null) rbAnalisis.Enabled = false;
            if (rbAnalisisNo != null) rbAnalisisNo.Enabled = false;
            if (dgvAnalisis != null) dgvAnalisis.Enabled = false;
            if (panel2 != null) panel2.Enabled = false;
            if (btnGuardarAnalisis != null)
            {
                btnGuardarAnalisis.Enabled = false;
                btnGuardarAnalisis.Visible = false;
            }
            if (btnEliminarAnalisis != null)
            {
                btnEliminarAnalisis.Enabled = false;
                btnEliminarAnalisis.Visible = false;
            }
            if (btnCancelarAnalisis != null) btnCancelarAnalisis.Visible = true;

            objRecepcion.resumen = false;
        }
    }
}
