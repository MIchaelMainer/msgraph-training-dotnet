using System.Text.Json.Serialization;
using System.Web;

namespace GraphTutorial.Http.Models
{
    public class HttpRequestMessageModel
    {
        public HttpRequestMessageModel(HttpRequestMessage httpRequestMessage)
        {
            Method = httpRequestMessage.Method.ToString();
            Path = httpRequestMessage.RequestUri?.LocalPath;

            if (!string.IsNullOrWhiteSpace(httpRequestMessage.RequestUri?.Query))
            {
                var queryParams = httpRequestMessage.RequestUri.Query.TrimStart('?').Split("&");
                QueryParameters = new Dictionary<String, String>(queryParams.Length);
                foreach (var queryParam in queryParams)
                {
                    var queryParamSegments = queryParam.Split("=");
                    QueryParameters.Add(queryParamSegments[0], HttpUtility.UrlDecode(queryParamSegments[1]));
                }
            }

            Headers = new Dictionary<String, String>(httpRequestMessage.Headers.Count());
            foreach (var header in httpRequestMessage.Headers)
            {
                // Note that there can be more than one value per key.
                Headers.Add(header.Key, header.Value.First());
            }

            Body = httpRequestMessage?.Content is StringContent ? httpRequestMessage.Content.ReadAsStringAsync().Result : string.Empty;
        }

        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("path")]
        public string? Path { get; set; }

        [JsonPropertyName("headers")]
        public IDictionary<string, string> Headers { get; set; }

        [JsonPropertyName("queryParameters")]
        public IDictionary<string, string>? QueryParameters { get; set; }

        [JsonPropertyName("body")]
        public string? Body { get; set; }
    }
}