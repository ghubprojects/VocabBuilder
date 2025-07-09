using System.Text;

namespace VocabBuilder.Services.Export;

public class CsvExporter : ICsvExporter
{
    public byte[] Export<T>(IEnumerable<T> data)
    {
        if (data == null || !data.Any())
            return [];

        var csvBuilder = new StringBuilder();
        var properties = typeof(T).GetProperties();

        // Header
        csvBuilder.AppendLine(string.Join(",", properties.Select(p => Escape(p.Name))));

        // Rows
        foreach (var item in data)
        {
            var values = properties.Select(p =>
            {
                var value = p.GetValue(item);
                return Escape(value?.ToString() ?? string.Empty);
            });

            csvBuilder.AppendLine(string.Join(",", values));
        }

        return Encoding.UTF8.GetBytes(csvBuilder.ToString());
    }

    private static string Escape(string value)
    {
        // Wrap in quotes if value contains comma, quote, or newline
        if (value.Contains('"'))
            value = value.Replace("\"", "\"\"");

        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            value = $"\"{value}\"";

        return value;
    }
}
