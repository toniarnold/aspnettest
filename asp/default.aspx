<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="asp._default" %>
<%@ Register TagPrefix="uc" TagName="calculator" Src="~/calculator/Main.ascx" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<title>Calculator</title>
<style>
.nunitimg { 
    position: absolute;
    top: 10px;
    right: 10px;
    height: 50px;
    width: 50px;
}
</style>
</head>
<body>
<form id="form1" runat="server">

<asp:ImageButton ID="testButton" runat="server" OnClick="testButton_Click"
    ImageUrl="nunit.png" CssClass="nunitimg" />
<uc:calculator ID="calculator" runat="server" 
    StorageLinkUrl="~/triptych.aspx" />

</form>
</body>
</html>
