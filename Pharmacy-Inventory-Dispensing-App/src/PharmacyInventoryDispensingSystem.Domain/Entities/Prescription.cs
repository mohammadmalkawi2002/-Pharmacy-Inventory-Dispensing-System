using PharmacyInventoryDispensingSystem.Domain.Common;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace PharmacyInventoryDispensingSystem.Domain.Entities;

public class Prescription : SoftDeletableEntity
{
    


    public string PrescriptionNumber { get; set; } = string.Empty;

    public string PatientName { get; set; } = string.Empty;

    public string? PatientPhone { get; set; }

    public string DoctorId { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime ValidTo { get; set; }

    public int MaxRefills { get; set; }

    public int RefillsUsed { get; set; }

    public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Active;


    public string? Notes { get; set; }

    public ICollection<PrescriptionItem> Items { get; set; } = [];

    public ICollection<Dispense> Dispenses { get; set; } = [];// 1 prescription can have many dispenses event  (dispense history)

    public bool IsValidOn(DateTime asOf) =>
        Status == PrescriptionStatus.Active && asOf.Date >= ValidFrom.Date && asOf <= ValidTo.Date;

    
    
    public bool HasRefillsRemaining() => RefillsUsed < MaxRefills; // or RefillsUsed < MaxRefills +1
}
