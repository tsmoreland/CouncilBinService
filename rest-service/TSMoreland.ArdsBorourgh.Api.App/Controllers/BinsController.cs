﻿using System;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TSMoreland.ArdsBorough.Api.DataTransferObjects.Response;
using BinsDomain = TSMoreland.ArdsBorough.Bins.Shared;
using DTO = TSMoreland.ArdsBorough.Api.DataTransferObjects;

namespace TSMoreland.ArdsBorough.Api.App.Controllers;

[Route("api/v{version:apiVersion}/bins")]
[ApiVersion("1")]
[ApiController]
public class BinsController : ControllerBase
{
    private readonly BinsDomain.IBinCollectionService _binCollectionService;
    private readonly IMapper _mapper;
    private readonly ILogger<BinsController> _logger;

    public BinsController(BinsDomain.IBinCollectionService binCollectionService, IMapper mapper, ILogger<BinsController> logger)
    {
        _binCollectionService = binCollectionService ?? throw new ArgumentNullException(nameof(binCollectionService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [Route("{postcode}/{houseNumber:int}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetAllUpcoming([FromRoute] string postcode, [FromRoute] int houseNumber, CancellationToken cancellationToken)
    {
        var collections = await _binCollectionService
            .FindBinCollectionInfoForAddress(houseNumber, new BinsDomain.PostCode(postcode), cancellationToken)
            .Select(pair => _mapper.Map<BinCollectionSummary>(pair))
            .ToListAsync(cancellationToken);

        return Ok(collections);
    }


    [HttpGet]
    [Route("{postcode}/{houseNumber:int}/current")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetThisWeeksType([FromRoute] string postcode, [FromRoute] int houseNumber, CancellationToken cancellationToken)
    {
        var collection = await _binCollectionService
            .FindThisWeeksBinCollectionInfoForAddress(houseNumber, new BinsDomain.PostCode(postcode), cancellationToken)
            .Select(pair => _mapper.Map<BinCollectionSummary>(pair))
            .ToListAsync(cancellationToken);

        return Ok(collection);
    }

    [HttpGet]
    [Route("{postcode}/{houseNumber:int}/next")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetNextWeeksType([FromRoute] string postcode, [FromRoute] int houseNumber, CancellationToken cancellationToken)
    {
        var collection = await _binCollectionService
            .FindNextWeeksBinCollectionInfoForAddress(houseNumber, new BinsDomain.PostCode(postcode), cancellationToken)
            .Select(pair => _mapper.Map<BinCollectionSummary>(pair))
            .ToListAsync(cancellationToken);

        return Ok(collection);
    }

    [HttpGet]
    [Route("{postcode}/{houseNumber}/details")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public IActionResult GetFullDetailsForThisWeek([FromRoute] string postcode, [FromRoute] int houseNumber)
    {
        return Ok("blue");
    }

    [HttpGet]
    [Route("{postcode}/{houseNumber}/{binType}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public IActionResult GetNextDateForType([FromRoute] string postcode, [FromRoute] int houseNumber, [FromRoute] DTO.BinType binType)
    {
        return Ok("blue");
    }


    [HttpGet]
    [Route("{postcode}/{houseNumber}/{binType}/period")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public IActionResult GetCollectionPeriod([FromRoute] string postcode, [FromRoute] int houseNumber, [FromRoute] DTO.BinType binType)
    {
        return Ok("blue");
    }
}