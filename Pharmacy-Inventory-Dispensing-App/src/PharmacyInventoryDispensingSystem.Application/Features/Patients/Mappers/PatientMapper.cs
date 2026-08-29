using PharmacyInventoryDispensingSystem.Application.Features.Patients.Common;
using PharmacyInventoryDispensingSystem.Application.Features.Patients.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Entities.Patients;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Mappers
{
    public static class PatientMapper
    {

        public static PatientResponseDto ToDto(this Patient entity) 
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new PatientResponseDto(
                Id: entity.Id,
                DocumentId: entity.DocumentId,
                DocumentType:GetDocumentType(entity.DocumentId),
                FullName:entity.FullName,
                DateOfBirth:entity.DateOfBirth,
                Age:CalculateAge(entity.DateOfBirth),
                PhoneNumber:entity.PhoneNumber,
                CreatedAtUtc:entity.CreatedAtUtc);

        }

        
        public static List<PatientResponseDto> ToDtos(this IEnumerable<Patient> entities) 
        {
            ArgumentNullException.ThrowIfNull(entities);

            return [
                    .. entities.Select(entity => entity.ToDto())
                
                   ];
        }

       private static PatientDocumentType GetDocumentType(string documentId) 
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(documentId);
            return documentId[0] switch
            {
                '1' => PatientDocumentType.Citizen,
                '2' => PatientDocumentType.Resident,
                _ => throw new InvalidOperationException(
                "Patient document ID has an invalid prefix.")
            };

            
        }

        private static int CalculateAge(DateTime dateOfBirth) 
        {
            DateTime todayDate = DateTime.UtcNow.Date;

            int age = todayDate.Year - dateOfBirth.Year;

            if (dateOfBirth.Date > todayDate.AddYears(-age)) 
            {

                age--;
            }        

            return age;
        }

    }
}
