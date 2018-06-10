<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Calculate.ascx.cs" Inherits="asp.calculator.View.Calculate" %>
<style>
    .button {
        height: 30px;
        width: 35px;
    }
</style>
<p>
    <asp:Button ID="addButton" runat="server" Text=" + " OnClick="addButton_Click" CssClass="button" />
    <asp:Button ID="subButton" runat="server" Text=" - " OnClick="subButton_Click" CssClass="button" />
    <asp:Button ID="mulButton" runat="server" Text=" * " OnClick="mulButton_Click" CssClass="button" />
    <asp:Button ID="divButton" runat="server" Text=" / " OnClick="divButton_Click" CssClass="button" />
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
    <asp:Button ID="clrButton" runat="server" Text=" C " OnClick="clrBUtton_Click" CssClass="button" />
    <asp:Button ID="clrAllButton" runat="server" Text="CA" OnClick="clrAllButton_Click" CssClass="button" />
</p>

<p>
    <%=this.Stack %>
</p>