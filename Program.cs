using ScrapeWilayah.ApiService;
using ScrapeWilayah.DataExportService;
using ScrapeWilayah.Logger;
using ScrapeWilayah.Model;
using ScrapeWilayah.ScraperService;
using ScrapeWilayah.StatisticService;

namespace ScrapeWilayah;

class Program
{
    static async Task Main(string[] args)
    {
        // Setup dependency injection
        ILogger logger = new ConsoleLogger();
        IWilayahApiService apiService = new WilayahApiService(logger);
        IWilayahScraperService scraperService = new WilayahScraperService(apiService, logger);
        IDataExportService exportService = new DataExportService.DataExportService(logger);
        IStatisticsService statisticsService = new StatisticsService(logger);

        try
        {
            logger.Info("Starting Indonesian regional data scraping process...");
                
            // Scrape all data
            List<WilayahData> allData = await scraperService.ScrapeAllWilayahAsync();
                
            // Export data to files
            exportService.SaveToCsv(allData, "wilayah_indonesia.csv");
            exportService.SaveToJson(allData, "wilayah_indonesia.json");
                
            // Display statistics
            statisticsService.DisplayStatistics(allData);
                
            logger.Success("Process completed successfully!");
        }
        catch (Exception ex)
        {
            logger.Error("An error occurred during the scraping process", ex);
        }

        logger.Info("Press any key to exit...");
        Console.ReadKey();
    }
}
