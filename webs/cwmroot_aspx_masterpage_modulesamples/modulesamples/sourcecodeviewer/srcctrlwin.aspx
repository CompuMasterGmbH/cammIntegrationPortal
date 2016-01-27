<%@ Register TagPrefix="Acme" TagName="SourceCtrl" Src="SrcCtrl.ascx"%>
<%@ Import Namespace="System.Text" %>
<%@ Import Namespace="System.IO" %>

    <link rel="stylesheet" href="style.css">

    <script language="C#" runat="server">

      public void Page_Load (Object sender, EventArgs e)
      {
           try 
          {
              String path = Request.QueryString["path"];

              if (path == null) 
              {
                  ErrorMessage.InnerHtml = "Please specify a path variable in the querystring.";
                  return;
              }

              String file = Request.QueryString["file"];

              if (file != null) {
                  Trace.Write("SrcCtrlWin", "Path is " + path);

                  String dir = path + "/";  //was File.GetDirectoryNameFromPath(path)
                  Trace.Write("SrcCtrlWin", "Dir is " + dir);

                  MySourceCtrl.filename = Server.MapPath(dir + "\\" + file);
              }
            }
            catch (Exception) 
            {
                // Ignored
            }
      }

    </script>

    <form runat="server">

        <Acme:SourceCtrl id="MySourceCtrl" showfilename="true" runat="server" />
        <span style="color:red" id="ErrorMessage" runat="server"/>

    </form>
