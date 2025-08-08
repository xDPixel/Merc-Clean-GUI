using System;
using System.Drawing;
using System.Windows.Forms;


namespace MyMaintenanceApp
{
    public class ModernTextBoxWithScrollbars : UserControl
    {
        private ModernRichTextBox textBox;
        private ModernScrollBar vScrollBar;
        private ModernScrollBar hScrollBar;
        private Panel cornerPanel;
        private bool _readOnly = false;
        private int _borderRadius = 0;

        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                _readOnly = value;
                if (textBox != null)
                    textBox.ReadOnly = value;
            }
        }

        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                _borderRadius = value;
                if (textBox != null)
                    textBox.BorderRadius = value;
            }
        }

        public override string Text
        {
            get => textBox?.Text ?? string.Empty;
            set
            {
                if (textBox != null)
                    textBox.Text = value;
            }
        }

        public override Font Font
        {
            get => textBox?.Font ?? base.Font;
            set
            {
                base.Font = value;
                if (textBox != null)
                    textBox.Font = value;
            }
        }

        public override Color BackColor
        {
            get => textBox?.BackColor ?? base.BackColor;
            set
            {
                base.BackColor = value;
                if (textBox != null)
                    textBox.BackColor = value;
            }
        }

        public override Color ForeColor
        {
            get => textBox?.ForeColor ?? base.ForeColor;
            set
            {
                base.ForeColor = value;
                if (textBox != null)
                    textBox.ForeColor = value;
            }
        }

        public ModernTextBoxWithScrollbars()
        {
            InitializeComponents();
            SetupScrollbars();
            ApplyTheme();
            ThemeManager.ThemeChanged += (s, e) => ApplyTheme();
        }

        private void InitializeComponents()
        {
            // Main textbox
            textBox = new ModernRichTextBox
            {
                Multiline = true,
                WordWrap = false,
                ScrollBars = RichTextBoxScrollBars.None,
                ReadOnly = _readOnly,
                BorderRadius = _borderRadius
            };
            textBox.TextChanged += OnTextChanged;
            textBox.Resize += OnTextBoxResize;

            // Vertical scrollbar
            vScrollBar = new ModernScrollBar
            {
                IsVertical = true,
                Width = 16,
                Dock = DockStyle.Right,
                Visible = false
            };
            vScrollBar.ValueChanged += OnVScrollValueChanged;

            // Horizontal scrollbar
            hScrollBar = new ModernScrollBar
            {
                IsVertical = false,
                Height = 16,
                Dock = DockStyle.Bottom,
                Visible = false
            };
            hScrollBar.ValueChanged += OnHScrollValueChanged;

            // Corner panel (where scrollbars meet)
            cornerPanel = new Panel
            {
                Size = new Size(16, 16),
                Visible = false
            };

            // Add controls
            Controls.Add(textBox);
            Controls.Add(vScrollBar);
            Controls.Add(hScrollBar);
            Controls.Add(cornerPanel);

            // Set textbox to fill remaining space
            textBox.Dock = DockStyle.Fill;
        }

        private void SetupScrollbars()
        {
            UpdateScrollbars();
        }

        private void UpdateScrollbars()
        {
            if (textBox == null) return;

            // Calculate if scrollbars are needed
            var textSize = TextRenderer.MeasureText(textBox.Text, textBox.Font);
            var clientSize = textBox.ClientSize;

            bool needVScroll = textSize.Height > clientSize.Height;
            bool needHScroll = textSize.Width > clientSize.Width;

            // Show/hide scrollbars
            vScrollBar.Visible = needVScroll;
            hScrollBar.Visible = needHScroll;
            cornerPanel.Visible = needVScroll && needHScroll;

            if (needVScroll)
            {
                var lines = textBox.Lines.Length;
                var visibleLines = clientSize.Height / textBox.Font.Height;
                vScrollBar.Maximum = Math.Max(0, lines - visibleLines);
                vScrollBar.LargeChange = Math.Max(1, visibleLines);
            }

            if (needHScroll)
            {
                var maxLineWidth = 0;
                foreach (var line in textBox.Lines)
                {
                    var lineWidth = TextRenderer.MeasureText(line, textBox.Font).Width;
                    maxLineWidth = Math.Max(maxLineWidth, lineWidth);
                }
                hScrollBar.Maximum = Math.Max(0, maxLineWidth - clientSize.Width);
                hScrollBar.LargeChange = Math.Max(1, clientSize.Width / 10);
            }

            // Position corner panel
            if (cornerPanel.Visible)
            {
                cornerPanel.Location = new Point(Width - 16, Height - 16);
            }
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            UpdateScrollbars();
        }

        private void OnTextBoxResize(object sender, EventArgs e)
        {
            UpdateScrollbars();
        }

        private void OnVScrollValueChanged(object sender, EventArgs e)
        {
            // Scroll textbox vertically
            var lineHeight = textBox.Font.Height;
            var scrollPosition = vScrollBar.Value * lineHeight;
            
            // Use SendMessage to scroll the textbox
            NativeMethods.SendMessage(textBox.Handle, NativeMethods.WM_VSCROLL, 
                new IntPtr(NativeMethods.SB_THUMBPOSITION | (scrollPosition << 16)), IntPtr.Zero);
        }

        private void OnHScrollValueChanged(object sender, EventArgs e)
        {
            // Scroll textbox horizontally
            NativeMethods.SendMessage(textBox.Handle, NativeMethods.WM_HSCROLL, 
                new IntPtr(NativeMethods.SB_THUMBPOSITION | (hScrollBar.Value << 16)), IntPtr.Zero);
        }

        public void ApplyTheme()
        {
            var theme = ThemeManager.Current;
            
            // Apply terminal colors to the textbox and all existing text
            if (textBox != null)
            {
                textBox.ApplyThemeToAllText();
            }
            
            if (cornerPanel != null)
            {
                cornerPanel.BackColor = theme.SecondaryBackground;
            }
            
            // Update scrollbars
            vScrollBar?.Invalidate();
            hScrollBar?.Invalidate();
        }

        public void AppendText(string text)
        {
            if (textBox != null)
            {
                var theme = ThemeManager.Current;
                
                // Move to end and set selection colors for new text
                textBox.SelectionStart = textBox.TextLength;
                textBox.SelectionLength = 0;
                textBox.SelectionColor = theme.TerminalText;
                textBox.SelectionBackColor = theme.TerminalBackground;
                
                // Append the text with theme colors
                textBox.AppendText(text);
                UpdateScrollbars();
                
                // Auto-scroll to bottom
                if (vScrollBar.Visible)
                {
                    vScrollBar.Value = vScrollBar.Maximum;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ThemeManager.ThemeChanged -= (s, e) => ApplyTheme();
            }
            base.Dispose(disposing);
        }
    }

    // Native methods for scrolling
    internal static class NativeMethods
    {
        public const int WM_VSCROLL = 0x0115;
        public const int WM_HSCROLL = 0x0114;
        public const int SB_THUMBPOSITION = 4;

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

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