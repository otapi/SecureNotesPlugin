@ECHO OFF
ECHO Creating KeePass plugin package
set PLUGIN_NAME=SecureNotesPlugin
SET PP=%~dp0
SET PARENT_PATH=%PP:~0,-1%
cd %~dp0
rd /s /q %~dp0\obj %~dp0\bin 2>nul
del ..\%PLUGIN_NAME%.plgx 2>nul
del "C:\Program Files (x86)\KeePass Password Safe 2\plugins\%PLUGIN_NAME%.plgx" 2>nul
"C:\Program Files (x86)\KeePass Password Safe 2\KeePass.exe" --plgx-create %PARENT_PATH%
move ..\%PLUGIN_NAME%.plgx "C:\Program Files (x86)\KeePass Password Safe 2\plugins\%PLUGIN_NAME%.plgx"
ECHO KeePass Plugin package has been created and copied to plugins, let's start KeePass
PAUSE

