using System.Text.Json.Serialization;
using System.Net.Http;

namespace GraphTutorial.Http.Models
{
    internal class HttpRequestMessage
    {
        internal HttpRequestMessage(HttpRequestMessage httpRequestMessage)
        {
            Method = httpRequestMessage.Method.ToString;
            Url = httpRequestMessage.Uri.ToString();
            Headers = httpRequestMessage.Headers;
            Body = httpRequestMessage.Content is StringContent ? httpRequestMessage.Content.ReadAsStringAsync().Result : string.Empty;
        }
        
        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("headers")]
        public IDictionary<string, string> Headers { get; set; }

        [JsonPropertyName("body")]
        public string Body { get; set; }
    }
}