using ScrapeWilayah.Model;

namespace ScrapeWilayah.DataExportService;

public interface IDataExportService
{
    void SaveToCsv(List<WilayahData> data, string filePath);
    void SaveToJson(List<WilayahData> data, string filePath);
}