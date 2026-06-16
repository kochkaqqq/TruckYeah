using Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects
{
    public class FullName : IEquatable<FullName>
    {
        private static readonly Regex NamePartRegex = new Regex(
             @"^[A-Za-zА-Яа-яЁё\.\-]{1,50}\z",
             RegexOptions.Singleline | RegexOptions.Compiled);
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string? MiddleName { get; init; }

        public string FullNameString => MiddleName == null
            ? $"{LastName} {FirstName}"
            : $"{LastName} {FirstName} {MiddleName}";

        public string FullNameReversed => MiddleName == null
            ? $"{FirstName} {LastName}"
            : $"{FirstName} {MiddleName} {LastName}";

        public FullName() { }

        public FullName(string lastName, string firstName, string? middleName = null)
        {
            if (!IsValidNamePart(lastName))
                throw new ValidationException(nameof(FullName), "Last name must be 1-50 characters long and can only contain letters (Latin or Cyrillic), dots, and hyphens.");

            if (!IsValidNamePart(firstName))
                throw new ValidationException(nameof(FullName), "First name must be 1-50 characters long and can only contain Latin letters, dots, and hyphens.");

            if (middleName != null && !IsValidNamePart(middleName))
                throw new ValidationException(nameof(FullName), "Middle name must be 1-50 characters long and can only contain Latin letters, dots, and hyphens, or be null.");

            LastName = lastName.Trim();
            FirstName = firstName.Trim();
            MiddleName = middleName?.Trim();
        }

        public FullName(string fullName)
        {
            var (lastName, firstName, middleName) = ParseFullName(fullName);

            if (!IsValidNamePart(lastName))
                throw new ValidationException(nameof(FullName), "Last name must be 1-50 characters long and can only contain Latin letters, dots, and hyphens.");

            if (!IsValidNamePart(firstName))
                throw new ValidationException(nameof(FullName), "First name must be 1-50 characters long and can only contain Latin letters, dots, and hyphens.");

            if (middleName != null && !IsValidNamePart(middleName))
                throw new ValidationException(nameof(FullName), "Middle name must be 1-50 characters long and can only contain Latin letters, dots, and hyphens, or be null.");

            LastName = lastName;
            FirstName = firstName;
            MiddleName = middleName;
        }

        public static bool IsValidNamePart(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var trimmed = value.Trim();

            if (!NamePartRegex.IsMatch(trimmed))
                return false;

            if (!Regex.IsMatch(trimmed, @"[A-Za-zА-Яа-яЁё]"))
                return false;

            return true;
        }

        public static bool IsValid(string fullName)
        {
            var (lastName, firstName, middleName) = ParseFullName(fullName);
            return IsValidNamePart(lastName) && IsValidNamePart(firstName) &&
                   (middleName == null || IsValidNamePart(middleName));
        }

        private static (string lastName, string firstName, string? middleName) ParseFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return (string.Empty, string.Empty, null);

            var parts = fullName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
                return (string.Empty, string.Empty, null);

            if (parts.Length == 2)
            {
                return (parts[0].Trim(), parts[1].Trim(), null);
            }
            else
            {
                // Фамилия, имя, отчество
                return (parts[0].Trim(), parts[1].Trim(), parts[2].Trim());
            }
        }

        public bool Equals(FullName? other)
        {
            if (other == null)
                return false;

            return string.Equals(LastName, other.LastName, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(FirstName, other.FirstName, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(MiddleName, other.MiddleName, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj)
        {
            if (obj is FullName other)
                return Equals(other);
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                LastName.ToUpperInvariant(),
                FirstName.ToUpperInvariant(),
                MiddleName?.ToUpperInvariant());
        }

        public override string ToString()
        {
            return FullNameString;
        }

        public static string ValidationErrorMessage { get; } =
            "Full name must contain last name and first name (1-50 characters each), middle name can be null or 1-50 characters. " +
            "Only Latin letters (A-Z, a-z), dots, and hyphens are allowed. Each part must contain at least one letter.";

        // Дополнительные полезные методы
        public string GetInitials()
        {
            if (MiddleName == null)
                return $"{LastName} {FirstName[0]}.";
            else
                return $"{LastName} {FirstName[0]}.{MiddleName[0]}.";
        }

        public string GetShortName()
        {
            if (MiddleName == null)
                return $"{FirstName} {LastName[0]}.";
            else
                return $"{FirstName} {LastName[0]}.{MiddleName[0]}.";
        }

        public bool HasMiddleName => MiddleName != null;

        public string ToUpperCase()
        {
            return FullNameString.ToUpperInvariant();
        }

        public string ToLowerCase()
        {
            return FullNameString.ToLowerInvariant();
        }

        public string Normalize()
        {
            // Приводим каждую часть к формату: первая буква заглавная, остальные строчные
            string NormalizePart(string part)
            {
                if (string.IsNullOrEmpty(part))
                    return part;

                // Обработка случаев с точками и дефисами
                var words = part.Split(new[] { '.', '-' }, StringSplitOptions.None);
                var normalized = string.Join("", words.Select((word, index) =>
                {
                    if (string.IsNullOrEmpty(word))
                        return index > 0 ? "-" : "";

                    var normalizedWord = char.ToUpper(word[0]) + word.Substring(1).ToLower();
                    if (index > 0 && part.Contains('-') && index <= part.Split('-').Length - 1)
                        return "-" + normalizedWord;
                    if (index > 0 && part.Contains('.') && index <= part.Split('.').Length - 1)
                        return "." + normalizedWord;
                    return normalizedWord;
                }));

                return normalized;
            }

            var normalizedLastName = NormalizePart(LastName);
            var normalizedFirstName = NormalizePart(FirstName);
            var normalizedMiddleName = MiddleName != null ? NormalizePart(MiddleName) : null;

            return normalizedMiddleName == null
                ? $"{normalizedLastName} {normalizedFirstName}"
                : $"{normalizedLastName} {normalizedFirstName} {normalizedMiddleName}";
        }
    }
}