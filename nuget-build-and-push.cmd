::REM -UpdateNuGetExecutable not required since it's updated by VS.NET mechanisms
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& 'libraries\base\cammWM\_CreateNewNuGetPackage\DoNotModify\New-NuGetPackage.ps1' -ProjectFilePath '.\libraries\base\cammWM\cammWM.VS2015.OpenSource.vbproj' -verbose -NoPrompt -PushPackageToNuGetGallery"
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& 'libraries\adminarea\cammWM.Admin\_CreateNewNuGetPackage\DoNotModify\New-NuGetPackage.ps1' -ProjectFilePath '.\libraries\adminarea\cammWM.Admin\cammWM.Admin.OpenSource.VS2015.vbproj' -verbose -NoPrompt -PushPackageToNuGetGallery"
::TODO
::- Mono vs. Win
::- ClientApp vs. WebApp vs. WebRoot-withAdminArea vs. WebRoot-withoutAdminArea
pause