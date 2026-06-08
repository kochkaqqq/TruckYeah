using Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects
{
    public class Phone : IEquatable<Phone>
    {
        private static readonly Regex CountryCodeRegex = new Regex(
            @"^\d{1,4}$",
            RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex NumberRegex = new Regex(
            @"^\d{10}$",
            RegexOptions.Singleline | RegexOptions.Compiled);

        public string CountryCode { get; init; }
        public string Number { get; init; }

        public string FullNumber => $"{CountryCode}{Number}";
        public string FullNumberWithPlus => $"+{CountryCode}{Number}";

        public Phone() { }

        public Phone(string countryCode, string number)
        {
            if (!IsValidCountryCode(countryCode))
                throw new ValidationException(nameof(Phone), "Country code must contain 1-4 digits.");

            if (!IsValidNumber(number))
                throw new ValidationException(nameof(Phone), "Phone number must contain exactly 10 digits.");

            CountryCode = countryCode;
            Number = number;
        }

        public Phone(string fullPhoneNumber)
        {
            var (countryCode, number) = ParseFullNumber(fullPhoneNumber);

            if (!IsValidCountryCode(countryCode))
                throw new ValidationException(nameof(Phone), "Country code must contain 1-4 digits.");

            if (!IsValidNumber(number))
                throw new ValidationException(nameof(Phone), "Phone number must contain exactly 10 digits.");

            CountryCode = countryCode;
            Number = number;
        }

        public static bool IsValidCountryCode(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && CountryCodeRegex.IsMatch(value);
        }

        public static bool IsValidNumber(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && NumberRegex.IsMatch(value);
        }

        public static bool IsValid(string fullPhoneNumber)
        {
            var (countryCode, number) = ParseFullNumber(fullPhoneNumber);
            return IsValidCountryCode(countryCode) && IsValidNumber(number);
        }

        private static (string countryCode, string number) ParseFullNumber(string fullNumber)
        {
            if (string.IsNullOrWhiteSpace(fullNumber))
                return (string.Empty, string.Empty);

            // Удаляем все пробелы, дефисы, скобки и точки
            var cleaned = fullNumber.Replace(" ", "")
                                   .Replace("-", "")
                                   .Replace("(", "")
                                   .Replace(")", "")
                                   .Replace(".", "");

            // Удаляем ведущий плюс, если есть
            if (cleaned.StartsWith("+"))
            {
                cleaned = cleaned.Substring(1);
            }

            // Извлекаем все цифры
            var digitsOnly = new string(cleaned.Where(char.IsDigit).ToArray());

            if (digitsOnly.Length >= 11 && digitsOnly.Length <= 14)
            {
                // Код страны - первые 1-4 цифры, номер - последние 10 цифр
                int codeLength = digitsOnly.Length - 10;
                string countryCode = digitsOnly.Substring(0, codeLength);
                string number = digitsOnly.Substring(codeLength);
                return (countryCode, number);
            }

            return (string.Empty, string.Empty);
        }

        public bool Equals(Phone? other)
        {
            if (other == null)
                return false;
            return CountryCode == other.CountryCode && Number == other.Number;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Phone other)
                return Equals(other);
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CountryCode, Number);
        }

        public override string ToString()
        {
            // Форматируем в красивый вид: +7 (123) 456-78-90
            if (Number.Length == 10)
            {
                return $"+{CountryCode} ({Number.Substring(0, 3)}) {Number.Substring(3, 3)}-{Number.Substring(6, 2)}-{Number.Substring(8, 2)}";
            }
            return $"+{CountryCode}{Number}";
        }

        public static string ValidationErrorMessage { get; } =
            "Телефон должен содержать код страны (1-4 цифры) и номер из 10 цифр.";
    }
}