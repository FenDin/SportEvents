param(
    [string]$ConnectionString = "Server=localhost;Database=sports-competitions;Trusted_Connection=True;TrustServerCertificate=True",
    [string]$Provider = "Microsoft.EntityFrameworkCore.SqlServer",
    [string]$ContextName = "SportsCompetitionsDbContext",
    [string]$ModelsDir = "Models\Db",
    [string]$ContextDir = "Data",
    [switch]$SkipBuildBeforeScaffold = $true
)

Clear-Host
$ErrorActionPreference = "Stop"

function Ensure-Dir([string]$path) {
    if (-not (Test-Path $path)) {
        Write-Host "Создаём папку $path"
        New-Item -ItemType Directory -Path $path | Out-Null
    }
}

function Clear-Dir([string]$path) {
    if (Test-Path $path) {
        Write-Host "Очищаем папку $path"
        Get-ChildItem $path -Recurse -Force -ErrorAction SilentlyContinue |
            Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
    }
    else {
        Ensure-Dir $path
    }
}

function Ensure-Package([string]$packageName) {
    Write-Host "Проверка пакета $packageName..."
    if (-not (dotnet list package | Select-String -SimpleMatch $packageName)) {
        Write-Host "Пакет не найден. Устанавливаем..."
        dotnet add package $packageName
        if ($LASTEXITCODE -ne 0) {
            throw "Ошибка установки $packageName"
        }
    }
    else {
        Write-Host "Пакет установлен."
    }
}

# ===============================
# Защита от удаления Models
# ===============================
if ($ModelsDir -eq "Models") {
    throw "Нельзя использовать корневую папку Models. Используй подпапку, например Models\Db."
}

# ===============================
# Проверка dotnet-ef
# ===============================
Write-Host "Проверка dotnet-ef..."
if (-not (dotnet tool list -g | Select-String "dotnet-ef")) {
    Write-Host "dotnet-ef не найден. Устанавливаем..."
    dotnet tool install --global dotnet-ef
    if ($LASTEXITCODE -ne 0) { throw "Ошибка установки dotnet-ef" }
}

# ===============================
# EF пакеты (как для пустого проекта)
# ===============================
Ensure-Package "Microsoft.EntityFrameworkCore"
Ensure-Package "Microsoft.EntityFrameworkCore.SqlServer"
Ensure-Package "Microsoft.EntityFrameworkCore.Design"
Ensure-Package "Microsoft.EntityFrameworkCore.Tools"

# ===============================
# Подготовка папок
# ===============================
Clear-Dir $ModelsDir
Ensure-Dir $ContextDir
# ===============================
# Удаление старого DbContext
# ===============================
$dbContextPath = Join-Path $ContextDir "$ContextName.cs"

if (Test-Path $dbContextPath) {
    Write-Host "Удаляем старый DbContext: $dbContextPath"
    Remove-Item $dbContextPath -Force
}

# ===============================
# (Опционально) проверка сборки ДО scaffold
# Обычно НЕ нужна, потому что мы сами удаляем сущности и сборка упадёт.
# ===============================
if (-not $SkipBuildBeforeScaffold) {
    Write-Host "Проверка сборки ДО scaffold..."
    dotnet build
    if ($LASTEXITCODE -ne 0) {
        throw "Проект не собирается. Исправь ошибки и запусти снова."
    }
}

# ===============================
# Scaffold
# ===============================
Write-Host "Запуск scaffold..."
dotnet ef dbcontext scaffold `
"$ConnectionString" `
$Provider `
--context $ContextName `
--context-dir $ContextDir `
--output-dir $ModelsDir `
--namespace "MVC.Core.Sports_competitions.Models.Db" `
--context-namespace "MVC.Core.Sports_competitions.Data" `
--schema dbo `
--use-database-names `
--no-onconfiguring `
--force

if ($LASTEXITCODE -ne 0) {
    throw "Scaffold завершился с ошибкой."
}

# ===============================
# Проверка сборки ПОСЛЕ scaffold
# ===============================
Write-Host "Проверка сборки ПОСЛЕ scaffold..."
dotnet build
if ($LASTEXITCODE -ne 0) {
    throw "После scaffold проект всё ещё не собирается. Смотри ошибки выше."
}

Write-Host ""
Write-Host "✔ Сущности успешно сгенерированы в $ModelsDir" -ForegroundColor Green