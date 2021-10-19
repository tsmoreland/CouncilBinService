using System.Collections.Generic;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TSMoreland.ArdsBorough.Api.DataTransferObjects.Response;
using TSMoreland.ArdsBorough.Bins.Collections.Shared;

namespace TSMoreland.ArdsBorough.WebApi.App.Controllers
{
    /// <summary>
    /// Controller hosting endpoints to look up URPN codes for addresses
    /// </summary>
    [Route("api/v{version:apiVersion}/address")]
    [ApiController]
    public sealed class AddressController : Controller
    {

        /// <summary>
        /// Returns URPN for address given by <paramref name="postcode"/> and <paramref name="houseNumber"/>
        /// </summary>
        /// <param name="postcode">postcode for the address to look up</param>
        /// <param name="houseNumber">house number for the address to look up</param>
        /// <returns>URPN number used to look up bin collection dates</returns>
        [HttpGet]
        [Route("{postcode}/{address}")]
        [Produces(typeof(string))]
        [Consumes(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        [SwaggerResponse(StatusCodes.Status200OK, "Successful response.", typeof(List<BinCollectionSummary>), MediaTypeNames.Application.Json)] 
        [SwaggerResponse(StatusCodes.Status404NotFound, "Address not found.", typeof(ProblemDetails), MediaTypeNames.Application.Json)] 
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid argument.", typeof(ProblemDetails), MediaTypeNames.Application.Json)] 
        public IActionResult GetURPNFromPostCodeAndHouseNumber([FromRoute] string postcode, [FromRoute] int houseNumber)
        {
            var postcodeValue = PostCode.ConvertOrThrow(postcode);


            return Ok();
        }
    }
}
