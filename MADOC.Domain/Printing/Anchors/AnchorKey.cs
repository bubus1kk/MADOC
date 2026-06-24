namespace MADOC.Domain.Printing.Anchors;

public readonly record struct AnchorKey
{
    public string Value { get; }

    public AnchorKey(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Ключ якоря не может быть пустым", nameof(value));
        }

        if (!ContainsOnlyAllowedCharacters(value))
        {
            throw new ArgumentException("Ключ якоря может содержать только латинские буквы, цифры, точку, дефис и " +
                "нижнее подчёркивание",
                nameof(value));
        }

        Value = value;
    }

    public override string ToString()
    {
        return Value;
    }

    private static bool ContainsOnlyAllowedCharacters(string value)
    {
        foreach (var character in value)
        {
            if (IsLatinLetter(character) || char.IsDigit(character))
            {
                continue;
            }

            if (character == '.' || character == '_' || character == '-')
            {
                continue;
            }

            return false;
        }

        return true;
    }

    private static bool IsLatinLetter(char character)
    {
        return character is >= 'a' and <= 'z' or >= 'A' and <= 'Z';
    }
}