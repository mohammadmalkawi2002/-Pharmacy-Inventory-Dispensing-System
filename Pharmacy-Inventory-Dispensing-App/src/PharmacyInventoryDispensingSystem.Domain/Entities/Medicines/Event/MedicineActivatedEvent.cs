using PharmacyInventoryDispensingSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Domain.Entities.Medicines.Event
{
    public  class MedicineActivatedEvent : DomainEvent
    {

        public Guid Id { get; private set; }
        public DateTimeOffset OccurredOn {  get; private set; }
        public MedicineActivatedEvent()
        {
            
        }


        public MedicineActivatedEvent(Guid id, DateTimeOffset occurredOn) 

        {
            Id= id;
            OccurredOn = occurredOn;
        }
    }

}
