<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="triptych.aspx.cs" Inherits="asp.triptych" %>
<%@ Register TagPrefix="uc" TagName="calculator" Src="~/calculator/Main.ascx" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<title>Calculator</title>
<style>
.calc-container {
    display: inline-grid;
    grid-template-columns: auto auto auto;
    grid-gap: 10px;
}
.calc-box  {
    border: 1px solid black;
    border-radius: 10px;
    padding: 30px;
}
</style>
</head>
<body>
<form id="form1" runat="server">
<asp:ScriptManager ID="ScriptManager1" runat="server" />

<div class="calc-container">
    <div class="calc-box ">
        <asp:UpdatePanel id="UpdatePanel1" runat="server"
            UpdateMode="Always" >
            <ContentTemplate>
                <uc:calculator ID="calculator1" runat="server"
                    Storage="Viewstate"
                    StorageLinkUrl="~/default.aspx?storage=viewstate"
                    />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div class="calc-box ">
        <asp:UpdatePanel id="UpdatePanel2" runat="server"
            UpdateMode="Always" >
            <ContentTemplate>
                <uc:calculator ID="calculator2" runat="server"
                    Storage="Session"
                    StorageLinkUrl="~/default.aspx?storage=session"
                    />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div class="calc-box ">
        <asp:UpdatePanel id="UpdatePanel3" runat="server"
            UpdateMode="Always" >
            <ContentTemplate>
                <uc:calculator ID="calculator3" runat="server"
                    Storage="Database"
                    StorageLinkUrl="~/default.aspx?storage=database"
                    />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>

</form>
</body>
</html>
