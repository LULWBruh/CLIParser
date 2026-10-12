namespace CliParserTests;

using CliParser;

public class DependencyTests
{
    private static Parser CreateParserWithConflictingOutputOptions()
    {
        Parser parser = new();
        var json = Option.Builder("json").AddShortAlias('j').AddLongAlias("json").Build();
        var xml = Option.Builder("xml").AddShortAlias('x').AddLongAlias("xml").Build();

        parser.AddOption(json);
        parser.AddOption(xml);
        parser.AddConflict("json", "xml");
        return parser;
    }

    public static TheoryData<string[]> ConflictArgs()
    {
        return new TheoryData<string[]>
        {
            new[] { "--json", "--xml" },
            new[] { "-j", "-x" },
            new[] { "--json", "-x" },
            new[] { "-j", "--xml" }
        };
    }

    [Theory]
    [MemberData(nameof(ConflictArgs))]
    public void Parse_ThrowsOnConflictingOptions_WhenBothSet(string[] args)
    {
        var parser = CreateParserWithConflictingOutputOptions();

        Assert.Throws<ConflictingOptionsSetException>(() => parser.Parse(args));
    }

    public static TheoryData<string[], string> SingleConflictOptionArgs()
    {
        return new TheoryData<string[], string>
        {
            { new[] { "--json" }, "json" },
            { new[] { "--xml" }, "xml" },
            { new[] { "-x" }, "xml" },
            { new[] { "-j" }, "json" }
        };
    }

    [Theory]
    [MemberData(nameof(SingleConflictOptionArgs))]
    public void Parse_AllowsSingleOption_WhenConflictDeclared(
        string[] args,
        string expectedSetOption
    )
    {
        var parser = CreateParserWithConflictingOutputOptions();

        var data = Assert.IsType<ParsedData>(parser.Parse(args));

        Assert.True(data.GetOption(expectedSetOption).IsSet);
    }

    private static Parser CreateParserWithDependency()
    {
        Parser parser = new();
        var recursive = Option
            .Builder("recursive")
            .AddShortAlias('r')
            .AddLongAlias("recursive")
            .Build();
        var depth = Option
            .Builder("depth")
            .AddShortAlias('d')
            .AddLongAlias("depth")
            .AddParameter(Parameter.IntParameter(min: 0, max: 10))
            .Build();

        parser.AddOption(recursive);
        parser.AddOption(depth);
        parser.AddDependency("depth", "recursive");
        return parser;
    }

    public static TheoryData<string[]> DependencyMissingArgs()
    {
        return new TheoryData<string[]> { new[] { "--depth", "2" }, new[] { "-d", "3" } };
    }

    [Theory]
    [MemberData(nameof(DependencyMissingArgs))]
    public void Parse_ThrowsWhenDependencyMissing(string[] args)
    {
        var parser = CreateParserWithDependency();

        Assert.Throws<ParserException>(() => parser.Parse(args));
    }

    public static TheoryData<string[]> DependencyPresentArgs()
    {
        return new TheoryData<string[]> { new[] { "--recursive", "--depth", "2" }, new[] { "-r", "-d", "3" } };
    }

    [Theory]
    [MemberData(nameof(DependencyPresentArgs))]
    public void Parse_AllowsDependentOption_WhenDependencyPresent(string[] args)
    {
        var parser = CreateParserWithDependency();

        var data = Assert.IsType<ParsedData>(parser.Parse(args));

        Assert.True(data.GetOption("recursive").IsSet);
        Assert.True(data.GetOption("depth").IsSet);
    }

    [Theory]
    [InlineData("missing", "depth")]
    [InlineData("depth", "missing")]
    private void AddDependency_ThrowsOnOptionNotFound(string depA, string depB)
    {
        Parser parser = new();
        parser.AddOption(
            Option.Builder("recursive").AddShortAlias('r').AddLongAlias("recursive").Build()
        );
        parser.AddOption(
            Option
                .Builder("depth")
                .AddShortAlias('d')
                .AddLongAlias("depth")
                .AddParameter(Parameter.IntParameter())
                .Build()
        );

        Assert.Throws<OptionNotFoundException>(() => parser.AddDependency(depA, depB));
    }
}