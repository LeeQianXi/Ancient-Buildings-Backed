using System.Data.Common;
using Buildings.Dtos;
using Buildings.Enums;
using Buildings.Exceptions;
using Buildings.Infrastructure.Data;
using Buildings.Infrastructure.Data.Entities.Secure;
using Buildings.Utils;
using Microsoft.EntityFrameworkCore;

namespace Buildings.Infrastructure.Repositories;

internal sealed class SecureRepository(
    ILogger<SecureRepository> logger,
    IDbContextFactory<BuildingDbContext> dbContextFactory,
    IIdGenerator<long> idGenerator
) : ISecureRepository
{
    public async ValueTask<bool> ExistsAccountAsync(long userId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.UserSecureInfos.AnyAsync(au =>
            au.UserId == userId && au.DeleteAt == null);
    }

    public async ValueTask<bool> ExistsAccountAsync(string email)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.UserSecureInfos.AnyAsync(au =>
            au.Email == email && au.DeleteAt == null);
    }

    public async ValueTask<SecureUserPublicInfo?> GetAccountAsync(long userId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.UserSecureInfos.AsNoTracking()
            .Where(au => au.UserId == userId && au.DeleteAt == null)
            .Select(au => new SecureUserPublicInfo
            {
                UserId = au.UserId,
                Email = au.Email,
                UserName = au.UserName,
                CreatedAt = au.CreatedAt,
                UpdatedAt = au.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async ValueTask<SecureUserPublicInfo?> GetAccountByEmailAsync(string email)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.UserSecureInfos.AsNoTracking()
            .Where(au => au.Email == email && au.DeleteAt == null)
            .Select(au => new SecureUserPublicInfo
            {
                UserId = au.UserId,
                Email = au.Email,
                UserName = au.UserName,
                CreatedAt = au.CreatedAt,
                UpdatedAt = au.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async ValueTask<SecureUserPublicInfo> InsertAccountAsync(string email, string passwordHash, string username)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        if (await dbContext.UserSecureInfos.AsNoTracking()
                .AnyAsync(au => au.Email == email && au.DeleteAt == null))
            throw new AccountException("Email already registered", AccountAction.Register);
        var uid = idGenerator.NextId() >> 8;
        var timeNow = DateTimeOffset.UtcNow;
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var nau = new UserSecureInfo
            {
                UserId = uid,
                Email = email,
                UserName = username,
                PasswordSaltHash = passwordHash,
                CreatedAt = timeNow,
                UpdatedAt = timeNow
            };

            await dbContext.UserSecureInfos.AddAsync(nau);
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is DbException)
        {
            await transaction.RollbackAsync();
            throw new AccountException("Account already exists.", AccountAction.Register);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogError(ex, "Failed to create account");
            throw new AccountException("Failed to create account", AccountAction.Register);
        }

        return new SecureUserPublicInfo
        {
            UserId = uid,
            Email = email,
            UserName = username,
            CreatedAt = timeNow,
            UpdatedAt = timeNow
        };
    }

    public async ValueTask<SecureUserPublicInfo> ChangePasswordAsync(long userId, string newPasswordHash)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var user = await dbContext.UserSecureInfos
            .FirstOrDefaultAsync(au => au.UserId == userId && au.DeleteAt == null);
        if (user is null) throw new AccountException("Account not found", AccountAction.ChangePassword);
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            user.PasswordSaltHash = newPasswordHash;
            user.UpdatedAt = DateTimeOffset.UtcNow;
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new AccountException("Failed to change password", AccountAction.ChangePassword);
        }

        return new SecureUserPublicInfo
        {
            UserId = userId,
            Email = user.Email,
            UserName = user.UserName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async ValueTask<SecureUserPublicInfo> ChangeEmailAsync(long userId, string newEmail)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var user = await dbContext.UserSecureInfos
            .FirstOrDefaultAsync(au => au.UserId == userId && au.DeleteAt == null);
        if (user == null) throw new AccountException("Account not found", AccountAction.ChangeEmail);

        // 检查邮箱是否已被其他用户使用
        if (user.Email != newEmail && await dbContext.UserSecureInfos.AsNoTracking()
                .AnyAsync(au => au.Email == newEmail && au.UserId != userId && au.DeleteAt == null))
            throw new AccountException("Email already in use", AccountAction.ChangeEmail);
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            user.Email = newEmail;
            user.UpdatedAt = DateTimeOffset.UtcNow;
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new AccountException("Failed to change email for user", AccountAction.ChangeEmail);
        }

        return new SecureUserPublicInfo
        {
            UserId = userId,
            Email = newEmail,
            UserName = user.UserName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async ValueTask DeleteAccountAsync(long userId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var user = await dbContext.UserSecureInfos
            .FirstOrDefaultAsync(au => au.UserId == userId && au.DeleteAt == null);
        if (user == null) throw new AccountException("Account not found", AccountAction.Delete);
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var dateTime = DateTimeOffset.UtcNow;
            user.DeleteAt = dateTime;
            user.UpdatedAt = dateTime;
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new AccountException("Failed to delete account", AccountAction.Delete);
        }
    }

    public async ValueTask<(long UserId, string PasswordSaltHash)> GetAccountVerifyAsync(string email)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var up = await dbContext.UserSecureInfos.AsNoTracking()
                     .Where(au => au.Email == email && au.DeleteAt == null)
                     .Select(au => new { au.UserId, au.PasswordSaltHash })
                     .FirstOrDefaultAsync() ??
                 throw new AuthenticationException("Email does not registered", AuthenticationResult.Unauthorized);
        return (up.UserId, up.PasswordSaltHash);
    }


    public async ValueTask CacheRefreshTokenAsync(long userId, string hash, string refreshToken,
        DateTimeOffset refreshTokenExpiry)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        if (!await dbContext.UserSecureInfos.AsNoTracking()
                .AnyAsync(au => au.UserId == userId && au.DeleteAt == null))
            throw new AuthenticationException("User does not registered");
        var ut = await dbContext.UserSecureTokens
            .Where(at => at.UserId == userId && at.Hash == hash)
            .FirstOrDefaultAsync();
        try
        {
            if (ut is null)
            {
                ut = new UserSecureToken
                {
                    UserId = userId,
                    Hash = hash,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiry = refreshTokenExpiry,
                    LastAcquired = DateTimeOffset.UtcNow
                };
                await dbContext.UserSecureTokens.AddAsync(ut);
            }
            else
            {
                ut.RefreshToken = refreshToken;
                ut.RefreshTokenExpiry = refreshTokenExpiry;
                dbContext.UserSecureTokens.Update(ut);
            }

            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new AccountException("Failed to cache refresh token", AccountAction.Delete);
        }
    }

    public async ValueTask<bool> ValidateRefreshTokenAsync(long userId, string hash, string refreshToken)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var at = await dbContext.UserSecureTokens
            .Where(at => at.UserId == userId && at.Hash == hash)
            .FirstOrDefaultAsync();
        // 不存在
        if (at is null) return false;
        // 已过期
        if (at.RefreshTokenExpiry <= DateTimeOffset.UtcNow)
        {
            dbContext.UserSecureTokens.Remove(at);
            await dbContext.SaveChangesAsync();
            return false;
        }
        // 不匹配

        if (at.RefreshToken != refreshToken)
            return false;

        at.LastAcquired = DateTimeOffset.UtcNow;
        dbContext.UserSecureTokens.Update(at);
        await dbContext.SaveChangesAsync();
        return true;
    }
}