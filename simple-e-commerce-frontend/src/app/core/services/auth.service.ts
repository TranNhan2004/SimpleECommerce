import { effect, inject, Injectable, signal } from '@angular/core';
import { KEYCLOAK_EVENT_SIGNAL } from 'keycloak-angular';
import Keycloak from 'keycloak-js';
import { environment } from '../../../environments/environment';
import { Role } from '../enums/role';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly keycloak = inject(Keycloak);
  private readonly keycloakEventSignal = inject(KEYCLOAK_EVENT_SIGNAL);

  private readonly authenticatedState = signal(false);
  private readonly loadingState = signal(true);
  private readonly usernameState = signal<string | null>(null);
  private readonly displayNameState = signal('Guest');
  private readonly accessTokenState = signal<string | null>(null);
  private readonly realmRolesState = signal<string[]>([]);
  private readonly resourceRolesState = signal<string[]>([]);

  readonly authenticated = this.authenticatedState.asReadonly();
  readonly loading = this.loadingState.asReadonly();
  readonly username = this.usernameState.asReadonly();
  readonly displayName = this.displayNameState.asReadonly();
  readonly accessToken = this.accessTokenState.asReadonly();
  readonly realmRoles = this.realmRolesState.asReadonly();
  readonly resourceRoles = this.resourceRolesState.asReadonly();

  isAuthenticated(): boolean {
    return this.authenticatedState();
  }

  isLoading(): boolean {
    return this.loadingState();
  }

  getDisplayName(): string {
    return this.displayNameState();
  }

  getUsername(): string | null {
    return this.usernameState();
  }

  getAccessToken(): string | null {
    return this.accessTokenState();
  }

  getRealmRoles(): string[] {
    return [...this.realmRolesState()];
  }

  getResourceRoles(): string[] {
    return [...this.resourceRolesState()];
  }

  constructor() {
    effect(() => {
      this.keycloakEventSignal();
      void this.syncFromKeycloak();
    });
  }

  async login(redirectUri: string = this.defaultRedirectUri()): Promise<void> {
    await this.keycloak.login({ redirectUri: this.normalizeRedirectUri(redirectUri) });
  }

  async register(redirectUri: string = this.defaultRedirectUri()): Promise<void> {
    await this.keycloak.login({ action: 'register', redirectUri: this.normalizeRedirectUri(redirectUri) });
  }

  async logout(redirectUri: string = this.defaultRedirectUri()): Promise<void> {
    await this.keycloak.logout({ redirectUri: this.normalizeRedirectUri(redirectUri) });
  }

  async ensureAuthenticated(redirectUri: string = this.defaultRedirectUri()): Promise<boolean> {
    if (this.authenticatedState()) {
      return true;
    }

    await this.login(redirectUri);
    return false;
  }

  async updateToken(minValiditySeconds = 30): Promise<boolean> {
    return this.keycloak.updateToken(minValiditySeconds);
  }

  hasRole(role: Role): boolean {
    return this.realmRolesState().includes(role) || this.resourceRolesState().includes(role);
  }

  getRoles(): string[] {
    return [...new Set([...this.realmRolesState(), ...this.resourceRolesState()])].sort();
  }

  private defaultRedirectUri(): string {
    return `${window.location.origin}/`;
  }

  private normalizeRedirectUri(redirectUri: string): string {
    if (/^https?:\/\//i.test(redirectUri)) {
      return redirectUri;
    }

    if (redirectUri.startsWith('/')) {
      return `${window.location.origin}${redirectUri}`;
    }

    return `${window.location.origin}/${redirectUri}`;
  }

  private async syncFromKeycloak(): Promise<void> {
    const authenticated = this.keycloak.authenticated ?? false;
    this.authenticatedState.set(authenticated);

    // ✅ Bỏ dòng this.loadingState.set(false) ở đây

    if (!authenticated) {
      this.usernameState.set(null);
      this.displayNameState.set('Guest');
      this.accessTokenState.set(null);
      this.realmRolesState.set([]);
      this.resourceRolesState.set([]);
      this.loadingState.set(false); // ✅ Set false sau khi đã clear state
      return;
    }

    this.accessTokenState.set(this.keycloak.token ?? null);
    this.realmRolesState.set(this.keycloak.realmAccess?.roles ?? []);
    this.resourceRolesState.set(this.keycloak.resourceAccess?.[environment.keycloak.clientId]?.roles ?? []);

    try {
      const profile = await this.keycloak.loadUserProfile();
      const fallbackName =
        this.keycloak.tokenParsed?.['preferred_username'] ?? profile.username ?? 'Authenticated user';
      const displayName = [profile.firstName, profile.lastName].filter(Boolean).join(' ').trim() || fallbackName;

      this.usernameState.set(profile.username ?? fallbackName);
      this.displayNameState.set(displayName);
    } catch {
      const fallbackName = this.keycloak.tokenParsed?.['preferred_username'] ?? 'Authenticated user';
      this.usernameState.set(fallbackName);
      this.displayNameState.set(fallbackName);
    } finally {
      this.loadingState.set(false); // ✅ Chỉ set false sau khi mọi thứ xong
    }
  }
}
