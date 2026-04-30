using OneSchema.AspNetCore.Api;
using OneSchema.AspNetCore.Validation;
using OneSchema.AspNetCore.Validation.Handlers;

namespace OneSchemaSandbox.Handlers;

/// <summary>
/// Demonstrates <see cref="RowIndex{TRow,TKey}"/>: indexes all contact rows by email in one
/// pass, issues a single batch query, then calls <c>ErrorForMatches</c> to flag every row
/// whose email was already found.
/// </summary>
public class ContactEmailExistsHandler : BatchValidationHookHandler<ContactRow>
{
    // Simulates the set of emails that already exist in the database.
    private static readonly HashSet<string> _existingEmails = new(StringComparer.OrdinalIgnoreCase)
    {
        "existing@example.com",
        "taken@acme.com",
    };

    protected override Task ValidateAsync(
        ValidationHookRequest<ContactRow> request,
        ValidationResultBuilder results,
        CancellationToken cancellationToken)
    {
        // Build an index keyed by email, ignoring rows with no email.
        var byEmail = request.Rows
            .Where(r => !string.IsNullOrWhiteSpace(r.Values.Email))
            .IndexBy(v => v.Email!, StringComparer.OrdinalIgnoreCase);

        // In a real handler this would be a DB query scoped to byEmail.Keys:
        //   var taken = await db.Users
        //       .Where(u => byEmail.Keys.Contains(u.Email))
        //       .Select(u => u.Email)
        //       .ToListAsync(cancellationToken);
        var taken = _existingEmails.Intersect(byEmail.Keys, StringComparer.OrdinalIgnoreCase);

        byEmail.ErrorForMatches(
            taken,
            results,
            column: "email",
            messageFactory: email => $"'{email}' is already registered.");

        return Task.CompletedTask;
    }
}
