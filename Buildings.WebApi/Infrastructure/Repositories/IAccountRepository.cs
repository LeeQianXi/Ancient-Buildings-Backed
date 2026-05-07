using Buildings.Dtos;

namespace Buildings.Infrastructure.Repositories;

public interface IAccountRepository
{
    /// <summary>
    ///     检查账号是否存在
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>账号是否存在</returns>
    ValueTask<bool> ExistsAccountAsync(long userId);

    /// <summary>
    ///     根据用户ID获取账号
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>账号信息</returns>
    ValueTask<AccountUserPublicInfos?> GetAccountAsync(long userId);

    ValueTask<bool> GetUserOnlineAsync(long userId);

    ValueTask<bool[]> GetUserOnlineAsync(IEnumerable<long> userIds);
    ValueTask InitializeUserAsync(SecureUserPublicInfo info);
}