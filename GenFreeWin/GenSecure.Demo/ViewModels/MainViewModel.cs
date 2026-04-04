using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GenSecure.Contracts;
using GenSecure.Core;

namespace GenSecure.Demo.ViewModels;

/// <summary>
/// Main view model for the GenSecure show-off application.
/// Demonstrates all capabilities of <see cref="IPersonSecureStore"/>
/// and <see cref="IMasterKeyBackupService"/>.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly IPersonSecureStore _store;
    private readonly IMasterKeyBackupService _backupService;
    private readonly GenSecureStoreOptions _options;

    // ── Store configuration ───────────────────────────────────────────────────

    /// <summary>Gets or sets the storage root directory path.</summary>
    [ObservableProperty]
    private string _rootDirectory = string.Empty;

    // ── Person input ──────────────────────────────────────────────────────────

    /// <summary>Gets or sets the stable person identifier.</summary>
    [ObservableProperty]
    private string _personId = "P001";

    /// <summary>Gets or sets the first name of the demo person.</summary>
    [ObservableProperty]
    private string _firstName = "Anna";

    /// <summary>Gets or sets the last name of the demo person.</summary>
    [ObservableProperty]
    private string _lastName = "Mustermann";

    /// <summary>Gets or sets the birth date (ISO 8601) of the demo person.</summary>
    [ObservableProperty]
    private string _birthDate = "1990-05-15";

    /// <summary>Gets or sets the address of the demo person.</summary>
    [ObservableProperty]
    private string _address = "Musterstraße 42, 12345 Berlin";

    /// <summary>Gets or sets an optional note for the demo person.</summary>
    [ObservableProperty]
    private string? _note;

    /// <summary>Gets or sets the storage mode — Encrypted for living persons, Plaintext for deceased.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEncryptedMode))]
    [NotifyPropertyChangedFor(nameof(IsPlaintextMode))]
    private StoreMode _storeMode = StoreMode.Encrypted;

    /// <summary>Gets or sets whether the Encrypted radio button is selected.</summary>
    public bool IsEncryptedMode
    {
        get => StoreMode == StoreMode.Encrypted;
        set { if (value) StoreMode = StoreMode.Encrypted; }
    }

    /// <summary>Gets or sets whether the Plaintext radio button is selected.</summary>
    public bool IsPlaintextMode
    {
        get => StoreMode == StoreMode.Plaintext;
        set { if (value) StoreMode = StoreMode.Plaintext; }
    }

    // ── Output ────────────────────────────────────────────────────────────────

    /// <summary>Gets or sets the formatted display text of the last loaded person.</summary>
    [ObservableProperty]
    private string _loadedPersonDisplay = string.Empty;

    /// <summary>Gets or sets the raw JSON content read from disk.</summary>
    [ObservableProperty]
    private string _rawContent = string.Empty;

    /// <summary>Gets or sets the accumulated log messages.</summary>
    [ObservableProperty]
    private string _logText = string.Empty;

    // ── Access control ────────────────────────────────────────────────────────

    /// <summary>Gets or sets the person ID used for access-control operations.</summary>
    [ObservableProperty]
    private string _grantPersonId = "P001";

    /// <summary>Gets or sets the Windows SID to grant access to.</summary>
    [ObservableProperty]
    private string _grantSid = string.Empty;

    /// <summary>Gets the list of Windows SIDs allowed to read the selected person record.</summary>
    [ObservableProperty]
    private ObservableCollection<string> _allowedSids = new();

    // ── Recovery ──────────────────────────────────────────────────────────────

    /// <summary>Gets or sets the full path of the current recovery key backup file.</summary>
    [ObservableProperty]
    private string _recoveryFilePath = string.Empty;

    // ── Info ──────────────────────────────────────────────────────────────────

    /// <summary>Gets or sets the display string for the current Windows user SID.</summary>
    [ObservableProperty]
    private string _currentUserSidDisplay = string.Empty;

    /// <summary>
    /// Initializes a new instance of <see cref="MainViewModel"/>.
    /// </summary>
    /// <param name="store">The encrypted person store.</param>
    /// <param name="backupService">The master key backup service.</param>
    /// <param name="options">The store configuration options.</param>
    public MainViewModel(
        IPersonSecureStore store,
        IMasterKeyBackupService backupService,
        GenSecureStoreOptions options)
    {
        _store = store;
        _backupService = backupService;
        _options = options;

        _rootDirectory = options.RootDirectory ?? Path.Combine(Path.GetTempPath(), "GenSecureDemo");

        var sSid = WindowsIdentity.GetCurrent().User?.Value ?? "(unbekannt)";
        _currentUserSidDisplay = $"Benutzer-SID: {sSid}";
        _grantSid = sSid;

        try { _recoveryFilePath = options.RecoveryKeyFilePath; }
        catch { _recoveryFilePath = string.Empty; }

        Log("GenSecure Demo gestartet.");
        Log($"Speicherort: {_rootDirectory}");
        Log($"Benutzer-SID: {sSid}");
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    /// <summary>Encrypts the entered person data and saves it to the store.</summary>
    [RelayCommand]
    private void SavePerson()
    {
        try
        {
            var person = new Models.DemoPerson
            {
                FirstName = FirstName,
                LastName = LastName,
                BirthDate = BirthDate,
                Address = Address,
                Note = Note
            };
            _store.Save(PersonId, person, StoreMode);
            Log($"✅ Person '{PersonId}' gespeichert ({StoreMode}).");
        }
        catch (Exception ex)
        {
            Log($"❌ Fehler beim Speichern: {ex.Message}");
        }
    }

    /// <summary>Decrypts and loads the person record for the current person ID.</summary>
    [RelayCommand]
    private void LoadPerson()
    {
        try
        {
            var person = _store.Load<Models.DemoPerson>(PersonId);
            var sb = new StringBuilder();
            sb.AppendLine($"ID:        {PersonId}");
            sb.AppendLine($"Vorname:   {person.FirstName}");
            sb.AppendLine($"Nachname:  {person.LastName}");
            sb.AppendLine($"Geb.-Dat.: {person.BirthDate}");
            sb.AppendLine($"Adresse:   {person.Address}");
            if (!string.IsNullOrEmpty(person.Note))
                sb.AppendLine($"Notiz:     {person.Note}");
            LoadedPersonDisplay = sb.ToString();
            Log($"✅ Person '{PersonId}' entschlüsselt geladen.");
        }
        catch (Exception ex)
        {
            LoadedPersonDisplay = string.Empty;
            Log($"❌ Fehler beim Laden: {ex.Message}");
        }
    }

    /// <summary>Checks whether a person record exists in the encrypted store.</summary>
    [RelayCommand]
    private void ExistsPerson()
    {
        try
        {
            var xExists = _store.Exists(PersonId);
            Log(xExists
                ? $"✅ Person '{PersonId}' existiert im Store."
                : $"ℹ️  Person '{PersonId}' ist NICHT im Store vorhanden.");
        }
        catch (Exception ex)
        {
            Log($"❌ Fehler: {ex.Message}");
        }
    }

    /// <summary>
    /// Removes both the data file and the key file for the current person.
    /// </summary>
    [RelayCommand]
    private void SoftDeletePerson()
    {
        try
        {
            _store.Delete(PersonId, DeleteMode.SoftDelete);
            LoadedPersonDisplay = string.Empty;
            Log($"✅ '{PersonId}' per SoftDelete entfernt (Daten- und Schlüsseldatei gelöscht).");
        }
        catch (Exception ex)
        {
            Log($"❌ Fehler beim SoftDelete: {ex.Message}");
        }
    }

    /// <summary>
    /// Removes only the key file, rendering the encrypted payload permanently
    /// unreadable — crypto-deletion per DSGVO Art. 17.
    /// </summary>
    [RelayCommand]
    private void SecureDeletePerson()
    {
        try
        {
            _store.Delete(PersonId, DeleteMode.SecureDelete);
            LoadedPersonDisplay = string.Empty;
            Log($"✅ '{PersonId}' per SecureDelete entfernt (nur Schlüsseldatei gelöscht – Daten dauerhaft unlesbar).");
        }
        catch (Exception ex)
        {
            Log($"❌ Fehler beim SecureDelete: {ex.Message}");
        }
    }

    /// <summary>Grants read access for the entered Windows SID on the entered person record.</summary>
    [RelayCommand]
    private void GrantAccess()
    {
        if (string.IsNullOrWhiteSpace(GrantSid))
        {
            Log("⚠️  Bitte einen Windows-SID eingeben.");
            return;
        }

        try
        {
            _store.GrantAccess(GrantPersonId, GrantSid);
            Log($"✅ Zugriff für SID '{GrantSid}' auf Person '{GrantPersonId}' gewährt.");
        }
        catch (Exception ex)
        {
            Log($"❌ Fehler beim Gewähren des Zugriffs: {ex.Message}");
        }
    }

    /// <summary>Retrieves and lists all Windows SID hashes permitted to access the entered person record.</summary>
    [RelayCommand]
    private void GetAllowedSids()
    {
        try
        {
            var lstHashes = _store.GetAllowedWindowsSidHashes(GrantPersonId);
            AllowedSids.Clear();
            foreach (var sHash in lstHashes)
                AllowedSids.Add(sHash);
            Log($"✅ {AllowedSids.Count} SID-Hash(es) für Person '{GrantPersonId}' geladen (HMAC-SHA256 mit Master-Key-Pepper, kein Klartext-SID).");
        }
        catch (Exception ex)
        {
            Log($"❌ Fehler: {ex.Message}");
        }
    }

    /// <summary>Reads and displays the raw encrypted payload file for the current person.</summary>
    [RelayCommand]
    private void ShowDataFile()
    {
        try
        {
            var sPath = Path.Combine(_options.DataDirectoryPath, GetFileId(PersonId) + ".person.json");
            if (!File.Exists(sPath))
            {
                RawContent = "(Datei nicht gefunden)";
                Log($"⚠️  Datendatei für '{PersonId}' nicht gefunden:\n{sPath}");
                return;
            }
            RawContent = File.ReadAllText(sPath);
            Log($"📄 Datendatei angezeigt: {sPath}");
        }
        catch (Exception ex)
        {
            Log($"❌ Fehler: {ex.Message}");
        }
    }

    /// <summary>Reads and displays the raw wrapped person-key file for the current person.</summary>
    [RelayCommand]
    private void ShowKeyFile()
    {
        try
        {
            var sPath = Path.Combine(_options.KeyDirectoryPath, GetFileId(PersonId) + ".key.json");
            if (!File.Exists(sPath))
            {
                RawContent = "(Datei nicht gefunden)";
                Log($"⚠️  Schlüsseldatei für '{PersonId}' nicht gefunden:\n{sPath}");
                return;
            }
            RawContent = File.ReadAllText(sPath);
            Log($"🔑 Schlüsseldatei angezeigt: {sPath}");
        }
        catch (Exception ex)
        {
            Log($"❌ Fehler: {ex.Message}");
        }
    }

    /// <summary>Reads and displays the DPAPI-protected local master key file.</summary>
    [RelayCommand]
    private void ShowMasterKeyFile()
    {
        try
        {
            var sPath = _options.LocalMasterKeyFilePath;
            if (!File.Exists(sPath))
            {
                RawContent = "(Datei nicht gefunden – Master-Schlüssel noch nicht initialisiert)";
                Log($"⚠️  Master-Key-Datei nicht gefunden:\n{sPath}");
                return;
            }
            RawContent = File.ReadAllText(sPath);
            Log($"🔐 Master-Key-Datei angezeigt: {sPath}");
        }
        catch (Exception ex)
        {
            Log($"❌ Fehler: {ex.Message}");
        }
    }

    /// <summary>Opens the storage root directory in Windows Explorer.</summary>
    [RelayCommand]
    private void OpenExplorer()
    {
        try
        {
            var sDir = _options.RootDirectory ?? string.Empty;
            if (Directory.Exists(sDir))
            {
                System.Diagnostics.Process.Start("explorer.exe", sDir);
                Log($"📂 Explorer geöffnet: {sDir}");
            }
            else
            {
                Log($"⚠️  Verzeichnis existiert noch nicht: {sDir}");
            }
        }
        catch (Exception ex)
        {
            Log($"❌ Fehler: {ex.Message}");
        }
    }

    /// <summary>Opens a folder picker and sets the selected path as the new storage root.</summary>
    [RelayCommand]
    private void ChooseDirectory()
    {
        var dlg = new Microsoft.Win32.OpenFolderDialog
        {
            Title = "GenSecure Speicherverzeichnis wählen",
            Multiselect = false
        };

        if (dlg.ShowDialog() == true)
        {
            _options.RootDirectory = dlg.FolderName;
            RootDirectory = dlg.FolderName;
            try { RecoveryFilePath = _options.RecoveryKeyFilePath; }
            catch { /* ignore until first use */ }
            Log($"📁 Neues Verzeichnis gesetzt: {dlg.FolderName}");
        }
    }

    // ── PasswordBox bridge ────────────────────────────────────────────────────

    /// <summary>
    /// Creates a passphrase-protected recovery backup of the local master key.
    /// Called from code-behind because <see cref="System.Windows.Controls.PasswordBox"/>
    /// does not support MVVM data binding.
    /// </summary>
    /// <param name="sPassphrase">Passphrase that protects the backup.</param>
    /// <param name="sPassphraseConfirm">Confirmation passphrase — must match <paramref name="sPassphrase"/>.</param>
    public void CreateRecoveryBackup(string sPassphrase, string sPassphraseConfirm)
    {
        if (string.IsNullOrWhiteSpace(sPassphrase))
        {
            Log("⚠️  Bitte eine Passphrase eingeben.");
            return;
        }

        if (sPassphrase != sPassphraseConfirm)
        {
            Log("❌ Passphrasen stimmen nicht überein.");
            return;
        }

        try
        {
            _backupService.CreateRecoveryKeyBackup(sPassphrase, xOverwrite: true);
            RecoveryFilePath = _options.RecoveryKeyFilePath;
            Log($"✅ Recovery-Backup erstellt: {RecoveryFilePath}");
        }
        catch (Exception ex)
        {
            Log($"❌ Fehler beim Erstellen des Backups: {ex.Message}");
        }
    }

    /// <summary>
    /// Attempts to restore the local master key from a passphrase-protected recovery backup.
    /// Called from code-behind because <see cref="System.Windows.Controls.PasswordBox"/>
    /// does not support MVVM data binding.
    /// </summary>
    /// <param name="sPassphrase">Passphrase used when creating the backup.</param>
    public void RestoreFromRecovery(string sPassphrase)
    {
        if (string.IsNullOrWhiteSpace(sPassphrase))
        {
            Log("⚠️  Bitte die Passphrase eingeben.");
            return;
        }

        try
        {
            var xSuccess = _backupService.TryRestoreLocalMasterKey(sPassphrase);
            if (xSuccess)
                Log("✅ Master-Schlüssel erfolgreich aus Recovery-Backup wiederhergestellt.");
            else
                Log("❌ Wiederherstellung fehlgeschlagen – Passphrase falsch oder Backup nicht vorhanden.");
        }
        catch (Exception ex)
        {
            Log($"❌ Fehler bei der Wiederherstellung: {ex.Message}");
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>Appends a timestamped entry to the log output.</summary>
    private void Log(string sMessage) =>
        LogText += $"[{DateTime.Now:HH:mm:ss}] {sMessage}{Environment.NewLine}";

    /// <summary>
    /// Derives a deterministic hex file name from a person ID using SHA-256,
    /// matching the internal file-naming convention of <c>GenSecure.Core</c>.
    /// </summary>
    /// <param name="sPersonId">The person identifier.</param>
    /// <returns>Lowercase hex-encoded SHA-256 digest of the UTF-8 encoded person ID.</returns>
    private static string GetFileId(string sPersonId)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(sPersonId));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
