namespace PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.Medicine
{
    public sealed record GetArchivedMedicinesRequest(
     string? SearchTerm = null,
     int PageNumber = 1,
     int PageSize = 10);
}
