<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EmailAccessLogs.aspx.cs" Inherits="Web.EmailAccessLogs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
         .jumbotron {
            background-color: #fb7e7e;
            color: #fff;         
            font-family: Montserrat, sans-serif;
        }
    </style>
    <div class="jumbotron bg-warning">
        <h1>Visitor informations</h1>
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

    <asp:Button ID="ButtonLocation" runat="server" CssClass="btn btn-default btn-xs" Text="Refresh users location" OnClick="ButtonLocation_Click" />
    <asp:DataGrid ID="DataGridLocation" runat="server" CssClass="table"
        CellPadding="2" CellSpacing="1"
        GridLines="Both">
        <HeaderStyle BackColor="darkblue" ForeColor="white" />
    </asp:DataGrid>

    <div class="panel-group">

        <asp:Button ID="ButtonBrowser" runat="server" Text="Get Browser Info" OnClick="ButtonBrowser_Click" class="btn btn-default btn-xs" />
        <div class="panel panel-success">
            <div class="panel-heading" data-toggle="collapse" data-target="#collapse2">
                Browser Infomations
            </div>
            <div class="panel-body panel-collapse collapse out" id="collapse2">
                <asp:Label ID="LabelBrowserInfo" runat="server" Text=""></asp:Label>
            </div>
        </div>
        <asp:Button ID="ButtonAccess" runat="server" Text="Get HttpContext Info" OnClick="ButtonAccess_Click" class="btn btn-default btn-xs" />
        <div class="panel panel-info">
            <div class="panel-heading" data-toggle="collapse" data-target="#collapse3">
                HttpContext Infomations
            </div>
            <div class="panel-body panel-collapse collapse out" id="collapse3">
                <asp:Label ID="LabelAccess" runat="server" Text=""></asp:Label>
            </div>
        </div>

    </div>








</asp:Content>
