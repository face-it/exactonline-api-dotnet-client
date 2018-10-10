using Newtonsoft.Json;
using System.Collections;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace ExactOnline.Client.Sdk.Helpers
{
	/// <summary>
	/// Class for stripping unnecessary Json tags from API Response
	/// </summary>
	public class ApiResponseCleaner
	{
	    private class ODataResponse
	    {
            [JsonProperty("d")]
            public RootNode Root { get; set; }
	    }

	    private class RootNode
	    {
            [JsonProperty("results")]
            public ArrayList Results { get; set; }
	    }

	    #region Public methods

		/// <summary>
		/// Fetch Json Object (Json within ['d'] name/value pair) from response
		/// </summary>
		/// <param name="response"></param>
		/// <returns></returns>
		public static string GetJsonObject(string response)
		{
		    var token = JToken.Parse(response);
		    var ar = token["d"];
		    return ar.ToString(Formatting.None);
		}

		public static string GetSkipToken(string response)
		{
		    var token = JToken.Parse(response);
		    var inner = token["d"];
		    if (inner.Type != JTokenType.Object)
		        return null;

		    var nextToken = inner["__next"];
		    if (nextToken == null)
		        return null;

		    var next = nextToken.ToString();
		    var match = Regex.Match(next, @"\$skiptoken=([^&#]*)");

		    // Extract the skip token
		    return match.Success ? match.Groups[1].Value : null;
		}

	    /// <summary>
	    /// Fetch Json Array (Json within ['d']['results']) from response
	    /// </summary>
	    public static string GetJsonArray(string response)
	    {
	        var token = JObject.Parse(response);
	        var results = token["d"];
	        if (results.Type == JTokenType.Array)
	            return results.ToString(Formatting.None);

	        var nested = results["results"];
	        return nested?.ToString(Formatting.None);
	    }

		#endregion
	}
}
