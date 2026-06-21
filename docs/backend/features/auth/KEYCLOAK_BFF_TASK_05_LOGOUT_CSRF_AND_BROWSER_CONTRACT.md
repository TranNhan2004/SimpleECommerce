# Task 05: Logout, CSRF, And Browser Contract

## Goal

Finalize the browser-facing BFF contract so authenticated requests and logout are safe and predictable.

## Code Areas Expected To Change

- `AuthenticationController`
- Auth middleware or filters for CSRF validation
- Session invalidation services
- Any endpoint that returns current auth state

## Work Items

1. Implement logout to:
   - invalidate the local session
   - delete the Redis entry for `auth:{session_id}`
   - clear the auth and CSRF cookies
   - optionally call Keycloak end-session endpoint
   - redirect or respond with the correct frontend logout target
2. Add CSRF validation for unsafe browser requests when cookie auth is used.
3. Add a lightweight auth-status endpoint such as `me` or `session`.
4. Define the browser contract for:
   - login redirect start
   - callback completion
   - logout
   - unauthorized session expiration
   - how the frontend discovers auth state without reading tokens or raw session storage
   - how the frontend sends credentialed requests so the browser includes the `HttpOnly` session cookie
5. Ensure CORS and cookie settings work for the frontend origin.
6. Define environment-specific cookie settings for:
   - local HTTP development
   - production HTTPS behind Cloudflared tunnel

## Keycloak UI Changes

1. Open the BFF client in Keycloak.
2. Open `Settings`.
3. Confirm `Valid post logout redirect URIs` includes the exact frontend logout landing page from:
   - `Auth:FrontendBaseUrl`
   - `Auth:PostLogoutRedirectPath`
4. Open `Realm settings` and review session timeout values so they do not conflict with backend session expectations.
5. If logout must terminate the upstream Keycloak session too, verify the end-session endpoint for the realm is enabled and reachable.

## Acceptance Criteria

- Browser logout clears the local session reliably.
- CSRF is required on unsafe cookie-authenticated requests.
- The frontend has a stable way to discover whether the user is authenticated.
- Cross-origin frontend requests can include the backend session cookie correctly.
- Cookie, CORS, and logout behavior are aligned with the configured frontend origin.

## Notes

- Keep the CSRF token separate from the `HttpOnly` auth cookie.
- Prefer keeping `session_id` inside an `HttpOnly` cookie named `APP_SESSION_ID` rather than exposing it to frontend JavaScript.
- For local frontend-to-backend API calls, the frontend should use credentialed requests so the browser sends the cookie automatically.
- Issue the CSRF cookie only after the backend session has been created successfully.
- For production behind Cloudflared tunnel, use HTTPS with `SameSite=None` and `Secure`.
- Prefer returning explicit 401 or 403 responses for expired sessions rather than silent redirects on API calls.
