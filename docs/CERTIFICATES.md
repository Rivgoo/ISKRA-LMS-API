# Data Protection Certificates

## Why do we need a certificate?

Iskra uses ASP.NET Core Data Protection to secure sensitive data, such as:
-   **Authentication Cookies** (preventing session hijacking).
-   **Anti-Forgery Tokens** (preventing CSRF).

By default, the keys used to encrypt this data are stored in the Database (`data_protection_keys` table).
However, storing keys in plain text is a security risk. If an attacker gains read access to the database, they can decrypt all user sessions.

To mitigate this, we use an **X.509 Certificate** to encrypt the master keys before they are saved to the database (**Encryption at Rest**). This means even with full database access, the keys (and thus the cookies) remain unreadable without the certificate file.

## How to generate a certificate (Dev/Docker)

For development or internal environments, you can generate a self-signed certificate using OpenSSL.

### Command

Run the following in your terminal (Bash or PowerShell):

```bash
# 1. Generate Private Key and Certificate Request
openssl req -x509 -newkey rsa:4096 -sha256 -nodes \
  -keyout iskra.key \
  -out iskra.crt \
  -subj "/CN=IskraDataProtection" \
  -days 3650

# 2. Export to PFX (PKCS#12) format required by .NET
openssl pkcs12 -export \
  -out iskra_dp_keys.pfx \
  -inkey iskra.key \
  -in iskra.crt \
  -password pass:Secret123!
```

### Configuration

1.  Place the generated `iskra_dp_keys.pfx` in your project root (or mount it into your Docker container).
2.  Update `appsettings.json`:

```json
"DataProtection": {
  "CertificatePath": "iskra_dp_keys.pfx",
  "CertificatePassword": "Secret123!"
}
```