using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.Services;
using MinTwitterApp.DTO;

namespace MinTwitterApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LikeController : ControllerBase
{
    private readonly LikePostService likePostService;

    public LikeController(LikePostService likePostService)
    {
        this.likePostService = likePostService;
    }

    [HttpPost("toggle")]
    public async Task<ActionResult<LikeResultDTO>> ToggleLike([FromBody] ToggleLikeRequest request)
    {
        var result = await likePostService.ToggleLikeAsync(request.UserId, request.PostId);
        return Ok(result);
    }
}
