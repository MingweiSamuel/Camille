$VERSION = $env:CAMI_VERSION
$PUSH = $env:CAMI_DO_DEPLOY

Remove-Item -Recurse -Force docs -ErrorAction Ignore

# Version logic:
# Keep one nightly version per MAJOR verison.
# Always update nightly version.
# Only update stable version if this is a stable version.

$IS_STABLE = ($VERSION -NotMatch 'nightly')
$NIGHTLY = "$($VERSION.Substring(0, $VERSION.IndexOf('.'))).x.x"

$MSG = "Autogen Docs $VERSION

Date: $((Get-Date).ToUniversalTime())
Commit: $(git rev-parse HEAD)
Stable: $IS_STABLE
"
Write-Output "$MSG"

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
        "`$1path: v\$NIGHTLY\_gen\`$2\`$3.cs`r`n`$1branch: gh-pages" | Out-File $_.FullName
}
$DEST = "docs\v\$NIGHTLY"
# Build HTML.
docfx build .\docfx_project\docfx.json -o "$DEST\docs"
# Create folder for generated files & copy over.
Get-ChildItem -Directory -Filter 'Camille.*' | ForEach-Object {
    Copy-Item -Path "$($_.Name)\gen" -Filter '*.cs' -Recurse -Destination "$DEST\_gen\$($_.Name)\" -ErrorAction Ignore
}

If ($IS_STABLE) {
    Copy-Item -Path "$DEST" -Destination "docs\v\$VERSION" -Recurse
}

Push-Location .\docs
# Check if there are substantial changes.
git add .
$diffs = (git diff --cached --numstat -- . ':(exclude)**/manifest.json' | ConvertFrom-Csv -Delimiter `t -Header r,w,f |
    Where-Object r -ne '-' |
    Measure-Object r,w -Sum).Sum

If ($PUSH -Ne $true) {
    Write-Output 'No "-Push", exiting.'
}
ElseIf (-Not $diffs -Or ($diffs[0] -LE 5 -And $diffs[1] -LE 5)) {
    Write-Output 'No substantial changes, exiting.'
}
Else {
    git commit -m "$MSG"
    git push -q
}
Pop-Location
