param (
    [Parameter(Mandatory=$false)]
    [string]$version = (Get-Date -Format 'yyyy.MM.dd')
)

# Get directory of the script
$scriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Definition

# Get path to the project file
$projectFile = "$scriptDirectory/Frank.PulseFlow/Frank.PulseFlow.csproj"

# Build the solution in the specified mode
Write-Host "Building solution in Release mode..." -ForegroundColor DarkCyan
dotnet build $projectFile --configuration Release --no-restore /p:Version=$version | Out-Null

# Exit with a success code
exit 0