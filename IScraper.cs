using MinimalApiDemo.Models;

namespace MinimalApiDemo;

public interface IScraper
{
    IEnumerable<Failure> ParseFailures(string sectorFilter);
}