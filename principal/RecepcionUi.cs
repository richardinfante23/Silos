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
        private Panel _panelGrillaRecepcion;

        private void AplicarUiRecepcionModerna()
        {
            if (_uiModernizada) return;
            _uiModernizada = true;

            this.Text = "Recepción";
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.MinimumSize = new Size(1024, 700);
            this.BackColor = UiTheme.Surface;
            this.Font = UiTheme.FontRegular;
            this.DoubleBuffered = true;

            tcRecepcion.Dock = DockStyle.Fill;
            tcRecepcion.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dtpHasta.Text = "1. Datos iniciales";
            tpAnalisis.Text = "2. Análisis";
            tpRomana.Text = "3. Romana";
            dtpHasta.BackColor = UiTheme.Surface;
            tpAnalisis.BackColor = UiTheme.Surface;
            tpRomana.BackColor = UiTheme.Surface;
            label11.Text = "En tránsito:";
            label24.Text = "Observaciones:";
            label7.Text = "Datos de guía";
            label6.Text = "Datos del ticket";
            label28.Text = "Análisis:";

            ConstruirLayoutDatosIniciales();
            ConstruirLayoutAnalisis();
            ConstruirLayoutRomana();
            EstilizarControlesRecepcion();
            ActualizarEspacioGrilla();
        }

        private void ConstruirLayoutDatosIniciales()
        {
            dtpHasta.SuspendLayout();
            dtpHasta.Controls.Clear();

            _hostDatosRecepcion = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                Padding = new Padding(12),
                BackColor = UiTheme.Surface
            };
            // Form first, grid expands with remaining space
            _hostDatosRecepcion.RowStyles.Add(new RowStyle(SizeType.Absolute, 168F)); // ticket
            _hostDatosRecepcion.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F)); // guia
            _hostDatosRecepcion.RowStyles.Add(new RowStyle(SizeType.Absolute, 108F)); // meta
            _hostDatosRecepcion.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // grid
            _hostDatosRecepcion.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));  // actions

            ReconstruirPanelInicio();
            ReconstruirPanelGuia();
            _panelMetaRecepcion = CrearPanelMeta();
            _panelGrillaRecepcion = CrearPanelGrilla();
            _panelAccionesRecepcion = CrearPanelAcciones();

            _hostDatosRecepcion.Controls.Add(pInicio, 0, 0);
            _hostDatosRecepcion.Controls.Add(pGuia, 0, 1);
            _hostDatosRecepcion.Controls.Add(_panelMetaRecepcion, 0, 2);
            _hostDatosRecepcion.Controls.Add(_panelGrillaRecepcion, 0, 3);
            _hostDatosRecepcion.Controls.Add(_panelAccionesRecepcion, 0, 4);

            dtpHasta.Controls.Add(_hostDatosRecepcion);
            dtpHasta.ResumeLayout(true);
        }

        private void ReconstruirPanelInicio()
        {
            pInicio.SuspendLayout();
            pInicio.Controls.Clear();
            pInicio.Dock = DockStyle.Fill;
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
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

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

            var row1 = CrearFilaCampos(
                new Control[] { label1, txtNroTcicket },
                new Control[] { label2, dtpFecha },
                new Control[] { label5, cbProducto },
                new Control[] { rbtFechaHasta, dtpFechaHasta });

            var conductorBox = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0)
            };
            conductorBox.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            conductorBox.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 88F));
            txtConductor.Dock = DockStyle.Fill;
            btnBuscarConductor.Dock = DockStyle.Fill;
            btnBuscarConductor.Margin = new Padding(6, 0, 0, 0);
            conductorBox.Controls.Add(txtConductor, 0, 0);
            conductorBox.Controls.Add(btnBuscarConductor, 1, 0);

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
            pGuia.Dock = DockStyle.Fill;
            pGuia.BorderStyle = BorderStyle.None;
            pGuia.BackColor = UiTheme.Card;
            pGuia.Padding = new Padding(14, 10, 14, 10);

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = UiTheme.Card
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));

            var header = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = UiTheme.Card
            };
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            label7.AutoSize = true;
            label7.Font = UiTheme.FontBold;
            label7.ForeColor = UiTheme.Primary;
            rbTipoNatural.AutoSize = true;
            rbJuridica.AutoSize = true;
            header.Controls.Add(label7, 0, 0);
            header.Controls.Add(rbTipoNatural, 1, 0);
            header.Controls.Add(rbJuridica, 2, 0);

            var rowGuia = CrearFilaCampos(
                new Control[] { label8, txtNroguia },
                new Control[] { label9, CrearCampoConUnidad(txtPesoGuia, label10) },
                null,
                null);

            var productorId = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            productorId.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 48F));
            productorId.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            cbLetras.Dock = DockStyle.Fill;
            txtDIProductor.Dock = DockStyle.Fill;
            productorId.Controls.Add(cbLetras, 0, 0);
            productorId.Controls.Add(txtDIProductor, 1, 0);

            var productorNombre = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            productorNombre.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            productorNombre.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 88F));
            txtNombreProductor.Dock = DockStyle.Fill;
            btnBuscarProductor.Dock = DockStyle.Fill;
            btnBuscarProductor.Margin = new Padding(6, 0, 0, 0);
            productorNombre.Controls.Add(txtNombreProductor, 0, 0);
            productorNombre.Controls.Add(btnBuscarProductor, 1, 0);

            lbPersona.Text = "Documento:";
            var rowProd = CrearFilaCampos(
                new Control[] { lbPersona, productorId },
                new Control[] { new Label { Text = "Nombre:", AutoSize = true, Anchor = AnchorStyles.Left }, productorNombre },
                null,
                null);

            var ejemplos = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = UiTheme.Card
            };
            lbEjemploletraConductor.ForeColor = UiTheme.TextMuted;
            lbEjemploCedulaCondcutor.ForeColor = UiTheme.TextMuted;
            lbEjemploletraConductor.AutoSize = true;
            lbEjemploCedulaCondcutor.AutoSize = true;
            lbEjemploletraConductor.Margin = new Padding(0, 2, 16, 0);
            lbEjemploCedulaCondcutor.Margin = new Padding(0, 2, 0, 0);
            ejemplos.Controls.Add(lbEjemploletraConductor);
            ejemplos.Controls.Add(lbEjemploCedulaCondcutor);

            foreach (Control c in new Control[] { txtNroguia, txtPesoGuia, txtDIProductor, txtNombreProductor })
                c.Dock = DockStyle.Fill;

            root.Controls.Add(header, 0, 0);
            root.Controls.Add(rowGuia, 0, 1);
            root.Controls.Add(rowProd, 0, 2);
            root.Controls.Add(ejemplos, 0, 3);
            pGuia.Controls.Add(root);
            pGuia.ResumeLayout(true);
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
                Dock = DockStyle.Fill,
                BackColor = UiTheme.Card,
                Padding = new Padding(14, 10, 14, 8)
            };

            var grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = UiTheme.Card
            };
            grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
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
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 124F));
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 124F));
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));

            label23.Text = "Silo:";
            label23.AutoSize = true;
            label23.Anchor = AnchorStyles.Left;
            cbSilos.Dock = DockStyle.Fill;
            btnAgregarSilos.Dock = DockStyle.Fill;
            btnEditarSilos.Dock = DockStyle.Fill;
            btnAgregarSilos.Margin = new Padding(10, 2, 4, 2);
            btnEditarSilos.Margin = new Padding(4, 2, 0, 2);
            label11.AutoSize = true;
            label11.Anchor = AnchorStyles.Left;
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
            label24.AutoSize = true;
            label24.Anchor = AnchorStyles.Left;
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
                Padding = new Padding(8),
                Visible = false
            };

            var title = new Label
            {
                Text = "Resultados",
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
                Padding = new Padding(2, 8, 2, 2),
                BackColor = UiTheme.Surface
            };

            foreach (var btn in new[] { btnGuardar, btnCancelar, btnAnular, btnImprimir, btnExportar })
            {
                btn.Size = new Size(120, 40);
                btn.Margin = new Padding(0, 0, 10, 0);
                btn.Image = null;
                btn.TextImageRelation = TextImageRelation.Overlay;
                btn.TextAlign = ContentAlignment.MiddleCenter;
                panel.Controls.Add(btn);
            }

            UiTheme.StylePrimaryButton(btnGuardar);
            UiTheme.StyleSecondaryButton(btnCancelar);
            UiTheme.StyleSecondaryButton(btnAnular);
            UiTheme.StyleSecondaryButton(btnImprimir);
            UiTheme.StyleSecondaryButton(btnExportar);
            btnAnular.ForeColor = UiTheme.Danger;
            return panel;
        }

        public void ActualizarEspacioGrilla()
        {
            if (_hostDatosRecepcion == null || _panelGrillaRecepcion == null) return;

            bool mostrar = !nuevo || (dgvRecepciones.DataSource is DataTable dt && dt.Rows.Count > 0)
                           || dgvRecepciones.Rows.Count > 0;

            _panelGrillaRecepcion.Visible = mostrar;
            if (mostrar)
            {
                _hostDatosRecepcion.RowStyles[0] = new RowStyle(SizeType.Absolute, 150F);
                _hostDatosRecepcion.RowStyles[1] = new RowStyle(SizeType.Absolute, 140F);
                _hostDatosRecepcion.RowStyles[2] = new RowStyle(SizeType.Absolute, 100F);
                _hostDatosRecepcion.RowStyles[3] = new RowStyle(SizeType.Percent, 100F);
            }
            else
            {
                _hostDatosRecepcion.RowStyles[0] = new RowStyle(SizeType.Absolute, 168F);
                _hostDatosRecepcion.RowStyles[1] = new RowStyle(SizeType.Absolute, 150F);
                _hostDatosRecepcion.RowStyles[2] = new RowStyle(SizeType.Absolute, 108F);
                _hostDatosRecepcion.RowStyles[3] = new RowStyle(SizeType.Absolute, 0F);
            }
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
                btn.Size = new Size(130, 40);
                btn.Margin = new Padding(0, 0, 10, 0);
                btn.Image = null;
                btn.TextAlign = ContentAlignment.MiddleCenter;
                actions.Controls.Add(btn);
            }
            UiTheme.StylePrimaryButton(btnGuardarAnalisis);
            UiTheme.StyleSecondaryButton(btnCancelarAnalisis);
            UiTheme.StyleDangerButton(btnEliminarAnalisis);

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
                btn.Size = new Size(130, 40);
                btn.Margin = new Padding(0, 0, 10, 0);
                btn.Image = null;
                btn.TextAlign = ContentAlignment.MiddleCenter;
                actions.Controls.Add(btn);
            }
            UiTheme.StylePrimaryButton(btnGuardarRomana);
            UiTheme.StyleSecondaryButton(btnCancelarRomana);
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

            UiTheme.StyleActionButton(
                btnBuscarConductor,
                UiTheme.Card,
                UiTheme.TextPrimary,
                Color.FromArgb(230, 236, 241),
                1,
                null,
                UiTheme.Border);
            UiTheme.StyleActionButton(
                btnBuscarProductor,
                UiTheme.Card,
                UiTheme.TextPrimary,
                Color.FromArgb(230, 236, 241),
                1,
                null,
                UiTheme.Border);
            UiTheme.StyleAddButton(btnAgregarSilos);
            UiTheme.StyleEditButton(btnEditarSilos);
            btnAgregarSilos.Text = "Agregar";
            btnEditarSilos.Text = "Editar";
        }
    }
}
