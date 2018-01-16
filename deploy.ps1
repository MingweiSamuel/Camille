Push-Location .\Camille

 [xml]$CSPROJ = Get-Content -Path .\Camille.csproj
 $VERSION = $CSPROJ.Project.PropertyGroup.Version
 $NEW_VERSION = "$VERSION-nightly-$ENV:APPVEYOR_REPO_COMMIT"

 Write-Host "version: $NEW_VERSION"

 dotnet msbuild /t:build /p:Configuration=Release
 dotnet msbuild /t:pack /p:Configuration=Release /p:Version="$NEW_VERSION" /p:PackageReleaseNotes="Nightly Release"

 Pop-Location