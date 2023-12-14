# Get directory of the script
$scriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Definition

# Get path to the project file
$projectFile = "$scriptDirectory/Frank.PulseFlow/Frank.PulseFlow.csproj"

# Restore NuGet packages
Write-Host "Restoring NuGet packages..." -ForegroundColor Cyan
dotnet restore $projectFile | Out-Null

# Exit with a success code
exit 0