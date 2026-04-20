// CsvExporter.cs
using System.Data;
using System.Reflection;
using System.Text;

namespace MaintenanceTracker.WinForms;

public static class CsvExporter
{
    public static void Export<T>(IEnumerable<T> items, string path)
    {
        var list = items.ToList();
        if (list.Count == 0) { File.WriteAllText(path, ""); return; }

        var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(",", props.Select(p => p.Name)));

        foreach (var item in list)
        {
            var row = props.Select(p =>
            {
                var val = p.GetValue(item);
                return Escape(val?.ToString() ?? "");
            });
            sb.AppendLine(string.Join(",", row));
        }
        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
    }

    private static string Escape(string s)
    {
        if (s.Contains(',') || s.Contains('"') || s.Contains('\n'))
            return "\"" + s.Replace("\"", "\"\"") + "\"";
        return s;
    }
}
