<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Calculate.ascx.cs" Inherits="asp.calculator.View.Calculate" %>
<p>
<asp:Button ID="addButton" runat="server" Text=" + " OnClick="addButton_Click" Height="2em" />
<asp:Button ID="subButton" runat="server" Text=" - " OnClick="subButton_Click" Height="2em" />
<asp:Button ID="mulButton" runat="server" Text=" * " OnClick="mulButton_Click" Height="2em" />
<asp:Button ID="divButton" runat="server" Text=" / " OnClick="divButton_Click" Height="2em" />
<Button ID="powButton" runat="server" OnServerClick="powButton_Click" 
    style="height:2em; position:relative; top:1px;" >
    x<sup>2</sup>
</Button>
<Button ID="sqrtButton" runat="server" OnServerClick="sqrtButton_Click" 
    style="height:2em;" >
    &radic;<span style='text-decoration:overline'>x</span>
</Button>
<asp:Button ID="clrButton" runat="server" Text=" C " OnClick="clrBUtton_Click" Height="2em" />
<asp:Button ID="clrAllButton" runat="server" Text="CA" OnClick="clrAllButton_Click" Height="2em" />
</p>

<p>
    <%=this.Stack %>
</p>