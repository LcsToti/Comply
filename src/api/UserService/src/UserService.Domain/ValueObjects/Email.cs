using System;
using System.Text.RegularExpressions;

namespace UserService.Domain.ValueObjects
{
    public sealed class Email
    {
        private static readonly Regex EmailRegex = new(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Value { get; }

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email não pode ser vazio ou nulo.", nameof(value));

            var trimmedValue = value.Trim();
            if (!EmailRegex.IsMatch(trimmedValue))
                throw new ArgumentException("Formato de email inválido.", nameof(value));

            Value = trimmedValue.ToLowerInvariant();
        }

        public static implicit operator string(Email email) => email.Value;

        public static implicit operator Email(string value) => new(value);

        public override string ToString() => Value;

        public override bool Equals(object? obj)
        {
            return obj is Email email && Value == email.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(Email left, Email right) => left?.Value == right?.Value;

        public static bool operator !=(Email left, Email right) => !(left == right);
    }
}
