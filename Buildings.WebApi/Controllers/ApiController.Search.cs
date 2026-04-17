using Microsoft.AspNetCore.Mvc;

namespace Buildings.Controllers;

public partial class ApiController
{
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string location,
        [FromQuery] int page,
        [FromQuery] int pageSize = 10)
    {
        return Ok();
    }
}