# This is a PowerShell Unit Test file. To run tests
# PM> Import-Module .\packages\Pester.3.3.8\tools\Pester.psm1
# PM> Set-Location .\build
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

	Context "Get-NugetExe" {
		It "Downloads nuget.exe and returns a file path" {
			$nugetExe = Get-NugetExe
			$nugetExe | Should Exist
			Remove-Item $nugetExe
		}
	}
}

Remove-Module Module