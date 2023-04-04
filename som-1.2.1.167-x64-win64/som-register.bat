@echo off

SETLOCAL

SET ROOT=%~1
SET TASK=%~2
SET VERS=%~3
SET SOM=%ROOT%\som.exe

REM detect architecture
SET SOMARCH=x86sse2-win32
IF /I "%Processor_Architecture%"=="AMD64" SET SOMARCH=x64-win64
IF /I "%PROCESSOR_ARCHITEW6432%"=="AMD64" SET SOMARCH=x64-win64

IF DEFINED SOMMODE (
  SET MODE=%SOMMODE%
  GOTO :setup
)

net session >nul 2>&1
IF %errorLevel% == 0 (
  SET MODE=system
) else (
  SET MODE=user
)

:setup
IF "%MODE%" == "system" (
  SET RKEY=HKLM
  SET DATA=%ProgramData%
  SET DESK=%PUBLIC%\Desktop
) ELSE (
  SET RKEY=HKCU
  SET DATA=%AppData%
  SET DESK=%USERPROFILE%\Desktop
)

SET PROD=MVTec Software Manager
SET NAME=%PROD% (%MODE%)
SET NAMECLI=%PROD% CLI (%MODE%)
SET MENU=%DATA%\Microsoft\Windows\Start Menu\Programs\%PROD%
SET SOMICON=%SOM%

IF "%TASK%" == "off" (
  GOTO :unregister
)
GOTO :register

:register
reg add "%RKEY%\Software\Microsoft\Windows\CurrentVersion\Uninstall\%NAME%" /v "DisplayName" /t REG_SZ /d "%NAME%" /f >nul
reg add "%RKEY%\Software\Microsoft\Windows\CurrentVersion\Uninstall\%NAME%" /v "DisplayIcon" /t REG_SZ /d "\"%SOM%\"" /f >nul
reg add "%RKEY%\Software\Microsoft\Windows\CurrentVersion\Uninstall\%NAME%" /v "Publisher" /t REG_SZ /d "MVTec Software GmbH" /f >nul
reg add "%RKEY%\Software\Microsoft\Windows\CurrentVersion\Uninstall\%NAME%" /v "InstallLocation" /t REG_SZ /d "\"%ROOT%\"" /f >nul
reg add "%RKEY%\Software\Microsoft\Windows\CurrentVersion\Uninstall\%NAME%" /v "UninstallString" /t REG_SZ /d "\"%SOM%\" remove" /f >nul
reg add "%RKEY%\Software\Microsoft\Windows\CurrentVersion\Uninstall\%NAME%" /v "URLInfoAbout" /t REG_SZ /d "https://www.mvtec.com/" /f >nul

MKDIR "%MENU%"
CALL :Shortcut "%MENU%\%NAME%.lnk" "%SOM%" "" "%SOMICON%"
CALL :Shortcut "%MENU%\%NAMECLI%.lnk" "%SOM%" "cli" "%SOMICON%"
CALL :Shortcut "%DESK%\%NAME%.lnk" "%SOM%" "" "%SOMICON%"

GOTO :refresh

:unregister
reg delete "%RKEY%\Software\Microsoft\Windows\CurrentVersion\Uninstall\%NAME%" /va /f >nul
reg delete "%RKEY%\Software\Microsoft\Windows\CurrentVersion\Uninstall\%NAME%" /f >nul
DEL "%MENU%\%NAME%.lnk"
DEL "%MENU%\%NAMECLI%.lnk"
DEL "%DESK%\%NAME%.lnk"
RMDIR "%MENU%"

:refresh
ie4uinit.exe -ClearIconCache
ie4uinit.exe -show

EXIT /B %ERRORLEVEL%

:Shortcut
set SCRIPT="%TEMP%\%RANDOM%-%RANDOM%-%RANDOM%-%RANDOM%.vbs"
echo Set oWS = WScript.CreateObject("WScript.Shell") >> %SCRIPT%
echo sLinkFile = "%~1" >> %SCRIPT%
echo Set oLink = oWS.CreateShortcut(sLinkFile) >> %SCRIPT%
echo oLink.TargetPath = "%~2" >> %SCRIPT%
IF NOT [%~3] == [] (
  echo oLink.Arguments = "%~3" >> %SCRIPT%
)
echo oLink.IconLocation = "%~4" >> %SCRIPT%
echo oLink.Save >> %SCRIPT%
cscript /nologo %SCRIPT%
del %SCRIPT%
EXIT /B 0
