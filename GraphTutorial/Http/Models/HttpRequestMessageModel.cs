using System.Text.Json.Serialization;
using System.Net.Http;
using static Microsoft.Graph.CoreConstants;

namespace GraphTutorial.Http.Models
{
    public class HttpRequestMessageModel
    {
        public HttpRequestMessageModel(HttpRequestMessage httpRequestMessage)
        {
            Method = httpRequestMessage.Method.ToString();
            Url = httpRequestMessage.RequestUri.ToString();
            
            var headers = new Dictionary<String, String>(httpRequestMessage.Headers.Count());

            foreach (var header in httpRequestMessage.Headers)
            {   
                // Note that there can be more than one value per key.
                headers.Add(header.Key, header.Value.First());
            }

            Headers = headers;

            Body = httpRequestMessage?.Content is StringContent ? httpRequestMessage.Content.ReadAsStringAsync().Result : string.Empty;



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