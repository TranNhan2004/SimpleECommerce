# Phase 6: Testing, Deployment, and Cleanup

## Goal

Validate the new flow end to end and remove the old assumptions.

## Summary

This phase proves the new model works in practice:

1. Browser login goes directly to Keycloak.
2. The SPA receives and uses tokens.
3. The backend validates those tokens.
4. The Keycloak UI matches the frontend design language.

## Prerequisites

- Phase 1 completed.
- Phase 2 completed.
- Phase 3 completed.
- Phase 4 completed.
- Phase 5 completed.

## Implementation Steps

### 1. Test the browser flow

- Sign in from the SPA through Keycloak.
- Confirm the SPA receives tokens.
- Confirm protected API calls succeed with the access token.

### 2. Test token validation

- Verify the backend rejects invalid signatures.
- Verify the backend rejects the wrong issuer.
- Verify the backend rejects the wrong audience.
- Verify the backend rejects expired tokens.

### 3. Test authorization

- Verify role-based access for customer routes.
- Verify role-based access for seller routes.
- Verify role-based access for admin routes.

### 4. Test the Keycloak theme

- Verify the login screen matches the Angular app styling direction.
- Verify the theme on mobile and desktop.
- Verify brand text, typography, buttons, and spacing.

### 5. Clean up legacy assumptions

- Remove old auth assumptions from code comments.
- Keep only token-validation and profile-sync logic.
- Remove obsolete environment variables and docs references.

### 6. Update deployment config

- Update Docker Compose if the frontend and Keycloak need different public URLs.
- Update environment values for issuer, audience, redirect URIs, and theme name.
- Keep secrets out of the SPA.

## Deliverables

- End-to-end flow verified.
- Theme validated.
- Legacy backend-auth assumptions removed.

## Notes

- This phase closes the migration by proving the browser no longer depends on the backend for login.
