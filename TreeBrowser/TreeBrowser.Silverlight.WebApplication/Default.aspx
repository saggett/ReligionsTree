<%@ Page Language="C#" EnableViewState="false" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
    Inherits="TreeBrowser.Silverlight.WebApplication.Default" %>

<%@ Register TagPrefix="cd" TagName="ContentDetail" Src="~/ContentDetail.ascx" %>
<%@ Register TagPrefix="rd" TagName="ReligionDetail" Src="~/ReligionDetail.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta name="verify-v1" content="CLgMrUPZPPq6tavy6eezPlYpBCjHRp4IHvaQp4nltJM=" />
    <meta property="og:title" content="Religions Tree: The Evolution of the World's Religions from Beginning to Present" />
    <meta property="og:type" content="website" />
    <meta property="og:image" content="http://www.religionstree.com/Resources/treekanjiblack.gif" />
    <meta property="og:url" content="http://www.religionstree.com/" />
    <meta property="og:site_name" content="Religions Tree" />
    <meta property="fb:admins" content="587449415" />
    <meta property="og:description" content="Religionstree.com contains the history and origins of the world's religions in tree form, revealing the evolution of religion from beginning to present." />
    <style type="text/css">
        html, body
        {
            height: 100%;
            overflow: auto;
        }
        body
        {
            padding: 0;
            margin: 0;
        }
        #silverlightControlHost
        {
            height: 100%;
            text-align: left;
        }
    </style>
    <script type="text/javascript" src="Silverlight.js"></script>
    <script type="text/javascript">
        function onSilverlightError(sender, args) {
            var appSource = "";
            if (sender != null && sender != 0) {
                appSource = sender.getHost().Source;
            }

            var errorType = args.ErrorType;
            var iErrorCode = args.ErrorCode;

            if (errorType == "ImageError" || errorType == "MediaError") {
                return;
            }

            var errMsg = "Unhandled Error in Silverlight Application " + appSource + "\n";

            errMsg += "Code: " + iErrorCode + "    \n";
            errMsg += "Category: " + errorType + "       \n";
            errMsg += "Message: " + args.ErrorMessage + "     \n";

            if (errorType == "ParserError") {
                errMsg += "File: " + args.xamlFile + "     \n";
                errMsg += "Line: " + args.lineNumber + "     \n";
                errMsg += "Position: " + args.charPosition + "     \n";
            }
            else if (errorType == "RuntimeError") {
                if (args.lineNumber != 0) {
                    errMsg += "Line: " + args.lineNumber + "     \n";
                    errMsg += "Position: " + args.charPosition + "     \n";
                }
                errMsg += "MethodName: " + args.methodName + "     \n";
            }

            throw new Error(errMsg);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server" style="height: 95%;">
    <div id="silverlightControlHost">
        <object data="data:application/x-silverlight-2," type="application/x-silverlight-2"
            width="100%" height="100%">
            <param name="source" value="ClientBin/TreeBrowser.Silverlight.Application.xap" />
            <param name="onError" value="onSilverlightError" />
            <param name="background" value="white" />
            <param name="initParams" value="lineageId=<%= this.Server.UrlEncode(this.Request.QueryString["lineageId"]) %>,contentName=<%= this.Server.UrlEncode(this.Request.QueryString["contentName"]) %>" />
            <param name="minRuntimeVersion" value="3.0.40624.0" />
            <param name="autoUpgrade" value="true" />
            <param name="windowless" value="false" />
            <div style="margin-left: 10px">
                <h1>
                    Religions Tree: The Evolution of the World's Religions from Beginning to Present</h1>
                <p>
                    <b>Download the Microsoft Silverlight browser plugin to enable the full functionality
                        of religionstree.com:</b></p>
                <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=4.0.50401.0" style="text-decoration: none;">
                    <img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="Get Microsoft Silverlight"
                        style="border-style: none" />
                </a><span style="margin-right: 10px;"><a href="Default.aspx">Home</a></span> <span
                    style="margin-right: 10px;"><a href="Default.aspx?contentName=About">About</a></span>
                <span style="margin-right: 10px;"><a href="Default.aspx?contentName=Bibliography">Bibliography</a></span>
                <br />
                <cd:ContentDetail ID="PageContent" runat="server" />
                <rd:ReligionDetail ID="ReligionContent" runat="server" />
            </div>
        </object>
        <iframe id="_sl_historyFrame" style="visibility: hidden; height: 0px; width: 0px; border: 0px">
        </iframe>
    </div>
    </form>
    <iframe src="http://www.facebook.com/plugins/like.php?href=http%3A%2F%2Fwww.religionstree.com&amp;layout=standard&amp;show_faces=false&amp;width=450&amp;action=like&amp;font=lucida+grande&amp;colorscheme=light&amp;height=50"
        scrolling="no" frameborder="0" style="border: none; overflow: hidden; width: 450px;
        position: absolute; bottom: 0px; left: 40px; height: 5%; min-height: 50px;" allowtransparency="true">
    </iframe>
</body>
</html>
