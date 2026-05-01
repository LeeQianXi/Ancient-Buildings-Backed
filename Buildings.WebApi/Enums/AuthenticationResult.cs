namespace Buildings.Enums;

[Serializable]
public enum AuthenticationResult
{
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    MethodNotAllowed = 405,
    ProxyAuthenticationRequired = 407
}