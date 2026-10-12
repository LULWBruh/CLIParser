using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = Xunit.Assert;

namespace CliParserTests;

[TestClass]
public partial class PlainArgumentsTests
{
    [TestMethod]
    public void Parse_CollectsArgumentsAfterDefaultPlainSeparator()
    {
        // Arrange
        var parser = new CliParser.Parser();

        // Act
        var result = (ParsedData)parser.Parse(new[] { "--", "file1.txt", "file2.txt" });

        // Assert
        Assert.Equal(2, result.GetPlainCount);
        CollectionAssert.AreEqual(new[] { "file1.txt", "file2.txt" }, result.PlainArguments.ToArray());
    }


    [TestMethod]
    public void Parse_TreatsEverythingAfterFirstPlainArgumentAsPlainWhenEnabled()
    {
        // Arrange
        var parser = new CliParser.Parser
        {
            AllPlainAfterFirst = true
        };
        var verbose = Option.Builder("verbose")
            .AddShortAlias('v')
            .Build();

        parser.AddOption(verbose);

        // Act
        var result = (ParsedData)parser.Parse(new[] { "asd", "-v", "arg1" });

        // Assert
        Assert.Equal(3, result.GetPlainCount);
        CollectionAssert.AreEqual(new[] { "asd", "-v", "arg1" }, result.PlainArguments.ToArray());
        Assert.False(result.GetOption("verbose").IsSet);
    }

    [TestMethod]
    public void GetPlainArgument_ThrowsWhenIndexIsOutOfRange()
    {
        // Arrange
        var parser = new CliParser.Parser();
        var result = (ParsedData)parser.Parse(new[] { "--", "only-one" });

        // Act + Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => result.GetPlainArgument(1));
    }
}