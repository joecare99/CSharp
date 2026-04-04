# GenSecure.Demo

WPF 4-tab show-off application for `GenSecure.Core` — demonstrates every public capability
of the encrypted person store in an interactive UI.

## Purpose

Provides a hands-on demonstration of AES-256-GCM envelope encryption, DPAPI key protection,
PBKDF2 recovery backup, HMAC-SHA256 access control, and plaintext mode for deceased persons.
Useful for evaluating the library and for presenting the security architecture.

## Key Features

- **Tab 1 – Personen-Datensatz:** Enter person data, choose `Encrypted` or `Plaintext` store
  mode, save/load/delete with full status logging.
- **Tab 2 – Schlüssel-Backup:** Create and restore the PBKDF2-SHA256 master-key recovery
  backup via `PasswordBox` (bridge pattern for MVVM).
- **Tab 3 – Zugriffsrechte:** Grant Windows SID access; display all allowed
  HMAC-SHA256 SID hashes stored in the key file.
- **Tab 4 – Rohdaten:** Inspect the raw `data/*.person.json`, `keys/*.key.json`, and
  `master/local-master.bin` files on disk.

## Targets

`net9.0-windows`

## Architecture

- **MVVM** via `CommunityToolkit.Mvvm` 8.x (`[ObservableProperty]`, `[RelayCommand]`)
- **DI** via `Microsoft.Extensions.DependencyInjection` — bootstrapped in `App.xaml.cs`
- `PasswordBox` values forwarded from code-behind (`MainWindow.xaml.cs`) to
  `MainViewModel.CreateRecoveryBackup` / `RestoreFromRecovery` (no MVVM binding needed)
- Store root defaults to `%TEMP%\GenSecureDemo`; selectable at runtime via folder picker

## Running

```bash
cd Gen_FreeWin/GenSecure.Demo
dotnet run
```
