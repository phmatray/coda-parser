using CodaParser.Lines;
using CodaParser.Statements;

namespace CodaParser.StatementParsers;

/// <summary>
/// Parser for the account of the other party.
/// </summary>
public class AccountOtherPartyParser
{
    /// <summary>
    /// Parse the relevant lines to a <see cref="AccountOtherParty"/>.
    /// </summary>
    /// <param name="lines">The lines with the information.</param>
    /// <returns>The account of the other party.</returns>
    public AccountOtherParty Parse(IEnumerable<ILine> lines)
    {
        var linesList = lines.ToList();
        
        var transactionPart2Line = Helpers.GetFirstLineOfType<TransactionPart2Line>(linesList);
        var transactionPart3Line = Helpers.GetFirstLineOfType<TransactionPart3Line>(linesList);

        var bic = "";
        if (transactionPart2Line != null)
        {
            bic = transactionPart2Line.OtherAccountBic.Value;
        }

        var number = "";
        var name = "";
        var currency = "";
        if (transactionPart3Line != null)
        {
            name = transactionPart3Line.OtherAccountName.Value;
            number = transactionPart3Line.OtherAccountNumberAndCurrency.Value;

            // let's try to parse number and currency
            if (!string.IsNullOrEmpty(number))
            {
                var lastSpace = number.LastIndexOf(' ');
                if (lastSpace != -1)
                {
                    currency = number[lastSpace..].Trim();
                    number = number[..lastSpace].Trim();
                }
            }
        }

        return new AccountOtherParty(
            name,
            bic,
            number,
            currency
        );
    }
}