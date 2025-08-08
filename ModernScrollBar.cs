using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MyMaintenanceApp
{
    public class ModernScrollBar : Control
    {
        private int _minimum = 0;
        private int _maximum = 100;
        private int _value = 0;
        private int _smallChange = 1;
        private int _largeChange = 10;
        private bool _isVertical = true;
        private bool _isDragging = false;
        private Point _dragStartPoint;
        private int _dragStartValue;
        private Rectangle _thumbRect;
        private Rectangle _trackRect;
        private bool _thumbHovered = false;
        private bool _thumbPressed = false;

        public event EventHandler ValueChanged;

        public int Minimum
        {
            get => _minimum;
            set
            {
                _minimum = value;
                if (_value < _minimum) Value = _minimum;
                Invalidate();
            }
        }

        public int Maximum
        {
            get => _maximum;
            set
            {
                _maximum = value;
                if (_value > _maximum) Value = _maximum;
                Invalidate();
            }
        }

        public int Value
        {
            get => _value;
            set
            {
                var newValue = Math.Max(_minimum, Math.Min(_maximum, value));
                if (_value != newValue)
                {
                    _value = newValue;
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        public int SmallChange
        {
            get => _smallChange;
            set => _smallChange = value;
        }

        public int LargeChange
        {
            get => _largeChange;
            set => _largeChange = value;
        }

        public bool IsVertical
        {
            get => _isVertical;
            set
            {
                _isVertical = value;
                Invalidate();
            }
        }

        public ModernScrollBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | 
                    ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
            Size = new Size(16, 200);
            ThemeManager.ThemeChanged += (s, e) => Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var theme = ThemeManager.Current;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Calculate track and thumb rectangles
            CalculateRectangles();

            // Draw track with rounded corners
            using (var brush = new SolidBrush(theme.SecondaryBackground))
            using (var trackPath = GetRoundedRectanglePath(_trackRect, 6))
            {
                g.FillPath(brush, trackPath);
            }

            // Draw thumb with enhanced rounded corners
            var thumbColor = _thumbPressed ? theme.ButtonActive : 
                           _thumbHovered ? theme.ButtonHover : theme.ButtonBackground;
            
            using (var brush = new SolidBrush(thumbColor))
            using (var path = GetRoundedRectanglePath(_thumbRect, 6))
            {
                g.FillPath(brush, path);
            }

            // Draw subtle border with rounded corners
            using (var pen = new Pen(theme.BorderColor, 1))
            using (var borderPath = GetRoundedRectanglePath(new Rectangle(0, 0, Width - 1, Height - 1), 6))
            {
                g.DrawPath(pen, borderPath);
            }
        }

        private void CalculateRectangles()
        {
            if (_isVertical)
            {
                _trackRect = new Rectangle(2, 2, Width - 4, Height - 4);
                var thumbHeight = Math.Max(20, (int)((double)Height * _largeChange / (_maximum - _minimum + _largeChange)));
                var thumbTop = (int)((double)(_value - _minimum) / (_maximum - _minimum) * (Height - thumbHeight - 4)) + 2;
                _thumbRect = new Rectangle(2, thumbTop, Width - 4, thumbHeight);
            }
            else
            {
                _trackRect = new Rectangle(2, 2, Width - 4, Height - 4);
                var thumbWidth = Math.Max(20, (int)((double)Width * _largeChange / (_maximum - _minimum + _largeChange)));
                var thumbLeft = (int)((double)(_value - _minimum) / (_maximum - _minimum) * (Width - thumbWidth - 4)) + 2;
                _thumbRect = new Rectangle(thumbLeft, 2, thumbWidth, Height - 4);
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

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            
            if (e.Button == MouseButtons.Left)
            {
                if (_thumbRect.Contains(e.Location))
                {
                    _isDragging = true;
                    _thumbPressed = true;
                    _dragStartPoint = e.Location;
                    _dragStartValue = _value;
                    Capture = true;
                    Invalidate();
                }
                else if (_trackRect.Contains(e.Location))
                {
                    // Click on track - move thumb to clicked position
                    var newValue = CalculateValueFromPoint(e.Location);
                    Value = newValue;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            if (_isDragging)
            {
                var delta = _isVertical ? e.Y - _dragStartPoint.Y : e.X - _dragStartPoint.X;
                var range = _maximum - _minimum;
                var trackSize = _isVertical ? Height - _thumbRect.Height - 4 : Width - _thumbRect.Width - 4;
                var valueChange = (int)((double)delta / trackSize * range);
                Value = _dragStartValue + valueChange;
            }
            else
            {
                var wasHovered = _thumbHovered;
                _thumbHovered = _thumbRect.Contains(e.Location);
                if (wasHovered != _thumbHovered)
                {
                    Invalidate();
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            
            if (_isDragging)
            {
                _isDragging = false;
                _thumbPressed = false;
                Capture = false;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _thumbHovered = false;
            Invalidate();
        }

        private int CalculateValueFromPoint(Point point)
        {
            var position = _isVertical ? point.Y - 2 : point.X - 2;
            var trackSize = _isVertical ? Height - 4 : Width - 4;
            var thumbSize = _isVertical ? _thumbRect.Height : _thumbRect.Width;
            var availableTrack = trackSize - thumbSize;
            
            if (availableTrack <= 0) return _minimum;
            
            var ratio = (double)position / availableTrack;
            return _minimum + (int)(ratio * (_maximum - _minimum));
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            var delta = e.Delta > 0 ? -_smallChange : _smallChange;
            Value += delta;
        }
    }
}