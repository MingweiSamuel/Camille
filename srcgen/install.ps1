Set-Location $PSScriptRoot

$mtx = New-Object System.Threading.Mutex($False, 'CamilleNpmMutex')

If (-Not ($mtx.WaitOne(30000))) {
    Write-Error 'Failed to acquire mutex.'
    Exit 10
}

Try {
    If ((Test-Path 'node_modules') -And ((Get-Item 'node_modules')[0].LastWriteTime `
        -GE (Get-Item 'package-lock.json')[0].LastWriteTime))
    {
        Write-Output 'No npm install needed.'
    }
    Else {
        If ([Version]$(npm -v) -GE [Version]'5.7.0') {
            npm ci --loglevel error
        } Else {
            npm install
        }
    }
}
Finally {
    $mtx.ReleaseMutex()
}
