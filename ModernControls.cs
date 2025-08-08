using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class ModernButton : Button
{
    public int BorderRadius { get; set; } = 10;

    public ModernButton()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
    }

    public void ApplyTheme(Theme theme)
    {
        BackColor = theme.ButtonBackground;
        ForeColor = theme.ButtonText;
        FlatAppearance.MouseOverBackColor = theme.ButtonHover;
        FlatAppearance.MouseDownBackColor = theme.ButtonPressed;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Create rounded rectangle path
        GraphicsPath path = CreateRoundedRectangle(ClientRectangle, BorderRadius);

        // Fill background
        using (SolidBrush brush = new SolidBrush(BackColor))
        {
            g.FillPath(brush, path);
        }

        // Draw text
        TextRenderer.DrawText(g, Text, Font, ClientRectangle, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

        path.Dispose();
    }

    private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
    {
        GraphicsPath path = new GraphicsPath();
        int diameter = radius * 2;

        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();

        return path;
    }
}

public class ModernTextBox : TextBox
{
    public int BorderRadius { get; set; } = 5;
    private bool _focused = false;

    public ModernTextBox()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
        BorderStyle = BorderStyle.None;
        Padding = new Padding(10, 8, 10, 8);
    }

    public void ApplyTheme(Theme theme)
    {
        BackColor = theme.InputBackground;
        ForeColor = theme.InputText;
        Invalidate();
    }

    protected override void OnEnter(EventArgs e)
    {
        _focused = true;
        Invalidate();
        base.OnEnter(e);
    }

    protected override void OnLeave(EventArgs e)
    {
        _focused = false;
        Invalidate();
        base.OnLeave(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Create rounded rectangle path
        GraphicsPath path = CreateRoundedRectangle(ClientRectangle, BorderRadius);

        // Fill background
        using (SolidBrush brush = new SolidBrush(BackColor))
        {
            g.FillPath(brush, path);
        }

        // Draw border
        Color borderColor = _focused ? ThemeManager.Current.Accent : ThemeManager.Current.Border;
        using (Pen pen = new Pen(borderColor, 2))
        {
            g.DrawPath(pen, path);
        }

        // Draw text
        Rectangle textRect = new Rectangle(Padding.Left, Padding.Top, Width - Padding.Horizontal, Height - Padding.Vertical);
        TextRenderer.DrawText(g, Text, Font, textRect, ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

        path.Dispose();
    }

    private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
    {
        GraphicsPath path = new GraphicsPath();
        int diameter = radius * 2;

        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();

        return path;
    }
}

public class ModernComboBox : ComboBox
{
    public int BorderRadius { get; set; } = 5;
    private bool _focused = false;

    public ModernComboBox()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
        DrawMode = DrawMode.OwnerDrawFixed;
        DropDownStyle = ComboBoxStyle.DropDownList;
        FlatStyle = FlatStyle.Flat;
    }

    public void ApplyTheme(Theme theme)
    {
        BackColor = theme.InputBackground;
        ForeColor = theme.InputText;
        Invalidate();
    }

    protected override void OnEnter(EventArgs e)
    {
        _focused = true;
        Invalidate();
        base.OnEnter(e);
    }

    protected override void OnLeave(EventArgs e)
    {
        _focused = false;
        Invalidate();
        base.OnLeave(e);
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        if (e.Index >= 0)
        {
            e.Graphics.FillRectangle(new SolidBrush(BackColor), e.Bounds);
            e.Graphics.DrawString(Items[e.Index].ToString(), Font, new SolidBrush(ForeColor), e.Bounds);
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Create rounded rectangle path
        GraphicsPath path = CreateRoundedRectangle(ClientRectangle, BorderRadius);

        // Fill background
        using (SolidBrush brush = new SolidBrush(BackColor))
        {
            g.FillPath(brush, path);
        }

        // Draw border
        Color borderColor = _focused ? ThemeManager.Current.Accent : ThemeManager.Current.Border;
        using (Pen pen = new Pen(borderColor, 2))
        {
            g.DrawPath(pen, path);
        }

        // Draw text
        if (SelectedIndex >= 0)
        {
            Rectangle textRect = new Rectangle(10, 0, Width - 30, Height);
            TextRenderer.DrawText(g, Items[SelectedIndex].ToString(), Font, textRect, ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }

        // Draw dropdown arrow
        Rectangle arrowRect = new Rectangle(Width - 20, Height / 2 - 3, 10, 6);
        Point[] arrow = {
            new Point(arrowRect.Left, arrowRect.Top),
            new Point(arrowRect.Right, arrowRect.Top),
            new Point(arrowRect.Left + arrowRect.Width / 2, arrowRect.Bottom)
        };
        using (SolidBrush brush = new SolidBrush(ForeColor))
        {
            g.FillPolygon(brush, arrow);
        }

        path.Dispose();
    }

    private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
    {
        GraphicsPath path = new GraphicsPath();
        int diameter = radius * 2;

        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();

        return path;
    }
}

public class ModernProgressBar : ProgressBar
{
    public int BorderRadius { get; set; } = 10;

    public ModernProgressBar()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
    }

    public void ApplyTheme(Theme theme)
    {
        BackColor = theme.ProgressBarBackground;
        ForeColor = theme.Accent;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Background
        using (SolidBrush brush = new SolidBrush(BackColor))
        {
            GraphicsPath bgPath = CreateRoundedRectangle(ClientRectangle, BorderRadius);
            g.FillPath(brush, bgPath);
            bgPath.Dispose();
        }

        // Progress
        if (Value > 0)
        {
            int progressWidth = (int)((double)Value / Maximum * Width);
            Rectangle progressRect = new Rectangle(0, 0, progressWidth, Height);
            
            using (SolidBrush brush = new SolidBrush(ForeColor))
            {
                GraphicsPath progressPath = CreateRoundedRectangle(progressRect, BorderRadius);
                g.FillPath(brush, progressPath);
                progressPath.Dispose();
            }
                if (isHovered)
                {
                    var hoverColor = Checked ? 
                        Color.FromArgb(Math.Min(255, theme.AccentColor.R + 20), 
                                     Math.Min(255, theme.AccentColor.G + 20), 
                                     Math.Min(255, theme.AccentColor.B + 20)) :
                        theme.ButtonHover;
                    bgColor = InterpolateColor(bgColor, hoverColor, animationProgress);
                }
                
                // Create gradient effect
                var gradientRect = new Rectangle(checkBoxRect.X, checkBoxRect.Y, checkBoxRect.Width, checkBoxRect.Height / 2);
                var lightColor = Color.FromArgb(20, 255, 255, 255);
                
                using (var mainBrush = new SolidBrush(bgColor))
                using (var gradientBrush = new LinearGradientBrush(gradientRect, lightColor, Color.Transparent, LinearGradientMode.Vertical))
                {
                    e.Graphics.FillPath(mainBrush, path);
                    e.Graphics.FillPath(gradientBrush, path);
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

            // Draw text with subtle shadow
            var shadowTextRect = new Rectangle(textRect.X + 1, textRect.Y + 1, textRect.Width, textRect.Height);
            var shadowColor = Color.FromArgb(60, 0, 0, 0);
            var textFlags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter;
            TextRenderer.DrawText(e.Graphics, Text, Font, shadowTextRect, shadowColor, textFlags);
            
            // Draw main text
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

    public class ModernRichTextBox : RichTextBox
    {
        private int borderRadius = 6;
        private bool isFocused = false;

        public int BorderRadius
        {
            get => borderRadius;
            set { borderRadius = value; Invalidate(); }
        }

        public ModernRichTextBox()
        {
            // Don't use UserPaint for RichTextBox as it handles its own text rendering
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | 
                    ControlStyles.OptimizedDoubleBuffer, true);
            
            BorderStyle = BorderStyle.None;
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            
            ApplyTheme();
            ThemeManager.ThemeChanged += (s, e) => ApplyTheme();
        }

        public void ApplyThemeToAllText()
        {
            var theme = ThemeManager.Current;
            
            // Save current selection
            int selStart = SelectionStart;
            int selLength = SelectionLength;
            
            // Select all text and apply theme colors
            SelectAll();
            SelectionColor = theme.TerminalText;
            SelectionBackColor = theme.TerminalBackground;
            
            // Restore original selection
            Select(selStart, selLength);
            
            // Update control colors
            BackColor = theme.TerminalBackground;
            ForeColor = theme.TerminalText;
        }

        private void ApplyTheme()
        {
            var theme = ThemeManager.Current;
            BackColor = theme.TerminalBackground;
            ForeColor = theme.TerminalText;
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
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            
            // Draw shadow
            var shadowRect = new Rectangle(2, 2, Width - 3, Height - 3);
            using (var shadowPath = GetRoundedRectanglePath(shadowRect, borderRadius))
            using (var shadowBrush = new SolidBrush(Color.FromArgb(15, 0, 0, 0)))
            {
                e.Graphics.FillPath(shadowBrush, shadowPath);
            }
            
            // Draw background
            using (var path = GetRoundedRectanglePath(rect, borderRadius))
            using (var bgBrush = new SolidBrush(BackColor))
            {
                e.Graphics.FillPath(bgBrush, path);
            }
            
            // Draw border
            var borderColor = isFocused ? theme.AccentColor : theme.BorderColor;
            var borderWidth = isFocused ? 2 : 1;
            
            using (var pen = new Pen(borderColor, borderWidth))
            using (var path = GetRoundedRectanglePath(rect, borderRadius))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }

        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            
            int diameter = radius * 2;
            var arc = new Rectangle(rect.Location, new Size(diameter, diameter));
            
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

    private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
    {
        GraphicsPath path = new GraphicsPath();
        int diameter = radius * 2;

        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();

        return path;
    }
}

public class ModernRichTextBox : RichTextBox
{
    public int BorderRadius { get; set; } = 5;
    private bool _focused = false;

    public ModernRichTextBox()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
        BorderStyle = BorderStyle.None;
        ScrollBars = RichTextBoxScrollBars.None;
    }

    public void ApplyTheme(Theme theme)
    {
        BackColor = theme.TerminalBackground;
        ForeColor = theme.TerminalText;
        Invalidate();
    }

    public void ApplyThemeToAllText()
    {
        // Save current selection
        int selectionStart = SelectionStart;
        int selectionLength = SelectionLength;
        
        // Select all text and apply theme colors
        SelectAll();
        SelectionColor = ThemeManager.Current.TerminalText;
        SelectionBackColor = ThemeManager.Current.TerminalBackground;
        
        // Restore original selection
        Select(selectionStart, selectionLength);
        
        // Update control colors
        BackColor = ThemeManager.Current.TerminalBackground;
        ForeColor = ThemeManager.Current.TerminalText;
    }

    protected override void OnEnter(EventArgs e)
    {
        _focused = true;
        Invalidate();
        base.OnEnter(e);
    }

    protected override void OnLeave(EventArgs e)
    {
        _focused = false;
        Invalidate();
        base.OnLeave(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
    }

    private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
    {
        GraphicsPath path = new GraphicsPath();
        int diameter = radius * 2;

        if (diameter > rect.Width) diameter = rect.Width;
        if (diameter > rect.Height) diameter = rect.Height;

        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();

        return path;
    }
}

public class ModernPanel : Panel
{
    public int BorderRadius { get; set; } = 10;
    public Color BorderColor { get; set; } = Color.Transparent;
    public int BorderWidth { get; set; } = 1;
    public bool EnableGradient { get; set; } = false;
    public Color GradientStartColor { get; set; } = Color.White;
    public Color GradientEndColor { get; set; } = Color.LightGray;
    public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical;

    public ModernPanel()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
    }

    public void ApplyTheme(Theme theme)
    {
        BackColor = theme.PanelBackground;
        BorderColor = theme.Border;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
        GraphicsPath path = CreateRoundedRectangle(rect, BorderRadius);

        // Fill background
        if (EnableGradient)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(rect, GradientStartColor, GradientEndColor, GradientDirection))
            {
                g.FillPath(brush, path);
            }
        }
        else
        {
            using (SolidBrush brush = new SolidBrush(BackColor))
            {
                g.FillPath(brush, path);
            }
        }

        // Draw border
        if (BorderWidth > 0 && BorderColor != Color.Transparent)
        {
            using (Pen pen = new Pen(BorderColor, BorderWidth))
            {
                g.DrawPath(pen, path);
            }
        }

        path.Dispose();
    }

    private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
    {
        GraphicsPath path = new GraphicsPath();
        int diameter = radius * 2;

        if (diameter > rect.Width) diameter = rect.Width;
        if (diameter > rect.Height) diameter = rect.Height;

        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();

        return path;
    }
}

public class ModernLabel : Label
{
    public bool EnableGlow { get; set; } = false;
    public Color GlowColor { get; set; } = Color.White;
    public int GlowSize { get; set; } = 2;

    public ModernLabel()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
    }

    public void ApplyTheme(Theme theme)
    {
        ForeColor = theme.Text;
        BackColor = Color.Transparent;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        if (EnableGlow)
        {
            // Draw glow effect
            for (int i = 1; i <= GlowSize; i++)
            {
                using (SolidBrush glowBrush = new SolidBrush(Color.FromArgb(50 / i, GlowColor)))
                {
                    g.DrawString(Text, Font, glowBrush, new PointF(i, i));
                    g.DrawString(Text, Font, glowBrush, new PointF(-i, i));
                    g.DrawString(Text, Font, glowBrush, new PointF(i, -i));
                    g.DrawString(Text, Font, glowBrush, new PointF(-i, -i));
                }
            }
        }

        // Draw main text
        using (SolidBrush textBrush = new SolidBrush(ForeColor))
        {
            g.DrawString(Text, Font, textBrush, new PointF(0, 0));
        }
    }
}

public class ModernCheckBox : CheckBox
{
    public int BorderRadius { get; set; } = 3;
    private bool _hovered = false;

    public ModernCheckBox()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
        FlatStyle = FlatStyle.Flat;
    }

    public void ApplyTheme(Theme theme)
    {
        ForeColor = theme.Text;
        BackColor = Color.Transparent;
        Invalidate();
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        _hovered = true;
        Invalidate();
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        _hovered = false;
        Invalidate();
        base.OnMouseLeave(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Calculate checkbox rectangle
        Rectangle checkRect = new Rectangle(0, (Height - 16) / 2, 16, 16);
        Rectangle textRect = new Rectangle(22, 0, Width - 22, Height);

        // Draw checkbox background
        Color bgColor = _hovered ? ThemeManager.Current.ButtonHover : ThemeManager.Current.InputBackground;
        using (SolidBrush brush = new SolidBrush(bgColor))
        {
            GraphicsPath path = CreateRoundedRectangle(checkRect, BorderRadius);
            g.FillPath(brush, path);
            path.Dispose();
        }

        // Draw checkbox border
        Color borderColor = _hovered ? ThemeManager.Current.Accent : ThemeManager.Current.Border;
        using (Pen pen = new Pen(borderColor, 1))
        {
            GraphicsPath path = CreateRoundedRectangle(checkRect, BorderRadius);
            g.DrawPath(pen, path);
            path.Dispose();
        }

        // Draw checkmark if checked
        if (Checked)
        {
            using (Pen pen = new Pen(ThemeManager.Current.Accent, 2))
            {
                g.DrawLines(pen, new Point[] {
                    new Point(checkRect.X + 3, checkRect.Y + 8),
                    new Point(checkRect.X + 6, checkRect.Y + 11),
                    new Point(checkRect.X + 12, checkRect.Y + 5)
                });
            }
        }

        // Draw text
        TextRenderer.DrawText(g, Text, Font, textRect, ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
    }

    private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
    {
        GraphicsPath path = new GraphicsPath();
        int diameter = radius * 2;

        if (diameter > rect.Width) diameter = rect.Width;
        if (diameter > rect.Height) diameter = rect.Height;

        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();

        return path;
    }
}