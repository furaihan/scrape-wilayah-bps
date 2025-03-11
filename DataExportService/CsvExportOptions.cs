using System.Globalization;

namespace ScrapeWilayah.DataExportService;

public class CsvExportOptions
{
    public char Delimiter { get; set; } = ',';
    public bool HasHeaderRecord { get; set; } = true;
    public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;
    public bool QuoteAllFields { get; set; } = false;
}
