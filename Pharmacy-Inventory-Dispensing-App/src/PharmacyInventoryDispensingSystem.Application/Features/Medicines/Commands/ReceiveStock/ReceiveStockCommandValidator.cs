using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Commands.ReceiveStock
{
    public sealed class ReceiveStockCommandValidator:AbstractValidator<ReceiveStockCommand>
    {
        public ReceiveStockCommandValidator()
        {
            RuleFor(command=>command.MedicineId)
                .NotEmpty()
             .WithMessage("Medicine id is required.");

            RuleFor(command => command.PackageQuantity)
               .NotEmpty()
           .GreaterThan(0)
           .WithMessage("Package quantity must be greater than zero.");




        }
    }
}
