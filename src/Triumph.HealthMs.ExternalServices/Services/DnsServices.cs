namespace Triumph.HealthMs.ExternalServices.Services;

public sealed class DnsServices (
    IHttpClientFactory httpClientFactory,
    ILogger<DnsServices> logger,
    AppSettings appSettings)
    : IDnsServices
{
    public async Task<bool> CreateSubdomain(string subdomain, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient("digitalocean");

        var payload = new
        {
            type = "CNAME",
            name = $"{subdomain}.facilities-app",
            data = appSettings.VercelCname,
            ttl = 3600
        };

        var response =
            await client.PostAsJsonAsync($"domains/{appSettings.MainDomain}/records", payload, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("There was a problem creating subdomain on digital ocean. Error: {Error}, Payload: {Subdomain}", error, subdomain);
            return false;
        }
        
        return true;
    }

    public async Task DeleteSubdomain(string subdomain, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient("digitalocean");

        var listResponse = await client.GetAsync($"domains/{appSettings.MainDomain}/records", cancellationToken);
        listResponse.EnsureSuccessStatusCode();

        var json = await listResponse.Content.ReadFromJsonAsync<JsonElement>(cancellationToken);
        var record = json.GetProperty("domain_records")
            .EnumerateArray()
            .FirstOrDefault(r => r.GetProperty("name").GetString() == subdomain);

        if (record.ValueKind == JsonValueKind.Undefined) return;

        var recordId = record.GetProperty("id").GetInt32();
        await client.DeleteAsync($"domains/{appSettings.MainDomain}/records/{recordId}", cancellationToken);
    }
}