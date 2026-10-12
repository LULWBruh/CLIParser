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
public class ParseFailureTests
{
    [TestMethod]
    public void Parse_ThrowsWhenRequiredOptionIsMissing()
    {
        // Arrange
        var parser = new CliParser.Parser();

        var urlOption = Option.Builder("url")
            .Required()
            .AddLongAlias("url")
            .AddParameter(Parameter.StringParameter())
            .Build();

        parser.AddOption(urlOption);

        // Act + Assert
        Assert.Throws<RequiredOptionMissingException>(() => parser.Parse(Array.Empty<string>()));
    }

    [TestMethod]
    public void Parse_ThrowsWhenConflictingOptionsAreBothSet()
    {
        // Arrange
        var parser = new CliParser.Parser();

        var verbose = Option.Builder("verbose")
            .AddShortAlias('v')
            .Build();

        var quiet = Option.Builder("quiet")
            .AddShortAlias('q')
            .Build();

        parser.AddOption(verbose);
        parser.AddOption(quiet);
        parser.AddConflict("verbose", "quiet");

        // Act + Assert
        Assert.Throws<ConflictingOptionsSetException>(() => parser.Parse(new[] { "-v", "-q" }));
    }
}