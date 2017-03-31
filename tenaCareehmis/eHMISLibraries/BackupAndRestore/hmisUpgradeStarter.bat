cd /D "C:\Program Files\eHMIS\v1\ScheduledBackUpAndRestore"

mkdir ..\runtimeprev

mkdir ..\SqlRecentCheckins

mkdir ..\runTime\SMSService


taskkill /F /IM SMSService.exe


xcopy ..\runTime ..\runTimePrev /S /Y /E 

xcopy ..\ScheduledBackUpAndRestore\HMISUPGRADE\SMSService ..\runTime\SMSService  /S /Y /E

xcopy ..\ScheduledBackUpAndRestore\HMISUPGRADE\SqlRecentCheckins ..\SqlRecentCheckins  /S /Y /E


copy ..\ScheduledBackUpAndRestore\HMISUPGRADE\Facility.1.xml ..\runTime\ConfigurationData /Y

copy ..\ScheduledBackUpAndRestore\HMISUPGRADE\District.1.xml ..\runTime\ConfigurationData /Y



copy ..\ScheduledBackUpAndRestore\HMISUPGRADE\BackUpAndRestore.exe ..\ScheduledBackUpAndRestore /Y

copy ..\ScheduledBackUpAndRestore\HMISUPGRADE\admin.dll ..\ScheduledBackUpAndRestore /Y

copy ..\ScheduledBackUpAndRestore\HMISUPGRADE\Utilities.dll ..\ScheduledBackUpAndRestore /Y

copy ..\ScheduledBackUpAndRestore\HMISUPGRADE\SqlManagement.dll ..\ScheduledBackUpAndRestore /Y

copy ..\ScheduledBackUpAndRestore\HMISUPGRADE\UtilitiesNew.dll ..\ScheduledBackUpAndRestore /Y



copy ..\ScheduledBackUpAndRestore\HMISUPGRADE\MainApp.exe ..\runTime /Y

copy ..\ScheduledBackUpAndRestore\HMISUPGRADE\MainApp.exe.config ..\runTime /Y

copy ..\ScheduledBackUpAndRestore\HMISUPGRADE\Version.txt ..\runTime /Y

copy ..\ScheduledBackUpAndRestore\HMISUPGRADE\ehmis.dll ..\runTime /Y

copy ..\ScheduledBackUpAndRestore\HMISUPGRADE\SqlManagement.dll ..\runTime /Y

copy ..\ScheduledBackUpAndRestore\HMISUPGRADE\admin.dll ..\runTime /Y

copy ..\ScheduledBackUpAndRestore\HMISUPGRADE\Utilities.dll ..\runTime /Y

copy ..\ScheduledBackUpAndRestore\HMISUPGRADE\UtilitiesNew.dll ..\runTime /Y


copy ..\ScheduledBackUpAndRestore\HMISUPGRADE\remoteHmisUpgrade1.bat  ..\ /Y

copy ..\ScheduledBackUpAndRestore\HMISUPGRADE\updateRemoteEthiopiaDb.bat  ..\ /Y

copy ..\ScheduledBackUpAndRestore\HMISUPGRADE\remoteHmisUpgrade2.bat  ..\ /Y

copy ..\ScheduledBackUpAndRestore\HMISUPGRADE\updateRemoteHmisUpgrader.bat  ..\ /Y


cd ..

CALL updateRemoteHmisUpgrader.bat

CALL remoteHmisUpgrade1.bat

CALL updateRemoteEthiopiaDb.bat

CALL remoteHmisUpgrade2.bat


del remoteHmisUpgrade1.bat  /Q 

del updateRemoteEthiopiaDb.bat /Q

del remoteHmisUpgrade2.bat /Q 

del updateRemoteHmisUpgrader.bat /Q

rmdir SqlRecentCheckins /Q /S


del ScheduledBackUpAndRestore\HMISUPGRADE\*.*  /Q /S
