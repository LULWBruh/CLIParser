namespace CliParserTests;

using CliParser;

public class AliasResolutionTests
{
    [Theory]
    [InlineData("-u", "https://example.com")]
    [InlineData("-U", "https://example.com")]
    public void Parse_RecognizesShortAlias(string arg1, string arg2)
    {
        Parser parser = new();
        parser.AddOption(TestSetup.CreateUrlOptionWithMultipleAliases());

        var data = Assert.IsType<ParsedData>(parser.Parse([arg1, arg2]));

        Assert.True(data.GetOption("url-option").IsSet);
    }

    [Theory]
    [InlineData("--url", "https://example.com")]
    [InlineData("--uri", "https://example.com")]
    public void Parse_RecognizesLongAlias(string arg1, string arg2)
    {
        Parser parser = new();
        parser.AddOption(TestSetup.CreateUrlOptionWithMultipleAliases());

        var data = Assert.IsType<ParsedData>(parser.Parse([arg1, arg2]));

        Assert.True(data.GetOption("url-option").IsSet);
    }

    [Theory]
    [InlineData("opt", 'a', "opt", 'b')]
    [InlineData("opt", 'a', "dif", 'a')]
    [InlineData("opt", 'a', "opt", 'a')]
    public void AddOption_ThrowsOnAliasConflict(
        string longAliasA,
        char shortAliasA,
        string longAliasB,
        char shortAliasB
    )
    {
        Parser parser = new();
        parser.AddOption(
            Option.Builder("first").AddLongAlias(longAliasA).AddShortAlias(shortAliasA).Build()
        );

        Assert.Throws<ConflictingOptionAliasesException>(() =>
            parser.AddOption(
                Option.Builder("second").AddLongAlias(longAliasB).AddShortAlias(shortAliasB).Build()
            )
        );
    }

    [Fact]
    public void AddOption_ThrowsOnOptionNameConflict()
    {
        Parser parser = new();
        parser.AddOption(Option.Builder("option").AddLongAlias("first").Build());

        Assert.Throws<NotUniqueOptionNameException>(() =>
            parser.AddOption(Option.Builder("option").AddLongAlias("second").Build())
        );
    }

    [Theory]
    [InlineData("help")]
    [InlineData("version")]
    public void BuildOption_ThrowsBadAlias_ForReservedLongAliases(string reservedAlias)
    {
        Assert.Throws<BadAliasException>(() => Option.Builder("bad").AddLongAlias(reservedAlias));
    }

    [Theory]
    [InlineData("")]
    [InlineData("-")]
    [InlineData("--")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("opti\non")]
    [InlineData("opti\ton")]
    public void BuildOption_ThrowsBadAlias_InvalidLongName(string alias)
    {
        Assert.Throws<BadAliasException>(() => Option.Builder("bad").AddLongAlias(alias));
    }

    [Theory]
    [InlineData('H')]
    [InlineData('V')]
    public void BuildOption_ThrowsBadAlias_ForReservedShortAlias(char alias)
    {
        Assert.Throws<BadAliasException>(() => Option.Builder("bad").AddShortAlias(alias));
    }

    public static TheoryData<char> AsciiControlChars = ['\0', '\b', '\n', '\r', '\t'];
    public static TheoryData<char> WhitespaceChars = [' ', '\u00A0'];
    public static TheoryData<char> DigitChars = ['0', '5', '9'];
    public static TheoryData<char> SymbolChars = ['-', '=', '+', '_', '!', '&', '*'];
    public static TheoryData<char> NonAsciiChars = ['á', 'ß', 'ø'];

    [Theory]
    [MemberData(nameof(AsciiControlChars))]
    [MemberData(nameof(WhitespaceChars))]
    [MemberData(nameof(DigitChars))]
    [MemberData(nameof(SymbolChars))]
    [MemberData(nameof(NonAsciiChars))]
    public void BuildOption_ThrowsBadAlias_InvalidChar(char alias)
    {
        Assert.Throws<BadAliasException>(() => Option.Builder("bad").AddShortAlias(alias));
        Assert.Throws<BadAliasException>(() =>
            Option.Builder("bad").AddLongAlias(alias.ToString())
        );
    }
}