using ScrapeWilayah.ApiService;
using ScrapeWilayah.Logger;
using ScrapeWilayah.Model;
namespace ScrapeWilayah.ScraperService;

public class WilayahScraperService(IWilayahApiService apiService, ILogger logger, int delayMs = 300)
    : IWilayahScraperService
{
    public async Task<List<WilayahData>> ScrapeAllWilayahAsync()
    {
        List<WilayahData> allData = new List<WilayahData>();
        logger.Info("Starting to scrape provinces...");
        
        var provinsiList = await apiService.GetWilayahAsync("provinsi");
        logger.Info($"Found {provinsiList.Count} provinces");
        
        for (int i = 0; i < provinsiList.Count; i++)
        {
            var provinsi = provinsiList[i];
            var provinsiData = await ScrapeProvinsiAsync(provinsi, i + 1, provinsiList.Count);
            allData.AddRange(provinsiData);
        }
        
        logger.Success($"Scraping completed. Total records: {allData.Count}");
        return allData;
    }

    private async Task<List<WilayahData>> ScrapeProvinsiAsync(dynamic provinsi, int index, int total)
    {
        List<WilayahData> provinsiData = new List<WilayahData>();
        logger.Info($"Processing province {index}/{total}: {provinsi.nama_bps}");
        
        var kabupatenList = await apiService.GetWilayahAsync("kabupaten", provinsi.kode_bps);
        
        for (int i = 0; i < kabupatenList.Count; i++)
        {
            var kabupaten = kabupatenList[i];
            var kabupatenData = await ScrapeKabupatenAsync(provinsi, kabupaten, i + 1, kabupatenList.Count);
            provinsiData.AddRange(kabupatenData);
        }
        
        return provinsiData;
    }

    private async Task<List<WilayahData>> ScrapeKabupatenAsync(dynamic provinsi, dynamic kabupaten, int index, int total)
    {
        List<WilayahData> kabupatenData = new List<WilayahData>();
        logger.Info($"  Processing district {index}/{total}: {kabupaten.nama_bps}");
        
        var kecamatanList = await apiService.GetWilayahAsync("kecamatan", kabupaten.kode_bps);
        
        foreach (var kecamatan in kecamatanList)
        {
            var wilayahData = CreateWilayahData(provinsi, kabupaten, kecamatan);
            kabupatenData.Add(wilayahData);
        }
        
        // Delay to prevent overwhelming the server
        if (delayMs > 0)
        {
            await Task.Delay(delayMs);
        }
        
        return kabupatenData;
    }

    private WilayahData CreateWilayahData(dynamic provinsi, dynamic kabupaten, dynamic kecamatan)
    {
        return new WilayahData
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
    }
}