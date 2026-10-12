namespace CliParserTests;

using CliParser;

public class SpecialResultsTests
{
    public static TheoryData<string[]> HelpArgs()
    {
        return TestSetup.HelpArgs();
    }

    [Theory]
    [MemberData(nameof(HelpArgs))]
    public void Parse_ReturnsHelpResult_ForHelpAliases(params string[] args)
    {
        var parser = TestSetup.CreateParserWithOptionalUrl();

        var result = parser.Parse(args);

        Assert.IsType<ParserInfo>(result);
    }

    public static TheoryData<string[]> VersionArgs()
    {
        return TestSetup.VersionArgs();
    }

    [Theory]
    [MemberData(nameof(VersionArgs))]
    public void Parse_ReturnsInfoResult_ForVersionAliases(params string[] args)
    {
        var parser = TestSetup.CreateParserWithOptionalUrl();

        var result = parser.Parse(args);

        Assert.IsType<ParserInfo>(result);
    }

    public static TheoryData<string[]> RegularInputArgs()
    {
        return new TheoryData<string[]>
        {
            new[] { "--url", "https://example.com" },
            new[] { "--uri", "https://example.com" },
            new[] { "-u", "https://example.com" },
            new[] { "-U", "https://example.com" }
        };
    }

    [Theory]
    [MemberData(nameof(RegularInputArgs))]
    public void Parse_ReturnsParseData_ForRegularInput(string[] args)
    {
        var parser = TestSetup.CreateParserWithOptionalUrl();

        var result = parser.Parse(args);

        Assert.IsType<ParsedData>(result);
    }

    [Fact]
    public void Parse_ShowHelpOnEmptyArguments_WhenEnabled()
    {
        var parser = TestSetup.CreateParserWithOptionalUrl();
        parser.ShowHelpOnEmptyArguments = true;

        var result = parser.Parse([]);

        Assert.IsType<ParserInfo>(result);
    }

    [Fact]
    public void Parse_DoesNotShowHelpOnEmptyArguments_WhenDisabled()
    {
        var parser = TestSetup.CreateParserWithOptionalUrl();
        parser.ShowHelpOnEmptyArguments = false;

        var result = parser.Parse([]);

        Assert.IsType<ParsedData>(result);
    }
}