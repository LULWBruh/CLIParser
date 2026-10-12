using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = Xunit.Assert;

namespace CliParser
{
    [TestClass]
    public class OptionResultTests
    {
        [TestMethod]
        public void GetOption_ReturnsSameResultWhenLookedUpByNameAndByInstance()
        {
            // Arrange
            var parser = new CliParser.Parser();

            var output = Option.Builder("output")
                .AddShortAlias('o')
                .AddParameter(Parameter.StringParameter())
                .Build();

            parser.AddOption(output);

            // Act
            var result = (ParsedData)parser.Parse(new[] { "-o", "log.txt" });

            // Assert
            Assert.Equal(
                result.GetOption("output").GetParameter<string>(0),
                result.GetOption(output).GetParameter<string>(0));
        }



        [TestMethod]
        public void GetParameter_ThrowsInvalidCastWhenRequestedTypeDoesNotMatch()
        {
            // Arrange
            var parser = new CliParser.Parser();

            var depth = Option.Builder("depth")
                .AddShortAlias('d')
                .AddParameter(Parameter.IntParameter())
                .Build();

            parser.AddOption(depth);

            var result = (ParsedData)parser.Parse(new[] { "-d", "4" });

            // Act + Assert
            Assert.Throws<InvalidCastException>(() => result.GetOption("depth").GetParameter<string>(0));
        }

        [TestMethod]
        public void EnumParameter_ThrowsWhenTypeIsNotEnum()
        {
            // Act + Assert
            Assert.Throws<InvalidOperationException>(() => Parameter.EnumParameter(typeof(string)));
        }
    }
}