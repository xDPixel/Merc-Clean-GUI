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