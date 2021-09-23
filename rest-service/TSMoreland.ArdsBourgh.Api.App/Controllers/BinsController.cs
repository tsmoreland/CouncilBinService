using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TSMoreland.ArdsBorough.Api.DataTransferObjects;
using static TSMoreland.ArdsBorough.Api.App.Helpers.LogSanitizer;

namespace TSMoreland.ArdsBorough.Api.App.Controllers;

[Route("api/{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiController]
public class BinsController : ControllerBase
{
    private readonly ILogger<BinsController> _logger;

    public BinsController(ILogger<BinsController> logger)
    {
        _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
    }


    [HttpGet]
    [Route("{postcode}/{houseNumber}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public IActionResult GetNextType([FromRoute] string postcode, [FromRoute] int houseNumber)
    {
        _logger.LogInformation("Query Bin for {postcode} {HouseNumber}", Sanitize(postcode), houseNumber);
        return Ok("blue");
    }

    [HttpGet]
    [Route("{postcode}/{houseNumber}/{binType}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public IActionResult GetNextDateForType([FromRoute] string postcode, [FromRoute] int houseNumber, [FromRoute] BinType binType)
    {
        _logger.LogInformation("Query Bin for {postcode} {HouseNumber}", Sanitize(postcode), houseNumber);
        return Ok("blue");
    }


    [HttpGet]
    [Route("{postcode}/{houseNumber}/{binType}/period")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public IActionResult GetCollectionPeriod([FromRoute] string postcode, [FromRoute] int houseNumber, [FromRoute] BinType binType)
    {
        _logger.LogInformation("Query Bin for {postcode} {HouseNumber}", Sanitize(postcode), houseNumber);
        return Ok("blue");
    }
}
