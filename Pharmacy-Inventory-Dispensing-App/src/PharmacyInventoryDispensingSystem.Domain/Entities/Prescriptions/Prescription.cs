using PharmacyInventoryDispensingSystem.Domain.Common;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Entities.Dispenses;
using PharmacyInventoryDispensingSystem.Domain.Enums;

namespace PharmacyInventoryDispensingSystem.Domain.Entities.Prescriptions;

public class Prescription : SoftDeletableEntity
{

    public string PrescriptionNumber { get; private set; } = string.Empty;

    public string PatientName { get; private set; } = string.Empty;

    public string? PatientPhone { get; private set; }

    public string DoctorId { get; private set; } = string.Empty;

    public DateTime ValidFrom { get; private set; }

    public DateTime ValidTo { get; private set; }

    public int MaxRefills { get; private set; }

    public int RefillsUsed { get; private set; }

    public PrescriptionStatus Status { get; private set; } = PrescriptionStatus.Active;

    public string? Notes { get; private set; }

    private readonly List<PrescriptionItem> _items = [];
    public IReadOnlyCollection<PrescriptionItem> Items => _items.AsReadOnly();

    private readonly List<Dispense> _dispenses = [];
    public IReadOnlyCollection<Dispense> Dispenses => _dispenses.AsReadOnly();




    private Prescription()
    {
        
    }


    private Prescription(Guid id,string prescriptionNumber, string patientName, string? patientPhone,
        string doctorId, DateTime validFrom, DateTime validTo, int maxRefills, string? notes)
        :base(id)
    {
        PrescriptionNumber = prescriptionNumber;
        PatientName = patientName;
        PatientPhone = patientPhone;
        DoctorId = doctorId;
        ValidFrom = validFrom.Date;
        ValidTo = validTo.Date;
        MaxRefills = maxRefills;
        Notes = notes;
        Status = PrescriptionStatus.Active;
        RefillsUsed = 0;
    }

    public static Result<Prescription> Create(Guid id,string prescriptionNumber,
        string patientName, string? patientPhone,
        string doctorId, DateTime validFrom, DateTime validTo, int maxRefills, string? notes = null)
    {

        if (id == Guid.Empty)
            return PrescriptionErrors.PrescriptionIdRequired;

        if (string.IsNullOrWhiteSpace(prescriptionNumber))
            return PrescriptionErrors.NumberRequired;

        if(string.IsNullOrWhiteSpace(patientName))
            return PrescriptionErrors.PatientNameRequired;

        if(string.IsNullOrWhiteSpace(doctorId))
            return PrescriptionErrors.DoctorIdRequired;

        if (validFrom.Date > validTo.Date)
            return PrescriptionErrors.InvalidValidityPeriod;

        if (maxRefills < 0)
            return PrescriptionErrors.MaxRefillsNegative;




        return new Prescription(id, prescriptionNumber.Trim(), patientName.Trim(),string.IsNullOrWhiteSpace(patientPhone)?string.Empty:patientPhone.Trim(), doctorId, validFrom, validTo, maxRefills,string.IsNullOrWhiteSpace(notes)?string.Empty:notes.Trim());


    }

    /// <summary>
    /// is used later to check whether a specific prescription is valid on a given date by verifying 
    /// that its status is Active and that the date falls within its validity period
    /// </summary>
    /// <param name="asOf"></param>
    /// <returns></returns>
    public bool IsValidOn(DateTime asOf) =>
        Status == PrescriptionStatus.Active && asOf.Date >= ValidFrom.Date && asOf <= ValidTo.Date;

    
    
    public bool HasRefillsRemaining() => RefillsUsed < MaxRefills; // or RefillsUsed < MaxRefills +1
}
