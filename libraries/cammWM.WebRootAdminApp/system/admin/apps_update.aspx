<%@ Page ValidateRequest="False" Language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.ApplicationUpdate" %>

<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Modify application" id="cammWebManager"
    SecurityObject="System - User Administration - Applications" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
<h3>
    <font face="Arial">Administration - Modify application</font></h3>
    <font face="Arial" size="2" color="red">
        <asp:label runat="server" id="lblErrMsg" />
	</font>
<table cellspacing="0" cellpadding="0" bgcolor="#ffffff" border="0" bordercolor="#C1C1C1">
    <tbody>
        <tr>
            <td valign="top">
                <table cellspacing="0" cellpadding="3" width="100%" border="0" bordercolor="#FFFFFF">
                    <tbody>
                        <tr>
                            <td colspan="2" bgcolor="#C1C1C1">
                                    <font face="Arial" size="2"><b>Application details</b></font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">ID</font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:label runat="server" id="lblField_ID" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">Name</font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:textbox runat="server" style="width: 200px" id="txtField_Title" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">Title (for internal use only)</font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:textbox runat="server" style="width: 200px" id="txtField_TitleAdminArea" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">Location</font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:dropdownlist runat="server" id="cmbLocation" style="width: 200px" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                <p>
                                    <font face="Arial" size="2">Market &nbsp;</font></p>
                            </td>
                            <td valign="Top" width="240">
                                <p>
                                    <font face="Arial" size="2">
                                        <asp:dropdownlist runat="server" id="cmbLanguage" style="width: 200px" />
                                    </font>
                                </p>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                <p>
                                    <font face="Arial" size="2">Disabled &nbsp;</font></p>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:dropdownlist runat="server" id="cmbAppDisabled" style="width: 200px" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">Required user flags</font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:textbox runat="server" style="width: 200px" id="txtField_RequiredUserFlags" MaxLength="4000" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">Remarks on required flags</font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:textbox runat="server" style="width: 200px" id="txtField_RequiredUserFlagsRemarks" textmode="multiline" rows="3" MaxLength="4000" />
                                    </font>
                            </td>
                        </tr>
						<tr>
						    <td valign="Top" width="160">
                                    <font face="Arial" size="2">General remarks</font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:textbox runat="server" style="width: 200px" id="txtField_GeneralRemarks" textmode="multiline" rows="2" MaxLength="1024" />
                                    </font>
                            </td>
						</tr>
					    <tr>
                            <td colspan="2" valign="Top">
                                <font face="Arial" size="2"><asp:hyperlink id="hypUsersMissingFlags" runat="server" target="_blank">&gt;&gt; Check for users with missing flags</asp:hyperlink></font>
                            </td>
                        </tr>
                        <%@ register tagprefix="camm" tagname="WebManagerAdminDelegates" src="administrative_delegates.ascx" %>
                        <camm:WebManagerAdminDelegates id="cammWebManagerAdminDelegates" runat="server" />
                        <tr>
                            <td valign="Top" colspan="2">
                                    &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" bgcolor="#C1C1C1">
                                    <font face="Arial" size="2"><b>Navigation details</b></font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">Navigation title in level 1</font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:textbox style="width: 200px" runat="server" id="txtField_Level1Title" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2"></font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:radiobutton groupname="Level1" runat="server" id="rbLevel1TitleText" text="Text"
                                            checked="true" />
                                        <asp:radiobutton groupname="Level1" runat="server" id="rbLevel1TitleHTML" text="HTML" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">Navigation title in level 2</font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:textbox style="width: 200px" runat="server" id="txtField_Level2Title" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2"></font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:radiobutton groupname="Level2" runat="server" id="rbLevel2TitleText" text="Text"
                                            checked="true" />
                                        <asp:radiobutton groupname="Level2" runat="server" id="rbLevel2TitleHTML" text="HTML" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">Navigation title in level 3</font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:textbox style="width: 200px" runat="server" id="txtField_Level3Title" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2"></font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:radiobutton groupname="Level3" runat="server" id="rbLevel3TitleText" text="Text"
                                            checked="true" />
                                        <asp:radiobutton groupname="Level3" runat="server" id="rbLevel3TitleHTML" text="HTML" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">Navigation title in level 4</font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:textbox style="width: 200px" runat="server" id="txtField_Level4Title" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2"></font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:radiobutton groupname="Level4" runat="server" id="rbLevel4TitleText" text="Text"
                                            checked="true" />
                                        <asp:radiobutton groupname="Level4" runat="server" id="rbLevel4TitleHTML" text="HTML" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">Navigation title in level 5</font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:textbox style="width: 200px" runat="server" id="txtField_Level5Title" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2"></font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:radiobutton groupname="Level5" runat="server" id="rbLevel5TitleText" text="Text"
                                            checked="true" />
                                        <asp:radiobutton groupname="Level5" runat="server" id="rbLevel5TitleHTML" text="HTML" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">Navigation title in level 6</font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:textbox style="width: 200px" runat="server" id="txtField_Level6Title" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2"></font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:radiobutton groupname="Level6" runat="server" id="rbLevel6TitleText" text="Text"
                                            checked="true" />
                                        <asp:radiobutton groupname="Level6" runat="server" id="rbLevel6TitleHTML" text="HTML" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">URL</font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:textbox style="width: 200px" runat="server" id="txtField_NavURL" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">Target frame</font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:textbox style="width: 200px" runat="server" maxlength="50" id="txtField_NavFrame" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">Tooltip text</font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:textbox style="width: 200px" runat="server" maxlength="1024" id="txtField_NavTooltip" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">Javascript:OnMouseOver</font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:textbox style="width: 200px" runat="server" maxlength="512" id="txtField_NavJSOnMOver" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">Javascript:OnMouseOut</font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:textbox style="width: 200px" runat="server" maxlength="512" id="txtField_NavJSOnMOut" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">Javascript:OnClick</font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:textbox style="width: 200px" runat="server" maxlength="512" id="txtField_NavJSOnClick" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">Status</font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:radiobutton runat="server" id="rbNew" text="New" groupname="Status" />
                                        <br>
                                        <asp:radiobutton runat="server" id="rbUpdate" text="Update" groupname="Status" />
                                        <br>
                                        <asp:radiobutton runat="server" id="rbStandard" text="Standard" groupname="Status" />
                                    </font>
                                    <br>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">Reset Status On<br>
                                        <font size="1"><em>Format: YYYY-MM-DD HH:MM:SS</em></font></font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:textbox style="width: 200px" runat="server" id="txtField_ResetIsNewUpdatedStatusOn" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">Sort ID</font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:textbox style="width: 200px" runat="server" id="txtField_Sort" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">Add Market ID to URL</font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:dropdownlist style="width: 200px" runat="server" id="cmbAddLanguageID2URL" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" colspan="2">
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" bgcolor="#C1C1C1">
                                    <font face="Arial" size="2"><b>Statistics</b></font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">Created on</font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:label runat="server" id="lblField_ReleasedOn" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">Created by</font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:hyperlink id="hypCreatedBy" runat="server" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">Last modification on</font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:label runat="server" id="lblField_ModifiedOn" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">Last modification by</font>
                            </td>
                            <td valign="Top" width="240">
                                    <font face="Arial" size="2">
                                        <asp:hyperlink id="hypLastModifiedBy" runat="server" />
                                    </font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" colspan="2">
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" bgcolor="#C1C1C1">
                                    <font face="Arial" size="2"><b>Authorizations</b></font>
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" colspan="2" id="tdAddLinks" runat="server">
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" colspan="2">
                            </td>
                        </tr>
                        <tr>
                            <td valign="Top" width="160">
                                    <font face="Arial" size="2">
                                        <asp:button runat="server" id="btnSubmit" text="Update application" />
                                    </font>
                            </td>
                            <td valign="Top" width="240">
                            </td>
                        </tr>
                    </tbody>
                </table>
            </td>
        </tr>
    </tbody>
</table>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="apps.aspx" id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
