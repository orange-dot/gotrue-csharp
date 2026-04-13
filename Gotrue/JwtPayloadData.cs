using System;
using Supabase.Gotrue.Mfa;

namespace Supabase.Gotrue
{
internal sealed class JwtPayloadData
{
    public DateTime IssuedAtUtc { get; set; }

    public DateTime ValidToUtc { get; set; }

    public string? AuthenticatorAssuranceLevel { get; set; }

    public AmrEntry[] CurrentAuthenticationMethods { get; set; } = Array.Empty<AmrEntry>();
}
}
