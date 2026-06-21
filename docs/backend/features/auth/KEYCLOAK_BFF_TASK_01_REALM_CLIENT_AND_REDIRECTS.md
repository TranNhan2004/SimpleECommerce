# Task 01: Realm, Client, And Redirect Setup

## Goal

Prepare Keycloak for a backend-owned OAuth flow using a confidential client.

## Why This Task Comes First

All backend implementation depends on the exact realm, client, redirect URI, logout URI, and identity claims coming from Keycloak.

## Backend Config Inputs

Use these config keys as the source of truth when implementing:

- `Auth:FrontendBaseUrl`
- `Auth:PostLoginRedirectPath`
- `Auth:PostLogoutRedirectPath`
- `Keycloak:BaseUrl`
- `Keycloak:Realm`
- `Keycloak:ClientId`
- `Keycloak:ClientSecret`
- `Keycloak:RedirectUri`
- `Keycloak:CallbackPath`
- `Keycloak:Scopes`

## Work Items

1. Confirm the canonical local backend origin for login callbacks.
2. Create or update the `SimpleECommerce` realm.
3. Configure a confidential OpenID Connect client for the backend BFF.
4. Configure valid login and logout redirect URIs for the backend callback and frontend post-login pages.
5. Confirm Keycloak is used only as the Identity Provider, not as the application authorization source.
6. Confirm tokens include `sub`, `email`, and `preferred_username`.
7. Confirm the browser can reach the Keycloak authorization page from the chosen dev or production domains.

## Keycloak UI Changes

1. Open `http://localhost:8080/admin/`.
2. Sign in with the Keycloak admin account.
3. Select the `SimpleECommerce` realm, or create it if it does not exist.
4. Open `Clients`.
5. Create or open the client whose ID matches `Keycloak:ClientId`.
6. Set these client values:
   - Client type: `OpenID Connect`
   - Client authentication: `On`
   - Authorization: `Off`
   - Standard flow: `On`
   - Direct access grants: `Off`
   - Implicit flow: `Off`
   - Service accounts: `Off` for the browser flow
7. In `Credentials`, copy the generated client secret into `Keycloak:ClientSecret`.
8. In `Settings`, configure:
   - Valid redirect URIs: include `Keycloak:RedirectUri`
   - Valid post logout redirect URIs: include `Auth:FrontendBaseUrl` plus `Auth:PostLogoutRedirectPath`
   - Web origins: include `Auth:FrontendBaseUrl`
9. Open `Client scopes` or `Evaluate` and verify the access token or userinfo payload includes:
    - `sub`
    - `email`
    - `preferred_username`

## Acceptance Criteria

- Keycloak has a confidential client for the backend BFF.
- The callback URI matches the backend URL we will actually run.
- The post-logout redirect URI matches the frontend URL we expect after logout.
- The client secret is stored only in backend configuration.
- Required identity claims are present in issued tokens or userinfo.
- The browser can reach the Keycloak login page during the redirect flow.

## Notes

- Do not use a public SPA client for this flow.
- Do not give the frontend the client secret.
- Do not model application roles in Keycloak for this backend flow.
- For this repo, development uses `https://dev-ecommerce.trannhanweb.io.vn` and `https://dev-api-ecommerce.trannhanweb.io.vn`.
- If Keycloak stays on `localhost`, the login redirect only works in browsers that can reach that local Keycloak instance.
