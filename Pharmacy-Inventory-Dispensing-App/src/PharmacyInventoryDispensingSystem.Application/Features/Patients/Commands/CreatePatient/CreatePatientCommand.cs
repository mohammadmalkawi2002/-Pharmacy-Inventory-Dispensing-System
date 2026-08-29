using MediatR;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Commands.CreatePatient
{
    public sealed record CreatePatientCommand(
        string DocumentId,
        string FullName,
        DateTime DateOfBirth,
        string PhoneNumber)
        :IRequest<Result<PatientResponseDto>>;
    
    
}
