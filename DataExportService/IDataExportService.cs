using ScrapeWilayah.Model;

namespace ScrapeWilayah.DataExportService;

public interface IDataExportService : IDisposable
{
    void SaveToCsv(List<WilayahData> data, string filePath, CsvExportOptions? options = null);
    Task SaveToCsvAsync(List<WilayahData> data, string filePath, CsvExportOptions? options = null, IProgress<int>? progress = null, CancellationToken cancellationToken = default);
    void SaveToJson(List<WilayahData> data, string filePath, JsonExportOptions? options = null);
    Task SaveToJsonAsync(List<WilayahData> data, string filePath, JsonExportOptions? options = null, IProgress<int>? progress = null, CancellationToken cancellationToken = default);
}