Option Explicit On
Option Strict On

Imports System.Web

Namespace CompuMaster.camm.SmartWebEditor

    ''' <summary>
    '''     The common interface for all SmartWcms editor controls
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminsupport]	18.02.2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Interface ISmartWcmsEditor

        ReadOnly Property cammWebManager() As CompuMaster.camm.WebManager.IWebManager

    End Interface

End Namespace