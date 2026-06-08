using Domain.Exceptions;

namespace Domain.ValueObjects
{
    public class CommentText : IEquatable<CommentText>
    {
        public string Value { get; init; }

        public CommentText() { }

        public CommentText(string text)
        {
            if (!IsValid(text))
                throw new ValidationException(nameof(CommentText), GetErrorMessage(text));
            Value = text.Trim();
        }

        public static bool IsValid(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var trimmed = value.Trim();

            if (trimmed.Length < 10 || trimmed.Length > 1000)
                return false;

            return true;
        }

        private static string GetErrorMessage(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "Comment cannot be empty or whitespace.";

            var trimmed = value.Trim();

            if (trimmed.Length < 10)
                return "Comment must be at least 10 characters long.";

            if (trimmed.Length > 1000)
                return "Comment cannot exceed 1000 characters.";

            return "Invalid comment text.";
        }

        public bool Equals(CommentText? other)
        {
            if (other == null)
                return false;
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj)
        {
            if (obj is CommentText other)
                return Equals(other);
            return false;
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return Value ?? string.Empty;
        }
    }
}