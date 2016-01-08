if exist ..\dbsetup\ResourceWriter.exe goto dbsetup
if exist ..\..\..\database\ResourceWriter.exe goto database
goto waitbeforequit

:dbsetup
..\dbsetup\ResourceWriter.exe 
goto end

:database
..\..\..\database\ResourceWriter.exe 
goto end

:waitbeforequit
pause

:end