using Buildings.Commands.Account;
using Buildings.Infrastructure.Data;
using Buildings.Infrastructure.Data.Entities.Account;
using Buildings.Infrastructure.Repositories;
using Buildings.Responses.Account;
using Buildings.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Buildings.Controllers;

[Authorize]
[ApiController]
[Route("/api/v1/[controller]")]
public class AccountController(
    ILogger<AccountController> logger,
    IAccountRepository accountRepository,
    IDbContextFactory<BuildingDbContext> dbContextFactory
) : ControllerBase
{
    #region Account

    [AllowAnonymous]
    [HttpGet("{userId:long}")]
    [EndpointSummary("获取账号信息")]
    [Tags("Account")]
    [ProducesResponseType<AccountPublicInfoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccountPublicAsync([FromRoute] long userId)
    {
        var account = await accountRepository.GetAccountAsync(userId);
        if (account is null)
            return NotFound("User doesn't exist.");
        return Ok(new AccountPublicInfoResponse
        {
            UserId = account.UserId,
            UserName = account.UserName,
            Profile = account.Profile,
            Gender = account.Gender.Code,
            Location = account.Location,
            Interest = account.Interest
        });
    }

    [HttpGet("me")]
    [EndpointSummary("获取自己信息")]
    [Tags("Account")]
    [ProducesResponseType<AccountFullInfoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccountFullAsync([FromHeader] long userId)
    {
        var account = await accountRepository.GetAccountAsync(userId);
        if (account is null)
            return NotFound("User doesn't exist.");
        var online = await accountRepository.GetUserOnlineAsync(userId);
        return Ok(new AccountFullInfoResponse
        {
            UserId = account.UserId,
            UserName = account.UserName,
            Profile = account.Profile,
            Gender = account.Gender.Code,
            Location = account.Location,
            Interest = account.Interest,
            Online = online
        });
    }

    #endregion

    #region Friends

    [HttpGet("friends")]
    [EndpointSummary("获取好友统计信息")]
    [Tags("Friends")]
    [ProducesResponseType<FriendsSummary>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFriendsSummaryAsync([FromHeader] long userId)
    {
        var account = await accountRepository.GetAccountAsync(userId);
        if (account is null)
            return NotFound("User doesn't exist.");
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var friends = await dbContext.FriendRelationInfos.AsNoTracking()
            .Where(e => e.UserId == userId)
            .Union(dbContext.FriendRelationInfos.AsNoTracking()
                .Where(e => e.FriendId == account.UserId))
            .Select(e => new { FriendId = e.UserId == userId ? e.FriendId : e.UserId, e.Status })
            .ToArrayAsync();
        var total = friends.Count(e => e.Status == RequestStatus.Accepted);
        var request = friends.Count(e => e.Status == RequestStatus.Pending);
        var onlineUserIds = await accountRepository.GetUserOnlineAsync(friends
            .Where(e => e.Status == RequestStatus.Accepted)
            .Select(e => e.FriendId));
        return Ok(new FriendsSummary
        {
            Total = total,
            Online = onlineUserIds.Length,
            Request = request
        });
    }

    [HttpPost("friends/all")]
    [EndpointSummary("获取分页全部好友")]
    [Tags("Friends")]
    [ProducesResponseType<SplitFriendsArrayResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSplitPageFriendsAllAsync(
        [FromHeader] long userId,
        [FromBody] SplitPageFriendsCommand command
    )
    {
        if (command.Page <= 0) return BadRequest("Page can not be negative.");
        var account = await accountRepository.GetAccountAsync(userId);
        if (account is null)
            return NotFound("User doesn't exist.");
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var friends = await dbContext.FriendRelationInfos.AsNoTracking()
            .Where(e => e.UserId == userId)
            .Union(dbContext.FriendRelationInfos.AsNoTracking()
                .Where(e => e.FriendId == account.UserId))
            .Where(e => e.Status == RequestStatus.Accepted)
            .AsSplitQuery()
            .Select(e => e.UserId == userId ? e.Friend : e.User)
            .Where(e => e.UserName.Contains(command.Search) || e.Profile.Contains(command.Search))
            .OrderBy(e => e.UpdatedAt)
            .Skip((command.Page - 1) * 16)
            .Take(16)
            .ToArrayAsync();
        var filtered = friends
            .Select(e => new FriendInfo
            {
                UserId = e.UserId,
                UserName = e.UserName,
                Profile = e.Profile,
                Location = e.Location,
                Interest = e.Interest
            })
            .ToArray();
        return Ok(new SplitFriendsArrayResponse
        {
            Users = filtered,
            Count = filtered.Length
        });
    }

    [HttpPost("friends/online")]
    [EndpointSummary("获取分页最近在线好友")]
    [Tags("Friends")]
    [ProducesResponseType<SplitFriendsArrayResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSplitPageFriendsOnlineAsync(
        [FromHeader] long userId,
        [FromBody] SplitPageFriendsCommand command
    )
    {
        if (command.Page <= 0) return BadRequest("Page can not be negative.");
        var account = await accountRepository.GetAccountAsync(userId);
        if (account is null)
            return NotFound("User doesn't exist.");
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var friends = await dbContext.FriendRelationInfos.AsNoTracking()
            .Where(e => e.UserId == userId)
            .Union(dbContext.FriendRelationInfos.AsNoTracking()
                .Where(e => e.FriendId == account.UserId))
            .Where(e => e.Status == RequestStatus.Accepted)
            .AsSplitQuery()
            .Select(e => e.UserId == userId ? e.Friend : e.User)
            .Where(e => e.UserName.Contains(command.Search) || e.Profile.Contains(command.Search))
            .AsSplitQuery()
            .Where(e => dbContext.UserSecureTokens.AsNoTracking()
                .Where(ut => ut.UserId == e.UserId)
                .Select(ut => ut.LastAcquired!)
                .Where(ut => ut != null)
                .Any(t => t > DateTimeOffset.UtcNow.AddMinutes(-20)))
            .OrderBy(e => e.UpdatedAt)
            .Skip((command.Page - 1) * 16)
            .Take(16)
            .ToArrayAsync();
        var filtered = friends
            .Select(e => new FriendInfo
            {
                UserId = e.UserId,
                UserName = e.UserName,
                Profile = e.Profile,
                Location = e.Location,
                Interest = e.Interest
            })
            .ToArray();
        return Ok(new SplitFriendsArrayResponse
        {
            Users = filtered,
            Count = filtered.Length
        });
    }

    [HttpPost("friends/recent")]
    [EndpointSummary("获取分页最近联系好友")]
    [Tags("Friends")]
    [ProducesResponseType<SplitFriendsArrayResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSplitPageFriendsRecentAsync(
        [FromHeader] long userId,
        [FromBody] SplitPageFriendsCommand command
    )
    {
        if (command.Page <= 0) return BadRequest("Page can not be negative.");
        var account = await accountRepository.GetAccountAsync(userId);
        if (account is null)
            return NotFound("User doesn't exist.");
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var friends = await dbContext.FriendRelationInfos.AsNoTracking()
            .Where(e => e.UserId == userId)
            .Select(e => new { Target = e.Friend, Entity = e })
            .Union(dbContext.FriendRelationInfos.AsNoTracking()
                .Where(e => e.FriendId == userId)
                .Select(e => new { Target = e.User, Entity = e }))
            .Where(e => e.Entity.Status == RequestStatus.Pending)
            .AsSplitQuery()
            .Where(e => e.Target.UserName.Contains(command.Search) || e.Target.Profile.Contains(command.Search))
            .OrderBy(e => e.Target.UpdatedAt)
            .Skip((command.Page - 1) * 16)
            .Take(16)
            .ToArrayAsync();
        var filtered = friends
            .Select(e => new FriendInfo
            {
                UserId = e.Target.UserId,
                UserName = e.Target.UserName,
                Profile = e.Target.Profile,
                Location = e.Target.Location,
                Interest = e.Target.Interest
            })
            .ToArray();
        return Ok(new SplitFriendsArrayResponse
        {
            Users = filtered,
            Count = filtered.Length
        });
    }

    #endregion

    #region Social

    [HttpGet("social/pending")]
    [EndpointSummary("获取好友请求列表")]
    [Tags("Social")]
    [ProducesResponseType<NewFriendRequest[]>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPendingRequestsAsync([FromHeader] long userId)
    {
        var account = await accountRepository.GetAccountAsync(userId);
        if (account is null)
            return NotFound("User doesn't exist.");
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var friends = await dbContext.FriendRelationInfos.AsNoTracking()
            .Where(e => e.UserId == userId)
            .Select(e => new { Target = e.Friend, Entity = e })
            .Union(dbContext.FriendRelationInfos.AsNoTracking()
                .Where(e => e.FriendId == userId)
                .Select(e => new { Target = e.User, Entity = e }))
            .Where(e => e.Entity.Status == RequestStatus.Pending)
            .ToArrayAsync();
        return Ok(friends.Select(e => new NewFriendRequest
        {
            UserId = e.Target.UserId,
            UserName = e.Target.UserName,
            CreatedAt = e.Entity.CreatedAt.ToRelativeTimeString()
        }));
    }

    [HttpGet("social/logs")]
    [EndpointSummary("获取好友最近操作(未实装)")]
    [Tags("Social")]
    [ProducesResponseType<RecentActivityLog[]>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRecentActionsAsync([FromHeader] long userId)
    {
        var account = await accountRepository.GetAccountAsync(userId);
        if (account is null)
            return NotFound("User doesn't exist.");

        return Ok(new RecentActivityLog[] { });
    }

    [HttpPut("social/dealRequest/accept")]
    [EndpointSummary("获取处理好友请求")]
    [Tags("Social")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AcceptRequestAsync(
        [FromHeader] long userId,
        [FromQuery] long targetUserId,
        [FromQuery] bool confirm
    )
    {
        var account = await accountRepository.GetAccountAsync(userId);
        if (account is null)
            return NotFound("User doesn't exist.");
        var target = await accountRepository.GetAccountAsync(targetUserId);
        if (target is null)
            return NotFound("User doesn't exist.");
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var friend = await dbContext.FriendRelationInfos.AsNoTracking()
            .Where(e => e.UserId == userId)
            .Select(e => new { Target = e.Friend, Entity = e })
            .Union(dbContext.FriendRelationInfos.AsNoTracking()
                .Where(e => e.FriendId == userId)
                .Select(e => new { Target = e.User, Entity = e }))
            .Where(e => e.Entity.Status == RequestStatus.Pending)
            .Where(e => e.Target.UserId == targetUserId)
            .FirstOrDefaultAsync();
        if (friend is null)
            return NotFound("Request doesn't exist.");
        friend.Entity.Status = confirm ? RequestStatus.Accepted : RequestStatus.Rejected;
        friend.Entity.ActionUserId = userId;
        dbContext.FriendRelationInfos.Update(friend.Entity);
        await dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("social/sendRequest")]
    [EndpointSummary("发送好友请求")]
    [Tags("Social")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SendNewRequestsAsync(
        [FromHeader] long userId,
        [FromQuery] long targetUserId,
        [FromServices] IIdGenerator<long> idGenerator
    )
    {
        var account = await accountRepository.GetAccountAsync(userId);
        if (account is null)
            return NotFound("User doesn't exist.");
        var target = await accountRepository.GetAccountAsync(targetUserId);
        if (target is null)
            return NotFound("User doesn't exist.");
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var relation = await dbContext.FriendRelationInfos.AsNoTracking()
            .Where(e => (e.UserId == userId && e.FriendId == targetUserId) ||
                        (e.FriendId == userId && e.UserId == targetUserId))
            .FirstOrDefaultAsync(e => e.Status != RequestStatus.Deleted || e.Status != RequestStatus.Rejected);
        if (relation is not null && relation.Status is not (RequestStatus.Rejected or RequestStatus.Deleted))
            return BadRequest("Relation Already Exists.");
        if (relation is null)
        {
            relation = new FriendRelationInfo
            {
                UserId = userId,
                FriendId = targetUserId,
                Id = idGenerator.NextId() >> 8,
                CreatedAt = DateTimeOffset.UtcNow
            };
            dbContext.FriendRelationInfos.Add(relation);
        }
        else
        {
            if (relation.Status is RequestStatus.Accepted)
                return Created();
            relation.Status = RequestStatus.Pending;
            dbContext.FriendRelationInfos.Update(relation);
        }

        await dbContext.SaveChangesAsync();
        return Created();
    }

    [AllowAnonymous]
    [HttpGet("social/collectTags")]
    [EndpointSummary("获取好友过滤标签")]
    [Tags("Social")]
    [ProducesResponseType<string[]>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSearchTagAsync()
    {
        return Ok((string[])
        [
            "mingqing", "garden", "greatwall", "gugong", "pagoda", "dougong", "mural", "cave", "woodcarving",
            "buddhist", "tang", "song"
        ]);
    }

    [HttpPost("social/search")]
    [EndpointSummary("获取用户搜索列表")]
    [Tags("Social")]
    [ProducesResponseType<AccountPublicInfoResponse[]>(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchFriendsAsync(
        [FromHeader] long userId,
        [FromBody] SearchFriendsCommand command
    )
    {
        var account = await accountRepository.GetAccountAsync(userId);
        if (account is null)
            return NotFound("User doesn't exist.");
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var friends = await dbContext.FriendRelationInfos.AsNoTracking()
            .Where(e => e.UserId == userId)
            .Select(e => new { Target = e.Friend, Entity = e })
            .Union(dbContext.FriendRelationInfos.AsNoTracking()
                .Where(e => e.FriendId == userId)
                .Select(e => new { Target = e.User, Entity = e }))
            .Where(e => e.Entity.Status == RequestStatus.Accepted || e.Entity.Status == RequestStatus.Pending)
            .Select(e => e.Target.UserId)
            .ToListAsync();
        var users = dbContext.UserAccountInfos.AsNoTracking()
            .Where(e => e.UserId != userId)
            .Where(e => !friends.Contains(e.UserId))
            .OrderBy(e => e.UpdatedAt)
            .Where(e => EF.Functions.JsonContains(e.Interest, command.SearchTags));
        users = command.Searches
            .Aggregate(users, (current, search) =>
                current.Where(e =>
                    e.UserName.Contains(search) ||
                    e.Profile.Contains(search)));
        var limit = await users
            .Skip((command.Page - 1) * 12)
            .Take(12)
            .Select(e => new AccountPublicInfoResponse
            {
                UserId = e.UserId,
                UserName = e.UserName,
                Profile = e.Profile,
                Location = e.Location,
                Interest = e.Interest,
                Gender = e.Gender.Code
            })
            .ToArrayAsync();
        return Ok(limit);
    }

    #endregion
}