namespace AuthZTests;

public class AllowUsersPath
{
    public readonly string PolicyName = AuthZPolicy.AllowUsersPath.ToString();

    [Test]
    public async Task AllowUsersPath_AllowsGETV1Users()
    {
        Assert.IsTrue(await TestHelpers.IsAllowedAsync(PolicyName, "/v1.0/users", HttpMethod.Get));
    }

    [Test]
    public async Task AllowUsersPath_AllowsGETBetaUsers()
    {
        Assert.IsTrue(await TestHelpers.IsAllowedAsync(PolicyName, "/beta/users", HttpMethod.Get));
    }

    [Test]
    public async Task AllowUsersPath_AllowsPOSTV1Users()
    {
        Assert.IsTrue(await TestHelpers.IsAllowedAsync(PolicyName, "/v1.0/users", HttpMethod.Post));
    }

    [Test]
    public async Task AllowUsersPath_AllowsPOSTBetaUsers()
    {
        Assert.IsTrue(await TestHelpers.IsAllowedAsync(PolicyName, "/beta/users", HttpMethod.Post));
    }

    [Test]
    public async Task AllowUsersPath_AllowsDeletionOfUsers()
    {
        Assert.IsTrue(await TestHelpers.IsAllowedAsync(PolicyName, "/v1.0/users/c7fc7171-6fd3-4e86-b059-c964ded904b9", HttpMethod.Delete));
    }

    [Test]
    public async Task AllowUsersPath_DisallowsGetAdminConsentPolicy()
    {
        Assert.IsFalse(await TestHelpers.IsAllowedAsync(PolicyName, "/v1.0/policies/adminConsentRequestPolicy", HttpMethod.Get));
    }
}