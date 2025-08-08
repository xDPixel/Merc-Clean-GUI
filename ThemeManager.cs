using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

namespace MyMaintenanceApp
{
    public enum ThemeType
    {
        DarkModern,
        DarkBlue,
        DarkPurple,
        LightModern,
        LightBlue,
        LightGreen
    }

    public class Theme
    {
        public Color PrimaryBackground { get; set; }
        public Color SecondaryBackground { get; set; }
        public Color AccentColor { get; set; }
        public Color ButtonBackground { get; set; }
        public Color ButtonHover { get; set; }
        public Color ButtonActive { get; set; }
        public Color TextPrimary { get; set; }
        public Color TextSecondary { get; set; }
        public Color BorderColor { get; set; }
        public Color SuccessColor { get; set; }
        public Color WarningColor { get; set; }
        public Color ErrorColor { get; set; }
        public Color TerminalBackground { get; set; }
        public Color TerminalText { get; set; }
        public string Name { get; set; }
        public bool IsDark { get; set; }
    }

    public static class ThemeManager
    {
        public static event EventHandler<ThemeType> ThemeChanged;
        private static ThemeType _currentTheme = ThemeType.DarkModern;

        public static ThemeType CurrentTheme
        {
            get => _currentTheme;
            set
            {
                _currentTheme = value;
                ThemeChanged?.Invoke(null, value);
            }
        }

        public static Theme GetTheme(ThemeType themeType)
        {
            return themeType switch
            {
                ThemeType.DarkModern => new Theme
                {
                    Name = "Dark Modern",
                    IsDark = true,
                    PrimaryBackground = Color.FromArgb(18, 18, 18),
                    SecondaryBackground = Color.FromArgb(32, 32, 32),
                    AccentColor = Color.FromArgb(0, 120, 215),
                    ButtonBackground = Color.FromArgb(45, 45, 48),
                    ButtonHover = Color.FromArgb(62, 62, 66),
                    ButtonActive = Color.FromArgb(0, 84, 153),
                    TextPrimary = Color.FromArgb(255, 255, 255),
                    TextSecondary = Color.FromArgb(200, 200, 200),
                    BorderColor = Color.FromArgb(60, 60, 60),
                    SuccessColor = Color.FromArgb(16, 185, 129),
                    WarningColor = Color.FromArgb(245, 158, 11),
                    ErrorColor = Color.FromArgb(239, 68, 68),
                    TerminalBackground = Color.FromArgb(12, 12, 12),
                    TerminalText = Color.FromArgb(204, 204, 204)
                },
                ThemeType.DarkBlue => new Theme
                {
                    Name = "Dark Blue",
                    IsDark = true,
                    PrimaryBackground = Color.FromArgb(15, 23, 42),
                    SecondaryBackground = Color.FromArgb(30, 41, 59),
                    AccentColor = Color.FromArgb(59, 130, 246),
                    ButtonBackground = Color.FromArgb(51, 65, 85),
                    ButtonHover = Color.FromArgb(71, 85, 105),
                    ButtonActive = Color.FromArgb(37, 99, 235),
                    TextPrimary = Color.FromArgb(248, 250, 252),
                    TextSecondary = Color.FromArgb(203, 213, 225),
                    BorderColor = Color.FromArgb(71, 85, 105),
                    SuccessColor = Color.FromArgb(34, 197, 94),
                    WarningColor = Color.FromArgb(251, 191, 36),
                    ErrorColor = Color.FromArgb(248, 113, 113),
                    TerminalBackground = Color.FromArgb(8, 15, 26),
                    TerminalText = Color.FromArgb(226, 232, 240)
                },
                ThemeType.DarkPurple => new Theme
                {
                    Name = "Dark Purple",
                    IsDark = true,
                    PrimaryBackground = Color.FromArgb(24, 24, 27),
                    SecondaryBackground = Color.FromArgb(39, 39, 42),
                    AccentColor = Color.FromArgb(147, 51, 234),
                    ButtonBackground = Color.FromArgb(63, 63, 70),
                    ButtonHover = Color.FromArgb(82, 82, 91),
                    ButtonActive = Color.FromArgb(124, 58, 237),
                    TextPrimary = Color.FromArgb(250, 250, 250),
                    TextSecondary = Color.FromArgb(212, 212, 216),
                    BorderColor = Color.FromArgb(82, 82, 91),
                    SuccessColor = Color.FromArgb(34, 197, 94),
                    WarningColor = Color.FromArgb(251, 191, 36),
                    ErrorColor = Color.FromArgb(248, 113, 113),
                    TerminalBackground = Color.FromArgb(16, 16, 20),
                    TerminalText = Color.FromArgb(228, 228, 231)
                },
                ThemeType.LightModern => new Theme
                {
                    Name = "Light Modern",
                    IsDark = false,
                    PrimaryBackground = Color.FromArgb(255, 255, 255),
                    SecondaryBackground = Color.FromArgb(248, 250, 252),
                    AccentColor = Color.FromArgb(59, 130, 246),
                    ButtonBackground = Color.FromArgb(241, 245, 249),
                    ButtonHover = Color.FromArgb(226, 232, 240),
                    ButtonActive = Color.FromArgb(37, 99, 235),
                    TextPrimary = Color.FromArgb(15, 23, 42),
                    TextSecondary = Color.FromArgb(71, 85, 105),
                    BorderColor = Color.FromArgb(203, 213, 225),
                    SuccessColor = Color.FromArgb(34, 197, 94),
                    WarningColor = Color.FromArgb(245, 158, 11),
                    ErrorColor = Color.FromArgb(239, 68, 68),
                    TerminalBackground = Color.FromArgb(250, 250, 250),
                    TerminalText = Color.FromArgb(30, 30, 30)
                },
                ThemeType.LightBlue => new Theme
                {
                    Name = "Light Blue",
                    IsDark = false,
                    PrimaryBackground = Color.FromArgb(240, 249, 255),
                    SecondaryBackground = Color.FromArgb(224, 242, 254),
                    AccentColor = Color.FromArgb(14, 165, 233),
                    ButtonBackground = Color.FromArgb(186, 230, 253),
                    ButtonHover = Color.FromArgb(147, 197, 253),
                    ButtonActive = Color.FromArgb(2, 132, 199),
                    TextPrimary = Color.FromArgb(12, 74, 110),
                    TextSecondary = Color.FromArgb(14, 116, 144),
                    BorderColor = Color.FromArgb(125, 211, 252),
                    SuccessColor = Color.FromArgb(34, 197, 94),
                    WarningColor = Color.FromArgb(245, 158, 11),
                    ErrorColor = Color.FromArgb(239, 68, 68),
                    TerminalBackground = Color.FromArgb(235, 245, 255),
                    TerminalText = Color.FromArgb(30, 58, 138)
                },
                ThemeType.LightGreen => new Theme
                {
                    Name = "Light Green",
                    IsDark = false,
                    PrimaryBackground = Color.FromArgb(240, 253, 244),
                    SecondaryBackground = Color.FromArgb(220, 252, 231),
                    AccentColor = Color.FromArgb(34, 197, 94),
                    ButtonBackground = Color.FromArgb(187, 247, 208),
                    ButtonHover = Color.FromArgb(134, 239, 172),
                    ButtonActive = Color.FromArgb(21, 128, 61),
                    TextPrimary = Color.FromArgb(20, 83, 45),
                    TextSecondary = Color.FromArgb(22, 101, 52),
                    BorderColor = Color.FromArgb(110, 231, 183),
                    SuccessColor = Color.FromArgb(34, 197, 94),
                    WarningColor = Color.FromArgb(245, 158, 11),
                    ErrorColor = Color.FromArgb(239, 68, 68),
                    TerminalBackground = Color.FromArgb(235, 250, 240),
                    TerminalText = Color.FromArgb(21, 128, 61)
                },
                _ => GetTheme(ThemeType.DarkModern)
            };
        }

        public static Theme Current => GetTheme(CurrentTheme);
    }
}