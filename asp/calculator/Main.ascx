<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Main.ascx.cs" Inherits="asp.calculator.Main" %>
<%@ Register TagPrefix="uc" TagName="Calculate" Src="~/calculator/View/Calculate.ascx" %>
<%@ Register TagPrefix="uc" TagName="Enter" Src="~/calculator/View/Enter.ascx" %>
<%@ Register TagPrefix="uc" TagName="Error" Src="~/calculator/View/Error.ascx" %>
<%@ Register TagPrefix="uc" TagName="Footer" Src="~/calculator/View/Footer.ascx" %>
<%@ Register TagPrefix="uc" TagName="Splash" Src="~/calculator/View/Splash.ascx" %>
<%@ Register TagPrefix="uc" TagName="Title" Src="~/calculator/View/Title.ascx" %>

<h1>Calculator</h1>

<uc:Title ID="title" runat="server" />
<hr />
<uc:Splash ID="splash" runat="server" />
<uc:Enter ID="enter" runat="server" />
<uc:Calculate ID="calculate" runat="server" />
<uc:Error ID="error" runat="server" />
<hr />
<uc:Footer ID="footer" runat="server" />
