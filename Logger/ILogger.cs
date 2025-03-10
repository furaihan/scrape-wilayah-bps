namespace ScrapeWilayah.Logger;

public interface ILogger
{
    void Info(string message);
    void Error(string message, Exception ex = null);
    void Success(string message);
    void Warning(string message);
    void Statistic(string message);
}