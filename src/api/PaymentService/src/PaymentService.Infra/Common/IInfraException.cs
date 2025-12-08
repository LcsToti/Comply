namespace Payments.Infra.Common;

public abstract class InfraException(string message) : Exception(message);