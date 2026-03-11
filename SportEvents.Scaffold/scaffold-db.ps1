param(
    [string]$ConnectionString = "Server=localhost;Database=sports-competitions;Trusted_Connection=True;TrustServerCertificate=True",
    [string]$Provider = "Microsoft.EntityFrameworkCore.SqlServer",
    [string]$ContextName = "SportsCompetitionsDbContext",
    [string]$ModelsDir = "Models\Db",
    [string]$ContextDir = "Data",
    [string]$WebProjectPath = "..\SportEvents.Web",
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

function Copy-DirContents([string]$source, [string]$destination) {
    Ensure-Dir $destination

    Write-Host "Копируем из $source в $destination"
    Copy-Item -Path (Join-Path $source '*') -Destination $destination -Recurse -Force
}

# ===============================
# Абсолютные пути
# ===============================
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$WebProjectFullPath = Resolve-Path (Join-Path $ScriptDir $WebProjectPath)

$SourceModelsPath = Join-Path $ScriptDir $ModelsDir
$SourceContextPath = Join-Path $ScriptDir $ContextDir

$TargetModelsPath = Join-Path $WebProjectFullPath $ModelsDir
$TargetContextPath = Join-Path $WebProjectFullPath $ContextDir

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
# EF пакеты в scaffold-проекте
# ===============================
Ensure-Package "Microsoft.EntityFrameworkCore"
Ensure-Package "Microsoft.EntityFrameworkCore.SqlServer"
Ensure-Package "Microsoft.EntityFrameworkCore.Design"
Ensure-Package "Microsoft.EntityFrameworkCore.Tools"

# ===============================
# Подготовка исходных папок scaffold
# ===============================
Clear-Dir $SourceModelsPath
Ensure-Dir $SourceContextPath

# ===============================
# Удаление старого DbContext в scaffold
# ===============================
$dbContextPath = Join-Path $SourceContextPath "$ContextName.cs"

if (Test-Path $dbContextPath) {
    Write-Host "Удаляем старый DbContext: $dbContextPath"
    Remove-Item $dbContextPath -Force
}

# ===============================
# Проверка сборки ДО scaffold
# ===============================
if (-not $SkipBuildBeforeScaffold) {
    Write-Host "Проверка сборки ДО scaffold..."
    dotnet build
    if ($LASTEXITCODE -ne 0) {
        throw "Проект SportEvents.Scaffold не собирается. Исправь ошибки и запусти снова."
    }
}

# ===============================
# Scaffold в текущий проект SportEvents.Scaffold
# ===============================
Write-Host "Запуск scaffold..."
dotnet ef dbcontext scaffold `
    "$ConnectionString" `
    $Provider `
    --context $ContextName `
    --context-dir $ContextDir `
    --output-dir $ModelsDir `
    --namespace "SportEvents.Scaffold.Models.Db" `
    --context-namespace "SportEvents.Scaffold.Data" `
    --schema dbo `
    --use-database-names `
    --no-onconfiguring `
    --force

if ($LASTEXITCODE -ne 0) {
    throw "Scaffold завершился с ошибкой."
}

# ===============================
# Подготовка целевых папок WEB
# ===============================
Clear-Dir $TargetModelsPath
Ensure-Dir $TargetContextPath

$targetDbContextPath = Join-Path $TargetContextPath "$ContextName.cs"
if (Test-Path $targetDbContextPath) {
    Write-Host "Удаляем старый DbContext в web: $targetDbContextPath"
    Remove-Item $targetDbContextPath -Force
}

# ===============================
# Копирование из Scaffold в Web
# ===============================
Copy-DirContents $SourceModelsPath $TargetModelsPath
Copy-DirContents $SourceContextPath $TargetContextPath

# ===============================
# Замена namespace после копирования
# ===============================
Write-Host "Исправляем namespace в скопированных файлах..."

Get-ChildItem $TargetModelsPath -Recurse -Filter *.cs | ForEach-Object {
    (Get-Content $_.FullName) `
        -replace 'namespace SportEvents\.Scaffold\.Models\.Db', 'namespace SportEvents.Web.Models.Db' `
        -replace 'using SportEvents\.Scaffold\.Models\.Db;', 'using SportEvents.Web.Models.Db;' `
        -replace 'using SportEvents\.Scaffold\.Data;', 'using SportEvents.Web.Data;' |
        Set-Content $_.FullName
}

Get-ChildItem $TargetContextPath -Recurse -Filter *.cs | ForEach-Object {
    (Get-Content $_.FullName) `
        -replace 'namespace SportEvents\.Scaffold\.Data', 'namespace SportEvents.Web.Data' `
        -replace 'using SportEvents\.Scaffold\.Models\.Db;', 'using SportEvents.Web.Models.Db;' |
        Set-Content $_.FullName
}

# ===============================
# Проверка сборки WEB-проекта
# ===============================
$WebProjectFile = Get-ChildItem -Path $WebProjectFullPath -Filter *.csproj | Select-Object -First 1
if (-not $WebProjectFile) {
    throw "Не найден .csproj в папке $WebProjectFullPath"
}

Write-Host "Проверка сборки WEB-проекта..."
dotnet build $WebProjectFile.FullName
if ($LASTEXITCODE -ne 0) {
    throw "После копирования SportEvents.Web не собирается. Смотри ошибки выше."
}

Write-Host ""
Write-Host "✔ Сущности успешно сгенерированы в SportEvents.Scaffold и перенесены в SportEvents.Web" -ForegroundColor Green