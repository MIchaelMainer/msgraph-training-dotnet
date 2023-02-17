namespace AuthZTests;

public class DisallowAllButOneWritePath
{
    public readonly string PolicyName = AuthZPolicy.DisallowAllButOneWritePath.ToString();

    [Test]
    public async Task DisallowAllButOneWritePath_AllowsGETV1Users()
    {
        Assert.IsTrue(await TestHelpers.IsAllowedAsync(PolicyName, "/v1.0/users", HttpMethod.Get));
    }

    [Test]
    public async Task DisallowAllButOneWritePath_AllowsGETBetaUsers()
    {
        Assert.IsTrue(await TestHelpers.IsAllowedAsync(PolicyName, "/beta/users", HttpMethod.Get));
    }

    [Test]
    public async Task DisallowAllButOneWritePath_AllowsPOSTV1Users()
    {
        Assert.IsTrue(await TestHelpers.IsAllowedAsync(PolicyName, "/v1.0/users", HttpMethod.Post));
    }

    [Test]
    public async Task DisallowAllButOneWritePath_DisallowsPOSTBetaUsers()
    {
        Assert.IsFalse(await TestHelpers.IsAllowedAsync(PolicyName, "/beta/users", HttpMethod.Post));
    }

    [Test]
    public async Task DisallowAllButOneWritePath_DisallowsDeletionOfUsers()
    {
        Assert.IsFalse(await TestHelpers.IsAllowedAsync(PolicyName, "/v1.0/users/c7fc7171-6fd3-4e86-b059-c964ded904b9", HttpMethod.Delete));
    }


    [Test]
    public async Task DisallowAllButOneWritePath_AllowsGetAdminConsentPolicy()
    {
        Assert.IsTrue(await TestHelpers.IsAllowedAsync(PolicyName, "/v1.0/policies/adminConsentRequestPolicy", HttpMethod.Get));
    }
}