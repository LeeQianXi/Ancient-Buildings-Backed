using Buildings.Commands.Account;
using Buildings.Commands.Friends;
using Buildings.Infrastructure.Repositories;
using Buildings.Responses.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buildings.Controllers;

[Authorize]
[ApiController]
[Route("/api/v1/[controller]")]
public class AccountController(
    ILogger<AccountController> logger,
    ISecureRepository secureRepository
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
        return Ok(new FriendsSummary());
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
        var account = await secureRepository.GetAccountAsync(userId);
        if (account is null)
            return NotFound("User doesn't exist.");
        return Ok(new SplitFriendsArrayResponse());
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
        var account = await secureRepository.GetAccountAsync(userId);
        if (account is null)
            return NotFound("User doesn't exist.");
        return Ok(new SplitFriendsArrayResponse());
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
        var account = await secureRepository.GetAccountAsync(userId);
        if (account is null)
            return NotFound("User doesn't exist.");
        return Ok(new SplitFriendsArrayResponse());
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