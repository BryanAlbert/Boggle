<#
.SYNOPSIS
    Copies the latest AppPackages to the Public Downloads folder.

.DESCRIPTION
    This script first checks the Windows and Android version strings in the project manifest
    files for consistency, then copies the installer files from the AppPackages folders
    to the Public Downloads folder C:\Users\Public\Downloads\Boggle. 

    TODO: implement this: 
    When using -CopyToNetwork, \\Bryan-Dell must be turned on (Bryan's old Dell Inspiron 
    13-7353 laptop). The network share is \\Bryan-Dell\Users\Public\Downloads and the 
    target folder is "Boggle". Note that the parent folders (e.g. \\Bryan-Dell\Users\)
    are not shared so the whole path needs to be typed in on a computer on the network. 

.PARAMETER Overwrite
    Set to true to overwrite the target file. 

.PARAMETER CopyToNetwork
    Copies AppPackages files to the Albert home network share on \\Bryan-Dell.
#>

param(
    [Parameter(HelpMessage = "Overwrite existing installer files")] [switch] $Overwrite,
    [Parameter(HelpMessage = "Copy to `"\\BRYAN-DELL\Users\Downloads\Boggle`"")] [switch] $CopyToNetwork)


if ($CopyToNetwork) {
    write-Host "Note: -CopyToNetwork is not implemented."
}

$solutionName = Split-Path $PWD -Leaf
$windowsManifest = ".\Platforms\Windows\Package.appxmanifest"
$windowsAppPackages = ".\bin\Release\net8.0-windows10.0.19041.0\win10-x64\AppPackages"
$androidManifest = ".\Platforms\Android\AndroidManifest.xml"
$androidAppPackages = ".\bin\Release\net8.0-android\AppPackages"
$public = Join-Path "C:\Users\Public\Downloads" $solutionName
$version = ([xml] (Get-Content $windowsManifest)).Package.Identity.Version
$versionName = ([xml] (Get-Content $androidManifest)).manifest.versionName
if ($version.Substring(0, $version.LastIndexOf(".")) -ne $versionName)
{
    Write-Host "Warning: Windows version $version does not match Android version $versionName"
    Write-Host "IncrementBuildNumber.ps1 updates both version strings."
}

$windowsSource = Get-ChildItem $windowsAppPackages *$version* -Directory
if ($windowsSource.Count -gt 1)
{
    Write-Host "Error: $windowsAppPackages has $($windowsSource.Count) folders with version $version"
    return
}

$androidSource = (Get-ChildItem $androidAppPackages *$versionName.apk)
$debug = $windowsSource | Where-Object { $_.Name.Contains("_Debug_") }
if ($null -ne $debug)
{
    Write-Host "Error: _Debug_ folder found. Did you build Release? Folder(s):"
    $windowsSource
    return
}

if ($null -ne $windowsSource)
{
    if ($null -eq $androidSource)
    {
        Write-Host "Error: directory for version $versionName not found in $androidAppPackages"
        return
    }

    $target = Join-Path $public (Split-Path $windowsSource -Leaf)
    if (Test-Path $target)
    {
        if ($Overwrite)
        {
            Write-Host "Deleting $target and copying $windowsSource to $public..."
            Remove-Item $target -Recurse 
            Copy-Item $windowsSource -Recurse -Destination $public
            Write-Host "Copying $androidSource directory to $public..."
            Copy-Item $androidSource -Destination $public
        }
        else
        {
            Write-Host "Error: directory already exists: $target"
            Write-Host "Did you run IncrementBuildNumber.ps1 and Publish Windows and Android?"
        }
    }
    else
    {
        Write-Host "Copying $windowsSource directory to $public..."
        Copy-Item $windowsSource -Recurse -Destination $public
        Write-Host "Copying $androidSource directory to $public..."
        Copy-Item $androidSource -Destination $public
    }
}
else
{
    Write-Host "Error: directory for version $version not found in $windowsAppPackages"
}

Write-Host
Write-Host "Finished."
