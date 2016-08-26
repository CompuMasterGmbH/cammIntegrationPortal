@echo off
::REM -UpdateNuGetExecutable not required since it's updated by VS.NET mechanisms

echo Build+Push cammWM / CIP SmartEditor - Base.Library ?
choice /n /c "YN" /m "(Y)es, (N)o"
if errorlevel 2 goto FinishedBuildCSEBase
echo Building + pushing...
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& 'libraries\smarteditor\baseeditor\_CreateNewNuGetPackage\DoNotModify\New-NuGetPackage.ps1' -ProjectFilePath '.\libraries\smarteditor\baseeditor\cammWM.SmartEditor.vbproj' -verbose -NoPrompt -DoNotUpdateNuSpecFile -PushPackageToNuGetGallery"
:FinishedBuildCSEBase
echo Task completed.
echo.

echo Build+Push cammWM / CIP SmartEditor - CommonMark.Library ?
choice /n /c "YN" /m "(Y)es, (N)o"
if errorlevel 2 goto FinishedBuildCSECommonMark
echo Building + pushing...
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& 'libraries\smarteditor\CommonMarkEditor\_CreateNewNuGetPackage\DoNotModify\New-NuGetPackage.ps1' -ProjectFilePath '.\libraries\smarteditor\CommonMarkEditor\cammWM.CommonMarkEditor.vbproj' -verbose -NoPrompt -DoNotUpdateNuSpecFile -PushPackageToNuGetGallery"
:FinishedBuildCSECommonMark
echo Task completed.
echo.

echo Build+Push cammWM / CIP SmartEditor - Upload.Library ?
choice /n /c "YN" /m "(Y)es, (N)o"
if errorlevel 2 goto FinishedBuildCSEUpload
echo Building + pushing...
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& 'libraries\smarteditor\webupload\_CreateNewNuGetPackage\DoNotModify\New-NuGetPackage.ps1' -ProjectFilePath '.\libraries\smarteditor\webupload\cammWM.SmartEditorUpload.vbproj' -verbose -NoPrompt -DoNotUpdateNuSpecFile -PushPackageToNuGetGallery"
:FinishedBuildCSEUpload
echo Task completed.
echo.

pause