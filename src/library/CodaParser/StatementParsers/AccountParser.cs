using CodaParser.Lines;

namespace CodaParser.StatementParsers;

/// <summary>
/// Parser for the account.
/// </summary>
public class AccountParser
{
    /// <summary>
    /// Parse the relevant lines in an <see cref="CodaParser.Statements.Account"/>.
    /// </summary>
    /// <param name="lines">The lines to parse.</param>
    /// <returns>The account of the customer.</returns>
    public Statements.Account Parse(IEnumerable<ILine> lines)
    {
        var linesList = lines.ToList();
        
        var identificationLine = Helpers.GetFirstLineOfType<IdentificationLine>(linesList);
        var initialStateLine = Helpers.GetFirstLineOfType<InitialStateLine>(linesList);

        return new Statements.Account(
            identificationLine?.AccountName.Value ?? "",
            identificationLine?.AccountBic.Value ?? "",
            identificationLine?.AccountCompanyIdentificationNumber.Value ?? "",
            initialStateLine?.Account.Number.Value ?? "",
            initialStateLine?.Account.Currency.CurrencyCode ?? "",
            initialStateLine?.Account.Country.CountryCode ?? "");
    }
}