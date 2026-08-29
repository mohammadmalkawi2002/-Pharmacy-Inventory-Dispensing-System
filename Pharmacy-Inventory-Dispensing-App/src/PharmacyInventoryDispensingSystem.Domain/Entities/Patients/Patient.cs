using PharmacyInventoryDispensingSystem.Domain.Common;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Domain.Entities.Patients
{
    public sealed class Patient : SoftDeletableEntity
    {
        /// <summary>
        /// Saudi national or resident identification number.
        /// Must be unique, consist of exactly 10 digits,
        /// and start with 1 for citizens or 2 for residents.
        /// </summary>
        public string DocumentId { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public ICollection<Prescription> Prescriptions { get; set; }
            = new List<Prescription>();
    }
}
