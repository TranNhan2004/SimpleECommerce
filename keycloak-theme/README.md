# Keycloak Theme

This folder contains the Keycloak login theme for SimpleECommerce.

## Languages

- `en`
- `vi`

## Theme Notes

- Theme name: `simple-ecommerce`
- Login theme directory: `keycloak-theme/login`
- The theme inherits from the built-in Keycloak login theme and overrides styling plus translated messages.
- The `messages/messages_en.properties` and `messages/messages_vi.properties` files provide the localized copy.

## Keycloak Realm Settings

To make the language switch work, enable internationalization in the realm and include both locales.

Important:

- The Keycloak Admin Console login page belongs to the `master` realm.
- If you are testing the admin console, enable internationalization and both locales on the `master` realm too.
- If only `SimpleECommerce` is configured, the admin-console login can still appear in English only.

Recommended locales:

- English
- Vietnamese

## Usage

If you use the root `docker-compose.yml`, the theme is already mounted into the Keycloak container at `/opt/keycloak/themes/simple-ecommerce`.

If you run Keycloak some other way, copy or mount this folder into the Keycloak themes directory, then select `simple-ecommerce` as the login theme in the realm settings.

## Enable It In Keycloak

1. Open the Keycloak Admin Console.
2. Select the `SimpleECommerce` realm.
3. Go to `Realm settings`.
4. Open the `Themes` tab.
5. Set `Login theme` to `simple-ecommerce`.
6. Enable `Internationalization`.
7. Add `en` and `vi` as supported locales.
8. Save the realm settings.

## Reload Notes

- If Keycloak is already running, recreate the container after adding the mount. A plain restart may keep the old container definition.
- Recommended command: `docker compose up -d --force-recreate keycloak`
- After changing theme files, refresh Keycloak or restart the container to make sure the new assets are loaded.

## Quick Language Test

If the switch does not appear, open the login page with a forced locale:

- English: `?kc_locale=en`
- Vietnamese: `?kc_locale=vi`

If `kc_locale=vi` still shows English, the current realm does not have Vietnamese enabled.

## Why You May Not See a Switch

- Keycloak only shows the language selector when the active realm has internationalization enabled and at least two locales configured.
- The admin-console login uses the `master` realm, so configuring only `SimpleECommerce` is not enough for that page.
- If you log out from the admin console and land on the `master` login page, you must configure `master` > `Realm settings` > `Localization` and `Themes`.
- If you want to test the storefront realm itself, use a login flow from the `SimpleECommerce` realm, not the admin console.

## Important

- The Keycloak Admin Console login page uses the `master` realm, not the `SimpleECommerce` realm.
- If you are testing by opening the admin console, you must set the `master` realm login theme too.
- The `simple-ecommerce` theme on `SimpleECommerce` only appears on login flows for that realm and its clients.
- The visible brand title in the login header is `Simple E-Commerce`.
