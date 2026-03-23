using OneSchema.AspNetCore.Api;
using OneSchema.AspNetCore.Validation;
using OneSchema.AspNetCore.Validation.Handlers;

namespace OneSchemaSandbox.Handlers;

/// <summary>
/// Validates product rows in batch — checks for duplicate SKUs across all rows.
/// </summary>
public class ProductUniquenessHandler : BatchValidationHookHandler<ProductRow>
{
    protected override Task ValidateAsync(
        ValidationHookRequest<ProductRow> request,
        ValidationResultBuilder results,
        CancellationToken cancellationToken)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in request.Rows)
        {
            if (string.IsNullOrWhiteSpace(row.Values.Sku))
            {
                results.ForRow(row).Error("sku", "SKU is required.");
                continue;
            }

            if (!seen.Add(row.Values.Sku))
            {
                results.ForRow(row).Error("sku", "Duplicate SKU.");
            }
        }

        return Task.CompletedTask;
    }
}

