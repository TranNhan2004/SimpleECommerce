import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import Keycloak from 'keycloak-js';
import { environment } from '../../../environments/environment';
import { Role } from '../enums/role';


const getRole = (keycloak: Keycloak): Role[] => {
  const realmRoles = keycloak.realmAccess?.roles ?? [];
  return [...realmRoles] as Role[];
};

const waitLoading = async (auth: AuthService): Promise<void> => {
  while (auth.isLoading()) {
    await new Promise<void>((resolve) => setTimeout(resolve, 25));
  }
}

export const notAuthGuard: CanActivateFn = async (route, state) => {
  const auth = inject(AuthService);
  const keycloak = inject(Keycloak);
  const router = inject(Router);

  await waitLoading(auth);

  if (!auth.isAuthenticated()) {
    return true;
  }

  const roles = getRole(keycloak);
  if (roles.includes(Role.Admin)) {
    return router.parseUrl('/admin');
  } else if (roles.includes(Role.Seller)) {
    return router.parseUrl('/seller');
  } else if (roles.includes(Role.Customer)) {
    return router.parseUrl('/customer/products');
  }

  return router.parseUrl('/forbidden');
};
