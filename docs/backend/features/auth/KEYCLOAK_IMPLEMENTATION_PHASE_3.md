# Phase 3: SPA Authentication Integration

## Goal

Let the Angular frontend own sign-in, sign-out, token handling, and route protection.

## Summary

The SPA is the only browser-facing OIDC client. It should manage the browser redirect, token storage strategy, refresh flow, and route guards.

## Prerequisites

- Phase 1 completed.
- Phase 2 completed.
- The Angular app structure and environment files are ready.
- The Keycloak client ID and redirect URIs are known.

## Implementation Steps

### 1. Add a Keycloak client integration layer

- Create a frontend auth service or adapter.
- Configure the Keycloak client ID and realm URL in the Angular environment files.
- Keep the client public.
- Enable PKCE in the SPA integration.

### 2. Handle login and logout

- Add a login action that redirects the browser to Keycloak.
- Add a logout action that signs the user out through Keycloak.
- Return the user to the Angular app after login and logout.

### 3. Manage auth state

- Store whether the user is authenticated.
- Store the current role set when the SPA needs it for UI decisions.
- Keep auth state reactive so guards and header UI update automatically.

### 4. Protect routes

- Add guards for customer, seller, and admin routes.
- Prevent unauthenticated access to protected screens.
- Redirect to Keycloak for login when required.

### 5. Call the backend with bearer tokens

- Attach the access token to API requests.
- Handle token expiry in the SPA flow.
- Avoid sending credentials to the backend for login.

## Deliverables

- SPA login and logout wired to Keycloak.
- Route guards implemented.
- Access token added to API requests.
- Auth state visible in the UI.

## Notes

- The SPA should not store or depend on a backend auth secret.
- If token refresh is handled by a library, keep it entirely on the frontend side.
