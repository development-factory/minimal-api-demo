namespace MinimalApiDemo.Configuration;

public class ScraperConfig
{
    public string SourceUrl { get; set; }
    public string CssClass { get; set; }
    public string AddressesSeparator { get; set; }
    public IEnumerable<string> Exclusions { get; set; }
}
