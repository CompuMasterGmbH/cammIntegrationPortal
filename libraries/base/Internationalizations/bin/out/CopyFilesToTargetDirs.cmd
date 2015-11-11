@echo off
echo Kopieren in lokale SVN Pfade relativ zum aktuellen Pfad 
echo (CWM muss hierzu hier vollstaendig ausgecheckt sein)
echo Nach Start und Erledigung des Vorgangs bitte Aenderungen dort committen!
pause
XCOPY /y cwm-lib\*.* ..\..\..\cammWM\
XCOPY /y db-sqls\*.* ..\..\..\..\..\database\
echo.
::KÖNNTE EVTL. AUCH ÜBER SVN-PFADE GEHEN WIE OBEN, JEDOCH UNKLAR OB NANT-NIGHTLY AUF CWEZEL EIN SVN-UPDATE AUSFÜHRT --> GGF. TODO TO CHECK
echo Kopieren zu CWEZEL ueber R:\NonServerDevices\CWezel 
echo Nach Start und Erledigung des Vorgangs bitte Aenderungen dort committen!
pause
XCOPY /y /s web-scripts-asp\*.* R:\NonServerDevices\CWezel\InetPub\cwmroot_asp\
XCOPY /y /s web-scripts-aspx\*.* R:\NonServerDevices\CWezel\InetPub\cwmroot_aspx\
echo.
echo Bitte JETZT Aenderungen committen - hierzu werden die Verzeichnisses
echo im Windows Explorer geoeffnet
pause
start explorer.exe R:\NonServerDevices\CWezel\InetPub
start explorer.exe ..\..\..\cammWM
start explorer.exe ..\..\..\..\..\database
