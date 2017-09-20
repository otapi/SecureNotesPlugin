ECHO Creating KeePass plugin package
set PLUGIN_NAME=KeeOneDriveSync
rd /s /q %~dp0\obj %~dp0\bin
del %~dp0%PLUGIN_NAME%.plgx
"C:\Program Files (x86)\KeePass Password Safe 2\KeePass.exe" --plgx-create %~dp0
PAUSE
'ren %~dp0%SourceFolder%.plgx %PLUGIN_NAME%.plgx
ECHO KeePass Plugin package has been created
