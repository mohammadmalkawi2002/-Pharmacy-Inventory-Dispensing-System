import { UserRole } from '../../../core/auth/auth.models';


export interface SystemUser {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: UserRole;
  isActive: boolean;
  createdAtUtc?: string;
}

export interface CreateUserDto {
  firstName: string;
  lastName: string;
  email: string;
  role: UserRole;
  password: string;
}

export interface UpdateUserDto {
  firstName?: string;
  lastName?: string;
  email?: string;
  role?: UserRole;
}
