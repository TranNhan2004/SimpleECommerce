# Task 02: Backend Auth Foundation

## Goal

Introduce the backend authentication foundation for BFF without implementing the full login flow yet.

## Code Areas Expected To Change

- `SimpleECommerceBackend.Api/Program.cs`
- `SimpleECommerceBackend.Api/Extensions/AuthenticationExtension.cs`
- New auth option models under API or Infrastructure
- New auth service abstractions for state, token exchange, and session storage

## Work Items

1. Add typed options for the `Auth` and `Keycloak` sections used by the BFF flow.
2. Add cookie authentication for browser sessions.
3. Decide whether JWT bearer remains enabled for non-browser callers.
4. Add a server-side store for:
   - OAuth state
   - Session records
   - token records under Redis keys like `auth:{session_id}`
5. Register an HTTP client for Keycloak token, userinfo, and logout calls.
6. Add consistent serialization, expiration, and secure cookie settings from config.
7. Create new application-layer auth session services and use cases for:
   - creating a session record after callback
   - reading the current session by `session_id`
   - updating tokens after refresh
   - revoking the session on logout

## Keycloak UI Changes

1. Open the BFF client in the Keycloak Admin Console.
2. Open `Settings`.
3. Re-check the client is confidential and that `Standard flow` is enabled.
4. Open `Credentials`.
5. Confirm the secret currently stored in Keycloak matches the secret that will be placed in backend config.
6. Open `Realm settings` and confirm the realm issuer base is the same realm referenced by `Keycloak:Authority` and `Keycloak:Realm`.

## Acceptance Criteria

- The backend can bind all BFF auth settings from configuration.
- Browser auth is represented by a cookie-based scheme.
- A clear Redis-backed persistence strategy exists for OAuth state and authenticated sessions.
- The backend has an HTTP client path ready to call Keycloak endpoints.
- No frontend token handling is required for the browser path.

## Notes

- Prefer server-side session storage over putting OAuth tokens directly in cookies.
- Reuse Redis-backed infrastructure where it fits the current project patterns.
- The cookie should carry only an opaque session identifier, not the token payload.
- If bearer auth stays, it should be secondary and explicit, not the default browser contract.
