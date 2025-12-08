using System;

namespace UserService.Domain.Exceptions
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException() 
            : base("Credenciais inválidas.")
        {
        }

        public InvalidCredentialsException(string message) 
            : base(message)
        {
        }

        public InvalidCredentialsException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
