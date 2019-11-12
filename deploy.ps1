$packages = "Camille.Enums","Camille.RiotApi","Camille.Lcu"

ForEach ($package in $packages)
{
    Push-Location ".\$package"

    Move-Item -Path ".\$package.csproj" -Destination ".\$package.csproj.bak"

    # Handle version
    [xml]$CSPROJ = Get-Content -Path ".\$package.csproj.bak"
    # Incrememt minor to make sure nightlies are ordered newer than releases.
    $ver = $CSPROJ.Project.PropertyGroup[0].Version.Split('.')
    $ver[2] = [int]$ver[2] + 1
    $ver = $ver -Join '.'
    # Append date for ordering, and include commit ID.
    $NEW_VERSION = "$($ver)-nightly-$($ENV:APPVEYOR_REPO_COMMIT_TIMESTAMP.subString(0, 10))-$($ENV:APPVEYOR_REPO_COMMIT.subString(0, 10))"

    Write-Host "version: $NEW_VERSION"

    $CSPROJ.Project.PropertyGroup[0].Version = "$NEW_VERSION"
    $CSPROJ.save("$pwd\$package.csproj")

    Pop-Location
}

# Build
dotnet msbuild /t:build /p:Configuration=Release
dotnet msbuild /t:pack /p:Configuration=Release /p:PackageReleaseNotes="Nightly Release"

ForEach ($package in $packages)
{
    Push-Location .\$package
    Move-Item -Force -Path .\$package.csproj.bak -Destination .\$package.csproj
    Pop-Location
}
