using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = Xunit.Assert;

namespace CliParser;

[TestClass]
public class ParseSuccessTests
{
    [DataTestMethod]
    [DataRow("-v")]
    [DataRow("--verbose")]
    public void Parse_SetsOptionReferencedByAlias(string alias)
    {
        // Arrange
        var parser = new CliParser.Parser();

        var verbose = Option.Builder("verbose")
            .AddShortAlias('v')
            .AddLongAlias("verbose")
            .Build();

        parser.AddOption(verbose);

        // Act
        var result = (ParsedData)parser.Parse(new[] { alias });

        // Assert
        Assert.True(result.GetOption("verbose").IsSet);
    }

    [DataTestMethod]
    [DataRow("-o", "file.txt")]
    [DataRow("--output", "file.txt")]
    public void Parse_ParsesStringParameterValueFromAlias(string alias, string expected)
    {
        // Arrange
        var parser = new CliParser.Parser();

        var output = Option.Builder("output")
            .AddShortAlias('o')
            .AddLongAlias("output")
            .AddParameter(Parameter.StringParameter())
            .Build();

        parser.AddOption(output);

        // Act
        var result = (ParsedData)parser.Parse(new[] { alias, expected });

        // Assert
        Assert.Equal(expected, result.GetOption("output").GetParameter<string>(0));
    }

    [DataTestMethod]
    [DataRow("3", 3)]
    [DataRow("10", 10)]
    public void Parse_ParsesIntParameterValue(string input, int expected)
    {
        // Arrange
        var parser = new CliParser.Parser();

        var depth = Option.Builder("depth")
            .AddShortAlias('d')
            .AddParameter(Parameter.IntParameter())
            .Build();

        parser.AddOption(depth);

        // Act
        var result = (ParsedData)parser.Parse(new[] { "-d", "3" });

        // Assert
        Assert.Equal(3, result.GetOption("depth").GetParameter<int>(0));
    }

    private enum FileType
    {
        Txt,
        Cfg,
        Ini
    }

    [TestMethod]
    public void Parse_ParsesEnumParameterValue()
    {
        // Arrange
        var parser = new CliParser.Parser();

        var extension = Option.Builder("extension")
            .AddShortAlias('e')
            .AddParameter(Parameter.EnumParameter(typeof(FileType)))
            .Build();

        parser.AddOption(extension);

        // Act
        var result = (ParsedData)parser.Parse(new[] { "-e", "Txt" });

        // Assert
        Assert.Equal(FileType.Txt, result.GetOption("extension").GetParameter<FileType>(0));
    }

    [DataTestMethod]
    [DataRow("-o file.txt")]
    [DataRow("--output file.txt")]
    public void Parse_StringOverloadParsesOutputParameter(string commandLine)
    {
        // Arrange
        var parser = new CliParser.Parser();

        var output = Option.Builder("output")
            .AddShortAlias('o')
            .AddLongAlias("output")
            .AddParameter(Parameter.StringParameter())
            .Build();

        parser.AddOption(output);

        // Act
        var result = (ParsedData)parser.Parse(commandLine);

        // Assert
        Assert.Equal("file.txt", result.GetOption("output").GetParameter<string>(0));
    }
}