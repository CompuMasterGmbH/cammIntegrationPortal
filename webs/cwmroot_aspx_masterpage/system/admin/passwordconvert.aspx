<%@ Page Language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.PasswordConvert" %>

<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Modify user account" id="cammWebManager"
    SecurityObject="System - User Administration - ServerSetup" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>

<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
<h3>
<font face="Arial">Administration - Password conversion</font></h3>
<asp:label runat="server" id="lblMsg" forecolor="green" />
<asp:panel runat="server" id="pnlPage">
<br /><br />
<b>Conversion</b><br>
This page allows you to convert the existing passwords. In order to reduce load on the webserver the conversion process will pause between requests. <br>
<span id="status"></span><br>
<span id="error"></span><br>
<a href="#" id="beginconversion">Begin conversion</a><a href="#" id="pauseconversion">Pause conversion</a>
<br><br>
<b>Log</b><br>
<div id="log"></div>
</asp:panel>
<script src="/system/admin/scripts/jquery-1.11.1.min.js" type="text/javascript"></script>
<script type="text/javascript">

var isConversionRunning = 0;
var currentlyProcessedPasswords = 0;
var totalPasswords = 0;
var convertRequest = NaN;
var currentTime = 0;
var requestNumber = 0;
function addStatusToLog(newlyProcessed)
{
	currentlyProcessedPasswords += parseInt(newlyProcessed);
	msg = "Processed: " + currentlyProcessedPasswords + "/ " + totalPasswords
	addLog(msg);
}

function setTotalPasswordsAndStartConverting(t)
{
	totalPasswords = t;
	addStatusToLog(0);
	processBatch();
}

function beginConversion()
{
	if(totalPasswords == 0)
	{
		addLog("Calculating how much passwords must be processed, please wait...");
		$.get("ajax_passwordconvert.aspx?action=count", setTotalPasswordsAndStartConverting).fail(serverError);
	}
	else
	{
		addStatusToLog(0);
		processBatch();
	}
}

function processError()
{
	serverError();
}

function processFinished()
{
	addLog("The conversion has finished successfully!");
	$("#pauseconversion").hide();
	isConversionRunning=0;
}
function processResponse(r)
{
	r = parseInt(r);
	if(isNaN(r) || r == -1)
	{
		processError();
		return;
	}
	if(r == 0)
	{
		processFinished();
		return;
	}
	addLog("Finished request " + requestNumber);
	addStatusToLog(r);
		
	var passedSeconds = (new Date().getTime() - currentTime) / 1000;
	if(passedSeconds < 60)
		passedSeconds = passedSeconds * 2; //by requirement: sleep twice the amount the last request took in order to reduce server load.
	else
		passedSeconds = 60;
	
	addLog("Sleeping for " + passedSeconds + " seconds before proceeding with next request");
	setTimeout(processBatch, passedSeconds * 1000);
}

function serverError(jqXHR, textstatus)
{
	if(textstatus != "abort")
	{
		addLog("Request " + requestNumber + " failed due to error on server:" + textstatus);
		isConversionRunning=0;
		$("#error").text("An error on the server occurred. Please try again later");
	}
	addLog("Paused request Nr.: " + textstatus)
}

function addLog(message)
{
	datestr = new Date().toString();
	
	logfield = $("#log");
	logfield.html(logfield.html() + "<br>" + datestr + ": " + message);
}

function processBatch()
{
	if(isConversionRunning)
	{
		currentTime = new Date().getTime();
		++requestNumber;
		addLog("Starting request Nr.: " + requestNumber);
		convertRequest = $.get("ajax_passwordconvert.aspx?action=convert", processResponse).fail(serverError);
	}
}

function processReset(r)
{
	if($.trim(r) == "ok")
	{
		$("#beginconversion").show();
		$("#beginconversion").text("Continue conversion");
		msg = "Pause successful. " + currentlyProcessedPasswords + " have been processed. " + totalPasswords + "in total."
		addLog(msg);
		$("#pauseconversion").hide();
	}
}

$("#beginconversion").click(function() {
if(isConversionRunning == 0)
{
	isConversionRunning=1;
	beginConversion();
	
}
$("#pauseconversion").show();
$("#beginconversion").hide();
});


$("#pauseconversion").click(function() {
	convertRequest.abort();
	isConversionRunning=0;	
	$("#beginconversion").hide();
	addLog("Resetting session, please wait");
	$.get("ajax_passwordconvert.aspx?action=reset", processReset).fail(serverError);
	
});

$(document).ready(function()
{
$("#pauseconversion").hide();

});

</script>

<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="configuration.aspx" id="cammWebManagerAdminMenu"
    runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
