<%@ Page Language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Page" debug="false" trace="false" %>
<%@ Register TagPrefix="camm" Assembly="camm.WebControls" Namespace="CompuMaster.camm.Controls.WebControls" %>
<%@ Register TagPrefix="StyleTemplates" Assembly="camm.WebControls" Namespace="CompuMaster.camm.Controls.WebControls.StyleTemplates" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" />
<html>
<body style="font-family: Arial">
<p><A href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/webcontrols/index.src">View source code of the page</A></p>

<h1>CompuMaster.camm.Controls.WebControls.LabelLines</h1>

<camm:LabelLine runat="server">This is a line</camm:LabelLine><camm:LabelLine runat="server">This is another line</camm:LabelLine><camm:LabelLine runat="server"> </camm:LabelLine><camm:LabelLine runat="server">This previous tag was empty and so it has been set invisible; that's why this is the 3rd line.</camm:LabelLine>

<h1>CompuMaster.camm.Controls.WebControls.ResizedImage</h1>

<table border="0" cellpadding="3" cellspacing="0">
<tr>
<td><camm:ResizedImage runat="server" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200" ScaleMode="Scale" /></td>
<td><camm:ResizedImage runat="server" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200" ScaleMode="Fit" /></td>
<td><camm:ResizedImage runat="server" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200" ScaleMode="FitExact" /></td>
</tr>
<tr>
<td align="center">Scale mode: Scale<br>Maximum resolution: 300x200<br>Real resolution: 267x200</td>
<td align="center">Scale mode: Fit<br>Destination resolution: 300x200<br>Real resolution: 300x200</strong></td>
<td align="center">Scale mode: FitExact<br>Destination resolution: 300x200<br>Real resolution: 300x200<br>(please note the white space inside of the image)</td>
</tr>
<tr>
<td><camm:ResizedImage runat="server" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200" RotateFlipType="1" /></td>
<td><camm:ResizedImage runat="server" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200" RotateFlipType="2" /></td>
<td><camm:ResizedImage runat="server" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200" RotateFlipType="3" /></td>
</tr>
<tr>
<td align="center">Rotated by 90 degrees</td>
<td align="center">Rotated by 180 degrees</td>
<td align="center">Rotated by 270 degrees</td>
</tr>
</table>

<h1>CompuMaster.camm.Controls.WebControls.ResizedImageWithFrame (default)</h1>

<table border="0" cellpadding="3" cellspacing="0">
<tr>
<td><camm:ResizedImageWithFrame runat="server" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200" MiddleFrameWidth=0 OuterFrameWidth=0 /></td>
</tr>
<tr>
<td align="center">Borders with gradient to #FFFFFF (8 px)</td>
</tr>
</table>

<h1>CompuMaster.camm.Controls.WebControls.ResizedImageWithFrame (multiple frames and style demos)</h1>

<table border="0" cellpadding="3" cellspacing="0">
<tr>
<td><camm:ResizedImageWithFrame runat="server" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200" FrameType=1 InnerFrameWidth="8" InnerFrameColor="#0000FF" MiddleFrameWidth=3 MiddleFrameColor="#D1D1D1" OuterFrameWidth=3 OuterFrameColor="#FF0000" /></td>
<td><camm:ResizedImageWithFrame runat="server" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200" FrameType=2 InnerFrameWidth="8" InnerFrameColor="#0000FF" MiddleFrameWidth=3 MiddleFrameColor="#D1D1D1" OuterFrameWidth=3 OuterFrameColor="#FF0000" /></td>
<td><camm:ResizedImageWithFrame runat="server" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200" FrameType=3 InnerFrameWidth="8" InnerFrameColor="#0000FF" MiddleFrameWidth=3 MiddleFrameColor="#D1D1D1" OuterFrameWidth=3 OuterFrameColor="#FF0000" /></td>
<td><camm:ResizedImageWithFrame runat="server" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200" FrameType=4 InnerFrameWidth="8" InnerFrameColor="#0000FF" MiddleFrameWidth=3 MiddleFrameColor="#D1D1D1" OuterFrameWidth=3 OuterFrameColor="#FF0000" /></td>
</tr>
<tr>
<td align="center">Frame type 0 - default</td>
<td align="center">Frame type 2 - middle frame crosses the outer frame</td>
<td align="center">Frame type 3 - inner frame crosses the outer frames</td>
<td align="center">Frame type 4 - middle and inner frames crosses the outer frames</td>
</tr>
<tr>
<td><StyleTemplates:ResizedImageStyle1 runat="server" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200" /></td>
<td><StyleTemplates:ResizedImageStyle2 runat="server" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200" /></td>
<td><StyleTemplates:ResizedImageStyle3 runat="server" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200" /></td>
<td></td>
</tr>
<tr>
<td align="center">StyleTemplates.ResizedImageStyle1<br>Borders with color #193C50 <br>(RGB:25,60,80) (1 px) + #FFFFFF (1 px)</td>
<td align="center">StyleTemplates.ResizedImageStyle2<br>Borders with color #696D6E <br>(RGB:105,109,110) (4 px) + #FFFFFF (1 px)</td>
<td align="center">StyleTemplates.ResizedImageStyle3<br>Borders with color #193C50 <br>(RGB:25,60,80) (1 px) + gradient<br> to #FFFFFF (1 px)</td>
<td align="center"></td>
</tr>
<tr>
<td><camm:ResizedImageWithFrame runat="server" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200" FrameType=1 InnerFrameWidth="8" InnerFrameColor="#000000" MiddleFrameWidth=3 MiddleFrameColor="#FFFFFF" OuterFrameWidth=3 OuterFrameColor="#000000" /></td>
<td><camm:ResizedImageWithFrame runat="server" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200" FrameType=1 InnerFrameWidth="8" InnerFrameColor="#0000FF" MiddleFrameWidth=3 MiddleFrameColor="#00FF00" OuterFrameWidth=3 OuterFrameColor="#0000FF" /></td>
<td><camm:ResizedImageWithFrame runat="server" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200" FrameType=1 InnerFrameWidth="8" InnerFrameColor="#00FF00" MiddleFrameWidth=3 MiddleFrameColor="#FF0000" OuterFrameWidth=3 OuterFrameColor="#00FF00" /></td>
<td><camm:ResizedImageWithFrame runat="server" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200" FrameType=1 InnerFrameWidth="8" InnerFrameColor="#FF0000" MiddleFrameWidth=3 MiddleFrameColor="#0000FF" OuterFrameWidth=3 OuterFrameColor="#FF0000" /></td>
</tr>
<tr>
<td align="center">Black/White/Black</td>
<td align="center">Blue/Green/Blue</td>
<td align="center">Green/Red/Green</td>
<td align="center">Red/Blue/Red</td>
</tr>
</table>

<h1>CompuMaster.camm.Controls.WebControls.ResizedImageWithAdvancedFrame (multiple frames and style demos)</h1>

<table border="0" cellpadding="3" cellspacing="0">
<tr>
<td><camm:ResizedImageWithAdvancedFrame runat="server" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200" FrameType=1 InnerFrameWidthLeft="12" InnerFrameWidthRight="0" InnerFrameWidthUpper="0" InnerFrameWidthLower="12" InnerFrameColor="#FFFFFF" MiddleFrameWidthLeft=0 MiddleFrameWidthRight=0 MiddleFrameWidthUpper=0 MiddleFrameWidthLower=0 MiddleFrameColor="#D1D1D1" OuterFrameWidthLeft=0 OuterFrameWidthRight=0 OuterFrameWidthUpper=0 OuterFrameWidthLower=0 OuterFrameColor="#FF0000" /></td>
<td><camm:ResizedImageWithAdvancedFrame runat="server" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200" FrameType=1 InnerFrameWidthLeft="0" InnerFrameWidthRight="0" InnerFrameWidthUpper="15" InnerFrameWidthLower="15" InnerFrameColor="#FFFFFF" MiddleFrameWidthLeft=0 MiddleFrameWidthRight=0 MiddleFrameWidthUpper=0 MiddleFrameWidthLower=0 MiddleFrameColor="#D1D1D1" OuterFrameWidthLeft=0 OuterFrameWidthRight=0 OuterFrameWidthUpper=0 OuterFrameWidthLower=0 OuterFrameColor="#FF0000" /></td>
<td><camm:ResizedImageWithAdvancedFrame runat="server" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200" FrameType=1 InnerFrameWidthLeft="8" InnerFrameWidthRight="8" InnerFrameWidthUpper="8" InnerFrameWidthLower="8" InnerFrameColor="#FF0000" MiddleFrameWidthLeft=3 MiddleFrameWidthRight=10 MiddleFrameWidthUpper=10 MiddleFrameWidthLower=3 MiddleFrameColor="#0000FF" OuterFrameWidthLeft=10 OuterFrameWidthRight=3 OuterFrameWidthUpper=3 OuterFrameWidthLower=10 OuterFrameColor="#FF0000" /></td>
</tr>
<tr>
<td align="center">No outer frames, Inner frame: colour gradient to #FFFFFF (white) with a width of 12 on the left and lower, none to the right and upper side</td>
<td align="center">No outer frames, Inner frame: colour gradient to #FFFFFF (white) with a width of 15 on the upper and lower, none to the left and right side</td>
<td align="center">Red outer frame with a width of 10 on the left and lower and a width of 3 on the upper and right side, blue middle frame with a width of 10 the right and upper and a width of 10 and a width of 3 on the left and lower side, inner colour gradient to red with a width of 8 on every side</td>
</tr>
</table>

<h1>CompuMaster.camm.Controls.WebControls.ResizedImageWithTextureShadow</h1>

<table border="0" cellpadding="3" cellspacing="0">
<tr>
<td><camm:ResizedImageWithTextureShadow runat="server" id="txture1_1" IgnoreErrors="false" TextureImageSource="Kastelruth.jpg" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200" /></td>
<td><camm:ResizedImageWithTextureShadow runat="server" id="txture1_2" TextureImageSource="Kastelruth.jpg" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200"
				BackgroundColor="#FFFFFF" ShadowColor="#D1D1D1"
				FrameWidth="25" ShadowWidth="5" ShadowStrength="50" ShadowGradientWidth="4" /></td>
<td><camm:ResizedImageWithTextureShadow runat="server" id="txture1_3" TextureImageSource="clouds.jpg" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200"
				FrameWidth="50" ShadowWidth="0" ShadowGradientWidth="0" /></td>
<td align="center"></td>
</tr>
<tr>
<td align="center">default</td>
<td align="center">Background texture visible with 25 px,<br>inside the resized image,<br>shadow grey and 50 % strength,<br>shadow gradient to the background 4 px,<br>shadow width 5 px</td>
<td align="center">Background texture visible with 50 px</td>
<td align="center"></td>
</tr>
<tr>
<td colspan="4"><camm:ResizedImageWithTextureShadow id="txture2_1" runat="server" TextureImageSource="Kastelruth.jpg" Source="flower.jpg" DestinationWidth="1024" DestinationHeight="768"
				Measurement="Percent" BackgroundColor="#FFFFFF" ShadowColor="#D1D1D1"
				FrameWidth="10" ShadowWidth="5" ShadowStrength="50" ShadowGradientWidth="2" /></td>
</tr>
<tr>
<td align="center" colspan="4">Background texture visible with 10 %,<br>inside the resized image,<br>shadow grey and 50 % strength,<br>shadow gradient to the background 2 %,<br>shadow width 5 %</td>
</tr>
</table>

<h1>CompuMaster.camm.Controls.WebControls.ResizedImageWithShadow</h1>

<table border="0" cellpadding="3" cellspacing="0">
<tr>
<td><camm:ResizedImageWithShadow runat="server" id="shd1" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200" /></td>
<td><camm:ResizedImageWithShadow runat="server" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200" BackgroundColor="#FF0000" /></td>
<td><camm:ResizedImageWithShadow runat="server" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200" BackgroundColor="#FFFFFF" ShadowColor="#0000FF" FrameWidth="0" ShadowWidth="5" ShadowStrength="100" ShadowGradientWidth="0" /></td>
<td><camm:ResizedImageWithShadow runat="server" Source="flower.jpg" DestinationWidth="300" DestinationHeight="200" BackgroundColor="#FFFFFF" ShadowColor="#0000FF" FrameWidth="2" ShadowWidth="5" ShadowStrength="70" ShadowGradientWidth="4" /></td>
</tr>
<tr>
<td align="center">default</td>
<td align="center">Red background color</td>
<td align="center">Red shadow color</td>
<td align="center">70 % red shadow color, additional gradient from the shadow area to the background color</td>
</tr>
</table>

<h1>CompuMaster.camm.Controls.WebControls.ResizedImage (error simulations)</h1>

<table border="0" cellpadding="3" cellspacing="0">
<tr>
<td><camm:ResizedImageWithFrame runat="server" Source="notexistingfile.jpg" DestinationWidth="300" DestinationHeight="200" /></td>
<td><camm:ResizedImageWithFrame runat="server" Source="flower.jpg" /></td>
</tr>
<tr>
<td align="center">Error simulation: file not found</td>
<td align="center">Error simulation: no destination dimensions defined</td>
</tr>
</table>

</body>
</html>