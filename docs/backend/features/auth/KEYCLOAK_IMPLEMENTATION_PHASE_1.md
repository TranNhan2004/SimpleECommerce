# Phase 1: Keycloak Setup

**Status**: ✅ Completed  
**Duration**: 1-2 days  
**Phase Overview**: [KEYCLOAK_IMPLEMENTATION_PLAN.md](./KEYCLOAK_IMPLEMENTATION_PLAN.md)

---

## Table of Contents

1. [Objectives](#objectives)
2. [Prerequisites](#prerequisites)
3. [Implementation Steps](#implementation-steps)
   - [Step 1.1: Install Keycloak with Docker](#step-11-install-keycloak-with-docker)
   - [Step 1.2: Create Realm](#step-12-create-realm)
   - [Step 1.3: Configure Realm Settings](#step-13-configure-realm-settings)
   - [Step 1.4: Create Client](#step-14-create-client)
   - [Step 1.5: Create Client Scopes](#step-15-create-client-scopes)
   - [Step 1.6: Create Realm Roles](#step-16-create-realm-roles)
   - [Step 1.7: Create Test Users](#step-17-create-test-users)
   - [Step 1.8: Test Keycloak Setup](#step-18-test-keycloak-setup)
   - [Step 1.9: Configure for Development + Production](#step-19-configure-for-development--production-optional)
4. [Verification Checklist](#verification-checklist)
5. [Troubleshooting](#troubleshooting)
6. [Next Steps](#next-steps)

---

## Objectives

- ✅ Set up local Keycloak instance using Docker
- ✅ Configure realm with authentication features
- ✅ Create confidential client for backend API
- ✅ Configure role-based access control
- ✅ Create test users with different roles
- ✅ Validate Keycloak installation and token generation

---

## Prerequisites

- [x] Docker Desktop installed and running
- [x] Docker Compose available
- [x] Postman or equivalent API testing tool
- [x] Internet connection for pulling Docker images
- [x] Basic understanding of OAuth2 and OpenID Connect

---

## Implementation Steps

### Step 1.1: Install Keycloak with Docker

#### Update Docker Compose File

The Keycloak services have already been added to your existing `docker-compose.yml` file in the project root. The configuration includes:

- **keycloak-db**: PostgreSQL 16 database for Keycloak data
- **keycloak**: Keycloak 24.0 server in development mode

**Key Configuration Details:**

- Keycloak will run on port `8080` (accessible from localhost)
- PostgreSQL will run on port `5433` on localhost (optional - only for direct database access/debugging)
  - Keycloak connects internally using Docker network (`keycloak-db:5432`)
  - Port 5433 exposure is useful for tools like pgAdmin but not required for Keycloak
- Default admin credentials: `admin` / `admin`
- Data will be persisted in the `keycloak_data` volume
- The API service (currently commented out) has been configured to depend on Keycloak

**Review the configuration** in `docker-compose.yml`:

```yaml
keycloak-db:
  image: postgres:16-alpine
  container_name: keycloak-postgres
  environment:
    POSTGRES_DB: keycloak
    POSTGRES_USER: keycloak
    POSTGRES_PASSWORD: keycloak_password
  volumes:
    - keycloak_data:/var/lib/postgresql/data
  ports:
    - "5433:5432"

keycloak:
  image: quay.io/keycloak/keycloak:24.0
  container_name: keycloak
  environment:
    KC_DB: postgres
    KC_DB_URL: jdbc:postgresql://keycloak-db:5432/keycloak
    KC_DB_USERNAME: keycloak
    KC_DB_PASSWORD: keycloak_password
    KEYCLOAK_ADMIN: admin
    KEYCLOAK_ADMIN_PASSWORD: admin
    KC_HOSTNAME: localhost
    KC_HOSTNAME_PORT: 8080
    KC_HTTP_ENABLED: true
    KC_HOSTNAME_STRICT_HTTPS: false
  command: start-dev
  ports:
    - "8080:8080"
  depends_on:
    - keycloak-db
```

#### Start Keycloak

```bash
# Navigate to project root
cd /home/nhan/Workspace/LearnCSharp/SimpleECommerce

# Start all services (including Keycloak)
docker-compose up -d

# Or start only Keycloak services
docker-compose up -d keycloak-db keycloak

# Check container status
docker ps

# View logs (optional)
docker-compose logs -f keycloak
```

**💡 Tip**: To stop Keycloak services when not in use:

```bash
# Stop only Keycloak services
docker-compose stop keycloak keycloak-db

# Or stop all services
docker-compose down
```

#### Verify Installation

1. Open browser: `http://localhost:8080`
2. Wait for Keycloak to fully start (may take 1-2 minutes)
3. Click on "Administration Console"
4. Login with credentials:
   - Username: `admin`
   - Password: `admin`

**✅ Checkpoint**: You should see the Keycloak Admin Console

---

### Step 1.2: Create Realm

#### Steps

1. In the Keycloak Admin Console, hover over the realm dropdown (currently showing "master")
2. Click **Create Realm** button
3. Fill in realm details:
   - **Realm name**: `SimpleECommerce`
   - **Enabled**: ON
4. Click **Create**

**✅ Checkpoint**: You should now see "SimpleECommerce" in the realm dropdown

---

### Step 1.3: Configure Realm Settings

#### General Settings

1. Navigate to: **Realm settings** in the left sidebar
2. **General** tab:
   - Display name: `SimpleECommerce Application`
   - HTML Display name: `<b>SimpleECommerce</b>`
   - Frontend URL: Leave empty for now (will be set to production URL later)
3. Click **Save**

**💡 For Production**: When deploying with Cloudflare tunnel:

- Set **Frontend URL** to your tunnel domain (e.g., `https://your-domain.com`)
- This ensures Keycloak uses the correct URL in redirects and token validation

#### Login Settings

1. Navigate to: **Realm settings** → **Login** tab
2. Configure the following:
   - ☑️ **User registration**: ON
   - ☑️ **Forgot password**: ON
   - ☑️ **Remember me**: ON
   - ☑️ **Email as username**: ON
   - ☑️ **Login with email**: ON
   - ☐ **Verify email**: OFF (enable in production)
   - **Require SSL**: External requests
3. Click **Save**

#### Email Settings (Optional for Production)

1. Navigate to: **Realm settings** → **Email** tab
2. Configure SMTP settings:
   ```
   Host: smtp.gmail.com
   Port: 587
   From: your-email@gmail.com
   Username: your-email@gmail.com
   Password: your-app-password
   Enable StartTLS: ON
   ```
3. Click **Save**
4. Click **Test connection** to verify

**💡 Note**: Email configuration is optional for development but required for production features like password reset and email verification.

**✅ Checkpoint**: Realm settings should be configured

---

### Step 1.4: Create Client

#### Create New Client

1. Navigate to: **Clients** in the left sidebar
2. Click **Create client** button

#### Step 1: General Settings

- **Client type**: `OpenID Connect`
- **Client ID**: `simple-e-commerce-backend`
- **Name**: `SimpleECommerce Backend API`
- **Description**: `Backend API client for SimpleECommerce`
- **Always display in console**: OFF
- Click **Next**

#### Step 2: Capability Config

- **Client authentication**: **ON** (this makes it a confidential client)
- **Authorization**: **OFF**
- **Authentication flow**:
  - ☑️ **Standard flow**: ON
  - ☑️ **Direct access grants**: ON
  - ☐ **Implicit flow**: OFF
  - ☑️ **Service accounts roles**: **ON** ⚠️ (Important for admin operations)
- Click **Next**

#### Step 3: Login Settings

Configure URLs for both development and production environments:

- **Root URL**: `http://localhost:5000`
- **Home URL**: `http://localhost:5000`
- **Valid redirect URIs**:
  - `http://localhost:5000/*` (Backend dev)
  - `http://localhost:4200/*` (Frontend dev - if using ng serve)
  - `http://localhost:4201/*` (Frontend dev - Docker)
  - `https://your-domain.com/*` (Production - replace with your actual tunnel domain)
  - `https://*.your-domain.com/*` (Production subdomains, if needed)
- **Valid post logout redirect URIs**:
  - `http://localhost:5000/*`
  - `http://localhost:4200/*`
  - `http://localhost:4201/*`
  - `https://your-domain.com/*`
- **Web origins**:
  - `http://localhost:5000`
  - `http://localhost:4200`
  - `http://localhost:4201`
  - `https://your-domain.com`
  - Or simply use `+` to allow CORS from all valid redirect URIs
- Click **Save**

**💡 Production Setup**: Once you have your Cloudflare tunnel domain:

1. Come back to this client configuration
2. Replace `https://your-domain.com/*` with your actual tunnel domain
3. Update your frontend's environment configuration to use the tunnel domain

**🔒 Security Best Practice**:

- Use specific URLs instead of wildcards when possible
- Remove localhost URLs from production Keycloak instances
- Keep separate Keycloak clients for dev and production if needed

#### Retrieve Client Secret

1. After saving, navigate to the **Credentials** tab
2. Copy the **Client Secret** value
3. **⚠️ IMPORTANT**: Save this secret securely - you'll need it for backend configuration

**✅ Checkpoint**: Client created with secret retrieved

**💡 Frontend Client (Optional for Phase 1, Required Later)**:

If your frontend will authenticate users directly (rather than through the backend), you should create a separate **public client** for the frontend:

1. Create another client: `simple-e-commerce-frontend`
2. **Client authentication**: **OFF** (public client - no secret)
3. **Authentication flow**:
   - ☑️ **Standard flow**: ON
   - ☐ **Direct access grants**: OFF (frontend should use standard flow)
4. **Valid redirect URIs**:
   - `http://localhost:4200/*`
   - `http://localhost:4201/*`
   - `https://your-domain.com/*`
5. **Valid post logout redirect URIs**: Same as redirect URIs
6. **Web origins**: `+`

This allows your Angular frontend to use Keycloak's login page for authentication.

---

### Step 1.5: Create Client Scopes

#### Create Roles Scope

1. Navigate to: **Client scopes** in the left sidebar
2. Click **Create client scope** button
3. Fill in details:
   - **Name**: `simple-e-commerce-roles`
   - **Type**: `Default`
   - **Protocol**: `OpenID Connect`
   - **Display on consent screen**: OFF
   - **Include in token scope**: ON
4. Click **Save**

#### Add Mapper for Realm Roles

1. In the `simple-e-commerce-roles` scope, go to the **Mappers** tab
2. Click **Add mapper** → **By configuration**
3. Select **User Realm Role**
4. Configure mapper:
   - **Name**: `realm-roles`
   - **Multivalued**: ON
   - **Token Claim Name**: `roles`
   - **Claim JSON Type**: `String`
   - **Add to ID token**: ON
   - **Add to access token**: ON
   - **Add to userinfo**: ON
5. Click **Save**

#### Assign Scope to Client

1. Navigate to: **Clients** → `simple-e-commerce-backend`
2. Go to **Client scopes** tab
3. Click **Add client scope** button
4. Select `simple-e-commerce-roles` from the list
5. Click **Add** → **Default**

**✅ Checkpoint**: Roles scope created and assigned to client

---

### Step 1.6: Create Realm Roles

#### Create Roles

1. Navigate to: **Realm roles** in the left sidebar
2. Click **Create role** button

#### Role 1: Customer

- **Role name**: `customer`
- **Description**: `Regular customer with shopping permissions`
- Click **Save**

#### Role 2: Seller

- **Role name**: `seller`
- **Description**: `Seller user with product management permissions`
- Click **Save**

#### Role 3: Admin

- **Role name**: `admin`
- **Description**: `Administrator with full system access`
- Click **Save**

**✅ Checkpoint**: Three realm roles created

---

### Step 1.7: Create Test Users

#### User 1: Admin User

1. Navigate to: **Users** in the left sidebar
2. Click **Add user** button
3. **User details**:
   - **Username**: `admin@test.com`
   - **Email**: `admin@test.com`
   - **First name**: `Admin`
   - **Last name**: `User`
   - **Email verified**: ON
   - **Enabled**: ON
4. Click **Create**

5. **Set Password**:
   - Go to **Credentials** tab
   - Click **Set password**
   - **Password**: `Admin@123`
   - **Password confirmation**: `Admin@123`
   - **Temporary**: OFF
   - Click **Save**
   - Confirm the action

6. **Assign Role**:
   - Go to **Role mapping** tab
   - Click **Assign role** button
   - Select `admin` from the list
   - Click **Assign**

#### User 2: Customer User

1. Click **Add user** button
2. **User details**:
   - **Username**: `customer@test.com`
   - **Email**: `customer@test.com`
   - **First name**: `Customer`
   - **Last name**: `User`
   - **Email verified**: ON
   - **Enabled**: ON
3. Click **Create**

4. **Set Password**: `Customer@123` (Temporary: OFF)
5. **Assign Role**: `customer`

#### User 3: Seller User

1. Click **Add user** button
2. **User details**:
   - **Username**: `seller@test.com`
   - **Email**: `seller@test.com`
   - **First name**: `Seller`
   - **Last name**: `User`
   - **Email verified**: ON
   - **Enabled**: ON
3. Click **Create**

4. **Set Password**: `Seller@123` (Temporary: OFF)
5. **Assign Role**: `seller`

**✅ Checkpoint**: Three test users created with appropriate roles

---

### Step 1.8: Test Keycloak Setup

#### Get Token Using Postman

Create a new POST request in Postman:

**Request Configuration:**

```
Method: POST
URL: http://localhost:8080/realms/SimpleECommerce/protocol/openid-connect/token
Headers:
  Content-Type: application/x-www-form-urlencoded
```

**Body (x-www-form-urlencoded):**

```
grant_type: password
client_id: simple-e-commerce-backend
client_secret: <YOUR_CLIENT_SECRET>
username: admin@test.com
password: Admin@123
scope: openid profile email simple-e-commerce-roles
```

**📝 Where to find the client secret:**

- You saved this in **Step 1.4** under "Retrieve Client Secret"
- If you didn't save it, go back to Keycloak Admin Console → **Clients** → `simple-e-commerce-backend` → **Credentials** tab → Copy the secret

**Send the request**

#### Expected Response

```json
{
  "access_token": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expires_in": 300,
  "refresh_expires_in": 1800,
  "refresh_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "token_type": "Bearer",
  "scope": "openid profile email simple-e-commerce-roles",
  "not-before-policy": 0,
  "session_state": "...",
  "id_token": "..."
}
```

#### Verify Token Contains Roles

1. Copy the `access_token` value from the response
2. Go to [https://jwt.io](https://jwt.io)
3. Paste the token in the "Encoded" section
4. In the "Decoded" section, verify the payload contains:

```json
{
  "exp": 1234567890,
  "iat": 1234567890,
  "sub": "...",
  "typ": "Bearer",
  "azp": "simple-e-commerce-backend",
  "preferred_username": "admin@test.com",
  "email": "admin@test.com",
  "email_verified": true,
  "given_name": "Admin",
  "family_name": "User",
  "roles": ["admin"],
  ...
}
```

**✅ Critical Checkpoint**: Verify that `roles` array contains `["admin"]`

**🔒 Security Note: Why can jwt.io decode tokens without a secret?**

You might notice that jwt.io can read your token without any secret key. This is **by design and completely secure**. Here's why:

**JWT Structure:**

- **Header + Payload**: BASE64 encoded (not encrypted) - anyone can decode and read
- **Signature**: Cryptographically signed - only the private key holder can create valid signatures

**Why this is secure:**

1. **JWT is for authentication, not encryption** - The token proves identity, not hides information
2. **The signature prevents tampering** - If someone changes the payload, the signature becomes invalid
3. **Only Keycloak can create valid tokens** - It uses its private key (RS256) to sign
4. **Your backend validates the signature** - Using Keycloak's public key to verify authenticity

**What this means:**

- ✅ **Anyone can READ** the token contents (email, roles, expiration)
- ❌ **Nobody can MODIFY** the token without invalidating the signature
- ❌ **Nobody can CREATE** valid tokens without Keycloak's private key

**Best Practices:**

- ⚠️ **Never put sensitive data in JWTs** (passwords, credit cards, SSN, etc.)
- ✅ **OK to include**: user ID, email, roles, permissions, expiration
- 🔒 **Sensitive data**: Fetch separately from backend using the token for authentication

**Example:**

```
Bad:  { "email": "user@example.com", "creditCard": "1234-5678-9012-3456" }
Good: { "email": "user@example.com", "roles": ["customer"], "userId": "123" }
```

If you need to transmit truly secret data, use HTTPS (which encrypts the entire communication, including the JWT in transit).

#### Test Other Users

Repeat the token request with the other test users:

**Customer User:**

```
username: customer@test.com
password: Customer@123
```

Expected roles: `["customer"]`

**Seller User:**

```
username: seller@test.com
password: Seller@123
```

Expected roles: `["seller"]`

**✅ Checkpoint**: All three users can authenticate and receive tokens with correct roles

---

### Step 1.9: Configure for Development + Production (Optional)

Since you're using Cloudflare Tunnel for production with a custom domain, here's how to configure Keycloak for both environments:

#### Development Configuration (Current)

Already configured in steps above:

- Keycloak: `http://localhost:8080`
- Backend: `http://localhost:5000`
- Frontend: `http://localhost:4200` or `http://localhost:4201`

#### Production Configuration (When Ready)

**1. Update Client Redirect URIs** (Step 1.4):

```
Valid redirect URIs:
- https://your-domain.com/*
- https://api.your-domain.com/*

Web origins:
- https://your-domain.com
- https://api.your-domain.com
```

**2. Update Realm Frontend URL** (Step 1.3):

```
Frontend URL: https://your-domain.com
```

**3. Cloudflare Tunnel Configuration**:

Your `cloudflared` service should proxy:

- `https://your-domain.com` → `http://frontend:80`
- `https://api.your-domain.com` → `http://backend:5000` (when API is ready)
- `https://auth.your-domain.com` → `http://keycloak:8080` (optional, if you want public Keycloak access)

**4. Environment Variables Strategy**:

Create separate environment files:

**Frontend** (`environment.ts` vs `environment.prod.ts`):

```typescript
// Development
export const environment = {
  production: false,
  apiUrl: "http://localhost:5000",
  keycloakUrl: "http://localhost:8080",
  keycloakRealm: "SimpleECommerce",
  keycloakClientId: "simple-e-commerce-frontend",
};

// Production
export const environment = {
  production: true,
  apiUrl: "https://api.your-domain.com",
  keycloakUrl: "https://auth.your-domain.com", // or use same domain with path
  keycloakRealm: "SimpleECommerce",
  keycloakClientId: "simple-e-commerce-frontend",
};
```

**Backend** (appsettings.Development.json vs appsettings.Production.json):

```json
// Development
{
  "Keycloak": {
    "auth-server-url": "http://localhost:8080/",
    "realm": "SimpleECommerce"
  }
}

// Production
{
  "Keycloak": {
    "auth-server-url": "https://auth.your-domain.com/",
    "realm": "SimpleECommerce"
  }
}
```

**5. Security Considerations**:

- ✅ Enable SSL/HTTPS for all production URLs
- ✅ Update `KC_HOSTNAME_STRICT_HTTPS: true` in production
- ✅ Remove localhost URLs from production client configuration
- ✅ Consider separate Keycloak realms or clients for dev/staging/prod

---

## Verification Checklist

Complete this checklist before proceeding to Phase 2:

- [x] Keycloak running on `http://localhost:8080`
- [x] Can access Keycloak Admin Console
- [x] `SimpleECommerce` realm created
- [x] Realm settings configured (user registration, forgot password, email as username)
- [x] Client `simple-e-commerce-backend` created
- [x] Client secret retrieved and stored securely
- [x] Service accounts enabled for client
- [x] `simple-e-commerce-roles` client scope created with realm role mapper
- [x] `simple-e-commerce-roles` scope assigned to client as default
- [x] Three realm roles created: `customer`, `seller`, `admin`
- [x] Three test users created with passwords set
- [x] Each user assigned appropriate role
- [x] Successfully obtained access token for admin user via Postman
- [x] Token contains `roles` claim with correct role
- [x] Verified token at jwt.io
- [x] Tested token generation for all three users

**🎉 Phase 1 Complete**: If all checkboxes are checked, you're ready for Phase 2!

---

## Troubleshooting

### Issue: Cannot access Keycloak at localhost:8080

**Solutions:**

- Check if containers are running: `docker-compose ps`
- Check container logs: `docker-compose logs keycloak`
- Ensure port 8080 is not used by another application
- Wait 1-2 minutes for Keycloak to fully start
- Restart Keycloak: `docker-compose restart keycloak`

### Issue: Cannot login to Admin Console

**Solutions:**

- Verify credentials are `admin` / `admin`
- Clear browser cache and cookies
- Try incognito/private browsing mode
- Check container environment variables in docker-compose file

### Issue: Token request returns 401 Unauthorized

**Solutions:**

- Verify client secret is correct
- Check username and password are correct
- Ensure user is enabled in Keycloak
- Verify client ID is `simple-e-commerce-backend`
- Check that "Direct access grants" is enabled for the client

### Issue: "Realm does not exist" error

This error occurs when the realm name in your token request URL doesn't match the realm in Keycloak.

**Solutions:**

1. **Verify realm was created** (Step 1.2):
   - Go to Keycloak Admin Console (`http://localhost:8080`)
   - Check the realm dropdown in the top-left corner
   - Should show `SimpleECommerce` (not just `master`)

2. **Check realm name spelling**:
   - Realm names are **case-sensitive**
   - Must be exactly: `SimpleECommerce` (capital S, capital E, no spaces)
   - Common mistakes: `simplee-commerce`, `simple-e-commerce`, `SimpleEcommerce`

3. **Verify token endpoint URL**:
   - Correct: `http://localhost:8080/realms/SimpleECommerce/protocol/openid-connect/token`
   - Make sure it's `/realms/` (plural) not `/realm/`
   - Make sure realm name matches exactly

4. **If realm doesn't exist**:
   - Go back to **Step 1.2** and create the `SimpleECommerce` realm
   - Ensure it's enabled
   - Try the token request again

**Quick test**: Open this URL in your browser:

```
http://localhost:8080/realms/SimpleECommerce/.well-known/openid-configuration
```

- If it returns JSON, the realm exists ✅
- If it returns an error, the realm doesn't exist or the name is wrong ❌

### Issue: Token doesn't contain roles claim

**Solutions:**

- Verify `roles` client scope is assigned to client as **Default** (not Optional)
- Check that role mapper is configured correctly
- Ensure user has role assigned in "Role mapping" tab
- Try generating a new token after fixing configuration

### Issue: Docker containers fail to start

**Solutions:**

- Ensure Docker Desktop is running
- Check available disk space
- Stop containers: `docker-compose down`
- Remove volumes: `docker volume rm simplee-commerce_keycloak_data` (if needed)
- Pull images again: `docker-compose pull keycloak keycloak-db`
- Start fresh: `docker-compose up -d keycloak-db keycloak`

---

## Next Steps

After completing Phase 1, proceed to:

**[Phase 2: Backend Configuration](./KEYCLOAK_IMPLEMENTATION_PHASE_2.md)**

In Phase 2, you will:

- Install Keycloak NuGet packages in the backend
- Configure appsettings.json with Keycloak settings
- Create KeycloakSettings configuration model
- Prepare for service implementation

---

_Phase 1 Last Updated: 2026-03-03_  
_Author: Development Team_
