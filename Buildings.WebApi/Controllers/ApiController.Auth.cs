using Microsoft.AspNetCore.Mvc;

namespace Buildings.Controllers;

public partial class ApiController
{
    [HttpGet("auth/register")]
    public async Task<IActionResult> Register(
        [FromQuery] string location,
        [FromQuery] int page,
        [FromQuery] int pageSize = 10)
    {
        return Ok();
    }
}