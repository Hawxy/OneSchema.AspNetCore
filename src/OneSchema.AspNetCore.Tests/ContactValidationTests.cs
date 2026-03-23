using System.Net;
using OneSchema.AspNetCore.Api;
using OneSchema.AspNetCore.Tests.Infrastructure;

namespace OneSchema.AspNetCore.Tests;

[ClassDataSource<AlbaFixture>(Shared = SharedType.PerTestSession)]
public class ContactValidationTests(AlbaFixture albaBootstrap) : AlbaTestBase(albaBootstrap)
{
    [Test]
    public async Task Valid_contact_returns_empty_array()
    {
        var body = new ValidationHookRequestBuilder()
            .WithJwt(TestConstants.GenerateValidJwt())
            .AddRow(1, new()
            {
                ["first_name"] = "Jane",
                ["last_name"] = "Doe",
                ["email"] = "jane@example.com",
                ["phone"] = "555-1234"
            })
            .BuildJson();

        var result = await Host.Scenario(s =>
        {
            s.Post.RawJson(body).ToUrl("/webhooks/validate-contacts");
            s.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var items = await result.ReadAsJsonAsync<ValidationHookResponseItem[]>();

        await Assert.That(items).IsNotNull();
        await Assert.That(items).IsEmpty();
    }

    [Test]
    public async Task Missing_email_returns_error()
    {
        var body = new ValidationHookRequestBuilder()
            .WithJwt(TestConstants.GenerateValidJwt())
            .AddRow(1, new()
            {
                ["first_name"] = "Jane",
                ["last_name"] = "Doe",
                ["email"] = "",
                ["phone"] = "555-1234"
            })
            .BuildJson();

        var result = await Host.Scenario(s =>
        {
            s.Post.RawJson(body).ToUrl("/webhooks/validate-contacts");
            s.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var items = await result.ReadAsJsonAsync<ValidationHookResponseItem[]>();

        await Assert.That(items).Count().IsEqualTo(1);
        await Assert.That(items![0].RowId).IsEqualTo(1);
        await Assert.That(items[0].Column).IsEqualTo("email");
        await Assert.That(items[0].Severity).IsEqualTo("error");
    }

    [Test]
    public async Task Invalid_email_returns_warning_with_suggestion()
    {
        var body = new ValidationHookRequestBuilder()
            .WithJwt(TestConstants.GenerateValidJwt())
            .AddRow(1, new()
            {
                ["first_name"] = "Jane",
                ["last_name"] = "Doe",
                ["email"] = "not-an-email",
                ["phone"] = "555-1234"
            })
            .BuildJson();

        var result = await Host.Scenario(s =>
        {
            s.Post.RawJson(body).ToUrl("/webhooks/validate-contacts");
            s.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var items = await result.ReadAsJsonAsync<ValidationHookResponseItem[]>();

        await Assert.That(items).Count().IsEqualTo(1);
        await Assert.That(items![0].Severity).IsEqualTo("warning");
        await Assert.That(items[0].Suggestion).IsNotNull();
    }

    [Test]
    public async Task Multiple_missing_fields_returns_multiple_errors()
    {
        var body = new ValidationHookRequestBuilder()
            .WithJwt(TestConstants.GenerateValidJwt())
            .AddRow(1, new()
            {
                ["first_name"] = "",
                ["last_name"] = "",
                ["email"] = "",
                ["phone"] = ""
            })
            .BuildJson();

        var result = await Host.Scenario(s =>
        {
            s.Post.RawJson(body).ToUrl("/webhooks/validate-contacts");
            s.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var items = await result.ReadAsJsonAsync<ValidationHookResponseItem[]>();

        // email + first_name + last_name = 3 errors
        await Assert.That(items).Count().IsEqualTo(3);
        await Assert.That(items!.All(i => i.Severity == "error")).IsTrue();
    }

    [Test]
    public async Task Multiple_rows_each_validated_independently()
    {
        var body = new ValidationHookRequestBuilder()
            .WithJwt(TestConstants.GenerateValidJwt())
            .AddRow(1, new()
            {
                ["first_name"] = "Jane",
                ["last_name"] = "Doe",
                ["email"] = "jane@example.com"
            })
            .AddRow(2, new()
            {
                ["first_name"] = "",
                ["last_name"] = "Smith",
                ["email"] = "bob@example.com"
            })
            .BuildJson();

        var result = await Host.Scenario(s =>
        {
            s.Post.RawJson(body).ToUrl("/webhooks/validate-contacts");
            s.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var items = await result.ReadAsJsonAsync<ValidationHookResponseItem[]>();

        // Row 1 is valid, Row 2 has missing first_name
        await Assert.That(items).Count().IsEqualTo(1);
        await Assert.That(items![0].RowId).IsEqualTo(2);
        await Assert.That(items[0].Column).IsEqualTo("first_name");
    }
}
