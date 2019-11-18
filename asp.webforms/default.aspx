<%@ Page Title="Calculator" Language="C#" MasterPageFile="~/asp.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="asp._default" %>

<%@ Register Assembly="iie.webforms" Namespace="iie" TagPrefix="iie" %>
<%@ Register TagPrefix="uc" TagName="calculator" Src="~/calculator/Main.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ImageButton ID="testButton" runat="server" OnClick="testButton_Click"
        ImageUrl="nunit.png" CssClass="nunitimg" />
    <iie:testresult id="testResult" runat="server"
        cssclass="testresult" />
    <uc:calculator id="calculator" runat="server"
        storagelinkurl="triptych.aspx"
        storagelinkclientid="TriptychLink" />
</asp:Content>