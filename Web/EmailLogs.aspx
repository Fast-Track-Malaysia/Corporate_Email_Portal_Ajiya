<%@ Page Title="Sent Logs" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EmailLogs.aspx.cs" Inherits="Web.EmailLogs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h1 class="font_bold">E-Mail Logs</h1>

    <asp:SqlDataSource ID="SqlDataSource" runat="server"
        ConnectionString="<%$ ConnectionStrings:WebConnectionString %>"
        SelectCommand="SELECT Id,CreateDate [TimeStamp], CardCode,CardName,Format,MailFr,MailTo,MailCC,Attachement as [Attachment],Subject,CreatedBy,Screen,Statementdate FROM ft_email_info WHERE Source=@DBName order by Id desc"
        OnSelected="SqlDataSource_Selected"
        FilterExpression="CardCode LIKE '{0}%' OR CardName LIKE '{0}%'">
        <FilterParameters>
            <asp:ControlParameter Name="Search" ControlID="txtSearch" PropertyName="Text" />
        </FilterParameters>
        <SelectParameters>
            <asp:Parameter Name="DBName" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>

    <asp:TextBox ID="txtSearch" runat="server"></asp:TextBox>
    <asp:Button ID="btnSearch" runat="server" Text="Search" />
    <asp:Button ID="btnExport" runat="server" class="btn btn-success btn-xs pull-right" Text="Excel" OnClick="btnExport_Click" PostBackUrl="~/EmailLogs.aspx" Visible="true" />
    <%--<asp:Button ID="btnExportPDF" runat="server" class="btn btn-danger btn-xs pull-right" Text="PDF" OnClick="btnExportPDF_Click" PostBackUrl="~/EmailLogs.aspx" Visible="true" />--%>


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
            AutoGenerateColumns="True" RowStyle-Wrap="false"
            AllowPaging="True"
            AllowSorting="True"
            ShowFooter="False"
            OnPreRender="GridView1_PreRender"
            OnRowCreated="GridView1_RowCreated">
            <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" Visible="true" />
            <SortedAscendingHeaderStyle CssClass="sorting_ASC"></SortedAscendingHeaderStyle>
            <SortedDescendingHeaderStyle CssClass="sorting_DESC"></SortedDescendingHeaderStyle>
        </asp:GridView>
    </div>



</asp:Content>
