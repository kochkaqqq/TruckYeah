using Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects
{
    public class CountryName : IEquatable<CountryName>
    {
        private static readonly Regex ValidationRegex = new Regex(
            @"^[A-Za-z][A-Za-z\s\'\-\.,\(\)]{0,98}[A-Za-z]$",
            RegexOptions.Singleline | RegexOptions.Compiled);

        public string Value { get; init; }

        public CountryName() { }

        public CountryName(string name)
        {
            if (!IsValid(name))
                throw new ValidationException(nameof(CountryName), GetDetailedErrorMessage(name));
            Value = name.Trim();
        }

        public static bool IsValid(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var trimmed = value.Trim();

            // Проверка длины
            if (trimmed.Length < 1 || trimmed.Length > 100)
                return false;

            // Проверка на допустимые символы
            if (!ValidationRegex.IsMatch(trimmed))
                return false;

            // Проверка, что не состоит только из пробелов или спецсимволов
            if (!Regex.IsMatch(trimmed, @"[A-Za-z]"))
                return false;

            return true;
        }

        private static string GetDetailedErrorMessage(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "Country name cannot be empty or whitespace.";

            var trimmed = value.Trim();

            if (trimmed.Length < 1 || trimmed.Length > 100)
                return "Country name must be between 1 and 100 characters long.";

            if (!Regex.IsMatch(trimmed, @"^[A-Za-z\s\'\-\.,\(\)]+$"))
                return "Country name can only contain Latin letters (A-Z, a-z), spaces, and special characters: ' - . , ( )";

            if (!Regex.IsMatch(trimmed, @"[A-Za-z]"))
                return "Country name must contain at least one Latin letter.";

            return ValidationErrorMessage;
        }

        public bool Equals(CountryName? other)
        {
            if (other == null)
                return false;
            return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj)
        {
            if (obj is CountryName other)
                return Equals(other);
            return false;
        }

        public override int GetHashCode()
        {
            return Value.ToUpperInvariant().GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }

        public static string ValidationErrorMessage { get; } =
            "Country name must be 1-100 characters long, contain only Latin letters (A-Z, a-z), " +
            "spaces, and special characters: ' - . , ( ). It cannot start or end with spaces.";

        // Дополнительные методы для работы с названием страны
        public string GetOfficialUpperCase()
        {
            return Value.ToUpperInvariant();
        }

        public string GetOfficialLowerCase()
        {
            return Value.ToLowerInvariant();
        }

        public bool IsCommonName(string possibleName)
        {
            if (string.IsNullOrWhiteSpace(possibleName))
                return false;

            return string.Equals(Value, possibleName, StringComparison.OrdinalIgnoreCase);
        }

        public string GetShortName()
        {
            // Возвращает первую часть названия (до запятой, скобки и т.д.)
            var separators = new[] { ',', '(', '(', '-' };
            var firstPart = Value.Split(separators, StringSplitOptions.RemoveEmptyEntries)[0];
            return firstPart.Trim();
        }
    }
}
