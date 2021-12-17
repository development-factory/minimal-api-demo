using Microsoft.AspNetCore.Mvc;
using MinimalApiDemo.Configuration;
using RestSharp;

namespace MinimalApiDemo.Extensions;

public static class ApiConfiguration
{
    public static void ConfigureEndpoints(this WebApplication app)
    {
        app.MapGet("/check", () =>
        {
            return Results.Ok(new
            {
                Message = "Welcome CMTEB Rest API"
            });
        });
        
        app.MapGet("/failures", ([FromQuery] string sector, IScraper scraper) =>
        {
            bool validInput = int.TryParse(sector, out int result);
            if (!validInput) return Results.BadRequest(new { ErrorMessage = "Sector parameter has to be numeric" });
            try
            {
                var failures = scraper.ParseFailures(sector);
                return Results.Ok(failures);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });
    }

    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.Configure<ScraperConfig>(builder.Configuration.GetSection("ScraperConfig"));

        builder.Services.AddScoped<IScraper, Scraper>();
        builder.Services.AddScoped<IRestClient, RestClient>();

        builder.Logging.AddLog4Net();
    }
}
