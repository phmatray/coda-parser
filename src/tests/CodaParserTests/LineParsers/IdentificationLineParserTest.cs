using System;
using CodaParser.LineParsers;
using CodaParser.Lines;
using NUnit.Framework;

namespace CodaParserTests.LineParsers;

[TestFixture]
public class IdentificationLineParserTest
{
    [Test]
    public void TestSample1()
    {
        var parser = new IdentificationLineParser();

        const string sample = "0000018011520105        0938409934CODELICIOUS               GEBABEBB   09029308273 00001          984309          834080       2";

        Assert.That(parser.CanAcceptString(sample), Is.True);

        var result = (IdentificationLine)parser.Parse(sample);

        Assert.That(result.CreationDate.Value, Is.EqualTo(new DateTime(2015, 01, 18)));
        Assert.That(result.BankIdentificationNumber.Value, Is.EqualTo("201"));
        Assert.That(result.ApplicationCode.Value, Is.EqualTo("05"));
        Assert.That(result.IsDuplicate, Is.False);
        Assert.That(result.FileReference.Value, Is.EqualTo("0938409934"));
        Assert.That(result.AccountName.Value, Is.EqualTo("CODELICIOUS"));
        Assert.That(result.AccountBic.Value, Is.EqualTo("GEBABEBB"));
        Assert.That(result.AccountCompanyIdentificationNumber.Value, Is.EqualTo("09029308273"));
        Assert.That(result.ExternalApplicationCode.Value, Is.EqualTo("00001"));
        Assert.That(result.TransactionReference.Value, Is.EqualTo("984309"));
        Assert.That(result.RelatedReference.Value, Is.EqualTo("834080"));
        Assert.That(result.VersionCode.Value, Is.EqualTo("2"));
    }
}