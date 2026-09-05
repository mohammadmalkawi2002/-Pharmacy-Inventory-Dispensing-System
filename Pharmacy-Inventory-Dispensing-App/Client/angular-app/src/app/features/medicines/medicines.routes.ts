import { Routes } from '@angular/router';
import { MedicinesListComponent } from './pages/medicines-list/medicines-list.component';
import { MedicineCreate } from './pages/medicine-create/medicine-create';
import { MedicineEdit } from './pages/medicine-edit/medicine-edit';
import { MedicineReceiveStock } from './pages/medicine-receive-stock/medicine-receive-stock';
import { MedicineDetailsComponent } from './pages/medicine-details/medicine-details.component';
import { Permissions } from '../../core/auth/auth.models';
import { roleGuard } from '../../core/guards/role.guard';

export const MEDICINE_ROUTES: Routes = [
  {
    path: '',
    component: MedicinesListComponent
  },
  {
    path: ':id/details',
    component: MedicineDetailsComponent
  },
  {
    path: 'create',
    component: MedicineCreate,
    canActivate: [roleGuard],
    data: { permissions: [Permissions.MedicinesCreate] }
  },
  {
    path: ':id/edit',
    component: MedicineEdit,
    canActivate: [roleGuard],
    data: { permissions: [Permissions.MedicinesUpdate] }
  },
  {
    path: ':id/receive-stock',
    component: MedicineReceiveStock,
    canActivate: [roleGuard],
    data: { permissions: [Permissions.MedicinesUpdate] } // Assume same permission as update
  }
];
