// ReSharper disable UnusedMember.Global

namespace CliParserTests;

using CliParser;

public class ParameterParsingTests
{
    public static TheoryData<string[], int> IntBoundaryInputs()
    {
        return new TheoryData<string[], int> { { new[] { "--depth", "0" }, 0 }, { new[] { "--depth", "10" }, 10 } };
    }

    [Theory]
    [MemberData(nameof(IntBoundaryInputs))]
    public void Parse_IntParameter_ParsesBoundaryValues(string[] args, int expectedValue)
    {
        Parser parser = new();
        parser.AddOption(
            Option
                .Builder("depth")
                .AddLongAlias("depth")
                .AddParameter(Parameter.IntParameter(min: 0, max: 10))
                .Build()
        );

        var data = Assert.IsType<ParsedData>(parser.Parse(args));

        Assert.Equal(expectedValue, data.GetOption("depth").GetParameter<int>(0));
    }

    public static TheoryData<string[]> IntOutOfRangeInputs()
    {
        return new TheoryData<string[]> { new[] { "--level", "1" }, new[] { "-l", "30" } };
    }

    [Theory]
    [MemberData(nameof(IntOutOfRangeInputs))]
    public void Parse_IntParameter_RejectsOutOfRange(string[] args)
    {
        Parser parser = new();
        parser.AddOption(
            Option
                .Builder("level")
                .AddLongAlias("level")
                .AddShortAlias('l')
                .AddParameter(Parameter.IntParameter(min: 5, max: 20))
                .Build()
        );

        Assert.Throws<ParserException>(() => parser.Parse(args));
    }

    [Theory]
    [InlineData("ABC-123", "bad")]
    [InlineData("YYY-567", " ")]
    [InlineData("ABC-987", "\n")]
    [InlineData("ZZZ-999", "ZZ-888")]
    public void Parse_StringParameter_AppliesRegexValidation(string okValue, string errValue)
    {
        Parser parser = new();
        parser.AddOption(
            Option
                .Builder("code")
                .AddLongAlias("code")
                .AddParameter(
                    Parameter.StringParameter(regexValidation: "^[A-Z]{3}-[0-9]{3}$")
                )
                .Build()
        );

        var data = Assert.IsType<ParsedData>(parser.Parse(["--code", okValue]));
        Assert.Equal(okValue, data.GetOption("code").GetParameter<string>(0));

        Assert.Throws<ParserException>(() => parser.Parse(["--code", errValue]));
    }

    public enum DayEnum
    {
        monday,
        tuesday,
        wednesday,
        thursday
    }

    public static TheoryData<string[], string, DayEnum> EnumDayInputs()
    {
        return new TheoryData<string[], string, DayEnum>
        {
            { new[] { "--day", "monday" }, "monday", DayEnum.monday },
            { new[] { "--day", "tuesday" }, "tuesday", DayEnum.tuesday },
            { new[] { "--day", "wednesday" }, "wednesday", DayEnum.wednesday }
        };
    }

    [Theory]
    [MemberData(nameof(EnumDayInputs))]
    public void Parse_EnumParameterString_ParsesAllowedValues(
        string[] args,
        string expectedStrValue,
        DayEnum _
    )
    {
        Parser parser = new();
        parser.AddOption(
            Option
                .Builder("day")
                .AddLongAlias("day")
                .AddParameter(Parameter.EnumParameter("monday", "tuesday", "wednesday"))
                .Build()
        );

        var data = Assert.IsType<ParsedData>(parser.Parse(args));

        Assert.Equal(expectedStrValue, data.GetOption("day").GetParameter<string>(0));
    }

    public static TheoryData<string[]> EnumInvalidNumberInputs()
    {
        return new TheoryData<string[]> { new[] { "--num", "V" }, new[] { "--num", "123" } };
    }

    [Theory]
    [MemberData(nameof(EnumInvalidNumberInputs))]
    public void Parse_EnumParameterString_ThrowOnWrongValues(string[] args)
    {
        Parser parser = new();
        parser.AddOption(
            Option
                .Builder("num")
                .AddLongAlias("num")
                .AddParameter(Parameter.EnumParameter("X", "XI", "XII"))
                .Build()
        );


        Assert.Throws<ParserException>(() => parser.Parse(args));
        //ParseData data = Assert.IsType<ParseData>(parser.Parse(args));

        //Assert.Throws<InvalidCastException>(() => data.GetOption("num").GetParameter<string>(0));
    }

    [Theory]
    [MemberData(nameof(EnumDayInputs))]
    public void Parse_EnumParameterType_ParsesAllowedValues(
        string[] args,
        string _,
        DayEnum expectedEnumValue
    )
    {
        Parser parser = new();
        parser.AddOption(
            Option
                .Builder("day")
                .AddLongAlias("day")
                .AddParameter(Parameter.EnumParameter(typeof(DayEnum)))
                .Build()
        );

        var data = Assert.IsType<ParsedData>(parser.Parse(args));

        Assert.Equal(expectedEnumValue, data.GetOption("day").GetParameter<DayEnum>(0));
    }

    public enum NumberEnum
    {
        X,
        XI,
        XII,
        XIII
    }

    public static TheoryData<string[]> EnumValidNumberInputs()
    {
        return new TheoryData<string[]> { new[] { "--num", "X" }, new[] { "--num", "XIII" } };
    }

    [Theory]
    [MemberData(nameof(EnumValidNumberInputs))]
    public void Parse_EnumParameterType_ThrowOnWrongValues(string[] args)
    {
        Parser parser = new();
        parser.AddOption(
            Option
                .Builder("num")
                .AddLongAlias("num")
                .AddParameter(Parameter.EnumParameter(typeof(NumberEnum)))
                .Build()
        );

        var data = Assert.IsType<ParsedData>(parser.Parse(args));

        Assert.Throws<InvalidCastException>(() =>
            data.GetOption("num").GetParameter<DayEnum>(0)
        );
    }

    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(int))]
    [InlineData(typeof(Func<>))]
    public void Parameter_EnumParameterType_ThrowsForNonEnumType(Type type)
    {
        Assert.Throws<InvalidOperationException>(() => Parameter.EnumParameter(type));
    }

    [Theory]
    [InlineData(NumberEnum.X, "x")]
    [InlineData(NumberEnum.X, "X")]
    [InlineData(NumberEnum.XI, "Xi")]
    [InlineData(NumberEnum.XI, "XI")]
    [InlineData(NumberEnum.XI, "xI")]
    public void Parse_CustomParameter_UsesParsingFunction(NumberEnum enumValue, string stringValue)
    {
        Parser parser = new();
        parser.AddOption(
            Option
                .Builder("mode")
                .AddLongAlias("mode")
                .AddParameter(
                    Parameter.CustomParameter(s => Enum.Parse<NumberEnum>(s, true))
                )
                .Build()
        );

        var data = Assert.IsType<ParsedData>(parser.Parse(["--mode", stringValue]));

        Assert.Equal(enumValue, data.GetOption("mode").GetParameter<NumberEnum>(0));
    }
}