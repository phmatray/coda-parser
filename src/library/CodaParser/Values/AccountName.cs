namespace CodaParser.Values;

public class AccountName
{
    public AccountName(string value)
    {
        Helpers.ValidateStringMultipleLengths(value, [26, 35], "AccountName");

        Value = value.Trim();
    }

    public string Value { get; }
}