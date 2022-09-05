<%@ Page Title="Business Partner" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BusinessPartner.aspx.cs" Inherits="Web.BusinessPartner" %>



<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link rel="stylesheet" href="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" />
    <style>
        a:focus, a:hover {
            color: #23527c;
            text-decoration: none;
        }
        table .sorting {
            background-image: url("Styles/images/sort_both.png");
        }

        table .sorting_ASC {
            background-image: url("Styles/images/sort_asc.png");
        }

        table .sorting_DESC {
            background-image: url("Styles/images/sort_desc.png");
        }

        table .sorting, table .sorting_ASC, table .sorting_DESC {
            background-repeat: no-repeat;
            background-position: center right;
        }
        .pager-info {
            float: right;
            padding: 0 1.333em;
        }
    </style>
     <script>
        function checkAll(objRef) {
            var GridView = objRef.parentNode.parentNode.parentNode;
            var inputList = GridView.getElementsByTagName("input");
            for (var i = 0; i < inputList.length; i++) {
                //Get the Cell To find out ColumnIndex
                var row = inputList[i].parentNode.parentNode;
                if (inputList[i].type == "checkbox" && objRef != inputList[i]) {
                    if (objRef.checked) {
                        inputList[i].checked = true;
                    }
                    else {
                        inputList[i].checked = false;
                    }
                }
            }
        }
    </script>

    <div class="row">
        <asp:Label ID="Label1" runat="server" Text="Tax Invoice Listing" class="label label-primary"></asp:Label>
        <asp:Label ID="Label2" runat="server" Text="" class="label label-danger"></asp:Label>
        <asp:Label ID="Label3" runat="server" Text="" class="label label-info" Visible="false"></asp:Label>
        <asp:Label ID="Label4" runat="server" Text="" class="label label-info" Visible="false"></asp:Label>

    </div>


    <asp:SqlDataSource ID="SqlDataSourceInvoices" runat="server" ConnectionString="<%$ ConnectionStrings:SapConnectionString %>" SelectCommand="SELECT [CardCode], [CardName], [Currency], [Balance] FROM OCRD WHERE [CardType] ='C'" OnSelected="SqlDataSourceInvoices_Selected"></asp:SqlDataSource>


    <asp:GridView ID="GridView1" runat="server" class="table table-striped table-bordered table-hover" AllowPaging="True" EmptyDataText="No Records Found" OnRowCreated="GridView1_RowCreated" ShowFooter="False" AllowSorting="True" AutoGenerateColumns="False" DataSourceID="SqlDataSourceInvoices" SortedAscendingHeaderStyle-CssClass="sorting_ASC" SortedDescendingHeaderStyle-CssClass="sorting_DESC" OnRowCommand="GridView1_RowCommand" OnPreRender="GridView1_PreRender1">
        <Columns>
            <asp:TemplateField>
                <EditItemTemplate>
                    <asp:CheckBox ID="CheckBox1" runat="server" />
                </EditItemTemplate>
                <HeaderTemplate>
                    <asp:CheckBox ID="chkSelectAll" runat="server" onclick="checkAll(this)" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chkCheck" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField HeaderText="CardCode" DataField="CardCode" SortExpression="CardCode" />
            <asp:BoundField HeaderText="CardName" DataField="CardName" SortExpression="CardName" />
            <asp:BoundField HeaderText="Currency" DataField="Currency" SortExpression="Currency" />
            <asp:BoundField HeaderText="Balance" DataField="Balance" SortExpression="Balance" DataFormatString="{0:f}">
                <ItemStyle HorizontalAlign="Right" />
            </asp:BoundField>

        </Columns>
        <PagerSettings Mode="NumericFirstLast" Visible="true" />
        <SortedAscendingHeaderStyle CssClass="sorting_ASC"></SortedAscendingHeaderStyle>
        <SortedDescendingHeaderStyle CssClass="sorting_DESC"></SortedDescendingHeaderStyle>
    </asp:GridView>

</asp:Content>
