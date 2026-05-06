using Buildings.Commands.Account;
using Buildings.Commands.Friends;
using Buildings.Infrastructure.Data;
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
    ISecureRepository secureRepository,
    IDbContextFactory<BuildingDbContext> dbContextFactory
) : ControllerBase
{
    #region Account

    [AllowAnonymous]
    [HttpGet]
    [Tags("Account")]
    [ProducesResponseType<AccountPublicInfo>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccountPublicAsync([FromQuery] long userId)
    {
        var account = await secureRepository.GetAccountAsync(userId);
        if (account is null)
            return NotFound("User doesn't exist.");
        return Ok(new AccountPublicInfo
        {
            UserId = account.UserId,
            Email = account.Email,
            UserName = account.UserName,
            CreatedAt = account.CreatedAt
        });
    }

    [HttpPost]
    [Tags("Account")]
    [ProducesResponseType<AccountFullInfo>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccountFullAsync([FromHeader] long userId)
    {
        var account = await secureRepository.GetAccountAsync(userId);
        if (account is null)
            return NotFound("User doesn't exist.");
        return Ok(new AccountFullInfo
        {
            UserId = account.UserId,
            Email = account.Email,
            UserName = account.UserName,
            CreatedAt = account.CreatedAt
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
        var account = await secureRepository.GetAccountAsync(userId);
        if (account is null)
            return NotFound("User doesn't exist.");
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var total = await dbContext.FriendRelations.AsNoTracking().CountAsync();
        var request = await dbContext.FriendRequests.AsNoTracking()
            .Where(e => e.TargetUserId == userId)
            .CountAsync();
        return Ok(new FriendsSummary
        {
            Total = total,
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
        var account = await secureRepository.GetAccountAsync(userId);
        if (account is null)
            return NotFound("User doesn't exist.");
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var friends = await dbContext.FriendRelations.AsNoTracking()
            .Where(e => e.TargetUserId == userId)
            .Select(e => e.FromUserId)
            .Concat(dbContext.FriendRelations.AsNoTracking()
                .Where(e => e.FromUserId == userId)
                .Select(e => e.TargetUserId))
            .ToArrayAsync();
        var filtered = await dbContext.UserInfos.AsNoTracking()
            .Where(e => friends.Contains(e.UserId))
            .Where(e => e.UserName.Contains(command.Search) || e.Description.Contains(command.Search))
            .Select(e => new FriendInfo
            {
                UserId = e.UserId,
                UserName = e.UserName,
                Description = e.Description,
                Avatar = e.Avatar,
                Location = e.Location,
                Online = e.Online,
                Tags = e.Tags
            })
            .Skip((command.Page - 1) * 16)
            .Take(16)
            .ToArrayAsync();

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
        var account = await secureRepository.GetAccountAsync(userId);
        if (account is null)
            return NotFound("User doesn't exist.");
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var friends = await dbContext.FriendRelations.AsNoTracking()
            .Where(e => e.TargetUserId == userId)
            .Select(e => e.FromUserId)
            .Concat(dbContext.FriendRelations.AsNoTracking()
                .Where(e => e.FromUserId == userId)
                .Select(e => e.TargetUserId))
            .ToArrayAsync();
        var filtered = await dbContext.UserInfos.AsNoTracking()
            .Where(e => friends.Contains(e.UserId))
            .Where(e => e.Online)
            .Where(e => e.UserName.Contains(command.Search) || e.Description.Contains(command.Search))
            .Select(e => new FriendInfo
            {
                UserId = e.UserId,
                UserName = e.UserName,
                Description = e.Description,
                Avatar = e.Avatar,
                Location = e.Location,
                Online = e.Online,
                Tags = e.Tags
            })
            .Skip((command.Page - 1) * 16)
            .Take(16)
            .ToArrayAsync();

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
        var account = await secureRepository.GetAccountAsync(userId);
        if (account is null)
            return NotFound("User doesn't exist.");
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var friends = await dbContext.FriendRelations.AsNoTracking()
            .Where(e => e.TargetUserId == userId)
            .Select(e => e.FromUserId)
            .Concat(dbContext.FriendRelations.AsNoTracking()
                .Where(e => e.FromUserId == userId)
                .Select(e => e.TargetUserId))
            .ToArrayAsync();
        var filtered = await dbContext.UserInfos.AsNoTracking()
            .Where(e => friends.Contains(e.UserId))
            .Where(e => e.UserName.Contains(command.Search) || e.Description.Contains(command.Search))
            .Select(e => new FriendInfo
            {
                UserId = e.UserId,
                UserName = e.UserName,
                Description = e.Description,
                Avatar = e.Avatar,
                Location = e.Location,
                Online = e.Online,
                Tags = e.Tags
            })
            .Skip((command.Page - 1) * 16)
            .Take(16)
            .ToArrayAsync();

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
        var account = await secureRepository.GetAccountAsync(userId);
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
        var account = await secureRepository.GetAccountAsync(userId);
        if (account is null)
            return NotFound("User doesn't exist.");
        return Ok(new RecentActivityLog[] { });
    }

    [HttpPost("social/dealRequest")]
    [Tags("Social")]
    [ProducesResponseType<RecentActivityLog[]>(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetNewRequestsAsync(
        [FromHeader] long userId,
        [FromQuery] long targetUserId,
        [FromQuery] bool confirm
    )
    {
        var account = await secureRepository.GetAccountAsync(userId);
        if (account is null)
            return NotFound("User doesn't exist.");
        return Ok();
    }

    [AllowAnonymous]
    [HttpGet("social/collentTags")]
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
    [ProducesResponseType<AccountPublicInfo[]>(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchFriendsAsync([FromBody] SearchFriendsCommand command)
    {
        return Ok(new AccountPublicInfo[] { });
    }

    #endregion
}