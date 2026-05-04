using Buildings.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buildings.Controllers;

[ApiController]
[Route("/api/account")]
[Authorize]
public class AccountController(
    ILogger<AccountController> logger,
    IAccountRepository accountRepository
) : ControllerBase
{
    [HttpGet("friends")]
    public async Task<IActionResult> GetSplitPageAsync()
    {
        return Ok();
    }
}