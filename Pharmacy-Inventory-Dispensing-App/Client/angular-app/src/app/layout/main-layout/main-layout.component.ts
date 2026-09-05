import { Component, signal } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NavbarComponent } from '../navbar/navbar.component';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [
    RouterModule,
    NavbarComponent,
    SidebarComponent,
    ToastModule,
    ConfirmDialogModule
  ],
  templateUrl: './main-layout.component.html',
  styleUrls: ['./main-layout.component.css']
})
export class MainLayoutComponent {
  sidebarCollapsed = signal(false);
  mobileMenuOpen = signal(false);

  toggleSidebar(): void {
    if (window.innerWidth <= 768) {
      this.mobileMenuOpen.update(v => !v);
    } else {
      this.sidebarCollapsed.update(c => !c);
    }
  }

  closeMobileMenu(): void {
    this.mobileMenuOpen.set(false);
  }
}
