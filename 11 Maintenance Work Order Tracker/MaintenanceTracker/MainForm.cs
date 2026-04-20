// MainForm.cs
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace MaintenanceTracker.WinForms;

public class MainForm : Form
{
    private readonly MaintenanceContext _db = new();
    private readonly ReportService _svc = new();

    private readonly TabControl tabs = new() { Dock = DockStyle.Fill };

    private readonly (string Title, Control Content, FlowLayoutPanel Bar)[] _pages;

    private readonly DataGridView gridTech = MakeGrid();
    private readonly DataGridView gridStatusOverall = MakeGrid();
    private readonly DataGridView gridStatusPerTech = MakeGrid();
    private readonly DataGridView gridWeekly = MakeGrid();
    private readonly DataGridView gridTop = MakeGrid();
    private readonly DataGridView gridBonus = MakeGrid();

    public MainForm()
    {
        Text = "Maintenance Work Order Tracker — Reports";
        MinimumSize = new Size(1100, 700);
        StartPosition = FormStartPosition.CenterScreen;

        _pages = new[]
        {
            ("Technician Summary", gridTech, MakeBar(LoadTechSummary, ExportTechSummary)),
            ("Status Summary", MakeSplitStatus(), MakeBar(LoadStatusSummary, ExportStatusSummary)),
            ("Weekly Labor Hours", gridWeekly, MakeBar(LoadWeekly, ExportWeekly)),
            ("Top Performer", gridTop, MakeBar(LoadTop, ExportTop)),
            ("Bonus Reports", gridBonus, MakeBar(LoadBonus, ExportBonus))
        };

        foreach (var (title, gridOrPanel, bar) in _pages)
        {
            var page = new TabPage(title);
            var panel = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2 };
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            panel.Controls.Add(bar, 0, 0);
            panel.Controls.Add(gridOrPanel, 0, 1);
            page.Controls.Add(panel);
            tabs.TabPages.Add(page);
        }

        Controls.Add(tabs);

        Load += async (_, __) =>
        {
            await SeedData.EnsureCreatedAndSeedAsync(_db);
            await LoadAll();
        };
    }

    private static DataGridView MakeGrid()
    {
        return new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells
        };
    }

    private Control MakeSplitStatus()
    {
        var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 220 };
        split.Panel1.Controls.Add(gridStatusOverall);
        split.Panel2.Controls.Add(gridStatusPerTech);
        return split;
    }

    private FlowLayoutPanel MakeBar(Func<Task> onRefresh, Action onExport)
    {
        var bar = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(8, 6, 8, 6) };
        var btnRefresh = new Button { Text = "Refresh", AutoSize = true };
        var btnExport = new Button { Text = "Export CSV", AutoSize = true };
        var btnSeed = new Button { Text = "Seed Sample Data", AutoSize = true };

        btnRefresh.Click += async (_, __) => await onRefresh();
        btnExport.Click += (_, __) => onExport();
        btnSeed.Click += async (_, __) => { await SeedData.EnsureCreatedAndSeedAsync(_db); await LoadAll(); };

        bar.Controls.AddRange(new Control[] { btnRefresh, btnExport, btnSeed });
        return bar;
    }

    private async Task LoadAll()
    {
        await LoadTechSummary();
        await LoadStatusSummary();
        await LoadWeekly();
        await LoadTop();
        await LoadBonus();
    }

    // ------- Loaders

    private async Task LoadTechSummary()
    {
        var rows = await _svc.TechnicianSummaryAsync(_db);
        gridTech.DataSource = rows;
    }

    private async Task LoadStatusSummary()
    {
        var (overall, perTech) = await _svc.StatusSummaryAsync(_db);
        gridStatusOverall.DataSource = overall.Select(x => new { Status = x.Status, Count = x.Count }).ToList();
        gridStatusPerTech.DataSource = perTech;
    }

    private async Task LoadWeekly()
    {
        var rows = await _svc.WeeklyLaborAsync(_db);
        gridWeekly.DataSource = rows;
    }

    private async Task LoadTop()
    {
        var top = await _svc.TopPerformerAsync(_db, minClosed: 3);
        gridTop.DataSource = (top is null) ? new List<TopPerf>() : new List<TopPerf> { top };
    }

    private async Task LoadBonus()
    {
        var bonus = await _svc.BonusAsync(_db);
        gridBonus.DataSource = new[]
        {
            new {
                BusiestWeek = bonus.BusiestWeek?.ToString("yyyy-MM-dd") ?? "(none)",
                Closed = bonus.BusiestClosed,
                OverdueOpenJobs = bonus.OverdueCount
            }
        }.ToList();
    }

    // ------- Exporters

    private void ExportTechSummary()
    {
        if (gridTech.DataSource is IEnumerable<TechSummary> list) Save(list, "tech_summary.csv");
        else SaveFromGrid(gridTech, "tech_summary.csv");
    }

    private void ExportStatusSummary()
    {
        // Export both grids separately
        SaveFromGrid(gridStatusOverall, "status_overall.csv");
        SaveFromGrid(gridStatusPerTech, "status_per_tech.csv");
    }

    private void ExportWeekly()
    {
        if (gridWeekly.DataSource is IEnumerable<WeeklyHours> list) Save(list, "weekly_hours.csv");
        else SaveFromGrid(gridWeekly, "weekly_hours.csv");
    }

    private void ExportTop()
    {
        if (gridTop.DataSource is IEnumerable<TopPerf> list) Save(list, "top_performer.csv");
        else SaveFromGrid(gridTop, "top_performer.csv");
    }

    private void ExportBonus()
    {
        SaveFromGrid(gridBonus, "bonus_reports.csv");
    }

    // Helpers
    private static void Save<T>(IEnumerable<T> items, string fileName)
    {
        using var sfd = new SaveFileDialog { FileName = fileName, Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*" };
        if (sfd.ShowDialog() == DialogResult.OK)
        {
            CsvExporter.Export(items, sfd.FileName);
            MessageBox.Show("Exported: " + sfd.FileName);
        }
    }

    private static void SaveFromGrid(DataGridView grid, string fileName)
    {
        var rows = new List<Dictionary<string, object?>>();
        var cols = grid.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).ToList();

        foreach (DataGridViewRow r in grid.Rows)
        {
            if (r.IsNewRow) continue;
            var dict = new Dictionary<string, object?>();
            foreach (var c in cols)
                dict[c.HeaderText] = r.Cells[c.Index].Value;
            rows.Add(dict);
        }

        // Convert to anonymous objects for CsvExporter
        var shaped = rows.Select(d => d.ToDictionary(k => k.Key, v => v.Value));
        // quick dynamic conversion:
        var table = new List<dynamic>();
        foreach (var d in shaped)
        {
            var obj = new System.Dynamic.ExpandoObject() as IDictionary<string, object?>;
            foreach (var kv in d) obj[kv.Key] = kv.Value;
            table.Add(obj);
        }

        using var sfd = new SaveFileDialog { FileName = fileName, Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*" };
        if (sfd.ShowDialog() == DialogResult.OK)
        {
            // Fallback path: serialize table via reflection of first element’s keys
            // Reuse CsvExporter by projecting to a typed anonymous at runtime (simple approach):
            // For simplicity, write a manual CSV here:
            var lines = new List<string>();
            var headers = cols.Select(c => c.HeaderText).ToList();
            lines.Add(string.Join(",", headers.Select(Escape)));
            foreach (var r in rows)
                lines.Add(string.Join(",", headers.Select(h => Escape(r.TryGetValue(h, out var v) ? v?.ToString() ?? "" : ""))));
            File.WriteAllLines(sfd.FileName, lines);
            MessageBox.Show("Exported: " + sfd.FileName);
        }

        static string Escape(string s)
        {
            if (s.Contains(',') || s.Contains('"') || s.Contains('\n'))
                return "\"" + s.Replace("\"", "\"\"") + "\"";
            return s;
        }
    }
}
