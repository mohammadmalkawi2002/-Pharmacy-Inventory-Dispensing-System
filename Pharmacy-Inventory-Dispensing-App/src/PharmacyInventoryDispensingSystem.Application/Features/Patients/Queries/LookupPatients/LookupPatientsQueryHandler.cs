using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Queries.LookupPatients
{
    public sealed class LookupPatientsQueryHandler(
     IPatientRepository patientRepository)
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
            var patients =
                await patientRepository.SearchForLookupAsync(
                    request.SearchTerm,
                    LookupLimit,
                    cancellationToken);

            return patients;

        }
    }
}
