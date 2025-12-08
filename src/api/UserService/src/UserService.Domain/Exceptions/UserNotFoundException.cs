using System;

namespace UserService.Domain.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string email) 
            : base($"Usuário com email '{email}' não encontrado.")
        {
        }

        public UserNotFoundException(string id, bool byId = true) 
            : base(byId ? $"Usuário com ID '{id}' não encontrado." : $"Usuário não encontrado.")
        {
        }

        public UserNotFoundException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
