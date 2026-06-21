# Task 04: User Context, Profile Sync, And Application Authorization

## Goal

Build the authenticated application user from the BFF session and keep authorization inside our application, not in Keycloak.

## Code Areas Expected To Change

- Current user context resolution
- User lookup and creation services
- User profile handlers
- Application authorization policy wiring

## Work Items

1. Build the current user principal from the BFF session instead of assuming a bearer token.
2. Resolve the Keycloak subject from the authenticated claims.
3. Decide how to handle the current `Guid` assumption for Keycloak `sub`:
   - keep it only if the realm guarantees UUID subjects
   - otherwise change local storage and parsing to support Keycloak string subjects safely
4. Create or update the local user profile on first authenticated use.
5. Load application roles or permissions from our own database after the user is resolved.
6. Map local application roles or permissions into backend authorization policies.
7. Ensure user context and profile logic still work for existing protected use cases.

## Keycloak UI Changes

1. Open `Realm settings` and then token or client scope related areas in the Admin Console.
2. Verify the token payload exposes:
   - `sub`
   - `email`
   - `preferred_username`
3. If any claim is missing, open the client `Client scopes` area.
4. Add or adjust mappers so those claims are included in the access token or userinfo response used by the backend.
5. Do not configure application roles in Keycloak for this task.

## Acceptance Criteria

- Authenticated browser requests can resolve the current user context.
- The backend can reliably map Keycloak identity to a local user record.
- Profile creation or sync no longer depends on frontend-provided bearer tokens.
- Authorization still works for customer, seller, and admin users using application-managed data.

## Notes

- The current repo assumes `sub` parses as `Guid`; this is a likely implementation risk and should be validated early.
- If we switch to string-based subject storage, document that migration before changing database schema.
- Keycloak should stay responsible for authentication and identity claims only.
