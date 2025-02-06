using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyMaintenanceApp
{
    public partial class MainForm : Form
    {
        // --- UI controls ---
        private TextBox txtTerminal;
        private Button btnRunAll;
        private Button btnSfcScan;
        private Button btnDismCheckHealth;
        private Button btnDismScanHealth;
        private Button btnDismRestoreHealth;
        private Button btnClearTemp;
        private Button btnClearCache;
        private Button btnDiskCleanup;
        private Button btnOptimizeDrives;
        private Button btnClearDNS;
        private Label lblFooter;
        private Button btnX;
        private Button btnWebsite;
        private Button btnCredits;
        private Button btnKillProcess;
        private Label lblStatus;

        // Current maintenance process (for kill)
        private Process currentProcess;

        // Avoid repeated printing of "disabled"/"enabled" messages
        private bool areButtonsDisabled = false;

        // A flag to stop the "Run All" sequence if the user kills or cancels
        private bool cancellationRequested = false;

        public MainForm()
        {
            InitializeComponent();

            // Check for administrative privileges
            if (!IsUserAdministrator())
            {
                MessageBox.Show("This application must be run as Administrator.", 
                    "Insufficient Privileges", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            UpdateStatus("Idle", Color.Gray);
        }

        private void InitializeComponent()
        {
            // Basic form setup
            this.Text = "MercClean - GUI | By DPixel Team";
            this.Size = new Size(900, 700);
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Prevent resizing
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 30);

            // Status label (above terminal)
            lblStatus = new Label
            {
                Text = "Status: Idle",
                Location = new Point(10, 10),
                Size = new Size(300, 20),
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblStatus);

            // Terminal-like TextBox
            txtTerminal = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Both,
                ReadOnly = true,
                Location = new Point(10, 40),
                Size = new Size(760, 320),
                Font = new Font("Consolas", 10),
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White
            };
            this.Controls.Add(txtTerminal);

            // Run All
            btnRunAll = new Button
            {
                Text = "Run All",
                Location = new Point(10, 370),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White
            };
            btnRunAll.Click += async (s, e) => await RunAllTasksAsync();
            this.Controls.Add(btnRunAll);

            // SFC Scan
            btnSfcScan = new Button
            {
                Text = "SFC Scan",
                Location = new Point(100, 370),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White
            };
            btnSfcScan.Click += async (s, e) =>
            {
                UpdateStatus("Running SFC Scan", Color.Green);
                AppendTerminal("Running SFC Scan...\r\n");
                await RunCommandAsync("sfc", "/scannow");
                UpdateStatus("Idle", Color.Gray);
            };
            this.Controls.Add(btnSfcScan);

            // DISM CheckHealth
            btnDismCheckHealth = new Button
            {
                Text = "DISM CheckHealth",
                Location = new Point(190, 370),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White
            };
            btnDismCheckHealth.Click += async (s, e) =>
            {
                UpdateStatus("Running DISM CheckHealth", Color.Green);
                AppendTerminal("Running DISM CheckHealth...\r\n");
                await RunCommandAsync("DISM", "/online /Cleanup-Image /CheckHealth");
                UpdateStatus("Idle", Color.Gray);
            };
            this.Controls.Add(btnDismCheckHealth);

            // DISM ScanHealth
            btnDismScanHealth = new Button
            {
                Text = "DISM ScanHealth",
                Location = new Point(320, 370),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White
            };
            btnDismScanHealth.Click += async (s, e) =>
            {
                UpdateStatus("Running DISM ScanHealth", Color.Green);
                AppendTerminal("Running DISM ScanHealth...\r\n");
                await RunCommandAsync("DISM", "/Online /Cleanup-Image /ScanHealth");
                UpdateStatus("Idle", Color.Gray);
            };
            this.Controls.Add(btnDismScanHealth);

            // DISM RestoreHealth
            btnDismRestoreHealth = new Button
            {
                Text = "DISM RestoreHealth",
                Location = new Point(450, 370),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White
            };
            btnDismRestoreHealth.Click += async (s, e) =>
            {
                UpdateStatus("Running DISM RestoreHealth", Color.Green);
                AppendTerminal("Running DISM RestoreHealth...\r\n");
                await RunCommandAsync("DISM", "/Online /Cleanup-Image /RestoreHealth");
                UpdateStatus("Idle", Color.Gray);
            };
            this.Controls.Add(btnDismRestoreHealth);

            // Clear Temp
            btnClearTemp = new Button
            {
                Text = "Clear Temp",
                Location = new Point(10, 410),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White
            };
            btnClearTemp.Click += (s, e) =>
            {
                UpdateStatus("Running Clear Temp", Color.Green);
                ClearTempFiles();
                UpdateStatus("Idle", Color.Gray);
            };
            this.Controls.Add(btnClearTemp);

            // Clear Cache
            btnClearCache = new Button
            {
                Text = "Clear Cache",
                Location = new Point(100, 410),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White
            };
            btnClearCache.Click += (s, e) =>
            {
                UpdateStatus("Running Clear Cache", Color.Green);
                ClearBrowserCache();
                UpdateStatus("Idle", Color.Gray);
            };
            this.Controls.Add(btnClearCache);

            // Disk Cleanup
            btnDiskCleanup = new Button
            {
                Text = "Disk Cleanup",
                Location = new Point(190, 410),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White
            };
            btnDiskCleanup.Click += async (s, e) =>
            {
                UpdateStatus("Running Disk Cleanup", Color.Green);
                await RunDiskCleanup();
                UpdateStatus("Idle", Color.Gray);
            };
            this.Controls.Add(btnDiskCleanup);

            // Optimize Drives
            btnOptimizeDrives = new Button
            {
                Text = "Optimize Drives",
                Location = new Point(320, 410),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White
            };
            btnOptimizeDrives.Click += async (s, e) =>
            {
                UpdateStatus("Running Optimize Drives", Color.Green);
                await OptimizeDrives();
                UpdateStatus("Idle", Color.Gray);
            };
            this.Controls.Add(btnOptimizeDrives);

            // Clear DNS
            btnClearDNS = new Button
            {
                Text = "Clear DNS",
                Location = new Point(450, 410),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White
            };
            btnClearDNS.Click += async (s, e) =>
            {
                UpdateStatus("Running Clear DNS", Color.Green);
                await ClearDNSCache();
                UpdateStatus("Idle", Color.Gray);
            };
            this.Controls.Add(btnClearDNS);

            // Kill Process
            btnKillProcess = new Button
            {
                Text = "Kill Process",
                Location = new Point(580, 410),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White
            };
            btnKillProcess.Click += (s, e) => KillCurrentProcess();
            this.Controls.Add(btnKillProcess);

            // Footer label
            lblFooter = new Label
            {
                Text = "Made By DPixel Team",
                Location = new Point(350, 625),
                Size = new Size(200, 20),
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblFooter);

            // "X" Button
            btnX = new Button
            {
                Text = "X",
                Location = new Point(10, 620),
                Size = new Size(40, 30),
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White
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
            btnWebsite = new Button
            {
                Text = "Website for iOS Mods",
                Location = new Point(60, 620),
                Size = new Size(160, 30),
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White
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
            btnCredits = new Button
            {
                Text = "Credits",
                Location = new Point(230, 620),
                Size = new Size(70, 30),
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White
            };
            btnCredits.Click += (s, e) => ShowCreditsPopup();
            this.Controls.Add(btnCredits);
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
                UpdateStatus("Idle", Color.Gray);
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
                UpdateStatus("Running All Tasks", Color.Green);
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
                UpdateStatus("Running Clear Temp", Color.Green);
                ClearTempFiles();
                if (cancellationRequested) return;

                // (6) Clear Browser Cache
                UpdateStatus("Running Clear Cache", Color.Green);
                ClearBrowserCache();
                if (cancellationRequested) return;

                // (7) Disk Cleanup
                UpdateStatus("Running Disk Cleanup", Color.Green);
                await RunDiskCleanup();
                if (cancellationRequested) return;

                // (8) Optimize Drives
                UpdateStatus("Running Optimize Drives", Color.Green);
                await OptimizeDrives();
                if (cancellationRequested) return;

                // (9) Clear DNS
                UpdateStatus("Running Clear DNS", Color.Green);
                await ClearDNSCache();
                if (cancellationRequested) return;

                AppendTerminal("All tasks completed.\r\n");
                UpdateStatus("Idle", Color.Gray);
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
        /// Clears browser cache.
        /// </summary>
        private void ClearBrowserCache()
        {
            string[] browserPaths =
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Google\Chrome\User Data"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Mozilla\Firefox\Profiles"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\Edge\User Data")
            };

            AppendTerminal("Clearing browser cache...\r\n");
            foreach (var path in browserPaths)
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
                                AppendTerminal($"Deleted: {file}\r\n");
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
                                 "Additional functionalities made by DPixel Team.";
            MessageBox.Show(creditsText, "Credits", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}