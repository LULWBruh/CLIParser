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
public class ParserConfigurationTests
{
    [TestMethod]
    public void AddOptionalOption_ThrowsWhenOptionNameIsRepeated()
    {
        // Arrange
        var parser = new CliParser.Parser();

        var first = Option.Builder("output")
            .AddShortAlias('o')
            .Build();

        var second = Option.Builder("output")
            .AddLongAlias("out")
            .Build();

        parser.AddOption(first);

        // Act + Assert
        Assert.Throws<NotUniqueOptionNameException>(() => parser.AddOption(second));
    }

    [TestMethod]
    public void AddOptionalOption_ThrowsWhenAliasConflictsWithExistingOption()
    {
        // Arrange
        var parser = new CliParser.Parser();

        var first = Option.Builder("output")
            .AddShortAlias('o')
            .Build();

        var second = Option.Builder("overwrite")
            .AddShortAlias('o')
            .Build();

        parser.AddOption(first);

        // Act + Assert
        Assert.Throws<ConflictingOptionAliasesException>(() => parser.AddOption(second));
    }

    [DataTestMethod]
    [DataRow("quiet")]
    [DataRow("missing")]
    [DataRow("unknown")]
    public void AddConflict_ThrowsWhenAnyNamedOptionDoesNotExist(string missingName)
    {
        // Arrange
        var parser = new CliParser.Parser();

        var verbose = Option.Builder("verbose")
            .AddShortAlias('v')
            .Build();

        parser.AddOption(verbose);

        // Act + Assert
        Assert.Throws<OptionNotFoundException>(() => parser.AddConflict("verbose", missingName));
    }

    [TestMethod]
    public void AddDependency_ThrowsWhenDependencyNameDoesNotExist()
    {
        // Arrange
        var parser = new CliParser.Parser();

        var append = Option.Builder("append")
            .AddShortAlias('a')
            .Build();

        parser.AddOption(append);

        // Act + Assert
        Assert.Throws<OptionNotFoundException>(() => parser.AddDependency("append", "output"));
    }

    [TestMethod]
    public void AddDependency_ThrowsWhenDependentNameDoesNotExist()
    {
        // Arrange
        var parser = new CliParser.Parser();

        var output = Option.Builder("output")
            .AddShortAlias('o')
            .AddParameter(Parameter.StringParameter())
            .Build();

        parser.AddOption(output);

        // Act + Assert
        Assert.Throws<OptionNotFoundException>(() => parser.AddDependency("append", "output"));
    }

    [DataTestMethod]
    [DataRow('V')]
    public void AddShortAlias_ThrowsWhenReservedVersionShortAliasIsUsed(char alias)
    {
        // Act + Assert
        Assert.Throws<BadAliasException>(() => Option.Builder("custom").AddShortAlias(alias));
    }

    [DataTestMethod]
    [DataRow("version")]
    [DataRow("help")]
    public void AddLongAlias_ThrowsWhenReservedBuiltInLongAliasIsUsed(string alias)
    {
        // Act + Assert
        Assert.Throws<BadAliasException>(() => Option.Builder("custom").AddLongAlias(alias));
    }
}