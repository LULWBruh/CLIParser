using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CliParser;

public abstract partial class Parameter {
    internal abstract object ParseParameter(string parameter);
    internal abstract string Name();
    internal abstract object DefaultValue { get; }
}

public sealed partial class BoolParameter {
    internal bool _defaultValue;

    internal BoolParameter(bool defaultValue) {
        _defaultValue = defaultValue;
    }

    internal override object ParseParameter(string parameter) {
        return bool.Parse(parameter);
    }

    internal override string Name() {
        return "<bool>";
    }

    internal override object DefaultValue => _defaultValue;
}

public sealed partial class EnumParameter {
    private object _defaultValue;
    internal bool CaseSensitive { get; init; }
    internal List<string> Values { get; init; }

    internal EnumParameter(Type enumType, bool caseSensitive) {
        CaseSensitive = caseSensitive;

        if (!CaseSensitive)
            Values = Enum.GetNames(enumType).Select(x => x.ToLower()).ToList();
        else
            Values = Enum.GetNames(enumType).ToList();

        _defaultValue = enumType.GetEnumNames()[0];
    }

    internal EnumParameter(string[] values) {
        if (!CaseSensitive)
            Values = values.Select(x => x.ToLower()).ToList();
        else
            Values = values.ToList();
        
        _defaultValue =  values[0];
    }

    internal override object ParseParameter(string parameter) {
        string output = CaseSensitive ?  parameter : parameter.ToLower();
        if (!Values.Contains(output)) {
            throw  new ParserException("Parameter " + parameter + " is invalid");
        }
        return output;
    }

    internal override string Name() {
        StringBuilder output = new();
        string separator = "[";

        foreach (var val in Values) {
            output.Append(separator + val);
            separator = " ";
        }
        
        return output.Append("]").ToString();
    }

    internal override object DefaultValue => _defaultValue;
}

public sealed partial class IntParameter {
    internal int MinValue { get; init; }
    internal int MaxValue { get; init; }
    internal Func<int, bool> CustomValidation { get; init; }
    internal int _defaultValue;

    internal IntParameter(int minValue, int maxValue, Func<int, bool>? customValidation, int defaultValue) {
        MinValue = minValue;
        MaxValue = maxValue;
        CustomValidation = customValidation is null ? x => true : customValidation;
        _defaultValue = defaultValue;
    }

    internal override object ParseParameter(string parameter) {
        int output = int.Parse(parameter);
        if (output < MinValue || output > MaxValue) {
            throw new ParserException("Parameter " + parameter + " is out of range " + MinValue + "-" + MaxValue);
        }
        if (!CustomValidation(output)) {
            throw new ParserException("Parameter " + parameter + " is invalid");
        }
        return output;
    }

    internal override string Name() {
        return "<int>";
    }

    internal override object DefaultValue => _defaultValue;
}

public sealed partial class StringParameter {
    private const string WILDCARD = ".*";
    internal Func<string, bool> CustomValidation { get; init; }
    internal string RegexPattern { get; init; }
    internal string _defaultValue;

    internal StringParameter(Func<string, bool>? customValidation, string? regexValidation, string defaultValue) {
        CustomValidation = customValidation is null ? x => true : customValidation;
        RegexPattern = regexValidation is null ? WILDCARD : regexValidation;
        _defaultValue = defaultValue;
    }

    internal override object ParseParameter(string parameter) {
        if (!Regex.IsMatch(parameter, RegexPattern)) {
            throw  new ParserException("Parameter " + parameter + " is invalid");
        }
        if  (!CustomValidation(parameter)) {
            throw new ParserException("Parameter " + parameter + " is invalid");
        }
        return parameter;
    }

    internal override string Name() {
        return "<string>";
    }

    internal override object DefaultValue => _defaultValue;
}

public sealed partial class CustomParameter<T> {
    internal Func<string, T> ParsingFunction { get; init; }
    internal Func<T, bool> CustomValidation { get; init; }
    internal T? _defaultValue;

    internal CustomParameter(Func<string, T> parsingFunction, Func<T, bool>? customValidation, T? defaultValue) {
        ParsingFunction = parsingFunction;
        CustomValidation = customValidation is null ? x => true : customValidation;
        _defaultValue = defaultValue;
    }

    internal override object ParseParameter(string parameter) {
        T output = ParsingFunction(parameter);
        if (!CustomValidation(output)) {
            throw new ParserException("Parameter " + parameter + " is invalid");
        }
        return output;
    }

    internal override string Name() {
        return $"<{typeof(T).ToString()}>";
    }

    internal override object DefaultValue => _defaultValue;
}
