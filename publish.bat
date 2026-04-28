@echo off
chcp 65001 > nul
title TrickLor — Build EXE

echo.
echo ╔══════════════════════════════════════╗
echo ║    TrickLor — Build Single EXE  ║
echo ╚══════════════════════════════════════╝
echo.

:: Kiểm tra dotnet
where dotnet >nul 2>&1
if %errorlevel% neq 0 (
    echo [LỖI] Không tìm thấy .NET SDK. Tải tại: https://dot.net
    pause
    exit /b 1
)

echo [1/3] Đang restore packages...
dotnet restore TrickLor.csproj
if %errorlevel% neq 0 (
    echo [LỖI] Restore thất bại.
    pause
    exit /b 1
)

echo.
echo [2/3] Đang build + đóng gói...
dotnet publish TrickLor.csproj ^
    -c Release ^
    -r win-x64 ^
    --self-contained true ^
    -p:PublishSingleFile=true ^
    -p:IncludeNativeLibrariesForSelfExtract=true ^
    -p:PublishTrimmed=false ^
    --nologo

if %errorlevel% neq 0 (
    echo [LỖI] Build thất bại.
    pause
    exit /b 1
)

echo.
echo [3/3] Hoàn tất!
echo.
echo ✅ File EXE nằm tại:
echo    bin\Release\net10.0-windows\win-x64\publish\TrickLor.exe
echo.

:: Mở thư mục output tự động
start "" "bin\Release\net10.0-windows\win-x64\publish"

pause
