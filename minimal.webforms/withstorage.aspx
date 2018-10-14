<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="withstorage.aspx.cs" Inherits="minimal.withstorage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>storage</title>
    <style>
        h1 {
            font-size: 1.25em;
        }

        ul {
            list-style-type: none;
            margin: 0;
            padding: 0;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>minimalist test setup with storage</h1>
            Session Storage:
            <asp:RadioButtonList ID="storageList" runat="server" RepeatDirection="Horizontal"
                AutoPostBack="true"
                OnSelectedIndexChanged="storageList_SelectedIndexChanged">
                <asp:ListItem Text="ViewState" Value="ViewState" Selected="True"></asp:ListItem>
                <asp:ListItem Text="Session" Value="Session"></asp:ListItem>
                <asp:ListItem Text="Database" Value="Database"></asp:ListItem>
            </asp:RadioButtonList>
            <asp:TextBox ID="contentTextBox" runat="server"></asp:TextBox>
            <asp:Button ID="submitButton" runat="server"
                Text="Submit"
                OnClick="submitButton_Click" />
            <asp:BulletedList ID="contentList" runat="server"
                EnableViewState="false">
            </asp:BulletedList>
        </div>
    </form>
</body>
</html>