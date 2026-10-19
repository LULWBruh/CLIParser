
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
            Option.Builder("a")
                .AddShortAlias('a')
                .AddParameter(Parameter.IntParameter())
        );
        parser.AddOption(
            Option.Builder("b")
                .AddShortAlias('b')
        );
        parser.AddOption(
            Option.Builder("c")
                .AddShortAlias('c')
        );
        
        // parser.AddDependency("a", "c");
        parser.AddConflict("a", "c");
        var result = parser.Parse("-a 1 -b -H");
        
        
        if (result is ParsedData pd) {
            pd.GetOption("b");
        }
        else {
            var pi = (ParserInfo)result;
            Console.WriteLine($"Info type: {pi.InfoType.ToString()}");
            switch (pi.InfoType) {
                case ParserInfoType.Help:
                    Console.WriteLine(pi.HelpString);
                    break;
                case ParserInfoType.Version:
                    Console.WriteLine(pi.VersionString);
                    break;
                case ParserInfoType.Exception:
                    Console.WriteLine(pi.ParsingException!.Message);
                    break;
            }
        }
    }
}