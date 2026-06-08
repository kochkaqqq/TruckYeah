using Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects
{
    public class Email : IEquatable<Email>
    {
        private static readonly Regex ValidationRegex = new Regex(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Value { get; init; }

        public Email() { }

        public Email(string email)
        {
            if (!IsValid(email))
                throw new ValidationException(nameof(Email), "Email must be a valid email address format.");
            Value = email;
        }

        public static bool IsValid(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && ValidationRegex.IsMatch(value);
        }

        public bool Equals(Email? other)
        {
            if (other == null)
                return false;
            return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj)
        {
            if (obj is Email other)
                return Value == other.Value;
            return false;
        }

        public override int GetHashCode()
        {
            return Value.ToLowerInvariant().GetHashCode();
        }

        public static string ValidationErrorMessage { get; } = "Email должен быть в правильном формате (пример: user@domain.com).";
    }
}