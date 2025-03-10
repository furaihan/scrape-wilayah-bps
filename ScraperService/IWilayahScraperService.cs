using ScrapeWilayah.Model;

namespace ScrapeWilayah.ScraperService;

public interface IWilayahScraperService
{
    Task<List<WilayahData>> ScrapeAllWilayahAsync();
}