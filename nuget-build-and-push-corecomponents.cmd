@echo off
::REM -UpdateNuGetExecutable not required since it's updated by VS.NET mechanisms

echo Build+Push cammWM / CIP.Library ?
choice /n /c "YN" /m "(Y)es, (N)o"
if errorlevel 2 goto FinishedBuildCIPLib
echo Building + pushing...
::PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& 'libraries\base\cammWM\_CreateNewNuGetPackage\DoNotModify\New-NuGetPackage.ps1' -ProjectFilePath '.\libraries\base\cammWM\cammWM.VS2015.OpenSource.vbproj' -verbose -NoPrompt -DoNotUpdateNuSpecFile"
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& 'libraries\base\cammWM\_CreateNewNuGetPackage\DoNotModify\New-NuGetPackage.ps1' -ProjectFilePath '.\libraries\base\cammWM\cammWM.VS2015.OpenSource.vbproj' -verbose -NoPrompt -DoNotUpdateNuSpecFile -PushPackageToNuGetGallery"
:FinishedBuildCIPLib
echo Task completed.
echo.

echo Build+Push cammWM / CIP.Admin.Library ?
choice /n /c "YN" /m "(Y)es, (N)o"
if errorlevel 2 goto FinishedBuildCIPAdminLib
echo Building + pushing...
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& 'libraries\adminarea\cammWM.Admin\_CreateNewNuGetPackage\DoNotModify\New-NuGetPackage.ps1' -ProjectFilePath '.\libraries\adminarea\cammWM.Admin\cammWM.Admin.OpenSource.VS2015.vbproj' -verbose -NoPrompt -DoNotUpdateNuSpecFile -PushPackageToNuGetGallery"
:FinishedBuildCIPAdminLib
echo Task completed.
echo.

::TODO
::- Mono vs. Win
::- ClientApp vs. WebApp vs. WebRoot-withAdminArea vs. WebRoot-withoutAdminArea
pause