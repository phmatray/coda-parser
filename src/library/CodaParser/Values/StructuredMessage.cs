namespace CodaParser.Values;

public class StructuredMessage
{
    public StructuredMessage(string value, TransactionCode transactionCode)
    {
        Helpers.ValidateStringMultipleLengths(value, [53, 73], "StructuredMessage");

        StructuredMessageType = value[..3];
        StructuredMessageFull = value[3..];

        if (StructuredMessageType is "101" or "102")
        {
            Value = StructuredMessageFull[..12];
        }
        else if (StructuredMessageType == "105" && StructuredMessageFull.Length >= 57)
        {
            Value = StructuredMessageFull.Substring(45, 12); // is start position 42 or 45?
        }
        else if (StructuredMessageType == "127" && transactionCode.Family.Value == "05")
        {
            SepaDirectDebit = new SepaDirectDebit(StructuredMessageFull);
        }
    }

    public SepaDirectDebit? SepaDirectDebit { get; }

    public string StructuredMessageFull { get; }

    public string StructuredMessageType { get; }

    public string Value { get; } = "";
}