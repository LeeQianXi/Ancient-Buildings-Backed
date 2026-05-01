using Buildings.Dtos;
using Buildings.Enums;
using Buildings.Exceptions;
using Buildings.Infrastructure.Data;
using Buildings.Infrastructure.Data.Entities;
using Buildings.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Buildings.Infrastructure.Repositories;

internal sealed class AccountRepository(
    ILogger<AccountRepository> logger,
    IDbContextFactory<BuildingDbContext> dbContextFactory,
    IIdGenerator<long> idGenerator
) : IAccountRepository
{
    public async ValueTask<bool> ExistsAccountAsync(long userId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.AccountUsers.AnyAsync(au =>
            au.UserId == userId && au.DeleteAt == null);
    }

    public async ValueTask<bool> ExistsAccountAsync(string email)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.AccountUsers.AnyAsync(au =>
            au.Email == email && au.DeleteAt == null);
    }

    public async ValueTask<AccountUserInfo?> GetAccountAsync(long userId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.AccountUsers.AsNoTracking()
            .Where(au => au.UserId == userId && au.DeleteAt == null)
            .Select(au => new AccountUserInfo
            {
                UserId = au.UserId,
                Email = au.Email,
                UserName = au.UserName,
                CreatedAt = au.CreatedAt,
                UpdatedAt = au.UpdateAt
            })
            .FirstOrDefaultAsync();
    }

    public async ValueTask<AccountUserInfo?> GetAccountByEmailAsync(string email)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.AccountUsers.AsNoTracking()
            .Where(au => au.Email == email && au.DeleteAt == null)
            .Select(au => new AccountUserInfo
            {
                UserId = au.UserId,
                Email = au.Email,
                UserName = au.UserName,
                CreatedAt = au.CreatedAt,
                UpdatedAt = au.UpdateAt
            })
            .FirstOrDefaultAsync();
    }

    public async ValueTask<AccountUserInfo> InsertAccountAsync(string email, string passwordHash, string username)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        if (await ExistsAccountAsync(dbContext, email))
            throw new AccountException("Email already registered", AccountAction.Register);
        var uid = idGenerator.NextId();
        var timeNow = DateTimeOffset.UtcNow;
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var nau = new AccountUser
            {
                UserId = uid,
                Email = email,
                UserName = username,
                PasswordSaltHash = passwordHash,
                CreatedAt = timeNow,
                UpdateAt = timeNow
            };

            await dbContext.AccountUsers.AddAsync(nau);
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
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

        return new AccountUserInfo
        {
            UserId = uid,
            Email = email,
            UserName = username,
            CreatedAt = timeNow,
            UpdatedAt = timeNow
        };
    }

    public async ValueTask<AccountUserInfo> ChangePasswordAsync(long userId, string newPasswordHash)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var user = await dbContext.AccountUsers
            .FirstOrDefaultAsync(au => au.UserId == userId && au.DeleteAt == null);
        if (user is null) throw new AccountException("Account not found", AccountAction.ChangePassword);
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            user.PasswordSaltHash = newPasswordHash;
            user.UpdateAt = DateTimeOffset.UtcNow;
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new AccountException("Failed to change password", AccountAction.ChangePassword);
        }

        return new AccountUserInfo
        {
            UserId = userId,
            Email = user.Email,
            UserName = user.UserName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdateAt
        };
    }

    public async ValueTask<AccountUserInfo> ChangeEmailAsync(long userId, string newEmail)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var user = await dbContext.AccountUsers
            .FirstOrDefaultAsync(au => au.UserId == userId && au.DeleteAt == null);
        if (user == null) throw new AccountException("Account not found", AccountAction.ChangeEmail);

        // 检查邮箱是否已被其他用户使用
        if (user.Email != newEmail && await dbContext.AccountUsers.AsNoTracking()
                .AnyAsync(au => au.Email == newEmail && au.UserId != userId && au.DeleteAt == null))
            throw new AccountException("Email already in use", AccountAction.ChangeEmail);
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            user.Email = newEmail;
            user.UpdateAt = DateTimeOffset.UtcNow;
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new AccountException("Failed to change email for user", AccountAction.ChangeEmail);
        }

        return new AccountUserInfo
        {
            UserId = userId,
            Email = newEmail,
            UserName = user.UserName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdateAt
        };
    }

    public async ValueTask DeleteAccountAsync(long userId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var user = await dbContext.AccountUsers
            .FirstOrDefaultAsync(au => au.UserId == userId && au.DeleteAt == null);
        if (user == null) throw new AccountException("Account not found", AccountAction.Delete);
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var dateTime = DateTimeOffset.UtcNow;
            user.DeleteAt = dateTime;
            user.UpdateAt = dateTime;
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
        var up = await dbContext.AccountUsers.AsNoTracking()
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
        if (!await ExistsAccountAsync(dbContext, userId)) throw new AuthenticationException("User does not registered");
        var ut = await dbContext.AccountTokens
            .Where(at => at.UserId == userId && at.Hash == hash)
            .FirstOrDefaultAsync();
        try
        {
            if (ut is null)
            {
                ut = new AccountTokens
                {
                    UserId = userId,
                    Hash = hash,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiry = refreshTokenExpiry
                };
                await dbContext.AccountTokens.AddAsync(ut);
            }
            else
            {
                ut.RefreshToken = refreshToken;
                ut.RefreshTokenExpiry = refreshTokenExpiry;
                dbContext.AccountTokens.Update(ut);
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
        var at = await dbContext.AccountTokens.AsNoTracking()
            .Where(at => at.UserId == userId && at.Hash == hash)
            .FirstOrDefaultAsync();
        if (at is null) return false;
        if (at.RefreshTokenExpiry <= DateTimeOffset.UtcNow) return false;
        return at.RefreshToken == refreshToken;
    }

    private async ValueTask<bool> ExistsAccountAsync(BuildingDbContext dbContext, long userId)
    {
        return await dbContext.AccountUsers.AnyAsync(au =>
            au.UserId == userId && au.DeleteAt == null);
    }

    private async ValueTask<bool> ExistsAccountAsync(BuildingDbContext dbContext, string email)
    {
        return await dbContext.AccountUsers.AnyAsync(au =>
            au.Email == email && au.DeleteAt == null);
    }

    private async ValueTask<AccountUserInfo?> GetAccountAsync(BuildingDbContext dbContext, long userId)
    {
        return await dbContext.AccountUsers.AsNoTracking()
            .Where(au => au.UserId == userId && au.DeleteAt == null)
            .Select(au => new AccountUserInfo
            {
                UserId = au.UserId,
                Email = au.Email,
                UserName = au.UserName,
                CreatedAt = au.CreatedAt,
                UpdatedAt = au.UpdateAt
            })
            .FirstOrDefaultAsync();
    }

    private async ValueTask<AccountUserInfo?> GetAccountByEmailAsync(BuildingDbContext dbContext, string email)
    {
        return await dbContext.AccountUsers.AsNoTracking()
            .Where(au => au.Email == email && au.DeleteAt == null)
            .Select(au => new AccountUserInfo
            {
                UserId = au.UserId,
                Email = au.Email,
                UserName = au.UserName,
                CreatedAt = au.CreatedAt,
                UpdatedAt = au.UpdateAt
            })
            .FirstOrDefaultAsync();
    }

    private async ValueTask<AccountUserInfo> InsertAccountAsync(BuildingDbContext dbContext, string email,
        string passwordHash, string username)
    {
        if (await ExistsAccountAsync(dbContext, email))
            throw new AccountException("Email already registered", AccountAction.Register);
        var uid = idGenerator.NextId();
        var timeNow = DateTimeOffset.UtcNow;
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var nau = new AccountUser
            {
                UserId = uid,
                Email = email,
                UserName = username,
                PasswordSaltHash = passwordHash,
                CreatedAt = timeNow,
                UpdateAt = timeNow
            };

            await dbContext.AccountUsers.AddAsync(nau);
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
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

        return new AccountUserInfo
        {
            UserId = uid,
            Email = email,
            UserName = username,
            CreatedAt = timeNow,
            UpdatedAt = timeNow
        };
    }

    private async ValueTask<AccountUserInfo> ChangePasswordAsync(BuildingDbContext dbContext, long userId,
        string newPasswordHash)
    {
        var user = await dbContext.AccountUsers
            .FirstOrDefaultAsync(au => au.UserId == userId && au.DeleteAt == null);
        if (user is null) throw new AccountException("Account not found", AccountAction.ChangePassword);
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            user.PasswordSaltHash = newPasswordHash;
            user.UpdateAt = DateTimeOffset.UtcNow;
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new AccountException("Failed to change password", AccountAction.ChangePassword);
        }

        return new AccountUserInfo
        {
            UserId = userId,
            Email = user.Email,
            UserName = user.UserName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdateAt
        };
    }

    private async ValueTask<AccountUserInfo> ChangeEmailAsync(BuildingDbContext dbContext, long userId, string newEmail)
    {
        var user = await dbContext.AccountUsers
            .FirstOrDefaultAsync(au => au.UserId == userId && au.DeleteAt == null);
        if (user == null) throw new AccountException("Account not found", AccountAction.ChangeEmail);

        // 检查邮箱是否已被其他用户使用
        if (user.Email != newEmail && await dbContext.AccountUsers.AsNoTracking()
                .AnyAsync(au => au.Email == newEmail && au.UserId != userId && au.DeleteAt == null))
            throw new AccountException("Email already in use", AccountAction.ChangeEmail);
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            user.Email = newEmail;
            user.UpdateAt = DateTimeOffset.UtcNow;
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new AccountException("Failed to change email for user", AccountAction.ChangeEmail);
        }

        return new AccountUserInfo
        {
            UserId = userId,
            Email = newEmail,
            UserName = user.UserName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdateAt
        };
    }

    private async ValueTask DeleteAccountAsync(BuildingDbContext dbContext, long userId)
    {
        var user = await dbContext.AccountUsers
            .FirstOrDefaultAsync(au => au.UserId == userId && au.DeleteAt == null);
        if (user == null) throw new AccountException("Account not found", AccountAction.Delete);
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var dateTime = DateTimeOffset.UtcNow;
            user.DeleteAt = dateTime;
            user.UpdateAt = dateTime;
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new AccountException("Failed to delete account", AccountAction.Delete);
        }
    }

    private async ValueTask<(long UserId, string PasswordSaltHash)> GetAccountVerifyAsync(BuildingDbContext dbContext,
        string email)
    {
        var up = await dbContext.AccountUsers.AsNoTracking()
                     .Where(au => au.Email == email && au.DeleteAt == null)
                     .Select(au => new { au.UserId, au.PasswordSaltHash })
                     .FirstOrDefaultAsync() ??
                 throw new AuthenticationException("Email does not registered");
        return (up.UserId, up.PasswordSaltHash);
    }

    private async ValueTask CacheRefreshTokenAsync(BuildingDbContext dbContext, long userId, string hash,
        string refreshToken,
        DateTimeOffset refreshTokenExpiry)
    {
        if (!await ExistsAccountAsync(dbContext, userId)) throw new AuthenticationException("User does not registered");
        var ut = await dbContext.AccountTokens
            .Where(at => at.UserId == userId && at.Hash == hash)
            .FirstOrDefaultAsync();
        try
        {
            if (ut is null)
            {
                ut = new AccountTokens
                {
                    UserId = userId,
                    Hash = hash,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiry = refreshTokenExpiry
                };
                await dbContext.AccountTokens.AddAsync(ut);
            }
            else
            {
                ut.RefreshToken = refreshToken;
                ut.RefreshTokenExpiry = refreshTokenExpiry;
                dbContext.AccountTokens.Update(ut);
            }

            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new AccountException("Failed to cache refresh token", AccountAction.Delete);
        }
    }

    private async ValueTask<bool> ValidateRefreshTokenAsync(BuildingDbContext dbContext, long userId, string hash,
        string refreshToken)
    {
        var at = await dbContext.AccountTokens.AsNoTracking()
            .Where(at => at.UserId == userId && at.Hash == hash)
            .FirstOrDefaultAsync();
        if (at is null) return false;
        if (at.RefreshTokenExpiry <= DateTimeOffset.UtcNow) return false;
        return at.RefreshToken == refreshToken;
    }
}