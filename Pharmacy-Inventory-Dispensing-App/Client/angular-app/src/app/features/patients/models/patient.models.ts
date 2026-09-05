export interface Patient {
  id: string;
  documentId: string;
  documentType?: string;
  fullName: string;
  age: number;
  phoneNumber: string;
  dateOfBirth?: string;
  createdAtUtc: string;
  isArchived?: boolean;
}

export interface CreatePatientDto {
  documentId: string;
  fullName: string;
  dateOfBirth: string;
  phoneNumber: string;
}

export interface UpdatePatientDto {
  documentId: string;
  fullName: string;
  dateOfBirth: string;
  phoneNumber: string;
}

export interface PatientQueryParams {
  pageNumber: number;
  pageSize: number;
  searchTerm?: string;
  documentType?: string;
  sortBy?: string;
  isDescending?: boolean;
}

export interface PatientLookupDto {
  id: string;
  fullName: string;
}
