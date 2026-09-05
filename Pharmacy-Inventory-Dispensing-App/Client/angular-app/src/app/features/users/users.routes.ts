import { Routes } from '@angular/router';
import { UsersListComponent } from './pages/users-list/users-list.component';
import { UserCreate } from './pages/user-create/user-create';
import { UserEdit } from './pages/user-edit/user-edit';
import { Permissions } from '../../core/auth/auth.models';
import { roleGuard } from '../../core/guards/role.guard';

export const USER_ROUTES: Routes = [
  {
    path: '',
    component: UsersListComponent
  },
  {
    path: 'create',
    component: UserCreate,
    canActivate: [roleGuard],
    data: { permissions: [Permissions.UsersCreate] }
  },
  {
    path: ':id/edit',
    component: UserEdit,
    canActivate: [roleGuard],
    data: { permissions: [Permissions.UsersUpdate] }
  }
];
