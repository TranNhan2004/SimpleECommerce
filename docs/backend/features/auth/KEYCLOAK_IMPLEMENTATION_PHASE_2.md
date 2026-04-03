# Phase 2: Backend Token Verification

## Goal

Make the backend validate Keycloak access tokens using public keys or JWKS.

## Status

Implemented in the API startup path and documented here as the current backend token-validation setup.

## Summary

The backend is a bearer-token protected API. It does not own interactive login, registration, or refresh for browser users.

## What Was Implemented

### 1. JWT bearer authentication

- The API registers the standard ASP.NET Core `JwtBearer` handler during startup.
- The backend issuer is built from `Keycloak:AuthServerUrl` and `Keycloak:Realm`.
- The backend audience is configured from `Keycloak:Resource`.
- Token audience verification is driven by `Keycloak:VerifyTokenAudience`.
- HTTPS metadata is enabled outside development.

Current implementation:

- [Program.cs](/home/thnhan1/SimpleECommerce/SimpleECommerceBackend/SimpleECommerceBackend.Api/Program.cs)

### 2. Token integrity checks

The backend validation path now relies on the standard JWT bearer middleware to enforce:

- signature validation against Keycloak public keys or JWKS
- issuer validation
- audience validation
- expiration validation
- not-before validation

### 3. Identity mapping

- The local business identity remains mapped to the Keycloak `sub` claim.
- `UserProfile.Id` stores the Keycloak user ID.
- Realm roles are used for authorization policies.
- The backend expects a roles claim such as `roles` or an equivalent claim mapping from the Keycloak token.

Supporting code:

- [UserProfile entity](/home/thnhan1/SimpleECommerce/SimpleECommerceBackend/SimpleECommerceBackend.Domain/Entities/UserProfile.cs)
- [IUserContext contract](/home/thnhan1/SimpleECommerce/SimpleECommerceBackend/SimpleECommerceBackend.Application/Interfaces/Security/IUserContext.cs)
- [Role policies](/home/thnhan1/SimpleECommerce/SimpleECommerceBackend/SimpleECommerceBackend.Api/Extensions/PolicyExtension.cs)

### 4. Browser auth dependency removed from the backend path

- The backend no longer needs to own browser login or token issuance.
- The browser should authenticate with Keycloak directly.
- The backend only validates the bearer token and applies authorization.
- The backend does not need the Keycloak auth helper library to validate access tokens.

### 5. Production settings to keep

- `Keycloak:AuthServerUrl` for the realm issuer base URL
- `Keycloak:Resource` for the backend audience
- `Keycloak:VerifyTokenAudience` for audience validation behavior
- `Keycloak:Realm` for the issuer path
- role claim mapping used by authorization policies
- local identity mapping based on `sub`

## Deliverables

- Bearer-token authentication configured.
- Keycloak token validation documented.
- Local identity mapping documented.
- Old browser auth flow retired from the backend path.

## Notes

- The backend behaves like a resource server.
- If machine-to-machine admin automation is still needed, keep that separate from browser authentication.
- The remaining phases can build on this without reintroducing browser-owned login in the backend.
