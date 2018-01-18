Push-Location .\Camille

 [xml]$CSPROJ = Get-Content -Path .\Camille.csproj
 $NEW_VERSION = "$($CSPROJ.Project.PropertyGroup.Version)-nightly-$($ENV:APPVEYOR_REPO_COMMIT_TIMESTAMP.subString(0, 10))-$($ENV:APPVEYOR_REPO_COMMIT.subString(0, 10))"

 Write-Host "version: $NEW_VERSION"

 dotnet msbuild /t:build /p:Configuration=Release
 dotnet msbuild /t:pack /p:Configuration=Release /p:Version="$NEW_VERSION" /p:PackageReleaseNotes="Nightly Release"


 Pop-Location
