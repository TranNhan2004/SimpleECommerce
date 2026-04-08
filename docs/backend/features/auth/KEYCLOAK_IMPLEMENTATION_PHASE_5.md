# Phase 5: Profile Sync and Authorization Rules

## Goal

Keep local business data in sync without taking ownership of browser authentication.

## Summary

Keycloak owns identity. The backend owns business data and authorization. The local profile should be created or updated after the user has authenticated through Keycloak.

## Prerequisites

- Phase 1 completed.
- Phase 2 completed.
- Phase 3 completed.
- Local `UserProfile` model exists.

## Implementation Steps

### 1. Sync the local profile

- Create or update the local `UserProfile` after the first authenticated API call.
- Use the Keycloak `sub` claim as the stable identity key.
- Store only business-specific profile data.

### 2. Map roles to policies

- Map `customer`, `seller`, and `admin` roles to backend authorization policies.
- Keep policy names descriptive and stable.
- Ensure controller or endpoint authorization matches business behavior.

### 3. Keep registration in Keycloak

- Do not own the browser registration flow in the backend.
- Keep account management and password recovery in Keycloak.
- Use backend provisioning only if a machine-to-machine workflow still requires it.

### 4. Handle admin automation separately

- If the backend needs user provisioning or synchronization jobs, keep that in a separate service-account flow.
- Do not mix admin automation with browser login.

## Deliverables

- Profile sync behavior documented.
- Authorization policy mapping documented.
- Identity mapping by `sub` documented.

## Notes

- The backend should not store password credentials.
- The backend should not become the source of truth for user login state.
