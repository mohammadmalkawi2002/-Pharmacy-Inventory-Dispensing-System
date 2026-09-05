using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Commands.ArchivePatient;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Commands.CreatePatient;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Commands.RestorePatient;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Commands.UpdatePatient;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Dtos;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Queries.GetArchivedPatients;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Queries.GetPatientByDocumentId;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Queries.GetPatientById;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Queries.GetPatients;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Queries.LookupPatients;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authorization;
using PharmacyInventoryDispensingSystem.Domain.Entities.Patients;
using PharmacyInventoryDispensingSystem.WebApi.Contracts.ApiResponse;
using PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.Patient;
using System.Numerics;

namespace PharmacyInventoryDispensingSystem.WebApi.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/patients")]
    [ApiVersion("1.0")]
    [Tags("Patients")]
    [Authorize] // user must authenticatd to access all endpoints:
    [Produces("application/json")]
    [ProducesResponseType<ApiErrorResponse>(
    StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiErrorResponse>(
    StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiErrorResponse>(
    StatusCodes.Status500InternalServerError)]
    public sealed class PatientsController(ISender sender) : ApiController
    {
        [HttpGet]
        [Authorize(Policy = Permissions.Patients.Read)]
        [ProducesResponseType<PaginatedList<PatientResponseDto>>(
        StatusCodes.Status200OK)]
        [ProducesResponseType<ApiErrorResponse>(
        StatusCodes.Status400BadRequest)]
        [EndpointSummary("Gets active patients")]
        [EndpointDescription("Returns a paginated list of active patients with optional searching, document type filtering, and sorting.")]
        [EndpointName("GetPatientsV1")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetPaged(
            [FromQuery] GetPatientsRequest request,
            CancellationToken cancellationToken) 
        {
            var query = new GetPatientsQuery(
                request.SearchTerm,
                request.DocumentType,
                request.SortBy,
                request.IsDescending,
                request.PageNumber,
                request.PageSize);

            var result = await sender.Send(
                query,
                cancellationToken);


            return result.Match(response => 
                Ok(response),
                Problem);
        
        }



        [HttpGet("archived")]
        [Authorize(Policy =PolicyNames.AdminOnly)]
        [ProducesResponseType<PaginatedList<PatientResponseDto>>(
        StatusCodes.Status200OK)]
        [ProducesResponseType<ApiErrorResponse>(
        StatusCodes.Status400BadRequest)]
        [EndpointName("GetArchivedPatientsV1")]
        [EndpointSummary("Gets archived patients")]
        [EndpointDescription(
        "Returns a paginated list of archived patients for administrative review and restoration.")]
        [MapToApiVersion("1.0")]

        public async Task<IActionResult> GetArchivedPatients(
        [FromQuery] GetPatientsRequest request,
        CancellationToken cancellationToken)
        {

            var query = new GetArchivedPatientsQuery(
             request.SearchTerm,
             request.DocumentType,
             request.SortBy,
             request.IsDescending,
             request.PageNumber,
             request.PageSize);

            var result = await sender.Send(
                query,
                cancellationToken);

            return result.Match(
                response => Ok(response),
                Problem);
        }




        [HttpGet("{patientId:guid}",Name ="GetPatientById")]
        [Authorize(Policy = Permissions.Patients.Read)]
        [ProducesResponseType<PatientResponseDto>(
       StatusCodes.Status200OK)]
        [ProducesResponseType<ApiErrorResponse>(
       StatusCodes.Status404NotFound)]
        [EndpointName("GetPatientByIdV1")]
        [EndpointSummary("Gets a patient by ID")]
        [MapToApiVersion("1.0")]
        [EndpointDescription(
       "Returns detailed information about  the specified patient if found.")]
        public async Task<ActionResult> GetById(
       Guid patientId,
       CancellationToken cancellationToken)
        {
            var query = new GetPatientByIdQuery(patientId);

            var result = await sender.Send(
                query,
                cancellationToken);

            return result.Match(
                response => Ok(response),
                Problem);
        }

        [HttpGet("lookup")]
        [Authorize(Policy = Permissions.Patients.Read)]
        [EndpointSummary("Search patients for selection")]
        [EndpointDescription(
    "Searches non-archived patients by full name or document ID for use in patient selection controls, such as the prescription creation form. Returns a limited set of matching patients.")]
        [EndpointName("LookupPatients")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType<List<PatientResponseDto>>(
       StatusCodes.Status200OK)]
        [ProducesResponseType<ApiErrorResponse>(
        StatusCodes.Status400BadRequest)]
       
        public async Task<IActionResult> Lookup(
    [FromQuery] string searchTerm,
    CancellationToken cancellationToken)
        {
            var query = new LookupPatientsQuery(searchTerm);

            var result = await sender.Send(query, cancellationToken);

            return result.Match(
                Ok,
                Problem);
        }






        [HttpPost]
        [Authorize(Policy = PolicyNames.ReceptionistOrAdmin)]
        [Consumes("application/json")]
        [ProducesResponseType<PatientResponseDto>(
        StatusCodes.Status201Created)]
        [ProducesResponseType<ApiErrorResponse>(
        StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApiErrorResponse>(
        StatusCodes.Status409Conflict)]
        [EndpointName("CreatePatientV1")]
        [EndpointSummary("Creates a new patient")]
        [MapToApiVersion("1.0")]
        [EndpointDescription(
        "Add a new patient to the system.")]

        public async Task<IActionResult> CreatePatient([FromBody] CreatePatientRequest request,CancellationToken cancellationToken ) 
        {
            var command = new CreatePatientCommand(
                request.DocumentId,
                request.FullName,
                request.DateOfBirth,
                request.PhoneNumber);

            var result = await sender.Send(
                command,
                cancellationToken);


            return result.Match(response => CreatedAtAction(
                nameof(GetById),
                new
                {
                    version = "1",
                    patientId = response.Id
                },
                         response),
                       Problem);
        }


        [HttpGet("by-document/{documentId}")]
        [Authorize(Policy = Permissions.Patients.Read)]
        [ProducesResponseType<PatientResponseDto>(
        StatusCodes.Status200OK)]
        [ProducesResponseType<ApiErrorResponse>(
        StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApiErrorResponse>(
        StatusCodes.Status404NotFound)]
        [EndpointName("GetPatientByDocumentIdV1")]
        [EndpointSummary("Gets a patient by document ID")]
        [MapToApiVersion("1.0")]
        [EndpointDescription(
        "Returns detailed information about the specified patient using a Saudi citizen or resident document ID if found.")]
        public async Task<ActionResult> GetPatientByDocumentId(
        string documentId,
        CancellationToken cancellationToken)
        {
            var query =
                new GetPatientByDocumentIdQuery(documentId);

            var result = await sender.Send(
                query,
                cancellationToken);

            return result.Match<ActionResult>(
                response => Ok(response),
                Problem);
        }


        [HttpPut("{patientId:guid}")]
        [Authorize(Policy = PolicyNames.ReceptionistOrAdmin)]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ApiErrorResponse>(
     StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ApiErrorResponse>(
     StatusCodes.Status404NotFound)]
        [ProducesResponseType<ApiErrorResponse>(
     StatusCodes.Status409Conflict)]
        [EndpointName("UpdatePatientV1")]
        [EndpointSummary("Updates an existing patient")]
        [EndpointDescription(
     "Updates the document ID, full name, date of birth, and phone number of an active patient.")]
        public async Task<ActionResult> UpdatePatient(
            Guid patientId,
            [FromBody] UpdatePatientRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdatePatientCommand(
                patientId,
                request.DocumentId,
                request.FullName,
                request.DateOfBirth,
                request.PhoneNumber);

            var result = await sender.Send(
                command,
                cancellationToken);

            return result.Match(
                _ => NoContent(),
                Problem);
        }



        //   Patient Lookup userd in frontend to create prescription
        //-search by FullName
        //-search by DocumentId
        //-AsNoTracking
        //- Take(20)
        //- لا Doctor ownership
        //-archived automatically excluded


        [HttpDelete("{patientId:guid}")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ApiErrorResponse>(
        StatusCodes.Status404NotFound)]
        [ProducesResponseType<ApiErrorResponse>(
        StatusCodes.Status409Conflict)]
        [MapToApiVersion("1.0")]
        [EndpointName("ArchivePatientV1")]
        [EndpointSummary("Archives a patient")]
        [EndpointDescription(
        "Archives a patient without deleting their prescriptions or dispensing history.")]
        public async Task<ActionResult> ArchivePatient(
            Guid patientId,
            CancellationToken cancellationToken)
        {
            var command = new ArchivePatientCommand(patientId);

            var result=await sender.Send(command, cancellationToken);

            return result.Match(
                _ => NoContent(),
                Problem);
        }




        [HttpPost("{patientId:guid}/restore")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ApiErrorResponse>(
       StatusCodes.Status404NotFound)]
        [ProducesResponseType<ApiErrorResponse>(
       StatusCodes.Status409Conflict)]
        [EndpointName("RestorePatientV1")]
        [MapToApiVersion("1.0")]
        [EndpointSummary("Restores an archived patient")]
        [EndpointDescription(
       "Restores a previously archived patient.")]
        public async Task<ActionResult> RestorePatient(
       Guid patientId,
       CancellationToken cancellationToken)
        {
            var command =
                new RestorePatientCommand(patientId);

            var result = await sender.Send(
                command,
                cancellationToken);

            return result.Match(
                _ => NoContent(),
                Problem);
        }





    }
}
