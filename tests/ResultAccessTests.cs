namespace CliParserTests;

using CliParser;

public class ResultAccessTests
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
            .AddShortAlias('u')
            .AddLongAlias("url")
            .AddParameter(Parameter.StringParameter())
            .Build();
    }

    public static TheoryData<string[], string> UrlArgs()
    {
        return new TheoryData<string[], string>
        {
            { new[] { "--url", "https://example.com" }, "https://example.com" },
            { new[] { "-u", "https://example.com" }, "https://example.com" }
        };
    }

    [Theory]
    [MemberData(nameof(UrlArgs))]
    public void ParseData_GetOptionByName_ReturnsResult(string[] args, string expectedUrl)
    {
        Parser parser = new();
        parser.AddOption(CreateUrlRequiredOption());

        var data = Assert.IsType<ParsedData>(parser.Parse(args));

        var option = data.GetOption("url");
        Assert.True(option.IsSet);
        Assert.Equal(expectedUrl, option.GetParameter<string>(0));
    }

    [Theory]
    [MemberData(nameof(UrlArgs))]
    public void ParseData_GetOptionByObject_ReturnsResult(string[] args, string expectedUrl)
    {
        Parser parser = new();
        var urlOption = CreateUrlRequiredOption();
        parser.AddOption(urlOption);

        var data = Assert.IsType<ParsedData>(parser.Parse(args));

        var option = data.GetOption(urlOption);
        Assert.True(option.IsSet);
        Assert.Equal(expectedUrl, option.GetParameter<string>(0));
    }

    [Fact]
    public void OptionResult_TryGetParameter_ReturnsFalseForWrongType()
    {
        Parser parser = new();
        parser.AddOption(
            Option
                .Builder("depth")
                .AddLongAlias("depth")
                .AddParameter(Parameter.IntParameter())
                .Build()
        );

        var data = Assert.IsType<ParsedData>(parser.Parse(["--depth", "7"]));
        var option = data.GetOption("depth");

        Assert.False(option.TryGetParameter(0, out string _));
    }

    [Fact]
    public void ParseData_GetOption_ThrowsForUnknownOptionName()
    {
        Parser parser = new();
        parser.AddOption(CreateUrlOption());

        var data = Assert.IsType<ParsedData>(parser.Parse([]));

        Assert.Throws<OptionNotFoundException>(() => data.GetOption("nonexistent"));
    }
}