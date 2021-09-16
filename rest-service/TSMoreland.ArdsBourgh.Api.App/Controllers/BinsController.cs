using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static TSMoreland.ArdsBourgh.Api.App.Helpers.LogSanitizer;

namespace TSMoreland.ArdsBourgh.Api.App.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BinsController : ControllerBase
{
    private readonly ILogger<BinsController> _logger;

    public BinsController(ILogger<BinsController> logger)
    {
        _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [Route("{postcode}/binCollection")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public IActionResult GetBinType([FromRoute] string postcode)
    {
        _logger.LogInformation("Query Bin for {postcode}", Sanitize(postcode));
        return Ok("blue");
    }
}
