using System;
using System.Drawing;
using System.Windows.Forms;

public class ModernTextBoxWithScrollbars : UserControl
{
    private ModernRichTextBox textBox;
    private ModernScrollBar verticalScrollBar;
    private ModernScrollBar horizontalScrollBar;
    private Panel cornerPanel;

    public ModernTextBoxWithScrollbars()
    {
        InitializeComponents();
        SetupEventHandlers();
    }

    private void InitializeComponents()
    {
        // Initialize the text box
        textBox = new ModernRichTextBox
        {
            Multiline = true,
            WordWrap = false,
            ScrollBars = RichTextBoxScrollBars.None,
            BorderStyle = BorderStyle.None,
            Dock = DockStyle.Fill,
            ReadOnly = true
        };

        // Initialize vertical scrollbar
        verticalScrollBar = new ModernScrollBar
        {
            Orientation = ScrollBarOrientation.Vertical,
            Dock = DockStyle.Right,
            Width = 20,
            Visible = false
        };

        // Initialize horizontal scrollbar
        horizontalScrollBar = new ModernScrollBar
        {
            Orientation = ScrollBarOrientation.Horizontal,
            Dock = DockStyle.Bottom,
            Height = 20,
            Visible = false
        };

        // Initialize corner panel
        cornerPanel = new Panel
        {
            Size = new Size(20, 20),
            Visible = false
        };

        // Add controls
        Controls.Add(textBox);
        Controls.Add(verticalScrollBar);
        Controls.Add(horizontalScrollBar);
        Controls.Add(cornerPanel);

        // Set initial size
        Size = new Size(300, 200);
    }

    private void SetupEventHandlers()
    {
        textBox.TextChanged += TextBox_TextChanged;
        textBox.Resize += TextBox_Resize;
        verticalScrollBar.Scroll += VerticalScrollBar_Scroll;
        horizontalScrollBar.Scroll += HorizontalScrollBar_Scroll;
        Resize += ModernTextBoxWithScrollbars_Resize;
    }

    public void ApplyTheme(Theme theme)
    {
        textBox.ApplyTheme(theme);
        textBox.ApplyThemeToAllText();
        verticalScrollBar.ApplyTheme(theme);
        horizontalScrollBar.ApplyTheme(theme);
        cornerPanel.BackColor = theme.ScrollBarBackground;
        BackColor = theme.TerminalBackground;
    }

    public void AppendText(string text)
    {
        // Set selection colors for new text
        textBox.SelectionStart = textBox.TextLength;
        textBox.SelectionLength = 0;
        textBox.SelectionColor = ThemeManager.Current.TerminalText;
        textBox.SelectionBackColor = ThemeManager.Current.TerminalBackground;
        
        textBox.AppendText(text);
        textBox.ScrollToCaret();
        UpdateScrollBars();
    }

    public void Clear()
    {
        textBox.Clear();
        UpdateScrollBars();
    }

    public string Text
    {
        get { return textBox.Text; }
        set 
        { 
            textBox.Text = value;
            UpdateScrollBars();
        }
    }

    public bool ReadOnly
    {
        get { return textBox.ReadOnly; }
        set { textBox.ReadOnly = value; }
    }

    public bool WordWrap
    {
        get { return textBox.WordWrap; }
        set 
        { 
            textBox.WordWrap = value;
            UpdateScrollBars();
        }
    }

    private void TextBox_TextChanged(object sender, EventArgs e)
    {
        UpdateScrollBars();
    }

    private void TextBox_Resize(object sender, EventArgs e)
    {
        UpdateScrollBars();
    }

    private void ModernTextBoxWithScrollbars_Resize(object sender, EventArgs e)
    {
        UpdateScrollBars();
        PositionCornerPanel();
    }

    private void UpdateScrollBars()
    {
        if (textBox == null) return;

        // Calculate if scrollbars are needed
        bool needVertical = textBox.GetPositionFromCharIndex(textBox.TextLength).Y + textBox.Font.Height > textBox.ClientSize.Height;
        bool needHorizontal = !textBox.WordWrap && GetMaxLineWidth() > textBox.ClientSize.Width;

        // Show/hide scrollbars
        verticalScrollBar.Visible = needVertical;
        horizontalScrollBar.Visible = needHorizontal;
        cornerPanel.Visible = needVertical && needHorizontal;

        if (needVertical)
        {
            UpdateVerticalScrollBar();
        }

        if (needHorizontal)
        {
            UpdateHorizontalScrollBar();
        }

        PositionCornerPanel();
    }

    private void UpdateVerticalScrollBar()
    {
        int totalHeight = textBox.GetPositionFromCharIndex(textBox.TextLength).Y + textBox.Font.Height;
        int visibleHeight = textBox.ClientSize.Height;
        
        if (totalHeight > visibleHeight)
        {
            verticalScrollBar.Maximum = totalHeight - visibleHeight;
            verticalScrollBar.LargeChange = visibleHeight;
            verticalScrollBar.SmallChange = textBox.Font.Height;
            verticalScrollBar.Value = Math.Min(verticalScrollBar.Value, verticalScrollBar.Maximum);
        }
    }

    private void UpdateHorizontalScrollBar()
    {
        int maxWidth = GetMaxLineWidth();
        int visibleWidth = textBox.ClientSize.Width;
        
        if (maxWidth > visibleWidth)
        {
            horizontalScrollBar.Maximum = maxWidth - visibleWidth;
            horizontalScrollBar.LargeChange = visibleWidth;
            horizontalScrollBar.SmallChange = 20;
            horizontalScrollBar.Value = Math.Min(horizontalScrollBar.Value, horizontalScrollBar.Maximum);
        }
    }

    private int GetMaxLineWidth()
    {
        int maxWidth = 0;
        string[] lines = textBox.Text.Split('\n');
        
        using (Graphics g = textBox.CreateGraphics())
        {
            foreach (string line in lines)
            {
                int lineWidth = (int)g.MeasureString(line, textBox.Font).Width;
                if (lineWidth > maxWidth)
                    maxWidth = lineWidth;
            }
        }
        
        return maxWidth;
    }

    private void VerticalScrollBar_Scroll(object sender, ScrollEventArgs e)
    {
        // Implement vertical scrolling logic
        // This is a simplified implementation
        textBox.SelectionStart = 0;
        textBox.ScrollToCaret();
    }

    private void HorizontalScrollBar_Scroll(object sender, ScrollEventArgs e)
    {
        // Implement horizontal scrolling logic
        // This is a simplified implementation
    }

    private void PositionCornerPanel()
    {
        if (cornerPanel.Visible)
        {
            cornerPanel.Location = new Point(
                Width - verticalScrollBar.Width,
                Height - horizontalScrollBar.Height
            );
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        // Draw border around the entire control
        using (Pen borderPen = new Pen(ThemeManager.Current.Border, 1))
        {
            e.Graphics.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
        }
        
        base.OnPaint(e);
    }
}