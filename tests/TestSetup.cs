namespace CliParserTests;

using CliParser;

public static class TestSetup
{
    public static Option CreateUrlOptionWithMultipleAliases()
    {
        return Option
            .Builder("url-option")
            .AddShortAlias('u')
            .AddShortAlias('U')
            .AddLongAlias("url")
            .AddLongAlias("uri")
            .AddParameter(Parameter.StringParameter())
            .Build();
    }
    public static Option CreateUrlRequiredOptionWithMultipleAliases()
    {
        return Option
            .Builder("url-option")
            .AddShortAlias('u')
            .AddShortAlias('U')
            .AddLongAlias("url")
            .AddLongAlias("uri")
            .AddParameter(Parameter.StringParameter())
            .Build();
    }

    public static Parser CreateParserWithOptionalUrl()
    {
        Parser parser = new();
        parser.AddOption(CreateUrlOptionWithMultipleAliases());
        return parser;
    }

    public static Parser CreateParserWithRequiredUrl()
    {
        Parser parser = new();
        parser.AddOption(CreateUrlRequiredOptionWithMultipleAliases());
        return parser;
    }

    // Removed invalid inputs
    public static TheoryData<string[]> HelpArgs()
    {
        return new TheoryData<string[]>
        {
            new[] { "--help" },
            new[] { "-H" }
        };
    }

    // Removed invalid inputs
    public static TheoryData<string[]> VersionArgs()
    {
        return new TheoryData<string[]>
        {
            new[] { "--version" },
            new[] { "-V" }
        };
    }
}