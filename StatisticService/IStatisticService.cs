using ScrapeWilayah.Model;

namespace ScrapeWilayah.StatisticService;

public interface IStatisticsService
{
    void DisplayStatistics(List<WilayahData> data);
}