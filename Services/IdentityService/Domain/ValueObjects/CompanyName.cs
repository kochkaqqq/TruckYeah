using Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects
{
    public class CompanyName : IEquatable<CompanyName>
    {
        private static readonly Regex ValidationRegex = new Regex(
            @"^[A-Za-z0-9][A-Za-z0-9\s\&\'\-\.,\(\)]{0,198}[A-Za-z0-9\)]$",
            RegexOptions.Singleline | RegexOptions.Compiled);

        public string Value { get; init; }

        public CompanyName() { }

        public CompanyName(string name)
        {
            if (!IsValid(name))
                throw new ValidationException(nameof(CompanyName), GetDetailedErrorMessage(name));
            Value = name.Trim();
        }

        public static bool IsValid(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var trimmed = value.Trim();

            // Проверка длины
            if (trimmed.Length < 1 || trimmed.Length > 200)
                return false;

            // Проверка на допустимые символы
            if (!ValidationRegex.IsMatch(trimmed))
                return false;

            // Проверка, что не состоит только из пробелов или спецсимволов
            if (!Regex.IsMatch(trimmed, @"[A-Za-z0-9]"))
                return false;

            return true;
        }

        private static string GetDetailedErrorMessage(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "Company name cannot be empty or whitespace.";

            var trimmed = value.Trim();

            if (trimmed.Length < 1 || trimmed.Length > 200)
                return "Company name must be between 1 and 200 characters long.";

            if (!Regex.IsMatch(trimmed, @"^[A-Za-z0-9\s\&\'\-\.,\(\)]+$"))
                return "Company name can only contain Latin letters (A-Z, a-z), numbers (0-9), spaces, and special characters: & ' - . , ( )";

            if (!Regex.IsMatch(trimmed, @"[A-Za-z0-9]"))
                return "Company name must contain at least one Latin letter or number.";

            return ValidationErrorMessage;
        }

        public bool Equals(CompanyName? other)
        {
            if (other == null)
                return false;
            return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj)
        {
            if (obj is CompanyName other)
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
            "Company name must be 1-200 characters long, contain only Latin letters (A-Z, a-z), " +
            "numbers (0-9), spaces, and special characters: & ' - . , ( ). It cannot start or end with spaces.";

        // Дополнительные методы для работы с названием компании
        public string GetAcronym()
        {
            var words = Value.Split(new[] { ' ', '-', '.', '&' }, StringSplitOptions.RemoveEmptyEntries);
            var acronym = string.Concat(words.Select(w => char.ToUpper(w[0])));
            return acronym;
        }

        public string GetUpperCase()
        {
            return Value.ToUpperInvariant();
        }

        public string GetLowerCase()
        {
            return Value.ToLowerInvariant();
        }

        public bool ContainsWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
                return false;

            var pattern = $@"\b{Regex.Escape(word)}\b";
            return Regex.IsMatch(Value, pattern, RegexOptions.IgnoreCase);
        }
    }
}