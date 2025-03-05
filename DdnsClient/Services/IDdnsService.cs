using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using DdnsClient.Dynu;

namespace DdnsClient.Services;

internal interface IDdnsService
{
    Task UpdateAsync();
    Task<bool> GetIpFromDdnsAsync();
}

internal class DdnsService : IDdnsService
{
    //private const string HostName = "askholmen.freeddns.org"; //83.233.228.4

    private IPAddress _ddnsPublicIp = IPAddress.None;
    private readonly ILogger<DdnsService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _aPIUrl;
    private readonly string _apiKey;
    private readonly string _hostName;

    public DdnsService(ILogger<DdnsService> logger, IConfiguration configuration, HttpClient httpClient)
    {
        _logger = logger;
        _aPIUrl = configuration.GetValue("Ddns:APIUrl", string.Empty)!;
        _apiKey = configuration.GetValue("Ddns:ApiKey", string.Empty)!;
        _hostName = configuration.GetValue("Ddns:Domain", string.Empty)!;
        _httpClient = httpClient;
    }

    private async Task<Domain> GetDomainFromDdnsAsync()
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Add("API-Key", _apiKey);
        try
        {
            var response = await _httpClient.GetAsync(_aPIUrl);
            if (response.IsSuccessStatusCode)
            {
                var root = await response.Content.ReadFromJsonAsync<Root>();
                if (root is not null)
                {
                    var domain = root.Domains.FirstOrDefault(s => s.Name == _hostName);
                    if (domain is not null)
                    {
                        return domain;
                    }
                }
            }
            else
            {
                _logger.LogWarning("Failed to get domain from Ddns {statuscode}", response.ReasonPhrase);
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get domain from Ddns");
        }
        return null!;
    }

    private async Task<bool> SetDomainToDdnsAsync(Domain domain, IPAddress iPAddress)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Add("API-Key", _apiKey);
        DomainUpdate domainUpdate = new DomainUpdate(
            _hostName,
            iPAddress.ToString(),
            domain.Ttl,
            domain.Ipv4,
            domain.Ipv6,
            domain.Ipv4WildcardAlias,
            domain.Ipv6WildcardAlias);
        var s = JsonSerializer.Serialize(domainUpdate);
        try
        {
            var response = await _httpClient.PostAsync($"{_aPIUrl}/{domain.Id}", new StringContent(s, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Updated IP address for {host} to {ip}", _hostName, iPAddress);
                return true;
            }
            else
            {
                _logger.LogWarning("Failed to update IP address for {host} to {ip}", _hostName, iPAddress);
                return false;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update domain to Ddns");
        }
        return false;
    }

    public async Task<bool> GetIpFromDdnsAsync()
    {
        try
        {
            var domain = await GetDomainFromDdnsAsync();
            if (domain is not null)
            {
                _ddnsPublicIp = IPAddress.Parse(domain.Ipv4Address);
                _logger.LogInformation("Public IP from Ddns for {host}: {ip}", _ddnsPublicIp, domain.Name);
                return true;
            }
            _logger.LogWarning("Failed to get public IP address from Ddns");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get public IP address from Ddns");
            return false;
        }
    }

    public async Task UpdateAsync()
    {
        _httpClient.DefaultRequestHeaders.Clear();
        IPAddress ip = await Ipify.GetPublicIPAddress(_httpClient);
        if (ip == IPAddress.None)
        {
            _logger.LogWarning("Failed to get public IP address");
            return;
        }

        if (_ddnsPublicIp.Equals(ip))
        {
            _logger.LogInformation("Public IP: {ip} is the same as Ddns IP {ip}", ip, _ddnsPublicIp);
            return;
        }
        else
        {
            _logger.LogInformation("Public IP is different from Ddns IP");
            var domain = await GetDomainFromDdnsAsync();
            if (domain is not null)
            {
                if (await SetDomainToDdnsAsync(domain, ip))
                {
                    _ddnsPublicIp = ip;
                    _logger.LogInformation("Updated Ddns IP to {ip}", ip);
                }
            }
        }
    }
}
