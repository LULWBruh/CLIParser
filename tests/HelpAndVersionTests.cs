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
public class HelpAndVersionTests
{
    [DataTestMethod]
    [DataRow("--version")]
    [DataRow("-V")]
    public void Parse_ReturnsInfoResultForBuiltInVersionAliases(string alias)
    {
        // Arrange
        var parser = new CliParser.Parser
        {
            Version = "demo v1.0.0"
        };

        // Act
        var result = parser.Parse(new[] { alias });

        // Assert
        Assert.IsType<ParserInfo>(result);
        Assert.Equal("demo v1.0.0", ((ParserInfo)result).VersionString);
    }

    [DataTestMethod]
    [DataRow("--help")]
    [DataRow("-H")]
    public void Parse_ReturnsHelpResultForBuiltInHelpAliases(string alias)
    {
        // Arrange
        var parser = new CliParser.Parser();

        // Act
        var result = parser.Parse(new[] { alias });

        // Assert
        Assert.IsType<ParserInfo>(result);
    }

    [TestMethod]
    public void Parse_ReturnsHelpResultWhenShowHelpOnEmptyArgumentsIsEnabled()
    {
        // Arrange
        var parser = new CliParser.Parser
        {
            ShowHelpOnEmptyArguments = true
        };

        // Act
        var result = parser.Parse(Array.Empty<string>());

        // Assert
        Assert.IsType<ParserInfo>(result);
    }
}