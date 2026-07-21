Write-Host "🧪 Running tests..."

# Limpa resultados antigos
Remove-Item -Recurse -Force coverage, TestResults -ErrorAction Ignore

# Executa testes com coverage
dotnet test `
  --collect:"XPlat Code Coverage" `
  --results-directory TestResults

Write-Host "📊 Generating reports..."

# Gera relatório HTML (ajuste principal aqui 👇)
dotnet ReportGenerator/ReportGenerator.dll `
  -reports:**/TestResults/**/coverage.cobertura.xml `
  -targetdir:coverage `
  -reporttypes:Html

Write-Host "✅ Done: coverage/index.html"