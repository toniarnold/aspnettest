<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="withroot.aspx.cs" Inherits="minimal.withroot" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>withroot</title>
    <style>
        h1 {
            font-size:1.25em;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Panel ID="mainPanel" runat="server" DefaultButton="submitButton">
            <h1>minimalist test setup with root</h1>
            <asp:TextBox ID="contentTextBox" runat="server"
                ></asp:TextBox>
            <asp:Button ID="submitButton" runat="server"
                Text="Submit"
                OnClick="submitButton_Click" />
            <asp:BulletedList ID="contentList" runat="server"
                BulletStyle="Numbered"
                ></asp:BulletedList>
        </asp:Panel>
    </form>
</body>
</html>
