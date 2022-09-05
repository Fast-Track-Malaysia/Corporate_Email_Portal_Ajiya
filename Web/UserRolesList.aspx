<%@ Page Title="User Roles" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserRolesList.aspx.cs" Inherits="Web.UserRolesList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table class="table">
        <tr>
            <td class="font_bold">
                <h1>Roles List</h1>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="btnNew" runat="server" class="btn btn-success btn-xs col-md-1" OnClick="btnNew_Click" Text="New" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="False" OnRowCommand="gvData_RowCommand"
                    class="table table-striped table-bordered table-hover"
                    CUseAccessibleHeader="true" PageSize="50" AllowSorting="true" AllowPaging="True">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton class="btn btn-primary btn-xs" ID="lnkSelect" runat="server" Text="Edit" CommandArgument="<%# Container.DataItemIndex %>" CommandName="Select"><i class="glyphicon glyphicon-pencil"></i></asp:LinkButton>
                                <asp:LinkButton class="btn btn-danger btn-xs" ID="lnkDelete" runat="server" Text="Delete" CommandArgument="<%# Container.DataItemIndex %>" CommandName="D" OnClientClick="return confirm('Are you sure want to delete this record?')"><i class="glyphicon glyphicon-remove icon-white"></i></asp:LinkButton>
                            </ItemTemplate>
                            <HeaderStyle Width="100px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="id" SortExpression="id" Visible="False">
                            <EditItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("ID") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblID" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Name" HeaderText="Role" SortExpression="Name" />
                        <asp:BoundField DataField="LastUpdateUser" HeaderText="Last Update User" SortExpression="lastupdateuser" ReadOnly="true" />
                        <asp:BoundField DataField="LastUpdateTime" HeaderText="Last Update Time" DataFormatString="{0:dd-MM-yyyy hh:mm:ss tt}" SortExpression="lastupdatetime" ReadOnly="true" />
                    </Columns>
                    <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />
                    <SortedAscendingHeaderStyle CssClass="sorting_ASC"></SortedAscendingHeaderStyle>
                    <SortedDescendingHeaderStyle CssClass="sorting_DESC"></SortedDescendingHeaderStyle>
                </asp:GridView>
                <br />
            </td>
        </tr>
    </table>
</asp:Content>
