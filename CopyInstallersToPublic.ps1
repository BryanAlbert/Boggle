param([Parameter(HelpMessage = "Overwrite existing installer files")] [switch] $Overwrite)

# Copies the latest AppPackages to the Public Downloads folder.

$windowsManifest = ".\Platforms\Windows\Package.appxmanifest"
$windowsAppPackages = ".\bin\Release\net7.0-windows10.0.19041.0\win10-x64\AppPackages"
$androidManifest = ".\Platforms\Android\AndroidManifest.xml"
$androidAppPackages = ".\bin\Release\net7.0-android\AppPackages"
$public = Join-Path "C:\Users\Public\Downloads" $solutionName

$version = ([xml] (Get-Content $windowsManifest)).Package.Identity.Version
$versionName = ([xml] (Get-Content $androidManifest)).manifest.versionName
if ($version.Substring(0, $version.LastIndexOf(".")) -ne $versionName)
{
    Write-Host "Warning: Windows version $version does not match Android version $versionName"
    Write-Host "IncrementBuildNumber.ps1 updates both version strings."
}

$windowsSource = (Get-ChildItem $windowsAppPackages *$version* -Directory)
$androidSource = (Get-ChildItem $androidAppPackages *$versionName.apk)
$debug = $windowsSource | Where-Object { $_.Name.Contains("_Debug_") }
if ($null -ne $debug)
{
    Write-Host "Error: _Debug_ folder found. Did you build Release? Folder(s):"
    $windowsSource
}
elseif ($null -ne $windowsSource)
{
    if ($null -eq $androidSource)
    {
        Write-Host "Error: directory for version $versionName not found in $androidAppPackages"
    }
    else
    {
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
}
else
{
    Write-Host "Error: directory for version $version not found in $windowsAppPackages"
}

Write-Host
Write-Host "Finished."
