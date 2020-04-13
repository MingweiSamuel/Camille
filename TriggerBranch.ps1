Param([string]$branch)

Start-AppveyorBuild -ApiKey $env:api_key `
  -ProjectSlug $env:APPVEYOR_PROJECT_SLUG `
  -Branch $branch `
  -EnvironmentVariables @{
    SCHEDULED_BUILD = $true
  }
