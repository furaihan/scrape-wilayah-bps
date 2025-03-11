using System.Text.Json;

namespace ScrapeWilayah.DataExportService;

public class JsonExportOptions
{
    public bool WriteIndented { get; set; } = true;
    public bool IgnoreNullValues { get; set; } = false;
    public JsonNamingPolicy PropertyNamingPolicy { get; set; } = null;
}