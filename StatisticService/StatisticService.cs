using ScrapeWilayah.Logger;
using ScrapeWilayah.Model;

namespace ScrapeWilayah.StatisticService;

public class StatisticsService : IStatisticsService
{
    private readonly ILogger _logger;

    public StatisticsService(ILogger logger)
    {
        _logger = logger;
    }

    public void DisplayStatistics(List<WilayahData> data)
    {
        _logger.Info("Generating statistics...");
            
        // Count unique provinces, districts, and subdistricts
        var totalProvinsi = data.Select(x => x.nama_provinsi).Distinct().Count();
        var totalKabupaten = data.Select(x => x.nama_kabupaten).Distinct().Count();
        var totalKecamatan = data.Select(x => x.nama_kecamatan).Distinct().Count();

        _logger.Statistic("Statistics for Indonesian Regional Data:");
        _logger.Statistic($"Total Provinces: {totalProvinsi}");
        _logger.Statistic($"Total Districts/Cities: {totalKabupaten}");
        _logger.Statistic($"Total Subdistricts: {totalKecamatan}");

        // Provinces with the most subdistricts
        var provinsiTerbanyak = data.GroupBy(x => x.nama_provinsi)
            .Select(g => new { Provinsi = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(5);

        _logger.Statistic("\nProvinces with Most Subdistricts:");
        foreach (var item in provinsiTerbanyak)
        {
            _logger.Statistic($"- {item.Provinsi}: {item.Count} subdistricts");
        }
    }
}