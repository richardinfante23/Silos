using System;
using System.Drawing;
using System.Windows.Forms;

namespace principal
{
    /// <summary>
    /// Diálogos de aviso, error y confirmación con el estilo de la aplicación.
    /// </summary>
    public static class UiDialog
    {
        public static DialogResult Show(string text)
        {
            return Show(null, text, UiTheme.AppShortName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static DialogResult Show(string text, string caption)
        {
            return Show(null, text, caption, MessageBoxButtons.OK, InferIcon(caption));
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            return Show(null, text, caption, buttons, InferIcon(caption));
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return Show(null, text, caption, buttons, icon);
        }

        public static DialogResult Show(IWin32Window owner, string text)
        {
            return Show(owner, text, UiTheme.AppShortName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption)
        {
            return Show(owner, text, caption, MessageBoxButtons.OK, InferIcon(caption));
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons)
        {
            return Show(owner, text, caption, buttons, InferIcon(caption));
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            using (var dlg = new DialogoMensaje(text, caption, buttons, icon))
            {
                return owner != null ? dlg.ShowDialog(owner) : dlg.ShowDialog();
            }
        }

        private static MessageBoxIcon InferIcon(string caption)
        {
            string key = (caption ?? string.Empty).ToLowerInvariant();
            if (key.Contains("error") || key.Contains("stop"))
                return MessageBoxIcon.Error;
            if (key.Contains("advert") || key.Contains("fallido") || key.Contains("warning"))
                return MessageBoxIcon.Warning;
            if (key.Contains("exito") || key.Contains("exitoso"))
                return MessageBoxIcon.Information;
            return MessageBoxIcon.Information;
        }

        private sealed class DialogoMensaje : Form
        {
            public DialogoMensaje(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
            {
                var paleta = PaletaDe(icon, caption);

                Text = string.IsNullOrWhiteSpace(caption) ? UiTheme.AppShortName : caption;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                StartPosition = FormStartPosition.CenterParent;
                ShowInTaskbar = false;
                MaximizeBox = false;
                MinimizeBox = false;
                ControlBox = true;
                BackColor = UiTheme.Card;
                Font = UiTheme.FontRegular;
                ForeColor = UiTheme.TextPrimary;
                Size = new Size(460, 240);
                MinimumSize = new Size(420, 200);
                Padding = new Padding(0);
                UiTheme.ApplyAppIcon(this);

                var root = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 3,
                    BackColor = UiTheme.Card
                };
                root.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
                root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                root.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));

                var header = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = paleta.Header,
                    Padding = new Padding(18, 0, 18, 0)
                };
                var titulo = new Label
                {
                    Text = Text,
                    Dock = DockStyle.Fill,
                    ForeColor = Color.White,
                    Font = UiTheme.FontBrand,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                header.Controls.Add(titulo);

                var cuerpo = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 1,
                    Padding = new Padding(18, 16, 18, 8),
                    BackColor = UiTheme.Card
                };
                cuerpo.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 52F));
                cuerpo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

                var badge = new Label
                {
                    Text = paleta.Glyph,
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                    ForeColor = paleta.Header,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                var mensaje = new Label
                {
                    Text = text ?? string.Empty,
                    Dock = DockStyle.Fill,
                    AutoSize = false,
                    Font = UiTheme.FontRegular,
                    ForeColor = UiTheme.TextPrimary,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                cuerpo.Controls.Add(badge, 0, 0);
                cuerpo.Controls.Add(mensaje, 1, 0);

                var pie = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    FlowDirection = FlowDirection.RightToLeft,
                    Padding = new Padding(12, 12, 16, 12),
                    BackColor = UiTheme.Surface,
                    WrapContents = false
                };

                AgregarBotones(pie, buttons, paleta);

                root.Controls.Add(header, 0, 0);
                root.Controls.Add(cuerpo, 0, 1);
                root.Controls.Add(pie, 0, 2);
                Controls.Add(root);

                AcceptButton = FindAccept(pie);
                CancelButton = FindCancel(pie);
            }

            private static Paleta PaletaDe(MessageBoxIcon icon, string caption)
            {
                string key = (caption ?? string.Empty).ToLowerInvariant();
                if (key.Contains("exito") || key.Contains("exitoso"))
                    return new Paleta(Color.FromArgb(26, 122, 92), "✓");

                // Error/Hand/Stop y Warning/Exclamation comparten el mismo valor numérico.
                switch ((int)icon)
                {
                    case 16: // Error / Hand / Stop
                        return new Paleta(UiTheme.Danger, "!");
                    case 48: // Warning / Exclamation
                        return new Paleta(Color.FromArgb(196, 122, 28), "!");
                    case 32: // Question
                        return new Paleta(UiTheme.Primary, "?");
                    default:
                        return new Paleta(UiTheme.Accent, "i");
                }
            }

            private void AgregarBotones(FlowLayoutPanel pie, MessageBoxButtons buttons, Paleta paleta)
            {
                switch (buttons)
                {
                    case MessageBoxButtons.YesNoCancel:
                        pie.Controls.Add(CrearBoton("Cancelar", DialogResult.Cancel, false));
                        pie.Controls.Add(CrearBoton("No", DialogResult.No, false));
                        pie.Controls.Add(CrearBoton("Sí", DialogResult.Yes, true, paleta.Header));
                        break;
                    case MessageBoxButtons.YesNo:
                        pie.Controls.Add(CrearBoton("No", DialogResult.No, false));
                        pie.Controls.Add(CrearBoton("Sí", DialogResult.Yes, true, paleta.Header));
                        break;
                    case MessageBoxButtons.OKCancel:
                        pie.Controls.Add(CrearBoton("Cancelar", DialogResult.Cancel, false));
                        pie.Controls.Add(CrearBoton("Aceptar", DialogResult.OK, true, paleta.Header));
                        break;
                    default:
                        pie.Controls.Add(CrearBoton("Aceptar", DialogResult.OK, true, paleta.Header));
                        break;
                }
            }

            private Button CrearBoton(string texto, DialogResult resultado, bool primario, Color? color = null)
            {
                var btn = new Button
                {
                    Text = texto,
                    DialogResult = resultado,
                    Size = new Size(108, 34),
                    Margin = new Padding(8, 0, 0, 0)
                };
                if (primario)
                    UiTheme.StyleCompactPrimaryButton(btn, texto, 108, 34);
                else
                    UiTheme.StyleCompactSecondaryButton(btn, texto, 108, 34);

                if (primario && color.HasValue)
                {
                    btn.BackColor = color.Value;
                    btn.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(color.Value, 0.08F);
                    btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(color.Value, 0.12F);
                }
                return btn;
            }

            private static Button FindAccept(Control pie)
            {
                foreach (Control c in pie.Controls)
                {
                    var b = c as Button;
                    if (b != null && (b.DialogResult == DialogResult.OK || b.DialogResult == DialogResult.Yes))
                        return b;
                }
                return null;
            }

            private static Button FindCancel(Control pie)
            {
                foreach (Control c in pie.Controls)
                {
                    var b = c as Button;
                    if (b != null && (b.DialogResult == DialogResult.Cancel || b.DialogResult == DialogResult.No))
                        return b;
                }
                return null;
            }

            private struct Paleta
            {
                public readonly Color Header;
                public readonly string Glyph;
                public Paleta(Color header, string glyph)
                {
                    Header = header;
                    Glyph = glyph;
                }
            }
        }
    }
}
