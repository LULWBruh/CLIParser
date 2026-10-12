namespace CliParserTests;

using CliParser;

public partial class PlainArgumentsTests
{
    public static TheoryData<string[]> PlainSeparatorArgs()
    {
        return new TheoryData<string[]>
        {
            new[] { "--url", "https://example.com", "--" },
            new[] { "--url", "https://example.com", "--", "one" },
            new[] { "--url", "https://example.com", "--", "one", "two" },
            new[] { "-u", "https://example.com", "--", "a", "b", "c" },
            new[] { "-u", "https://example.com", "--", "a", "b", "c", "d" }
        };
    }

    [Theory]
    [MemberData(nameof(PlainSeparatorArgs))]
    public void Parse_SeparatesPlainArguments_ByPlainSeparator(string[] args)
    {
        var parser = TestSetup.CreateParserWithRequiredUrl();

        var data = Assert.IsType<ParsedData>(parser.Parse(args));

        Assert.Equal(
            args.SkipWhile(x => x != "--").Skip(1).Count(),
            data.GetPlainCount
        );
    }

    public static TheoryData<string[], int> AllPlainAfterFirstArgs()
    {
        return new TheoryData<string[], int>
        {
            { new[] { "--url", "https://example.com", "--", "first", "-u", "still-plain" }, 3 },
            { new[] { "-u", "https://example.com", "--", "alpha", "--url", "beta" }, 3 }
        };
    }

    [Theory]
    [MemberData(nameof(AllPlainAfterFirstArgs))]
    public void Parse_AllPlainAfterFirst_TreatsRemainingAsPlain(
        string[] args,
        int expectedPlainCount
    )
    {
        var parser = TestSetup.CreateParserWithRequiredUrl();
        parser.AllPlainAfterFirst = true;

        var data = Assert.IsType<ParsedData>(parser.Parse(args));

        Assert.Equal(expectedPlainCount, data.GetPlainCount);
    }

    public static TheoryData<string[]> RawPlainArgs()
    {
        return new TheoryData<string[]>
            { new[] { "prior", "first", "u", "still-plain" }, new[] { "u", "still-plain" } };
    }

    [Theory]
    [MemberData(nameof(RawPlainArgs))]
    public void Parse_CollectsPlainArgumentsInOriginalOrder(string[] plainArgsIn)
    {
        var parser = TestSetup.CreateParserWithRequiredUrl();

        IReadOnlyList<string> args = new[] { "--url", "https://example.com", "--" }
            .Concat(plainArgsIn)
            .ToList();

        var data = Assert.IsType<ParsedData>(parser.Parse(args));

        Assert.Equal(plainArgsIn, data.PlainArguments);
        Assert.Equal(plainArgsIn.Length, data.GetPlainCount);
        for (var i = 0; i < plainArgsIn.Length; ++i)
            Assert.Equal(plainArgsIn[i], data.GetPlainArgument(i));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Parse_GetPlainArgument_ThrowsOnInvalidIndex(int index)
    {
        var parser = TestSetup.CreateParserWithRequiredUrl();

        var data = Assert.IsType<ParsedData>(
            parser.Parse(["--url", "https://example.com", "--", "x"])
        );

        Assert.Throws<ArgumentOutOfRangeException>(() => data.GetPlainArgument(index));
    }
}