
using CliParser;

namespace Sandbox;

enum Resolution {
    FullHD,
    HD,
    Other
}

enum FileType
{
    Txt,
    Cfg,
    Ini
}

class Program {
    static void Main(string[] args) {
        
        Parser parser = new Parser();
        parser.Version = "2.1.0-stable";
        parser.AppName = "Sandbox";
        parser.ErrorReturnsParseInfo = true;
        parser.AddOption(
            Option.Builder("graphics")
                .AddDescription("Some random description")
                .AddShortAlias('g')
                .AddLongAlias("graphics")
                .AddParameter(Parameter.BoolParameter())
                .AddParameter(Parameter.IntParameter())
                .AddParameter(Parameter.EnumParameter(typeof(Resolution), caseSensitive: true))
            );
        // parser.AddOption(
        //     Option.Builder("asd")
        //         .Required()
        //         .AddShortAlias('j')
        //         .AddLongAlias("jkljkl")
        //     );
        // parser.AddDependency("graphics", "asd");
        // parser.AddConflict("graphics", "asd");
        
        // Console.WriteLine(parser.GenerateVersionString());
        // Console.WriteLine(parser.GenerateHelpString());
        
        var result = parser.Parse("");
        if (result is ParsedData pd) {
            pd.GetOption("graphics");
        }
        else {
            var pi = (ParserInfo)result;
            Console.WriteLine($"Version: {pi.VersionString}");
            Console.WriteLine($"Help string:\n{pi.HelpString}");
            
        }


    }
}