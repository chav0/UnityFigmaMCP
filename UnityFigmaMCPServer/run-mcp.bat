@echo off
setlocal

set "SCRIPT_DIR=%~dp0"
cd /d "%SCRIPT_DIR%"

dotnet build UnityFigmaMCPServer.sln -c Debug --nologo -v q 1>&2

dotnet exec bin\Debug\net8.0\UnityFigmaMCPServer.dll
