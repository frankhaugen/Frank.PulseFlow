param (
    [Parameter(Mandatory=$false)]
    [string]$version = (Get-Date -Format 'yyyy.MM.dd')
)

# Get directory of the script
$scriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Definition

# Get path to the output directory
$outputDirectory = "$scriptDirectory/.artifacts"

# Get path to the publish directory
$publishDirectory = "$outputDirectory/publish"

# Get path to the package directory
$packageDirectory = "$outputDirectory/packages"

# Get path to the project file
$projectFile = "$scriptDirectory/Frank.PulseFlow/Frank.PulseFlow.csproj"

# Restore NuGet packages
Write-Host "Restoring NuGet packages..." -ForegroundColor DarkCyan
dotnet restore $projectFile | Out-Null

# Clean output directories
if (Test-Path $publishDirectory) { Remove-Item "$publishDirectory/*" -Recurse }
if (Test-Path $packageDirectory) { Remove-Item "$packageDirectory/*" -Recurse }

# Build the solution in the specified mode
Write-Host "Building solution in Release mode..." -ForegroundColor DarkCyan
dotnet build $projectFile --configuration Release --no-restore /p:Version=$version | Out-Null

# Pack NuGet packages
Write-Host "Packing NuGet packages..." -ForegroundColor DarkCyan
dotnet pack $projectFile --configuration Release --output $packageDirectory --no-build /p:Version=$version /p:PackageVersion=$version | Out-Null

# Publish the solution and pack NuGet packages
Write-Host "Publishing the project..." -ForegroundColor DarkCyan
dotnet publish $projectFile --configuration Release --output $publishDirectory --no-build /p:Version=$version /p:PackageVersion=$version | Out-Null
Write-Host "Project has been published." -ForegroundColor Green