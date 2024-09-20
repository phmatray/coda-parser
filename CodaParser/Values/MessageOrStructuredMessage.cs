namespace CodaParser.Values;

public class MessageOrStructuredMessage
{
    public MessageOrStructuredMessage(string value, TransactionCode transactionCode)
    {
        Helpers.ValidateStringMultipleLengths(value, new[] { 54, 74 }, "MessageOrStructuredMessage");

        var hasStructuredMessage = value[..1] == "1" ? true : false;
        StructuredMessage = null;
        Message = null;

        if (hasStructuredMessage)
        {
            StructuredMessage = new StructuredMessage(value[1..], transactionCode);
        }
        else
        {
            Message = new Message(value[1..]);
        }
    }

    public Message? Message { get; }

    public StructuredMessage? StructuredMessage { get; }
}