using Buildings.Commands.Account;
using Buildings.Commands.Friends;
using Buildings.Infrastructure.Data;
using Buildings.Infrastructure.Data.Entities.Account;
using Buildings.Infrastructure.Repositories;
using Buildings.Responses.Account;
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
    [HttpGet]
    [Tags("Account")]
    [ProducesResponseType<AccountPublicInfoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccountPublicAsync([FromQuery] long userId)
    {
        var account = await accountRepository.GetAccountAsync(userId);
        if (account is null)
            return NotFound("User doesn't exist.");
        var online = await accountRepository.GetUserOnlineAsync(userId);
        return Ok(new AccountPublicInfoResponse
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

    [HttpPost]
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
            .Union(dbContext.FriendRelationInfos.AsNoTracking()
                .Where(e => e.FriendId == account.UserId))
            .Where(e => e.Status == RequestStatus.Pending)
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

    #endregion

    #region Social

    [HttpGet("social/pending")]
    [Tags("Social")]
    [ProducesResponseType<NewFriendRequest[]>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPendingRequestsAsync([FromHeader] long userId)
    {
        var account = await accountRepository.GetAccountAsync(userId);
        if (account is null)
            return NotFound("User doesn't exist.");
        return Ok(new NewFriendRequest[] { });
    }

    [HttpGet("social/logs")]
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

    [HttpPut("social/dealRequest")]
    [Tags("Social")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetNewRequestsAsync(
        [FromHeader] long userId,
        [FromQuery] long targetUserId,
        [FromQuery] bool confirm
    )
    {
        var account = await accountRepository.GetAccountAsync(userId);
        if (account is null)
            return NotFound("User doesn't exist.");
        return Ok();
    }

    [HttpPut("social/sendRequest")]
    [Tags("Social")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SendNewRequestsAsync(
        [FromHeader] long userId,
        [FromQuery] long targetUserId
    )
    {
        var account = await accountRepository.GetAccountAsync(userId);
        if (account is null)
            return NotFound("User doesn't exist.");
        var target = await accountRepository.GetAccountAsync(targetUserId);
        if (target is null)
            return NotFound("User doesn't exist.");
        return Ok();
    }

    [AllowAnonymous]
    [HttpGet("social/collectTags")]
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

    [AllowAnonymous]
    [HttpPost("social/search")]
    [Tags("Social")]
    [ProducesResponseType<AccountPublicInfoResponse[]>(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchFriendsAsync([FromBody] SearchFriendsCommand command)
    {
        return Ok(new AccountPublicInfoResponse[] { });
    }

    #endregion
}