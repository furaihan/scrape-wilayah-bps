using System.Text.Json;
using ScrapeWilayah.Logger;
using ScrapeWilayah.Model;

namespace ScrapeWilayah.ApiService;

public class WilayahApiService : IWilayahApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    private readonly string _baseUrl = "https://sig.bps.go.id/rest-bridging/getwilayah";

    public WilayahApiService(ILogger logger)
    {
        _httpClient = new HttpClient();
        _logger = logger;
    }

    public async Task<List<WilayahResponse>> GetWilayahAsync(string level, string parent = "0", string periode_merge = "2023_1.2022")
    {
        string url = $"{_baseUrl}?level={level}&parent={parent}&periode_merge={periode_merge}";

        try
        {
            _logger.Info($"Making API request: {url}");
            HttpResponseMessage response = await _httpClient.GetAsync(url);
                
            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                var wilayahList = JsonSerializer.Deserialize<List<WilayahResponse>>(jsonString);
                _logger.Success($"Received {wilayahList?.Count ?? 0} {level} records for parent: {parent}");
                return wilayahList ?? new List<WilayahResponse>();
            }
            else
            {
                _logger.Error($"API request failed with status code: {response.StatusCode}");
                return new List<WilayahResponse>();
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"Exception during API request to {url}", ex);
            return new List<WilayahResponse>();
        }
    }
}
