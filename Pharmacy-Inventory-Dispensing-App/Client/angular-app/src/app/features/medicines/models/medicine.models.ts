export type MedicineForm = 'Tablet' | 'Capsule' | 'Syrup' | 'Cream' | 'Injection' | 'Drops';
export type StockUnit = 'Tablet' | 'Capsule' | 'Bottle' | 'Vial' | 'Ampoule' | 'Tube';
export type PackageUnit = 'Box' | 'Pack';
export interface Medicine {
  id: string;
  code: string;
  name: string;
  strength: string;
  form: MedicineForm;
  stockUnit: string;
  packageUnit: string;
  unitsPerPackage: number;
  quantityInStock: number;
  reorderLevel: number;
  stockStatus: string;
  isActive: boolean;
  createdAtUtc?: string;
  updatedAtUtc?: string;
  updatedBy?: string;
}



export interface CreateMedicineDto {
  code: string;
  name: string;
  strength: string;
  form: MedicineForm;
  stockUnit: string;
  packageUnit: string;
  unitsPerPackage: number;
  reorderLevel: number;
}

export interface UpdateMedicineDto {
  code: string;
  name: string;
  strength: string;
  form: MedicineForm;
  stockUnit: string;
  packageUnit: string;
  unitsPerPackage: number;
  reorderLevel: number;
}

export interface ReceiveStockDto {
  packageQuantity: number;
}

export interface ReceiveStockResponse {
  medicineId: string;
  receivedPackages: number;
  packageUnit: string;
  receivedQuantity: number;
  stockUnit: string;
  oldQuantity: number;
  newQuantity: number;
}

/**
 * Computed stock status — not stored in backend.
 */
export type StockStatus = 'Normal' | 'Low Stock' | 'Out of Stock' | string;

export function getStockSeverity(status: StockStatus): 'success' | 'warn' | 'danger' {
  switch (status) {
    case 'Normal': return 'success';
    case 'Low Stock': return 'warn';
    case 'Out of Stock': return 'danger';
    default: return 'warn';
  }
}

export const MEDICINE_FORM_OPTIONS: { label: string; value: MedicineForm }[] = [
  { label: 'Tablet', value: 'Tablet' },
  { label: 'Capsule', value: 'Capsule' },
  { label: 'Syrup', value: 'Syrup' },
  { label: 'Cream', value: 'Cream' },
  { label: 'Injection', value: 'Injection' },
  { label: 'Drops', value: 'Drops' },
];

export const STOCK_UNIT_OPTIONS: { label: string; value: StockUnit }[] = [
  { label: 'Tablet', value: 'Tablet' },
  { label: 'Capsule', value: 'Capsule' },
  { label: 'Bottle', value: 'Bottle' },
  { label: 'Vial', value: 'Vial' },
  { label: 'Ampoule', value: 'Ampoule' },
  { label: 'Tube', value: 'Tube' }
];

export const PACKAGE_UNIT_OPTIONS: { label: string; value: PackageUnit }[] = [
  { label: 'Box', value: 'Box' },
  { label: 'Pack', value: 'Pack' }
];

export interface MedicineLookupDto {
  id: string;
  code?: string;
  name: string;
  strength?: string;
  form?: string;
  stockUnit?: string;
}
