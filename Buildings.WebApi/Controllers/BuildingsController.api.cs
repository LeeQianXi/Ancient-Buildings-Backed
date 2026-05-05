using Buildings.Commands.Buildings;
using Buildings.Responses.Buildings;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Buildings.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
[Tags("Buildings")]
public class BuildingsController(
    ILogger<BuildingsController> logger
) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<BuildingSummaryResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummeryAsync()
    {
        //TODO: GetData
        return Ok(new BuildingSummaryResponse
        {
            Total = 0,
            Categories = new string[] { }.WrapAsPair(),
            Dynasties = new string[] { }.WrapAsPair(),
            Provinces = new string[] { }.WrapAsPair()
        });
    }

    [HttpPost]
    [ProducesResponseType<SplitPageResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSplitPageAsync(
        [FromQuery] SplitPageCommand command,
        [FromServices] IValidator<SplitPageCommand> validator
    )
    {
        var validate = await validator.ValidateAsync(command);
        if (!validate.IsValid) throw new ValidationException(validate.Errors);
        //TODO: GetData
        return Ok(new SplitPageResponse());
    }

    /// <summary>
    ///     获取hash路径下的文章
    /// </summary>
    /// <param name="hash"></param>
    /// <returns></returns>
    [HttpGet("{hash}")]
    [ProducesResponseType<BuildingArticle>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetArticleByHashAsync([FromRoute] string hash)
    {
        logger.LogInformation("GET /api/buildings/{hash}", hash);
        return Ok(new BuildingArticle());
    }

    /// <summary>
    ///     重定向到文章
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [HttpGet("name/{name}")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetArticleByNameAsync([FromRoute] string name)
    {
        var hash = name;
        return RedirectToAction("GetArticleByHash", "Buildings", new { hash });
    }
}