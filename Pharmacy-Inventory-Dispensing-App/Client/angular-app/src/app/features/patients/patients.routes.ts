import { Routes } from '@angular/router';
import { PatientsListComponent } from './pages/patients-list/patients-list.component';
import { PatientCreate } from './pages/patient-create/patient-create';
import { PatientEdit } from './pages/patient-edit/patient-edit';
import { Permissions } from '../../core/auth/auth.models';
import { roleGuard } from '../../core/guards/role.guard';

export const PATIENT_ROUTES: Routes = [
  {
    path: '',
    component: PatientsListComponent
  },
  {
    path: 'create',
    component: PatientCreate,
    canActivate: [roleGuard],
    data: { permissions: [Permissions.PatientsCreate] }
  },
  {
    path: ':id/edit',
    component: PatientEdit,
    canActivate: [roleGuard],
    data: { permissions: [Permissions.PatientsUpdate] }
  }
];
