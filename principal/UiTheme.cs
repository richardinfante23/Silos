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
            if (form == null) return;

            form.BackgroundImage = null;
            form.BackgroundImageLayout = ImageLayout.None;
            form.BackColor = Surface;
            form.Font = FontRegular;
            form.ForeColor = TextPrimary;
            ApplyAppIcon(form);

            if (form.IsMdiContainer)
            {
                foreach (Control control in form.Controls)
                {
                    if (control is MdiClient mdi)
                    {
                        mdi.BackColor = Surface;
                        break;
                    }
                }
            }

            foreach (Control child in form.Controls)
                StyleControlTree(child);

            ApplyActionButtons(form);
            _themeAppliedForms.Add(form);
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
            button.ImageAlign = ContentAlignment.MiddleLeft;
            button.TextImageRelation = TextImageRelation.ImageBeforeText;
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.UseVisualStyleBackColor = false;
            button.AutoSize = false;
            button.Padding = new Padding(10, 0, 12, 0);

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
                else if (key.Contains("guardar") || key.Contains("aceptar") || key.Contains("imprimir") || key.Contains("exportar") || key.Contains("buscar") || key.Contains("login"))
                {
                    var iconKey = key.Contains("buscar") ? "buscar"
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
            }

            SpaceActionButtonClusters(root);
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
                            }
                        };
                    }

                    if (!_themeAppliedForms.Contains(form))
                    {
                        // Evitar repintar el shell principal (tiene botones de navegación propios).
                        if (!(form is principalFom))
                        {
                            ApplyDialogChrome(form);
                            ApplyActionButtons(form);
                        }
                        _themeAppliedForms.Add(form);
                    }
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

        private static void StyleControlTree(Control control)
        {
            if (control == null) return;

            if (control is Button)
            {
                // Los botones de acción se estilizan en ApplyActionButtons.
            }
            else if (control is TextBox || control is ComboBox || control is DateTimePicker)
            {
                control.Font = FontRegular;
                control.ForeColor = TextPrimary;
            }
            else if (control is DataGridView grid)
            {
                grid.BackgroundColor = Card;
                grid.BorderStyle = BorderStyle.None;
                grid.EnableHeadersVisualStyles = false;
                grid.ColumnHeadersDefaultCellStyle.BackColor = Primary;
                grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                grid.ColumnHeadersDefaultCellStyle.Font = FontBold;
                grid.DefaultCellStyle.Font = FontRegular;
                grid.DefaultCellStyle.SelectionBackColor = Accent;
                grid.DefaultCellStyle.SelectionForeColor = Color.White;
                grid.RowHeadersVisible = false;
                grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(236, 242, 245);
            }
            else if (control is GroupBox || control is Panel || control is TabControl)
            {
                if (!(control is Panel) || control.BackColor == SystemColors.Control || control.BackColor == SystemColors.InactiveBorder)
                    control.BackColor = Surface;
            }

            foreach (Control child in control.Controls)
                StyleControlTree(child);
        }
    }
}
