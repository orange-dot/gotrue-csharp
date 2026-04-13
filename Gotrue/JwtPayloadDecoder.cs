using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Supabase.Gotrue.Mfa;

namespace Supabase.Gotrue
{
internal static class JwtPayloadDecoder
{
    public static JwtPayloadData Decode(string jwt)
    {
        if (string.IsNullOrWhiteSpace(jwt))
        {
            throw new ArgumentException("JWT cannot be empty.", nameof(jwt));
        }

        var segments = jwt.Split('.');

        if (segments.Length < 2)
        {
            throw new InvalidOperationException("JWT was not in a recognized format.");
        }

        var payloadJson = DecodeBase64Url(segments[1]);
        var payload = JsonConvert.DeserializeObject<JObject>(payloadJson)
            ?? throw new InvalidOperationException("JWT payload could not be parsed.");

        var issuedAt = ReadUnixTime(payload, "iat");
        var validTo = ReadUnixTime(payload, "exp");

        if (issuedAt is null || validTo is null)
        {
            throw new InvalidOperationException("JWT payload did not include valid iat/exp claims.");
        }

        var amrToken = payload["amr"];

        return new JwtPayloadData
        {
            IssuedAtUtc = issuedAt.Value,
            ValidToUtc = validTo.Value,
            AuthenticatorAssuranceLevel = payload["aal"]?.ToString(),
            CurrentAuthenticationMethods = amrToken is JArray amrArray
                ? amrArray.ToObject<AmrEntry[]>() ?? Array.Empty<AmrEntry>()
                : Array.Empty<AmrEntry>()
        };
    }

    private static DateTime? ReadUnixTime(JObject payload, string claimName)
    {
        var token = payload[claimName];

        if (token is null)
        {
            return null;
        }

        if (!TryGetLong(token, out var unixSeconds))
        {
            return null;
        }

        return DateTimeOffset.FromUnixTimeSeconds(unixSeconds).UtcDateTime;
    }

    private static bool TryGetLong(JToken token, out long value)
    {
        switch (token.Type)
        {
            case JTokenType.Integer:
                value = token.Value<long>();
                return true;
            case JTokenType.Float:
                value = Convert.ToInt64(token.Value<double>());
                return true;
            case JTokenType.String:
                return long.TryParse(token.Value<string>(), out value);
            default:
                value = default;
                return false;
        }
    }

    private static string DecodeBase64Url(string value)
    {
        var normalized = value.Replace('-', '+').Replace('_', '/');

        switch (normalized.Length % 4)
        {
            case 2:
                normalized += "==";
                break;
            case 3:
                normalized += "=";
                break;
            case 0:
                break;
            default:
                throw new InvalidOperationException("JWT payload used an invalid base64url length.");
        }

        var bytes = Convert.FromBase64String(normalized);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }
}
}
