<#
.SYNOPSIS
    ProcessDaemon 一键构建与打包脚本
.DESCRIPTION
    支持框架依赖模式与独立自包含 (Self-Contained) 模式发布
#>
param(
    [switch]$SelfContained = $false
)

Write-Host ">>> 正在检测 dotnet 环境..." -ForegroundColor Cyan
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Error "未找到 dotnet SDK。请安装 .NET 8.0 SDK: https://dotnet.microsoft.com/download/dotnet/8.0"
    exit 1
}

$outputDir = Join-Path $PSScriptRoot "publish"
if ($SelfContained) {
    Write-Host ">>> 正在生成【自包含独立单文件版】(目标机器无需安装任何 .NET 运行时)..." -ForegroundColor Yellow
    dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true -o (Join-Path $outputDir "standalone")
} else {
    Write-Host ">>> 正在生成【轻量框架依赖版】(单文件体积仅数 MB)..." -ForegroundColor Green
    dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o (Join-Path $outputDir "framework-dependent")
}

Write-Host "`n>>> 发布完成！已输出至: $outputDir" -ForegroundColor Green
