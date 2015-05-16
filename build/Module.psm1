function CreateAssemblyKeyFile([string] $environmentVariableName, [string] $fileName) {
	$base64string = [Environment]::GetEnvironmentVariable($environmentVariableName)
	$bytes = [Convert]::FromBase64String($base64string)
	$filePath = (Get-Location).Path + $fileName
	Write-Host "Writing $environmentVariableName to $filePath"
	Set-Content -Path $filePath -Value $bytes -Encoding Byte
}

function Get-NugetExe() {
    $currentDirectory = (Get-Location).Path
    $nugetExe = "$currentDirectory\nuget.exe"
	Write-Host "Downloading from nuget.org to $nugetExe"
    (new-object Net.WebClient).DownloadFile("http://www.nuget.org/nuget.exe", $nugetExe)
	return $nugetExe
}

Export-ModuleMember -Function CreateAssemblyKeyFile
Export-ModuleMember -Function Get-NuGetExe