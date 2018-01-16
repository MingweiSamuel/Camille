Push-Location .\Camille

 [xml]$CSPROJ = Get-Content -Path .\Camille.csproj
 $VERSION = $CSPROJ.Project.PropertyGroup.Version
 $NEW_VERSION = "$VERSION-nightly-$ENV:APPVEYOR_REPO_COMMIT"

 Write-Host "version: $NEW_VERSION"

 dotnet msbuild /t:pack /p:Configuration=Release /p:Platform=AnyCPU /p:Version="$NEW_VERSION" /p:AssemblyName=Camille

 Pop-Location