export type UserRole = 'Admin' | 'Receptionist' | 'Doctor' | 'Pharmacist';

export interface AuthUser {
  id: string;
  email: string;
  name: string;
  firstName?: string;
  lastName?: string;
  roles: UserRole[];
  permissions: string[];
}

export interface AuthState {
  user: AuthUser | null;
  accessToken: string | null;
  refreshToken?: string | null;
  accessTokenExpiresAtUtc?: string | null;
  refreshTokenExpiresAtUtc?: string | null;
  isAuthenticated: boolean;
  loading: boolean;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface ResetPasswordRequest {
  email: string;
  token: string;
  newPassword: string;

}

export interface ChangePasswordRequest {
  currentPassword: string,
  newPassword: string

}


//this is interface for login response from backend(must match backend response):
export interface LoginResponse {

  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  roles: string[];
  permissions: string[];

  accessToken: string;
  accessTokenExpiresAtUtc: string;
  refreshToken: string;
  refreshTokenExpiresAtUtc: string;
}

export interface RefreshResponse {
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  roles: string[];
  permissions: string[];

  accessToken: string;
  accessTokenExpiresAtUtc: string;
  refreshToken: string;
  refreshTokenExpiresAtUtc: string;
}

export interface UserProfile {
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  roles: string[];
  permissions: string[];
}

/**
 * Backend permission names — matches backend response format with Permissions. prefix.(object)
 */
export const Permissions = {
  // Users
  UsersRead: 'Permissions.Users.Read',
  UsersCreate: 'Permissions.Users.Create',
  UsersUpdate: 'Permissions.Users.Update',
  UsersActivate: 'Permissions.Users.Activate',
  UsersDeactivate: 'Permissions.Users.Deactivate',

  // Patients
  PatientsRead: 'Permissions.Patients.Read',
  PatientsCreate: 'Permissions.Patients.Create',
  PatientsUpdate: 'Permissions.Patients.Update',
  PatientsDelete: 'Permissions.Patients.Delete',

  // Medicines
  MedicinesRead: 'Permissions.Medicines.Read',
  MedicinesCreate: 'Permissions.Medicines.Create',
  MedicinesUpdate: 'Permissions.Medicines.Update',
  MedicinesDelete: 'Permissions.Medicines.Delete',
  MedicinesActivate: 'Permissions.Medicines.Activate',
  MedicinesDeactivate: 'Permissions.Medicines.Deactivate',
  MedicinesReadLowStock: 'Permissions.Medicines.ReadLowStock',

  // Prescriptions
  PrescriptionsRead: 'Permissions.Prescriptions.Read',
  PrescriptionsCreate: 'Permissions.Prescriptions.Create',
  PrescriptionsUpdate: 'Permissions.Prescriptions.Update',
  PrescriptionsCancel: 'Permissions.Prescriptions.Cancel',
  PrescriptionsLookup: 'Permissions.Prescriptions.Lookup',

  // Dispenses
  DispensesRead: 'Permissions.Dispenses.Read',
  DispensesCreate: 'Permissions.Dispenses.Create',

  // Auth
  AuthChangePassword: 'Permissions.Auth.ChangePassword',
} as const;

/**
 * Role → Permission mapping for frontend fallback.
 * Backend authorization remains the source of truth.
 */
export const ROLE_PERMISSIONS: Record<UserRole, string[]> = {
  Admin: Object.values(Permissions),

  Receptionist: [
    Permissions.PatientsRead,
    Permissions.PatientsCreate,
    Permissions.PatientsUpdate,
    Permissions.AuthChangePassword,
  ],

  Doctor: [
    Permissions.PatientsRead,
    Permissions.MedicinesRead,
    Permissions.PrescriptionsRead,
    Permissions.PrescriptionsCreate,
    Permissions.PrescriptionsUpdate,
    Permissions.PrescriptionsCancel,
    Permissions.PrescriptionsLookup,
    Permissions.AuthChangePassword,
  ],

  Pharmacist: [
    Permissions.MedicinesRead,
    Permissions.MedicinesReadLowStock,
    Permissions.PrescriptionsRead,
    Permissions.PrescriptionsLookup,
    Permissions.DispensesRead,
    Permissions.DispensesCreate,
    Permissions.AuthChangePassword,
  ],
};
