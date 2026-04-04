# GenSecure.Contracts

Public interface contracts and enums for the GenSecure encrypted person store.
No implementation dependency — reference from any application layer without pulling in crypto code.

## Purpose

Defines the stable public API surface for DSGVO-compliant encrypted storage of person records.
Keeps the implementation (`GenSecure.Core`) behind an abstraction so application code can be
tested with mocks and swapped without recompilation.

## Key Features

- `IPersonSecureStore` — save, load, delete, exists, and access-control operations
- `IMasterKeyBackupService` — PBKDF2 recovery key creation and restore
- `DeleteMode` — `SoftDelete` (removes files) vs. `SecureDelete` (crypto-deletion, DSGVO Art. 17)
- `StoreMode` — `Encrypted` (AES-256-GCM, living persons) vs. `Plaintext` (deceased / historical)

## Targets

`net9.0-windows`

## Public API

```csharp
// Store / retrieve a person record
void Save<T>(string sPersonId, T value, StoreMode eStoreMode = StoreMode.Encrypted);
T    Load<T>(string sPersonId);
bool Exists(string sPersonId);

// Deletion
void Delete(string sPersonId, DeleteMode eMode);
//   DeleteMode.SoftDelete   — removes data file + key file
//   DeleteMode.SecureDelete — removes only the DEK (crypto-deletion); plaintext fallback: data file

// Access control
void GrantAccess(string sPersonId, string sWindowsSid);
IReadOnlyCollection<string> GetAllowedWindowsSidHashes(string sPersonId);

// Recovery
void CreateRecoveryKeyBackup(string sPassphrase, bool xOverwrite = false);
bool TryRestoreLocalMasterKey(string sPassphrase);
```

## Architecture

Contracts-only project: no NuGet dependencies, no implementation code.
Intended to be the only reference needed by ViewModel / application projects.
