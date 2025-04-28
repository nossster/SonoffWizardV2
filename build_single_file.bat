@echo off
REM ── build_single_file.bat ──────────────────────────────
REM Требует установленный .NET 8 SDK (или VS 2022+).

set CONFIG=Release
set RUNTIME=win-x64
set PUBLISH_DIR=publish_single

dotnet publish SonoffWizardV2.csproj ^
 -c %CONFIG% ^
 -r %RUNTIME% ^
 -o %PUBLISH_DIR% ^
 /p:PublishSingleFile=true ^
 /p:SelfContained=true ^
 /p:IncludeAllContentForSelfExtract=true

echo ======================================================
echo ready! check it: %PUBLISH_DIR%\SonoffWizardV2.exe
pause


