<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Calculate.ascx.cs" Inherits="asp.calculator.View.Calculate" %>
<style>
    .button {
        height: 30px;
        width: 35px;
    }
</style>
<p>
    <asp:button id="addButton" runat="server" text=" + " onclick="addButton_Click" cssclass="button" />
    <asp:button id="subButton" runat="server" text=" - " onclick="subButton_Click" cssclass="button" />
    <asp:button id="mulButton" runat="server" text=" * " onclick="mulButton_Click" cssclass="button" />
    <asp:button id="divButton" runat="server" text=" / " onclick="divButton_Click" cssclass="button" />
    <button id="powButton" runat="server"
        onserverclick="powButton_Click"
        class="button"
        type="button"
        style="position: relative; top: 1px;">
        x<sup>2</sup>
    </button>
    <button id="sqrtButton" runat="server"
        onserverclick="sqrtButton_Click"
        class="button"
        type="button">
        &radic;<span style='text-decoration: overline'>x</span>
    </button>
    <asp:button id="clrButton" runat="server" text=" C " onclick="clrButton_Click" cssclass="button" />
    <asp:button id="clrAllButton" runat="server" text="CA" onclick="clrAllButton_Click" cssclass="button" />
</p>

<p>
    <%=this.Stack %>
</p>