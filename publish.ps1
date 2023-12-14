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

# Get path to the project file
$projectFile = "$scriptDirectory/Frank.PulseFlow/Frank.PulseFlow.csproj"

# Publish the solution and pack NuGet packages
Write-Host "Publishing the project..." -ForegroundColor DarkCyan
dotnet publish $projectFile --configuration Release --output $publishDirectory --no-build /p:Version=$version /p:PackageVersion=$version | Out-Null
Write-Host "Project has been published." -ForegroundColor Green

# Exit with a success code
exit 0