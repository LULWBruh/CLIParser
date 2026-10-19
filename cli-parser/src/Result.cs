using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace CliParser;

public abstract class ParseResult { }

public enum ParserInfoType {
    Help,
    Version,
    Exception
    
}

public sealed class ParserInfo : ParseResult {
    public string VersionString { get; private init; }
    public string HelpString { get; private init; }
    public Exception? ParsingException { get; private init; }
    public ParserInfoType InfoType { get; private init; }
    internal ParserInfo(ParserInfoType infoType, string helpString, string versionString, Exception? error=null) {
        InfoType = infoType;
        VersionString = versionString;
        ParsingException = error;
        HelpString =  helpString;
    }
}

public sealed partial class ParsedData : ParseResult {
    internal Dictionary<string, OptionResult> Options { get; private init; } = new();

    internal ParsedData(Dictionary<string, Option> options) {
        AddAllOptions(options);
    }

    public List<string> PlainArguments { get; private init; } = new();

    public int GetPlainCount => PlainArguments.Count;

    public OptionResult GetOption(string name) {
        if (Options.TryGetValue(name, out OptionResult? optionResult)) {
            return optionResult;
        }
        throw new OptionNotFoundException(name);
    }

    public OptionResult GetOption(Option option) {
        return GetOption(option.Name);
    }

    public string GetPlainArgument(int index) {
        return PlainArguments[index];
    }
}

public sealed partial class OptionResult {
    private List<object> ParameterResults { get; init; } = new();
    public int GetParameterCount => ParameterResults.Count;
    public bool IsSet { get; internal set; } = false;
    internal Option _option;

    internal OptionResult(Option option) {
        _option = option;
    }
    
    public T GetParameter<T>(int index) {
        if (index < 0 || index >= _option.Parameters.Count) throw new IndexOutOfRangeException();

        object result;
        if (IsSet) {
            result = ParameterResults[index];
        }
        else {
            result = _option.Parameters[index].DefaultValue;
        }

        if (result is T resultT) return resultT; // including enum

        if (typeof(T).IsEnum && result is string enumName) {
            if (Enum.TryParse(typeof(T), enumName, true, out object? enumResult)) {
                return (T)enumResult;
            }
        }

        throw new InvalidCastException($"Cannot cast parameter to type {typeof(T).Name}");
    }

    public bool TryGetParameter<T>(int index, out T? result) {
        result = default;

        try {
            result = GetParameter<T>(index);
            return true;
        }
        catch {
            return false;
        }
    }
}