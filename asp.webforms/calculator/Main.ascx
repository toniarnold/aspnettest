<%@ control language="C#" autoeventwireup="true" codebehind="Main.ascx.cs" inherits="asp.calculator.Main" %>
<%@ register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>
<%@ register assembly="asplib.webforms" namespace="asplib.View" tagprefix="asplib" %>
<%@ register tagprefix="uc" tagname="Calculate" src="~/calculator/View/Calculate.ascx" %>
<%@ register tagprefix="uc" tagname="Enter" src="~/calculator/View/Enter.ascx" %>
<%@ register tagprefix="uc" tagname="Error" src="~/calculator/View/Error.ascx" %>
<%@ register tagprefix="uc" tagname="Footer" src="~/calculator/View/Footer.ascx" %>
<%@ register tagprefix="uc" tagname="Splash" src="~/calculator/View/Splash.ascx" %>
<%@ register tagprefix="uc" tagname="Title" src="~/calculator/View/Title.ascx" %>
<style>
    .header-container {
        display: inline-grid;
        grid-template-columns: auto auto auto;
        width: 100%;
    }

    .hamburger-box {
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
        background-color: rgba(255, 255, 255, 1);
        font-size: small;
    }

    .link {
        text-decoration: none;
    }
</style>

<div class="header-container">
    <asplib:sharebutton id="shareButton" runat="server"
        onserverclick="shareButton_Click" />
    <a href="<%= this.StorageLinkUrl %>" id="<%= this.StorageLinkClientID %>" class="hamburger-box">Session Storage: <%= this.Storage %> <%= this.Encrypted %>
    </a>
    <div class="hamburger-box" id="hamburgerDiv" runat="server">
        <span class="hamburger">☰</span>
    </div>
    <ajaxtoolkit:popupcontrolextender id="PopEx" runat="server"
        targetcontrolid="hamburgerDiv"
        popupcontrolid="gridViewPanel"
        position="Bottom"
        offsetx="-100" />
    <asp:objectdatasource id="sessionDumpDataSource" runat="server"
        typename="asp.calculator.Main"
        selectmethod="AllMainRows" />
    <asp:updatepanel id="gridViewPanel" runat="server"
        updatemode="Always">
        <ContentTemplate>
            <asp:GridView ID="sessionDumpGridView" runat="server"
                DataSourceID="sessionDumpDataSource"
                ItemType="asplib.Model.Main"
                OnRowCommand="gridView_RowCommand"
                DataKeyNames="session"
                AutoGenerateColumns="false"
                EmptyDataText="no saved session objects found"
                CssClass="grid">
                <Columns>
                    <asp:BoundField DataField="created" HeaderText="Created" />
                    <asp:BoundField DataField="changed" HeaderText="Changed" />
                    <asp:TemplateField HeaderText="Stack">
                        <ItemTemplate>
                            <asp:Label ID="stackLabel" runat="server"
                                Text='<%# Item.GetInstance<asp.calculator.Control.Calculator>().StackHtmlString %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="deleteLinkButton" runat="server"
                                CommandName="Del"
                                CommandArgument="<%# Item.session %>"
                                CssClass="link"
                                Text="&#x232b;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:HyperLink ID="linkHyperLink" runat="server"
                                CssClass="link"
                                Text="&#x1f517;"
                                NavigateUrl="<%# this.Url(Item.session) %>" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </asp:updatepanel>
</div>

<uc:title id="title" runat="server" />
<hr />
<uc:splash id="splash" runat="server" />
<uc:enter id="enter" runat="server" />
<uc:calculate id="calculate" runat="server" />
<uc:error id="error" runat="server" />
<hr />
<uc:footer id="footer" runat="server" />