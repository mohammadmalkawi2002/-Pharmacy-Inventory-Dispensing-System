using MediatR;
using Microsoft.EntityFrameworkCore;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Patients;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Queries.LookupPatients
{
    public sealed class LookupPatientsQueryHandler(
     IGenericRepository<Patient> patientRepository)
     : IRequestHandler<
         LookupPatientsQuery,
         Result<List<PatientLookupDto>>>
    {
        // you can change it when You need 
        private const int LookupLimit = 20;

        public async Task<Result<List<PatientLookupDto>>> Handle(
            LookupPatientsQuery request,
            CancellationToken cancellationToken)
        {

            string normalizedTerm = request.SearchTerm.Trim();

            var patients= await patientRepository.Query()
                           .Where(p=>p.FullName.Contains(normalizedTerm) || 
                            p.DocumentId.StartsWith(normalizedTerm))
                           .OrderBy(p=>p.FullName)
                           .ThenBy(p=>p.Id)
                           .Take(LookupLimit)
                           .Select(p=>new PatientLookupDto (p.Id,p.DocumentId,p.FullName))
                           .ToListAsync(cancellationToken);

            return patients;

        }
    }
}
