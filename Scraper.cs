using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using RestSharp;
using MinimalApiDemo.Configuration;
using MinimalApiDemo.Models;
using System.Globalization;

namespace MinimalApiDemo;

public class Scraper : IScraper
{
    private readonly IRestClient _restClient;
    private readonly ILogger<Scraper> _logger;
    private readonly ScraperConfig _scraperConfig;

    public Scraper(IRestClient restClient, ILogger<Scraper> logger, IOptions<ScraperConfig> scraperConfig)
    {
        _restClient = restClient;
        _logger = logger;
        _scraperConfig = scraperConfig.Value;
    }

    public IEnumerable<Failure> ParseFailures(string sectorFilter)
    {
        List<Failure> failures = new();
        var rows = ParseTable(_scraperConfig.CssClass, sectorFilter)?.Descendants("tr");

        foreach (var row in rows)
        {
            try
            {
                var cells = row.SelectNodes("td")?.Select(node => node.InnerText);
                var addresses = cells?.ElementAt(1)?.Split(_scraperConfig.AddressesSeparator);
                if (addresses is null) continue;
                foreach (var address in addresses)
                {
                    if (_scraperConfig.Exclusions.Any(e => address.Contains(e))) continue;
                    _ = int.TryParse(cells.ElementAt(0), out int sector);
                    var until = DateTime.ParseExact(cells.ElementAt(4), "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
                    failures.Add(new Failure
                    {
                        Sector = sector,
                        Address = address.Trim(),
                        FailureType = cells.ElementAt(2).Trim(),
                        Cause = cells.ElementAt(3).Trim(),
                        Until = until,
                        DateAndTime = DateTime.Now
                    });
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error encountered when scraping page: {e.Message}, {e.StackTrace}");
            }
        }
        return failures;
    }

    public HtmlNode ParseTable(string cssClass, string sectorFilter)
    {
        string content = GetPageContent();
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(content);
        HtmlNode result = string.IsNullOrWhiteSpace(sectorFilter) ?
            htmlDocument.DocumentNode.SelectSingleNode($"//table[@class='{cssClass}']") :
            htmlDocument.DocumentNode.SelectSingleNode($"//div[@id='S{sectorFilter}']");
        return result;
    }

    public string GetPageContent()
    {
        var response = _restClient.Execute(new RestRequest(_scraperConfig.SourceUrl, Method.GET));
        return response.Content;
    }
}
