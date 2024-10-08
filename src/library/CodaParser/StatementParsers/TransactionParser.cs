using CodaParser.Lines;
using CodaParser.Statements;
using CodaParser.Values;

namespace CodaParser.StatementParsers;

/// <summary>
/// Parser of a transaction.
/// </summary>
public class TransactionParser
{
    /// <summary>
    /// Parse the relevant lines into a <see cref="Transaction"/>.
    /// </summary>
    /// <param name="lines">The lines to parse.</param>
    /// <returns>A single <see cref="Transaction"/>.</returns>
    public Transaction Parse(IEnumerable<ILine> lines)
    {
        var linesList = lines.ToList();
        
        var transactionPart1Line = Helpers.GetFirstLineOfType<TransactionPart1Line>(linesList);

        var transactionDate = new DateTime(1, 1, 1);
        var valutaDate = new DateTime(1, 1, 1);
        var amount = 0.0m;
        SepaDirectDebit? sepaDirectDebit = null;

        var statementSequence = 0;
        var transactionSequence = 0;
        if (transactionPart1Line != null)
        {
            valutaDate = transactionPart1Line.ValutaDate.Value;
            transactionDate = transactionPart1Line.TransactionDate.Value;
            amount = transactionPart1Line.Amount.Value;
            statementSequence = transactionPart1Line.StatementSequenceNumber.Value;
            transactionSequence = transactionPart1Line.SequenceNumber.Value;
            if (transactionPart1Line.MessageOrStructuredMessage.StructuredMessage != null)
            {
                sepaDirectDebit = transactionPart1Line.MessageOrStructuredMessage.StructuredMessage.SepaDirectDebit;
            }
        }

        var informationPart1Line = Helpers.GetFirstLineOfType<InformationPart1Line>(linesList);

        var structuredMessage = "";
        if (!string.IsNullOrEmpty(transactionPart1Line?.MessageOrStructuredMessage.StructuredMessage?.Value))
        {
            structuredMessage = transactionPart1Line?.MessageOrStructuredMessage.StructuredMessage?.Value;
        }
        else if (!string.IsNullOrEmpty(informationPart1Line?.MessageOrStructuredMessage.StructuredMessage?.Value))
        {
            structuredMessage = informationPart1Line?.MessageOrStructuredMessage.StructuredMessage?.Value;
        }

        var linesWithAccountInfo = Helpers.FilterLinesOfTypes(
            linesList,
            [
                LineType.TransactionPart2,
                LineType.TransactionPart3
            ]);

        var accountOtherPartyParser = new AccountOtherPartyParser();
        var account = accountOtherPartyParser.Parse(linesWithAccountInfo);

        var message = ConstructMessage(linesList);

        return new Transaction(
            account,
            statementSequence,
            transactionSequence,
            transactionDate,
            valutaDate,
            amount,
            message,
            structuredMessage,
            sepaDirectDebit);
    }

    private string ConstructMessage(IEnumerable<ILine> lines)
    {
        var linesList = lines.ToList();
        
        var transactionLines = Helpers.FilterLinesOfTypes(
            linesList,
            [
                LineType.TransactionPart1,
                LineType.TransactionPart2,
                LineType.TransactionPart3
            ]);

        var message = string.Join("", transactionLines.Select(line =>
        {
            Message? m = line switch
            {
                TransactionPart1Line l => l.MessageOrStructuredMessage.Message,
                TransactionPart2Line l => l.Message,
                TransactionPart3Line l => l.Message,
                _ => throw new InvalidOperationException("Don't know how to get the message of an object of type " + line.GetType().Name)
            };

            return m?.Value ?? "";
        }));

        if (string.IsNullOrEmpty(message))
        {
            var transactionLine = Helpers.GetFirstLineOfType<TransactionPart2Line>(linesList);
            if (transactionLine != null && !string.IsNullOrEmpty(transactionLine.ClientReference.Value))
            {
                message = transactionLine.ClientReference.Value;
            }

            var informationLines = Helpers.FilterLinesOfTypes(
                linesList,
                [
                    LineType.InformationPart1,
                    LineType.InformationPart2,
                    LineType.InformationPart3
                ]);

            if (!string.IsNullOrEmpty(message))
            {
                message += " ";
            }

            message += string.Join("", informationLines.Select(line =>
            {
                Message? m = line switch
                {
                    InformationPart1Line i => i.MessageOrStructuredMessage.Message,
                    InformationPart2Line i => i.Message,
                    InformationPart3Line i => i.Message,
                    _ => throw new InvalidOperationException("Don't know how to get the message of an object of type " + line.GetType().Name)
                };

                return m?.Value ?? "";
            }));
        }

        return message.Trim();
    }
}