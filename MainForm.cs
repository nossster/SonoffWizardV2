using System.Text;
using System.Text.Json;
using Microsoft.Win32;
using System.ComponentModel;
using Sonoff.Core;
using Zeroconf;

namespace SonoffWizardV2
{
    public partial class MainForm : Form
    {
        /* ───────────────  поля  ─────────────── */
        private bool IsDirty = false;
        private BindingList<DeviceEntry> devices = new();

        private readonly string devicesFile;

        public MainForm()
        {
            InitializeComponent();

            /* путь %APPDATA%\SonoffWizardV2\devices.json */
            var appDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SonoffWizardV2");
            Directory.CreateDirectory(appDir);
            devicesFile = Path.Combine(appDir, "devices.json");

            BindButtons();
            SetupDarkTheme();
            InitializeGrid();
            LoadDevices();
        }

        #region ─── GUI: тема и таблица ─────────────────────
        private void SetupDarkTheme()
        {
            BackColor = Color.FromArgb(45, 45, 48);
            ForeColor = Color.White;
            foreach (Control c in Controls) { c.BackColor = BackColor; c.ForeColor = ForeColor; }

            statusStrip.BackColor = Color.FromArgb(30, 30, 30);
            statusStrip.ForeColor = Color.White;

            dataGridViewDevices.BackgroundColor = Color.White;
            dataGridViewDevices.GridColor       = Color.DarkGray;

            var cell = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.Black,
                SelectionBackColor = Color.SteelBlue,
                SelectionForeColor = Color.White
            };
            dataGridViewDevices.DefaultCellStyle = cell;

            dataGridViewDevices.EnableHeadersVisualStyles = false;
            dataGridViewDevices.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(60, 60, 60);
            dataGridViewDevices.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }

        private void InitializeGrid()
        {
            dataGridViewDevices.AutoGenerateColumns = false;
            dataGridViewDevices.DataSource = devices;

            // порядок: Name – Endpoint – IP – Device ID – Port
            AddCol("Name",     "Name",      150);
            AddCol("Endpoint", "Endpoint",   90);
            AddCol("Host",     "IP",        140);
            AddCol("DeviceID", "Device ID", 160);
            AddCol("Port",     "Port",       60, typeof(int));

            dataGridViewDevices.CurrentCellDirtyStateChanged += (_, _) =>
            {
                if (dataGridViewDevices.IsCurrentCellDirty)
                    dataGridViewDevices.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };
            dataGridViewDevices.CellValueChanged += (_, _) => { if (Visible) MarkDirty(); };
        }
        private void AddCol(string prop, string hdr, int w, Type? t = null)
        {
            var col = new DataGridViewTextBoxColumn
                { HeaderText = hdr, DataPropertyName = prop, Width = w };
            if (t != null) col.ValueType = t;
            dataGridViewDevices.Columns.Add(col);
        }
        #endregion

        #region ─── Привязка кнопок ─────────────────────────
        private void BindButtons()
        {
            buttonAdd.Click      += buttonAdd_Click;
            buttonDelete.Click   += buttonDelete_Click;
            buttonSave.Click     += buttonSave_Click;
            buttonGenerate.Click += buttonGenerate_Click;
            buttonScan.Click     += buttonScan_Click;
            buttonHelp.Click     += buttonHelp_Click;
            FormClosing          += MainForm_FormClosing;
        }
        #endregion

        #region ─── Статус-бар ──────────────────────────────
        private void MarkDirty() => (IsDirty, statusLabel.Text) = (true, "Unsaved changes!");
        private void MarkClean() => (IsDirty, statusLabel.Text) = (false, "All changes saved.");
        #endregion

        #region ─── Add / Delete ────────────────────────────
        private void buttonAdd_Click(object? _, EventArgs __)
        { devices.Add(new DeviceEntry()); MarkDirty(); }

        private void buttonDelete_Click(object? _, EventArgs __)
        {
            foreach (DataGridViewRow r in dataGridViewDevices.SelectedRows)
                if (r.DataBoundItem is DeviceEntry d) devices.Remove(d);
            MarkDirty();
        }
        #endregion

        #region ─── Save / Load ─────────────────────────────
        private void buttonSave_Click(object? _, EventArgs __) => SaveDevices();
        private void SaveDevices()
        {
            var json = JsonSerializer.Serialize(devices, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(devicesFile, json);
            MarkClean();
        }
        private void LoadDevices()
        {
            if (!File.Exists(devicesFile)) return;
            try
            {
                var list = JsonSerializer.Deserialize<List<DeviceEntry>>(File.ReadAllText(devicesFile))
                           ?? new();
                devices = new BindingList<DeviceEntry>(list);
                dataGridViewDevices.DataSource = devices;
                MarkClean();
            }
            catch
            {
                MessageBox.Show("devices.json corrupted – new file will be created.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion

        #region ─── BAT generation ─────────────────────────
        private async void buttonGenerate_Click(object? _, EventArgs __)
        {
            statusLabel.Text = "Generating BAT…"; buttonGenerate.Enabled = false;
            await Task.Delay(200);

            string? gizmo = DetectGizmoInstallPath();
            if (gizmo == null)
            {
                MessageBox.Show("Gizmo Server not found – cancelled.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Error!";
                buttonGenerate.Enabled = true; return;
            }
            string dir = Path.Combine(gizmo, "batch"); Directory.CreateDirectory(dir);

            await Task.Run(() =>
            {
                WriteBat(Path.Combine(dir, "PowerOn.bat"),  true);
                WriteBat(Path.Combine(dir, "PowerOff.bat"), false);
            });
            statusLabel.Text = "BAT created."; buttonGenerate.Enabled = true;
        }

        private void WriteBat(string path, bool on)
        {
            using var w = new StreamWriter(path, false, Encoding.Default);
            w.WriteLine("@echo off\n");
            w.WriteLine("set \"WORKING_DIR=%~dp0\"");
            w.WriteLine("set \"LOG_FILE=%WORKING_DIR%relay_log.txt\"\n");

            int idx = 1;
            foreach (var d in devices)
            {
                w.WriteLine($"set deviceid{idx}={d.DeviceID}");
                w.WriteLine($"set host{idx}={d.Host}:{d.Port}\n");
                idx++;
            }

            idx = 1;
            foreach (var d in devices)
            {
                w.WriteLine($"if \"%1\"==\"{d.Endpoint}\" (");
                w.WriteLine($"    set deviceid=%deviceid{idx}%");
                w.WriteLine($"    set host=%host{idx}%");
                w.WriteLine(")\n");
                idx++;
            }

            w.WriteLine("if not defined host (");
            w.WriteLine("    echo [%date% %time%] Error: wrong endpoint \"%1\" >> \"%LOG_FILE%\"");
            w.WriteLine("    exit /b");
            w.WriteLine(")\n");

            w.WriteLine($"\"%WORKING_DIR%SonoffWizardV2.exe\" %deviceid% %host% %1 {(on ? 1 : 0)}");
        }
        #endregion

        #region ─── Scan (mDNS) ─────────────────────────────
        private async void buttonScan_Click(object? _, EventArgs __)
        {
            statusLabel.Text = "Scanning…"; buttonScan.Enabled = false;
            try
            {
                devices.Clear();
                var hosts = await ZeroconfResolver.ResolveAsync("_ewelink._tcp.local.");

                var seenId = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                if (hosts != null)
                {
                    foreach (var h in hosts)
                    {
                        if (h?.Services == null) continue;
                        if (!h.Services.TryGetValue("_ewelink._tcp.local.", out var svc)) continue;

                        string id = h.Id ?? "";
                        if (string.IsNullOrWhiteSpace(id) || id.Count(c => c == '.') == 3)
                        {
                            var p = (h.DisplayName ?? "").Split('_');
                            if (p.Length == 2) id = p[1];
                        }

                        if (!seenId.Add(id)) continue;          // дубликат по Device ID

                        devices.Add(new DeviceEntry
                        {
                            Name     = h.DisplayName ?? "(unnamed)",
                            Endpoint = "1",
                            Host     = h.IPAddress ?? "",
                            DeviceID = id,
                            Port     = svc.Port
                        });
                    }
                }
                statusLabel.Text = devices.Count == 0 ? "No devices." : $"Found {devices.Count}.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Scan failed",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Scan error.";
            }
            finally { buttonScan.Enabled = true; }
        }
        #endregion

        #region ─── Help ────────────────────────────────────
        private void buttonHelp_Click(object? _, EventArgs __) =>
            MessageBox.Show(
@"Add – create row
Delete – remove selected
Save – write JSON
Scan – discover Sonoff DIY
Generate BAT – create On/Off scripts
Endpoint – channel number (usually 1)",
            "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        #endregion

        #region ─── Gizmo detection ────────────────────────
        private static string? DetectGizmoInstallPath()
        {
            string[] roots =
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
            };
            foreach (var root in roots)
            {
                using var h = Registry.LocalMachine.OpenSubKey(root);
                if (h == null) continue;
                foreach (var sub in h.GetSubKeyNames())
                {
                    using var k = h.OpenSubKey(sub);
                    string? name = k?.GetValue("DisplayName") as string;
                    string? loc  = k?.GetValue("InstallLocation") as string;
                    if (!string.IsNullOrEmpty(name) && name.Contains("Gizmo Server") &&
                        !string.IsNullOrEmpty(loc)  && Directory.Exists(loc))
                        return loc;
                }
            }
            return null;
        }
        #endregion

        #region ─── Closing ────────────────────────────────
        private void MainForm_FormClosing(object? _, FormClosingEventArgs e)
        {
            if (!IsDirty) return;
            var res = MessageBox.Show("Save changes?", "Exit",
                                      MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (res == DialogResult.Yes) SaveDevices();
            else if (res == DialogResult.Cancel) e.Cancel = true;
        }
        #endregion
    }

    /* ---------- DTO ---------- */
    public class DeviceEntry
    {
        public string Name     { get; set; } = "";
        public string Endpoint { get; set; } = "1";
        public string Host     { get; set; } = "";
        public string DeviceID { get; set; } = "";
        public int    Port     { get; set; } = 8081;
    }
}
