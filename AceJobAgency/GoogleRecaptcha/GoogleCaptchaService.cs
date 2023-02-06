using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AceJobAgency.GoogleRecaptcha
{
	public class GoogleCaptchaService
	{
		private GoogleCaptchaConfig _config;

		public GoogleCaptchaService(IOptions<GoogleCaptchaConfig> config)
		{
			_config = config.Value;
		}

		public virtual async Task<GoogleCaptchaResponse> ResVer(string _Token)
		{
			GoogleCaptchaData _MyData = new GoogleCaptchaData
			{
				response = _Token,
				secret = _config.SecretKey,

			};
			HttpClient client = new HttpClient();
			var response = await client.GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?=secret{_MyData.secret}&response={_MyData.response} ");
			var caps = JsonConvert.DeserializeObject<GoogleCaptchaResponse>(response);
			return caps;
		}
	}
	public class GoogleCaptchaData
	{
		//token
		public string response { get; set; }
		public string secret { get; set; }
	}
	
}
