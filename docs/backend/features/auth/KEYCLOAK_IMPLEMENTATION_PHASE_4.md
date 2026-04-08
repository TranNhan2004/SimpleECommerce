# Phase 4: Keycloak Theme and UI Alignment

## Goal

Customize the Keycloak UI so it visually matches the SimpleECommerce frontend.

## Summary

This phase makes the Keycloak login, registration, and recovery screens feel like part of the same product as the Angular app.

Theme files live in [keycloak-theme](../../../../keycloak-theme). The login theme is localized with English and Vietnamese message bundles.

The frontend visual language to mirror is:

- Font family: `Plus Jakarta Sans`, then `Segoe UI`, then sans-serif fallback.
- Backgrounds: white to soft sky-red gradients.
- Accent colors: sky blue, slate text, restrained violet/rose highlights.
- Brand treatment: a compact gradient brand mark and uppercase tagline.
- Shape language: rounded-sm cards, soft borders, and light shadows.

## Prerequisites

- Phase 1 completed.
- Frontend style tokens are known from `src/styles.css`.
- The desired brand text is known from the frontend i18n files.

## Design Direction

- Keep the login page clean and focused.
- Match the e-commerce brand tone, not a generic admin-console look.
- Reuse the same spacing rhythm and header feel from the frontend shell.
- Make the login card feel like a storefront companion, not a separate system.

## Implementation Steps

### 1. Create a custom theme

- Create a Keycloak login theme directory at `keycloak-theme/login`.
- Add a theme name for the project, for example `simple-ecommerce`.
- Keep templates and assets isolated from the default Keycloak theme.
- Provide localized message bundles for `en` and `vi` under `messages/`.

### 2. Align typography

- Use `Plus Jakarta Sans` as the primary typeface.
- Keep headings bold and compact.
- Keep body text easy to scan.

### 3. Align colors and surfaces

- Use white cards with soft slate borders.
- Use sky-blue for primary actions and focus states.
- Keep gradients restrained and brand-safe.
- Match button and input treatment to the frontend style system.

### 4. Align layout and spacing

- Use a centered card layout.
- Keep login actions grouped and obvious.
- Keep the form responsive on mobile.
- Match button height, border radius, and spacing proportions to the Angular app.

### 5. Customize all user-facing screens

- Update login.
- Update registration.
- Update password reset.
- Update error and info pages.
- Add branding to the top of the form.

### 6. Test the theme

- Verify the theme loads in development.
- Verify responsive behavior.
- Verify accessibility and contrast.
- Verify the experience matches the frontend direction on both desktop and mobile.

## Deliverables

- Custom Keycloak theme files.
- Theme preview screenshots.
- Theme assets and styling notes.
- English and Vietnamese message bundles.

## Notes

- Keep the theme minimal and product-oriented.
- Do not copy the Angular app wholesale; translate its design language into the Keycloak screens.
