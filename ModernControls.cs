using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

namespace MyMaintenanceApp
{
    public class ModernButton : Button
    {
        private int borderRadius = 8;
        private Color hoverColor;
        private Color normalColor;
        private bool isHovered = false;
        private Timer animationTimer;
        private float animationProgress = 0f;

        public int BorderRadius
        {
            get => borderRadius;
            set { borderRadius = value; Invalidate(); }
        }

        public ModernButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                    ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            Cursor = Cursors.Hand;
            
            animationTimer = new Timer { Interval = 16 };
            animationTimer.Tick += AnimationTimer_Tick;
            
            ApplyTheme();
            ThemeManager.ThemeChanged += (s, e) => ApplyTheme();
        }

        private void ApplyTheme()
        {
            var theme = ThemeManager.Current;
            normalColor = theme.ButtonBackground;
            hoverColor = theme.ButtonHover;
            BackColor = normalColor;
            ForeColor = theme.TextPrimary;
            FlatAppearance.BorderColor = theme.BorderColor;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            StartAnimation();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            StartAnimation();
        }

        private void StartAnimation()
        {
            animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (isHovered && animationProgress < 1f)
            {
                animationProgress += 0.1f;
            }
            else if (!isHovered && animationProgress > 0f)
            {
                animationProgress -= 0.1f;
            }
            else
            {
                animationTimer.Stop();
                return;
            }

            animationProgress = Math.Max(0f, Math.Min(1f, animationProgress));
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Interpolate colors for smooth animation
            var currentColor = InterpolateColor(normalColor, hoverColor, animationProgress);
            
            using (var path = GetRoundedRectanglePath(ClientRectangle, borderRadius))
            using (var brush = new SolidBrush(currentColor))
            {
                e.Graphics.FillPath(brush, path);
            }

            // Draw text
            var textRect = ClientRectangle;
            var textFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
            TextRenderer.DrawText(e.Graphics, Text, Font, textRect, ForeColor, textFlags);
        }

        private Color InterpolateColor(Color color1, Color color2, float progress)
        {
            var r = (int)(color1.R + (color2.R - color1.R) * progress);
            var g = (int)(color1.G + (color2.G - color1.G) * progress);
            var b = (int)(color1.B + (color2.B - color1.B) * progress);
            return Color.FromArgb(r, g, b);
        }

        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            var diameter = radius * 2;
            var size = new Size(diameter, diameter);
            var arc = new Rectangle(rect.Location, size);

            // Top left arc
            path.AddArc(arc, 180, 90);

            // Top right arc
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom right arc
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom left arc
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
    }

    public class ModernCheckBox : CheckBox
    {
        private int checkBoxSize = 18;
        private int borderRadius = 4;
        private bool isHovered = false;
        private Timer animationTimer;
        private float animationProgress = 0f;

        public int CheckBoxSize
        {
            get => checkBoxSize;
            set { checkBoxSize = value; Invalidate(); }
        }

        public int BorderRadius
        {
            get => borderRadius;
            set { borderRadius = value; Invalidate(); }
        }

        public ModernCheckBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                    ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            Cursor = Cursors.Hand;
            
            animationTimer = new Timer { Interval = 16 };
            animationTimer.Tick += AnimationTimer_Tick;
            
            ApplyTheme();
            ThemeManager.ThemeChanged += (s, e) => ApplyTheme();
        }

        private void ApplyTheme()
        {
            var theme = ThemeManager.Current;
            ForeColor = theme.TextPrimary;
            BackColor = theme.PrimaryBackground;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            StartAnimation();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            StartAnimation();
        }

        private void StartAnimation()
        {
            animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (isHovered && animationProgress < 1f)
            {
                animationProgress += 0.15f;
            }
            else if (!isHovered && animationProgress > 0f)
            {
                animationProgress -= 0.15f;
            }
            else
            {
                animationTimer.Stop();
                return;
            }

            animationProgress = Math.Max(0f, Math.Min(1f, animationProgress));
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var theme = ThemeManager.Current;
            var checkBoxRect = new Rectangle(0, (Height - checkBoxSize) / 2, checkBoxSize, checkBoxSize);
            var textRect = new Rectangle(checkBoxSize + 8, 0, Width - checkBoxSize - 8, Height);

            // Draw checkbox background
            using (var path = GetRoundedRectanglePath(checkBoxRect, borderRadius))
            {
                var bgColor = Checked ? theme.AccentColor : theme.ButtonBackground;
                if (isHovered)
                {
                    var hoverColor = Checked ? 
                        Color.FromArgb(Math.Min(255, theme.AccentColor.R + 20), 
                                     Math.Min(255, theme.AccentColor.G + 20), 
                                     Math.Min(255, theme.AccentColor.B + 20)) :
                        theme.ButtonHover;
                    bgColor = InterpolateColor(bgColor, hoverColor, animationProgress);
                }
                
                using (var brush = new SolidBrush(bgColor))
                {
                    e.Graphics.FillPath(brush, path);
                }

                // Draw border
                using (var pen = new Pen(theme.BorderColor, 1))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }

            // Draw checkmark
            if (Checked)
            {
                var checkColor = theme.IsDark ? Color.White : Color.White;
                using (var pen = new Pen(checkColor, 2))
                {
                    var checkPoints = new Point[]
                    {
                        new Point(checkBoxRect.X + 4, checkBoxRect.Y + checkBoxRect.Height / 2),
                        new Point(checkBoxRect.X + checkBoxRect.Width / 2, checkBoxRect.Y + checkBoxRect.Height - 5),
                        new Point(checkBoxRect.X + checkBoxRect.Width - 3, checkBoxRect.Y + 3)
                    };
                    e.Graphics.DrawLines(pen, checkPoints);
                }
            }

            // Draw text
            var textFlags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter;
            TextRenderer.DrawText(e.Graphics, Text, Font, textRect, ForeColor, textFlags);
        }

        private Color InterpolateColor(Color color1, Color color2, float progress)
        {
            var r = (int)(color1.R + (color2.R - color1.R) * progress);
            var g = (int)(color1.G + (color2.G - color1.G) * progress);
            var b = (int)(color1.B + (color2.B - color1.B) * progress);
            return Color.FromArgb(r, g, b);
        }

        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            var diameter = radius * 2;
            var size = new Size(diameter, diameter);
            var arc = new Rectangle(rect.Location, size);

            if (radius == 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            // Top left arc
            path.AddArc(arc, 180, 90);

            // Top right arc
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom right arc
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom left arc
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
    }

    public class ModernTextBox : TextBox
    {
        private int borderRadius = 6;
        private bool isFocused = false;

        public int BorderRadius
        {
            get => borderRadius;
            set { borderRadius = value; Invalidate(); }
        }

        public ModernTextBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                    ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            
            BorderStyle = BorderStyle.None;
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            
            ApplyTheme();
            ThemeManager.ThemeChanged += (s, e) => ApplyTheme();
        }

        private void ApplyTheme()
        {
            var theme = ThemeManager.Current;
            BackColor = theme.SecondaryBackground;
            ForeColor = theme.TextPrimary;
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            isFocused = true;
            Invalidate();
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            isFocused = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            
            var theme = ThemeManager.Current;
            var borderColor = isFocused ? theme.AccentColor : theme.BorderColor;
            var borderWidth = isFocused ? 2 : 1;
            
            using (var path = GetRoundedRectanglePath(ClientRectangle, borderRadius))
            using (var brush = new SolidBrush(BackColor))
            using (var pen = new Pen(borderColor, borderWidth))
            {
                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawPath(pen, path);
            }

            // Draw text manually
            var textRect = new Rectangle(8, 0, Width - 16, Height);
            var textFlags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter;
            var displayText = string.IsNullOrEmpty(Text) ? "" : Text;
            TextRenderer.DrawText(e.Graphics, displayText, Font, textRect, ForeColor, textFlags);
        }

        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            var diameter = radius * 2;
            var size = new Size(diameter, diameter);
            var arc = new Rectangle(rect.Location, size);

            if (radius == 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            // Top left arc
            path.AddArc(arc, 180, 90);

            // Top right arc
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom right arc
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom left arc
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
    }
}