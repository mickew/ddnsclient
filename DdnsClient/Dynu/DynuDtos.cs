using System.Text.Json.Serialization;

namespace DdnsClient.Dynu;

public record DomainUpdate(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("ipv4Address")] string Ipv4Address,
    [property: JsonPropertyName("ttl")] int Ttl,
    [property: JsonPropertyName("ipv4")] bool Ipv4,
    [property: JsonPropertyName("ipv6")] bool Ipv6,
    [property: JsonPropertyName("ipv4WildcardAlias")] bool Ipv4WildcardAlias,
    [property: JsonPropertyName("ipv6WildcardAlias")] bool Ipv6WildcardAlias
);

public record Domain(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("unicodeName")] string UnicodeName,
    [property: JsonPropertyName("token")] string Token,
    [property: JsonPropertyName("state")] string State,
    [property: JsonPropertyName("group")] string Group,
    [property: JsonPropertyName("ipv4Address")] string Ipv4Address,
    [property: JsonPropertyName("ipv6Address")] object Ipv6Address,
    [property: JsonPropertyName("ttl")] int Ttl,
    [property: JsonPropertyName("ipv4")] bool Ipv4,
    [property: JsonPropertyName("ipv6")] bool Ipv6,
    [property: JsonPropertyName("ipv4WildcardAlias")] bool Ipv4WildcardAlias,
    [property: JsonPropertyName("ipv6WildcardAlias")] bool Ipv6WildcardAlias,
    [property: JsonPropertyName("createdOn")] DateTime CreatedOn,
    [property: JsonPropertyName("updatedOn")] DateTime UpdatedOn
);

public record Root(
    [property: JsonPropertyName("statusCode")] int StatusCode,
    [property: JsonPropertyName("domains")] IReadOnlyList<Domain> Domains
);
