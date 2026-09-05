import { PrescriptionStatus } from '../../prescriptions/models/prescription.models';

export interface PrescriptionLookupRequest {
  prescriptionNumber: string;
  patientDocumentId: string;
}

export interface LookupPrescriptionItemDto {
  prescriptionItemId: string;
  medicineId: string;
  medicineCode: string;
  medicineName: string;
  strength: string;
  form: string;
  stockUnit: string;
  quantityPrescribed: number;
  quantityInStock: number;
  maxFillCount: number;
  fillUsedCount: number;
  remainingFillCount: number;
  dosageInstructions?: string | null;
  canDispense: boolean;
  unavailableReason?: string | null;

  // UI state only
  selected?: boolean;
}

export interface LookupPrescriptionResponse {
  prescriptionId: string;
  prescriptionNumber: string;
  patientName: string;
  patientDocumentId: string;
  doctorName: string;
  validFrom: string;
  validTo: string;
  status: PrescriptionStatus | string;
  notes?: string | null;
  canDispense: boolean;
  unavailableReason?: string | null;
  items: LookupPrescriptionItemDto[];
}

export interface CreateDispenseRequest {
  prescriptionId: string;
  documentId: string;
  prescriptionItemIds: string[];
  notes?: string | null;
}

export interface DispenseItemDto {
  id: string;
  prescriptionItemId: string;
  medicineId: string;
  medicineCode: string;
  medicineName: string;
  strength: string;
  stockUnit: string;
  quantity: number;
  dosageInstructions?: string | null;
}

export interface DispenseDetailsDto {
  id: string;
  prescriptionId: string;
  prescriptionNumber: string;
  patientId: string;
  patientName: string;
  patientDocumentId: string;
  pharmacistId: string;
  pharmacistName: string;
  dispensedAt: string;
  notes?: string | null;
  items: DispenseItemDto[];
}

export interface DispenseResponseDto {
  id: string;
  prescriptionId: string;
  prescriptionNumber: string;
  patientName: string;
  pharmacistName: string;
  dispensedAt: string;
}

export interface DispenseQueryParams {
  pageNumber: number;
  pageSize: number;
  searchTerm?: string;
  fromDate?: string;
  toDate?: string;
}

// Aliases for compatibility
export type LookedUpPrescription = LookupPrescriptionResponse;
export type LookedUpPrescriptionItem = LookupPrescriptionItemDto;
