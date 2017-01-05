<%@ Control Language="vb" AutoEventWireup="false" %>
<script language="vb" runat="server">
public LanguageID as integer
</script>
<SCRIPT type="text/javascript" language=javascript>
        window.onunload = function(){saveState(menuId);}
        var openDisplay = "block";
        var closedDisplay = "none";
        var MenuStateCookieName = "<% If Session("System_Username") <> "" Then Response.Write ("MenuStateSecured" & LanguageID) Else Response.Write ("MenuStateAnonymous" & LanguageID) %>";
        getState(menuId);

        // expandit function
        //   calls toggleSection function after a
        //   short delay so that images display
        //   correctly
        function expandit(itemId, levID, Updated)
        {
	        window.setTimeout("eval(toggleSection('" + itemId + "', " + levID + ", " + Updated + "))",10);
        }

        // toggleSection function
        //   opens or closes menu section
        function toggleSection(itemId, levID, Updated) {
			//levID is not more needed, see it as a reserved parameter
	        var arrowId = itemId.replace("SectionPanel", "ArrowImage");
	        var arrow, item;
	        var itemNo;

	        if (document.all) {
		        item = document.all[itemId];
		        arrow = document.all[arrowId];
	        }
	        if (!document.all && document.getElementById) {
		        item = document.getElementById(itemId);
		        arrow = document.getElementById(arrowId);
	        }
	        if (item)
	        {
				itemNo = itemId.indexOf('_',21); //get last no. letter
				itemNo = itemId.slice(20,itemNo);
				if (item.style.display == closedDisplay)
				{
				    if (Updated == 1)
					{
					    arrow.src = arrowDownUpdatedArray[ElementLevels[itemNo]].src;
					}
					else
					{
					    arrow.src = arrowDownArray[ElementLevels[itemNo]].src;
					}
				    item.style.display = openDisplay;
				}
				else
				{
				    if (Updated == 1)
					{
					    arrow.src = arrowRightUpdatedArray[ElementLevels[itemNo]].src;
					}
					else
					{
					    arrow.src = arrowRightArray[ElementLevels[itemNo]].src;
					}
				    item.style.display = closedDisplay;
				}
			}
        }

        // saveState function
        //   sets the state of each menu section and
        //   adds the open menu sections to a cookie
        function saveState(menuId) {
	        var cookieInfo = "";
	        for (i = 0; i < sectionCount; i++) {
		        var item;
		        var itemId = menuClientId.replace(menuId + "_Menu",menuId + "_NavSecs__ctl" + i + "_SectionPanel");

		        if (document.all) {
			        item = document.all[itemId];
		        }
		        if (!document.all && document.getElementById) {
			        item = document.getElementById(itemId);
		        }
		        if (item.style.display == openDisplay)
			        cookieInfo = cookieInfo + "|" + i;
	        }
	        setCookie(MenuStateCookieName, cookieInfo, getExpirationDate(4));
        }

        // getState function
        //   gets and opens menu sections based on
        //   the values from the cookie set in saveState
        function getState(menuId) {
			//var sectionstotal = (getCookie('MenuDrops'));
	        var sections = new Array();
	        sections = getCookie(MenuStateCookieName).split("|");
	        if ((getCookie(MenuStateCookieName) != "") && (getCookie(MenuStateCookieName) != ";")) // && (sectionstotal == ElementLevels.length))
				for (i = 1; i < sections.length; i++)
				{
				    var itemId = menuClientId.replace(menuId + "_Menu",menuId + "_NavSecs__ctl" + sections[i] + "_SectionPanel");
				    eval(expandit(itemId,0,0));
				}
			else
				{
				    var itemId = menuClientId.replace(menuId + "_Menu",menuId + "_NavSecs__ctl0_SectionPanel");
				    eval(expandit(itemId,0,0));
				}
        }

        // getCookie function
        //   gets the menu cookie
        function getCookie(cookieName) {
	        var cookie;
	        cookie = "" + document.cookie;
	        var start = cookie.indexOf(cookieName);
	        if (cookie == "" || start == -1)
		        return "";
	        var end = cookie.indexOf(';',start);
	        if (end == -1)
		        end = cookie.length;
	        return unescape(cookie.substring(start+cookieName.length + 1,end));
        }

        // setCookie function
        //   sets the menu cookie
        function setCookie(cookieName, value, expires) {
	        cookieInfo = cookieName + "=" + escape(value) + ";MenuDrops=" + escape(ElementLevels.length) + ";path=/;expires=" + expires
	        document.cookie = cookieInfo;
	        return document.cookie;
        }

        // getExpirationDate function
        //   gets the menu cookie from the browser
        function getExpirationDate(days){
	        today = new Date();
	        today.setTime(Date.parse(today) + (days * 60 * 60 * 24 * 100));
	        return  today.toGMTString();
        }
</SCRIPT>