import { Routes } from '@angular/router';
import { PrescriptionsListComponent } from './pages/prescriptions-list/prescriptions-list.component';
import { PrescriptionCreate } from './pages/prescription-create/prescription-create';
import { PrescriptionDetails } from './pages/prescription-details/prescription-details';
import { Permissions } from '../../core/auth/auth.models';
import { roleGuard } from '../../core/guards/role.guard';

export const PRESCRIPTION_ROUTES: Routes = [
  {
    path: '',
    component: PrescriptionsListComponent
  },
  {
    path: 'create',
    component: PrescriptionCreate,
    canActivate: [roleGuard],
    data: { permissions: [Permissions.PrescriptionsCreate] }
  },
  {
    path: ':id',
    component: PrescriptionDetails,
    canActivate: [roleGuard],
    data: { permissions: [Permissions.PrescriptionsRead] }
  }
];
