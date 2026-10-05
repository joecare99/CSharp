using System;

namespace AhnWin52Backup.Cli.Credentials;

internal interface IPasswordCredentialStore
{
    void SetPassword(char[] password);
}
