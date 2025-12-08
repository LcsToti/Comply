using System;

namespace UserService.Domain.Exceptions
{
    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException(string email) 
            : base($"Usuário com email '{email}' já existe.")
        {
        }

        public UserAlreadyExistsException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
