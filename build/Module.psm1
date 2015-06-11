function CreateAssemblyKeyFile([string] $environmentVariableName, [string] $fileName) {
	$base64string = [Environment]::GetEnvironmentVariable($environmentVariableName)
	$bytes = [Convert]::FromBase64String($base64string)
	$filePath = [IO.Path]::Combine((Get-Location).Path, $fileName)
	Write-Host "Writing from $environmentVariableName environment variable to $filePath"
	Set-Content -Path $filePath -Value $bytes -Encoding Byte
}

Export-ModuleMember -Function CreateAssemblyKeyFile