# Phase 1: Realm and Client Setup

## Goal

Set up Keycloak from the Admin Console so the browser uses a public SPA client and the backend only validates tokens.

## Status

Completed.

## Summary

This phase configures the identity foundation for the new flow:

1. The Angular SPA redirects the browser to Keycloak.
2. Keycloak authenticates the user with the authorization-code flow and PKCE.
3. Keycloak returns tokens to the SPA.
4. The backend validates those tokens as a resource server.

## What You Will Configure in the Admin Console

- A realm for the application.
- Realm roles for customer, seller, and admin access.
- A public SPA client for browser sign-in.
- Backend audience mapping so the API can validate tokens.
- Optional user registration and account recovery settings.
- Optional test users and role assignments for validation.

## Prerequisites

- Keycloak 24 or later is running.
- You can access the Keycloak Admin Console.
- The realm name is decided, for example `SimpleECommerce`.
- The Angular app redirect URIs are known.
- The backend API audience/client ID is known.

## Step-by-Step Setup

### Step 1: Open the Keycloak Admin Console

- Start Keycloak.
- Open the admin console in the browser.
- Sign in with the admin account.
- Confirm you are in the correct Keycloak instance before making changes.

### Step 2: Create or select the realm

- Use the realm selector in the top-left of the admin console.
- If the `SimpleECommerce` realm does not exist, create it.
- If it already exists, open it.
- Confirm the realm display name and locale settings if you need them.

### Step 3: Configure realm-level login settings

- In the left menu, open **Realm settings**.
- Open the **Login** tab.
- Set **User registration** to **On** if you want users to create accounts themselves.
- Keep **Forgot password** on if users should be able to recover access without admin help.
- Keep **Remember me** off unless the business wants longer-lived browser sessions.
- In **Email settings**, keep **Email as username** on if you want users to log in with email addresses.
- Keep **Login with email** on if email should be accepted on the login screen.
- Keep **Verify email** on if accounts must confirm ownership before sign-in is considered complete.
- In **User info settings**, keep **Edit username** on only if users are allowed to change their username after account creation.
- Leave the rest of the Login tab at the default values unless your product rules require a change.
- Keep these settings aligned with the product flow, not with backend-owned login.

#### If the user chooses their role during registration

- Let the signup form ask the user to choose an **account type**.
- Use `customer` and `seller` as the available choices in the app.
- Store the selected choice in your application flow.
- If the user chooses `customer`, let registration finish normally.
- If the user chooses `seller`, let registration finish normally as well, but tag the account with the selected role so the backend can authorize it later.
- Do not use Keycloak default roles to make this choice for the user.
- Leave Keycloak default roles empty unless you intentionally want a universal default.

### Step 4: Create realm roles

- In the left menu, open **Roles**.
- Stay on the **Realm roles** tab.
- Click **Create role**.
- Enter `customer` as the role name.
- Add a short description if you want, such as `Default shopper account`.
- Click **Save**.
- Click **Create role** again.
- Enter `seller` as the role name.
- Add a description such as `Marketplace seller account`.
- Click **Save**.
- Click **Create role** again.
- Enter `admin` as the role name.
- Add a description such as `Administrative account`.
- Click **Save**.
- Keep the role names stable, because the backend authorization policies will use them.
- Avoid adding backend-specific roles unless you truly need a separate machine-to-machine role.

### Step 5: Create the public SPA client

- Open the Clients section.
- Create a new OpenID Connect client, for example `simple-e-commerce-frontend`.
- Set the client type to public.
- Turn client authentication off.
- Enable standard flow.
- Keep direct access grants off.
- Keep implicit flow off.
- On the client Settings page, open **Capability config** and set the **PKCE method** to **S256** if the field is available.
- If you do not see PKCE during client creation, finish creating the client first and then check the saved client settings page.
- Save the client.

### Step 6: Configure SPA client URLs

- Open the SPA client settings and fill the fields like this.

#### Frontend SPA client values

| Field                           | What to enter                                                                                                           | Why                                                                       |
| ------------------------------- | ----------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------- |
| Root URL                        | Leave empty, or set the main frontend URL if you want a base link in the admin console.                                 | The SPA does not need a server root URL for login.                        |
| Home URL                        | Leave empty, or set the main frontend landing page such as `http://localhost:4200/`.                                    | Optional convenience value for navigation.                                |
| Valid redirect URIs             | Add the exact frontend callback origins and paths, for example `http://localhost:4200/*` and `http://localhost:4201/*`. | Keycloak must allow the browser to return to the Angular app after login. |
| Valid post logout redirect URIs | Add the same frontend URLs used after logout, for example `http://localhost:4200/*` and `http://localhost:4201/*`.      | Keycloak must allow the browser to return after logout.                   |
| Web origins                     | Add the exact frontend origins, for example `http://localhost:4200` and `http://localhost:4201`.                        | This enables browser requests from the SPA to Keycloak.                   |
| Admin URL                       | Leave empty.                                                                                                            | A public SPA does not need an admin callback URL.                         |

- Include every local frontend port you actually run, such as `4200` and `4201` if both are used.
- Add your production frontend domain or tunnel domain in the same fields when you deploy.
- Keep the URI list as narrow as practical.

### Step 7: Configure audience and token content

1. Open **Clients** in the left menu.
2. Select the SPA client you created earlier.
3. Open the **Client scopes** tab.
4. If you are still on the realm-level **Client scopes** page, switch to the SPA client first; the **Dedicated scope** row only appears inside the selected client.
5. Open the **Dedicated scope** row, then go to the **Mappers** tab inside that client scope.
6. Click **Configure a new mapper**.
7. In the mapper list, choose **Audience**.
8. Fill the mapper form like this:
   1. **Name**: enter the audience name you want to appear in the token, for example `simple-e-commerce-backend`.
   2. **Included Client Audience**: leave empty unless you created a separate Keycloak client for the backend.
   3. **Included Custom Audience**: enter the backend API identifier if the backend is not a Keycloak client, for example `simple-e-commerce-backend`.
   4. **Add to ID token**: leave **Off** unless the frontend also needs the audience in the ID token.
   5. **Add to access token**: set **On** so the backend can validate the audience.
   6. **Add to lightweight access token**: leave **Off**.
   7. **Add to token introspection**: set **On** if you want the audience to appear in introspection responses too.
9. Click **Save**.
10. Confirm the generated access token still includes the default OIDC claims, especially `sub`, because the backend uses that value to map the local user profile.
11. If you preview the token in **Evaluate**, click **Generated access token** on the right side.
12. The **Generated user info** panel is not the JWT, so it will not show `aud`.
13. In the access token view, the `aud` claim will still show the SPA client unless the audience mapper above is applied correctly. The backend is not a Keycloak scope; it is represented by the audience value you add here.

### Step 8: Create test users

1. Open **Users** in the left menu.
2. Click **Add user**.
3. Create a customer test user:
   1. Enter a username and email.
   2. Click **Save**.
   3. Open the **Credentials** tab.
   4. Set a password.
   5. Turn **Temporary** off if you want to avoid a forced password change on first login.
   6. Open the **Role mappings** tab.
   7. Click **Assign role**.
   8. Select the `customer` role.
   9. Click **Assign**.
4. Create a seller test user:
   1. Click **Add user** again.
   2. Enter a username and email.
   3. Click **Save**.
   4. Open the **Credentials** tab.
   5. Set a password.
   6. Turn **Temporary** off if needed.
   7. Open the **Role mappings** tab.
   8. Click **Assign role**.
   9. Select the `seller` role.
   10. Click **Assign**.
5. If you want an admin test user, repeat the same flow and assign the `admin` role only when you intentionally need admin access.

### Step 9: Verify the setup

1. In the SPA client, open the **Client scopes** tab and use **Evaluate** with the test user to preview the issued token.
2. Confirm the generated access token includes:
   1. The expected issuer.
   2. The backend audience.
   3. The `sub` claim.
   4. The expected realm roles, such as `customer` or `seller`.
3. Open the Angular app and sign in with the customer test user.
4. Repeat the sign-in with the seller test user.
5. Call a protected backend endpoint with the received bearer token.
6. Confirm the backend accepts valid tokens and rejects missing, expired, or invalid tokens.
7. If the backend rejects the token, return to Step 7 and check the audience mapper, then return to Step 8 and confirm the backend client is separate from the SPA client.

## Deliverables

- Realm configured.
- Public SPA client configured.
- Backend audience mapping documented.
- Redirect URIs and web origins documented.
- Test users created for validation.
- Token contents verified.

Phase 1 is complete once these deliverables are in place.

## Notes

- Use a separate confidential or service-account client only for backend automation if needed.
- Do not use direct access grants for the browser login path.
- Do not put any client secret in the SPA.
- Do not use Keycloak default roles to decide the business role when the user chooses between customer and seller.
- Keep the admin console configuration in sync with the frontend URLs and branding decisions.
