# GenSecure.Core

DSGVO-compliant AES-256-GCM envelope-encryption library for sensitive person records.
Designed for teams that synchronise genealogy data via Git / SVN and need cryptographic
access control and audit-safe deletion.

## Purpose

Implements `IPersonSecureStore` and `IMasterKeyBackupService` from `GenSecure.Contracts`.
Provides envelope encryption (per-person DEK wrapped with a DPAPI-protected master key),
a passphrase-based recovery mechanism, and plaintext mode for deceased / historical persons
that no longer require DSGVO protection.

## Key Features

- **AES-256-GCM** data encryption and DEK wrapping — authenticated, tamper-evident
- **DPAPI `CurrentUser`** — master key bound to the local Windows account; zero network exposure
- **PBKDF2-SHA256 recovery backup** (600 000 iterations) — machine-migration and disaster recovery
- **HMAC-SHA256 SID pseudonymisation** — raw Windows SIDs never written to disk;
  pepper derived via HKDF from master key prevents enumeration attacks on key files
- **Plaintext store mode** — human-readable JSON for deceased persons; no key file created
- **Crypto-deletion** (DSGVO Art. 17) — `SecureDelete` removes only the DEK; ciphertext
  becomes permanently unreadable without modifying or deleting the payload
- **Deterministic file names** — `SHA-256(personId)` hex; no identity information on disk

## Targets

`net9.0-windows`  *(DPAPI and Windows Identity APIs require Windows)*

## Getting Started

```csharp
// Register via DI (Microsoft.Extensions.DependencyInjection)
services.AddGenSecureStore(options =>
{
    options.RootDirectory = @"C:\MyApp\Store";
});
```

```csharp
// Inject and use
public class PersonService(IPersonSecureStore store)
{
    public void SaveLivingPerson(string sId, PersonDto dto)
        => store.Save(sId, dto);                               // Encrypted (default)

    public void SaveDeceasedPerson(string sId, PersonDto dto)
        => store.Save(sId, dto, StoreMode.Plaintext);          // Human-readable JSON

    public PersonDto GetPerson(string sId)
        => store.Load<PersonDto>(sId);                         // Auto-detects mode

    public void DeleteForGdpr(string sId)
        => store.Delete(sId, DeleteMode.SecureDelete);         // Crypto-deletion
}
```

## File Layout

```
<RootDirectory>/
├── data/
│   ├── <sha256(personId)>.person.json   ← AES-256-GCM ciphertext  OR  plaintext JSON
├── keys/
│   ├── <sha256(personId)>.key.json      ← Wrapped DEK + HMAC-SHA256 SID hashes
└── master/
    ├── local-master.bin                 ← DPAPI-protected master key  [NEVER publish]
    └── recovery-key.json                ← PBKDF2+AES-GCM backup      [NEVER publish]
```

> **Safe to publish:** only `data/*.person.json` (encrypted records — ciphertext is
> computationally secure; note `PersonId` field is still plaintext).  
> **Never publish:** `keys/`, `master/`.

## Cryptographic Stack

| Layer | Algorithm | Details |
|-------|-----------|---------|
| Payload encryption | AES-256-GCM | 12-byte nonce, 16-byte tag, per-person random DEK |
| DEK wrapping | AES-256-GCM | Master key as wrapping key |
| Master key storage | DPAPI `CurrentUser` | `ProtectedData.Protect` / `Unprotect` |
| Recovery backup | PBKDF2-SHA256 + AES-256-GCM | 600 000 iterations, random 32-byte salt |
| SID hashing | HMAC-SHA256 | Key = HKDF-SHA256(masterKey, "GenSecure-SID-Pepper") |
| File naming | SHA-256(personId) | Hex, lowercase — no identity leak |

## Architecture

```
GenSecure.Contracts          ← Public API surface (interfaces, enums)
GenSecure.Core
  ├── PersonSecureStore       ← IPersonSecureStore implementation
  ├── MasterKeyBackupService  ← IMasterKeyBackupService implementation
  ├── CryptoUtilities         ← AES-GCM, HKDF, HMAC-SHA256, PBKDF2 helpers (internal)
  ├── FileModels              ← Internal JSON file models (internal)
  ├── GenSecureStoreOptions   ← Configuration (RootDirectory, file name conventions)
  ├── DependencyInjectionExtensions ← AddGenSecureStore() extension
  └── WindowsIdentityUtilities ← GetCurrentUserSid() helper (internal)
```

## Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| `Microsoft.Extensions.DependencyInjection` | 10.x | DI registration |
| `System.Security.Cryptography.ProtectedData` | 9.x | DPAPI on Windows |

## Contributing

- All crypto code lives in `CryptoUtilities` — keep it `internal`.
- New store operations must go through `EnsureAccessAllowed` before touching keys.
- Every new feature requires a corresponding test in `GenSecure.Core.Tests`.
- Raw SIDs must never be passed to any file-writing code path — always call
  `CryptoUtilities.ToSidHash(sSid, arrSidPepperKey)` first.
