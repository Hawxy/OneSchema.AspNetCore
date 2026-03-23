using OneSchema.AspNetCore.Validation;
using OneSchema.AspNetCore.Validation.Handlers;

namespace OneSchemaSandbox.Handlers;

/// <summary>
/// Validates contacts using the strongly-typed <see cref="ContactRow"/> model.
/// </summary>
public class ContactValidationHandler : RowValidationHookHandler<ContactRow>
{
    protected override ValueTask ValidateRowAsync(
        RowValidationContext<ContactRow> context,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(context.Values.Email))
        {
            context.Error("email", "Email is required.");
        }
        else if (!context.Values.Email.Contains('@'))
        {
            context.Warning("email", "Email appears invalid.")
                .WithSuggestion("user@example.com");
        }

        if (string.IsNullOrWhiteSpace(context.Values.FirstName))
        {
            context.Error("first_name", "First name is required.");
        }

        if (string.IsNullOrWhiteSpace(context.Values.LastName))
        {
            context.Error("last_name", "Last name is required.");
        }

        return ValueTask.CompletedTask;
    }
}

