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
        /// Identifying document number, such as a national ID.
        /// Must be unique, consist of 10 digits, and start with 2.
        /// </summary>
        public string DocumentId { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public ICollection<Prescription> Prescriptions { get; set; }
            = new List<Prescription>();
    }
}
