using ScrapeWilayah.Model;

namespace ScrapeWilayah.StatisticService;

public interface IStatisticService
{
    void DisplayStatistics(List<WilayahData> data);
}