using System.Net;
using OneSchema.AspNetCore.Api;
using OneSchema.AspNetCore.Tests.Infrastructure;

namespace OneSchema.AspNetCore.Tests;

[ClassDataSource<AlbaFixture>(Shared = SharedType.PerTestSession)]
public class ProductUniquenessTests(AlbaFixture albaBootstrap) : AlbaTestBase(albaBootstrap)
{
    [Test]
    public async Task Unique_skus_return_no_errors()
    {
        var body = new ValidationHookRequestBuilder()
            .AddRow(1, new() { ["sku"] = "ABC-001", ["name"] = "Widget", ["price"] = "9.99" })
            .AddRow(2, new() { ["sku"] = "ABC-002", ["name"] = "Gadget", ["price"] = "19.99" })
            .BuildJson();

        var result = await Host.Scenario(s =>
        {
            s.Post.RawJson(body).ToUrl("/webhooks/validate-products");
            s.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var items = await result.ReadAsJsonAsync<ValidationHookResponseItem[]>();

        await Assert.That(items).IsEmpty();
    }

    [Test]
    public async Task Duplicate_sku_returns_error_on_second_row()
    {
        var body = new ValidationHookRequestBuilder()
            .AddRow(1, new() { ["sku"] = "ABC-001", ["name"] = "Widget", ["price"] = "9.99" })
            .AddRow(2, new() { ["sku"] = "ABC-001", ["name"] = "Duplicate", ["price"] = "5.00" })
            .BuildJson();

        var result = await Host.Scenario(s =>
        {
            s.Post.RawJson(body).ToUrl("/webhooks/validate-products");
            s.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var items = await result.ReadAsJsonAsync<ValidationHookResponseItem[]>();

        await Assert.That(items).Count().IsEqualTo(1);
        await Assert.That(items![0].RowId).IsEqualTo(2);
        await Assert.That(items[0].Column).IsEqualTo("sku");
        await Assert.That(items[0].Severity).IsEqualTo("error");
        await Assert.That(items[0].Message).Contains("Duplicate");
    }

    [Test]
    public async Task Missing_sku_returns_error()
    {
        var body = new ValidationHookRequestBuilder()
            .AddRow(1, new() { ["sku"] = "", ["name"] = "No SKU", ["price"] = "1.00" })
            .BuildJson();

        var result = await Host.Scenario(s =>
        {
            s.Post.RawJson(body).ToUrl("/webhooks/validate-products");
            s.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var items = await result.ReadAsJsonAsync<ValidationHookResponseItem[]>();

        await Assert.That(items).Count().IsEqualTo(1);
        await Assert.That(items![0].Column).IsEqualTo("sku");
        await Assert.That(items[0].Message).Contains("required");
    }

    [Test]
    public async Task Duplicate_sku_is_case_insensitive()
    {
        var body = new ValidationHookRequestBuilder()
            .AddRow(1, new() { ["sku"] = "abc-001", ["name"] = "Lower", ["price"] = "1.00" })
            .AddRow(2, new() { ["sku"] = "ABC-001", ["name"] = "Upper", ["price"] = "2.00" })
            .BuildJson();

        var result = await Host.Scenario(s =>
        {
            s.Post.RawJson(body).ToUrl("/webhooks/validate-products");
            s.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var items = await result.ReadAsJsonAsync<ValidationHookResponseItem[]>();

        await Assert.That(items).Count().IsEqualTo(1);
        await Assert.That(items![0].RowId).IsEqualTo(2);
    }
    
}
