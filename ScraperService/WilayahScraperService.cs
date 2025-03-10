using ScrapeWilayah.ApiService;
using ScrapeWilayah.Logger;
using ScrapeWilayah.Model;

namespace ScrapeWilayah.ScraperService;

public class WilayahScraperService : IWilayahScraperService
    {
        private readonly IWilayahApiService _apiService;
        private readonly ILogger _logger;
        private readonly int _delayMs;

        public WilayahScraperService(IWilayahApiService apiService, ILogger logger, int delayMs = 300)
        {
            _apiService = apiService;
            _logger = logger;
            _delayMs = delayMs;
        }

        public async Task<List<WilayahData>> ScrapeAllWilayahAsync()
        {
            List<WilayahData> allData = new List<WilayahData>();

            _logger.Info("Starting to scrape provinces...");
            var provinsiList = await _apiService.GetWilayahAsync("provinsi");
            _logger.Info($"Found {provinsiList.Count} provinces");

            int provinsiCounter = 0;
            foreach (var provinsi in provinsiList)
            {
                provinsiCounter++;
                _logger.Info($"Processing province {provinsiCounter}/{provinsiList.Count}: {provinsi.nama_bps}");
                
                // Get districts for this province
                var kabupatenList = await _apiService.GetWilayahAsync("kabupaten", provinsi.kode_bps);
                
                int kabupatenCounter = 0;
                foreach (var kabupaten in kabupatenList)
                {
                    kabupatenCounter++;
                    _logger.Info($"  Processing district {kabupatenCounter}/{kabupatenList.Count}: {kabupaten.nama_bps}");
                    
                    // Get subdistricts for this district
                    var kecamatanList = await _apiService.GetWilayahAsync("kecamatan", kabupaten.kode_bps);
                    
                    foreach (var kecamatan in kecamatanList)
                    {
                        WilayahData data = new WilayahData
                        {
                            kode_provinsi = provinsi.kode_bps,
                            nama_provinsi = provinsi.nama_bps,
                            kode_provinsi_dagri = provinsi.kode_dagri,
                            nama_provinsi_dagri = provinsi.nama_dagri,
                            kode_kabupaten = kabupaten.kode_bps,
                            nama_kabupaten = kabupaten.nama_bps,
                            kode_kabupaten_dagri = kabupaten.kode_dagri,
                            nama_kabupaten_dagri = kabupaten.nama_dagri,
                            kode_kecamatan = kecamatan.kode_bps,
                            nama_kecamatan = kecamatan.nama_bps,
                            kode_kecamatan_dagri = kecamatan.kode_dagri,
                            nama_kecamatan_dagri = kecamatan.nama_dagri
                        };
                        allData.Add(data);
                    }
                    
                    // Delay to prevent overwhelming the server
                    if (_delayMs > 0)
                    {
                        await Task.Delay(_delayMs);
                    }
                }
            }

            _logger.Success($"Scraping completed. Total records: {allData.Count}");
            return allData;
        }
    }