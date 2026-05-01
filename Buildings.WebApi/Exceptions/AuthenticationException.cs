using System.Runtime.Serialization;
using Buildings.Enums;

namespace Buildings.Exceptions;

[Serializable]
public class AuthenticationException : Exception
{
    public AuthenticationException(AuthenticationResult result = AuthenticationResult.BadRequest)
    {
        Result = result;
    }

    public AuthenticationException(string message, AuthenticationResult result = AuthenticationResult.BadRequest) :
        base(message)
    {
        Result = result;
    }

    public AuthenticationException(string message, Exception innerException,
        AuthenticationResult result = AuthenticationResult.BadRequest) : base(message, innerException)
    {
        Result = result;
    }

    [Obsolete]
    protected AuthenticationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    public AuthenticationResult Result { get; set; }
}