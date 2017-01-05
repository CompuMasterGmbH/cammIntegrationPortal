<%@ Page Language="vb" ValidateRequest="false"  Inherits="CompuMaster.camm.WebManager.Pages.Administration.UpdateEmailDetail" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" SecurityObject="System - Mail Queue Monitor" />
<html>	
	<head>
	<title>Update mail detail</title>
	<link rel="stylesheet" type="text/css" href="/sysdata/style_standard.css">
	<script language="javascript">
	    function ValidateEmailId(ToEmail,CCEmail,BccEmail)
	    {
	        var myArr = new Array();
	         var emailfilter=/^\w+[\+\.\w-]*@([\w-]+\.)*\w+[\w-]*\.([a-z]{2,4}|\d+)$/i;
	         
	            var txEmail=document.getElementById(ToEmail);

	            if(trim(txEmail.value.toString()) == '')
	            {
	                alert("Please enter receipient email address.");
	                return false;
	            }
	             myArr = txEmail.value.split(","); 
	             
	             for(var i=0;i<myArr.length;i++)
                 {
                    
                    var EmailId="";
                    if(myArr[i].toString().indexOf('<')!=-1)
                    {
                         EmailId=myArr[i].toString().substring(myArr[i].toString().indexOf('<')+1,myArr[i].toString().indexOf('>'))
                    }
                    else
                    {
                        EmailId=myArr[i].toString();
                    }   
                    
                        
                        var returnval=emailfilter.test(EmailId);
                        
                        if (returnval == false) 
                        {
                            alert('Invalid E-Mail:' + myArr[i]);
                            return false;
                        }
                    
                    
                }
           
            
            txEmail=document.getElementById(CCEmail);
	         
	         if(trim(txEmail.value.toString()) != "")
	         {
	            myArr = txEmail.value.split(","); 
	            
	             for(var i=0;i<myArr.length;i++)
                 {
                     
                     var EmailId="";
                    if(myArr[i].toString().indexOf('<')!=-1)
                    {
                         EmailId=myArr[i].toString().substring(myArr[i].toString().indexOf('<')+1,myArr[i].toString().indexOf('>'))
                    }
                    else
                    {
                        EmailId=myArr[i].toString();
                    }   
                    
                    var returnval=emailfilter.test(EmailId);
                    if (returnval == false) 
                    {
                        alert('Invalid Cc E-Mail:' + myArr[i]);
                        return false;
                    }
                    
                } 
            }
            
            
            txEmail=document.getElementById(BccEmail);
	         
	         
	         if(trim(txEmail.value.toString()) != "")
	         {
	            myArr = txEmail.value.split(","); 
	             
	             for(var i=0;i<myArr.length;i++)
                {
                     var EmailId="";
                    if(myArr[i].toString().indexOf('<')!=-1)
                    {
                         EmailId=myArr[i].toString().substring(myArr[i].toString().indexOf('<')+1,myArr[i].toString().indexOf('>'))
                    }
                    else
                    {
                        EmailId=myArr[i].toString();
                    }   
                
                    var returnval=emailfilter.test(EmailId);
                    if (returnval == false) 
                    {
                        alert('Invalid Bcc E-Mail:' + myArr[i]);
                        return false;
                    }
                    
                } 
            }
            
            return true;

	         
            
	    }
	   
     function trim(s) 
	{
	    return s.replace(/^\s+|\s+$/g,"");
    }	
    
	</script>
	</head>
	<body leftmargin="0" rightmargin="0" topmargin="0" bottommargin="0">
		<form id="Form1" method="post" runat="server">			
			<table border="0" cellpadding="5" cellspacing="0" width="100%">
				
						
						
				<tr>
					<td width="10%" valign="top" align="Left"><asp:Label Runat="server" ID="lblFrom" Text="From"/></td>
					<td><asp:TextBox ID="txtFrom" runat="server" width="250px"></asp:TextBox></td>
				</tr>
				<tr>
					<td width="10%" valign="top" align="Left"><asp:Label Runat="server" ID="lblTo" Text="To"/></td>
					<td><asp:TextBox ID="txtTo" runat="server" width="250px"></asp:TextBox></td>
				</tr>
				<tr>
					<td width="10%" valign="top" align="Left"><asp:Label Runat="server" ID="lblCC" Text="Cc"/></td>
					<td><asp:TextBox ID="txtCc" runat="server" width="250px"></asp:TextBox></td>
				</tr>
				<tr>
					<td width="10%" valign="top" align="Left"><asp:Label Runat="server" ID="lblBcc" Text="Bcc"/></td>
					<td><asp:TextBox ID="txtBcc" runat="server" width="250px"></asp:TextBox></td>
				</tr>
				<tr>
				    <td></td>
					<td align="left"><asp:Button Runat="server" ID="btnSend" Font-Bold="True" Width="110" Text="Send" />
					<asp:Button Runat="server" ID="btnClose" Visible="false" Font-Bold="True" Width="110" Text="Close" />
					</td>
				</tr>
				<tr>
				    <td>&nbsp;</td>
				    <td><P><FONT face="Arial" color="green" size="2"><asp:Label id="lblMsg" runat="server" /><br> &nbsp;</FONT></P></td>
					
				</tr>	
				
			</table>
			<br>			
		</form>
	</body>
</HTML>
