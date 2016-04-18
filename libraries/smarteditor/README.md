# SmartEditor

## Editions
There are several editor components available. Please use the one which is appropriate to you.

### SmartPlainHtmlEditor
``` asp
<%@ Page ValidateRequest="false" EnableEventValidation="false" %>
<%@ Register TagPrefix="cammWebEdit" Namespace="CompuMaster.camm.SmartWebEditor.Controls" 
      Assembly="cammWM.SmartEditor" %>
<html>
<body>
<form runat="server">
<cammWebEdit:SmartPlainHtmlEditor marketlookupmode="3" runat="server" id="MainEditor" 
      securityobjecteditmode="{Security Object for Editors}" 
      AlternativeDataMarkets="1,10000" />
</form>
</body>
</html>
```

### SmartCommonMarkEditor
``` asp
<%@ Page ValidateRequest="false" EnableEventValidation="false" %>
<%@ Register TagPrefix="cammWebEdit" Namespace="CompuMaster.camm.SmartWebEditor.Controls" 
      Assembly="cammWM.CommonMarkEditor" %>
<html>
<body>
<form runat="server">
<cammWebEdit:SmartCommonMarkEditor marketlookupmode="3" runat="server" id="MainEditor" 
      securityobjecteditmode="{Security Object for Editors}"
      AlternativeDataMarkets="1,10000" />
</form>
</body>
</html>
```

## Available MarketLookupModes
``` vb.net
        ''' <summary>
        '''     Contains informations about how to handle the viewonly mode in different market, langs
        ''' </summary>
        Public Enum MarketLookupModes As Integer
            ''' <summary>
            '''     Data is only available in an international version and this is valid for all languages/markets
            ''' </summary>
            ''' <remarks>
            '''     This value is the same as None, just the name is more explainable
            '''     If the lookup process fails, an HTTP error 404 will be returned to the browser.
            ''' </remarks>
            SingleMarket = 0
            ''' <summary>
            '''     Data is only available in an international version and this is valid for all languages/markets
            ''' </summary>
            ''' <remarks>
            '''     This value is the same as SingleMarket, just the name is more simplified
            '''     If the lookup process fails, an HTTP error 404 will be returned to the browser.
            ''' </remarks>
            None = 0
            ''' <summary>
            '''     Data is maintained for every market separately, the language markets (e. g. "English", "French", etc. are handled as a separate market)
            ''' </summary>
            ''' <remarks>
            '''     If the lookup process fails, an HTTP error 404 will be returned to the browser.
            ''' </remarks>
            Market = 1
            ''' <summary>
            '''     Data is maintained for every language/market separately; when there is no value for a market it will be searched for some compatible language data
            ''' </summary>
            ''' <remarks>
            '''     Example: When the visitor is in market "German/Austria" but there is only some content available for market "German", the German data will be used.
            '''     If the lookup process fails, an HTTP error 404 will be returned to the browser.
            ''' </remarks>
            Language = 2
            ''' <summary>
            '''     Data is maintained for every language/market separately; when there is no value for the current market, the sWCMS control tries to lookup a best matching content
            ''' </summary>
            ''' <remarks>
            '''     When the user requests a page in e. g. market 559 ("French/France"), there will be the following order for the lookup process:
            '''     <list>
            '''         <item>Current market, in ex. ID 559 / French/France</item>
            '''         <item>Current language of market, in ex. ID 3 / French</item>
            '''         <item>Until customized by propert AlternativeDataMarkets: English universal, ID 1</item>
            '''         <item>Until customized by propert AlternativeDataMarkets: Worldwide market, ID 10000</item>
            '''         <item>International, ID 0</item>
            '''     </list>
            '''     If the lookup process fails, an HTTP error 404 will be returned to the browser.
            ''' </remarks>
            BestMatchingLanguage = 3
        End Enum
```

So, for a single text for every UI language/market, you might want to use:
``` asp
<cammWebEdit:SmartCommonMarkEditor marketlookupmode="0" runat="server" id="MainEditor" 
      securityobjecteditmode="{Security Object for Editors}" />
```

## Additional samples
### Sizing of edit textbox using style attributes
``` asp
<cammWebEdit:SmartCommonMarkEditor marketlookupmode="0" runat="server" id="MainEditor" 
      securityobjecteditmode="{Security Object for Editors}" 
      CssWidth="100%" CssHeight="280px" />
```
### Sizing of edit textbox using HTML textbox columns/rows attributes
``` asp
<cammWebEdit:SmartCommonMarkEditor marketlookupmode="0" runat="server" id="MainEditor" 
      securityobjecteditmode="{Security Object for Editors}" 
      Columns=80 Rows=25 />
```

### Accessing the content from a specified server ID
By default, the address (e. g.) "/content.aspx" provides different content on different servers. So, the intranet and the extranet are able to show independent content.
In some cases, you might want to override this behaviour and you want to show on the same URL the same content in the extranet as well as in the intranet. In this case, you would setup this property on the extranet server's scripts to show the content of the intranet server.
``` asp
<cammWebEdit:SmartCommonMarkEditor marketlookupmode="0" runat="server" id="MainEditor" 
      securityobjecteditmode="{Security Object for Editors}" 
      ContentOfServerID="2" />
```

Alternatively, you can assign the ContentOfServerID value by web.config settings for whole directories
``` xml
<configuration>
	<appSettings>
		<add key="WebManager.WebManager.WebEditor.ContentOfServerID" value="2" />
	</appSettings>
</configuration>
```

### Accessing the content for a specified server DocumentID
An identifier of the current document, by default its URL
``` asp
<cammWebEdit:SmartCommonMarkEditor marketlookupmode="0" runat="server" id="MainEditor" 
      securityobjecteditmode="{Security Object for Editors}" 
      DocumentID="HttpServerError404DefaultMessage" />
```
