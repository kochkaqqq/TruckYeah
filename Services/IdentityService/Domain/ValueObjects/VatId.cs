using Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects
{
    public class VatId : IEquatable<VatId>
    {
        private static readonly Regex VatIdRegex = new Regex(
            @"^(AT|BE|BG|CY|CZ|DE|DK|EE|EL|ES|FI|FR|HR|HU|IE|IT|LT|LU|LV|MT|NL|PL|PT|RO|SE|SI|SK)[A-Z0-9]{2,13}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Value { get; init; }

        public VatId() { }

        public VatId(string vatId)
        {
            if (!IsValid(vatId))
                throw new ValidationException(nameof(VatId), "Invalid EU VAT ID format. Expected country code (2 letters) followed by 2-13 alphanumeric characters.");

            Value = vatId.Trim().ToUpperInvariant();
        }

        public static bool IsValid(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var trimmed = value.Trim().ToUpperInvariant();

            if (trimmed.Length < 4 || trimmed.Length > 15)
                return false;

            return VatIdRegex.IsMatch(trimmed);
        }

        public string GetCountryCode()
        {
            if (string.IsNullOrEmpty(Value))
                return string.Empty;

            return Value[..2];
        }

        public string GetLocalNumber()
        {
            if (string.IsNullOrEmpty(Value) || Value.Length <= 2)
                return string.Empty;

            return Value[2..];
        }

        public bool Equals(VatId? other)
        {
            if (other == null)
                return false;
            return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj)
        {
            if (obj is VatId other)
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