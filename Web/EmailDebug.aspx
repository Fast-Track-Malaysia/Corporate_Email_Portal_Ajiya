<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EmailDebug.aspx.cs" Inherits="Web.EmailDebug" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">



    <div class="jumbotron">
        <h1>Debug page</h1>
        <p>
            <b>No of Users Online: <%=Application["OnlineVisitors"].ToString()%></b>
            <asp:Label class="text-danger" ID="Label_Errors" runat="server" Text=""></asp:Label>


        </p>
    </div>

    <asp:Button ID="ButtonRefresh" runat="server" CssClass="btn btn-default btn-xs" Text="Refresh online users" OnClick="ButtonRefresh_Click" />
    <asp:Label class="text-danger" ID="Label_OnlineUser" runat="server" Text=""></asp:Label>
    <asp:DataGrid ID="UserGrid" runat="server" CssClass="table"
        CellPadding="2" CellSpacing="1"
        GridLines="Both">
        <HeaderStyle BackColor="darkblue" ForeColor="white" />
    </asp:DataGrid>

    <div class="panel-group">

        <div class="panel panel-primary">
            <div class="panel-heading" data-toggle="collapse" data-target="#collapse1">
                <asp:Button ID="GenerateScheme" runat="server" Text="Button" OnClick="GenerateScheme_Click" class="btn btn-default btn-xs" />
                Generate Schema Files
            </div>
            <div class="panel-body panel-collapse collapse" id="collapse1">
                <asp:Label ID="Label_CreateSchema" runat="server" Text=""></asp:Label>
            </div>
        </div>

        <div class="panel panel-success">
            <div class="panel-heading" data-toggle="collapse" data-target="#collapse2">
                <asp:Button ID="ButtonBrowser" runat="server" Text="Get Browser Info" OnClick="ButtonBrowser_Click" class="btn btn-default btn-xs" />
                Browser Infomations
            </div>
            <div class="panel-body panel-collapse collapse" id="collapse2">
                <asp:Label ID="LabelBrowserInfo" runat="server" Text=""></asp:Label>
            </div>
        </div>

        <div class="panel panel-info">
            <div class="panel-heading" data-toggle="collapse" data-target="#collapse3">
                <asp:Button ID="ButtonAccess" runat="server" Text="Get Access Info" OnClick="ButtonAccess_Click" class="btn btn-default btn-xs" />
                Access Infomations
            </div>
            <div class="panel-body panel-collapse collapse" id="collapse3">
                <asp:Label ID="LabelAccess" runat="server" Text=""></asp:Label>
            </div>
        </div>

    </div>
</asp:Content>
