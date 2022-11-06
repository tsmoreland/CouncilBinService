//
// Copyright (c) 2022 Terry Moreland
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.Net.Mime;
using System.Runtime.CompilerServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Swashbuckle.AspNetCore.Annotations;
using TSMoreland.ArdsBorough.Api.DataTransferObjects.Response;
using TSMoreland.ArdsBorough.Bins.Collections.Shared;
using DTO = TSMoreland.ArdsBorough.Api.DataTransferObjects;

namespace TSMoreland.ArdsBorough.WebApi.App.Controllers;

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
    public async IAsyncEnumerable<BinCollectionSummary> GetAllUpcoming([FromRoute] string postcode, [FromRoute] int houseNumber, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        IAsyncEnumerable<BinCollectionSummary> collections = _binCollectionService
            .FindBinCollectionInfoForAddress(houseNumber, new PostCode(postcode), cancellationToken)
            .Select(pair => _mapper.Map<BinCollectionSummary>(pair))
            .AsAsyncEnumerable();

        await foreach (BinCollectionSummary summary in collections.WithCancellation(cancellationToken))
        {
            yield return summary;
        }
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
    public async IAsyncEnumerable<BinCollectionSummary> GetThisWeeksType([FromRoute] string postcode, [FromRoute] int houseNumber, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        IAsyncEnumerable<BinCollectionSummary> collections = _binCollectionService
            .FindThisWeeksBinCollectionInfoForAddress(houseNumber, new PostCode(postcode), cancellationToken)
            .Select(pair => _mapper.Map<BinCollectionSummary>(pair))
            .AsAsyncEnumerable();

        await foreach (BinCollectionSummary summary in collections.WithCancellation(cancellationToken))
        {
            yield return summary;
        }
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
    public async IAsyncEnumerable<BinCollectionSummary> GetNextWeeksType([FromRoute] string postcode, [FromRoute] int houseNumber, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        IAsyncEnumerable<BinCollectionSummary> collection = _binCollectionService
            .FindNextWeeksBinCollectionInfoForAddress(houseNumber, new PostCode(postcode), cancellationToken)
            .Select(pair => _mapper.Map<BinCollectionSummary>(pair))
            .AsAsyncEnumerable();

        await foreach (BinCollectionSummary summary in collection.WithCancellation(cancellationToken))
        {
            yield return summary;
        }

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


    /// <inheritdoc/>
    public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
    {
        return base.ValidationProblem(modelStateDictionary);
    }
}
