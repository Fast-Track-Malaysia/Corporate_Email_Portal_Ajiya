<%@ Page Title="Receipt Template" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReceiptTemplate.aspx.cs" Inherits="Web.ReceiptTemplate" %>

<%@ MasterType VirtualPath="~/Site.Master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor" TagPrefix="ajax" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table class="table">
        <tr>
            <td class="font_bold">
                <h1>Receipt Template</h1>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="btnNew" runat="server" class="btn btn-success btn-xs col-md-1" OnClick="btnNew_Click" Text="New" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblUser" runat="server" Visible="False"></asp:Label>
                <asp:Label ID="lblCompany" runat="server" Visible="False"></asp:Label>
                <asp:Label ID="lblScreen" runat="server" Visible="False"></asp:Label>
                <asp:Label ID="lblId" runat="server" Visible="False"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" OnRowCommand="gvData_RowCommand"
                    class="table table-striped table-bordered table-hover"
                    CUseAccessibleHeader="true" PageSize="50" AllowSorting="true" AllowPaging="True">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="btnEdit" Text="Edit" CommandArgument="<%# Container.DataItemIndex %>" runat="server" CommandName="E" CssClass="btn btn-primary btn-xs btn-block"></asp:LinkButton>
                                <asp:LinkButton ID="btnDelete" CommandArgument="<%# Container.DataItemIndex %>" runat="server" CommandName="D" Text="Delete" CssClass="btn btn-danger btn-xs btn-block" OnClientClick="return confirm('Do you want to delete?')" />
                            </ItemTemplate>
                            <HeaderStyle Width="100px" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="Format" HeaderText="Format" SortExpression="Format" ItemStyle-CssClass="font_bold" ReadOnly="true" />
                        <asp:TemplateField HeaderText="id" SortExpression="id" Visible="False">
                            <EditItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("ID") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblID" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="EmailSubject" HeaderText="Email Subject" SortExpression="emailsubject" ReadOnly="true" />
                        <asp:BoundField DataField="EmailContent" HtmlEncode="False" HeaderText="Email Content" SortExpression="emailcontent" ReadOnly="true" />
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

    <asp:Button ID="btnShowPopup" runat="server" Style="display: none" />
    <ajax:ModalPopupExtender ID="pnlPopUp_ModalPopupExtender" runat="server"
        Enabled="True"
        TargetControlID="btnShowPopup"
        Drag="true"
        PopupControlID="pnlPopUp" RepositionMode="RepositionOnWindowResize"
        CancelControlID="btnCancel" BackgroundCssClass="modalBackground" DropShadow="true">
    </ajax:ModalPopupExtender>

    <asp:Panel ID="pnlPopUp" runat="server" Style="background-color: white;">
        <div class="container" style="width: 700px; padding: 10px;">
            <div class="row">

                <div class="col-md-2">
                    <asp:Label runat="server" Text="Format :" for="txtFormat"></asp:Label>
                </div>
                <div class="col-md-4">
                    <asp:TextBox ID="txtFormat" runat="server" class="form-control"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <asp:CheckBox ID="chkIsDefault" runat="server" Text="Default" />
                </div>
                <div class="col-md-4">
                    <asp:Label ID="Mode" runat="server" Visible="false" Text=""></asp:Label>
                </div>
                <div class="col-md-4">
                    <asp:Label ID="ID" runat="server" Visible="false" Text=""></asp:Label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-2">
                    <asp:Label runat="server" Text="Subject :" for="txtSubject"></asp:Label>
                </div>
                <div class="col-md-10">
                    <asp:TextBox ID="txtSubject" runat="server" class="form-control"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <asp:Label runat="server" Text="Content :" for="EmailEditor"></asp:Label>
                    <ajax:Editor ID="EmailEditor" runat="server" Height="400px" />
                </div>
            </div>
            <div class="row justify-content-between">
                <div class="col-md-4">
                    <asp:Label ID="lblError" runat="server" Text="" class="label label-danger"></asp:Label>
                </div>
                <div class="col-md-4">
                </div>
                <div class="col-md-4">
                    <div class="pull-right">
                        <asp:Button ID="btnSave" runat="server" class="btn-primary btn-xs" Text="Save" OnClick="btnSave_Click" OnClientClick="return confirm('Do you want to Update?')" />
                        <asp:Button ID="btnCancel" runat="server" class="btn-warning btn-xs" Text="Cancel" />
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>


</asp:Content>