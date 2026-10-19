# CLIParser

Jednoduchý parser argumentů příkazové řádky pro .NET. Parser používá pojmenované
options, krátké i dlouhé aliasy a typované parametry.

## Základní použití

Následující příklad je kompletní `Program.cs` malého nástroje pro zpracování
souborů. Ukazuje styl konfigurace parseru používaný v `Sandbox/Program.cs`.

```csharp
using CliParser;

namespace FileTool;

enum OutputFormat
{
    Text,
    Json,
    Csv
}

class Program
{
    static void Main(string[] args)
    {
        Parser parser = new Parser();
        parser.Version = "2.1.0-stable";
        parser.AppName = "file-tool";
        parser.ErrorReturnsParseInfo = true;
        parser.ShowHelpOnEmptyArguments = true;

        parser.AddOption(
            Option.Builder("input")
                .AddShortAlias('i')
                .AddLongAlias("input")
                .AddDescription("Vstupní soubor ke zpracování.")
                .AddParameter(Parameter.StringParameter())
                .Required()
        );

        parser.AddOption(
            Option.Builder("output")
                .AddShortAlias('o')
                .AddLongAlias("output")
                .AddDescription("Cílový soubor. Pokud není zadán, výsledek se vypíše na konzoli.")
                .AddParameter(Parameter.StringParameter(
                    regexValidation: @"^.+\.(txt|json|csv)$"))
        );

        parser.AddOption(
            Option.Builder("format")
                .AddShortAlias('f')
                .AddLongAlias("format")
                .AddDescription("Formát výstupu.")
                .AddParameter(Parameter.EnumParameter(typeof(OutputFormat)))
        );

        parser.AddOption(
            Option.Builder("level")
                .AddShortAlias('l')
                .AddLongAlias("level")
                .AddDescription("Úroveň zpracování od 1 do 5.")
                .AddParameter(Parameter.IntParameter(min: 1, max: 5, defaultValue: 1))
        );

        parser.AddOption(
            Option.Builder("verbose")
                .AddShortAlias('v')
                .AddLongAlias("verbose")
                .AddDescription("Zapne podrobné logování.")
        );

        parser.AddOption(
            Option.Builder("dry-run")
                .AddShortAlias('d')
                .AddLongAlias("dry-run")
                .AddDescription("Pouze zobrazí, co by se provedlo.")
        );

        // --output dává smysl pouze společně s --input.
        parser.AddDependency("output", "input");

        // --dry-run a --output není možné použít současně.
        parser.AddConflict("dry-run", "output");

        ParseResult result = parser.Parse(args);

        if (result is ParsedData pd)
        {
            OptionResult input = pd.GetOption("input");
            OptionResult output = pd.GetOption("output");
            OptionResult format = pd.GetOption("format");
            OptionResult level = pd.GetOption("level");
            OptionResult verbose = pd.GetOption("verbose");
            OptionResult dryRun = pd.GetOption("dry-run");

            string inputPath = input.GetParameter<string>(0);
            string outputPath = output.GetParameter<string>(0);
            OutputFormat outputFormat = format.GetParameter<OutputFormat>(0);
            int processingLevel = level.GetParameter<int>(0);
            bool isVerbose = verbose.IsSet;

            Console.WriteLine($"Input: {inputPath}");
            Console.WriteLine($"Output: {(output.IsSet ? outputPath : "console")}");
            Console.WriteLine($"Format: {outputFormat}");
            Console.WriteLine($"Level: {processingLevel}");
            Console.WriteLine($"Verbose: {isVerbose}");
            Console.WriteLine($"Dry run: {dryRun.IsSet}");

            if (pd.GetPlainCount > 0)
            {
                Console.WriteLine("Plain arguments:");
                for (int i = 0; i < pd.GetPlainCount; i++)
                {
                    Console.WriteLine($"  {pd.GetPlainArgument(i)}");
                }
            }

            // Zde může následovat vlastní zpracování souboru.
            // inputPath, outputPath, outputFormat, processingLevel,
            // isVerbose a dryRun.IsSet obsahují hodnoty z příkazové řádky.
        }
        else
        {
            ParserInfo pi = (ParserInfo)result;
            Console.WriteLine($"Info type: {pi.InfoType}");

            switch (pi.InfoType)
            {
                case ParserInfoType.Help:
                    Console.WriteLine(pi.HelpString);
                    break;
                case ParserInfoType.Version:
                    Console.WriteLine(pi.VersionString);
                    break;
                case ParserInfoType.Exception:
                    Console.WriteLine(pi.ParsingException!.Message);
                    Console.WriteLine();
                    Console.WriteLine(pi.HelpString);
                    break;
            }
        }
    }
}
```

Parser automaticky poskytuje také vestavěné aliasy `-H`/`--help` a
`-V`/`--version`. Proto je není potřeba registrovat pomocí `AddOption`.

## Příklady spuštění

```text
file-tool --input data.txt --format json --level 3
file-tool -i data.txt -f csv -o result.csv -v
file-tool -i data.txt --dry-run
file-tool -i data.txt --format text report.txt
file-tool --help
file-tool --version
```

První neoptionový argument ukončí parsování options, pokud je
`AllPlainAfterFirst` nastavené na `true` (výchozí hodnota). Všechny další
argumenty se uloží do `ParsedData.PlainArguments`. Pro explicitní oddělení lze
použít `--`:

```text
file-tool -i data.txt -- --soubor-ktery-neni-option.txt dalsi-argument
```

## Builder options

Option se vytváří přes `Option.Builder("name")` a lze jí nastavit:

| Metoda | Význam |
| --- | --- |
| `AddShortAlias('x')` | Krátký alias, například `-x`. |
| `AddLongAlias("example")` | Dlouhý alias, například `--example`. |
| `AddParameter(...)` | Přidá jeden nebo více parametrů option. |
| `AddDescription("...")` | Text zobrazovaný v automatické nápovědě. |
| `Required()` | Option musí být v argumentech přítomná. |

Název optionu (`"input"`) se používá při čtení výsledku přes
`ParsedData.GetOption`. Alias slouží pouze pro zápis v příkazové řádce.

## Typy parametrů

Knihovna obsahuje několik připravených typů parametrů:

```csharp
Parameter.BoolParameter(defaultValue: false)
Parameter.IntParameter(min: 1, max: 10, defaultValue: 1)
Parameter.StringParameter(regexValidation: @"^\w+\.txt$")
Parameter.EnumParameter(typeof(OutputFormat))
Parameter.EnumParameter("small", "medium", "large")
```

Vlastní typ lze vytvořit pomocí `CustomParameter<T>`:

```csharp
parser.AddOption(
    Option.Builder("port")
        .AddShortAlias('p')
        .AddParameter(Parameter.CustomParameter(
            value => int.Parse(value),
            value => value >= 1024 && value <= 65535,
            defaultValue: 8080))
);
```

Parametry se čtou podle jejich pořadí v optionu (`0`, `1`, ...). Pokud option
nebyla zadána, `GetParameter<T>` vrací její výchozí hodnotu. Přítomnost option
bez parametru lze ověřit přes `OptionResult.IsSet`.

## Výsledek parsování

`Parse` vrací abstraktní `ParseResult`, který je buď:

- `ParsedData` při úspěšném zpracování argumentů,
- `ParserInfo` pro help, verzi nebo chybu při nastavení
  `ErrorReturnsParseInfo = true`.

Při běžném výsledku jsou k dispozici:

```csharp
ParsedData data = (ParsedData)parser.Parse(args);
OptionResult option = data.GetOption("input");
bool wasSet = option.IsSet;
string value = option.GetParameter<string>(0);
int plainCount = data.GetPlainCount;
string plainArgument = data.GetPlainArgument(0);
```

`ParserInfo.InfoType` rozlišuje `Help`, `Version` a `Exception`. Texty
`HelpString` a `VersionString` jsou vygenerované z konfigurace parseru.