<%@ Page Language="C#" Trace="false" %>
<%@ Register TagPrefix="Acme" TagName="SourceCtrl" Src="SrcCtrl.ascx"%>
<%@ Import Namespace="System.Text" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>

<html>
  <head>
    <title>camm Web-Manager Module Samples Source Viewer</title>

    <link rel="stylesheet" href="style.css">

    <script language="C#" runat="server">

      public bool showtitle = true;
      public bool listonly = false;
      public String width = "100%";
      public String path;
      private String font;

      public void Page_Load (Object sender, EventArgs e) {
            if ( !Page.IsPostBack ){
                font = Request.QueryString["font"];
                if (font != null) {
                    FontSize.SelectedIndex = (Int32.Parse(font) - 1);
                } else {
                    font = (String) Session["SrcViewer.FontSize"];
                    if ( font != null ) {
                        FontSize.SelectedIndex = (Int32.Parse(font.ToString()) - 1);
                    } else {
                        FontSize.SelectedIndex = 2;
                    }
                }
            }

            if ( font == null ) {
                font = FontSize.SelectedItem.Value;
            }

            MySourceCtrl.FontSize = Int32.Parse(FontSize.SelectedItem.Value);

            try
            {
              if (path == null)
                  path = Request.QueryString["path"];

              if (path == null) {
                  ErrorMessage.InnerHtml = "Please specify a path variable in the querystring.";
                  return;
              }

              String defaultFile = null;
              String defaultFilename = null;
              String file = Request.QueryString["file"];

              if (file != null) {
                  String dir = new FileInfo(Server.MapPath(path)).DirectoryName;
                  MySourceCtrl.filename = dir + "\\" + file;
              }

              FileStream fs = new FileStream(Server.MapPath(path),FileMode.Open,FileAccess.Read);
              StreamReader sr = new StreamReader(fs);
              String line;

              // Loop through each line of the .src file
              while ((line = sr.ReadLine()) != null) {
                  Trace.Write("SrcViewer", "Reading in Line");

                  HtmlTableRow row = new HtmlTableRow();

                  HtmlTableCell cell = new HtmlTableCell();

                  String[] list = line.Split(new char[] { ':' });

                  if (!(list.Length > 1)) {
                      break;
                  }

                  String sourcegroup = list[0];  // Grouping of files

                  cell.InnerHtml = "<b>" + sourcegroup + ": </b>";
                  cell.Style.Add("padding-right","10");
                  row.Cells.Add(cell);

                  String[] sourcelist = list[1].Split(new char[] { ',' });

                  HttpCookie codeCookie = Page.Request.Cookies["langpref"];

                  String value = "VB";
                  if (codeCookie != null) {
                      value = codeCookie.Value;
                  }

                  // Try to find a match for the langpref cookie
                  if ((file==null)&&(sourcelist.Length>0)&&(sourcegroup.StartsWith(value))) {
                      String dir = new FileInfo(Server.MapPath(path)).DirectoryName;
                      MySourceCtrl.filename = dir + "\\" + sourcelist[0];
                      file = sourcelist[0];
                  }

                  // Just in case there is no match, store a pointer to the first file
                  if ((defaultFile==null)&&(sourcelist.Length>0)) {
                      String dir = new FileInfo(Server.MapPath(path)).DirectoryName;
                      defaultFilename = dir + "\\" + sourcelist[0];
                      defaultFile = sourcelist[0];
                  }

                  // Create the list of file hyperlinks for this sourcegroup
                  StringBuilder sb = new StringBuilder();
                  Trace.Write("SrcViewer", "Source Group = " + sourcegroup);
                  for (int i=0; i<sourcelist.Length; i++) {
                      String sourcefile = sourcelist[i];
                      if (sourcegroup == "VS.NET") {

                          String dir = new FileInfo(Server.MapPath(path)).DirectoryName;
                          String fullPath = dir + "\\" + sourcefile;
                          Match m = Regex.Match(fullPath, ".+\\\\(?<end>.+)");

                          // TODO: remove path from sourcefile

                          // IF REQUESTOR IS NOT LOCAL
                          if (Request.ServerVariables["REMOTE_ADDR"] != Request.ServerVariables["LOCAL_ADDR"]) {
                              HttpCookie cookieQS = Page.Request.Cookies["__quickstart_ver1"];

                              // for EXE samples, change virtual path to physical path if cookie is present
                              if (cookieQS != null) {
                                  if ( sourcefile.StartsWith("\\") || sourcefile.StartsWith("/") ) {
                                      sourcefile = sourcefile.Substring(1);
                                  }
                                  fullPath = Path.Combine((String)cookieQS.Values["installroot"], sourcefile);
                              }

                              // if no cookie is present, redirect user to the setup page
                              if (cookieQS == null) {
                                  fullPath = "/quickstart/setup.aspx?ReturnUrl=" + Request.Url;
                              }
                          }
                          sb.Append("<a href='" +  fullPath + "'>");
                          sb.Append(m.Groups["end"].ToString());
                      }
                      else {
                          if (listonly)
                              sb.Append("<a target='_blank' href='srcctrlwin.aspx?path=" + path + "&file=" + sourcefile + "'>");
                          else
                              sb.Append("<a href='srcview.aspx?path="
                                        + path + "&file=" + sourcefile + "&font=" + font + "'>");

                          sb.Append(sourcefile);
                      }
                      sb.Append("</a> &nbsp;&nbsp");
                  }

                  cell = new HtmlTableCell();
                  cell.InnerHtml = sb.ToString();
                  row.Cells.Add(cell);
                  SourceTable.Rows.Add(row);
              }

              // If there was no match for the langpref cookie,
              // show the first file of the first sourcegroup

              if (file==null)
                  MySourceCtrl.filename = defaultFilename;

              fs.Close();

              if (showtitle != false) {
                 if (file != null)
                     Title.InnerHtml = file;
                 else if (defaultFile != null)
                     Title.InnerHtml = defaultFile;
              }
              else
                  Title.InnerHtml = "";

            }
            catch (Exception exc) {
                Trace.Write("Exception", exc.ToString());
            }

            Session["SrcViewer.FontSize"] = font;
      }

      public void LinkButton_Click (Object sender, EventArgs e)
      {
          // do nothing
      }

      public void Size_Select (Object sender, EventArgs e)
      {
            MySourceCtrl.FontSize = Int32.Parse(FontSize.SelectedItem.Value);
            Session["SrcViewer.FontSize"] = FontSize.SelectedItem.Value.ToString();
      }

    </script>
  </head>
  <body bgcolor="#FFFFFF" text="#000000">
    <form id="Form" runat="server">
        <div class="SampleHeader" style="width:<%=width%>">
            <div class="SampleTitle">
                <table width="100%">
                    <tr>
                        <td>
                            <span class="SampleTitle" id="Title" runat="server">ASP.NET Source Code Viewer</span>
                        </td>
                        <td align="right">
                            <span class="SampleTitle">Font Size:</span>
                            <asp:DropDownList class="Select" id="FontSize" OnSelectedIndexChanged="Size_Select" AutoPostBack runat="server">
                              <asp:ListItem value="1" runat="server">1</asp:ListItem>
                              <asp:ListItem value="2" runat="server">2</asp:ListItem>
                              <asp:ListItem value="3" runat="server">3</asp:ListItem>
                              <asp:ListItem value="4" runat="server">4</asp:ListItem>
                              <asp:ListItem value="5" runat="server">5</asp:ListItem>
                              <asp:ListItem value="6" runat="server">6</asp:ListItem>
                              <asp:ListItem value="7" runat="server">7</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </div>
            <table id="SourceTable" EnableViewState="true" style="font: 8pt Verdana" runat="server"/>
        </div>

        <Acme:SourceCtrl id="MySourceCtrl" runat="server" />
        <span style="color:red" id="ErrorMessage" runat="server"/>

    </form>
  </body>
</html>

