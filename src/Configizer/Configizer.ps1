$dp0 = [System.IO.Path]::GetDirectoryName($myInvocation.MyCommand.Path)

Add-Type -Path Configizer.dll

[Configizer.Program]::Apply("borko.csconfig", $null)