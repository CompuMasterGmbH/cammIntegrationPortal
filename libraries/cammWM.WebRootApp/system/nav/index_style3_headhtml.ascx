<%@ Control Language="vb" AutoEventWireup="false" %>
<script runat="server" language="vb">
public cammWebManager as CompuMaster.camm.WebManager.WMSystem
</script>
<base target="frame_main">
<!--#include virtual="/sysdata/includes/metalink.aspx"-->
<STYLE type="text/css">A:active {
	FONT-WEIGHT: normal; COLOR: #0000ff; FONT-FAMILY: Arial; TEXT-DECORATION: none
}
A:link {
	FONT-WEIGHT: normal; COLOR: #000080; FONT-FAMILY: Arial; TEXT-DECORATION: none
}
A:visited {
	FONT-WEIGHT: normal; COLOR: #888888; FONT-FAMILY: Arial; TEXT-DECORATION: none
}
A:hover {
	FONT-WEIGHT: normal; COLOR: #585888; FONT-FAMILY: Arial; TEXT-DECORATION: underline
}
A.NavHeader0:active {
	FONT-WEIGHT: bold; FONT-SIZE: 13px; COLOR: #ffffff; TEXT-DECORATION: none
}
A.NavHeader0:link {
	FONT-WEIGHT: bold; FONT-SIZE: 13px; COLOR: #ffffff; TEXT-DECORATION: none
}
A.NavHeader0:visited {
	FONT-WEIGHT: bold; FONT-SIZE: 13px; COLOR: #ffffff; TEXT-DECORATION: none
}
A.NavHeader0:hover {
	FONT-WEIGHT: bold; FONT-SIZE: 13px; COLOR: #e1e1e1; TEXT-DECORATION: underline
}
A.NavHeader1:active {
	FONT-WEIGHT: bold; FONT-SIZE: 13px; COLOR: #00426b; TEXT-DECORATION: none;
}
A.NavHeader1:link {
	FONT-WEIGHT: bold; FONT-SIZE: 13px; COLOR: #00426b; TEXT-DECORATION: none;
}
A.NavHeader1:visited {
	FONT-WEIGHT: bold; FONT-SIZE: 13px; COLOR: #00426b; TEXT-DECORATION: none;
}
A.NavHeader1:hover {
	FONT-WEIGHT: bold; FONT-SIZE: 13px; COLOR: #6a6d6e; TEXT-DECORATION: underline;
}
A.NavHeader2:active {
	FONT-WEIGHT: bold; FONT-SIZE: 13px; COLOR: #333333; TEXT-DECORATION: none
}
A.NavHeader2:link {
	FONT-WEIGHT: bold; FONT-SIZE: 13px; COLOR: #333333; TEXT-DECORATION: none
}
A.NavHeader2:visited {
	FONT-WEIGHT: bold; FONT-SIZE: 13px; COLOR: #333333; TEXT-DECORATION: none
}
A.NavHeader2:hover {
	FONT-WEIGHT: bold; FONT-SIZE: 13px; COLOR: #808080; TEXT-DECORATION: underline
}
A.NavHeader3:active {
	FONT-WEIGHT: normal; FONT-SIZE: 13px; COLOR: #333333; TEXT-DECORATION: none
}
A.NavHeader3:link {
	FONT-WEIGHT: normal; FONT-SIZE: 13px; COLOR: #333333; TEXT-DECORATION: none
}
A.NavHeader3:visited {
	FONT-WEIGHT: normal; FONT-SIZE: 13px; COLOR: #333333; TEXT-DECORATION: none
}
A.NavHeader3:hover {
	FONT-WEIGHT: normal; FONT-SIZE: 13px; COLOR: #808080; TEXT-DECORATION: underline
}
.NavHeader0 {
	FONT-WEIGHT: bold; FONT-SIZE: 13px; COLOR: #ffffff; TEXT-DECORATION: none
}
DIV.NavHeader2 {
	FONT-WEIGHT: bold; FONT-SIZE: 13px; COLOR: #333333; TEXT-DECORATION: none
}
DIV.NavHeader3 {
	FONT-SIZE: 13px; COLOR: #333333; TEXT-DECORATION: none
}
DIV.NavHeader4 {
	FONT-SIZE: 12px; COLOR: #333333; TEXT-DECORATION: none
}
DIV.NavHeader5 {
	FONT-SIZE: 10px; COLOR: #333333; TEXT-DECORATION: none
}
SPAN.NavHeader1 {
    FONT-WEIGHT: bold; FONT-SIZE: 13px; COLOR: #00426b; TEXT-DECORATION: none;
}
SPAN.NavHeader2 {
	FONT-WEIGHT: bold; FONT-SIZE: 13px; COLOR: #333333; TEXT-DECORATION: none
}
BODY {
    COLOR: #000000;
    BACKGROUND-COLOR: #ffff66;
    FONT-FAMILY: Arial,"Andale Mono",sans-serif;
    FONT-SIZE: 13px;
    scrollbar-3d-light-color: #E1E1E1;
    scrollbar-arrow-color: #585888;
    scrollbar-base-color: #C1C1C1;
    scrollbar-dark-shadow-color: #E1E1E1;
    scrollbar-track-color: #E1E1E1;
    scrollbar-face-color: #E1E1E1;
    scrollbar-highlight-color: #E1E1E1;
    scrollbar-shadow-color: #E1E1E1;}
</STYLE>