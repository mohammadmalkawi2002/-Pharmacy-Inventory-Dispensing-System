using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Common.Interfaces.Repositories
{
    public interface IUnitOfWork:IDisposable
    {
        //add your IRepo
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
          

    }
}
