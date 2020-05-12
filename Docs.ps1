$VERSION = $env:CAMI_VERSION

Remove-Item 'docs' -Recurse -Force -ErrorAction Ignore

# Version logic:
# Keep one nightly version per MAJOR verison.
# Always update nightly version.
# Only update stable version if this is a stable version.

$IS_STABLE = $VERSION -NotMatch 'nightly'
$NIGHTLY = "$($VERSION.Substring(0, $VERSION.IndexOf('.'))).x.x"

$MSG = "Autogen Docs $VERSION

Date: $((Get-Date).ToUniversalTime())
Commit: $(git rev-parse HEAD)
Stable: $IS_STABLE
Spec Hash: $env:CAMI_SPEC_HASH
"
Write-Output $MSG

# Configure git.
git config --global credential.helper store
if ($env:APPVEYOR) {
    Add-Content "$HOME\.git-credentials" "https://$($env:access_token):x-oauth-basic@github.com`n"
    git config --global user.name "Appveyor"
    git config --global user.email "$(git show -s --format='%ae' HEAD)"
    git config --global core.autocrlf true
    git config --global core.safecrlf false # Hide CRLF warnings.
}

# Clone docs.
git clone -q --single-branch --branch gh-pages "$(git remote get-url origin)" docs
# Install docfx.
choco install docfx -y | Out-Null
# Create metadata.
docfx metadata .\docfx_project\docfx.json
# Change links to generated files.
Get-ChildItem "docfx_project\api" -Filter '*.yml' | ForEach-Object {
    (Get-Content $_.FullName -raw) -replace '(?m)^(\s+)path:\s+(Camille\.[\w\.]+)\/gen\/(.*)\.cs\r?\n\s+branch:\s+\S+',
        "`$1path: v\$NIGHTLY\_gen\`$2\`$3.cs`r`n`$1branch: gh-pages" | Out-File -FilePath $_.FullName -Encoding 'UTF8'
}
$DEST = "docs\v\$NIGHTLY"
# Build HTML.
docfx build .\docfx_project\docfx.json -o "$DEST\docs"
# Create folder for generated files & copy over.
Get-ChildItem -Directory -Filter 'Camille.*' | ForEach-Object {
    Remove-Item "$DEST\_gen\$($_.Name)" -Recurse -Force -ErrorAction Ignore
    Copy-Item -Path "$($_.Name)\gen" -Filter '*.cs' -Destination "$DEST\_gen\$($_.Name)" -Recurse -ErrorAction Ignore
}
$HASHFILE = "$DEST\spechash.txt"
$env:CAMI_SPEC_HASH | Out-File -FilePath $HASHFILE -Encoding 'ASCII'

If ($IS_STABLE) {
    Remove-Item "docs\v\$VERSION" -Recurse -Force -ErrorAction Ignore
    Copy-Item -Path "$DEST" -Destination "docs\v\$VERSION" -Recurse
}

Push-Location 'docs'
# Check if there are substantial changes.
git diff --quiet -- $HASHFILE
$UNCHANGED = 0 -Eq $LastExitCode

If ($env:CAMI_DO_DEPLOY -Ne $true) {
    Write-Output 'CAMI_DO_DEPLOY not set to true, exiting.'
}
ElseIf ($IS_STABLE) {
    Write-Output "Releasing stable version. Nightly spec hash unchanged: $UNCHANGED."
}
ElseIf ($UNCHANGED) {
    Write-Output 'No substantial changes, exiting.'
    # Turn off NuGet deploy.
    $env:CAMI_DO_DEPLOY = $false
}
Else {
    git add .
    git commit -m $MSG
    git push -q
}
Pop-Location

Write-Output "CAMI_DO_DEPLOY=$env:CAMI_DO_DEPLOY"
Write-Output "CAMI_VERSION=$env:CAMI_VERSION"
Write-Output "CAMI_SPEC_HASH=$env:CAMI_SPEC_HASH"
