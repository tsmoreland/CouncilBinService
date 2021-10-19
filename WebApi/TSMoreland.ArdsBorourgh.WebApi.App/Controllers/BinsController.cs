using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using TSMoreland.ArdsBorough.Api.DataTransferObjects.Response;
using TSMoreland.ArdsBorough.Bins.Collections.Shared;
using DTO = TSMoreland.ArdsBorough.Api.DataTransferObjects;

namespace TSMoreland.ArdsBorough.Api.App.Controllers;

/// <summary/>
[Route("api/v{version:apiVersion}/bins")]
[ApiController]
public class BinsController : ControllerBase
{
    private readonly IBinCollectionService _binCollectionService;
    private readonly IMapper _mapper;
    private readonly ILogger<BinsController> _logger;

    /// <summary/>
    public BinsController(IBinCollectionService binCollectionService, IMapper mapper, ILogger<BinsController> logger)
    {
        _binCollectionService = binCollectionService ?? throw new ArgumentNullException(nameof(binCollectionService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get a summary of all upcoming bin collection details
    /// </summary>
    /// <param name="postcode" example="SW1A 1AA" >postcode of the address to find details of</param>
    /// <param name="houseNumber" example="1" >house number of the address to find details of</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>a collection of <see cref="BinCollectionSummary"/></returns>
    [HttpGet]
    [Route("{postcode}/{houseNumber:int}")]
    [ApiVersion("1")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerResponse(StatusCodes.Status200OK, "Successful response.", typeof(List<BinCollectionSummary>), MediaTypeNames.Application.Json)] 
    [SwaggerResponse(StatusCodes.Status404NotFound, "Address not found.", typeof(ProblemDetails), MediaTypeNames.Application.Json)] 
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid argument.", typeof(ProblemDetails), MediaTypeNames.Application.Json)] 
    public async Task<IActionResult> GetAllUpcoming([FromRoute] string postcode, [FromRoute] int houseNumber, CancellationToken cancellationToken)
    {
        var collections = await _binCollectionService
            .FindBinCollectionInfoForAddress(houseNumber, new PostCode(postcode), cancellationToken)
            .Select(pair => _mapper.Map<BinCollectionSummary>(pair))
            .ToListAsync(cancellationToken);

        return Ok(collections);
    }

    /// <summary>
    /// Returns the bin(s) collected during this week, will be empty if after the collection day
    /// </summary>
    /// <param name="postcode"></param>
    /// <param name="houseNumber"></param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>a collection of <see cref="BinCollectionSummary"/></returns>
    [HttpGet]
    [Route("{postcode}/{houseNumber:int}/current")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ApiVersion("1")]
    [SwaggerResponse(StatusCodes.Status200OK, "Successful response.", typeof(List<BinCollectionSummary>), MediaTypeNames.Application.Json)] 
    [SwaggerResponse(StatusCodes.Status404NotFound, "Address not found.", typeof(ProblemDetails), MediaTypeNames.Application.Json)] 
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid argument.", typeof(ProblemDetails), MediaTypeNames.Application.Json)] 
    public async Task<IActionResult> GetThisWeeksType([FromRoute] string postcode, [FromRoute] int houseNumber, CancellationToken cancellationToken)
    {
        var collection = await _binCollectionService
            .FindThisWeeksBinCollectionInfoForAddress(houseNumber, new PostCode(postcode), cancellationToken)
            .Select(pair => _mapper.Map<BinCollectionSummary>(pair))
            .ToListAsync(cancellationToken);

        return Ok(collection);
    }

    /// <summary>
    /// Returns the bin(s) collected during the next week, will be empty if after the collection day
    /// </summary>
    /// <param name="postcode"></param>
    /// <param name="houseNumber"></param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>a collection of <see cref="BinCollectionSummary"/></returns>
    [HttpGet]
    [Route("{postcode}/{houseNumber:int}/next")]
    [ApiVersion("1")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerResponse(StatusCodes.Status200OK, "Successful response.", typeof(List<BinCollectionSummary>), MediaTypeNames.Application.Json)] 
    [SwaggerResponse(StatusCodes.Status404NotFound, "Address not found.", typeof(ProblemDetails), MediaTypeNames.Application.Json)] 
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid argument.", typeof(ProblemDetails), MediaTypeNames.Application.Json)] 
    public async Task<IActionResult> GetNextWeeksType([FromRoute] string postcode, [FromRoute] int houseNumber, CancellationToken cancellationToken)
    {
        var collection = await _binCollectionService
            .FindNextWeeksBinCollectionInfoForAddress(houseNumber, new PostCode(postcode), cancellationToken)
            .Select(pair => _mapper.Map<BinCollectionSummary>(pair))
            .ToListAsync(cancellationToken);

        return Ok(collection);
    }

    /// <summary>
    /// Returns the full details of bin(s) collected during this week, will be empty if after the collection day
    /// </summary>
    /// <param name="postcode"></param>
    /// <param name="houseNumber"></param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>a collection of <see cref="BinCollectionSummary"/></returns>
    [HttpGet]
    [Route("{postcode}/{houseNumber}/details")]
    [ApiVersion("0")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerResponse(StatusCodes.Status200OK, "Successful response.", typeof(List<BinCollectionSummary>), MediaTypeNames.Application.Json)] 
    [SwaggerResponse(StatusCodes.Status404NotFound, "Address not found.", typeof(ProblemDetails), MediaTypeNames.Application.Json)] 
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid argument.", typeof(ProblemDetails), MediaTypeNames.Application.Json)] 
    public IActionResult GetFullDetailsForThisWeek([FromRoute] string postcode, [FromRoute] int houseNumber, CancellationToken cancellationToken)
    {
        return Ok("blue");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="postcode"></param>
    /// <param name="houseNumber"></param>
    /// <param name="binType"></param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>a collection of <see cref="BinCollectionSummary"/></returns>
    [HttpGet]
    [Route("{postcode}/{houseNumber}/{binType}")]
    [ApiVersion("0")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Address not found.", typeof(ProblemDetails), MediaTypeNames.Application.Json)] 
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid argument.", typeof(ProblemDetails), MediaTypeNames.Application.Json)] 
    public IActionResult GetNextDateForType([FromRoute] string postcode, [FromRoute] int houseNumber, [FromRoute] DTO.BinType binType, CancellationToken cancellationToken)
    {
        return Ok("blue");
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="postcode"></param>
    /// <param name="houseNumber"></param>
    /// <param name="binType"></param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>a collection of <see cref="BinCollectionSummary"/></returns>
    [HttpGet]
    [Route("{postcode}/{houseNumber}/{binType}/period")]
    [ApiVersion("0")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Address not found.", typeof(ProblemDetails), MediaTypeNames.Application.Json)] 
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid argument.", typeof(ProblemDetails), MediaTypeNames.Application.Json)] 
    public IActionResult GetCollectionPeriod([FromRoute] string postcode, [FromRoute] int houseNumber, [FromRoute] DTO.BinType binType, CancellationToken cancellationToken)
    {
        return Ok("blue");
    }
}
