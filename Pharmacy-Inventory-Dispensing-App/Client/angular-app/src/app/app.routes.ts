import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { Permissions } from './core/auth/auth.models';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/pages/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'forgot-password',
    loadComponent: () => import('./features/auth/pages/forgot-password/forgot-password').then(m => m.ForgotPasswordComponent)
  },
  {
    path: 'reset-password',
    loadComponent: () => import('./features/auth/pages/reset-password/reset-password').then(m => m.ResetPasswordComponent)
  },
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)
      },
      {
        path: 'profile',
        loadComponent: () => import('./features/profile/pages/profile/profile').then(m => m.ProfileComponent)
      },
      {
        path: 'patients',
        loadChildren: () => import('./features/patients/patients.routes').then(m => m.PATIENT_ROUTES),
        canActivate: [roleGuard],
        data: { permissions: [Permissions.PatientsRead] }
      },
      {
        path: 'medicines',
        loadChildren: () => import('./features/medicines/medicines.routes').then(m => m.MEDICINE_ROUTES),
        canActivate: [roleGuard],
        data: { permissions: [Permissions.MedicinesRead] }
      },
      {
        path: 'prescriptions',
        loadChildren: () => import('./features/prescriptions/prescriptions.routes').then(m => m.PRESCRIPTION_ROUTES),
        canActivate: [roleGuard],
        data: { permissions: [Permissions.PrescriptionsRead] }
      },
      {
        path: 'dispensing',
        loadChildren: () => import('./features/dispensing/dispensing.routes').then(m => m.DISPENSING_ROUTES),
        canActivate: [roleGuard],
        data: { permissions: [Permissions.DispensesRead] }
      },
      {
        path: 'users',
        loadChildren: () => import('./features/users/users.routes').then(m => m.USER_ROUTES),
        canActivate: [roleGuard],
        data: { permissions: [Permissions.UsersRead] }
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
