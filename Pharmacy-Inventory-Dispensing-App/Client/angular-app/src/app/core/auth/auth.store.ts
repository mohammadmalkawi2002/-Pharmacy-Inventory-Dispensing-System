import { Injectable, computed, signal } from '@angular/core';
import { AuthState, AuthUser, LoginResponse, ROLE_PERMISSIONS, UserRole } from './auth.models';

const TOKEN_STORAGE_KEY = 'pharma_access_token';
const REFRESH_TOKEN_STORAGE_KEY = 'pharma_refresh_token';
const USER_STORAGE_KEY = 'pharma_user';

@Injectable({
  providedIn: 'root'
})
export class AuthStore {
  readonly #state = signal<AuthState>(this.#loadInitialState());

  // Public readonly signals
  readonly user = computed<AuthUser | null>(() => this.#state().user);
  readonly accessToken = computed<string | null>(() => this.#state().accessToken);
  readonly refreshToken = computed<string | null>(() => this.#state().refreshToken ?? null);
  readonly isAuthenticated = computed<boolean>(() => this.#state().isAuthenticated);
  readonly loading = computed<boolean>(() => this.#state().loading);
  readonly permissions = computed<string[]>(() => this.#state().user?.permissions ?? []);
  readonly userRoles = computed<UserRole[]>(() => this.#state().user?.roles ?? []);
  readonly primaryRole = computed<UserRole | null>(() => this.#state().user?.roles[0] ?? null);

  #loadInitialState(): AuthState {
    try {
      const token = this.#retrieveToken();
      const refreshToken = localStorage.getItem(REFRESH_TOKEN_STORAGE_KEY);
      const userJson = localStorage.getItem(USER_STORAGE_KEY);

      if (token && refreshToken && userJson) {
        const user = JSON.parse(userJson) as AuthUser;
        return {
          user,
          accessToken: token,
          refreshToken: refreshToken,
          isAuthenticated: true,
          loading: false,
        };
      }
    } catch {
      this.#clearStorage();
    }

    return {
      user: null,
      accessToken: null,
      refreshToken: null,
      isAuthenticated: false,
      loading: false,
    };
  }

  setLoading(loading: boolean): void {
    this.#state.update(s => ({ ...s, loading }));
  }

  setLogin(response: LoginResponse): void {
    const roles = (response.roles ?? []) as UserRole[];
    const fullName = `${response.firstName || ''} ${response.lastName || ''}`.trim() || response.email;

    const user: AuthUser = {
      id: response.userId,
      email: response.email,
      name: fullName,
      firstName: response.firstName,
      lastName: response.lastName,
      roles: roles,
      permissions: response.permissions?.length
        ? response.permissions
        : this.#derivePermissions(roles),
    };

    this.#state.set({
      user,
      accessToken: response.accessToken,
      refreshToken: response.refreshToken,
      accessTokenExpiresAtUtc: response.accessTokenExpiresAtUtc,
      refreshTokenExpiresAtUtc: response.refreshTokenExpiresAtUtc,
      isAuthenticated: true,
      loading: false,
    });

    this.#storeToken(response.accessToken);
    if (response.refreshToken) {
      localStorage.setItem(REFRESH_TOKEN_STORAGE_KEY, response.refreshToken);
    }
    localStorage.setItem(USER_STORAGE_KEY, JSON.stringify(user));
  }

  updateToken(token: string, refreshToken?: string): void {
    this.#state.update(s => ({
      ...s,
      accessToken: token,
      ...(refreshToken ? { refreshToken } : {})
    }));
    this.#storeToken(token);
    if (refreshToken) {
      localStorage.setItem(REFRESH_TOKEN_STORAGE_KEY, refreshToken);
    }
  }

  logout(): void {
    this.#state.set({
      user: null,
      accessToken: null,
      refreshToken: null,
      accessTokenExpiresAtUtc: null,
      refreshTokenExpiresAtUtc: null,
      isAuthenticated: false,
      loading: false,
    });
    this.#clearStorage();
  }

  hasPermission(permission: string): boolean {
    const user = this.#state().user;
    if (!user)
      return false;

    // Admin role has all permissions
    if (user.roles.includes('Admin'))
      return true;

    if (!user.permissions || !user.permissions.length) return false;

    // Direct match
    if (user.permissions.includes(permission)) return true;

    // Robust prefix matching: handles both 'Permissions.X.Y' and 'X.Y'
    const withPrefix = permission.startsWith('Permissions.') ? permission : `Permissions.${permission}`;
    const withoutPrefix = permission.startsWith('Permissions.') ? permission.substring(12) : permission;

    return user.permissions.some(p => p === withPrefix || p === withoutPrefix);
  }

  hasAnyPermission(permissions: string[]): boolean {
    return permissions.some(p => this.hasPermission(p));
  }

  hasRole(role: UserRole): boolean {
    const user = this.#state().user;
    if (!user) return false;
    if (user.roles.includes('Admin')) return true;
    return user.roles.includes(role);
  }

  // --- Token Storage ---

  #storeToken(token: string): void {
    localStorage.setItem(TOKEN_STORAGE_KEY, token);
  }

  #retrieveToken(): string | null {
    return localStorage.getItem(TOKEN_STORAGE_KEY);
  }

  #clearStorage(): void {
    localStorage.removeItem(TOKEN_STORAGE_KEY);
    localStorage.removeItem(REFRESH_TOKEN_STORAGE_KEY);
    localStorage.removeItem(USER_STORAGE_KEY);
  }

  #derivePermissions(roles: UserRole[]): string[] {
    const perms = new Set<string>();
    for (const role of roles) {
      const rolePerms = ROLE_PERMISSIONS[role];
      if (rolePerms) {
        rolePerms.forEach(p => perms.add(p));
      }
    }
    return Array.from(perms);
  }
}
