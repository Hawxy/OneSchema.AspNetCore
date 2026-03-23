using System.Net;
using OneSchema.AspNetCore.Tests.Infrastructure;

namespace OneSchema.AspNetCore.Tests;

[ClassDataSource<AlbaFixture>(Shared = SharedType.PerTestSession)]
public class JwtAuthenticationTests(AlbaFixture albaBootstrap) : AlbaTestBase(albaBootstrap)
{
    [Test]
    public async Task Valid_jwt_allows_request()
    {
        var body = new ValidationHookRequestBuilder()
            .WithJwt(TestConstants.GenerateValidJwt())
            .AddRow(1, new()
            {
                ["first_name"] = "Jane",
                ["last_name"] = "Doe",
                ["email"] = "jane@example.com"
            })
            .BuildJson();

        await Host.Scenario(s =>
        {
            s.Post.RawJson(body).ToUrl("/webhooks/validate-contacts");
            s.StatusCodeShouldBe(HttpStatusCode.OK);
        });
    }

    [Test]
    public async Task Missing_jwt_returns_unauthorized()
    {
        var body = new ValidationHookRequestBuilder()
            // No JWT set — defaults to empty string
            .AddRow(1, new()
            {
                ["first_name"] = "Jane",
                ["last_name"] = "Doe",
                ["email"] = "jane@example.com"
            })
            .BuildJson();

        await Host.Scenario(s =>
        {
            s.Post.RawJson(body).ToUrl("/webhooks/validate-contacts");
            s.StatusCodeShouldBe(HttpStatusCode.Unauthorized);
        });
    }

    [Test]
    public async Task Invalid_jwt_signature_returns_unauthorized()
    {
        var body = new ValidationHookRequestBuilder()
            .WithJwt(TestConstants.GenerateJwtWithWrongSecret())
            .AddRow(1, new()
            {
                ["first_name"] = "Jane",
                ["last_name"] = "Doe",
                ["email"] = "jane@example.com"
            })
            .BuildJson();

        await Host.Scenario(s =>
        {
            s.Post.RawJson(body).ToUrl("/webhooks/validate-contacts");
            s.StatusCodeShouldBe(HttpStatusCode.Unauthorized);
        });
    }

    [Test]
    public async Task Tampered_jwt_returns_unauthorized()
    {
        var validJwt = TestConstants.GenerateValidJwt();
        // Corrupt the signature by flipping the last character
        var tampered = validJwt[..^1] + (validJwt[^1] == 'A' ? 'B' : 'A');

        var body = new ValidationHookRequestBuilder()
            .WithJwt(tampered)
            .AddRow(1, new()
            {
                ["first_name"] = "Jane",
                ["last_name"] = "Doe",
                ["email"] = "jane@example.com"
            })
            .BuildJson();

        await Host.Scenario(s =>
        {
            s.Post.RawJson(body).ToUrl("/webhooks/validate-contacts");
            s.StatusCodeShouldBe(HttpStatusCode.Unauthorized);
        });
    }

    [Test]
    public async Task Unprotected_endpoint_allows_request_without_jwt()
    {
        var body = new ValidationHookRequestBuilder()
            // No JWT
            .AddRow(1, new() { ["sku"] = "ABC-001", ["name"] = "Widget", ["price"] = "9.99" })
            .BuildJson();

        await Host.Scenario(s =>
        {
            s.Post.RawJson(body).ToUrl("/webhooks/validate-products");
            s.StatusCodeShouldBe(HttpStatusCode.OK);
        });
    }
}
