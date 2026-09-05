import { Routes } from '@angular/router';
import { DispensingWorkflowComponent } from './pages/dispensing-workflow/dispensing-workflow.component';
import { DispensingRecordsComponent } from './pages/dispensing-records/dispensing-records.component';
import { DispenseDetailsComponent } from './pages/dispense-details/dispense-details.component';
import { Permissions } from '../../core/auth/auth.models';
import { roleGuard } from '../../core/guards/role.guard';

export const DISPENSING_ROUTES: Routes = [
  {
    path: '',
    component: DispensingWorkflowComponent,
    canActivate: [roleGuard],
    data: { permissions: [Permissions.DispensesCreate] }
  },
  {
    path: 'records',
    component: DispensingRecordsComponent,
    canActivate: [roleGuard],
    data: { permissions: [Permissions.DispensesRead] }
  },
  {
    path: 'records/:id',
    component: DispenseDetailsComponent,
    canActivate: [roleGuard],
    data: { permissions: [Permissions.DispensesRead] }
  }
];
