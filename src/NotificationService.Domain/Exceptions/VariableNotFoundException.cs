namespace NotificationService.Domain.Exceptions;

public class VariableNotFoundException : Exception
{
    public string VariableName { get; }
    public string ServiceName { get; }

    public VariableNotFoundException(string message, 
        string variableName,
        string serviceName)
    {
        this.VariableName = variableName;
        this.ServiceName = serviceName;
    }

    public VariableNotFoundException(string message,
        string variableName,
        string serviceName,
        Exception innerException)
    {
        this.VariableName = variableName;
        this.ServiceName = serviceName;
    }
}