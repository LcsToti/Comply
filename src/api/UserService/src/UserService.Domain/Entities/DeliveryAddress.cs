using System;

namespace UserService.Domain.Entities
{
    public record DeliveryAddress
    {
        public string Id { get; init; }
        public string Street { get; init; }
        public string Number { get; init; }
        public string City { get; init; }
        public string State { get; init; }
        public string ZipCode { get; init; }

        public DeliveryAddress(string street, string number, string city, string state, string zipCode)
        {
            if (string.IsNullOrWhiteSpace(street)) throw new ArgumentException("Rua inválida", nameof(street));
            if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Número inválido", nameof(number));
            if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("Cidade inválida", nameof(city));
            if (string.IsNullOrWhiteSpace(state)) throw new ArgumentException("Estado inválido", nameof(state));
            if (string.IsNullOrWhiteSpace(zipCode)) throw new ArgumentException("CEP inválido", nameof(zipCode));
            
            Id = Guid.NewGuid().ToString();
            Street = street.Trim();
            Number = number.Trim();
            City = city.Trim();
            State = state.Trim();
            ZipCode = zipCode.Trim();
        }
    }
}
