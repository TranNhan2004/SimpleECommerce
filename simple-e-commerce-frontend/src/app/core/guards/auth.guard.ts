import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import Keycloak from 'keycloak-js';
import { environment } from '../../../environments/environment';
import { Role } from '../enums/role';


const hasRole = (keycloak: Keycloak, role: Role): boolean => {
  const realmRoles = keycloak.realmAccess?.roles ?? [];
  const resourceRoles = keycloak.resourceAccess?.[environment.keycloak.clientId]?.roles ?? [];
  return realmRoles.includes(role) || resourceRoles.includes(role);
};

const waitLoading = async (auth: AuthService): Promise<void> => {
  while (auth.isLoading()) {
    await new Promise<void>((resolve) => setTimeout(resolve, 25));
  }
}

export const authGuard: CanActivateFn = async (route, state) => {
  const auth = inject(AuthService);
  const keycloak = inject(Keycloak);
  const router = inject(Router);

  await waitLoading(auth);

  if (!auth.isAuthenticated()) {
    await keycloak.login({
      redirectUri: `${window.location.origin}${state.url}`,
    });
    return false;
  }

  const requiredRoles = (route.data['roles'] as Role[] | undefined) ?? [];
  if (!requiredRoles.length) return true;

  const isAllowed = requiredRoles.some((role) => hasRole(keycloak, role));
  return isAllowed ? true : router.parseUrl('/forbidden');
};
