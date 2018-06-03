<%@ Page Title="Triptych" Language="C#" MasterPageFile="~/asp.Master" AutoEventWireup="true" CodeBehind="triptych.aspx.cs" Inherits="asp.triptych" %>
<%@ Register TagPrefix="uc" TagName="calculator" Src="~/calculator/Main.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="calc-container">
    <div class="calc-box ">
        <asp:UpdatePanel id="UpdatePanel1" runat="server"
            UpdateMode="Always" >
            <ContentTemplate>
                <uc:calculator ID="calculatorViewstate" runat="server"
                    Storage="Viewstate"
                    StorageLinkUrl="default.aspx?storage=viewstate"
                    StorageLinkClientID="StorageLinkViewstate"
                    />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div class="calc-box ">
        <asp:UpdatePanel id="UpdatePanel2" runat="server"
            UpdateMode="Always" >
            <ContentTemplate>
                <uc:calculator ID="calculatorSession" runat="server"
                    Storage="Session"
                    StorageLinkUrl="default.aspx?storage=session"
                    StorageLinkClientID="StorageLinkSession"
                    />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div class="calc-box ">
        <asp:UpdatePanel id="UpdatePanel3" runat="server"
            UpdateMode="Always" >
            <ContentTemplate>
                <uc:calculator ID="calculatorDatabase" runat="server"
                    Storage="Database"
                    StorageLinkUrl="default.aspx?storage=database"
                    StorageLinkClientID="StorageLinkDatabase"
                    />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
</asp:Content>
