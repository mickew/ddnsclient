using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace DdnsClient;

internal static class Ipify
{
    private static async Task<string> GetPublicAddressFromipify(HttpClient httpClient, bool useHttps = false)
    {
        var endpoint = useHttps ? "https://api.ipify.org" : "http://api.ipify.org";

        try
        {
            var response = await httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

        }
        catch (Exception)
        {
        }
        return string.Empty;
    }

    private static async Task<string> GetPublicAddressFromifconfig(HttpClient httpClient, bool useHttps = false)
    {
        var endpoint = useHttps ? "https://ifconfig.co/json" : "http://ifconfig.co/json";

        try
        {
            var response = await httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var ifconfig = await response.Content.ReadFromJsonAsync<IfConfig>();
                if (ifconfig != null)
                {
                    return ifconfig.Ip;
                }
            }

        }
        catch (Exception)
        {
        }
        return string.Empty;
    }

    public static async Task<IPAddress> GetPublicIPAddress(HttpClient httpClient, bool useHttps = false)
    {
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        string address = await GetPublicAddressFromipify(httpClient, useHttps);
        if (string.IsNullOrWhiteSpace(address))
        {
            address = await GetPublicAddressFromifconfig(httpClient, useHttps);
        }
        IPAddress ipAddress;
        if (!IPAddress.TryParse(address, out ipAddress!))
        {
            return IPAddress.None;
        }
        return ipAddress;
    }

    public record IfConfig(
        [property: JsonPropertyName("ip")] string Ip
    );
}
