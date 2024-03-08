<#
.SYNOPSIS
    Increments the build number for Windows and Android.
    
.DESCRIPTION
    This script loads the Windows project's Package.appmanifest file, extracts the 
    Package/Identity/Version attribute, increments the third digit and updates
    the attribute, saving the modified xml file in place. It then loads the Android 
    project's AndroidManifest.xml file and sets the manifest.versionName to the 
    first three digits of the UWP version string, saving that file in place as well. 
    Without the -Increment switch, this script only displays various version numbers. 

.PARAMETER Increment
    Increments the build numbers, without this switch the current version strings 
    are only displayed.
#>

param([Parameter(HelpMessage = "Increment (default only displays version strings)")] [switch] $Increment)


$solutionName = Split-Path $PWD -Leaf
$windowsManifest = ".\Platforms\Windows\Package.appxmanifest"
$androidManifest = ".\Platforms\Android\AndroidManifest.xml"
$windowsAppPackages = ".\bin\Release\net8.0-windows10.0.19041.0\win10-x64\AppPackages"
$androidAppPackages = ".\bin\Release\net8.0-android\AppPackages"
$public = Join-Path "C:\Users\Public\Downloads" $solutionName
[xml] $xmlWindows = Get-Content $windowsManifest
[xml] $xmlAndroid = Get-Content $androidManifest
$version = $xmlWindows.Package.Identity.Version
$versionName = $xmlAndroid.manifest.versionName
if ($version.Substring(0, $version.LastIndexOf(".")) -ne $versionName)
{
    Write-Host "Warning: Windows version $version does not match Android version $versionName"
    Write-Host "The Android version will be set to the Windows version."
}

Write-Host "Windows version string is $version from $windowsManifest"
Write-Host "Android version string is $versionName from $androidManifest"
Write-Host "Most recent installers:"
$file = (Get-ChildItem $windowsAppPackages | Sort-Object CreationTime | Select-Object -Last 1).Name
Write-Host (Join-Path $windowsAppPackages $file)
$file = (Get-ChildItem $androidAppPackages | Sort-Object CreationTime | Select-Object -Last 1).Name
Write-Host (Join-Path $androidAppPackages $file)

(Get-ChildItem -Directory $public | Sort-Object CreationTime | Select-Object -Last 1).FullName
(Get-ChildItem (Join-Path $public *.apk) | Sort-Object CreationTime | Select-Object -Last 1).FullName
if ($Increment)
{
    $old = $version.Split(".")
    $buildNumber = [int] $old[2] + 1
    $newVersion = $old[0] + "." + $old[1] + "." + $buildNumber + "." +$old[3]
    Write-Host "Incrementing build number to $buildNumber"
    Write-Host "New Windows version string: $newVersion"
    $xmlWindows.Package.Identity.Version = $newVersion
    $xmlWindows.Save((Get-ChildItem $windowsManifest).FullName)
    $newVersion = $newVersion.Substring(0, $newVersion.LastIndexOf("."))
    Write-Host "New Android version string: $newVersion"
    $xmlAndroid.manifest.versionName = $newVersion
    $xmlAndroid.Save((Get-ChildItem $androidManifest).FullName)
}

Write-Host
Write-Host "Steps to create sideloading installers:"
Write-Host "1. Increment build number with -Increment"
Write-Host "2. Select Release, Framework net8.0-windows10.0.19041.0"
Write-Host "3. Publish, Sideloading, no automatic updates, sign with bryan, don't change version, "
write-Host "   Publishing profile MSIX-win10-x64.pubxml, no symbol files, Create."
Write-Host "4. Select Release, Framework: net8.0-android"
Write-Host "5. Publish, Distribute..., Ad Hoc, sign with bryan-qkr, Save As, replace existing, "
Write-Host "   password MiaIsTheGBOAT, then run Copy-ApkFile.ps1"
Write-Host "6. Run Copy-InstallersToPublic.ps1"
Write-Host "6. Zip the Boggle_<version>_Test folder in C:\Users\Public\Downloads\Boggle"
Write-Host "7. Upload the zip and apk files to Google Drive's Apps/Boggle folder"
