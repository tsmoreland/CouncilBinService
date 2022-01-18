//
// Copyright © 2022 Terry Moreland
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
