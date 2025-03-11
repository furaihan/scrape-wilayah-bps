using ScrapeWilayah.Logger;
using ScrapeWilayah.Model;

namespace ScrapeWilayah.StatisticService;

public class StatisticService(ILogger logger) : IStatisticService
{
    public void DisplayStatistics(List<WilayahData> data)
    {
        logger.Info("Generating statistics...");
            
        // Count unique provinces, districts, and subdistricts
        var totalProvinsi = data.Select(x => x.nama_provinsi).Distinct().Count();
        var totalKabupaten = data.Select(x => x.nama_kabupaten).Distinct().Count();
        var totalKecamatan = data.Select(x => x.nama_kecamatan).Distinct().Count();

        logger.Statistic("Statistics for Indonesian Regional Data:");
        logger.Statistic($"Total Provinces: {totalProvinsi}");
        logger.Statistic($"Total Districts/Cities: {totalKabupaten}");
        logger.Statistic($"Total Subdistricts: {totalKecamatan}");

        // Provinces with the most subdistricts
        var provinsiTerbanyak = data.GroupBy(x => x.nama_provinsi)
            .Select(g => new { Provinsi = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(5);

        logger.Statistic("\nProvinces with Most Subdistricts:");
        foreach (var item in provinsiTerbanyak)
        {
            logger.Statistic($"- {item.Provinsi}: {item.Count} subdistricts");
        }
    }
}