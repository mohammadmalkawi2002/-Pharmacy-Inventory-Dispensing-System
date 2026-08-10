namespace PharmacyInventoryDispensingSystem.Domain.Enums;

public enum MovementType
{
    Receive = 1, // stock (+) Batch receive
    Dispense = 2,//stock (-)   Dispense workflow
    Adjustment = 3,// EX:  stok+=10 or stok-=10 // damaged , RECorrection calculation
    Expired = 4  //(-)
}
