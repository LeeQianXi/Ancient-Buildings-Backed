using Buildings.Dtos;
using Buildings.Infrastructure.Data;
using Buildings.Infrastructure.Data.Entities.Account;
using Buildings.Utils;
using Microsoft.EntityFrameworkCore;

namespace Buildings.Infrastructure.Repositories;

public class AccountRepository(
    ILogger<AccountRepository> logger,
    IDbContextFactory<BuildingDbContext> dbContextFactory,
    IIdGenerator<long> idGenerator
) : IAccountRepository
{
    public async ValueTask<bool> ExistsAccountAsync(long userId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.UserAccountInfos.AnyAsync(au =>
            au.UserId == userId && au.DeleteAt == null);
    }

    public async ValueTask<AccountUserPublicInfos?> GetAccountAsync(long userId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.UserAccountInfos.AsNoTracking()
            .Where(au => au.UserId == userId && au.DeleteAt == null)
            .Select(au => new AccountUserPublicInfos
            {
                UserId = au.UserId,
                UserName = au.UserName,
                Profile = au.Profile,
                Location = au.Location,
                Gender = au.Gender,
                Interest = au.Interest
            })
            .FirstOrDefaultAsync();
    }

    public async ValueTask<bool> GetUserOnlineAsync(long userId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var times = await dbContext.UserSecureTokens.AsNoTracking()
            .Where(e => e.UserId == userId)
            .Select(e => e.LastAcquired)
            .ToArrayAsync();
        return times
            .Where(t => t is not null)
            .Any(t => t?.AddMinutes(20) > DateTimeOffset.UtcNow);
    }

    public async ValueTask<bool[]> GetUserOnlineAsync(IEnumerable<long> userIds)
    {
        // 防止多次枚举
        var userIdList = userIds.ToList();
        if (userIdList.Count == 0)
            return [];
        // 提取不重复的 id 用于查询
        var distinctIds = userIdList.Distinct().ToList();
        var now = DateTimeOffset.UtcNow;
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        // 查询所有有效 token（LastAcquired 非空且仍在 20 分钟有效期内）
        var validTokenUserIds = await dbContext.UserSecureTokens
            .AsNoTracking()
            .Where(t => distinctIds.Contains(t.UserId) && t.LastAcquired != null)
            .Select(t => new { t.UserId, t.LastAcquired })
            .ToListAsync()
            .ContinueWith(task =>
            {
                // 在内存中过滤出真正在线的用户 ID
                return task.Result
                    .Where(x => x.LastAcquired!.Value.AddMinutes(20) > now)
                    .Select(x => x.UserId)
                    .Distinct()
                    .ToHashSet();
            });
        // 按原输入顺序构建返回数组
        var result = new bool[userIdList.Count];
        for (var i = 0; i < userIdList.Count; i++) result[i] = validTokenUserIds.Contains(userIdList[i]);
        return result;
    }

    public async ValueTask InitializeUserAsync(SecureUserPublicInfo info)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var account = new UserAccountInfo
        {
            UserId = info.UserId,
            UserName = info.UserName,
            CreatedAt = info.CreatedAt
        };
        await dbContext.UserAccountInfos.AddAsync(account);
        await dbContext.SaveChangesAsync();
    }
}