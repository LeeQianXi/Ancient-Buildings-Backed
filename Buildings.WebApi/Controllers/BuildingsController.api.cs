using Buildings.Commands.Buildings;
using Buildings.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Buildings.Controllers;

[ApiController]
[Route("/api/buildings")]
public class BuildingsController(
    ILogger<BuildingsController> logger,
    IDbContextFactory<BuildingDbContext> dbContextFactory
) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSplitPageAsync([FromBody] SplitPageCommand command)
    {
        return Ok();
    }

    [HttpGet("provinces")]
    [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProvincesAsync()
    {
        return Ok();
    }

    [HttpGet("categories")]
    [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategoriesAsync()
    {
        return Ok();
    }

    [HttpGet("dynasties")]
    [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDynastiesAsync()
    {
        return Ok();
    }
}