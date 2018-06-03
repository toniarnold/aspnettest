<%@ Page Title="Calculator" Language="C#" MasterPageFile="~/asp.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="asp._default" %>
<%@ Register TagPrefix="uc" TagName="calculator" Src="~/calculator/Main.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
.nunitimg { 
    position: absolute;
    top: 10px;
    right: 10px;
    height: 50px;
    width: 50px;
}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<asp:ImageButton ID="testButton" runat="server" OnClick="testButton_Click"
    ImageUrl="nunit.png" CssClass="nunitimg" />
<uc:calculator ID="calculator" runat="server" 
    StorageLinkUrl="triptych.aspx" 
    StorageLinkClientID="TriptychLink" />
</asp:Content>

