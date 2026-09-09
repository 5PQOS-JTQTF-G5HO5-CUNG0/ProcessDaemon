@echo off
chcp 65001 > nul
echo ========================================================
echo  ProcessDaemon - 编译与单文件打包脚本 (.NET 8.0/9.0)
echo ========================================================

where dotnet >nul 2>nul
if %errorlevel% neq 0 (
    echo [错误] 系统未检测到 dotnet 命令，请先安装 .NET 8.0 SDK 或确保已加入 PATH。
    pause
    exit /b 1
)

echo.
echo [1/2] 正在编译 Debug 版本进行语法与静态检查...
dotnet build -c Debug
if %errorlevel% neq 0 (
    echo [错误] Debug 编译未通过！
    pause
    exit /b 1
)

echo.
echo [2/2] 正在发布 Release 独立单文件 (Single-File)...
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o ./publish

if %errorlevel% equ 0 (
    echo.
    echo ========================================================
    echo  打包发布成功！输出目录: ./publish
    echo  单文件可执行程序: ./publish/ProcessDaemon.exe
    echo ========================================================
) else (
    echo [错误] 发布单文件失败！
)

pause
