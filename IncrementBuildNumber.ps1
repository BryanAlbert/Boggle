param([Parameter(HelpMessage = "Only display version strings")] [switch] $NoIncrement)

# Increments the build number for Windows and Android. Loads the Windows project's
# Package.appmanifest file, extracting the Package/Identity/Version attribute,
# incrementing the third digit, and resetting the attribute. Saves the modified
# xml file in place. It then loads the Android project's AndroidManifest.xml
# file and sets the manifest.versionName to the first three digits of the Windows
# version string and saves the file in place. -NoIncrement only displays various 
# version numbers. 

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
if ($NoIncrement -ne $true)
{
    $old = $version.Split(".")
    $buildNumber = [int] $old[2] + 1
    $newVersion = $old[0] + "." + $old[1] + "." + $buildNumber + "." +$old[3]
    Write-Host "Incrementing build number to $buildNumber, new Windows version string: $newVersion"
    $xmlWindows.Package.Identity.Version = $newVersion
    $xmlWindows.Save((Get-ChildItem $windowsManifest).FullName)
}

Write-Host "Android version string is $versionName from $androidManifest"
if ($NoIncrement -ne $true)
{
    $newVersion = $newVersion.Substring(0, $newVersion.LastIndexOf("."))
    Write-Host "New Android version string: $newVersion"
    $xmlAndroid.manifest.versionName = $newVersion
    $xmlAndroid.Save((Get-ChildItem $androidManifest).FullName)
}

if ($NoIncrement -eq $true)
{
    Write-Host "Most recent installers:"
    $file = (Get-ChildItem $windowsAppPackages | Sort-Object CreationTime | Select-Object -Last 1).Name
    Write-Host (Join-Path $windowsAppPackages $file)
    $file = (Get-ChildItem $androidAppPackages | Sort-Object CreationTime | Select-Object -Last 1).Name
    Write-Host (Join-Path $androidAppPackages $file)

    (Get-ChildItem -Directory $public | Sort-Object CreationTime | Select-Object -Last 1).FullName
    (Get-ChildItem (Join-Path $public *.apk) | Sort-Object CreationTime | Select-Object -Last 1).FullName
}

Write-Host
Write-Host "Steps to create sideloading installers:"
Write-Host "Increment build number with this script"
Write-Host "Select Release, Framework net8.0-windows10.0.19041.0"
Write-Host "Publish, Sideloading, no automatic updates, sign with bryan, don't change version, Publishing profile MSIX-win10-x64.pubxml, no symbol files, Create."
Write-Host "Select Release, Framework: net8.0-android"
Write-Host "Publish, Distribute..., Ad Hoc, sign with bryan-qkr, Save As, password MiaIsTheGBOAT, then run CopyApkFile.ps1"
write-Host "Run CopyInstallersToPublic.ps1"
