function CreateAssemblyKeyFile([string] $environmentVariableName, [string] $fileName) {
	$base64string = [Environment]::GetEnvironmentVariable($environmentVariableName)
	$bytes = [Convert]::FromBase64String($base64string)
	$filePath = [IO.Path]::Combine((Get-Location).Path, $fileName)
	Write-Host "Writing from $environmentVariableName environment variable to $filePath"
	Set-Content -Path $filePath -Value $bytes -Encoding Byte
}

function Get-NugetExe() {
    $nugetExe = [IO.Path]::Combine((Get-Location).Path, "nuget.exe")
	Write-Host "Downloading from nuget.org to $nugetExe"
    (new-object Net.WebClient).DownloadFile("http://www.nuget.org/nuget.exe", $nugetExe)
	return $nugetExe
}

Export-ModuleMember -Function CreateAssemblyKeyFile
Export-ModuleMember -Function Get-NuGetExe