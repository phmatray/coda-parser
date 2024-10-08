using CodaParser.Lines;
using CodaParser.Statements;

namespace CodaParser.StatementParsers;

/// <summary>
/// Parser of a single statement.
/// </summary>
public class StatementParser
{
    /// <summary>
    /// Parse the lines into a <see cref="Statement"/>.
    /// </summary>
    /// <param name="lines">The lines to parse.</param>
    /// <returns>A single <see cref="Statement"/>.</returns>
    public Statement Parse(IEnumerable<ILine> lines)
    {
        var linesList = lines.ToList();

        var date = new DateTime(1, 1, 1);
        var identificationLine = Helpers.GetFirstLineOfType<IdentificationLine>(linesList);
        if (identificationLine != null)
        {
            date = identificationLine.CreationDate.Value;
        }

        var initialBalance = 0.0m;
        var initialStateLine = Helpers.GetFirstLineOfType<InitialStateLine>(linesList);
        if (initialStateLine != null)
        {
            initialBalance = initialStateLine.Balance.Value;
        }

        var newBalance = 0.0m;
        var newStateLine = Helpers.GetFirstLineOfType<NewStateLine>(linesList);
        if (newStateLine != null)
        {
            newBalance = newStateLine.Balance.Value;
        }

        var messageParser = new MessageParser();
        var informationalMessage = messageParser.Parse(linesList.OfType<MessageLine>());

        var accountParser = new AccountParser();
        var account = accountParser.Parse(
            Helpers.FilterLinesOfTypes(
                linesList,
                [
                    LineType.Identification,
                    LineType.InitialState
                ]
            )
        );

        var transactionLines = GroupTransactions(linesList.OfType<IInformationOrTransactionLine>());

        var transactionParser = new TransactionParser();
        var transactions = transactionLines.Select(l => transactionParser.Parse(l));

        return new Statement(
            date,
            account,
            initialBalance,
            newBalance,
            informationalMessage,
            transactions
        );
    }

    private IEnumerable<IEnumerable<IInformationOrTransactionLine>> GroupTransactions(IEnumerable<IInformationOrTransactionLine> lines)
    {
        var transactions = new Dictionary<int, List<IInformationOrTransactionLine>>();
        var idx = -1;
        var sequenceNumber = -1;

        foreach (var transactionOrInformationLine in lines)
        {
            if (transactions.Count == 0 || sequenceNumber != transactionOrInformationLine.SequenceNumber.Value)
            {
                sequenceNumber = transactionOrInformationLine.SequenceNumber.Value;
                idx += 1;

                transactions[idx] = [];
            }

            transactions[idx].Add(transactionOrInformationLine);
        }

        return transactions.Values;
    }
}