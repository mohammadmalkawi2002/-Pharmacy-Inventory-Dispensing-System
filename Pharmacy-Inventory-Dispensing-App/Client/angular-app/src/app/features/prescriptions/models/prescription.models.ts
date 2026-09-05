export type PrescriptionStatus = 'Active' | 'Cancelled' | 'Expired';

export interface PrescriptionItem {
  id?: string;
  medicineId: string;
  medicineCode?: string;
  medicineName?: string;
  strength?: string;
  form?: string;
  stockUnit?: string;
  quantityPrescribed?: number;
  maxFillCount?: number;
  fillUsedCount?: number;
  remainingFillCount?: number;
  dosageInstructions?: string;
  // Legacy / fallback fields
  quantity?: number;
  refills?: number;
  medicineStrength?: string;
  medicineForm?: string;
  medicineUnit?: string;
}

export interface Prescription {
  id: string;
  prescriptionNumber: string;
  patientId: string;
  patientDocumentId?: string;
  patientName?: string;
  doctorName?: string;
  validFrom: string;
  validTo: string;
  status: PrescriptionStatus;
  notes?: string;
  createdAtUtc?: string;
  createdAt?: string;
  items: PrescriptionItem[];
}

export interface CreatePrescriptionDto {
  patientId: string;
  validFrom: string;
  validTo: string;
  notes?: string;
  items: {
    medicineId: string;
    quantityPrescribed: number;
    maxFillCount: number;
    dosageInstructions?: string;
  }[];
}

export interface UpdatePrescriptionDto {
  validFrom?: string;
  validTo?: string;
  notes?: string;
  items?: {
    medicineId: string;
    quantityPrescribed: number;
    maxFillCount: number;
    dosageInstructions?: string;
  }[];
}

export function getPrescriptionStatusSeverity(status: PrescriptionStatus): 'success' | 'danger' | 'warn' | 'secondary' {
  switch (status) {
    case 'Active': return 'success';
    case 'Cancelled': return 'danger';
    case 'Expired': return 'warn';
    default: return 'secondary';
  }
}

export interface PrescriptionQueryParams {
  pageNumber: number;
  pageSize: number;
  searchTerm?: string;
  status?: string;
  sortBy?: string;
  isDescending?: boolean;
}
