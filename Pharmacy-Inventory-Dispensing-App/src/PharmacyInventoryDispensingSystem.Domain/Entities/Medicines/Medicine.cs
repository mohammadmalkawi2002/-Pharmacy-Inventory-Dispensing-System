using PharmacyInventoryDispensingSystem.Domain.Common;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Batches;
using PharmacyInventoryDispensingSystem.Domain.Entities.Medicines.Event;
using PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace PharmacyInventoryDispensingSystem.Domain.Entities.Medicines;

public sealed class Medicine : SoftDeletableEntity
{

    /// <summary>
    /// Unique identifier for the medicine 
    /// </summary>
    public string Code { get; private set; } = string.Empty; 

    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Strength of the medicine, e.g., 500mg, 250mg, etc.
    /// </summary>
    public string Strength { get; private set; } = string.Empty;


    public MedicineForm Form { get;private set; } 

     public string Unit { get; private set; } = string.Empty; // e.g., Box, Vial

    /// <summary>
    ///Minimum stock level before reordering to monitor inventory levels (Alert threshold).
    /// </summary>
    public int ReorderLevel { get;private set; } 
    public bool IsActive { get;private set; }


    private readonly List<MedicineBatch> _batches = [];
    public IReadOnlyCollection<MedicineBatch> Batches => _batches.AsReadOnly();

    private readonly List<PrescriptionItem> _prescriptionItems = [];
    public IReadOnlyCollection<PrescriptionItem> PrescriptionItems => _prescriptionItems.AsReadOnly();

    private Medicine()
    {
        //For EF-Core
    }


    private Medicine(
    Guid id,
    string code,
    string name,
    string strength,
    MedicineForm form,
    string unit,
    int reorderLevel)
    : base(id)
    {
        Code = code;
        Name = name;
        Strength = strength;
        Form = form;
        Unit = unit;
        ReorderLevel = reorderLevel;
        IsActive = true;
    }

 




    public static Result<Medicine> Create(Guid id,string code, string name, string strength, MedicineForm form, string unit, int reorderLevel)
    {
          if (id == Guid.Empty)
             {
            return MedicineErrors.MedicineIdRequired;
            }

            if (string.IsNullOrWhiteSpace(name)) 
            {
                return MedicineErrors.NameRequired;
            }



            if (string.IsNullOrWhiteSpace(code))
            {
                return MedicineErrors.CodeRequired;
            }


            if (string.IsNullOrWhiteSpace(strength)) 
            { 
                return MedicineErrors.StrengthRequired;
            }



            if (reorderLevel < 0) 
            {
                return MedicineErrors.ReorderLevelNegative;
            }

        if (!Enum.IsDefined(form) )
        {
            return MedicineErrors.InvalidForm;

        }
        return new Medicine(id, code, name, strength, form, unit, reorderLevel);
    }





    public Result<Updated> Update(
     string code,
     string name,
     string strength,
     MedicineForm form,
     string unit,
     int reorderLevel)
    {
        if (string.IsNullOrWhiteSpace(code))
            return MedicineErrors.CodeRequired;

        

        if (string.IsNullOrWhiteSpace(name))
            return MedicineErrors.NameRequired;

        

        if (string.IsNullOrWhiteSpace(strength))
            return MedicineErrors.StrengthRequired;

        if (!Enum.IsDefined(form))
            return MedicineErrors.InvalidForm;

        if (reorderLevel < 0)
            return MedicineErrors.ReorderLevelNegative;

        Code = code;
        Name = name;
        Strength = strength;
        Form = form;
        Unit = unit;
        ReorderLevel = reorderLevel;

        return Result.Updated;
    }

    public  Result<Updated> Activate() 
    {
        if (IsActive)
            return MedicineErrors.AlreadyActive;


        IsActive= true;
        AddDomainEvent(new MedicineActivatedEvent(Id, DateTimeOffset.UtcNow));
        return    Result.Updated;
    
    }


    public Result<Updated> Deactivate()
    {
        if (!IsActive)
            return MedicineErrors.AlreadyInactive;

        IsActive = false;
        AddDomainEvent(new MedicineDeactivatedEvent(Id, DateTimeOffset.UtcNow));
        return Result.Updated;
    }

    /// <summary>
    /// Inactive medicines cannot be used in new prescriptions(use this method):
    /// </summary>
    /// <returns> </returns>
    public Result<Success> EnsureCanBePrescribed()
    {
        if (IsDeleted)
            return MedicineErrors.Archived(Code);

        if (!IsActive)
            return MedicineErrors.Inactive(Code);

        return Result.Success;
    }
}
