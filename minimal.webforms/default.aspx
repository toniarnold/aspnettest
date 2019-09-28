<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="minimal._default" %>

<%@ Register Assembly="iie.webforms" Namespace="iie" TagPrefix="iie" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>minimal</title>
    <style>
        h1 {
            font-size: 1.25em;
        }

        a {
            text-decoration: none;
        }

        ul {
            list-style-type: none;
            margin: 0;
            padding: 0;
        }

        .nunitimg {
            position: absolute;
            top: 8px;
            right: 8px;
            height: 50px;
            width: 50px;
        }

        .testresult {
            position: absolute;
            right: 0px;
            top: 59px;
            width: 140px;
            background-color: rgba(255, 255, 255, 1);
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>minimalist test setup</h1>
            <ul>
                <li><a href="withroot.aspx" id="withroot-link">with root</a></li>
                <li><a href="withstorage.aspx" id="withstorage-link">with storage</a></li>
            </ul>
            <asp:ImageButton ID="testButton" runat="server" OnClick="testButton_Click"
                ImageUrl="nunit.png" CssClass="nunitimg" />
            <iie:testresult id="testResult" runat="server"
                cssclass="testresult" />
        </div>
    </form>
</body>
</html>