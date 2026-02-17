using Microsoft.Extensions.Localization;
using Minimal.Domain;

namespace Minimal.Api.Common.Accounting.Validators;

/// <summary>
/// Validator for accounting documents to ensure data integrity
/// </summary>
public class DocumentValidator
{
    private readonly IStringLocalizer<SharedResource> _localizer;

    public DocumentValidator(IStringLocalizer<SharedResource> localizer)
    {
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }
    /// <summary>
    /// Validate that the total debit and credit amounts in the document are equal (accounting equation holds).
    /// </summary>
    public (bool IsValid, string ErrorMessage) ValidateBalance(Document document)
    {
        if (document?.DocumentItems == null || !document.DocumentItems.Any())
        {
            return (false, _localizer["documentMustHaveAtLeastOneItem"]);
        }

        var debit = document.DocumentItems.Sum(x => x.Debit);
        var credit = document.DocumentItems.Sum(x => x.Credit);

        if (debit != credit)
        {
            return (false, string.Format(_localizer["accountingEquationViolated"], debit, credit));
        }

        return (true, string.Empty);
    }

    /// <summary>
    /// Validate that the debit and credit amounts in the document items are non-negative and that at least one of them has a value greater than zero.
    /// </summary>
    public (bool IsValid, string ErrorMessage) ValidateAmounts(Document document)
    {
        if (document?.DocumentItems == null)
        {
            return (false, _localizer["invalidDocument"]);
        }

        if (document.DocumentItems.Any(x => x.Debit < 0 || x.Credit < 0))
        {
            return (false, _localizer["debitCreditCannotBeNegative"]);
        }

        if (document.DocumentItems.All(x => x.Debit == 0 && x.Credit == 0))
        {
            return (false, _localizer["atLeastOneItemMustHaveValue"]);
        }

        return (true, string.Empty);
    }

    /// <summary>
    /// Validate the document by checking both the balance and the amounts. Returns a tuple indicating whether the document is valid and an error message if it's not.
    /// </summary>
    public (bool IsValid, string ErrorMessage) ValidateDocument(Document document)
    {
        var amountValidation = ValidateAmounts(document);
        if (!amountValidation.IsValid)
            return amountValidation;

        var balanceValidation = ValidateBalance(document);
        if (!balanceValidation.IsValid)
            return balanceValidation;

        return (true, string.Empty);
    }
}
