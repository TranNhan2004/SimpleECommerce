# Keycloak SPA-First Implementation Plan

## Document Information

- **Project**: SimpleECommerce
- **Date**: 2026-03-18
- **Target**: FE -> Keycloak -> Token -> BE
- **Strategy**: SPA-first OIDC with Keycloak as the browser identity provider
- **Status**: Phase 1 complete; Phase 2 implemented; remaining phases draft

---

## Architecture Summary

### Target Flow

1. The Angular frontend redirects the browser to Keycloak.
2. Keycloak authenticates the user with the authorization-code flow and PKCE.
3. Keycloak returns tokens to the SPA.
4. The SPA calls the backend with `Authorization: Bearer <access_token>`.
5. The backend validates the token signature with Keycloak public keys or JWKS, checks issuer and audience, and authorizes the request.

### Rules

- The SPA is the only browser-facing OIDC client.
- The backend does not own interactive login, registration, password reset, or token refresh for browser users.
- The backend is a resource server, not a browser auth broker.
- Public SPA clients should not use a client secret.
- Backend admin or service-account flows are allowed only for machine-to-machine automation.
- Local user identity should map to the Keycloak `sub` claim.

---

## What Is Removed

The following legacy concepts are not part of the new plan:

- Backend-owned `/auth/login`, `/auth/register`, and `/auth/refresh` browser flow.
- Custom JWT generation and password hashing.
- Direct access grants for browser login.
- The backend acting as a proxy between the SPA and Keycloak.
- Any flow that requires the SPA to know a backend client secret.

---

## Implementation Phases

Detailed implementation lives in separate phase documents:

- [Phase 1](./KEYCLOAK_IMPLEMENTATION_PHASE_1.md)
- [Phase 2](./KEYCLOAK_IMPLEMENTATION_PHASE_2.md)
- [Phase 3](./KEYCLOAK_IMPLEMENTATION_PHASE_3.md)
- [Phase 4](./KEYCLOAK_IMPLEMENTATION_PHASE_4.md)
- [Phase 5](./KEYCLOAK_IMPLEMENTATION_PHASE_5.md)
- [Phase 6](./KEYCLOAK_IMPLEMENTATION_PHASE_6.md)

## Phase 1: Realm and Client Setup

### Goal

Configure Keycloak so the browser uses a public SPA client and the backend only validates tokens.

### Tasks

- Create or update the `SimpleECommerce` realm.
- Configure realm roles such as `customer`, `seller`, and `admin`.
- Create a public SPA client, for example `simple-e-commerce-spa`.
- Enable the authorization-code flow with PKCE.
- Disable direct access grants for the browser client.
- Configure valid redirect URIs, logout redirect URIs, and web origins for the Angular app.
- Configure a backend resource-server client such as `simple-e-commerce-backend` if the backend needs explicit audience mapping.
- Confirm tokens include the role and subject claims needed by the backend.

### Deliverables

- Realm configuration documented.
- SPA client configuration documented.
- Backend resource-server validation requirements documented.

---

## Phase 2: Backend Token Verification

### Goal

Make the backend validate Keycloak access tokens using public keys or JWKS.

### Status

Implemented in the API startup path and documented in the phase file.

### Tasks

- Configure bearer-token authentication in the backend API.
- Validate issuer, audience, and signature against Keycloak public keys.
- Load realm public keys or JWKS from Keycloak.
- Remove any dependency on client secrets for browser authentication.
- Deprecate interactive auth endpoints from the browser path.
- Map the Keycloak `sub` claim to the local `UserProfile` identity.
- Keep authorization policies based on Keycloak roles.

### Deliverables

- Backend authentication middleware configured for JWT bearer validation.
- Local identity mapping documented.
- Legacy browser auth endpoints marked deprecated or removed.

---

## Phase 3: SPA Authentication Integration

### Goal

Let the Angular frontend own sign-in, sign-out, token handling, and route protection.

### Tasks

- Integrate the SPA with Keycloak using OIDC Authorization Code Flow + PKCE.
- Add login and logout actions in the frontend.
- Handle token refresh in the SPA.
- Add auth state management for the UI.
- Protect routes with guards.
- Send access tokens on API requests.
- Surface role-based UI states where needed.

### Deliverables

- SPA login and logout wired to Keycloak.
- Route guards and auth state implemented.
- API requests include Bearer tokens.

---

## Phase 4: Keycloak Theme and UI Alignment

### Goal

Customize the Keycloak UI so it visually matches the SimpleECommerce frontend.

### Design Direction

Base the Keycloak theme on the frontend identity:

- Use a clean, lightweight layout.
- Match the brand tone of the Angular app.
- Prefer the frontend typography style, such as `Plus Jakarta Sans`.
- Reuse the frontend visual language: slate text, sky-blue accents, soft borders, and restrained gradients.
- Keep the login experience minimal and e-commerce oriented.
- Apply the same brand mark treatment and spacing rhythm used in the frontend shell.

### Tasks

- Create a custom Keycloak login theme.
- Update colors, typography, buttons, inputs, and spacing to align with the frontend UI.
- Add a branded header or logo treatment.
- Customize error, login, registration, and reset-password screens.
- Ensure responsive behavior on mobile and desktop.
- Keep accessibility intact.

### Deliverables

- Custom Keycloak theme files.
- Theme screenshots or preview notes.
- Shared design tokens or CSS variable mapping if needed.

---

## Phase 5: Profile Sync and Authorization Rules

### Goal

Keep local business data in sync without taking ownership of browser authentication.

### Tasks

- Create or update local `UserProfile` records after the first authenticated API call.
- Map Keycloak roles to backend authorization policies.
- Store only local business data in the backend.
- Use service accounts only for admin automation if required.
- Keep registration and account management in Keycloak.

### Deliverables

- Profile sync behavior documented.
- Authorization policy mapping documented.

---

## Phase 6: Testing, Deployment, and Cleanup

### Goal

Validate the new flow end to end and remove the old assumptions.

### Tasks

- Test browser login through Keycloak and token use against the API.
- Verify backend rejects invalid signatures, issuers, audiences, and expired tokens.
- Verify role-based authorization.
- Verify Keycloak theme rendering.
- Update Docker Compose and environment variables.
- Remove old auth assumptions from backend docs and code comments.
- Keep only the support flows that still make sense for the new architecture.

### Deliverables

- End-to-end authentication flow verified.
- Theme validated.
- Old backend-auth flow retired.

---

## Out of Scope

- Backend-managed password storage.
- Custom JWT issuance by the backend.
- Backend-proxy login and refresh for browser users.
- Direct access grants for SPA login.
- Storing client secrets in the SPA.

---

## Success Criteria

- The SPA signs in directly against Keycloak.
- The backend validates Keycloak tokens with public keys or JWKS.
- The backend no longer owns browser authentication flow.
- Keycloak UI matches the SimpleECommerce frontend direction.
- Role-based authorization works end to end.
- Local business profile data still syncs correctly.

---

## Notes

If you want a full implementation doc set again, create separate documents only for the phases that remain after this refactor. The old phase files were removed because they described the wrong architecture.
