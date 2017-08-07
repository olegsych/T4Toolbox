# This is a PowerShell Unit Test file. To run tests
# PM> Install-Module Pester -Scope CurrentUser
# PM> Invoke-Pester

Import-Module .\Module.psm1

Describe "Module" {
	Context "CreateAssemblyFile" {
		$originalLocation = Get-Location

		It "Converts value of environment variable from Base64 to binary and writes it to file in current location" {
			$keyBytes = [Guid]::NewGuid().ToByteArray()
			$env:TestKey = [Convert]::ToBase64String($keyBytes)
			Set-Location "TestDrive:\" 

			CreateAssemblyKeyFile "TestKey" "Test.snk"
			
			$fileBytes = Get-Content -Path "TestDrive:\Test.snk" -Encoding Byte
			$fileBytes | Should Be $keyBytes
		}

		Set-Location $originalLocation
	}
}

Remove-Module Module