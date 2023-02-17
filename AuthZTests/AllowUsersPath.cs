using GraphTutorial.AuthZ;
using GraphTutorial.AuthZ.Models;
using GraphTutorial.Http.Models;

namespace AuthZTests;

public class AllowUsersPath
{
    [Test]
    public async Task AllowUsersPath_AllowsGETV1Users()
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/users");
        var isAllowed = await AuthZHandler.IsAuthorizedAsync(AuthZPolicy.AllowUsersPath.ToString(), new HttpRequestMessageModel(requestMessage));
        Assert.IsTrue(isAllowed);
    }

    [Test]
    public async Task AllowUsersPath_AllowsGETBetaUsers()
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/beta/users");
        var isAllowed = await AuthZHandler.IsAuthorizedAsync(AuthZPolicy.AllowUsersPath.ToString(), new HttpRequestMessageModel(requestMessage));
        Assert.IsTrue(isAllowed);
    }

    [Test]
    public async Task AllowUsersPath_AllowsPOSTV1Users()
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://graph.microsoft.com/v1.0/users");
        var isAllowed = await AuthZHandler.IsAuthorizedAsync(AuthZPolicy.AllowUsersPath.ToString(), new HttpRequestMessageModel(requestMessage));
        Assert.IsTrue(isAllowed);
    }

    [Test]
    public async Task AllowUsersPath_AllowsPOSTBetaUsers()
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://graph.microsoft.com/beta/users");
        var isAllowed = await AuthZHandler.IsAuthorizedAsync(AuthZPolicy.AllowUsersPath.ToString(), new HttpRequestMessageModel(requestMessage));
        Assert.IsTrue(isAllowed);
    }


    [Test]
    public async Task AllowUsersPath_DisallowsAnAdminPath()
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/policies/adminConsentRequestPolicy");
        var isAllowed = await AuthZHandler.IsAuthorizedAsync(AuthZPolicy.AllowUsersPath.ToString(), new HttpRequestMessageModel(requestMessage));
        Assert.IsFalse(isAllowed);
    }
}