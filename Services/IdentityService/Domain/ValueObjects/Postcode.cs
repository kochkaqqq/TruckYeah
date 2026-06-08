using Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects
{
    public class Postcode : IEquatable<Postcode>
    {
        private static readonly Regex EuPostcodeRegex = new Regex(
            @"^[A-Za-z0-9\s\-]{2,10}$",
            RegexOptions.Compiled);

        public string Value { get; init; }

        public Postcode() { }

        public Postcode(string postcode)
        {
            if (!IsValid(postcode))
                throw new ValidationException(nameof(Postcode), "Invalid European postcode format.");
            Value = postcode.Trim().ToUpperInvariant();
        }

        public static bool IsValid(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var trimmed = value.Trim();

            if (trimmed.Length < 2 || trimmed.Length > 10)
                return false;

            return EuPostcodeRegex.IsMatch(trimmed);
        }

        public bool Equals(Postcode? other)
        {
            if (other == null)
                return false;
            return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj)
        {
            if (obj is Postcode other)
                return Equals(other);
            return false;
        }

        public override int GetHashCode()
        {
            return Value?.ToUpperInvariant().GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return Value ?? string.Empty;
        }
    }
}
