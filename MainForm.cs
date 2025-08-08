using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Principal;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyMaintenanceApp
{
    public partial class MainForm : Form
    {
        // --- UI controls ---
        private ModernTextBoxWithScrollbars txtTerminal;
        private ModernButton btnRunAll;
        private ModernButton btnSfcScan;
        private ModernButton btnDismCheckHealth;
        private ModernButton btnDismScanHealth;
        private ModernButton btnDismRestoreHealth;
        private ModernButton btnClearTemp;
        private ModernButton btnClearCache;
        private ModernButton btnDiskCleanup;
        private ModernButton btnOptimizeDrives;
        private ModernButton btnClearDNS;
        private Label lblFooter;
        private Label lblVersion;
        private ModernButton btnX;
        private ModernButton btnWebsite;
        private ModernButton btnCredits;
        private ModernButton btnKillProcess;
        private Label lblStatus;
        private ModernCheckBox chkClearLoginData;
        private ThemeSwitcher themeSwitcher;
        // Removed unused panel fields to fix build warnings

        // Current maintenance process (for kill)
        private Process currentProcess;

        // Avoid repeated printing of "disabled"/"enabled" messages
        private bool areButtonsDisabled = false;

        // A flag to stop the "Run All" sequence if the user kills or cancels
        private bool cancellationRequested = false;

        public MainForm()
        {
            InitializeComponent();
            ApplyTheme();
            ThemeManager.ThemeChanged += (s, e) => ApplyTheme();

            // Enable form rounded corners
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | 
                         ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Region = CreateRoundedRegion(this.ClientRectangle, 12);
            this.Resize += (s, e) => this.Region = CreateRoundedRegion(this.ClientRectangle, 12);

            // Check for administrative privileges
            if (!IsUserAdministrator())
            {
                MessageBox.Show("This application must be run as Administrator.", 
                    "Insufficient Privileges", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            UpdateStatus("Idle", ThemeManager.Current.TextSecondary);
            
            // Check for updates asynchronously
            _ = CheckForUpdatesAsync();
        }

        private void ApplyTheme()
        {
            var theme = ThemeManager.Current;
            
            // Apply theme to form
            this.BackColor = theme.PrimaryBackground;
            
            // Apply theme to terminal
            if (txtTerminal != null)
            {
                txtTerminal.BackColor = theme.TerminalBackground;
                txtTerminal.ForeColor = theme.TerminalText;
                txtTerminal.ApplyTheme();
                txtTerminal.Invalidate();
            }
            
            // Apply theme to status label
            if (lblStatus != null)
            {
                lblStatus.ForeColor = theme.TextPrimary;
                lblStatus.BackColor = theme.PrimaryBackground;
            }
            
            // Apply theme to footer label
            if (lblFooter != null)
            {
                lblFooter.ForeColor = theme.TextSecondary;
                lblFooter.BackColor = theme.PrimaryBackground;
            }
            
            // Apply theme to version label
            if (lblVersion != null)
            {
                lblVersion.ForeColor = theme.TextSecondary;
                lblVersion.BackColor = theme.PrimaryBackground;
            }
            
            // Refresh form appearance
            this.Invalidate();
        }
        
        private Region CreateRoundedRegion(Rectangle rect, int radius)
        {
            using (var path = new GraphicsPath())
            {
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
                return new Region(path);
            }
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            var theme = ThemeManager.Current;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw form border with rounded corners
            using (var borderPen = new Pen(theme.BorderColor, 2))
            using (var path = new GraphicsPath())
            {
                var rect = new Rectangle(1, 1, Width - 2, Height - 2);
                var radius = 12;
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
                e.Graphics.DrawPath(borderPen, path);
            }
        }

        private void InitializeComponent()
        {
            // Basic form setup
            this.Text = "MercClean - GUI | By @DangerousPixel";
            this.Size = new Size(950, 720);
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | 
                         ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            
            // Set form icon
            try
            {
                this.Icon = new Icon("icon.ico");
            }
            catch
            {
                // If icon file is not found, continue without icon
            }

            // Status label (above terminal)
            lblStatus = new Label
            {
                Text = "Status: Idle",
                Location = new Point(20, 15),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblStatus);

            // Terminal-like TextBox
            txtTerminal = new ModernTextBoxWithScrollbars
            {
                ReadOnly = true,
                Location = new Point(20, 60),
                Size = new Size(900, 280),
                Font = new Font("Consolas", 10F),
                BorderRadius = 8
            };
            this.Controls.Add(txtTerminal);

            // Run All
            btnRunAll = new ModernButton
            {
                Text = "🚀 Run All",
                Location = new Point(20, 360),
                Size = new Size(140, 45),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BorderRadius = 10
            };
            btnRunAll.Click += async (s, e) => await RunAllTasksAsync();
            this.Controls.Add(btnRunAll);

            // SFC Scan
            btnSfcScan = new ModernButton
            {
                Text = "🔧 SFC Scan",
                Location = new Point(180, 360),
                Size = new Size(120, 45),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                BorderRadius = 8
            };
            btnSfcScan.Click += async (s, e) =>
            {
                UpdateStatus("Running SFC Scan", ThemeManager.Current.SuccessColor);
                AppendTerminal("Running SFC Scan...\r\n");
                await RunCommandAsync("sfc", "/scannow");
                UpdateStatus("Idle", ThemeManager.Current.TextSecondary);
            };
            this.Controls.Add(btnSfcScan);

            // DISM CheckHealth
            btnDismCheckHealth = new ModernButton
            {
                Text = "🩺 DISM Check",
                Location = new Point(320, 360),
                Size = new Size(120, 45),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                BorderRadius = 8
            };
            btnDismCheckHealth.Click += async (s, e) =>
            {
                UpdateStatus("Running DISM CheckHealth", ThemeManager.Current.SuccessColor);
                AppendTerminal("Running DISM CheckHealth...\r\n");
                await RunCommandAsync("DISM", "/online /Cleanup-Image /CheckHealth");
                UpdateStatus("Idle", ThemeManager.Current.TextSecondary);
            };
            this.Controls.Add(btnDismCheckHealth);

            // DISM ScanHealth
            btnDismScanHealth = new ModernButton
            {
                Text = "🔍 DISM Scan",
                Location = new Point(460, 360),
                Size = new Size(120, 45),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                BorderRadius = 8
            };
            btnDismScanHealth.Click += async (s, e) =>
            {
                UpdateStatus("Running DISM ScanHealth", ThemeManager.Current.SuccessColor);
                AppendTerminal("Running DISM ScanHealth...\r\n");
                await RunCommandAsync("DISM", "/Online /Cleanup-Image /ScanHealth");
                UpdateStatus("Idle", ThemeManager.Current.TextSecondary);
            };
            this.Controls.Add(btnDismScanHealth);

            // DISM RestoreHealth
            btnDismRestoreHealth = new ModernButton
            {
                Text = "🔄 DISM Restore",
                Location = new Point(600, 360),
                Size = new Size(120, 45),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                BorderRadius = 8
            };
            btnDismRestoreHealth.Click += async (s, e) =>
            {
                UpdateStatus("Running DISM RestoreHealth", ThemeManager.Current.SuccessColor);
                AppendTerminal("Running DISM RestoreHealth...\r\n");
                await RunCommandAsync("DISM", "/Online /Cleanup-Image /RestoreHealth");
                UpdateStatus("Idle", ThemeManager.Current.TextSecondary);
            };
            this.Controls.Add(btnDismRestoreHealth);

            // Clear Temp
            btnClearTemp = new ModernButton
            {
                Text = "🗑️ Clear Temp",
                Location = new Point(20, 420),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                BorderRadius = 6
            };
            btnClearTemp.Click += (s, e) =>
            {
                UpdateStatus("Running Clear Temp", ThemeManager.Current.SuccessColor);
                ClearTempFiles();
                UpdateStatus("Idle", ThemeManager.Current.TextSecondary);
            };
            this.Controls.Add(btnClearTemp);

            // Clear Cache
            btnClearCache = new ModernButton
            {
                Text = "💾 Clear Cache",
                Location = new Point(150, 420),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                BorderRadius = 6
            };
            btnClearCache.Click += (s, e) =>
            {
                UpdateStatus("Running Clear Cache", ThemeManager.Current.SuccessColor);
                ClearBrowserCache();
                UpdateStatus("Idle", ThemeManager.Current.TextSecondary);
            };
            this.Controls.Add(btnClearCache);

            // Disk Cleanup
            btnDiskCleanup = new ModernButton
            {
                Text = "💿 Disk Cleanup",
                Location = new Point(280, 420),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                BorderRadius = 6
            };
            btnDiskCleanup.Click += async (s, e) =>
            {
                UpdateStatus("Running Disk Cleanup", ThemeManager.Current.SuccessColor);
                await RunDiskCleanup();
                UpdateStatus("Idle", ThemeManager.Current.TextSecondary);
            };
            this.Controls.Add(btnDiskCleanup);

            // Optimize Drives
            btnOptimizeDrives = new ModernButton
            {
                Text = "⚡ Optimize Drives",
                Location = new Point(410, 420),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                BorderRadius = 6
            };
            btnOptimizeDrives.Click += async (s, e) =>
            {
                UpdateStatus("Running Optimize Drives", ThemeManager.Current.SuccessColor);
                await OptimizeDrives();
                UpdateStatus("Idle", ThemeManager.Current.TextSecondary);
            };
            this.Controls.Add(btnOptimizeDrives);

            // Clear DNS
            btnClearDNS = new ModernButton
            {
                Text = "🌐 Clear DNS",
                Location = new Point(540, 420),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                BorderRadius = 6
            };
            btnClearDNS.Click += async (s, e) =>
            {
                UpdateStatus("Running Clear DNS", ThemeManager.Current.SuccessColor);
                await ClearDNSCache();
                UpdateStatus("Idle", ThemeManager.Current.TextSecondary);
            };
            this.Controls.Add(btnClearDNS);

            // Kill Process
            btnKillProcess = new ModernButton
            {
                Text = "❌ Kill Process",
                Location = new Point(670, 420),
                Size = new Size(100, 40),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                BorderRadius = 6
            };
            btnKillProcess.Click += (s, e) => KillCurrentProcess();
            this.Controls.Add(btnKillProcess);

            // Clear Login Data Checkbox (isolated, next to buttons)
            chkClearLoginData = new ModernCheckBox
            {
                Text = "🔒 Clear Browser Login Data",
                Location = new Point(20, 480),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Checked = false // Disabled by default
            };
            chkClearLoginData.CheckedChanged += (s, e) =>
            {
                if (chkClearLoginData.Checked)
                {
                    DialogResult result = MessageBox.Show(
                        "Warning: This will clear all saved login data from browsers.\n\n" +
                        "You will need to log in again to all websites and browser profiles.\n\n" +
                        "Do you want to continue?",
                        "Clear Login Data Warning",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );
                    
                    if (result == DialogResult.No)
                    {
                        chkClearLoginData.Checked = false;
                    }
                }
            };
            this.Controls.Add(chkClearLoginData);

            // Theme Switcher
            themeSwitcher = new ThemeSwitcher
            {
                Location = new Point(750, 15),
                Size = new Size(140, 40)
            };
            this.Controls.Add(themeSwitcher);

            // Footer label
            lblFooter = new Label
            {
                Text = "Made By @DangerousPixel",
                Location = new Point(600, 625),
                Size = new Size(150, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblFooter);

            // Version label (bottom right)
            lblVersion = new Label
            {
                Text = $"v{Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}",
                Location = new Point(780, 625),
                Size = new Size(100, 20),
                Font = new Font("Arial", 9, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleRight
            };
            this.Controls.Add(lblVersion);

            // "X" Button
            btnX = new ModernButton
            {
                Text = "❌",
                Location = new Point(20, 620),
                Size = new Size(50, 40),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BorderRadius = 8
            };
            btnX.Click += (s, e) =>
            {
                Process.Start(new ProcessStartInfo("https://x.com/dangerousPixel")
                {
                    UseShellExecute = true
                });
            };
            this.Controls.Add(btnX);

            // Website for iOS Mods Button
            btnWebsite = new ModernButton
            {
                Text = "🌐 Website for iOS Mods",
                Location = new Point(80, 620),
                Size = new Size(170, 40),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                BorderRadius = 8
            };
            btnWebsite.Click += (s, e) =>
            {
                Process.Start(new ProcessStartInfo("https://dpixel.co")
                {
                    UseShellExecute = true
                });
            };
            this.Controls.Add(btnWebsite);

            // Credits Button
            btnCredits = new ModernButton
            {
                Text = "👥 Credits",
                Location = new Point(260, 620),
                Size = new Size(100, 40),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                BorderRadius = 8
            };
            btnCredits.Click += (s, e) => ShowCreditsPopup();
            this.Controls.Add(btnCredits);

            // GitHub Button
            ModernButton btnGitHub = new ModernButton
            {
                Text = "🐙 GitHub",
                Location = new Point(370, 620),
                Size = new Size(100, 40),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                BorderRadius = 8
            };
            btnGitHub.Click += (s, e) =>
            {
                Process.Start(new ProcessStartInfo("https://github.com/xDPixel")
                {
                    UseShellExecute = true
                });
            };
            this.Controls.Add(btnGitHub);
        }

        /// <summary>
        /// Toggles the enabled state of the main buttons (except Kill).
        /// Uses a bool to avoid duplicate "disabled" or "enabled" messages.
        /// </summary>
        private void ToggleButtons(bool isEnabled)
        {
            if (!isEnabled && !areButtonsDisabled)
            {
                AppendTerminal("A task is running. Buttons disabled.\r\n");
                areButtonsDisabled = true;
            }
            else if (isEnabled && areButtonsDisabled)
            {
                AppendTerminal("Buttons re-enabled.\r\n");
                areButtonsDisabled = false;
            }

            // Actually toggle them
            btnRunAll.Enabled = isEnabled;
            btnSfcScan.Enabled = isEnabled;
            btnDismCheckHealth.Enabled = isEnabled;
            btnDismScanHealth.Enabled = isEnabled;
            btnDismRestoreHealth.Enabled = isEnabled;
            btnClearTemp.Enabled = isEnabled;
            btnClearCache.Enabled = isEnabled;
            btnDiskCleanup.Enabled = isEnabled;
            btnOptimizeDrives.Enabled = isEnabled;
            btnClearDNS.Enabled = isEnabled;
            chkClearLoginData.Enabled = isEnabled;
            // Kill Process remains enabled
        }

        /// <summary>
        /// Kills the currently running process (if any) and cancels the "Run All" sequence.
        /// </summary>
        private void KillCurrentProcess()
        {
            try
            {
                // Signal no further tasks in "Run All" should continue
                cancellationRequested = true;

                if (currentProcess != null && !currentProcess.HasExited)
                {
                    currentProcess.Kill();
                    AppendTerminal("Process killed.\r\n");
                }
            }
            catch (Exception ex)
            {
                AppendTerminal($"Error killing process: {ex.Message}\r\n");
            }
            finally
            {
                currentProcess = null;
                // We won't re-enable here (avoids double "Buttons re-enabled").
                // The ongoing command or "Run All" flow will finalize and re-enable.
                UpdateStatus("Idle", ThemeManager.Current.TextSecondary);
            }
        }

        /// <summary>
        /// Runs a single command asynchronously with output in the terminal.
        /// </summary>
        private async Task RunCommandAsync(string command, string arguments)
        {
            try
            {
                ToggleButtons(false); // disable main buttons
                await Task.Run(() =>
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = command,
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };

                    using (var proc = new Process { StartInfo = psi })
                    {
                        currentProcess = proc;
                        proc.OutputDataReceived += (s, e) =>
                        {
                            if (!string.IsNullOrEmpty(e.Data))
                                AppendTerminal(e.Data + Environment.NewLine);
                        };
                        proc.ErrorDataReceived += (s, e) =>
                        {
                            if (!string.IsNullOrEmpty(e.Data))
                                AppendTerminal("ERROR: " + e.Data + Environment.NewLine);
                        };
                        proc.Start();
                        proc.BeginOutputReadLine();
                        proc.BeginErrorReadLine();
                        proc.WaitForExit();
                    }
                });
            }
            catch (Exception ex)
            {
                AppendTerminal($"Error running command: {ex.Message}\r\n");
                AppendTerminal($"Stack Trace: {ex.StackTrace}\r\n");
            }
            finally
            {
                currentProcess = null;
                // Re-enable main buttons after the command completes
                ToggleButtons(true);
            }
        }

        /// <summary>
        /// Sequentially runs all tasks, with cancellation checks after each step.
        /// </summary>
        private async Task RunAllTasksAsync()
        {
            try
            {
                ToggleButtons(false);
                UpdateStatus("Running All Tasks", ThemeManager.Current.SuccessColor);
                cancellationRequested = false;

                // (1) SFC
                AppendTerminal("Running SFC Scan...\r\n");
                await RunCommandAsync("sfc", "/scannow");
                if (cancellationRequested) return;

                // (2) DISM CheckHealth
                AppendTerminal("Running DISM CheckHealth...\r\n");
                await RunCommandAsync("DISM", "/online /Cleanup-Image /CheckHealth");
                if (cancellationRequested) return;

                // (3) DISM ScanHealth
                AppendTerminal("Running DISM ScanHealth...\r\n");
                await RunCommandAsync("DISM", "/Online /Cleanup-Image /ScanHealth");
                if (cancellationRequested) return;

                // (4) DISM RestoreHealth
                AppendTerminal("Running DISM RestoreHealth...\r\n");
                await RunCommandAsync("DISM", "/Online /Cleanup-Image /RestoreHealth");
                if (cancellationRequested) return;

                // (5) Clear Temp
                UpdateStatus("Running Clear Temp", ThemeManager.Current.SuccessColor);
                ClearTempFiles();
                if (cancellationRequested) return;

                // (6) Clear Browser Cache
                UpdateStatus("Running Clear Cache", ThemeManager.Current.SuccessColor);
                ClearBrowserCache();
                if (cancellationRequested) return;

                // (7) Disk Cleanup
                UpdateStatus("Running Disk Cleanup", ThemeManager.Current.SuccessColor);
                await RunDiskCleanup();
                if (cancellationRequested) return;

                // (8) Optimize Drives
                UpdateStatus("Running Optimize Drives", ThemeManager.Current.SuccessColor);
                await OptimizeDrives();
                if (cancellationRequested) return;

                // (9) Clear DNS
                UpdateStatus("Running Clear DNS", ThemeManager.Current.SuccessColor);
                await ClearDNSCache();
                if (cancellationRequested) return;

                AppendTerminal("All tasks completed.\r\n");
                UpdateStatus("Idle", ThemeManager.Current.TextSecondary);
            }
            catch (Exception ex)
            {
                AppendTerminal($"Error running all tasks: {ex.Message}\r\n");
                AppendTerminal($"Stack Trace: {ex.StackTrace}\r\n");
            }
            finally
            {
                // Re-enable the buttons at the end of "Run All"
                ToggleButtons(true);
            }
        }

        /// <summary>
        /// Clears temporary files.
        /// </summary>
        private void ClearTempFiles()
        {
            string tempPath = Path.GetTempPath();
            try
            {
                AppendTerminal("Clearing temporary files...\r\n");
                foreach (string file in Directory.GetFiles(tempPath))
                {
                    try
                    {
                        File.Delete(file);
                        AppendTerminal($"Deleted: {file}\r\n");
                    }
                    catch (Exception ex)
                    {
                        AppendTerminal($"Failed to delete {file}: {ex.Message}\r\n");
                    }
                }
                AppendTerminal("Temporary files cleared.\r\n");
            }
            catch (Exception ex)
            {
                AppendTerminal($"Error clearing temporary files: {ex.Message}\r\n");
            }
        }

        /// <summary>
        /// Clears browser cache (excluding login data).
        /// </summary>
        private void ClearBrowserCache()
        {
            string[] cachePaths =
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Google\Chrome\User Data\Default\Cache"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Google\Chrome\User Data\Default\Code Cache"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Mozilla\Firefox\Profiles"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\Edge\User Data\Default\Cache"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\Edge\User Data\Default\Code Cache")
            };

            AppendTerminal("Clearing browser cache (excluding login data)...\r\n");
            foreach (var path in cachePaths)
            {
                if (Directory.Exists(path))
                {
                    try
                    {
                        foreach (string file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
                        {
                            try
                            {
                                File.Delete(file);
                                AppendTerminal($"Deleted cache file: {file}\r\n");
                            }
                            catch (Exception ex)
                            {
                                AppendTerminal($"Failed to delete {file}: {ex.Message}\r\n");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AppendTerminal($"Error clearing cache in {path}: {ex.Message}\r\n");
                    }
                }
            }
            AppendTerminal("Browser cache cleared.\r\n");
            
            // Clear login data if checkbox is checked
            if (chkClearLoginData.Checked)
            {
                ClearBrowserLoginData();
            }
        }

        /// <summary>
        /// Clears browser login data (passwords, cookies, session data).
        /// </summary>
        private void ClearBrowserLoginData()
        {
            string[] loginDataPaths =
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Google\Chrome\User Data\Default\Login Data"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Google\Chrome\User Data\Default\Cookies"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Google\Chrome\User Data\Default\Web Data"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Mozilla\Firefox\Profiles"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\Edge\User Data\Default\Login Data"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\Edge\User Data\Default\Cookies"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\Edge\User Data\Default\Web Data")
            };

            AppendTerminal("Clearing browser login data...\r\n");
            foreach (var path in loginDataPaths)
            {
                if (File.Exists(path))
                {
                    try
                    {
                        File.Delete(path);
                        AppendTerminal($"Deleted login data: {path}\r\n");
                    }
                    catch (Exception ex)
                    {
                        AppendTerminal($"Failed to delete {path}: {ex.Message}\r\n");
                    }
                }
                else if (Directory.Exists(path) && path.Contains("Firefox"))
                {
                    // For Firefox, clear specific login-related files
                    try
                    {
                        string[] firefoxLoginFiles = { "cookies.sqlite", "logins.json", "key4.db", "signons.sqlite" };
                        foreach (string profileDir in Directory.GetDirectories(path))
                        {
                            foreach (string loginFile in firefoxLoginFiles)
                            {
                                string filePath = Path.Combine(profileDir, loginFile);
                                if (File.Exists(filePath))
                                {
                                    File.Delete(filePath);
                                    AppendTerminal($"Deleted Firefox login data: {filePath}\r\n");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AppendTerminal($"Error clearing Firefox login data in {path}: {ex.Message}\r\n");
                    }
                }
            }
            AppendTerminal("Browser login data cleared.\r\n");
        }

        /// <summary>
        /// Runs Disk Cleanup (SAGE run).
        /// </summary>
        private async Task RunDiskCleanup()
        {
            AppendTerminal("Running Disk Cleanup...\r\n");
            await RunCommandAsync("cleanmgr.exe", "/sagerun:1");
        }

        /// <summary>
        /// Optimizes drives (defragmentation).
        /// </summary>
        private async Task OptimizeDrives()
        {
            AppendTerminal("Optimizing drives...\r\n");
            await RunCommandAsync("defrag.exe", "C: /O");
        }

        /// <summary>
        /// Clears DNS cache.
        /// </summary>
        private async Task ClearDNSCache()
        {
            AppendTerminal("Clearing DNS cache...\r\n");
            await RunCommandAsync("ipconfig", "/flushdns");
        }

        /// <summary>
        /// Checks if the current user is an administrator.
        /// </summary>
        private bool IsUserAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// Safely appends text to the terminal TextBox.
        /// </summary>
        private void AppendTerminal(string text)
        {
            if (txtTerminal.InvokeRequired)
            {
                txtTerminal.Invoke(new Action(() => txtTerminal.AppendText(text)));
            }
            else
            {
                txtTerminal.AppendText(text);
            }
        }

        /// <summary>
        /// Updates the status label.
        /// </summary>
        private void UpdateStatus(string status, Color color)
        {
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke(new Action(() => UpdateStatus(status, color)));
            }
            else
            {
                lblStatus.Text = $"Status: {status}";
                lblStatus.ForeColor = color;
            }
        }

        /// <summary>
        /// Shows the credits popup.
        /// </summary>
        private void ShowCreditsPopup()
        {
            string creditsText = "Thanks to Merc Clean Developer for the base functionality.\n" +
                                 "Additional functionalities made by @DangerousPixel.";
            MessageBox.Show(creditsText, "Credits", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Checks for updates from GitHub releases.
        /// </summary>
        private async Task CheckForUpdatesAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Merc-Clean-GUI");
                    
                    var response = await client.GetStringAsync("https://api.github.com/repos/xDPixel/Merc-Clean-GUI/releases/latest");
                    var release = JsonSerializer.Deserialize<GitHubRelease>(response);
                    
                    var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                    var latestVersion = new Version(release.tag_name.TrimStart('v'));
                    
                    if (latestVersion > currentVersion)
                    {
                        this.Invoke(new Action(() => ShowUpdateDialog(release.tag_name)));
                    }
                }
            }
            catch
            {
                // Silently fail if update check fails (no internet, API issues, etc.)
            }
        }

        /// <summary>
        /// Shows the update dialog.
        /// </summary>
        private void ShowUpdateDialog(string latestVersion)
        {
            var result = MessageBox.Show(
                $"A new version ({latestVersion}) is available!\n\nWould you like to download the update?",
                "Update Available",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                Process.Start(new ProcessStartInfo("https://github.com/xDPixel/Merc-Clean-GUI/releases")
                {
                    UseShellExecute = true
                });
            }
        }

        /// <summary>
        /// GitHub release model for JSON deserialization.
        /// </summary>
        private class GitHubRelease
        {
            public string tag_name { get; set; }
            public string name { get; set; }
            public bool prerelease { get; set; }
        }
    }
}