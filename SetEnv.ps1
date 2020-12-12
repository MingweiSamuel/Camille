# Set CAMI_DO_DEPLOY
$COMMIT_MESSAGE = git log -1 --pretty=format:%B
$SCHEDULED_BUILD = $env:GITHUB_EVENT_NAME -Eq 'schedule'
$DISPATCHED_BUILD = $env:GITHUB_EVENT_NAME -Eq 'workflow_dispatch'
# $REPO_BRANCH = If ($env:ACTION_REF -Match '^refs/heads/') { $env:ACTION_REF -Replace '^refs/heads/' } Else { $null }
$REPO_TAG = If ($env:ACTION_REF -Match '^refs/tags/') { $env:ACTION_REF -Replace '^refs/tags/' } Else { $null }

# Assume we are on a valid ref; we control when this is called in gh-actions.
$env:CAMI_DO_DEPLOY = `
    # If this is a dispatched build, only deploy if flag is set.
    If ($DISPATCHED_BUILD) {
        # Can be 'true' or 'force'
        $env:ACTION_DISPATCH_DEPLOY
    }
    # Otherwise, check normal deploy conditions.
    ElseIf (($COMMIT_MESSAGE -NotMatch '\[no deploy\]') -Or $SCHEDULED_BUILD -Or $REPO_TAG) {
        'true'
    }
    # False, no-deploy case.
    Else {
        ''
    }

# Set CAMI_VERSION
# Get latest tagged version.
$VERSION_TAG = git describe --abbrev=0 --tags --match 'v/*.*.*'
# Get largest version if multiple tags (TODO: may fail with double-digit versions).
$VERSION_TAG = git tag --points-at $VERSION_TAG --sort=-version:refname --list 'v/*.*.*' | Select-Object -First 1

# Default: Stable release.
$null, $env:CAMI_VERSION = $VERSION_TAG -Split '/',2
# Conditional: Nightly release
# If this isn't a tag build, or if the version is negative (prerelease).
If ($VERSION_TAG -Ne $REPO_TAG -Or -Not ($env:CAMI_VERSION -As [Version])) {
    # Incrememt minor to make sure nightlies are ordered newer than releases.
    $ver = $env:CAMI_VERSION -Split '\.'
    $ver[2] = [int]$ver[2] + 1
    $ver = $ver -Join '.'
    # Append date for ordering, and include commit ID.
    $date = (Get-Date).ToUniversalTime().ToString('o').Substring(0, 10)
    $env:CAMI_VERSION = "$ver-nightly-$date-$(git rev-parse --short=10 HEAD)"
}

# Set CAMI_SPEC_HASH
$env:CAMI_SPEC_HASH = (
    Invoke-RestMethod -Uri 'http://www.mingweisamuel.com/riotapi-schema/openapi-3.0.0.min.json').Info.Version

Write-Host "CAMI_DO_DEPLOY=$env:CAMI_DO_DEPLOY"
Write-Host "CAMI_VERSION=$env:CAMI_VERSION"
Write-Host "CAMI_SPEC_HASH=$env:CAMI_SPEC_HASH"
