namespace AuthZTests;

public class HttpRequestModelTests
{
    static readonly HttpRequestMessage httpRequestMessage = new();
    static HttpRequestMessageModel httpRequestMessageModel;
    static readonly string testUrl = "https://graph.microsoft.com/v1.0/users?$select=id,displayName";

    [SetUp]
    public void Setup()
    {
        // Add method and URL
        httpRequestMessage.Method = HttpMethod.Get;
        httpRequestMessage.RequestUri = new Uri(testUrl);

        // Add Authorization header, sample from https://jwt.io/
        httpRequestMessage.Headers.Authorization = new("bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c");

        // Add body
        var body = System.Text.Json.JsonSerializer.Serialize(
            new
            {
                displayName = "Michael Mainer",
                jobTitle = "SWE",
                mail = "michael@chambele.onmicrosoft.com",
                mobilePhone = "5128675309",
                id = "5cae63f5-0216-4766-8ca3-84d5e288e796"
            });

        httpRequestMessage.Content = new StringContent(body);

        // Convert System.Net.Http.HttpRequestMessage to the model we will evaluate in the policy
        httpRequestMessageModel = new HttpRequestMessageModel(httpRequestMessage);
    }

    [Test]
    public void Test1()
    {
        Assert.IsTrue(httpRequestMessageModel.Method == HttpMethod.Get.ToString());
        Assert.IsTrue(httpRequestMessageModel.Path == httpRequestMessage.RequestUri?.LocalPath);
        Assert.IsTrue(httpRequestMessageModel.Headers.TryGetValue("Authorization", out var authz));
        Assert.IsTrue(httpRequestMessageModel.QueryParameters?.TryGetValue("$select", out var select));
        Assert.IsNotEmpty(httpRequestMessageModel.Body);
    }
}