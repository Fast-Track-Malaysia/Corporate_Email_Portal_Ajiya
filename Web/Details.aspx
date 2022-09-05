<%@ Page Title="My Details" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="Web.Details" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table class="table">
        <tr>
            <td colspan="4" class="font_bold">
                <h1 class="font_bold">My Details</h1>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:Button ID="btnUpdate" runat="server" class="btn btn-warning btn-xs col-md-1" Text="Update" OnClick="btnUpdate_Click" />
            </td>
        </tr>
        <tr>
            <td width="150px">User Name :</td>
            <td width="200px">
                <asp:Label ID="lblUserID" runat="server"></asp:Label>
            </td>
            <td width="100px">Password :</td>
            <td>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" Width="100%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>Name :            </td>
            <td>
                <asp:TextBox ID="txtName" runat="server" Width="100%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>Role :</td>
            <td colspan="3">
                <asp:DropDownList ID="ddlRole" runat="server" Height="25px" Width="100%"
                    Visible="False">
                </asp:DropDownList>
                <asp:Label ID="lblRole" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>Email :</td>
            <td colspan="3">
                <asp:TextBox ID="txtEmail" runat="server" Width="100%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="4">&nbsp;</td>
        </tr>
    </table>
</asp:Content>
