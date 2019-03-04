Remove-Item -Recurse -Force -ErrorAction Ignore docs

git config --global credential.helper store
Add-Content "$HOME\.git-credentials" "https://$($env:access_token):x-oauth-basic@github.com`n"
git config --global user.name "Appveyor"
git config --global user.email "$($env:APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL)"
git clone -q --single-branch --branch gh-pages "$(git remote get-url origin)" docs

choco install docfx -y
docfx .\docfx_project\docfx.json

Push-Location .\docs

git add .
git commit -m "Autogenerating Documentation $(Get-Date)"
git push

Pop-Location
