using Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects
{
    public class City : IEquatable<City>
    {
        private static readonly Regex ValidationRegex = new Regex(
            @"^[A-Za-z\s\.\'-]{1,100}\z",
            RegexOptions.Singleline | RegexOptions.Compiled);

        public string Value { get; init; }

        public City() { }

        public City(string city)
        {
            if (!IsValid(city))
                throw new ValidationException(nameof(City), "City name must be 1-100 characters long and can only contain Latin letters, spaces, dots, apostrophes, and hyphens.");
            Value = city.Trim();
        }

        public static bool IsValid(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var trimmed = value.Trim();

            // Проверка на допустимые символы
            if (!ValidationRegex.IsMatch(trimmed))
                return false;

            // Проверка, что не состоит только из пробелов или спецсимволов
            if (!Regex.IsMatch(trimmed, @"[A-Za-z]"))
                return false;

            return true;
        }

        public bool Equals(City? other)
        {
            if (other == null)
                return false;
            return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj)
        {
            if (obj is City other)
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
            "Название города должно быть от 1 до 100 символов и может содержать только латинские буквы (A-Z, a-z), " +
            "пробелы, точки, апострофы и дефисы.";

        // Дополнительные методы для удобной работы
        public string ToUpperCase()
        {
            return Value.ToUpperInvariant();
        }

        public string ToLowerCase()
        {
            return Value.ToLowerInvariant();
        }

        public string Normalize()
        {
            // Приводим к формату: первая буква каждого слова заглавная, остальные строчные
            var words = Value.Split(new[] { ' ', '-', '\'' }, StringSplitOptions.RemoveEmptyEntries);
            var normalized = string.Join(" ", words.Select(word =>
                char.ToUpper(word[0]) + word.Substring(1).ToLower()));
            return normalized;
        }
    }
}