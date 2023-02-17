namespace AuthZTests;

public class DisallowGETsWithoutSelect
{
    public readonly string PolicyName = AuthZPolicy.DisallowGETsWithoutSelect.ToString();

    [Test]
    public async Task DisallowGETsWithoutSelect_DisallowsGETV1Users()
    {
        Assert.IsFalse(await TestHelpers.IsAllowedAsync(PolicyName, "/v1.0/users", HttpMethod.Get));
    }

    [Test]
    public async Task DisallowGETsWithoutSelect_DisallowsGETBetaUsers()
    {
        Assert.IsFalse(await TestHelpers.IsAllowedAsync(PolicyName, "/beta/users", HttpMethod.Get));
    }

    [Test]
    public async Task DisallowGETsWithoutSelect_AllowsPOSTV1Users()
    {
        Assert.IsTrue(await TestHelpers.IsAllowedAsync(PolicyName, "/v1.0/users", HttpMethod.Post));
    }

    [Test]
    public async Task DisallowGETsWithoutSelect_AllowsPOSTBetaUsers()
    {
        Assert.IsTrue(await TestHelpers.IsAllowedAsync(PolicyName, "/beta/users", HttpMethod.Post));
    }

    [Test]
    public async Task DisallowGETsWithoutSelect_AllowsDeletionOfUsers()
    {
        Assert.IsTrue(await TestHelpers.IsAllowedAsync(PolicyName, "/v1.0/users/c7fc7171-6fd3-4e86-b059-c964ded904b9", HttpMethod.Delete));
    }

    [Test]
    public async Task DisallowGETsWithoutSelect_DisallowsGetAdminConsentPolicy()
    {

        Assert.IsFalse(await TestHelpers.IsAllowedAsync(PolicyName, "/v1.0/policies/adminConsentRequestPolicy", HttpMethod.Get));
    }

    [Test]
    public async Task DisallowGETsWithoutSelect_AllowsGETV1UsersWithSelect()
    {
        Assert.IsTrue(await TestHelpers.IsAllowedAsync(PolicyName, "/v1.0/users?select=displayName", HttpMethod.Get));
    }

    [Test]
    public async Task DisallowGETsWithoutSelect_AllowsGETBetaUsersWithSelect()
    {
        Assert.IsTrue(await TestHelpers.IsAllowedAsync(PolicyName, "/beta/users?select=displayName", HttpMethod.Get));
    }

    [Test]
    public async Task DisallowGETsWithoutSelect_AllowsGetAdminConsentPolicyWithSelect()
    {
        Assert.IsTrue(await TestHelpers.IsAllowedAsync(PolicyName, "/v1.0/policies/adminConsentRequestPolicy?select=isEnabled", HttpMethod.Get));
    }

    [Test]
    public async Task DisallowGETsWithoutSelect_AllowsGETV1UsersWithDollarSelect()
    {
        Assert.IsTrue(await TestHelpers.IsAllowedAsync(PolicyName, "/v1.0/users?$select=displayName", HttpMethod.Get));
    }

    [Test]
    public async Task DisallowGETsWithoutSelect_AllowsGETBetaUsersWithDollarSelect()
    {
        Assert.IsTrue(await TestHelpers.IsAllowedAsync(PolicyName, "/beta/users?$select=displayName", HttpMethod.Get));
    }

    [Test]
    public async Task DisallowGETsWithoutSelect_AllowsGetAdminConsentPolicyWithDollarSelect()
    {
        Assert.IsTrue(await TestHelpers.IsAllowedAsync(PolicyName, "/v1.0/policies/adminConsentRequestPolicy?$select=isEnabled", HttpMethod.Get));
    }

    [Test]
    public async Task DisallowGETsWithoutSelect_AllowsGETV1UsersWithSelectAsSecondQueryStringParam()
    {
        Assert.IsTrue(await TestHelpers.IsAllowedAsync(PolicyName, "/v1.0/users?$filter=something&$select=displayName", HttpMethod.Get));
    }
}