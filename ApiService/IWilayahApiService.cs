using ScrapeWilayah.Model;

namespace ScrapeWilayah.ApiService;

public interface IWilayahApiService
{
    Task<List<WilayahResponse>> GetWilayahAsync(string level, string parent = "0", string periode_merge = "2023_1.2022");
}