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

# Clean output directories
if (Test-Path $publishDirectory) { Remove-Item "$publishDirectory/*" -Recurse }
if (Test-Path $packageDirectory) { Remove-Item "$packageDirectory/*" -Recurse }

# Pack NuGet packages
Write-Host "Packing NuGet packages..." -ForegroundColor DarkCyan
dotnet pack $projectFile --configuration Release --output $packageDirectory --no-build /p:Version=$version /p:PackageVersion=$version | Out-Null

# Exit with a success code
exit 0
