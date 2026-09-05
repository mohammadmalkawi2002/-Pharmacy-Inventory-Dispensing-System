import { Component, EventEmitter, Output, inject } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { AuthStore } from '../../core/auth/auth.store';
import { AuthService } from '../../core/auth/auth.service';
import { ThemeService } from '../../core/services/theme.service';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';
import { MenuModule } from 'primeng/menu';
import { TagModule } from 'primeng/tag';
import { MenuItem } from 'primeng/api';
import { UserRole } from '../../core/auth/auth.models';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    RouterModule,
    ButtonModule,
    TooltipModule,
    MenuModule,
    TagModule
  ],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {
  readonly authStore = inject(AuthStore);
  readonly authService = inject(AuthService);
  readonly themeService = inject(ThemeService);
  readonly router = inject(Router);

  @Output() toggleSidebar = new EventEmitter<void>();

  userMenuItems: MenuItem[] = [
    {
      label: 'Profile',
      icon: 'pi pi-user',
      command: () => this.router.navigate(['/profile'])
    },
    { separator: true },
    {
      label: 'Logout',
      icon: 'pi pi-sign-out',
      styleClass: 'text-red-500',
      command: () => this.authService.logout(this.authStore.refreshToken()!)
        .subscribe()
    }
  ];

  getRoleSeverity(role: UserRole | null): 'success' | 'info' | 'warn' | 'danger' | 'secondary' | 'contrast' {
    switch (role) {
      case 'Admin': return 'danger';
      case 'Doctor': return 'info';
      case 'Pharmacist': return 'success';
      case 'Receptionist': return 'warn';
      default: return 'secondary';
    }
  }

  getRoleLabel(role: UserRole | null): string {
    return role ?? 'User';
  }
}
