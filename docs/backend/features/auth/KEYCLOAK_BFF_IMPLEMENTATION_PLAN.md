# Keycloak OAuth BFF Implementation Plan

## Objective

Move the application to a Backend For Frontend authentication model:

1. The browser starts login with the backend, not directly with Keycloak.
2. Keycloak authenticates the user with authorization code flow.
3. The backend exchanges the code for tokens with a confidential client.
4. The backend stores the authenticated session server-side in Redis and issues only an opaque session identifier to the browser.
5. The browser calls the backend with cookies plus CSRF protection.
6. The browser never stores Keycloak access tokens for API authorization.

## Why This Plan Replaces The Current One

The old docs were written around a SPA-first bearer-token model. The repo now contains configuration that points to a BFF direction instead:

- `Auth` settings already define session-cookie and CSRF names.
- `Keycloak` settings already define login callback, logout, token, and userinfo endpoints.
- `AuthenticationController` already exists as the intended browser auth entry point.
- The current JWT bearer setup is still useful for service-to-service access, but it should no longer be the primary browser flow.
- Application authorization should live in our own domain and database, not in Keycloak realm roles.

## Scope

This document set is intentionally backend-first. It covers the work we need before frontend integration:

- Keycloak realm and confidential client setup for BFF
- Backend OAuth login and callback flow
- Server-side session persistence
- Cookie authentication and CSRF
- Authenticated user resolution and local profile sync
- Application-managed authorization
- Logout, auth status, and verification

Frontend integration work can start only after these backend tasks are reviewed and accepted.

## Current Repo Assumptions

- Development frontend origin: `https://dev-ecommerce.trannhanweb.io.vn`
- Development backend origin: `https://dev-api-ecommerce.trannhanweb.io.vn`
- Local dev targets behind the tunnel: frontend `http://localhost:4200`, backend `http://localhost:5000`
- Production frontend origin: `https://ecommerce.trannhanweb.io.vn`
- Production backend origin: `https://api-ecommerce.trannhanweb.io.vn`
- Docker backend published port: `5001`
- Keycloak base URL: `http://localhost:8080`
- Keycloak realm from config: `SimpleECommerce`
- Current callback path from config: `/api/v1.0/auth/login/callback`

## Important Decision Before Implementation

The plan treats `https://dev-api-ecommerce.trannhanweb.io.vn` as the canonical backend origin for development because:

- the Cloudflare tunnel maps that domain to local backend `http://localhost:5000`
- `appsettings.Development.json` uses `https://dev-api-ecommerce.trannhanweb.io.vn/api/v1.0/auth/login/callback`

If we want to use a different development domain or local backend port later, we should update the redirect URI and related Keycloak client settings before implementation.

## Environment Policy

### Development

- Development uses HTTPS through the Cloudflare tunnel domains.
- Frontend development origin is `https://dev-ecommerce.trannhanweb.io.vn`.
- Backend development origin is `https://dev-api-ecommerce.trannhanweb.io.vn`.
- Those public domains are forwarded to local frontend `http://localhost:4200` and local backend `http://localhost:5000`.
- The authenticated session is represented by an `HttpOnly` cookie named `APP_SESSION_ID` that contains only `session_id`.
- The CSRF token is generated and returned only after authentication succeeds and the backend session is created.
- Use `Secure` cookies in development because the browser-facing tunnel domains are HTTPS.

### Production

- Use HTTPS end to end.
- Frontend production origin is `https://ecommerce.trannhanweb.io.vn`.
- Backend production origin is `https://api-ecommerce.trannhanweb.io.vn`.
- Use `Secure` cookies.
- Use `SameSite=None` for the authenticated browser session when frontend and backend are separated across origins.
- Apply the production cookie policy when deploying behind Cloudflared tunnel.

## Target Backend Architecture

### Browser-facing authentication

- Use backend auth endpoints under `/api/v1.0/auth/*`
- Use OAuth authorization code flow with a confidential Keycloak client and PKCE
- Store OAuth `state` and `code_verifier` server-side with a short expiration
- Exchange the authorization code from the callback endpoint using the stored `code_verifier`
- Store session data in Redis under keys like `auth:{session_id}`
- Persist at minimum:
  - `access_token`
  - `refresh_token`
  - token expiry metadata
  - identity claims needed to rebuild the principal
  - session metadata such as created time, last refresh time, and logout status

### Browser session

- Issue an `HttpOnly` session cookie named `APP_SESSION_ID` that contains only the opaque `session_id`
- Issue a CSRF cookie readable by the browser
- Require the configured CSRF header on unsafe browser requests
- Do not expose access tokens or refresh tokens to JavaScript
- If the frontend needs auth state, expose it through backend endpoints such as `me` or `session` rather than exposing tokens
- The frontend should call the backend with credentials enabled so the browser automatically includes the `HttpOnly` session cookie
- The CSRF cookie should be issued only after successful authentication and session creation

### Backend identity

- Build the authenticated principal from Keycloak claims
- Resolve or create the local user profile from the Keycloak subject
- Load authorization from the application database or application-defined permission model
- Keep bearer token support only if we still need non-browser automation

## Session Storage Strategy

The proposed Redis model is valid and is a better fit than placing large token payloads in browser cookies.

Recommended shape:

- Redis key: `auth:{session_id}`
- Value:
  - `access_token`
  - `refresh_token`
  - `access_token_expires_at`
  - `refresh_token_expires_at`
  - `subject`
  - `email`
  - `preferred_username`
  - optional `id_token` if needed for RP-initiated logout
- TTL:
  - no longer than the refresh token lifetime
  - cleared immediately on logout or session revocation

Recommended security rule:

- In development, the browser should receive `session_id` through an `HttpOnly` cookie named `APP_SESSION_ID`, using `Secure` whenever the tunnel domain is HTTPS.
- In production, the browser should receive `session_id` through a `Secure`, `HttpOnly` cookie.
- The frontend should not store the raw `session_id` in local storage or other JavaScript-readable storage.
- The frontend should rely on the browser to send the cookie automatically on backend requests.
- If the frontend needs to know whether the user is logged in, it should call a backend auth-status endpoint.
- Because the browser is redirected to Keycloak during login, the Keycloak authorization endpoint must also be reachable from the browser. If the browser is not running on the same machine as Keycloak, expose Keycloak through HTTPS as well.

## Deliverables

- A new auth document set for BFF
- Backend-first task breakdown with acceptance criteria
- Keycloak Admin Console instructions in every task file

## Task Order

- [Task 01](./KEYCLOAK_BFF_TASK_01_REALM_CLIENT_AND_REDIRECTS.md)
- [Task 02](./KEYCLOAK_BFF_TASK_02_BACKEND_AUTH_FOUNDATION.md)
- [Task 03](./KEYCLOAK_BFF_TASK_03_LOGIN_CALLBACK_AND_SESSION.md)
- [Task 04](./KEYCLOAK_BFF_TASK_04_USER_CONTEXT_AND_PROFILE_SYNC.md)
- [Task 05](./KEYCLOAK_BFF_TASK_05_LOGOUT_CSRF_AND_BROWSER_CONTRACT.md)
- [Task 06](./KEYCLOAK_BFF_TASK_06_TESTING_SWAGGER_AND_CLEANUP.md)

## Out Of Scope For This Pass

- Angular implementation details
- Final production ingress and reverse proxy hardening
- Social identity providers
- Keycloak theme work
- Mobile app auth flow
