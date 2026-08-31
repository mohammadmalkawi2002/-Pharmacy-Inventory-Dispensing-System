using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Commands.ActivateMedicine;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Commands.ArchiveMedicine;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Commands.CreateMedicine;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Commands.DeactivateMedicine;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Commands.ReceiveStock;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Commands.RestoreMedicine;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Commands.UpdateMedicine;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.GetArchivedMedicines;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.GetLowStockMedicines;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.GetMedicineByCode;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.GetMedicineById;
using PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.GetMedicines;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authorization;
using PharmacyInventoryDispensingSystem.WebApi.Contracts.ApiResponse;
using PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.Medicine;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PharmacyInventoryDispensingSystem.WebApi.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/medicines")]
    [ApiVersion("1.0")]
    [Tags("Medicines")]
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status500InternalServerError)]
    public sealed class MedicinesController(ISender sender) : ApiController
    {
        [HttpGet]
        [Authorize(Policy = Permissions.Medicines.Read)]
        [ProducesResponseType<PaginatedList<MedicineResponseDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Gets medicines")]
        [EndpointDescription(
    "Returns a paginated list of medicines with optional searching, form filtering, stock unit filtering, active status filtering, and sorting.")]
        [EndpointName("GetMedicinesV1")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetPaged(
            [FromQuery] GetMedicinesRequest request,
            CancellationToken cancellationToken)
        {
            var query = new GetMedicinesQuery(
                request.SearchTerm,
                request.Form,
                request.StockUnit,
                request.IsActive,
                request.SortBy,
                request.IsDescending,
                request.PageNumber,
                request.PageSize);

            var result = await sender.Send(query, cancellationToken);

            return result.Match(
                response => Ok(response),
                Problem);
        }

        [HttpGet("low-stock")]
        [Authorize(Policy = Permissions.Medicines.ReadLowStock)]
        [ProducesResponseType<PaginatedList<MedicineResponseDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Gets low stock medicines")]
        [EndpointDescription("Returns a paginated list of medicines where quantity in stock is greater than 0 and less than or equal to the reorder level.")]
        [EndpointName("GetLowStockMedicinesV1")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetLowStockMedicines(
            [FromQuery] GetLowStockMedicinesRequest request,
            CancellationToken cancellationToken)
        {
            var query = new GetLowStockMedicinesQuery(
                request.SearchTerm,
                request.IsActive,
                request.PageNumber,
                request.PageSize);
               

            var result = await sender.Send(query, cancellationToken);

            return result.Match(
                response => Ok(response),
                Problem);
        }

        [HttpGet("archived")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        [ProducesResponseType<PaginatedList<MedicineResponseDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Gets archived medicines")]
        [EndpointDescription("Returns a paginated list of archived medicines for administrative review and restoration.")]
        [EndpointName("GetArchivedMedicinesV1")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetArchivedMedicines(
            [FromQuery] GetArchivedMedicinesRequest request,
            CancellationToken cancellationToken)
        {
            var query = new GetArchivedMedicinesQuery(
                request.SearchTerm,
                request.PageNumber,
                request.PageSize);

            var result = await sender.Send(query, cancellationToken);

            return result.Match(
                response => Ok(response),
                Problem);
        }

        [HttpGet("by-code/{code}")]
        [Authorize(Policy = Permissions.Medicines.Read)]
        [ProducesResponseType<MedicineDetailsResponseDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
        [EndpointSummary("Gets a medicine by code")]
        [EndpointDescription("Returns detailed information about the specified medicine using its barcode/code if found.")]
        [EndpointName("GetMedicineByCodeV1")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetMedicineByCode(
            string code,
            CancellationToken cancellationToken)
        {
            var query = new GetMedicineByCodeQuery(code);

            var result = await sender.Send(query, cancellationToken);

            return result.Match<ActionResult>(
                response => Ok(response),
                Problem);
        }

        [HttpGet("{id:guid}", Name = "GetMedicineById")]
        [Authorize(Policy = Permissions.Medicines.Read)]
        [ProducesResponseType<MedicineDetailsResponseDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
        [EndpointSummary("Gets a medicine by ID")]
        [EndpointDescription("Returns detailed information about the specified medicine if found.")]
        [EndpointName("GetMedicineByIdV1")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var query = new GetMedicineByIdQuery(id);

            var result = await sender.Send(query, cancellationToken);

            return result.Match(
                response => Ok(response),
                Problem);
        }

        [HttpPost]
        [Authorize(Policy = Permissions.Medicines.Create)]
        [Consumes("application/json")]
        [ProducesResponseType<MedicineDetailsResponseDto>(StatusCodes.Status201Created)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status409Conflict)]
        [EndpointSummary("Creates a new medicine")]
        [EndpointDescription("Adds a new medicine to the inventory catalog.")]
        [EndpointName("CreateMedicineV1")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> CreateMedicine(
            [FromBody] CreateMedicineRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateMedicineCommand(
                request.Code,
                request.Name,
                request.Strength,
                request.Form,
                request.StockUnit,
                request.PackageUnit,
                request.UnitsPerPackage,
                request.ReorderLevel);

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


        [HttpPost("{id:guid}/stock/receive")]
        [Authorize(Policy = Permissions.Medicines.Update)]
        [Consumes("application/json")]
        [ProducesResponseType<ReceiveStockResponseDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status409Conflict)]
        [EndpointSummary("Receives medicine stock")]
        [EndpointDescription(
    "Receives medicine packages and increases stock using the medicine's configured units per package.")]
        [EndpointName("ReceiveMedicineStockV1")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> ReceiveStock(
    Guid id,
    [FromBody] ReceiveStockRequest request,
    CancellationToken cancellationToken)
        {
            var command = new ReceiveStockCommand(
                id,
                request.PackageQuantity);

            var result = await sender.Send(command, cancellationToken);

            return result.Match(
                response => Ok(response),
                Problem);
        }











        [HttpPut("{id:guid}")]
        [Authorize(Policy = Permissions.Medicines.Update)]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status409Conflict)]
        [EndpointSummary("Updates an existing medicine")]
        [EndpointDescription(
    "Updates medicine details and stock configuration. Stock quantity cannot be updated through this endpoint.")]
        [EndpointName("UpdateMedicineV1")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> UpdateMedicine(
            Guid id,
            [FromBody] UpdateMedicineRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateMedicineCommand(
                id,
                request.Code,
                request.Name,
                request.Strength,
                request.Form,
                request.StockUnit,
                request.PackageUnit,
                request.UnitsPerPackage,
                request.ReorderLevel);

            var result = await sender.Send(command, cancellationToken);

            return result.Match(
                _ => NoContent(),
                Problem);
        }

        [HttpPost("{id:guid}/activate")]
        [Authorize(Policy = Permissions.Medicines.Activate)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status409Conflict)]
        [EndpointSummary("Activates a medicine")]
        [EndpointDescription("Activates an inactive medicine.")]
        [EndpointName("ActivateMedicineV1")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> ActivateMedicine(
            Guid id,
            CancellationToken cancellationToken)
        {
            var command = new ActivateMedicineCommand(id);

            var result = await sender.Send(command, cancellationToken);

            return result.Match(
                _ => NoContent(),
                Problem);
        }

        [HttpPost("{id:guid}/deactivate")]
        [Authorize(Policy = Permissions.Medicines.Deactivate)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status409Conflict)]
        [EndpointSummary("Deactivates a medicine")]
        [EndpointDescription("Deactivates an active medicine.")]
        [EndpointName("DeactivateMedicineV1")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> DeactivateMedicine(
            Guid id,
            CancellationToken cancellationToken)
        {
            var command = new DeactivateMedicineCommand(id);

            var result = await sender.Send(command, cancellationToken);

            return result.Match(
                _ => NoContent(),
                Problem);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status409Conflict)]
        [EndpointSummary("Archives a medicine")]
        [EndpointDescription("Archives a medicine without deleting historical records.")]
        [EndpointName("ArchiveMedicineV1")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> ArchiveMedicine(
            Guid id,
            CancellationToken cancellationToken)
        {
            var command = new ArchiveMedicineCommand(id);

            var result = await sender.Send(command, cancellationToken);

            return result.Match(
                _ => NoContent(),
                Problem);
        }

        [HttpPost("{id:guid}/restore")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status409Conflict)]
        [EndpointSummary("Restores an archived medicine")]
        [EndpointDescription("Restores a previously archived medicine.")]
        [EndpointName("RestoreMedicineV1")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> RestoreMedicine(
            Guid id,
            CancellationToken cancellationToken)
        {
            var command = new RestoreMedicineCommand(id);

            var result = await sender.Send(command, cancellationToken);

            return result.Match(
                _ => NoContent(),
                Problem);
        }
    }
}
