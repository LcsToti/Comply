using System;
using System.Text.RegularExpressions;

namespace UserService.Domain.ValueObjects
{
    public sealed class Password
    {
        private static readonly Regex PasswordRegex = new(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            RegexOptions.Compiled);

        public string Value { get; }

        public Password(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Senha não pode ser vazia ou nula.", nameof(value));

            if (value.Length < 8)
                throw new ArgumentException("Senha deve ter pelo menos 8 caracteres.", nameof(value));

            if (!PasswordRegex.IsMatch(value))
                throw new ArgumentException(
                    "Senha deve conter pelo menos: 1 letra minúscula, 1 letra maiúscula, 1 dígito e 1 caractere especial (@$!%*?&).", 
                    nameof(value));

            Value = value;
        }

        public static implicit operator string(Password password) => password.Value;

        public static implicit operator Password(string value) => new(value);

        public override string ToString() => "***";

        public override bool Equals(object? obj)
        {
            return obj is Password password && Value == password.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(Password left, Password right) => left?.Value == right?.Value;

        public static bool operator !=(Password left, Password right) => !(left == right);
    }
}
