# Get the script directory
$scriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Definition

# Get the project file for the test project
$projectFile = "$scriptDirectory/Frank.PulseFlow.Tests/Frank.PulseFlow.Tests.csproj"

# Test the project
Write-Host "Running unit tests..." -ForegroundColor DarkCyan
dotnet test $projectFile --configuration Release

# Exit with a success code
exit 0