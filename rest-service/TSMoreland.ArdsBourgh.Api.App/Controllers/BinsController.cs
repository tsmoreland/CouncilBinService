using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TSMoreland.ArdsBorough.Api.DataTransferObjects;
using TSMoreland.ArdsBorough.Api.WebServiceFacade.Shared;
using static TSMoreland.ArdsBorough.Api.App.Helpers.LogSanitizer;

namespace TSMoreland.ArdsBorough.Api.App.Controllers;

[Route("api/v{version:apiVersion}/bins")]
//[Route("api/bins")]
[ApiVersion("1")]
[ApiController]
public class BinsController : ControllerBase
{
    private readonly IWebServiceFacadeFactory _webServiceFacadeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BinsController> _logger;

    public BinsController(IWebServiceFacadeFactory webServiceFacadeFactory, IConfiguration configuration, ILogger<BinsController> logger)
    {
        _webServiceFacadeFactory = webServiceFacadeFactory;
        _configuration = configuration; // temporary
        _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [Route("{postcode}/{houseNumber:int}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetThisWeeksType([FromRoute] string postcode, [FromRoute] int houseNumber, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Query Bin for {postcode} {HouseNumber}", Sanitize(postcode), houseNumber);
        var secret = _configuration["test-data:secret"] ?? string.Empty;
        var webService = _webServiceFacadeFactory.Build(secret);

        List<string> rounds = new();
        await foreach (var round in webService.GetRoundsForDate(postcode, houseNumber, DateOnly.FromDateTime(DateTime.Now), cancellationToken))
        {
            rounds.Add(round);
        }

        return Ok(rounds);
    }

    [HttpGet]
    [Route("{postcode}/{houseNumber}/details")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public IActionResult GetFullDetailsForThisWeek([FromRoute] string postcode, [FromRoute] int houseNumber)
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
