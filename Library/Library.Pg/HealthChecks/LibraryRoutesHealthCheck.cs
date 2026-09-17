using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Library.Pg.HealthChecks;

public class LibraryRoutesHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LibraryRoutesHealthCheck(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("LibraryApiClient");
            

            var response = await client.GetAsync("api/controller", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy("LibraryController HTTP routes are accessible and responding with success status codes.");
            }

            return HealthCheckResult.Unhealthy($"LibraryController API route returned unexpected status code: {(int)response.StatusCode} ({response.StatusCode}).");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                description: "Failed to reach LibraryController HTTP endpoints. The service might be down or misconfigured.", 
                exception: ex);
        }
    }
}