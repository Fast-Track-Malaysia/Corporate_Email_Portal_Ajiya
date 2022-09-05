<%@ Page Title="User" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="User.aspx.cs" Inherits="Web.User" %>



<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table class="table">
        <tr>
            <td colspan="4" class="font_bold">
                <h1 class="font_bold"><asp:Label ID="lblHeader" runat="server" ></asp:Label></h1>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:Button class="btn btn-warning btn-xs col-md-1" ID="btnUpdate" runat="server" Text="Update" OnClick="btnUpdate_Click" />
            </td>
        </tr>
        <tr>
            <td width="150px">User ID :
            </td>
            <td width="200px">
                <asp:TextBox ID="txtUserID" runat="server" Width="100%"></asp:TextBox>
            </td>
            <td width="100px">Password :
            </td>
            <td>
                <asp:Label ID="lblPassword" runat="server" Visible="False"></asp:Label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" Width="100%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>Name :
            </td>
            <td>
                <asp:TextBox ID="txtName" runat="server" Width="100%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>Role :
            </td>
            <td>
                <asp:DropDownList ID="ddlRole" runat="server" Height="25px" Width="100%">
                </asp:DropDownList>
            </td>
            <td></td>
            <td>
                <asp:CheckBox ID="chkIsActive" runat="server" Text="Is Active" />
            </td>
        </tr>
        <tr>
            <td>Email :
            </td>
            <td colspan="3">
                <asp:TextBox ID="txtEmail" runat="server" Width="100%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="4">&nbsp;</td>
        </tr>
    </table>
</asp:Content>
