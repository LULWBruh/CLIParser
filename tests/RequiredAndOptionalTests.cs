namespace CliParserTests;

using CliParser;

public class RequiredAndOptionalTests
{
    private static Option CreateUrlOption()
    {
        return Option
            .Builder("url")
            .AddShortAlias('u')
            .AddLongAlias("url")
            .AddParameter(Parameter.StringParameter())
            .Build();
    }
    private static Option CreateUrlRequiredOption()
    {
        return Option
            .Builder("url")
            .Required()
            .AddShortAlias('u')
            .AddLongAlias("url")
            .AddParameter(Parameter.StringParameter())
            .Build();
    }

    private static Option CreateRecursiveOption()
    {
        return Option
            .Builder("recursive")
            .AddShortAlias('r')
            .AddLongAlias("recursive")
            .AddParameter(Parameter.BoolParameter())
            .Build();
    }

    [Fact]
    public void Parse_ThrowsRequiredOptionMissing_WhenRequiredAbsent()
    {
        Parser parser = new();
        parser.AddOption(CreateUrlRequiredOption());

        Assert.Throws<RequiredOptionMissingException>(() => parser.Parse([]));
    }

    public static TheoryData<string[]> HelpArgs()
    {
        return TestSetup.HelpArgs();
    }

    public static TheoryData<string[]> VersionArgs()
    {
        return TestSetup.VersionArgs();
    }


    [Theory]
    [MemberData(nameof(HelpArgs))]
    [MemberData(nameof(VersionArgs))]
    public void Parse_DoesNotThrowsRequiredOptionMissing_WhenHelpOrVersionPresent(string[] args)

    {
        Parser parser = new();
        parser.AddOption(CreateUrlRequiredOption());

        _ = parser.Parse(args);
    }

    [Fact]
    public void Parse_LeavesOptionalOptionUnset_WhenMissing()
    {
        Parser parser = new();
        parser.AddOption(CreateRecursiveOption());

        var data = Assert.IsType<ParsedData>(parser.Parse([]));

        Assert.False(data.GetOption("recursive").IsSet);
    }

    public static TheoryData<string[]> RequiredPresentArgs()
    {
        return new TheoryData<string[]>
            { new[] { "--url", "https://example.com" }, new[] { "-u", "https://example.com" } };
    }

    [Theory]
    [MemberData(nameof(RequiredPresentArgs))]
    public void Parse_AcceptsRequiredOption_WhenPresent(string[] args)
    {
        Parser parser = new();
        parser.AddOption(CreateUrlRequiredOption());

        var data = Assert.IsType<ParsedData>(parser.Parse(args));

        Assert.True(data.GetOption("url").IsSet);
    }
}