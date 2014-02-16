param(
	[string]$Name=$([System.Environment]::MachineName),
	[string[]]$Params=$null
)

$dp0 = [System.IO.Path]::GetDirectoryName($myInvocation.MyCommand.Path)

Add-Type -Path "$dp0\Configizer.dll"

[Configizer.Program]::Apply($Name, $Params)

#TODO
# - nice default algorithm to search for "config" directory  

