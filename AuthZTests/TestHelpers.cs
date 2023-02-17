namespace AuthZTests;

public static class TestHelpers
{
    const string Host = "https://graph.microsoft.com";

    public static async Task<bool> IsAllowedAsync(string policyName, string path, HttpMethod method)
    {
        var requestMessage = new HttpRequestMessage(method, $"{Host}{path}");
        return await AuthZHandler.IsAuthorizedAsync(policyName, new HttpRequestMessageModel(requestMessage));
    }
}