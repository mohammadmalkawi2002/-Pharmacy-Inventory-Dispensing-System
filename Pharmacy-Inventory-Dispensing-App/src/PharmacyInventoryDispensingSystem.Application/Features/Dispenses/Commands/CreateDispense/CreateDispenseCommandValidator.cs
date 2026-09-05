using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Dispenses.Commands.CreateDispense
{
    public sealed class CreateDispenseCommandValidator
     : AbstractValidator<CreateDispenseCommand>
    {
        public CreateDispenseCommandValidator()
        {
            RuleFor(command => command.PrescriptionId)
                .NotEmpty()
                .WithMessage("Prescription ID is required.");

            RuleFor(command => command.DocumentId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Patient document ID is required.")
                .Matches(@"^[12][0-9]{9}$")
                .WithMessage(
                    "Document ID must contain 10 digits and start with 1 or 2.");

            RuleFor(command => command.PrescriptionItemIds)
                .NotEmpty()
                .WithMessage(
                    "At least one prescription item must be selected.");

            RuleForEach(command => command.PrescriptionItemIds)
                .NotEmpty()
                .WithMessage(
                    "Prescription item ID cannot be empty.");

            RuleFor(command => command.PrescriptionItemIds)
                .Must(itemIds =>
                    itemIds.Distinct().Count() == itemIds.Count)
                .When(command =>
                    command.PrescriptionItemIds is { Count: > 0 })
                .WithMessage(
                    "Duplicate prescription item IDs are not allowed.");

            RuleFor(command => command.Notes)
    .MaximumLength(300);
        }
    }

    }
