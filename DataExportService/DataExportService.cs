using System.Text;
using System.Text.Json;
using ScrapeWilayah.Logger;
using ScrapeWilayah.Model;

namespace ScrapeWilayah.DataExportService;

public class DataExportService(ILogger logger) : IDataExportService
{
    public void SaveToCsv(List<WilayahData> data, string filePath)
    {
        try
        {
            logger.Info($"Saving {data.Count} records to CSV: {filePath}");
            StringBuilder csv = new StringBuilder();

            // Header CSV
            csv.AppendLine("kode_provinsi,nama_provinsi,kode_provinsi_dagri,nama_provinsi_dagri," +
                           "kode_kabupaten,nama_kabupaten,kode_kabupaten_dagri,nama_kabupaten_dagri," +
                           "kode_kecamatan,nama_kecamatan,kode_kecamatan_dagri,nama_kecamatan_dagri");

            foreach (var item in data)
            {
                // Ensure data containing commas is quoted
                string line = string.Format(
                    "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\"",
                    item.kode_provinsi,
                    item.nama_provinsi,
                    item.kode_provinsi_dagri,
                    item.nama_provinsi_dagri,
                    item.kode_kabupaten,
                    item.nama_kabupaten,
                    item.kode_kabupaten_dagri,
                    item.nama_kabupaten_dagri,
                    item.kode_kecamatan,
                    item.nama_kecamatan,
                    item.kode_kecamatan_dagri,
                    item.nama_kecamatan_dagri);

                csv.AppendLine(line);
            }

            File.WriteAllText(filePath, csv.ToString());
            logger.Success($"CSV file saved successfully: {filePath}");
        }
        catch (Exception ex)
        {
            logger.Error($"Error saving CSV file: {filePath}", ex);
            throw;
        }
    }

    public void SaveToJson(List<WilayahData> data, string filePath)
    {
        try
        {
            logger.Info($"Saving {data.Count} records to JSON: {filePath}");
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(data, options);
            File.WriteAllText(filePath, jsonString);
            logger.Success($"JSON file saved successfully: {filePath}");
        }
        catch (Exception ex)
        {
            logger.Error($"Error saving JSON file: {filePath}", ex);
            throw;
        }
    }
}