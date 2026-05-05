using Buildings.Infrastructure.Repositories;
using Buildings.Responses.Forum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buildings.Controllers;

[Authorize]
[ApiController]
[Route("/api/v1/[controller]")]
[Tags("Forum")]
public class ForumController(
    ILogger<SecureController> logger,
    IAccountRepository accountRepository
) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<ForumSummaryResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummeryAsync()
    {
        //TODO: GetData
        return Ok(new ForumSummaryResponse());
    }
}