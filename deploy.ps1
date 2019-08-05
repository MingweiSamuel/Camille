Push-Location .\Camille

# Handle version
[xml]$CSPROJ = Get-Content -Path .\Camille.csproj
# Incrememt minor to make sure nightlies are ordered newer than releases.
$ver = $CSPROJ.Project.PropertyGroup.Version[0].Split('.')
$ver[2] = [int]$ver[2] + 1
$ver = $ver -Join '.'
# Append date for ordering, and include commit ID.
$NEW_VERSION = "$($ver)-nightly-$($ENV:APPVEYOR_REPO_COMMIT_TIMESTAMP.subString(0, 10))-$($ENV:APPVEYOR_REPO_COMMIT.subString(0, 10))"

Write-Host "version: $NEW_VERSION"

# Build
dotnet msbuild /t:build /p:Configuration=Release
dotnet msbuild /t:pack /p:Configuration=Release /p:Version="$NEW_VERSION" /p:PackageReleaseNotes="Nightly Release"

Pop-Location
