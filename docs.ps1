Remove-Item -Recurse -Force -ErrorAction Ignore docs

# Configure git.
git config --global credential.helper store
Add-Content "$HOME\.git-credentials" "https://$($env:access_token):x-oauth-basic@github.com`n"
git config --global user.name "Appveyor"
git config --global user.email "$($env:APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL)"
git config --global core.autocrlf true
git config --global core.safecrlf false # Hide CRLF warnings.

# Clone docs.
git clone -q --single-branch --branch gh-pages "$(git remote get-url origin)" docs
# Install docfx.
choco install docfx -y | Out-Null
# Create metadata.
docfx metadata .\docfx_project\docfx.json
# Change links to generated files.
Get-ChildItem "docfx_project\api" -Filter *.yml |
ForEach-Object {
  (Get-Content $_.FullName -raw) -replace '(?m)^(\s+)path:\s+Camille\/gen\/(.*)\.cs\r?\n\s+branch:\s+master',
    "`$1path: _gen\`$2.cs`r`n`$1branch: gh-pages" | Out-File $_.FullName
}
# Build HTML.
docfx build .\docfx_project\docfx.json
# Create folder for generated files & copy over.
New-Item -ItemType directory -Path docs\_gen -Force
Copy-Item .\Camille\gen\*.cs .\docs\_gen\

Push-Location .\docs
# Check if there are substantial changes.
$diffs = ((git diff --numstat) | ConvertFrom-Csv -Delimiter `t -Header r,w,f | Measure-Object r,w -Sum).Sum
If (-not $diffs -or ($diffs[0] -le 5 -and $diffs[1] -le 5)) {
  Write-Output "No substantial changes, returning."
  Return
}

git add .
git commit -m "Autogenerating Documentation $(Get-Date)"
git push
Pop-Location



















