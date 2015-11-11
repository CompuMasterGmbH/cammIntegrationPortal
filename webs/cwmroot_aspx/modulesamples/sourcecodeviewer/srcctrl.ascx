<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Collections" %>
<%@ Import Namespace="System.Collections.Specialized" %>
<%@ Import Namespace="System.Text.RegularExpressions" %>
<%@ Import Namespace="System.Security.Permissions" %>
<script runat="server" language="C#">

public String filename;
private bool _showfilename = false;
private Int32 _fontsize = 3;
const string TAG_FNTRED  = "<font color= \"red\">";
const string TAG_FNTBLUE = "<font color= \"blue\">" ;
const string TAG_FNTGRN  = "<font color= \"green\">" ;
const string TAG_FNTMRN  = "<font color=\"maroon\">" ;
const string TAG_EFONT   = "</font>" ;

public Boolean ShowFileName {
    get {
        return _showfilename;
    }
    set {
        _showfilename = value;
    }
}

public Int32 FontSize {
    get {
        return _fontsize;
    }
    set {
        _fontsize = value;
    }
}

protected override void Render(HtmlTextWriter output) {

    string err_message = "<p><b>Source Viewer Error: cannot show this file</b>";
                                 //+ "<p>Either the file does not exist, or your configuration settings for "
                                 //+ "the source viewer do not allow files in this directory to be viewed.  To "
                                 //+ "edit the configuration settings, see the <b>web.config</b> file at the "
                                 //+ "root of the quickstart directory.  Change the &lt;sourceview&gt; setting "
                                 //+ "to point at the root directory of the quickstart.  All files "
                                 //+ "under this directory will be accessible to the source viewer.";

    //err_message += "<pre>";
    //err_message += "&lt;configuration&gt;\n";
    //err_message += "  &lt;system.web&gt;\n";
    //err_message += "    &lt;sourceview&gt;\n";
    //err_message += "      &lt;add key=\"root\" value=\"c:\\Program Files\\Microsoft.Net\\FrameworkSDK\\Samples\\Quickstart\" /&gt;\n";
    //err_message += "    &lt;/sourceview&gt;\n";
    //err_message += "  &lt;/system.web&gt;\n";
    //err_message += "&lt;/configuration&gt;\n";
    //err_message += "</pre>";

    try {
        if (filename != null) {
            Trace.Write("SrcCtrl", filename);

            String dir = (String) ((NameValueCollection) Context.GetConfig("system.web/sourceview"))["root"];
		dir = Server.MapPath (dir);

            //Trace.Write("Security Check", "<p>" + filename + " contains " + dir + "? ");
            //Trace.Write("Security Check", String.Compare(filename, 0, dir, 0, dir.Length, true).ToString());
            //Trace.Write("Security Check", "<p>" + filename + "==" + dir + "\\web.config" + "? ");
            //Trace.Write("Security Check", String.Compare(filename,dir + "\\web.config",true).ToString());

            //if ((String.Compare(filename, 0, dir, 0, dir.Length, true)!=0)||(String.Compare(filename,dir + "\\web.config",true)==0)) {
                 //Response.Write(err_message);
                 //return;
            //}

       // This step makes the filename canonical (removes any ..\..\).
         String fullFilename = new FileInfo(filename).FullName.ToLower();

       // Set the file permissions so that only files in the QuickStart
       // directory can be accessed.
       FileIOPermission filePerms = new FileIOPermission(PermissionState.None);
       //filePerms.AddPathList(FileIOPermissionAccess.Read, new String[]
                      //{Path.Combine(dir, "aspplus"), 
                      //Path.Combine(dir, "winforms"), 
                      //Path.Combine(dir, "howto")}); 
       filePerms.AddPathList(FileIOPermissionAccess.Read, dir); //Replaced 4 lines above with this               
       filePerms.AllFiles = FileIOPermissionAccess.NoAccess;
       filePerms.PermitOnly();

       // Checks to make sure that the user cannot view the aspplus, winforms,
       // and howto web.configs.
       if((fullFilename.IndexOf("aspplus\\web.config") != -1) ||
          (fullFilename.IndexOf("winforms\\web.config") != -1) ||
          (fullFilename.IndexOf("howto\\web.config") != -1))
           {
            Response.Write(err_message);
            return;
           }

            if (ShowFileName)
            Response.Write("<h3>" + new FileInfo(filename).Name + "</h3>");

            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            StringWriter textBuffer = new StringWriter();
            String sourceLine;

            if ( _fontsize > 5 ) {
                textBuffer.Write("<font size=\"" + _fontsize + "\"><b>\r\n");
            } else {
                textBuffer.Write("<font size=\"" + _fontsize + "\">\r\n");
            }

            if((filename.ToLower()).EndsWith(".cs")) {
                textBuffer.Write("<pre>\r\n");
                while((sourceLine = sr.ReadLine()) != null) {
                    textBuffer.Write(FixCSLine(sourceLine)) ;
                    textBuffer.Write("\r\n");
                }
                textBuffer.Write("</pre>");
            } else if((filename.ToLower()).EndsWith(".js")) {
                textBuffer.Write("<pre>\r\n");
                while((sourceLine = sr.ReadLine()) != null) {
                    textBuffer.Write(FixJSLine(sourceLine)) ;
                    textBuffer.Write("\r\n");
                }
                textBuffer.Write("</pre>");
            } else if((filename.ToLower()).EndsWith(".vb")) {
                textBuffer.Write("<pre>\r\n");
                while((sourceLine = sr.ReadLine()) != null) {
                    textBuffer.Write(FixVBLine(sourceLine)) ;
                    textBuffer.Write("\r\n");
                }
                textBuffer.Write("</pre>");
            } else {
                String lang = "VB";
                bool isInScriptBlock = false;
                bool isInMultiLine = false;

                textBuffer.Write("<pre>\r\n");
                while((sourceLine = sr.ReadLine()) != null) {
                    // First we want to grab the global language
                    // for this page by a Page directive.  Or
                    // possibly from a script block.
                    lang = GetLangFromLine(sourceLine, lang);
                    if (IsScriptBlockTagStart(sourceLine)) {
                        textBuffer.Write(FixAspxLine(sourceLine));
                        isInScriptBlock = true;
                    }
                    else if (IsScriptBlockTagEnd(sourceLine)) {
                        textBuffer.Write(FixAspxLine(sourceLine));
                        isInScriptBlock = false;
                    }
                    else if (IsMultiLineTagStart(sourceLine) && !isInMultiLine) {
                        isInMultiLine = true;
                        textBuffer.Write("<font color=blue><b>" + HttpUtility.HtmlEncode(sourceLine));
                    }
                    else if (IsMultiLineTagEnd(sourceLine) && isInMultiLine) {
                        isInMultiLine = false;
                        textBuffer.Write(HttpUtility.HtmlEncode(sourceLine) + "</b></font>");
                    }
                    else if (isInMultiLine) {
                        textBuffer.Write(HttpUtility.HtmlEncode(sourceLine));
                    }
                    else {
                        if (isInScriptBlock == true) {
                            if ( lang.ToLower() == "c#" ) {
                                textBuffer.Write(FixCSLine(sourceLine));
                            } else if ( lang.ToLower() == "vb" ) {
                                textBuffer.Write(FixVBLine(sourceLine));
                            } else if ( lang.ToLower() == "jscript" || lang.ToLower() == "javascript" ) {
                                textBuffer.Write(FixJSLine(sourceLine));
                            }
                        }
                        else {
                            textBuffer.Write(FixAspxLine(sourceLine));
                        }
                    }
                    textBuffer.Write("\r\n");
                }
                textBuffer.Write("</pre>");
            }
            if ( _fontsize > 5 ) {
                textBuffer.Write("</b></font>\r\n");
            } else {
                textBuffer.Write("</font>\r\n");
            }

            Response.Write(textBuffer.ToString());

            fs.Close();
        }
    }
    catch (Exception e) {
        Response.Write(err_message);
    }
}
string GetLangFromLine(string sourceLine, string defLang) {
    if ( sourceLine == null ) {
        return defLang;
    }

    Match langMatch = Regex.Match(sourceLine, "(?i)<%@\\s*Page\\s*.*Language\\s*=\\s*\"(?<lang>[^\"]+)\"");
    if ( langMatch.Success ) {
        return langMatch.Groups["lang"].ToString();
    }

    langMatch = Regex.Match(sourceLine, "(?i)(?=.*runat\\s*=\\s*\"?server\"?)<script.*language\\s*=\\s*\"(?<lang>[^\"]+)\".*>");
    if ( langMatch.Success ) {
        return langMatch.Groups["lang"].ToString();
    }

    langMatch = Regex.Match(sourceLine, "(?i)<%@\\s*WebService\\s*.*Language\\s*=\\s*\"?(?<lang>[^\"]+)\"?");
    if ( langMatch.Success ) {
        return langMatch.Groups["lang"].ToString();
    }

    return defLang;
}

string FixCSLine(string sourceLine) {
    if (sourceLine == null)
        return null;

    sourceLine = Regex.Replace(sourceLine, "(?i)(\\t)", "    ");
    sourceLine = HttpUtility.HtmlEncode(sourceLine);

    String[] keywords = new String[]
        {"private", "protected", "public", "namespace", "class", "break",
         "for", "if", "else", "while", "switch", "case", "using",
         "return", "null", "void", "int", "bool", "string", "float",
         "this", "new", "true", "false", "const", "static", "base",
         "foreach", "in", "try", "catch", "finally", "get", "set", "char", "default"};

    String CombinedKeywords = "(?<keyword>" + String.Join("|", keywords) + ")";

    sourceLine = Regex.Replace(sourceLine, "\\b" + CombinedKeywords + "\\b(?<!//.*)", TAG_FNTBLUE + "${keyword}" + TAG_EFONT);
    sourceLine = Regex.Replace(sourceLine, "(?<comment>//.*$)", TAG_FNTGRN + "${comment}" + TAG_EFONT);

    return sourceLine;
}

string FixJSLine(string sourceLine) {
    if (sourceLine == null)
        return null;

    sourceLine = Regex.Replace(sourceLine, "(?i)(\\t)", "    ");
    sourceLine = HttpUtility.HtmlEncode(sourceLine);

    String[] keywords = new String[]
        {"private", "protected", "public", "namespace", "class", "var",
         "for", "if", "else", "while", "switch", "case", "using", "get",
         "return", "null", "void", "int", "string", "float", "this", "set",
         "new", "true", "false", "const", "static", "package", "function",
         "internal", "extends", "super", "import", "default", "break", "try",
         "catch", "finally" };

    String CombinedKeywords = "(?<keyword>" + String.Join("|", keywords) + ")";

    sourceLine = Regex.Replace(sourceLine, "\\b" + CombinedKeywords + "\\b(?<!//.*)", TAG_FNTBLUE + "${keyword}" + TAG_EFONT);
    sourceLine = Regex.Replace(sourceLine, "(?<comment>//.*$)", TAG_FNTGRN + "${comment}" + TAG_EFONT);

    return sourceLine;
}

string FixVBLine(string sourceLine) {
    if (sourceLine == null)
        return null;

    sourceLine = Regex.Replace(sourceLine, "(?i)(\\t)", "    ");
    sourceLine = HttpUtility.HtmlEncode(sourceLine);

    String[] keywords = new String[]
        {"Private", "Protected", "Public", "End Namespace", "Namespace",
         "End Class", "Exit", "Class", "Goto", "Try", "Catch", "End Try",
         "For", "End If", "If", "Else", "ElseIf", "Next", "While", "And",
         "Do", "Loop", "Dim", "As", "End Select", "Select", "Case", "Or",
         "Imports", "Then", "Integer", "Long", "String", "Overloads", "True",
         "Overrides", "End Property", "End Sub", "End Function", "Sub", "Me",
         "Function", "End Get", "End Set", "Get", "Friend", "Inherits",
         "Implements","Return", "Not", "New", "Shared", "Nothing", "Finally",
         "False", "Me", "My", "MyBase" };


    String CombinedKeywords = "(?<keyword>" + String.Join("|", keywords) + ")";

    sourceLine = Regex.Replace(sourceLine, "(?i)\\b" + CombinedKeywords + "\\b(?<!'.*)", TAG_FNTBLUE + "${keyword}" + TAG_EFONT);
    sourceLine = Regex.Replace(sourceLine, "(?<comment>'(?![^']*&quot;).*$)", TAG_FNTGRN + "${comment}" + TAG_EFONT);

    return sourceLine;
}

string FixAspxLine(string sourceLine ) {
    string searchExpr;      // search string
    string replaceExpr;     // replace string

    if ((sourceLine == null) || (sourceLine.Length == 0))
        return sourceLine;

    // Search for \t and replace it with 4 spaces.
    sourceLine = Regex.Replace(sourceLine, "(?i)(\\t)", "    ");
    sourceLine = HttpUtility.HtmlEncode(sourceLine);


    // Single line comment or #include references.
    searchExpr = "(?i)(?<a>(^.*))(?<b>(&lt;!--))(?<c>(.*))(?<d>(--&gt;))(?<e>(.*))";
    replaceExpr = "${a}" + TAG_FNTGRN + "${b}${c}${d}" + TAG_EFONT + "${e}";

    if (Regex.IsMatch(sourceLine, searchExpr))
        return Regex.Replace(sourceLine, searchExpr, replaceExpr);

    // Colorize <%@ <type>
    searchExpr = "(?i)" + "(?<a>(&lt;%@))" + "(?<b>(.*))" + "(?<c>(%&gt;))";
    replaceExpr = "<font color=blue><b>${a}${b}${c}</b></font>";

    if (Regex.IsMatch(sourceLine, searchExpr))
        sourceLine = Regex.Replace(sourceLine, searchExpr, replaceExpr);

    // Colorize <%# <type>
    searchExpr = "(?i)" + "(?<a>(&lt;%#))" + "(?<b>(.*))" + "(?<c>(%&gt;))";
    replaceExpr = "${a}" + "<font color=red><b>" + "${b}" + "</b></font>" + "${c}";

    if (Regex.IsMatch(sourceLine, searchExpr))
        sourceLine = Regex.Replace(sourceLine, searchExpr, replaceExpr);

    // Colorize tag <type>
    searchExpr = "(?i)" + "(?<a>(&lt;)(?!%)(?!/?asp:)(?!/?template)(?!/?property)(?!/?ibuyspy:)(/|!)?)" + "(?<b>[^;\\s&]+)" + "(?<c>(\\s|&gt;|\\Z))";
    replaceExpr = "${a}" + TAG_FNTMRN + "${b}" + TAG_EFONT + "${c}";

    if (Regex.IsMatch(sourceLine, searchExpr))
        sourceLine = Regex.Replace(sourceLine, searchExpr, replaceExpr);

    // Colorize asp:|template for runat=server tags <type>
    searchExpr = "(?i)(?<a>&lt;/?)(?<b>(asp:|template|property|IBuySpy:).*)(?<c>&gt;)?";
    replaceExpr = "${a}<font color=blue><b>${b}</b></font>${c}";

    if (Regex.IsMatch(sourceLine, searchExpr))
        sourceLine = Regex.Replace(sourceLine, searchExpr, replaceExpr);

    //colorize begin of tag char(s) "<","</","<%"
    searchExpr = "(?i)(?<a>(&lt;)(/|!|%)?)";
    replaceExpr = TAG_FNTBLUE + "${a}" + TAG_EFONT;

    if (Regex.IsMatch(sourceLine, searchExpr))
        sourceLine = Regex.Replace(sourceLine, searchExpr, replaceExpr);

    // Colorize end of tag char(s) ">","/>"
    searchExpr = "(?i)(?<a>(/|%)?(&gt;))";
    replaceExpr = TAG_FNTBLUE + "${a}" + TAG_EFONT;

    if (Regex.IsMatch(sourceLine, searchExpr))
        sourceLine = Regex.Replace(sourceLine, searchExpr, replaceExpr);

    return sourceLine;
}

bool IsScriptBlockTagStart(String source) {
    if (Regex.IsMatch(source, "<script.*runat=\"?server\"?.*>")) {
        return true;
    }
    if (Regex.IsMatch(source, "(?i)<%@\\s*WebService")) {
        return true;
    }
    return false;
}

bool IsScriptBlockTagEnd(String source) {
    if (Regex.IsMatch(source, "</script.*>")) {
        return true;
    }
    return false;
}

bool IsMultiLineTagStart(String source) {
    String searchExpr = "(?i)(?!.*&gt;)(?<a>&lt;/?)(?<b>(asp:|template|property|IBuySpy:).*)";

    source = HttpUtility.HtmlEncode(source);
    if ( Regex.IsMatch(source, searchExpr) ) {
        return true;
    }
    return false;
}

bool IsMultiLineTagEnd(String source) {
    String searchExpr = "(?i)&gt;";

    source = HttpUtility.HtmlEncode(source);
    if ( Regex.IsMatch(source, searchExpr) ) {
        return true;
    }
    return false;
}
</script>