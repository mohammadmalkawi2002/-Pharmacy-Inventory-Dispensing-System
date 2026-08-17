using PharmacyInventoryDispensingSystem.Domain.Common;

namespace PharmacyInventoryDispensingSystem.Domain.Entities.Medicines.Event;

public class MedicineDeactivatedEvent : DomainEvent
{
    public Guid Id { get; private set; }

    public DateTimeOffset OccurredOn { get; private set; }

    public MedicineDeactivatedEvent()
    {
    }

    public MedicineDeactivatedEvent(Guid id, DateTimeOffset occurredOn)
    {
        Id = id;
        OccurredOn = occurredOn;
    }
}
