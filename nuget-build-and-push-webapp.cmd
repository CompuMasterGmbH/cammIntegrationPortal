@echo off
::REM -UpdateNuGetExecutable not required since it's updated by VS.NET mechanisms

set /p PackageVersion=Enter version no. for NuGet package:

echo Build+Push cammWM.WebApp Library ?
choice /n /c "YN" /m "(Y)es, (N)o"
if errorlevel 2 goto FinishedBuildCWMWebApp
::echo NuGet self-update...
::PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& 'libraries\cammWM.WebApp\_CreateNewNuGetPackage\DoNotModify\NuGet.exe' update -self"
echo Building + pushing...
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& 'libraries\cammWM.WebApp\_CreateNewNuGetPackage\DoNotModify\New-NuGetPackage.ps1' -ProjectFilePath '.\libraries\cammWM.WebApp\cammWM.WebApp.vbproj' -verbose -NoPrompt -DoNotUpdateNuSpecFile -PushPackageToNuGetGallery"
::PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& 'libraries\cammWM.WebApp\_CreateNewNuGetPackage\DoNotModify\New-NuGetPackage.ps1' -ProjectFilePath '.\libraries\cammWM.WebApp\cammWM.WebApp.vbproj' -verbose -NoPrompt -DoNotUpdateNuSpecFile"
::PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& 'libraries\cammWM.WebApp\_CreateNewNuGetPackage\DoNotModify\New-NuGetPackage.ps1' -ProjectFilePath '.\libraries\cammWM.WebApp\cammWM.WebApp.vbproj' -verbose -NoPrompt -DoNotUpdateNuSpecFile -VersionNumber %PackageVersion% -PushPackageToNuGetGallery"
:FinishedBuildCWMWebApp
echo Task completed.
echo.
::goto AbortNoPause

echo Build+Push cammWM.WebRootApp Library ?
choice /n /c "YN" /m "(Y)es, (N)o"
if errorlevel 2 goto FinishedBuildCWMWebRootApp
echo Building + pushing...
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& 'libraries\cammWM.WebRootApp\_CreateNewNuGetPackage\DoNotModify\New-NuGetPackage.ps1' -ProjectFilePath '.\libraries\cammWM.WebRootApp\cammWM.WebRootApp.vbproj' -verbose -NoPrompt -DoNotUpdateNuSpecFile -PushPackageToNuGetGallery"
::PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& 'libraries\cammWM.WebRootApp\_CreateNewNuGetPackage\DoNotModify\New-NuGetPackage.ps1' -ProjectFilePath '.\libraries\cammWM.WebRootApp\cammWM.WebRootApp.vbproj' -verbose -NoPrompt -DoNotUpdateNuSpecFile" 
:FinishedBuildCWMWebRootApp
echo Task completed.
echo.

echo Build+Push cammWM.WebRootAdminApp Library ?
choice /n /c "YN" /m "(Y)es, (N)o"
if errorlevel 2 goto FinishedBuildCWMWebRootAdminApp
echo Building + pushing...
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& 'libraries\cammWM.WebRootAdminApp\_CreateNewNuGetPackage\DoNotModify\New-NuGetPackage.ps1' -ProjectFilePath '.\libraries\cammWM.WebRootAdminApp\cammWM.WebRootAdminApp.vbproj' -verbose -NoPrompt -DoNotUpdateNuSpecFile -PushPackageToNuGetGallery"
::PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& 'libraries\cammWM.WebRootAdminApp\_CreateNewNuGetPackage\DoNotModify\New-NuGetPackage.ps1' -ProjectFilePath '.\libraries\cammWM.WebRootAdminApp\cammWM.WebRootAdminApp.vbproj' -verbose -NoPrompt -DoNotUpdateNuSpecFile" 
:FinishedBuildCWMWebRootAdminApp
echo Task completed.
echo.

pause
:AbortNoPause