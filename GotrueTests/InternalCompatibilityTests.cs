using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supabase.Gotrue;

namespace GotrueTests
{
    [TestClass]
    public class InternalCompatibilityTests
    {
        [TestMethod("Gotrue: JwtPayloadDecoder decodes aal and amr claims")]
        public void JwtPayloadDecoderDecodesExpectedClaims()
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("37c304f8-51aa-419a-a1af-06154e63707a"));

            var descriptor = new SecurityTokenDescriptor
            {
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature),
                Claims = new Dictionary<string, object>
                {
                    ["aal"] = "aal2",
                    ["amr"] = new[]
                    {
                        new Dictionary<string, object>
                        {
                            ["method"] = "password",
                            ["timestamp"] = 1710000000L
                        }
                    }
                }
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(tokenHandler.CreateToken(descriptor));

            var payload = JwtPayloadDecoder.Decode(token);

            Assert.AreEqual("aal2", payload.AuthenticatorAssuranceLevel);
            Assert.AreEqual(1, payload.CurrentAuthenticationMethods.Length);
            Assert.AreEqual("password", payload.CurrentAuthenticationMethods[0].Method);
            Assert.AreEqual(1710000000L, payload.CurrentAuthenticationMethods[0].Timestamp);
            Assert.IsTrue(payload.ValidToUtc > payload.IssuedAtUtc);
        }

        [TestMethod("Gotrue: QueryStringCollection preserves multiple values and spaces")]
        public void QueryStringCollectionPreservesMultipleValuesAndSpaces()
        {
            var query = QueryStringCollection.Parse("?provider=google&scopes=special+scopes+please");

            query.Add("prompt", "consent");

            Assert.AreEqual("google", query.Get("provider"));
            Assert.AreEqual("special scopes please", query.Get("scopes"));
            Assert.AreEqual("provider=google&scopes=special+scopes+please&prompt=consent", query.ToString());
        }
    }
}
