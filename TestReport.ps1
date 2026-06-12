Write-Host "Running tests..."

Remove-Item -Recurse -Force coverage, TestResults -ErrorAction Ignore

dotnet test --collect:"XPlat Code Coverage" --results-directory TestResults

Write-Host "Generating reports..."

dotnet tools/reportgenerator/ReportGenerator.dll -reports:**/coverage.cobertura.xml -targetdir:coverage -reporttypes:Html

Write-Host "Done: coverage/index.html"