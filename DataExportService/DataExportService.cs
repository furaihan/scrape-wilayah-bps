using System.Text.Json;
using ScrapeWilayah.Logger;
using ScrapeWilayah.Model;
using CsvHelper;
using CsvHelper.Configuration;

namespace ScrapeWilayah.DataExportService;

public class DataExportService : IDataExportService
{
    private readonly ILogger _logger;
    private bool _disposed = false;

    public DataExportService(ILogger logger)
    {
        _logger = logger;
    }

    public void SaveToCsv(List<WilayahData> data, string filePath, CsvExportOptions? options = null)
    {
        ValidateFilePath(filePath, ".csv");
        options ??= new CsvExportOptions();

        try
        {
            _logger.Info($"Saving {data.Count} records to CSV: {filePath}");
            EnsureDirectoryExists(filePath);

            using var writer = new StreamWriter(filePath);
            using var csv = new CsvWriter(writer, new CsvConfiguration(options.Culture)
            {
                Delimiter = options.Delimiter.ToString(),
                HasHeaderRecord = options.HasHeaderRecord,
                ShouldQuote = args => options.QuoteAllFields || args.Field.Contains(options.Delimiter) ||
                                      args.Field.Contains('"') || args.Field.Contains('\n')
            });

            // Write header
            csv.WriteHeader<WilayahData>();
            csv.NextRecord();

            // Write records
            foreach (var record in data)
            {
                csv.WriteRecord(record);
                csv.NextRecord();
            }

            _logger.Success($"CSV file saved successfully: {filePath}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Error saving CSV file: {filePath}", ex);
            throw;
        }
    }

    public async Task SaveToCsvAsync(List<WilayahData> data, string filePath, CsvExportOptions? options = null,
        IProgress<int>? progress = null, CancellationToken cancellationToken = default)
    {
        ValidateFilePath(filePath, ".csv");
        options ??= new CsvExportOptions();

        try
        {
            _logger.Info($"Saving {data.Count} records to CSV: {filePath}");
            EnsureDirectoryExists(filePath);

            using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true);
            using var writer = new StreamWriter(stream);
            using var csv = new CsvWriter(writer, new CsvConfiguration(options.Culture)
            {
                Delimiter = options.Delimiter.ToString(),
                HasHeaderRecord = options.HasHeaderRecord,
                ShouldQuote = args => options.QuoteAllFields || args.Field.Contains(options.Delimiter) ||
                                      args.Field.Contains('"') || args.Field.Contains('\n')
            });

            // Write header - CsvHelper doesn't have async for these operations
            // so we'll write synchronously but use async for file operations
            csv.WriteHeader<WilayahData>();
            csv.NextRecord();

            // Write records with progress reporting
            int totalRecords = data.Count;
            for (int i = 0; i < totalRecords; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                csv.WriteRecord(data[i]);
                csv.NextRecord();

                // Report progress periodically
                if (i % 100 == 0 || i == totalRecords - 1)
                {
                    progress?.Report((i + 1) * 100 / totalRecords);
                    await writer.FlushAsync();
                }
            }

            await writer.FlushAsync();
            _logger.Success($"CSV file saved successfully: {filePath}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Error saving CSV file: {filePath}", ex);
            throw;
        }
    }

    public void SaveToJson(List<WilayahData> data, string filePath, JsonExportOptions? options = null)
    {
        ValidateFilePath(filePath, ".json");
        options ??= new JsonExportOptions();

        try
        {
            _logger.Info($"Saving {data.Count} records to JSON: {filePath}");
            EnsureDirectoryExists(filePath);

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = options.WriteIndented,
                PropertyNamingPolicy = options.PropertyNamingPolicy,
                DefaultIgnoreCondition = options.IgnoreNullValues
                    ? System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                    : System.Text.Json.Serialization.JsonIgnoreCondition.Never
            };

            string jsonString = JsonSerializer.Serialize(data, jsonOptions);
            File.WriteAllText(filePath, jsonString);

            _logger.Success($"JSON file saved successfully: {filePath}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Error saving JSON file: {filePath}", ex);
            throw;
        }
    }

    public async Task SaveToJsonAsync(List<WilayahData> data, string filePath, JsonExportOptions? options = null,
        IProgress<int>? progress = null, CancellationToken cancellationToken = default)
    {
        ValidateFilePath(filePath, ".json");
        options ??= new JsonExportOptions();

        try
        {
            _logger.Info($"Saving {data.Count} records to JSON: {filePath}");
            EnsureDirectoryExists(filePath);

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = options.WriteIndented,
                PropertyNamingPolicy = options.PropertyNamingPolicy,
                DefaultIgnoreCondition = options.IgnoreNullValues
                    ? System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                    : System.Text.Json.Serialization.JsonIgnoreCondition.Never
            };

            // For larger datasets, we'll report progress during serialization
            using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true);

            // Report start of process
            progress?.Report(0);

            await JsonSerializer.SerializeAsync(stream, data, jsonOptions, cancellationToken);

            // Report completion
            progress?.Report(100);

            _logger.Success($"JSON file saved successfully: {filePath}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Error saving JSON file: {filePath}", ex);
            throw;
        }
    }

    private void ValidateFilePath(string filePath, string expectedExtension)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be empty", nameof(filePath));
        }

        string extension = Path.GetExtension(filePath).ToLowerInvariant();
        if (extension != expectedExtension)
        {
            throw new ArgumentException($"File extension must be {expectedExtension}", nameof(filePath));
        }
    }

    private void EnsureDirectoryExists(string filePath)
    {
        string? directoryPath = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            _logger.Info($"Created directory: {directoryPath}");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources if needed
            }

            // Free unmanaged resources if needed
            _disposed = true;
        }
    }

    ~DataExportService()
    {
        Dispose(false);
    }
}