import { Injectable, signal, effect } from '@angular/core';

export type ThemeMode = 'light' | 'dark';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  readonly themeMode = signal<ThemeMode>('light');

  constructor() {
    const savedTheme = localStorage.getItem('pharma_theme') as ThemeMode | null;
    if (savedTheme === 'dark' || savedTheme === 'light') {
      this.themeMode.set(savedTheme);
    }

    effect(() => {
      const mode = this.themeMode();
      document.documentElement.setAttribute('data-theme', mode);
      if (mode === 'dark') {
        document.documentElement.classList.add('p-dark');
      } else {
        document.documentElement.classList.remove('p-dark');
      }
      localStorage.setItem('pharma_theme', mode);
    });
  }

  toggleDarkMode(): void {
    this.themeMode.update(m => (m === 'light' ? 'dark' : 'light'));
  }

  isDark(): boolean {
    return this.themeMode() === 'dark';
  }
}
