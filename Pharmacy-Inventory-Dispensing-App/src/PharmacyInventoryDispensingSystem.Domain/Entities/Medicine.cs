using PharmacyInventoryDispensingSystem.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace PharmacyInventoryDispensingSystem.Domain.Entities;

public class Medicine : SoftDeletableEntity
{

    // Unique identifier for the medicine 
    public string Code { get; set; } = string.Empty; 

    public string Name { get; set; } = string.Empty;

    // Strength of the medicine, e.g., 500mg, 250mg, etc.
    public string Strength { get; set; } = string.Empty;

    // Tablets, Capsules, Syrup, Injection, Cream, Drops, 

    public string Form { get; set; } = string.Empty;

     public string Unit { get; set; } = string.Empty; // e.g., Box, Vial


    // Minimum stock level before reordering to monitor inventory levels (Alert threshold).
    public int ReorderLevel { get; set; }
    public bool IsActive { get; set; } 


    public ICollection<MedicineBatch> Batches { get; set; } = [];

    public ICollection<PrescriptionItem> PrescriptionItems { get; set; } = [];
}
