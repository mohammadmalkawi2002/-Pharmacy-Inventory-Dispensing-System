using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Queries.GetPatientByDocumentId
{
    public sealed class GetPatientByDocumentIdQueryValidator 
        : AbstractValidator<GetPatientByDocumentIdQuery>
    {
        public GetPatientByDocumentIdQueryValidator()
        {
            RuleFor(query => query.DocumentId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Document ID is required.")
                .Matches(@"^[12][0-9]{9}$")
                .WithMessage("Document ID must consist of 10 digits and start with 1 or 2.");
                
        }
    }
}
