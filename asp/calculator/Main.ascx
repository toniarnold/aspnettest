<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Main.ascx.cs" Inherits="asp.calculator.Main" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="asplib" Namespace="asplib" TagPrefix="asplib" %>
<%@ Register TagPrefix="uc" TagName="Calculate" Src="~/calculator/View/Calculate.ascx" %>
<%@ Register TagPrefix="uc" TagName="Enter" Src="~/calculator/View/Enter.ascx" %>
<%@ Register TagPrefix="uc" TagName="Error" Src="~/calculator/View/Error.ascx" %>
<%@ Register TagPrefix="uc" TagName="Footer" Src="~/calculator/View/Footer.ascx" %>
<%@ Register TagPrefix="uc" TagName="Splash" Src="~/calculator/View/Splash.ascx" %>
<%@ Register TagPrefix="uc" TagName="Title" Src="~/calculator/View/Title.ascx" %>
<style>
.headlink-container { 
    display: inline-grid;
    grid-template-columns: auto auto auto;
    width: 100%;
}
.headlink-box {
    display: flex;
    align-items: center;
    justify-content: center;
}
.hamburger {
    border: 1px solid black;
    padding: 2px; 
    display: block;
}
.grid {
    background-color:rgba(255, 255, 255, 1);
    font-size: small;
}
.link {
    text-decoration: none;
}
</style>
<div class="headlink-container">
    <div></div>
    <asp:HyperLink ID="storageLink" runat="server"
        CssClass="headlink-box"
        ToolTip="click to select the storage type"
        >
        Session Storage: <%= this.Storage %>
    </asp:HyperLink>
    <div class="headlink-box" ID="headlinkDiv" runat="server">
        <span class="hamburger">&#x2630;</span>
    </div>
    <ajaxToolkit:PopupControlExtender ID="PopEx" runat="server"
        TargetControlID="headlinkDiv"
        PopupControlID="gridViewPanel"
        Position="Bottom" 
        OffsetX="-180"
        />
    <asp:ObjectDataSource ID="sessionDumpDataSource" runat="server" 
        TypeName="asp.calculator.Main"
        SelectMethod="AllMainRows"
        />
    <asp:UpdatePanel id="gridViewPanel" runat="server"
        UpdateMode="Always" >
        <ContentTemplate>
            <asp:GridView ID="sessionDumpGridView" runat="server"
                DataSourceID="sessionDumpDataSource"
                ItemType="asplib.Model.Main"
                OnRowCommand="gridView_RowCommand"
                DataKeyNames="session"
                AutoGenerateColumns="false"
                EmptyDataText="no saved session objects found"
                CssClass="grid"
                >
                <Columns>
                    <asp:BoundField DataField="created" HeaderText="Created" />
                    <asp:BoundField DataField="changed" HeaderText="Changed" />
                    <asp:TemplateField HeaderText="Stack">
                        <ItemTemplate>
                            <%# Item.GetInstance<asp.calculator.Control.Calculator>().StackHtmlString %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="deleteButton" runat="server" 
                                CommandName="Del"
                                CommandArgument="<%# Item.session %>"
                                CssClass="link"
                                Text="&#x232b;"
                                />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:HyperLink ID="link" runat="server" 
                                CssClass="link"
                                Text="&#x1f517;" 
                                NavigateUrl="<%# this.Url(Item.session) %>"
                                />                    
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>

<uc:Title ID="title" runat="server" />
<hr />
<uc:Splash ID="splash" runat="server" />
<uc:Enter ID="enter" runat="server" />
<uc:Calculate ID="calculate" runat="server" />
<uc:Error ID="error" runat="server" />
<hr />
<uc:Footer ID="footer" runat="server" />
