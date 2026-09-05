using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Commands.CancelPrescription;
using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Commands.CreatePrescription;
using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Commands.UpdatePrescription;
using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Queries.GetPrescriptionById;
using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Queries.GetPrescriptions;
using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Queries.LookupPrescription;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authorization;
using PharmacyInventoryDispensingSystem.WebApi.Contracts.ApiResponse;
using PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.Prescriptions;

namespace PharmacyInventoryDispensingSystem.WebApi.Controllers
{
    
    [ApiController]
    [Route("api/v{version:apiVersion}/prescriptions")]
    [ApiVersion("1.0")]
    [Tags("Prescriptions")]
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status500InternalServerError)]
    public sealed class PrescriptionsController(ISender sender) : ApiController
    {
        [HttpGet]
        [Authorize(Policy = Permissions.Prescriptions.Read)]
        [ProducesResponseType<PaginatedList<PrescriptionSummaryDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Gets prescriptions")]
        [EndpointDescription(
       "Returns a paginated list of prescriptions with optional searching, status filtering, and sorting. Doctors can access only their own prescriptions, while administrators can access all prescriptions.")]
        [EndpointName("GetPrescriptionsV1")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetPaged(
            [FromQuery] GetPrescriptionsRequest request,
            CancellationToken cancellationToken)
        {
            var query = new GetPrescriptionsQuery(
                request.SearchTerm,
                request.Status,
                request.SortBy,
                request.IsDescending,
                request.PageNumber,
                request.PageSize);

            var result = await sender.Send(query, cancellationToken);

            return result.Match(
                response => Ok(response),
                Problem);
        }


        [HttpGet("{id:guid}", Name = "GetPrescriptionById")]
        [Authorize(Policy = Permissions.Prescriptions.Read)]
        [ProducesResponseType<PrescriptionDetailsDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
        [EndpointSummary("Gets a prescription by ID")]
        [EndpointDescription(
        "Returns detailed information about the specified prescription if found and the authenticated user is permitted to access it.")]
        [EndpointName("GetPrescriptionByIdV1")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var query = new GetPrescriptionByIdQuery(id);

            var result = await sender.Send(query, cancellationToken);

            return result.Match(
                response => Ok(response),
                Problem);
        }


        [HttpGet("lookup")]
        [Authorize(Policy = Permissions.Prescriptions.Lookup)]
        [ProducesResponseType<LookupPrescriptionResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
        [EndpointSummary("Looks up a prescription")]
        [EndpointDescription(
       "Looks up a prescription using the prescription number and patient document ID for pharmacist review before dispensing.")]
        [EndpointName("LookupPrescriptionV1")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> LookupPrescription(
       [FromQuery] LookupPrescriptionRequest request,
       CancellationToken cancellationToken)
        {
            var query = new LookupPrescriptionQuery(
                request.PrescriptionNumber,
                request.DocumentId);

            var result = await sender.Send(query, cancellationToken);

            return result.Match(
                response => Ok(response),
                Problem);
        }


        [HttpPost]
        [Authorize(Policy = Permissions.Prescriptions.Create)]
        [Consumes("application/json")]
        [ProducesResponseType<CreatePrescriptionResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status409Conflict)]
        [EndpointSummary("Creates a new prescription")]
        [EndpointDescription(
      "Creates a new prescription for a patient with one or more prescribed medicines.")]
        [EndpointName("CreatePrescriptionV1")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> CreatePrescription(
            [FromBody] CreatePrescriptionRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreatePrescriptionCommand(
                request.PatientId,
                request.ValidFrom,
                request.ValidTo,
                request.Notes,
                request.Items
                    .Select(item => new CreatePrescriptionItemCommand(
                        item.MedicineId,
                        item.QuantityPrescribed,
                        item.MaxFillCount,
                        item.DosageInstructions))
                    .ToList());

            var result = await sender.Send(command, cancellationToken);

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


        [HttpPut("{prescriptionId:guid}")]
        [Authorize(Policy = Permissions.Prescriptions.Update)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status409Conflict)]
        [EndpointSummary("Updates a prescription")]
        [EndpointDescription(
    "Updates an active prescription before any dispensing has occurred. " +
    "Doctors can update only their own prescriptions, while administrators can update any prescription.")]
        [EndpointName("UpdatePrescriptionV1")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> Update(
    Guid prescriptionId,
    [FromBody] UpdatePrescriptionRequest request,
    CancellationToken cancellationToken)
        {
            var command = new UpdatePrescriptionCommand(
                prescriptionId,
                request.ValidFrom,
                request.ValidTo,
                request.Notes,
                request.Items
                    .Select(item => new UpdatePrescriptionItemCommand(
                        item.MedicineId,
                        item.QuantityPrescribed,
                        item.MaxFillCount,
                        item.DosageInstructions))
                    .ToList());

            var result = await sender.Send(command, cancellationToken);

            return result.Match(
                _ => NoContent(),
                Problem);
        }


        [HttpPost("{id:guid}/cancel")]
            [Authorize(Policy = Permissions.Prescriptions.Cancel)]
            [ProducesResponseType(StatusCodes.Status204NoContent)]
            [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
            [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
            [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status409Conflict)]
            [EndpointSummary("Cancels a prescription")]
            [EndpointDescription(
           "Cancels an active prescription while preserving the prescription and its historical dispensing records.")]
            [EndpointName("CancelPrescriptionV1")]
            [MapToApiVersion("1.0")]
            public async Task<ActionResult> CancelPrescription(
                Guid id,
                CancellationToken cancellationToken)
            {
                var command = new CancelPrescriptionCommand(id);

                var result = await sender.Send(command, cancellationToken);

                return result.Match(
                    _ => NoContent(),
                    Problem);
            }






        }
}
