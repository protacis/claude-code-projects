@echo off
set "INVENTOR_PATH=C:\Program Files\Autodesk\Inventor 2026\Bin\Inventor.exe"

if exist "%INVENTOR_PATH%" (
    start "" "%INVENTOR_PATH%"
) else (
    echo Inventor 2026 not found at: %INVENTOR_PATH%
    pause
)
