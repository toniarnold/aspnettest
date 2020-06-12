<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Footer.ascx.cs" Inherits="asp.calculator.View.Footer" %>
<asp:Button ID="enterButton" runat="server" OnClick="enterButton_Click"
    Text="Enter >"
    Style="position: relative; z-index: 2000;" />
<!-- Style hack, fixes Selenium "Element not clickable at point" due to ajaxtoolkit:popupcontrolextender
     auto-assigning style="left: 0px; top: 96px; visibility: visible; position: absolute; z-index: 1000;"
     to Main.ascx' gridViewPanel (other Browsers get left: 658px)-->