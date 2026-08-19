namespace PharmacyInventoryDispensingSystem.Domain.Enums;

public enum PrescriptionStatus
{
 
    Active = 1,//  Valid for dispensing while within ValidFrom/ValidTo 
    
    Cancelled = 2, // manually cancelled(by doctot); still visible in history
    Expired = 3 // ValidTo passed or system-marked expired;
}
