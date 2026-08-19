using PharmacyInventoryDispensingSystem.Domain.Common;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Dispenses;
using PharmacyInventoryDispensingSystem.Domain.Entities.Patients;
using PharmacyInventoryDispensingSystem.Domain.Enums;

namespace PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;

public sealed class Prescription : SoftDeletableEntity
{
    public string PrescriptionNumber { get; set; } = string.Empty;

    public Guid PatientId { get; set; }

    public string DoctorId { get; set; } = string.Empty;

    public DateTime ValidFrom { get; set; }

    public DateTime ValidTo { get; set; }

    public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Active;

    public string? Notes { get; set; }

    public Patient Patient { get; set; } = null!;

    public ICollection<PrescriptionItem> Items { get; set; }
        = new List<PrescriptionItem>();

    public ICollection<Dispense> Dispenses { get; set; }
        = new List<Dispense>();

    /// <summary>
    /// is used later to check whether a specific prescription is valid on a given date by verifying 
    /// that its status is Active and that the date falls within its validity period
    /// </summary>
    /// <param name="asOf"></param>
    /// <returns></returns>
    public bool IsValidOn(DateTime asOf) =>
        Status == PrescriptionStatus.Active && asOf.Date >= ValidFrom.Date && asOf <= ValidTo.Date;

    
    
}
