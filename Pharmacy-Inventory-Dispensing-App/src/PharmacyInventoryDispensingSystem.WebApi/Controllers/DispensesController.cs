using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.Dispenses.Commands.CreateDispense;
using PharmacyInventoryDispensingSystem.Application.Features.Dispenses.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.Dispenses.Queries.GetDispenseById;
using PharmacyInventoryDispensingSystem.Application.Features.Dispenses.Queries.GetDispenses;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authorization;
using PharmacyInventoryDispensingSystem.WebApi.Contracts.ApiResponse;
using PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.Dispense;

namespace PharmacyInventoryDispensingSystem.WebApi.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/dispenses")]
    [ApiVersion("1.0")]
    [Tags("Dispenses")]
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType<ApiErrorResponse>(
    StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiErrorResponse>(
    StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiErrorResponse>(
    StatusCodes.Status500InternalServerError)]
    public sealed class DispensesController(ISender sender) : ApiController
    {
        [HttpGet]
        [Authorize(Policy = Permissions.Dispenses.Read)]
        [ProducesResponseType<PaginatedList<DispenseResponseDto>>(
            StatusCodes.Status200OK)]
        [ProducesResponseType<ApiErrorResponse>(
            StatusCodes.Status400BadRequest)]
        [EndpointSummary("Gets dispensing history")]
        [EndpointDescription(
            "Returns a paginated list of dispensing records with optional search and date filtering.")]
        [EndpointName("GetDispensesV1")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetPaged(
            [FromQuery] GetDispensesRequest request,
            CancellationToken cancellationToken)
        {
            var query = new GetDispensesQuery(
                request.SearchTerm,
                request.FromDate,
                request.ToDate,
                request.PageNumber,
                request.PageSize);

            var result = await sender.Send(
                query,
                cancellationToken);

            return result.Match(
                response => Ok(response),
                Problem);
        }

        [HttpGet("{id:guid}", Name = "GetDispenseById")]
        [Authorize(Policy = Permissions.Dispenses.Read)]
        [ProducesResponseType<DispenseDetailsDto>(
            StatusCodes.Status200OK)]
        [ProducesResponseType<ApiErrorResponse>(
            StatusCodes.Status404NotFound)]
        [EndpointSummary("Gets a dispensing record by ID")]
        [EndpointDescription(
            "Returns the dispensing record with its prescription, patient, and dispensed medicine details.")]
        [EndpointName("GetDispenseByIdV1")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var query = new GetDispenseByIdQuery(id);

            var result = await sender.Send(
                query,
                cancellationToken);

            return result.Match(
                response => Ok(response),
                Problem);
        }

        [HttpPost]
        [Authorize(Policy = Permissions.Dispenses.Create)]
        [Consumes("application/json")]
        [ProducesResponseType<DispenseDetailsDto>(
            StatusCodes.Status201Created)]
        [ProducesResponseType<ApiErrorResponse>(
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApiErrorResponse>(
            StatusCodes.Status404NotFound)]
        [ProducesResponseType<ApiErrorResponse>(
            StatusCodes.Status409Conflict)]
        [EndpointSummary("Dispenses prescription items")]
        [EndpointDescription(
            "Dispenses the complete prescribed quantity for each selected prescription item.")]
        [EndpointName("CreateDispenseV1")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> Create(
            [FromBody] CreateDispenseRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateDispenseCommand(
                request.PrescriptionId,
                request.DocumentId,
                request.PrescriptionItemIds,
                request.Notes);

            var result = await sender.Send(
                command,
                cancellationToken);

            return result.Match(
                response => CreatedAtAction(
                    nameof(GetById),
                    new
                    {
                        version = "1",
                        id = response.Id
                    },
                    response),
                Problem);
        }
    }
}
