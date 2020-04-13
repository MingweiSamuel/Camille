Push-Location .\Camille

# Handle version
[xml]$CSPROJ = Get-Content -Path .\Camille.csproj
# Incrememt minor to make sure nightlies are ordered newer than releases.
$OLD_VERSION = $CSPROJ.Project.PropertyGroup.Version[0]

if ($env:APPVEYOR_REPO_TAG -And ($env:APPVEYOR_REPO_TAG_NAME -Match '^v\d+.\d+.\d+$')) {
    # Stable release.
    $NEW_VERSION = $env:APPVEYOR_REPO_TAG_NAME.Substring(1)
    #if ($OLD_VERSION -Ne $NEW_VERSION) {
    #    Write-Output "Tag version $NEW_VERSION does not match CSPROJ version $OLD_VERSION, not deploying."
    #    Exit 10
    #}
    $RELEASE_NOTES = "Stable Release"
}
else {
    # Nightly release.
    $ver = $OLD_VERSION.Split('.')
    $ver[2] = [int]$ver[2] + 1
    $ver = $ver -Join '.'
    # Append date for ordering, and include commit ID.
    $NEW_VERSION = "$($ver)-nightly-$($ENV:APPVEYOR_REPO_COMMIT_TIMESTAMP.Substring(0, 10))-$($ENV:APPVEYOR_REPO_COMMIT.Substring(0, 10))"
    $RELEASE_NOTES = "Nightly Release"
}

Write-Host "version: $NEW_VERSION"

# Build
dotnet msbuild /t:build /p:Configuration=Release
dotnet msbuild /t:pack /p:Configuration=Release /p:Version="$NEW_VERSION" /p:PackageReleaseNotes="$RELEASE_NOTES"

Pop-Location
