<#
.SYNOPSIS
    Copies the newest Boggle Android Archive to the AppPackages folder.

.DESCRIPTION
    This script reads the Version from the Android manifest, finds the most recently-
    built and Archived apk file in the ~\AppData\Local\Xamarin\Mono for Android\Archives 
    folder and checks its version, then copies the signed apk to the Android AppPackages 
    folder, giving it a versioned filename.

.PARAMETER Overwrite 
    Set to true to overwrite the target file. 
#>

param([Parameter(HelpMessage = "Overwrite existing apk file")] [switch] $Overwrite)


$solutionName = Split-Path $PWD -Leaf
$windowsManifest = ".\Platforms\Windows\Package.appxmanifest"
$androidManifest = ".\Platforms\Android\AndroidManifest.xml"
$appPackages = ".\bin\Release\net8.0-android\AppPackages"
$version = ([xml] (Get-Content $windowsManifest)).Package.Identity.Version
$versionName = ([xml] (Get-Content $androidManifest)).manifest.versionName
if ($version.Substring(0, $version.LastIndexOf(".")) -ne $versionName)
{
    Write-Host "Warning: Windows version $version does not match Android version $versionName"
    Write-Host "Use IncrementBuildNumjber.ps1 to update both version strings."
}

Push-Location 'C:\Users\bryan\AppData\Local\Xamarin\Mono for Android\Archives'
$sourceDirectory = Get-ChildItem -Path 20*\$solutionName* | Sort-Object CreationTime | Select-Object -Last 1
[xml] $xml = (Get-Content (Join-Path $sourceDirectory archive.xml))
$packageVersionName = $xml.Archive.PackageVersionName
if ($packageVersionName -eq $versionName)
{
    $sourceName = $xml.Archive.PackageName + '.apk'
    $sourcePath = Join-Path $sourceDirectory signed-apks $sourceName
    if (Test-Path $sourcePath)
    {
        $targetFolder = Join-Path (Split-Path -Parent $xml.Archive.SolutionPath) $appPackages
        $targetFile = $xml.Archive.PackageName + '-' + $versionName + '.apk'
        $destinationPath = Join-Path $targetFolder $targetFile
        Write-Host "apk package name: $($xml.Archive.PackageName)"
        Write-Host "apk VersionName: $versionName"
        if (!(Test-Path $destinationPath) -or $Overwrite)
        {
            if ($Overwrite) {
                Write-Host "Overwriting target..."
            }

            if (!(Test-Path $targetFolder))
            {
                Write-Host "Creating directory: $targetFolder"
                New-Item -ItemType Directory $targetFolder
            }

            Write-Host "Copying $sourcePath to $destinationPath..."
            Copy-Item $sourceName $destinationPath
        }
        else
        {
            Write-Host "Error: apk already exists: $destinationPath"
            Write-Host "Did you run IncrementBuildNumber and Publish a new release?"
        }
    }
    else
    {
        Write-Host "Error: signed apk file not found: ($sourcePath)."
        Write-Host "Did you sign the apk? Use the Distribute... button, Ad Hoc, choose bryan-qkr."
        Write-Host "Save As to wherever, Password: MisIsTheGBOAT"
    }
}
else
{
    Write-Host "Error: Android version name $versionName does not match the newest Archive's version name $packageVersionName."
    Write-Host "Did you Archive the latest build? Choose Release then right-click the $solutionName solution and Publish..."
}

Write-Host
Write-Host "Finished."
Pop-Location
