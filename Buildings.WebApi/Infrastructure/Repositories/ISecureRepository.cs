using Buildings.Dtos;
using Buildings.Exceptions;

namespace Buildings.Infrastructure.Repositories;

public interface ISecureRepository
{
    /// <summary>
    ///     检查账号是否存在
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>账号是否存在</returns>
    ValueTask<bool> ExistsAccountAsync(long userId);

    /// <summary>
    ///     检查账号是否存在
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <returns>账号是否存在</returns>
    ValueTask<bool> ExistsAccountAsync(string email);

    /// <summary>
    ///     根据用户ID获取账号
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>账号信息</returns>
    ValueTask<SecureUserPublicInfo?> GetAccountAsync(long userId);

    /// <summary>
    ///     根据邮箱获取账号
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <returns>账号信息</returns>
    ValueTask<SecureUserPublicInfo?> GetAccountByEmailAsync(string email);

    /// <summary>
    ///     新建账号
    ///     <br />需求初始化验证码
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <param name="passwordHash">密码哈希</param>
    /// <param name="username">用户名</param>
    /// <returns>新建的账号信息</returns>
    /// <exception cref="AccountException">邮箱已注册时抛出</exception>
    ValueTask<SecureUserPublicInfo> InsertAccountAsync(string email, string passwordHash, string username);

    /// <summary>
    ///     修改账号密码
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="newPasswordHash">新密码哈希</param>
    /// <returns>是否修改成功</returns>
    /// <exception cref="AccountException">账号不存在时抛出</exception>
    ValueTask<SecureUserPublicInfo> ChangePasswordAsync(long userId, string newPasswordHash);

    /// <summary>
    ///     修改账号邮箱
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="newEmail">新邮箱</param>
    /// <returns>是否修改成功</returns>
    ValueTask<SecureUserPublicInfo> ChangeEmailAsync(long userId, string newEmail);

    /// <summary>
    ///     删除账号
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>是否删除成功</returns>
    ValueTask DeleteAccountAsync(long userId);

    /// <summary>
    ///     根据邮箱查找账号的验证密钥
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <returns>用户ID和密码哈希</returns>
    /// <exception cref="AuthenticationException">邮箱未注册时抛出</exception>
    ValueTask<(long UserId, string PasswordSaltHash)> GetAccountVerifyAsync(string email);

    /// <summary>
    ///     保存RefreshToken
    /// </summary>
    /// <param name="userId">用户Id</param>
    /// <param name="hash"></param>
    /// <param name="refreshToken">RT</param>
    /// <param name="refreshTokenExpiry">RT过期时间</param>
    /// <returns></returns>
    ValueTask CacheRefreshTokenAsync(long userId, string hash, string refreshToken, DateTimeOffset refreshTokenExpiry);

    ValueTask<bool> ValidateRefreshTokenAsync(long userId, string hash, string refreshToken);
}