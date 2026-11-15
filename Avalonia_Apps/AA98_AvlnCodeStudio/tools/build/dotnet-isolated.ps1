[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [ValidateSet('build', 'test', 'run', 'restore', 'publish', 'msbuild')]
    [string]$Command,

    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$Arguments
)

if (@($Arguments | Where-Object { $_ -match '(?i)CustomAfterDirectoryBuildProps' }).Count -ne 0) {
    Write-Error 'CustomAfterDirectoryBuildProps is reserved by the isolation wrapper.'
    exit 2
}

$artifactRoot = [System.IO.Path]::GetFullPath(
    (Join-Path $PSScriptRoot '..\..\..\..\artifacts\windows'))
New-Item -ItemType Directory -Force -Path $artifactRoot | Out-Null

$isolationProps = Join-Path $PSScriptRoot 'IsolatedArtifacts.props'
& dotnet $Command "-p:HostArtifactRoot=$artifactRoot" "-p:CustomAfterDirectoryBuildProps=$isolationProps" @Arguments
exit $LASTEXITCODE
