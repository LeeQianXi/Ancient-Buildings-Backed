using System.Runtime.Serialization;
using Buildings.Enums;

namespace Buildings.Exceptions;

[Serializable]
public class AccountException : Exception
{
    public AccountException(AccountAction action = AccountAction.Default)
    {
        AccountAction = action;
    }

    public AccountException(string message, AccountAction action = AccountAction.Default) : base(message)
    {
        AccountAction = action;
    }

    public AccountException(string message, Exception inner, AccountAction action = AccountAction.Default) : base(
        message, inner)
    {
        AccountAction = action;
    }

    [Obsolete]
    protected AccountException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public AccountAction AccountAction { get; init; }
}