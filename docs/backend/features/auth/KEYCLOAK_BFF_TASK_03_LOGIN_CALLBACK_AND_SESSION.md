# Task 03: Login, Callback, And Session Establishment

## Goal

Implement the core BFF login flow from backend login endpoint to Redis-backed authenticated session.

## Code Areas Expected To Change

- `SimpleECommerceBackend.Api/Controllers/V1/AuthenticationController.cs`
- New DTOs or response models for login and callback
- New auth services for state protection and code exchange
- Cookie sign-in plumbing

## Work Items

1. Implement `GET /api/v1.0/auth/login` to:
   - create a one-time OAuth state record
   - create and store a PKCE `code_verifier`
   - derive the PKCE `code_challenge`
   - remember the desired post-login redirect
   - redirect the browser to Keycloak authorization endpoint with `state` and `code_challenge`
2. Implement `GET` callback endpoint at `Keycloak:CallbackPath` to:
   - validate state
   - load the stored `code_verifier`
   - exchange authorization code plus `code_verifier` for tokens
   - load user claims from the token or userinfo endpoint
   - create the Redis session record at `auth:{session_id}`
   - persist the token bundle and session metadata with TTL aligned to refresh-token expiry
   - issue the session cookie
   - generate and return the CSRF cookie only after authentication succeeds
   - redirect the browser to the configured frontend post-login path
3. Handle failure cases:
   - missing or invalid state
   - token exchange failure
   - missing required claims
   - expired or replayed callback
4. Log auth events without logging raw tokens or secrets.
5. Define whether token refresh is:
   - performed lazily on the next authenticated request
   - or handled by forcing re-login when the stored access token expires

## Keycloak UI Changes

1. Open the BFF client in Keycloak.
2. Open `Settings`.
3. Confirm `Valid redirect URIs` contains the exact callback URI from `Keycloak:RedirectUri`.
4. Confirm `Web origins` contains the frontend origin from `Auth:FrontendBaseUrl`.
5. Confirm `Valid post logout redirect URIs` includes the frontend logout landing page.
6. Open `Client scopes` and confirm the `openid`, `profile`, and `email` scopes are available for the client.

## Acceptance Criteria

- Hitting the backend login endpoint redirects the browser to Keycloak.
- A valid callback creates an authenticated backend session.
- The browser receives an `HttpOnly` auth cookie named `APP_SESSION_ID` containing only `session_id`, plus a CSRF cookie.
- The session cookie is designed to be sent automatically by the browser on subsequent frontend-to-backend requests.
- Tokens stay in backend-controlled storage.
- Auth failures return controlled, supportable responses.

## Notes

- Keep the callback route exactly aligned with Keycloak config.
- The callback should tolerate the frontend being a separate origin.
- Use short-lived OAuth state records to reduce replay risk.
- Use PKCE for the browser-started authorization code flow even though the backend is a confidential client.
- Store enough session metadata in Redis to rebuild the application principal without re-calling Keycloak on every request.
- In development, the cookie policy should support `https://dev-ecommerce.trannhanweb.io.vn` and `https://dev-api-ecommerce.trannhanweb.io.vn`.
- In production, switch to HTTPS with `SameSite=None` and `Secure`.
