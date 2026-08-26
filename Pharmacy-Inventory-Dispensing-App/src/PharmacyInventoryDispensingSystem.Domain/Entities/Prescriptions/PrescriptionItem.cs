using PharmacyInventoryDispensingSystem.Domain.Common;
using PharmacyInventoryDispensingSystem.Domain.Entities.Dispenses;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;
using System.ComponentModel.DataAnnotations;

namespace PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;

public class PrescriptionItem:AuditableEntity
{

    public Guid PrescriptionId { get; set; }
    public Prescription Prescription { get; set; } = null!;
    public Guid MedicineId { get; set; }
    public Medicine Medicine { get; set; } = null!;

    public int QuantityPrescribed { get; set; }  

    public int QuantityDispensed { get; set; }

    public int MaxRefill { get; set; } //2

    public int RefillUsed { get; set; }//1 refill

    public string? DosageInstructions { get; set; }// 1 2 6 == 12 pice ==> number of boxes 

   

    public ICollection<DispenseItem> DispenseItems { get; set; } = [];

    public int RemainingQuantity => QuantityPrescribed - QuantityDispensed; 

    public bool CanDispense(int requestedQuantity) =>
        requestedQuantity > 0 && requestedQuantity <= RemainingQuantity;
}
