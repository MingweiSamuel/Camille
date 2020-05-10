# Set CAMI_DO_DEPLOY
$env:CAMI_DO_DEPLOY = (($env:APPVEYOR_REPO_COMMIT_MESSAGE -NotMatch '\[no deploy\]') `
    -Or $env:APPVEYOR_SCHEDULED_BUILD -Or $env:SCHEDULED_BUILD -Or $env:APPVEYOR_REPO_TAG_NAME) `
    -And ($env:APPVEYOR_REPO_BRANCH -Match '^release/') `
    -And (-Not $env:APPVEYOR_PULL_REQUEST_NUMBER)


# Set CAMI_VERSION
# Get latest tagged version.
$VERSION_TAG = git describe --abbrev=0 --tags --match 'v*.*.*'
# Get largest version if multiple tags.
$VERSION_TAG = git tag --points-at "$VERSION_TAG" --sort=-version:refname --list 'v*.*.*' | Select-Object -First 1

# Default: Stable release.
$env:CAMI_VERSION = $VERSION_TAG.Substring(1)
# Conditional: Nightly release
# If this isn't a tag build, or if the version is negative (prerelease).
If ($VERSION_TAG -Ne $env:APPVEYOR_REPO_TAG_NAME -Or -Not ($env:CAMI_VERSION -As [Version])) {
    # Incrememt minor to make sure nightlies are ordered newer than releases.
    $ver = $VERSION_TAG.Substring(1).Split('.')
    $ver[2] = [int]$ver[2] + 1
    $ver = $ver -Join '.'
    # Append date for ordering, and include commit ID.
    $date = (Get-Date).ToUniversalTime().ToString("o").Substring(0, 10)
    $env:CAMI_VERSION = "$($ver)-nightly-$date-$(git rev-parse --short=10 HEAD)"
}

Write-Output "CAMI_DO_DEPLOY=$env:CAMI_DO_DEPLOY"
Write-Output "CAMI_VERSION=$env:CAMI_VERSION"
