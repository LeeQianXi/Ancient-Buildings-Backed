using Buildings.Commands.Forum;
using Buildings.Infrastructure.Repositories;
using Buildings.Responses.Forum;
using Buildings.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buildings.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
[Tags("Forum")]
public class ForumController(
    ILogger<SecureController> logger,
    ISecureRepository secureRepository
) : ControllerBase
{
    [HttpGet("post/summary")]
    [EndpointSummary("获取帖子统计信息")]
    [ProducesResponseType<ForumSummaryResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPostSummaryAsync()
    {
        //TODO: GetData
        return Ok(new ForumSummaryResponse
        {
            TotalPosts = 0,
            TotalUsers = 0,
            HotPostSlugs = []
        });
    }

    [HttpGet("post")]
    [EndpointSummary("获取帖子分页列表")]
    [ProducesResponseType<ForumSplitPageResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPostListAsync([FromQuery] SplitPageForumCommand command)
    {
        //TODO: GetData
        return Ok(new ForumSplitPageResponse
        {
            TotalCount = 0
        });
    }

    [HttpGet("post/{postId:long}")]
    [EndpointSummary("获取帖子具体内容")]
    [ProducesResponseType<PostDataResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPostDataAsync(
        [FromRoute] long postId
    )
    {
        //TODO: GetData
        return Ok();
    }

    [HttpGet("post/{postId:long}/profile")]
    [EndpointSummary("获取帖子简介内容")]
    [ProducesResponseType<PostSlugInfo>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPostSlugAsync(
        [FromRoute] long postId
    )
    {
        //TODO: GetData
        return Ok();
    }

    [HttpPost("post/{postId:long}/comment")]
    [EndpointSummary("帖子发送评论")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> PublishCommentAsync(
        [FromRoute] long postId,
        [FromBody] PublishCommentCommand command,
        [FromServices] IIdGenerator<long> idGenerator
    )
    {
        //TODO: GetData
        return Ok();
    }

    [Authorize]
    [HttpPost("publish")]
    [EndpointSummary("发布帖子")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> PublishNewPostAsync(
        [FromBody] PublishPostCommand command,
        [FromServices] IIdGenerator<long> idGenerator
    )
    {
        //TODO: GetData
        var postId = idGenerator.NextId() >> 8;
        return Created();
    }
}