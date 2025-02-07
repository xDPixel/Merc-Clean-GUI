# MercClean - GUI

A Windows desktop application (WinForms) that provides a one-stop GUI for various system maintenance tasks—like SFC scans, DISM checks, clearing temp files/cache, and more. This tool is especially helpful for quickly troubleshooting and cleaning up Windows machines, with the ability to cancel operations mid-run if needed.

---

## Features

1. **Run All**  
   - Sequentially executes a full suite of scans and cleanups:
     - SFC (`sfc /scannow`)
     - DISM CheckHealth, ScanHealth, RestoreHealth
     - Clear temporary files, browser caches
     - Disk Cleanup
     - Drive optimization
     - DNS cache flush

2. **Individual Maintenance Tasks**  
   - Each function—SFC, DISM, Clear Temp, etc.—is available as a separate button.

3. **Kill Process**  
   - Allows you to cancel a currently running maintenance operation at any time, including stopping the entire “Run All” sequence.

4. **Detailed Terminal Output**  
   - Displays real-time command output (stdout/stderr) in a terminal-like text box, so you can monitor progress and see any errors.

5. **Links & Credits**  
   - Quick buttons to open relevant websites or show credits.

---

## Screenshots

![MercClean GUI Screenshot](https://i.postimg.cc/zDn109YC/Screenshot-2025-02-06-123747.png)
---

## Requirements

- **Windows 10 or higher**  
- <del> **.NET 6+** or **.NET Framework 4.7.2+** (depending on your project setup) <del> 
- NoW YoU CaN RuN ApP DiReCtLy WiThOuT AnY DePeNdANcIeS :)
- **Administrator privileges** (the application checks at startup)

---

## Installation and Usage

1. **Clone or Download** this repository.
2. **Open in Visual Studio** (or another IDE supporting WinForms) and ensure you have the correct .NET SDK/Target.
3. Build the solution, which creates an `.exe` (or `.exe + .dll` for .NET 6+) inside the `bin\Debug\net6.0-windows\` or `bin\Release\net6.0-windows\` folder (depending on your build configuration).
4. **Run as Administrator**. The application will display an error if not run with elevated privileges.

**OR JUST INSTALL LATEST RELEASE :)**

---

## How to Use

1. **Launch `MercClean - GUI.exe`** (right-click and “Run as administrator,” if necessary).
2. Click any of the maintenance buttons, such as:
   - **SFC Scan** to run `sfc /scannow`.
   - **DISM CheckHealth** / **ScanHealth** / **RestoreHealth** for DISM checks.
   - **Clear Temp** or **Clear Cache** for quick file clean-ups.
   - **Disk Cleanup** or **Optimize Drives** for thorough Windows maintenance.
   - **Clear DNS** to flush the DNS cache.
3. **Run All** to perform every task in sequence.
4. If you need to abort, press **Kill Process**, which stops the current command and cancels any remaining tasks.

---

## Contributing

Contributions and pull requests are welcome! If you have bug fixes or new features to add:

1. Fork this repository.
2. Create a new branch with a descriptive name.
3. Commit and push your changes.
4. Create a pull request describing the modifications.

---

## Credits

- **Merc Clean Developer** – For the original command-line concept.
- **DPixel Team** – For additional functionalities, GUI improvements, and the kill-process feature.
