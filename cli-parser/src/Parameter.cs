namespace CliParser;

public abstract partial class Parameter {
    public static CustomParameter<T> CustomParameter<T>(Func<string, T> parsingFunction,
        Func<T, bool>? customValidation = null, T? defaultValue = default) {
        return new CustomParameter<T>(parsingFunction, customValidation, defaultValue);
    }

    public static BoolParameter BoolParameter(bool defaultValue=false) {
        return new BoolParameter(defaultValue);
    }

    // default value explicitly first value
    public static EnumParameter EnumParameter(string firstValue, params string[] values) {
        return new EnumParameter([firstValue, ..values]);
    }

    // default value explicitly first value
    public static EnumParameter EnumParameter(Type type, bool caseSensitive = false) {
        if (type == null) {
            throw new ArgumentNullException(nameof(type));
        }
        if (!type.IsEnum) {
            throw new InvalidOperationException($"{nameof(type)} is not enumerable.");
        }
        return new EnumParameter(type, caseSensitive);
    }

    public static IntParameter IntParameter(int min = int.MinValue, int max = int.MaxValue,
        Func<int, bool>? customValidation = null, int defaultValue=0) {
        return new IntParameter(min, max, customValidation, defaultValue);
    }

    public static StringParameter StringParameter(Func<string, bool>? customValidation = null,
        string? regexValidation = null, string defaultValue="") {
        return new StringParameter(customValidation, regexValidation, defaultValue);
    }
}

public sealed partial class BoolParameter : Parameter { }

public sealed partial class EnumParameter : Parameter { }

public sealed partial class IntParameter : Parameter { }

public sealed partial class StringParameter : Parameter { }

public sealed partial class CustomParameter<T> : Parameter { }