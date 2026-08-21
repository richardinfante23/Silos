using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace principal
{
    /// <summary>
    /// Identidad visual compartida del Sistema de Gestión de Pesaje.
    /// </summary>
    public static class UiTheme
    {
        public const string AppName = "Sistema de Gestión de Pesaje";
        public const string AppShortName = "Gestión de Pesaje";
        public const string CompanyName = "Agroalianza Guárico C.A.";

        public static readonly Color Primary = Color.FromArgb(18, 42, 66);
        public static readonly Color PrimaryDark = Color.FromArgb(12, 28, 44);
        public static readonly Color Accent = Color.FromArgb(14, 122, 128);
        public static readonly Color AccentHover = Color.FromArgb(10, 98, 104);
        public static readonly Color Surface = Color.FromArgb(241, 244, 247);
        /// <summary>Fondo sutil para franjas de filtro (un tono bajo Surface).</summary>
        public static readonly Color FilterStrip = Color.FromArgb(234, 238, 242);
        public static readonly Color Card = Color.White;
        public static readonly Color Border = Color.FromArgb(210, 218, 226);
        public static readonly Color TextPrimary = Color.FromArgb(28, 35, 43);
        public static readonly Color TextMuted = Color.FromArgb(95, 108, 122);
        public static readonly Color Danger = Color.FromArgb(170, 55, 55);
        public static readonly Color DangerHover = Color.FromArgb(140, 40, 40);
        public static readonly Color Sidebar = Color.FromArgb(22, 48, 72);

        public static readonly Font FontRegular = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        public static readonly Font FontBold = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        public static readonly Font FontTitle = new Font("Segoe UI", 16F, FontStyle.Bold);
        public static readonly Font FontSubtitle = new Font("Segoe UI", 10F, FontStyle.Regular);
        public static readonly Font FontBrand = new Font("Segoe UI", 11F, FontStyle.Bold);
        public static readonly Font FontButton = new Font("Segoe UI", 9.5F, FontStyle.Bold);

        public static void ApplyForm(Form form)
        {
            if (form == null || form.IsDisposed) return;

            ApplyAppIcon(form);

            if (IsCustomLayoutForm(form))
            {
                _themeAppliedForms.Add(form);
                return;
            }

            if (form is principalFom)
            {
                foreach (Control control in form.Controls)
                {
                    if (control is MdiClient mdi)
                    {
                        mdi.BackColor = Surface;
                        break;
                    }
                }
                _themeAppliedForms.Add(form);
                return;
            }

            if (_themeAppliedForms.Contains(form)) return;

            form.BackgroundImage = null;
            form.BackgroundImageLayout = ImageLayout.None;
            form.BackColor = Surface;
            form.Font = FontRegular;
            form.ForeColor = TextPrimary;

            foreach (Control child in form.Controls)
                StyleControlTree(child);

            ApplyActionButtons(form);
            ApplyDialogChrome(form);
            _themeAppliedForms.Add(form);
        }

        /// <summary>
        /// Listados tipo catálogo (título + grilla + botones) con el mismo lenguaje que Recepción.
        /// </summary>
        public static void ApplyCatalogScreen(Form form)
        {
            if (form == null || form.IsDisposed) return;
            if (_catalogLayoutForms.Contains(form)) return;

            ApplyForm(form);

            DataGridView grid = form.Controls.OfType<DataGridView>().FirstOrDefault();
            if (grid == null) return;

            Label title = form.Controls.OfType<Label>()
                .OrderBy(l => l.Top)
                .FirstOrDefault(l => l.Font != null && l.Font.Size >= 12);

            if (title == null && !string.IsNullOrWhiteSpace(form.Text))
            {
                title = new Label
                {
                    Text = form.Text,
                    AutoSize = false
                };
            }

            List<Button> buttons = form.Controls.OfType<Button>()
                .OrderBy(b => b.Left)
                .ToList();

            var host = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = title != null ? 3 : 2,
                Padding = new Padding(16, 12, 16, 12),
                BackColor = Surface
            };
            host.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            int row = 0;
            if (title != null)
            {
                host.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
                title.AutoSize = false;
                title.Dock = DockStyle.Fill;
                title.TextAlign = ContentAlignment.MiddleLeft;
                title.Font = FontTitle;
                title.ForeColor = Primary;
                title.BackColor = Color.Transparent;
                host.Controls.Add(title, 0, row++);
            }

            host.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            StyleDataGridView(grid);
            grid.Dock = DockStyle.Fill;
            grid.Margin = new Padding(0);
            host.Controls.Add(grid, 0, row++);

            if (buttons.Count > 0)
            {
                host.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
                var bar = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    FlowDirection = FlowDirection.RightToLeft,
                    WrapContents = false,
                    Padding = new Padding(0, 8, 0, 0),
                    BackColor = Surface
                };
                foreach (Button button in buttons.OrderByDescending(b => b.Left))
                {
                    button.Margin = new Padding(8, 0, 0, 0);
                    bar.Controls.Add(button);
                }
                host.Controls.Add(bar, 0, row);
            }

            form.Controls.Add(host);
            form.FormBorderStyle = FormBorderStyle.Sizable;
            form.MaximizeBox = true;
            form.MinimizeBox = true;
            if (form.MinimumSize.Width < 640 || form.MinimumSize.Height < 400)
                form.MinimumSize = new Size(640, 400);
            if (form.MdiParent != null)
                form.WindowState = FormWindowState.Maximized;

            _catalogLayoutForms.Add(form);
            form.FormClosed += (s, e) =>
            {
                var closed = s as Form;
                if (closed != null)
                    _catalogLayoutForms.Remove(closed);
            };
        }

        /// <summary>Diálogos pequeños de selección (combo + etiqueta).</summary>
        public static void ApplyPickerDialog(Form form, Label title, ComboBox combo)
        {
            if (form == null) return;
            ApplyForm(form);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.MaximizeBox = false;
            form.MinimizeBox = false;
            form.StartPosition = FormStartPosition.CenterParent;
            form.ClientSize = new Size(380, 170);
            form.BackColor = Surface;

            if (title != null)
            {
                title.Font = FontTitle;
                title.ForeColor = Primary;
                title.BackColor = Color.Transparent;
                title.AutoSize = false;
                title.SetBounds(24, 18, 332, 32);
                title.TextAlign = ContentAlignment.MiddleLeft;
            }

            if (combo != null)
            {
                StyleComboBox(combo);
                combo.DropDownStyle = ComboBoxStyle.DropDownList;
                combo.SetBounds(24, 64, 332, 28);
            }
        }

        private static bool IsCustomLayoutForm(Form form)
        {
            return form is loginForm
                || form is recepcionForm
                || form is DespachoForm
                || form is productorNatural
                || form is nuevoUsuarioForm
                || form is personaJuridicaForm
                || form is asociacionForm
                || form is conductoresForm
                || form is tasaForm
                || form is reporteCostosForm
                || form is productorPickerForm;
        }

        public static void StylePrimaryButton(Button button)
        {
            StyleActionButton(button, Accent, Color.White, AccentHover, 0, GetActionIcon("guardar", Color.White));
        }

        public static void StyleSecondaryButton(Button button)
        {
            StyleActionButton(button, Card, TextPrimary, Color.FromArgb(230, 236, 241), 1, null, Border);
        }

        public static void StyleDangerButton(Button button)
        {
            StyleActionButton(button, Danger, Color.White, DangerHover, 0, GetActionIcon("eliminar", Color.White));
        }

        public static void StyleNewButton(Button button)
        {
            StyleActionButton(button, Accent, Color.White, AccentHover, 0, GetActionIcon("nuevo", Color.White));
        }

        public static void StyleAddButton(Button button)
        {
            StyleActionButton(button, Accent, Color.White, AccentHover, 0, GetActionIcon("agregar", Color.White));
        }

        public static void StyleEditButton(Button button)
        {
            StyleActionButton(button, Primary, Color.White, PrimaryDark, 0, GetActionIcon("editar", Color.White));
        }

        /// <summary>
        /// Botones planos modernos: Guardar / Nuevo / Cancelar / Eliminar / Agregar / Editar.
        /// </summary>
        public static void StyleActionButton(
            Button button,
            Color backColor,
            Color foreColor,
            Color hoverColor,
            int borderSize,
            Image icon = null,
            Color? borderColor = null)
        {
            if (button == null) return;

            bool wasTallIconButton =
                button.Height >= 45
                || button.ImageAlign == ContentAlignment.TopCenter
                || button.TextAlign == ContentAlignment.BottomCenter
                || button.Image != null;

            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = borderSize;
            if (borderSize > 0)
                button.FlatAppearance.BorderColor = borderColor ?? Border;
            button.FlatAppearance.MouseOverBackColor = hoverColor;
            button.FlatAppearance.MouseDownBackColor = hoverColor;
            button.BackColor = backColor;
            button.ForeColor = foreColor;
            button.Font = FontButton;
            button.Cursor = Cursors.Hand;
            button.Image = icon;
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.UseVisualStyleBackColor = false;
            button.AutoSize = false;
            // Sin icono: Overlay + MiddleCenter evita texto descentrado (ImageBeforeText/MiddleLeft
            // desplaza el layout aunque Image sea null). Padding L/R asimétrico solo con icono.
            if (icon == null)
            {
                button.ImageAlign = ContentAlignment.MiddleCenter;
                button.TextImageRelation = TextImageRelation.Overlay;
                button.Padding = new Padding(8, 0, 8, 0);
            }
            else
            {
                button.ImageAlign = ContentAlignment.MiddleLeft;
                button.TextImageRelation = TextImageRelation.ImageBeforeText;
                button.Padding = new Padding(10, 0, 12, 0);
            }

            if (wasTallIconButton && button.Height > 40)
            {
                int newHeight = 38;
                int bottom = button.Bottom;
                button.Height = newHeight;
                if (button.Parent != null)
                    button.Top = Math.Max(0, bottom - newHeight);
            }

            if (button.Height < 34)
                button.Height = 34;

            // Evitar que el texto se parta ("Agrega" / "r").
            int minWidth = icon == null ? 100 : 118;
            if (button.Width < minWidth)
                button.Width = minWidth;
        }

        public static void ApplyActionButtons(Control root)
        {
            if (root == null) return;

            foreach (Button button in FindButtons(root))
            {
                // Filtros compactos (p.ej. Buscar del Listado): no reaplicar icono/min-width 118.
                if (IsCompactActionButton(button))
                    continue;

                string key = ((button.Name ?? string.Empty) + " " + (button.Text ?? string.Empty)).ToLowerInvariant();

                if (key.Contains("eliminar") || key.Contains("anular") || key.Contains("borrar"))
                {
                    StyleDangerButton(button);
                    button.Tag = "action:eliminar";
                }
                else if (key.Contains("agregar"))
                {
                    StyleAddButton(button);
                    button.Tag = "action:agregar";
                }
                else if (key.Contains("editar"))
                {
                    StyleEditButton(button);
                    button.Tag = "action:editar";
                }
                else if (key.Contains("nuevo"))
                {
                    StyleNewButton(button);
                    button.Tag = "action:nuevo";
                }
                else if (key.Contains("guardar") || key.Contains("aceptar") || key.Contains("imprimir") || key.Contains("exportar") || key.Contains("buscar") || key.Contains("login") || key.Contains("filtrar"))
                {
                    var iconKey = key.Contains("buscar") || key.Contains("filtrar") ? "buscar"
                        : key.Contains("imprimir") ? "imprimir"
                        : key.Contains("exportar") ? "exportar"
                        : key.Contains("login") ? "login"
                        : "guardar";
                    StyleActionButton(button, Accent, Color.White, AccentHover, 0, GetActionIcon(iconKey, Color.White));
                    button.Tag = "action:guardar";
                }
                else if (key.Contains("cancelar") || key.Contains("cerrar") || key.Contains("salir"))
                {
                    StyleActionButton(button, Card, TextPrimary, Color.FromArgb(230, 236, 241), 1, GetActionIcon("cancelar", TextPrimary), Border);
                    button.Tag = "action:cancelar";
                }
                else if (key.Contains("limpiar") || key.Contains("todos") || key.Contains("entrada") || key.Contains("salida"))
                {
                    StyleSecondaryButton(button);
                    button.Tag = "action:secundario";
                }
            }

            SpaceActionButtonClusters(root);
        }

        /// <summary>Tag para botones de filtro compactos que no deben recibir icono de acción.</summary>
        public const string CompactActionTag = "ui:compact-action";

        public static bool IsCompactActionButton(Button button)
        {
            return button != null
                && string.Equals(button.Tag as string, CompactActionTag, StringComparison.Ordinal);
        }

        /// <summary>Botón primario compacto (sin icono) para franjas de filtro estrechas.</summary>
        public static void StyleCompactPrimaryButton(Button button, string text, int width = 100, int height = 32)
        {
            if (button == null) return;
            StyleActionButton(button, Accent, Color.White, AccentHover, 0, null);
            AplicarEstiloCompacto(button, text, width, height);
            button.ForeColor = Color.White;
        }

        /// <summary>Botón secundario compacto para campos con Buscar inline.</summary>
        public static void StyleCompactSecondaryButton(Button button, string text, int width = 88, int height = 30)
        {
            if (button == null) return;
            StyleActionButton(button, Card, TextPrimary, Color.FromArgb(230, 236, 241), 1, null, Border);
            AplicarEstiloCompacto(button, text, width, height);
            button.ForeColor = TextPrimary;
        }

        private static void AplicarEstiloCompacto(Button button, string text, int width, int height)
        {
            button.Tag = CompactActionTag;
            button.Image = null;
            button.ImageAlign = ContentAlignment.MiddleCenter;
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.TextImageRelation = TextImageRelation.Overlay;
            button.Padding = new Padding(6, 0, 6, 0);
            button.Text = text ?? string.Empty;
            button.AutoSize = false;
            button.Size = new Size(width, height);
            button.MinimumSize = new Size(width, height);
            button.MaximumSize = new Size(width, height);
            button.Dock = DockStyle.None;
            button.UseVisualStyleBackColor = false;
        }

        private static void SpaceActionButtonClusters(Control root)
        {
            if (root == null) return;

            var parents = new HashSet<Control>();
            foreach (Button button in FindButtons(root))
            {
                if (IsActionButton(button) && button.Parent != null)
                    parents.Add(button.Parent);
            }

            foreach (Control parent in parents)
            {
                var rowButtons = parent.Controls.OfType<Button>()
                    .Where(IsActionButton)
                    .OrderBy(b => b.Top)
                    .ThenBy(b => b.Left)
                    .ToList();

                if (rowButtons.Count < 2) continue;

                // Agrupar por fila (misma altura aproximada).
                var rows = new List<List<Button>>();
                foreach (Button button in rowButtons)
                {
                    List<Button> row = rows.FirstOrDefault(r => Math.Abs(r[0].Top - button.Top) <= 18);
                    if (row == null)
                    {
                        row = new List<Button>();
                        rows.Add(row);
                    }
                    row.Add(button);
                }

                foreach (List<Button> row in rows)
                {
                    // Solo reordenar barras Guardar/Eliminar/Cancelar/Nuevo.
                    // No mover Agregar/Editar (van junto a combos).
                    var actionable = row.Where(b =>
                    {
                        string tag = b.Tag as string ?? string.Empty;
                        return tag == "action:guardar"
                            || tag == "action:eliminar"
                            || tag == "action:cancelar"
                            || tag == "action:nuevo";
                    }).OrderBy(b => b.Left).ToList();

                    if (actionable.Count < 2) continue;

                    bool clustered = false;
                    for (int i = 1; i < actionable.Count; i++)
                    {
                        if (actionable[i].Left - actionable[i - 1].Right <= 28)
                        {
                            clustered = true;
                            break;
                        }
                    }
                    if (!clustered) continue;

                    const int gap = 14;
                    const int width = 120;
                    const int height = 38;
                    int total = actionable.Count * width + (actionable.Count - 1) * gap;
                    int start = Math.Max(12, (parent.ClientSize.Width - total) / 2);
                    int top = actionable.Max(b => b.Bottom) - height;
                    if (top < 0) top = actionable[0].Top;

                    for (int i = 0; i < actionable.Count; i++)
                    {
                        Button button = actionable[i];
                        button.SetBounds(start + i * (width + gap), top, width, height);
                    }
                }
            }
        }

        private static bool IsActionButton(Button button)
        {
            if (button == null) return false;
            string tag = button.Tag as string;
            if (!string.IsNullOrEmpty(tag) && tag.StartsWith("action:", StringComparison.Ordinal))
                return true;

            string key = ((button.Name ?? string.Empty) + " " + (button.Text ?? string.Empty)).ToLowerInvariant();
            return key.Contains("guardar") || key.Contains("cancelar") || key.Contains("eliminar")
                || key.Contains("nuevo") || key.Contains("anular") || key.Contains("cerrar")
                || key.Contains("agregar") || key.Contains("editar");
        }

        private static readonly Dictionary<string, Image> _actionIcons = new Dictionary<string, Image>(StringComparer.OrdinalIgnoreCase);

        public static Image GetActionIconPublic(string kind, Color color)
        {
            return GetActionIcon(kind, color);
        }

        private static Image GetActionIcon(string kind, Color color)
        {
            string cacheKey = kind + "|" + color.ToArgb();
            Image cached;
            if (_actionIcons.TryGetValue(cacheKey, out cached))
                return cached;

            Image icon = CreateActionGlyph(kind, color);
            _actionIcons[cacheKey] = icon;
            return icon;
        }

        private static Image CreateActionGlyph(string kind, Color color)
        {
            var bmp = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (var pen = new Pen(color, 1.8f))
                using (var brush = new SolidBrush(color))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    kind = (kind ?? string.Empty).ToLowerInvariant();

                    if (kind == "guardar" || kind == "aceptar")
                    {
                        // Disco / save
                        g.DrawRectangle(pen, 3, 2, 10, 12);
                        g.FillRectangle(brush, 5, 2, 6, 4);
                        g.DrawLine(pen, 5, 10, 11, 10);
                        g.DrawLine(pen, 5, 12, 9, 12);
                    }
                    else if (kind == "agregar" || kind == "nuevo")
                    {
                        g.DrawLine(pen, 8, 3, 8, 13);
                        g.DrawLine(pen, 3, 8, 13, 8);
                    }
                    else if (kind == "editar")
                    {
                        // Lápiz
                        g.DrawLine(pen, 3, 13, 7, 12);
                        g.DrawLine(pen, 7, 12, 13, 4);
                        g.DrawLine(pen, 11, 3, 13, 5);
                        g.DrawLine(pen, 4, 11, 5, 13);
                    }
                    else if (kind == "cancelar" || kind == "cerrar" || kind == "salir")
                    {
                        g.DrawLine(pen, 4, 4, 12, 12);
                        g.DrawLine(pen, 12, 4, 4, 12);
                    }
                    else if (kind == "eliminar" || kind == "anular" || kind == "borrar")
                    {
                        g.DrawLine(pen, 4, 5, 12, 5);
                        g.DrawLine(pen, 6, 5, 6, 3);
                        g.DrawLine(pen, 10, 5, 10, 3);
                        g.DrawLine(pen, 5, 5, 5, 13);
                        g.DrawLine(pen, 11, 5, 11, 13);
                        g.DrawLine(pen, 5, 13, 11, 13);
                        g.DrawLine(pen, 7, 7, 7, 11);
                        g.DrawLine(pen, 9, 7, 9, 11);
                    }
                    else if (kind == "buscar")
                    {
                        g.DrawEllipse(pen, 3, 3, 8, 8);
                        g.DrawLine(pen, 10, 10, 13, 13);
                    }
                    else if (kind == "imprimir")
                    {
                        g.DrawRectangle(pen, 4, 2, 8, 4);
                        g.FillRectangle(brush, 3, 6, 10, 6);
                        g.DrawRectangle(pen, 5, 10, 6, 4);
                    }
                    else if (kind == "exportar")
                    {
                        g.DrawLine(pen, 8, 3, 8, 11);
                        g.DrawLine(pen, 5, 6, 8, 3);
                        g.DrawLine(pen, 11, 6, 8, 3);
                        g.DrawLine(pen, 4, 13, 12, 13);
                    }
                    else if (kind == "login")
                    {
                        g.DrawEllipse(pen, 5, 2, 6, 6);
                        g.DrawArc(pen, 3, 8, 10, 8, 200, 140);
                    }
                    else
                    {
                        g.FillEllipse(brush, 5, 5, 6, 6);
                    }
                }
            }
            return bmp;
        }

        private static IEnumerable<Button> FindButtons(Control root)
        {
            var list = new List<Button>();
            CollectButtons(root, list);
            return list;
        }

        private static void CollectButtons(Control control, List<Button> list)
        {
            if (control == null) return;
            if (control is Button button)
                list.Add(button);
            foreach (Control child in control.Controls)
                CollectButtons(child, list);
        }

        public static void StyleNavButton(Button button)
        {
            if (button == null) return;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(32, 68, 98);
            button.BackColor = Color.Transparent;
            button.ForeColor = Color.White;
            button.Font = FontBold;
            button.Cursor = Cursors.Hand;
            button.Image = null;
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.UseVisualStyleBackColor = false;
        }

        public static void StyleTextBox(TextBox textBox)
        {
            if (textBox == null) return;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.BackColor = Card;
            textBox.ForeColor = TextPrimary;
            textBox.Font = FontRegular;
        }

        public static void StyleComboBox(ComboBox comboBox)
        {
            if (comboBox == null) return;
            comboBox.FlatStyle = FlatStyle.Standard;
            comboBox.BackColor = Card;
            comboBox.ForeColor = TextPrimary;
            comboBox.Font = FontRegular;
        }

        public static void StyleDateTimePicker(DateTimePicker picker)
        {
            if (picker == null) return;
            picker.CalendarForeColor = TextPrimary;
            picker.CalendarMonthBackground = Card;
            picker.ForeColor = TextPrimary;
            picker.Font = FontRegular;
            try { picker.BackColor = Card; }
            catch { /* algunos entornos no permiten BackColor en DTP */ }
        }

        /// <summary>
        /// Marco de 1px (color Border) para que el campo se lea sobre Surface sin sombra.
        /// </summary>
        public static Panel WrapInputFrame(Control field)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            var frame = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(1),
                BackColor = Border,
                Margin = new Padding(0)
            };
            field.Dock = DockStyle.Fill;
            field.Margin = new Padding(0);
            frame.Controls.Add(field);
            return frame;
        }

        public static Image CreateBrandMark(int size)
        {
            var bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                using (var brush = new SolidBrush(Accent))
                    g.FillEllipse(brush, 2, 2, size - 4, size - 4);

                using (var pen = new Pen(Color.White, Math.Max(2f, size / 18f)))
                {
                    int m = size / 5;
                    g.DrawEllipse(pen, m, m, size - 2 * m, size - 2 * m);
                    g.DrawLine(pen, size / 2f, m + 2, size / 2f, size - m - 2);
                    g.DrawLine(pen, m + 2, size / 2f, size - m - 2, size / 2f);
                }
            }
            return bmp;
        }

        private static Icon _appIcon;
        private static readonly HashSet<Form> _iconAppliedForms = new HashSet<Form>();
        private static readonly HashSet<Form> _themeAppliedForms = new HashSet<Form>();
        private static readonly HashSet<Form> _catalogLayoutForms = new HashSet<Form>();
        private static bool _globalIconHookInstalled;

        /// <summary>
        /// Icono de aplicación (barra de tareas / ventanas minimizadas).
        /// </summary>
        public static Icon GetAppIcon()
        {
            if (_appIcon != null)
                return _appIcon;

            try
            {
                string[] candidates =
                {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.ico"),
                    Path.Combine(Application.StartupPath, "logo.ico"),
                    Path.Combine(Path.GetDirectoryName(Application.ExecutablePath) ?? string.Empty, "logo.ico")
                };

                foreach (string path in candidates)
                {
                    if (!string.IsNullOrEmpty(path) && File.Exists(path))
                    {
                        // Clone so file isn't locked exclusively.
                        using (var loaded = new Icon(path))
                            _appIcon = (Icon)loaded.Clone();
                        return _appIcon;
                    }
                }
            }
            catch
            {
                // Fallback generated below.
            }

            _appIcon = CreateGeneratedAppIcon();
            return _appIcon;
        }

        public static void ApplyAppIcon(Form form)
        {
            if (form == null) return;
            try
            {
                form.Icon = GetAppIcon();
                form.ShowIcon = true;
            }
            catch
            {
                // Ignore icon assignment failures on remote/locked environments.
            }
        }

        /// <summary>
        /// Aplica icono y botones de acción modernos a cualquier formulario abierto.
        /// </summary>
        public static void InstallGlobalFormIcon()
        {
            if (_globalIconHookInstalled) return;
            _globalIconHookInstalled = true;

            Application.Idle += (sender, e) =>
            {
                Form[] open;
                try
                {
                    open = new Form[Application.OpenForms.Count];
                    ((System.Collections.ICollection)Application.OpenForms).CopyTo(open, 0);
                }
                catch
                {
                    return;
                }

                foreach (Form form in open)
                {
                    if (form == null || form.IsDisposed) continue;

                    if (!_iconAppliedForms.Contains(form))
                    {
                        ApplyAppIcon(form);
                        _iconAppliedForms.Add(form);
                        form.FormClosed += (s, args) =>
                        {
                            var closed = s as Form;
                            if (closed != null)
                            {
                                _iconAppliedForms.Remove(closed);
                                _themeAppliedForms.Remove(closed);
                                _catalogLayoutForms.Remove(closed);
                            }
                        };
                    }

                    if (!_themeAppliedForms.Contains(form) && !(form is principalFom))
                        ApplyForm(form);
                }
            };
        }

        /// <summary>
        /// Limpia fondos antiguos y tipografía de títulos en diálogos CRUD.
        /// </summary>
        public static void ApplyDialogChrome(Form form)
        {
            if (form == null || form.IsMdiContainer) return;

            form.BackgroundImage = null;
            form.BackgroundImageLayout = ImageLayout.None;
            if (form.BackColor == SystemColors.Control || form.BackColor == Color.White || form.BackgroundImage != null)
                form.BackColor = Surface;

            foreach (Control child in form.Controls)
            {
                if (child is Label label && label.Font != null && label.Font.Size >= 14)
                {
                    label.Font = FontTitle;
                    label.ForeColor = Primary;
                    label.BackColor = Color.Transparent;
                }
            }
        }

        private static Icon CreateGeneratedAppIcon()
        {
            using (Bitmap bmp = new Bitmap(64, 64))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                using (var bg = new SolidBrush(Primary))
                    g.FillRectangle(bg, 0, 0, 64, 64);

                using (var accent = new SolidBrush(Accent))
                {
                    // Silo body
                    g.FillRectangle(accent, 20, 18, 24, 30);
                    // Dome
                    g.FillEllipse(accent, 18, 10, 28, 18);
                }

                using (var white = new Pen(Color.White, 2.5f))
                {
                    // Scale beam
                    g.DrawLine(white, 14, 48, 50, 48);
                    g.DrawLine(white, 32, 36, 32, 48);
                    g.DrawEllipse(white, 12, 50, 10, 8);
                    g.DrawEllipse(white, 42, 50, 10, 8);
                }

                IntPtr hIcon = bmp.GetHicon();
                using (Icon temp = Icon.FromHandle(hIcon))
                    return (Icon)temp.Clone();
            }
        }

        public static void StyleDataGridView(DataGridView grid)
        {
            if (grid == null) return;

            grid.BackgroundColor = Card;
            grid.BorderStyle = BorderStyle.None;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Primary;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = FontBold;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Primary;
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            if (grid.ColumnHeadersHeight < 32)
                grid.ColumnHeadersHeight = 32;
            grid.DefaultCellStyle.Font = FontRegular;
            grid.DefaultCellStyle.BackColor = Card;
            grid.DefaultCellStyle.ForeColor = TextPrimary;
            grid.DefaultCellStyle.SelectionBackColor = Accent;
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);
            grid.RowHeadersVisible = false;
            grid.RowTemplate.Height = 28;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(236, 242, 245);
            grid.GridColor = Border;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        }

        private static void StyleControlTree(Control control)
        {
            if (control == null) return;

            string typeName = control.GetType().Name;
            if (typeName.IndexOf("ReportViewer", StringComparison.OrdinalIgnoreCase) >= 0)
                return;

            if (control is Button)
            {
                // Los botones de acción se estilizan en ApplyActionButtons.
            }
            else if (control is TextBox textBox)
            {
                bool readOnly = textBox.ReadOnly;
                StyleTextBox(textBox);
                if (readOnly)
                {
                    textBox.ReadOnly = true;
                    textBox.BackColor = FilterStrip;
                }
            }
            else if (control is ComboBox comboBox)
            {
                StyleComboBox(comboBox);
            }
            else if (control is DateTimePicker picker)
            {
                StyleDateTimePicker(picker);
            }
            else if (control is DataGridView grid)
            {
                StyleDataGridView(grid);
            }
            else if (control is GroupBox groupBox)
            {
                groupBox.Font = FontBold;
                groupBox.ForeColor = Primary;
                groupBox.BackColor = Card;
            }
            else if (control is TabControl tabControl)
            {
                tabControl.Font = FontBold;
                try { tabControl.BackColor = Surface; }
                catch { /* TabControl.BackColor no siempre es asignable */ }
            }
            else if (control is TabPage tabPage)
            {
                tabPage.BackColor = Surface;
                tabPage.UseVisualStyleBackColor = false;
            }
            else if (control is Panel panel)
            {
                if (panel.BackgroundImage != null)
                    panel.BackgroundImage = null;
                if (panel.BackColor == SystemColors.Control || panel.BackColor == SystemColors.InactiveBorder)
                    panel.BackColor = Surface;
            }
            else if (control is Label label)
            {
                bool onDark = IsDarkSurface(label.BackColor) || (label.Parent != null && IsDarkSurface(label.Parent.BackColor));
                if (!onDark && label.ForeColor != Color.White)
                {
                    label.BackColor = Color.Transparent;
                    if (label.Font != null && label.Font.Size >= 14)
                    {
                        label.Font = FontTitle;
                        label.ForeColor = Primary;
                    }
                    else
                    {
                        label.Font = FontRegular;
                        label.ForeColor = TextMuted;
                    }
                }
            }
            else if (control is CheckBox || control is RadioButton)
            {
                control.Font = FontRegular;
                control.ForeColor = TextPrimary;
                if (control.BackColor == SystemColors.Control)
                    control.BackColor = Color.Transparent;
            }

            foreach (Control child in control.Controls)
                StyleControlTree(child);
        }

        private static bool IsDarkSurface(Color color)
        {
            if (color.A == 0) return false;
            return color.GetBrightness() < 0.35f;
        }
    }
}
