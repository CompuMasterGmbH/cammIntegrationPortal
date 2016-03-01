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

### SmartPlainHtmlEditor
``` asp
<%@ Page ValidateRequest="false" EnableEventValidation="false" %>
<%@ Register TagPrefix="cammWebEdit" Namespace="CompuMaster.camm.SmartWebEditor.Controls" 
      Assembly="cammWM.CommonMarkEditor" %>
<html>
<body>
<form runat="server">
<cammWebEdit:CommonMarkEditor marketlookupmode="3" runat="server" id="MainEditor" 
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
<cammWebEdit:CommonMarkEditor marketlookupmode="0" runat="server" id="MainEditor" 
      securityobjecteditmode="{Security Object for Editors}" />
```

