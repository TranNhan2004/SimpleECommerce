# Task 06: Testing, Swagger, And Cleanup

## Goal

Verify the backend-first BFF flow and retire the leftover SPA-first assumptions from the API surface and docs.

## Code Areas Expected To Change

- API tests around authentication extensions and auth controller flow
- Swagger or OpenAPI auth notes
- Old auth comments, placeholders, and mismatched assumptions

## Work Items

1. Add backend tests for:
   - option binding
   - login redirect generation
   - callback state validation
   - token exchange failure handling
   - Redis session creation and revocation
   - cookie issuance
   - logout and session invalidation
   - CSRF rejection on unsafe requests
2. Decide how Swagger should represent the new browser flow:
   - document that browser users authenticate through backend login endpoints
   - keep bearer auth only if service-to-service callers still use it
3. Remove or rewrite old auth documentation and comments that describe SPA-owned tokens as the main flow.
4. Validate local run steps against actual ports and environment files.

## Keycloak UI Changes

1. Open `Users`.
2. Create or verify at least these test users:
   - customer user
   - seller user
   - admin user
3. Confirm the users have the identity claims the backend expects, especially `sub`, `email`, and `preferred_username`.
4. Run a manual login through the browser for each user and confirm the resulting backend session behaves as expected.
5. Verify application authorization from your own database or seeded data rather than from Keycloak role mapping.

## Acceptance Criteria

- Automated tests cover the core backend BFF flow.
- Manual verification succeeds for login, authenticated API use, and logout.
- Swagger and docs no longer imply the browser should send bearer tokens directly.
- The auth feature folder reflects the BFF design only.

## Notes

- Keep test documentation explicit about which local URL is canonical for backend login.
- If bearer auth remains for automation, separate that documentation from the browser auth path.
- Make sure tests cover the case where Redis session state is missing or expired while the browser still sends cookies.
