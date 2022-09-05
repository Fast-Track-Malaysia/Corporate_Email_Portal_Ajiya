<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GeneralLogs.aspx.cs" Inherits="Web.GeneralLogs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <h1 class="font_bold">System Logs</h1>

    <asp:SqlDataSource ID="SqlDataSource" runat="server"
        SelectCommand="SELECT Id,Screen,MessageType,Message,UserSign,TimeStamp,Remarks FROM ft_logs Order By Id DESC"
        OnSelected="SqlDataSource_Selected"
        FilterExpression="MessageType LIKE '{0}%'">
        <FilterParameters>
            <asp:ControlParameter Name="Search" ControlID="cbxSearch" PropertyName="Text" />
        </FilterParameters>
    </asp:SqlDataSource>

    <asp:DropDownList ID="cbxSearch" runat="server">
        <asp:ListItem Enabled="true" Text="Select" Value=""></asp:ListItem>
        <asp:ListItem Text="Info" Value="Info"></asp:ListItem>
        <asp:ListItem Text="Error" Value="Error"></asp:ListItem>
    </asp:DropDownList>
    <asp:Button ID="btnSearch" runat="server" Text="Refresh" />

    <asp:Label ID="LabelDefault" runat="server" Text="" class="label label-default"></asp:Label>
    <asp:Label ID="LabelPrimary" runat="server" Text="" class="label label-primary"></asp:Label>
    <asp:Label ID="LabelSuccess" runat="server" Text="" class="label label-success"></asp:Label>
    <asp:Label ID="LabelInfo" runat="server" Text="" class="label label-info"></asp:Label>
    <asp:Label ID="LabelWarning" runat="server" Text="" class="label label-warning"></asp:Label>
    <asp:Label ID="LabelDanger" runat="server" Text="" class="label label-danger"></asp:Label>

    <div class="table-responsive">
        <asp:GridView class="table table-striped table-bordered table-hover"
            ID="GridView1" runat="server"
            DataSourceID="SqlDataSource"
            DataKeyNames="Id"
            EmptyDataText="There are no data records to display."
            AutoGenerateColumns="True"
            AllowPaging="True"
            AllowSorting="True"
            OnPreRender="GridView1_PreRender"
            OnRowCreated="GridView1_RowCreated">
            <PagerSettings Mode="NumericFirstLast" Visible="true" />
            <SortedAscendingHeaderStyle CssClass="sorting_ASC"></SortedAscendingHeaderStyle>
            <SortedDescendingHeaderStyle CssClass="sorting_DESC"></SortedDescendingHeaderStyle>
        </asp:GridView>
    </div>

</asp:Content>
