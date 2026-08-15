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
        private TableLayoutPanel _hostDatosRecepcion;
        private Panel _panelSuperiorRecepcion;
        private Panel _panelGrillaRecepcion;
        private Panel _panelFormularioRecepcion;
        private Panel _panelFiltrosListado;
        private FlowLayoutPanel _barraModoRecepcion;
        private Button _btnModoListado;
        private Button _btnModoCaptura;
        private Button _btnBuscarListado;
        private Label _lblModoRecepcion;
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
        private const float AlturaFiltrosListado = 144F;

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
            tcRecepcion.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dtpHasta.Text = "1. Datos";
            tpAnalisis.Text = "2. Análisis";
            tpRomana.Text = "3. Romana";
            dtpHasta.BackColor = UiTheme.Surface;
            tpAnalisis.BackColor = UiTheme.Surface;
            tpRomana.BackColor = UiTheme.Surface;
            label1.Text = "Nro ticket";
            label2.Text = "Fecha";
            label5.Text = "Producto";
            label3.Text = "Cédula conductor";
            label27.Text = "Nombre conductor";
            label4.Text = "Placa";
            label6.Text = "Datos iniciales";
            label7.Text = "Datos guía";
            label8.Text = "Nro guía";
            label9.Text = "Peso guía";
            label11.Text = "Estatus";
            label23.Text = "Silo";
            label24.Text = "Observaciones";
            label28.Text = "Análisis:";

            // Pasos siempre presentes; se habilitan según el estatus.
            if (!tcRecepcion.TabPages.Contains(dtpHasta))
                tcRecepcion.TabPages.Add(dtpHasta);
            if (!tcRecepcion.TabPages.Contains(tpAnalisis))
                tcRecepcion.TabPages.Add(tpAnalisis);
            if (!tcRecepcion.TabPages.Contains(tpRomana))
                tcRecepcion.TabPages.Add(tpRomana);

            ConstruirLayoutDatosIniciales();
            ConstruirLayoutAnalisis();
            ConstruirLayoutRomana();
            EstilizarControlesRecepcion();
            ActualizarPasosRecepcion();
            AplicarModoRecepcion(!nuevo);
        }

        private void ConstruirLayoutDatosIniciales()
        {
            dtpHasta.SuspendLayout();
            dtpHasta.Controls.Clear();

            _hostDatosRecepcion = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(10, 10, 16, 10),
                BackColor = UiTheme.Surface
            };
            _hostDatosRecepcion.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F)); // modo
            _hostDatosRecepcion.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));  // formulario
            _hostDatosRecepcion.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));  // grilla
            // 64px: botón 38 + Padding top/bottom (8+10) + margen; evita clip del borde inferior.
            _hostDatosRecepcion.RowStyles.Add(new RowStyle(SizeType.Absolute, 74F)); // acciones

            _barraModoRecepcion = CrearBarraModo();
            _panelFormularioRecepcion = CrearPanelFormulario();
            _panelFiltrosListado = CrearPanelFiltrosListado();
            _panelGrillaRecepcion = CrearPanelGrilla();
            _panelAccionesRecepcion = CrearPanelAcciones();

            _panelSuperiorRecepcion = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UiTheme.Surface
            };
            _panelFormularioRecepcion.Dock = DockStyle.Fill;
            _panelFiltrosListado.Dock = DockStyle.Fill;
            _panelSuperiorRecepcion.Controls.Add(_panelFormularioRecepcion);
            _panelSuperiorRecepcion.Controls.Add(_panelFiltrosListado);

            _hostDatosRecepcion.Controls.Add(_barraModoRecepcion, 0, 0);
            _hostDatosRecepcion.Controls.Add(_panelSuperiorRecepcion, 0, 1);
            _hostDatosRecepcion.Controls.Add(_panelGrillaRecepcion, 0, 2);
            _hostDatosRecepcion.Controls.Add(_panelAccionesRecepcion, 0, 3);

            dtpHasta.Controls.Add(_hostDatosRecepcion);
            dtpHasta.ResumeLayout(true);
        }

        private FlowLayoutPanel CrearBarraModo()
        {
            var bar = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(2, 6, 2, 2),
                BackColor = UiTheme.Surface
            };

            _btnModoCaptura = new Button { Text = "Captura", Size = new Size(110, 32), Margin = new Padding(0, 0, 8, 0) };
            _btnModoListado = new Button { Text = "Listado", Size = new Size(110, 32), Margin = new Padding(0, 0, 12, 0) };
            _lblModoRecepcion = new Label
            {
                AutoSize = true,
                Margin = new Padding(8, 8, 0, 0),
                ForeColor = UiTheme.TextMuted,
                Font = UiTheme.FontRegular,
                Text = "Complete los datos del ticket"
            };

            UiTheme.StylePrimaryButton(_btnModoCaptura);
            UiTheme.StyleSecondaryButton(_btnModoListado);
            _btnModoCaptura.Width = 110;
            _btnModoCaptura.Height = 32;
            _btnModoListado.Width = 110;
            _btnModoListado.Height = 32;

            _btnModoCaptura.Click += (s, e) => AplicarModoRecepcion(false);
            _btnModoListado.Click += (s, e) =>
            {
                nuevo = false;
                btnGuardar.Text = "Buscar";
                txtNroTcicket.ReadOnly = false;
                cbEstatus.Enabled = true;
                AplicarModoRecepcion(true);
            };

            bar.Controls.Add(_btnModoCaptura);
            bar.Controls.Add(_btnModoListado);
            bar.Controls.Add(_lblModoRecepcion);
            return bar;
        }

        private Panel CrearPanelFormulario()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UiTheme.Surface,
                AutoScroll = true,
                Padding = new Padding(0, 0, 4, 0)
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

            pInicio.MinimumSize = new Size(0, 168);
            pInicio.Height = 168;
            pInicio.Dock = DockStyle.Top;
            pGuia.MinimumSize = new Size(0, 168);
            pGuia.Height = 168;
            pGuia.Dock = DockStyle.Top;
            _panelMetaRecepcion.MinimumSize = new Size(0, 118);
            _panelMetaRecepcion.Height = 118;
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
            if (_hostDatosRecepcion == null) return;

            if (listado)
            {
                _lblModoRecepcion.Text = "Busque recepciones y haga doble clic para abrirlas";
                _hostDatosRecepcion.RowStyles[1] = new RowStyle(SizeType.Absolute, AlturaFiltrosListado);
                _hostDatosRecepcion.RowStyles[2] = new RowStyle(SizeType.Percent, 100F);
                if (_panelFiltrosListado != null)
                {
                    _panelFiltrosListado.Visible = true;
                    _panelFiltrosListado.BringToFront();
                }
                if (_panelFormularioRecepcion != null)
                    _panelFormularioRecepcion.Visible = false;
                if (_panelGrillaRecepcion != null)
                    _panelGrillaRecepcion.Visible = true;
                PrepararFiltrosListado();
                UiTheme.StyleSecondaryButton(_btnModoCaptura);
                UiTheme.StylePrimaryButton(_btnModoListado);
            }
            else
            {
                _lblModoRecepcion.Text = "Complete o edite los datos del ticket";
                _hostDatosRecepcion.RowStyles[1] = new RowStyle(SizeType.Percent, 100F);
                _hostDatosRecepcion.RowStyles[2] = new RowStyle(SizeType.Absolute, 0F);
                if (_panelFiltrosListado != null)
                    _panelFiltrosListado.Visible = false;
                if (_panelFormularioRecepcion != null)
                {
                    _panelFormularioRecepcion.Visible = true;
                    _panelFormularioRecepcion.BringToFront();
                    RestaurarFormularioCaptura();
                }
                if (_panelGrillaRecepcion != null)
                    _panelGrillaRecepcion.Visible = false;
                // Hasta solo aplica en listado.
                if (rbtFechaHasta != null) rbtFechaHasta.Visible = false;
                if (dtpFechaHasta != null) dtpFechaHasta.Visible = false;
                UiTheme.StylePrimaryButton(_btnModoCaptura);
                UiTheme.StyleSecondaryButton(_btnModoListado);
            }

            _btnModoCaptura.Width = 110;
            _btnModoCaptura.Height = 32;
            _btnModoListado.Width = 110;
            _btnModoListado.Height = 32;
            ActualizarEspacioGrilla();
        }

        private void RestaurarFormularioCaptura()
        {
            if (pInicio == null || pGuia == null || _panelMetaRecepcion == null) return;
            pInicio.MinimumSize = new Size(0, 168);
            pInicio.Height = 168;
            pInicio.Visible = true;
            pGuia.Visible = true;
            pGuia.MinimumSize = new Size(0, 168);
            pGuia.Height = 168;
            _panelMetaRecepcion.Visible = true;
            _panelMetaRecepcion.MinimumSize = new Size(0, 118);
            _panelMetaRecepcion.Height = 118;
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
        }

        private Panel CrearPanelFiltrosListado()
        {
            // Host con padding externo: el borde del strip nunca queda flush al borde del tab.
            var host = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UiTheme.Surface,
                Padding = new Padding(12, 8, 16, 8),
                Visible = false
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
            clienteBox.Controls.Add(_txtFiltroCliente, 1, 0);

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
            cell.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

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
            lbTotal.AutoSize = true;
            lbTotal.Font = UiTheme.FontBold;
            lbTotal.ForeColor = UiTheme.Accent;

            var header = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = UiTheme.Card
            };
            header.Controls.Add(label6);
            header.Controls.Add(lbTotal);

            // Textos explícitos: el Designer tenía label3/label4 cruzados.
            label1.Text = "Nro ticket";
            label2.Text = "Fecha";
            label5.Text = "Producto";
            label3.Text = "Cédula conductor";
            label27.Text = "Nombre conductor";
            label4.Text = "Placa";
            rbtFechaHasta.Text = "Hasta";

            UiTheme.StyleSecondaryButton(btnBuscarConductor);
            btnBuscarConductor.Text = "Buscar";
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
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));

            var header = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = UiTheme.Card
            };
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            label7.Text = "Datos guía";
            label7.AutoSize = true;
            label7.Font = UiTheme.FontBold;
            label7.ForeColor = UiTheme.Primary;
            rbTipoNatural.AutoSize = true;
            rbJuridica.AutoSize = true;
            header.Controls.Add(label7, 0, 0);
            header.Controls.Add(rbTipoNatural, 1, 0);
            header.Controls.Add(rbJuridica, 2, 0);

            label8.Text = "Nro guía";
            label9.Text = "Peso guía";
            var rowGuia = CrearFilaCampos(
                new Control[] { label8, txtNroguia },
                new Control[] { label9, CrearCampoConUnidad(txtPesoGuia, label10) },
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

            UiTheme.StyleSecondaryButton(btnBuscarProductor);
            btnBuscarProductor.Text = "Buscar";
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
            box.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
            campo.Dock = DockStyle.Fill;
            btn.Dock = DockStyle.Fill;
            btn.Margin = new Padding(6, 0, 0, 0);
            box.Controls.Add(campo, 0, 0);
            box.Controls.Add(btn, 1, 0);
            return box;
        }

        private Control CrearCampoConUnidad(Control campo, Label unidad)
        {
            var box = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0)
            };
            box.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            box.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 28F));
            campo.Dock = DockStyle.Fill;
            unidad.AutoSize = true;
            unidad.Anchor = AnchorStyles.Left;
            unidad.Margin = new Padding(6, 4, 0, 0);
            box.Controls.Add(campo, 0, 0);
            box.Controls.Add(unidad, 1, 0);
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
            cell.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

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
                pair[1].Dock = DockStyle.Fill;
                cell.Controls.Add(pair[1], 0, 1);
            }
            row.Controls.Add(cell, col, 0);
        }

        private Panel CrearPanelMeta()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                BackColor = UiTheme.Card,
                Padding = new Padding(14, 10, 14, 8),
                Height = 118
            };

            var grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = UiTheme.Card
            };
            grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
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
            btnAgregarSilos.Margin = new Padding(8, 2, 4, 2);
            btnEditarSilos.Margin = new Padding(4, 2, 0, 2);

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

            var title = new Label
            {
                Text = "Resultados  ·  clic en encabezado para filtrar  ·  doble clic para abrir",
                Font = UiTheme.FontBold,
                ForeColor = UiTheme.Primary,
                Dock = DockStyle.Top,
                Height = 24,
                TextAlign = ContentAlignment.MiddleLeft
            };

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
            panel.Controls.Add(title);
            return panel;
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
            if (_hostDatosRecepcion == null || _panelGrillaRecepcion == null) return;

            if (_modoListado)
            {
                _panelGrillaRecepcion.Visible = true;
                _hostDatosRecepcion.RowStyles[1] = new RowStyle(SizeType.Absolute, AlturaFiltrosListado);
                _hostDatosRecepcion.RowStyles[2] = new RowStyle(SizeType.Percent, 100F);
            }
            else
            {
                _panelGrillaRecepcion.Visible = false;
                _hostDatosRecepcion.RowStyles[1] = new RowStyle(SizeType.Percent, 100F);
                _hostDatosRecepcion.RowStyles[2] = new RowStyle(SizeType.Absolute, 0F);
            }
        }

        public void ActualizarPasosRecepcion()
        {
            // Siempre visibles; se habilitan según estatus / modo.
            if (!tcRecepcion.TabPages.Contains(tpAnalisis))
                tcRecepcion.TabPages.Add(tpAnalisis);
            if (!tcRecepcion.TabPages.Contains(tpRomana))
                tcRecepcion.TabPages.Add(tpRomana);

            bool tieneRegistro = objRecepcion != null && objRecepcion.id > 0;
            tpAnalisis.Enabled = tieneRegistro;
            tpRomana.Enabled = tieneRegistro && objRecepcion.id_estatus >= 2;

            if (!tpAnalisis.Enabled && tcRecepcion.SelectedTab == tpAnalisis)
                tcRecepcion.SelectedTab = dtpHasta;
            if (!tpRomana.Enabled && tcRecepcion.SelectedTab == tpRomana)
                tcRecepcion.SelectedTab = tieneRegistro ? tpAnalisis : dtpHasta;
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
            rbAnalisisNo.Margin = new Padding(0, 6, 0, 0);
            top.Controls.Add(label28);
            top.Controls.Add(rbAnalisis);
            top.Controls.Add(rbAnalisisNo);

            panel2.Dock = DockStyle.Fill;
            panel2.BorderStyle = BorderStyle.None;
            panel2.BackColor = UiTheme.Card;
            panel2.Padding = new Padding(10);
            panel2.AutoScroll = true;
            label33.Font = UiTheme.FontBold;
            label33.ForeColor = UiTheme.Primary;
            lbAdvertencia.ForeColor = UiTheme.Danger;
            dgvAnalisis.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

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
            host.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));

            panel1.Dock = DockStyle.Fill;
            panel1.BorderStyle = BorderStyle.None;
            panel1.BackColor = UiTheme.Card;
            panel1.AutoScroll = true;
            panel1.Padding = new Padding(16);
            if (!panel1.Controls.Contains(btnPesoEntrada))
                panel1.Controls.Add(btnPesoEntrada);
            if (!panel1.Controls.Contains(btnPesoSalida))
                panel1.Controls.Add(btnPesoSalida);

            var actions = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(2, 8, 2, 2),
                BackColor = UiTheme.Surface
            };
            foreach (var btn in new[] { btnGuardarRomana, btnCancelarRomana })
            {
                btn.Size = new Size(130, 38);
                btn.Margin = new Padding(0, 0, 10, 0);
                actions.Controls.Add(btn);
            }
            UiTheme.ApplyActionButtons(actions);
            UiTheme.StylePrimaryButton(btnPesoEntrada);
            UiTheme.StylePrimaryButton(btnPesoSalida);
            btnPesoEntrada.Size = new Size(150, 28);
            btnPesoSalida.Size = new Size(150, 28);

            host.Controls.Add(panel1, 0, 0);
            host.Controls.Add(actions, 0, 1);
            tpRomana.Controls.Add(host);
            tpRomana.ResumeLayout(true);
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

            UiTheme.StyleSecondaryButton(btnBuscarConductor);
            UiTheme.StyleSecondaryButton(btnBuscarProductor);
            btnBuscarConductor.Text = "Buscar";
            btnBuscarProductor.Text = "Buscar";
            UiTheme.StyleAddButton(btnAgregarSilos);
            UiTheme.StyleEditButton(btnEditarSilos);
            btnAgregarSilos.Text = "Agregar";
            btnEditarSilos.Text = "Editar";
            btnAgregarSilos.Width = 110;
            btnEditarSilos.Width = 100;
            btnAgregarSilos.Height = 28;
            btnEditarSilos.Height = 28;
        }
    }
}
