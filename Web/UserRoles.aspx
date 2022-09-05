<%@ Page Title="User" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserRoles.aspx.cs" Inherits="Web.UserRoles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">

        function ShowPopup() {
            $("#btnShowPopup").click();
        }

    </script>

    <table class="table">
        <tr>
            <td colspan="4" class="font_bold">
                <h1 class="font_bold">
                    <asp:Label ID="lblHeader" runat="server"></asp:Label></h1>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:Button class="btn btn-warning btn-xs col-md-1" ID="btnUpdate" runat="server" Text="Update" OnClick="btnUpdate_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="2">Role :
            </td>
            <td colspan="2">
                <asp:TextBox ID="txtRoleName" runat="server" Width="50%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:Button ID="btnNew" runat="server" class="btn btn-success btn-xs col-md-1" OnClick="btnNew_Click" Text="New" />
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="False"
                    class="table table-striped table-bordered table-hover"
                    CUseAccessibleHeader="true" PageSize="50" AllowSorting="true" AllowPaging="True">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
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
                        <asp:TemplateField HeaderText="Menu ID" SortExpression="Menu" ItemStyle-Width="150px" Visible ="false">
                            <ItemTemplate>
                                <asp:Label ID="lblMenu" runat="server" Text='<%# Bind("Menu") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Menu" SortExpression="PageName" ItemStyle-Width="150px">
                            <ItemTemplate>
                                <asp:Label ID="lblPageName" runat="server" Text='<%# Bind("PageName") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />
                    <SortedAscendingHeaderStyle CssClass="sorting_ASC"></SortedAscendingHeaderStyle>
                    <SortedDescendingHeaderStyle CssClass="sorting_DESC"></SortedDescendingHeaderStyle>
                </asp:GridView>
                <br />
            </td>
        </tr>
    </table>
    <div class="row">
        <button type="button" style="display: none;" id="btnShowPopup" class="btn btn-primary btn-lg"
            data-toggle="modal" data-target="#myModal">
        </button>

        <div class="modal fade" id="myModal">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span></button>
                        <h5 class="modal-title">Menu</h5>
                    </div>
                    <div class="modal-body">
                        <div class="row">
                            <div class="form-group">
                                <label class="col-sm-3 control-label" for="name">Menu Item:</label>
                                <div class="col-sm-6">
                                    <asp:DropDownList ID="cmbMenuItem" runat="server" CssClass="form-control input-sm"></asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="btnAddItem" runat="server" CssClass="btn btn-default" Text="Add" OnClick="btnAddItem_Click" OnClientClick="return confirm('Are you sure want to add this menu item?')" />
                        <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>
    </div>
</asp:Content>
