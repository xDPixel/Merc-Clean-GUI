using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Linq;

namespace MyMaintenanceApp
{
    public class ThemeSwitcher : UserControl
    {
        private ModernButton themeButton;
        private ContextMenuStrip themeMenu;
        private bool isMenuOpen = false;

        public ThemeSwitcher()
        {
            InitializeComponent();
            SetupThemeMenu();
            ApplyTheme();
            ThemeManager.ThemeChanged += (s, e) => ApplyTheme();
        }

        private void InitializeComponent()
        {
            Size = new Size(120, 35);
            
            themeButton = new ModernButton
            {
                Text = "🎨 Theme",
                Dock = DockStyle.Fill,
                BorderRadius = 8,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular)
            };
            
            themeButton.Click += ThemeButton_Click;
            Controls.Add(themeButton);
        }

        private void SetupThemeMenu()
        {
            themeMenu = new ContextMenuStrip
            {
                ShowImageMargin = false,
                ShowCheckMargin = true,
                RenderMode = ToolStripRenderMode.Professional
            };

            // Dark themes
            var darkHeader = new ToolStripLabel("Dark Themes")
            {
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            themeMenu.Items.Add(darkHeader);
            themeMenu.Items.Add(new ToolStripSeparator());

            AddThemeMenuItem("🌙 Dark Modern", ThemeType.DarkModern);
            AddThemeMenuItem("💙 Dark Blue", ThemeType.DarkBlue);
            AddThemeMenuItem("💜 Dark Purple", ThemeType.DarkPurple);

            themeMenu.Items.Add(new ToolStripSeparator());

            // Light themes
            var lightHeader = new ToolStripLabel("Light Themes")
            {
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            themeMenu.Items.Add(lightHeader);
            themeMenu.Items.Add(new ToolStripSeparator());

            AddThemeMenuItem("☀️ Light Modern", ThemeType.LightModern);
            AddThemeMenuItem("🔵 Light Blue", ThemeType.LightBlue);
            AddThemeMenuItem("🟢 Light Green", ThemeType.LightGreen);

            themeMenu.Renderer = new ModernMenuRenderer();
        }

        private void AddThemeMenuItem(string text, ThemeType themeType)
        {
            var menuItem = new ToolStripMenuItem(text)
            {
                Tag = themeType,
                Checked = ThemeManager.CurrentTheme == themeType,
                CheckOnClick = false
            };
            
            menuItem.Click += (s, e) =>
            {
                ThemeManager.CurrentTheme = themeType;
                UpdateMenuChecks();
            };
            
            themeMenu.Items.Add(menuItem);
        }

        private void UpdateMenuChecks()
        {
            foreach (ToolStripMenuItem item in themeMenu.Items.OfType<ToolStripMenuItem>())
            {
                if (item.Tag is ThemeType themeType)
                {
                    item.Checked = ThemeManager.CurrentTheme == themeType;
                }
            }
        }

        private void ThemeButton_Click(object sender, EventArgs e)
        {
            if (!isMenuOpen)
            {
                var buttonLocation = themeButton.PointToScreen(new Point(0, themeButton.Height));
                themeMenu.Show(buttonLocation);
                isMenuOpen = true;
                themeMenu.Closed += (s, args) => isMenuOpen = false;
            }
        }

        private void ApplyTheme()
        {
            var theme = ThemeManager.Current;
            BackColor = theme.PrimaryBackground;
            
            if (themeMenu != null)
            {
                themeMenu.BackColor = theme.SecondaryBackground;
                themeMenu.ForeColor = theme.TextPrimary;
                
                foreach (ToolStripItem item in themeMenu.Items)
                {
                    if (item is ToolStripLabel label && (label.Text.Contains("Dark Themes") || label.Text.Contains("Light Themes")))
                    {
                        item.ForeColor = theme.TextSecondary;
                        item.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    }
                    else if (item is ToolStripMenuItem menuItem)
                    {
                        item.ForeColor = theme.TextPrimary;
                    }
                    item.BackColor = theme.SecondaryBackground;
                }
                
                // Force menu to refresh
                themeMenu.Invalidate();
            }
        }
    }

    public class ModernMenuRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            var theme = ThemeManager.Current;
            
            if (e.Item.Selected)
            {
                using (var brush = new SolidBrush(theme.ButtonHover))
                {
                    e.Graphics.FillRectangle(brush, e.Item.ContentRectangle);
                }
            }
            else
            {
                using (var brush = new SolidBrush(theme.SecondaryBackground))
                {
                    e.Graphics.FillRectangle(brush, e.Item.ContentRectangle);
                }
            }
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            var theme = ThemeManager.Current;
            e.TextColor = theme.TextPrimary;
            base.OnRenderItemText(e);
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            var theme = ThemeManager.Current;
            using (var pen = new Pen(theme.BorderColor))
            {
                var rect = e.Item.ContentRectangle;
                e.Graphics.DrawLine(pen, rect.Left + 5, rect.Top + rect.Height / 2, 
                                   rect.Right - 5, rect.Top + rect.Height / 2);
            }
        }

        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            var theme = ThemeManager.Current;
            var checkRect = new Rectangle(e.ImageRectangle.X, e.ImageRectangle.Y + 2, 12, 12);
            
            using (var brush = new SolidBrush(theme.AccentColor))
            using (var pen = new Pen(Color.White, 2))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillEllipse(brush, checkRect);
                
                // Draw checkmark
                var checkPoints = new Point[]
                {
                    new Point(checkRect.X + 3, checkRect.Y + 6),
                    new Point(checkRect.X + 5, checkRect.Y + 8),
                    new Point(checkRect.X + 9, checkRect.Y + 4)
                };
                e.Graphics.DrawLines(pen, checkPoints);
            }
        }
    }
}