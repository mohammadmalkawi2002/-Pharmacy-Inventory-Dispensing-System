using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyInventoryDispensingSystem.Application.Features.Dashboard.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.Dashboard.Queries;
using PharmacyInventoryDispensingSystem.WebApi.Contracts.ApiResponse;

namespace PharmacyInventoryDispensingSystem.WebApi.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/dashboard")]
    [ApiVersion("1.0")]
    [Tags("Dashboard")]
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType<ApiErrorResponse>(
    StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiErrorResponse>(
    StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiErrorResponse>(
    StatusCodes.Status500InternalServerError)]
    public sealed class DashboardController(ISender sender) : ApiController
    {
        [HttpGet("summary")]
        [ProducesResponseType<DashboardSummaryDto>(
            StatusCodes.Status200OK)]
        [EndpointSummary("Gets dashboard statistics")]
        [EndpointDescription(
            "Returns dashboard statistics permitted for the authenticated user.")]
        [EndpointName("GetDashboardSummaryV1")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetSummary(
            CancellationToken cancellationToken)
        {
            var query = new GetDashboardSummaryQuery();

            var result = await sender.Send(
                query,
                cancellationToken);

            return result.Match(
                response => Ok(response),
                Problem);
        }
    }
}
