import { Component, Input, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AuthStore } from '../../core/auth/auth.store';
import { Permissions } from '../../core/auth/auth.models';
import { TooltipModule } from 'primeng/tooltip';

export interface NavMenuItem {
  label: string;
  icon: string;
  route: string;
  permission?: string;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterModule, TooltipModule],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent {
  readonly authStore = inject(AuthStore);

  @Input() collapsed = false;

  readonly menuItems: NavMenuItem[] = [
    {
      label: 'Dashboard',
      icon: 'pi pi-home',
      route: '/dashboard'
    },
    {
      label: 'Patients',
      icon: 'pi pi-users',
      route: '/patients',
      permission: Permissions.PatientsRead
    },
    {
      label: 'Medicines',
      icon: 'pi pi-box',
      route: '/medicines',
      permission: Permissions.MedicinesRead
    },
    {
      label: 'Prescriptions',
      icon: 'pi pi-file',
      route: '/prescriptions',
      permission: Permissions.PrescriptionsRead
    },
    {
      label: 'Dispensing',
      icon: 'pi pi-shopping-cart',
      route: '/dispensing',
      permission: Permissions.DispensesRead
    },
    {
      label: 'Users',
      icon: 'pi pi-user',
      route: '/users',
      permission: Permissions.UsersRead
    }
  ];

  isPermitted(item: NavMenuItem): boolean {
    if (!item.permission) return true;
    return this.authStore.hasPermission(item.permission);
  }
}
