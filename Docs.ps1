$VERSION = $env:CAMI_VERSION

Remove-Item -Recurse -Force 'docs' -ErrorAction Ignore

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
Write-Host $MSG


# Clone docs.
git clone -q --single-branch --branch gh-pages "$(git remote get-url origin)" docs
# Copy git credentials.
if ($env:CI) {
    git -C docs config credential.helper "$(git config credential.helper)"
    git -C docs config 'http.https://github.com/.extraheader' "$(git config 'http.https://github.com/.extraheader')"
    git -C docs config core.autocrlf input
    git -C docs config core.safecrlf false
}

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
# Clear out output folder.
Remove-Item -Recurse -Force "$DEST\*" -ErrorAction Ignore

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
git diff --quiet -- $(Resolve-Path -Relative ..\$HASHFILE)
$UNCHANGED = 0 -Eq $LastExitCode

If ($env:CAMI_DO_DEPLOY -Ne 'true') {
    Write-Host 'CAMI_DO_DEPLOY not set to true, exiting.'
    $env:CAMI_DO_DEPLOY = ''
}
ElseIf ($IS_STABLE) {
    Write-Host "Releasing stable version. Nightly spec hash unchanged: $UNCHANGED."
}
ElseIf ($UNCHANGED) {
    Write-Host 'No substantial changes, exiting.'
    # Turn off NuGet deploy.
    $env:CAMI_DO_DEPLOY = ''
}
Else {
    Write-Host 'CAMI_DO_DEPLOY set to true, has substantial changes.'
}

If ($env:CAMI_DO_DEPLOY -Eq 'true') {
    git add -A
    git -c 'user.name=github-actions[bot]' `
        -c 'user.email=41898282+github-actions[bot]@users.noreply.github.com' `
        commit -m $MSG
    git push --quiet
}

Pop-Location

Write-Host "CAMI_DO_DEPLOY=$env:CAMI_DO_DEPLOY"
Write-Host "CAMI_VERSION=$env:CAMI_VERSION"
Write-Host "CAMI_SPEC_HASH=$env:CAMI_SPEC_HASH"
