using Newtonsoft.Json;

namespace Supabase.Gotrue.Mfa
{
	public class MfaAdminDeleteFactorResponse
	{
		// Id of the factor that was successfully deleted
		[JsonProperty("id")]
		public string Id { get; set; }
	}
}
