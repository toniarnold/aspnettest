<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Main.ascx.cs" Inherits="asp.calculator.Main" %>
<%@ Register TagPrefix="uc" TagName="Calculate" Src="~/calculator/View/Calculate.ascx" %>
<%@ Register TagPrefix="uc" TagName="Enter" Src="~/calculator/View/Enter.ascx" %>
<%@ Register TagPrefix="uc" TagName="Error" Src="~/calculator/View/Error.ascx" %>
<%@ Register TagPrefix="uc" TagName="Footer" Src="~/calculator/View/Footer.ascx" %>
<%@ Register TagPrefix="uc" TagName="Splash" Src="~/calculator/View/Splash.ascx" %>
<%@ Register TagPrefix="uc" TagName="Title" Src="~/calculator/View/Title.ascx" %>
<style>
.headlink-container { 
    width: 100%;
}
.headlink-box {
  display: flex;
  align-items: center;
  justify-content: center;
}
</style>
<div class="headlink-container">
    <asp:HyperLink ID="storageLink" runat="server"
        CssClass="headlink-box"
        ToolTip="click to select the storage type"
        >
        Session Storage: <%= this.Storage %>
    </asp:HyperLink>
</div>

<uc:Title ID="title" runat="server" />
<hr />
<uc:Splash ID="splash" runat="server" />
<uc:Enter ID="enter" runat="server" />
<uc:Calculate ID="calculate" runat="server" />
<uc:Error ID="error" runat="server" />
<hr />
<uc:Footer ID="footer" runat="server" />
