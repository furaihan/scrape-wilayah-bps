using ScrapeWilayah.ApiService;
using ScrapeWilayah.DataExportService;
using ScrapeWilayah.Logger;
using ScrapeWilayah.Model;
using ScrapeWilayah.ScraperService;
using ScrapeWilayah.StatisticService;
using System.Text.Json;

namespace ScrapeWilayah;

class Program
{
    private const string CSV_FILENAME = "wilayah_indonesia.csv";
    private const string JSON_FILENAME = "wilayah_indonesia.json";

    static async Task Main(string[] args)
    {
        // Setup dependency injection
        ILogger logger = new ConsoleLogger();
        IWilayahApiService apiService = new WilayahApiService(logger);
        IWilayahScraperService scraperService = new WilayahScraperService(apiService, logger);
        IDataExportService exportService = new DataExportService.DataExportService(logger);
        IStatisticService statisticService = new StatisticService.StatisticService(logger);

        try
        {
            bool exitRequested = false;
            
            while (!exitRequested)
            {
                DisplayMenu();
                string choice = Console.ReadLine() ?? string.Empty;
                
                switch (choice.Trim())
                {
                    case "1":
                        await ScrapeAndSaveDataAsync(scraperService, exportService, statisticService, logger);
                        break;
                    
                    case "2":
                        await DisplayStatisticsFromFileAsync(statisticService, logger);
                        break;
                    
                    case "3":
                        exitRequested = true;
                        logger.Info("Exiting application...");
                        break;
                    
                    default:
                        logger.Error("Invalid option. Please try again.");
                        break;
                }
                
                if (!exitRequested)
                {
                    logger.Info("Press Enter to return to the main menu...");
                    Console.ReadLine();
                    Console.Clear();
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error("An error occurred during execution", ex);
        }
    }
    
    private static void DisplayMenu()
    {
        Console.WriteLine("=== Indonesian Regional Data Scraper ===");
        Console.WriteLine("1. Scrape data and save to files");
        Console.WriteLine("2. Display statistics from existing file");
        Console.WriteLine("3. Exit");
        Console.Write("Select an option (1-3): ");
    }
    
    private static async Task ScrapeAndSaveDataAsync(
        IWilayahScraperService scraperService, 
        IDataExportService exportService, 
        IStatisticService statisticService,
        ILogger logger)
    {
        logger.Info("Starting Indonesian regional data scraping process...");
        
        // Scrape all data
        List<WilayahData> allData = await scraperService.ScrapeAllWilayahAsync();
        
        // Export data to files
        exportService.SaveToCsv(allData, CSV_FILENAME);
        exportService.SaveToJson(allData, JSON_FILENAME);
        
        // Display statistics
        statisticService.DisplayStatistics(allData);
        
        logger.Success("Scraping and saving completed successfully!");
    }
    
    private static async Task DisplayStatisticsFromFileAsync(IStatisticService statisticService, ILogger logger)
    {
        logger.Info("Loading data from previously saved file...");
        
        if (!File.Exists(JSON_FILENAME))
        {
            logger.Error($"File not found: {JSON_FILENAME}. Please run the scraper first.");
            return;
        }
        
        try
        {
            string jsonContent = await File.ReadAllTextAsync(JSON_FILENAME);
            var allData = JsonSerializer.Deserialize<List<WilayahData>>(jsonContent);
            
            if (allData == null || allData.Count == 0)
            {
                logger.Error("No data found in the file or file is empty.");
                return;
            }
            
            logger.Success($"Loaded {allData.Count} records from {JSON_FILENAME}");
            statisticService.DisplayStatistics(allData);
        }
        catch (Exception ex)
        {
            logger.Error($"Error loading data from {JSON_FILENAME}", ex);
        }
    }
}