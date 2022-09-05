<%@ Page Title="E-Mail Address" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EmailAddress.aspx.cs" Inherits="Web.EmailAddress" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h1 class="font_bold">Business Partner E-Mail Address</h1>

    <asp:SqlDataSource ID="SqlDataSource" runat="server"
        ConnectionString="<%$ ConnectionStrings:WebConnectionString %>" OnSelected="SqlDataSource_Selected"
        SelectCommand="SELECT id,CardCode,CardName,Name,CardType,Team,Format,[To],Cc,DBName,Dept,Link,UserSign,TimeStamp FROM FT_EMAIL_ADD WHERE DBName=@DBName "
        DeleteCommand="DELETE FROM FT_EMAIL_ADD WHERE Id=@Id"
        FilterExpression="CardCode LIKE '{0}%' OR CardName LIKE '{0}%'">
        <FilterParameters>
            <asp:ControlParameter Name="Search" ControlID="txtSearch" PropertyName="Text" />
        </FilterParameters>
        <DeleteParameters>
            <asp:ControlParameter ControlID="GridView1" Name="Id" PropertyName="SelectedDataKey" />
        </DeleteParameters>
        <SelectParameters>
            <asp:Parameter Name="DBName" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>


    <asp:UpdatePanel ID="UpdatePanel1" EnablePartialRendering="true" runat="server">
        <ContentTemplate>
            <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                <ProgressTemplate>
                    <div>
                        <asp:Image ID="processMessage" runat="server" src="Styles/images/loading/loading.gif" alt="loading" />
                        Loading...     
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>


            <span>Search Business Partner:   </span>
            <br />
            <asp:TextBox ID="txtSearch" runat="server"></asp:TextBox>
            <asp:Button ID="btnSearch" runat="server" class="btn btn-info btn-xs" Text="Search" />
            <asp:Button ID="btnNew" runat="server" class="btn btn-success btn-xs" Text="New" OnClick="btnNew_Click" />
            <asp:Button ID="btnSync" runat="server" class="btn btn-danger btn-xs" Text="Sync" OnClick="btnSync_Click" OnClientClick="return confirm('Do you want Sync SAP BP?')" />


            <asp:Label ID="lblUser" runat="server" Visible="False"></asp:Label>
            <asp:Label ID="lblScreen" runat="server" Visible="False"></asp:Label>
            <asp:Label ID="lblId" runat="server" Visible="False"></asp:Label>
            <asp:Label ID="Label1" runat="server" Text="BP Address" class="label label-primary"></asp:Label>
            <asp:Label ID="Label2" runat="server" Text="" class="label label-danger"></asp:Label>
            <asp:Label ID="Label3" runat="server" Text="" class="label label-info"></asp:Label>
            <asp:Label ID="Label4" runat="server" Text="" class="label label-success"></asp:Label>

            <asp:Button ID="btnExport" runat="server" class="btn btn-success btn-xs pull-right" Text="Excel" OnClick="btnExport_Click" PostBackUrl="~/EmailAddress.aspx" Visible="false" />


            <asp:GridView ID="GridView1" runat="server" class="table table-striped table-bordered table-hover"
                EmptyDataText="There are no data records to display."
                DataSourceID="SqlDataSource"
                DataKeyNames="Id"
                AutoGenerateColumns="False"
                AllowPaging="True"
                AllowSorting="True"
                ShowFooter="False"
                OnPreRender="GridView1_PreRender"
                OnRowCreated="GridView1_RowCreated">
                <Columns>
                    <asp:TemplateField ShowHeader="False">
                        <ItemTemplate>
                            <asp:LinkButton ID="EditButton" runat="server" CommandName="Edit" Text="Edit" CssClass="btn btn-primary btn-xs btn-block" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:LinkButton ID="btnUpdate" runat="server" CommandArgument='<%# Eval("Id") %>' OnClick="btnUpdate_Click" Text="Update" CssClass="btn btn-warning btn-xs btn-block" />
                            <asp:LinkButton ID="CancelButton" runat="server" CommandName="Cancel" Text="Cancel" CssClass="btn btn-success btn-xs btn-block" />
                            <asp:LinkButton ID="DeleteButton" runat="server" CommandName="Delete" Text="Delete" CssClass="btn btn-danger btn-xs btn-block" OnClientClick="return confirm('Do you want to delete?')" />
                        </EditItemTemplate>
                        <FooterTemplate>
                            <asp:LinkButton ID="InsertButton" runat="server" CommandName="Insert" Text="Insert" />
                        </FooterTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="Id" HeaderText="ID" SortExpression="Id" ReadOnly="true" />
                    <asp:BoundField DataField="CardCode" HeaderText="CardCode" SortExpression="CardCode" ReadOnly="true" />
                    <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ReadOnly="true" ControlStyle-Width="200px" />
                    <%--<asp:BoundField DataField="Team" HeaderText="Team" SortExpression="Team" ReadOnly="true"/>--%>
                    <asp:BoundField DataField="Format" HeaderText="Format" SortExpression="Format" ReadOnly="true" />
                    <asp:BoundField DataField="To" HeaderText="To" SortExpression="To" ReadOnly="true" />
                    <asp:BoundField DataField="Cc" HeaderText="Cc" SortExpression="Cc" ReadOnly="true" />
                    <asp:BoundField DataField="DBName" HeaderText="Company" SortExpression="DBName" ReadOnly="true" />
                    <asp:BoundField DataField="Dept" HeaderText="Dept" SortExpression="Dept" ReadOnly="true" />
                    <%-- <asp:BoundField DataField="Link" HeaderText="Link" SortExpression="Link" ReadOnly="true"/>--%>
                    <asp:BoundField DataField="UserSign" HeaderText="User" SortExpression="UserSign" ReadOnly="true" />
                    <asp:BoundField DataField="TimeStamp" HeaderText="LastUpdate" SortExpression="TimeStamp" DataFormatString="{0:dd.MM.yyyy hh:mm:sstt}" ReadOnly="true" ItemStyle-Width="180px" />

                </Columns>
                <PagerSettings Mode="NumericFirstLast" Visible="true" />
                <SortedAscendingHeaderStyle CssClass="sorting_ASC"></SortedAscendingHeaderStyle>
                <SortedDescendingHeaderStyle CssClass="sorting_DESC"></SortedDescendingHeaderStyle>
            </asp:GridView>

            <asp:Button ID="btnShowPopup" runat="server" Style="display: none" />
            <ajax:ModalPopupExtender ID="pnlPopUp_ModalPopupExtender" runat="server"
                Enabled="True"
                TargetControlID="btnShowPopup"
                Drag="true"
                PopupControlID="pnlPopUp" RepositionMode="RepositionOnWindowResize"
                CancelControlID="btnCancel" BackgroundCssClass="modalBackground" DropShadow="true">
            </ajax:ModalPopupExtender>

            <asp:Panel ID="pnlPopUp" runat="server" Style="background-color: white;">
                <div class="panel-heading">
                    <h1>E-Mail Address</h1>
                    <hr style="margin: 0px;" />
                </div>
                <div class="container" style="width: 700px; padding: 10px;">
                    <div class="row">
                        <div class="col-md-2">
                            <asp:Label runat="server" for="ddlCompany">Company :</asp:Label>
                        </div>
                        <div class="col-md-4">
                            <asp:SqlDataSource ID="sdsCompany" runat="server" ConnectionString="<%$ ConnectionStrings:WebConnectionString %>" SelectCommand="SELECT [DBName], [Company] FROM [ft_db]"></asp:SqlDataSource>
                            <asp:DropDownList class="form-control" ID="ddlCompany" runat="server" DataSourceID="sdsCompany" DataTextField="Company" DataValueField="DBName"></asp:DropDownList>
                        </div>
                        <div class="col-md-2">
                            <asp:Label runat="server" for="txtFormat">Format :</asp:Label>
                        </div>
                        <div class="col-md-4">
                            <asp:DropDownList class="form-control" ID="ddlFormat" runat="server" DataSourceID="sdsFormat" DataTextField="Format" DataValueField="Format"></asp:DropDownList>
                            <asp:SqlDataSource ID="sdsFormat" runat="server" ConnectionString="<%$ ConnectionStrings:WebConnectionString %>" SelectCommand="SELECT [Format], [Format] FROM [ft_email_template]"></asp:SqlDataSource>

                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-2">
                            CardCode:
                        </div>
                        <div class="col-md-4">
                            <asp:TextBox ID="txtCardCode" runat="server" class="form-control"></asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            Department:
                        </div>
                        <div class="col-md-4">
                            <asp:TextBox ID="txtDept" runat="server" class="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-2">
                            CardName:
                        </div>
                        <div class="col-md-10">
                            <asp:TextBox ID="txtCardName" runat="server" class="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-2">
                            To:
                        </div>
                        <div class="col-md-10">
                            <asp:TextBox ID="txtTo" runat="server" TextMode="multiline" Rows="5" class="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-2">
                            Cc:
                        </div>
                        <div class="col-md-10">
                            <asp:TextBox ID="txtCc" runat="server" TextMode="multiline" Rows="5" class="form-control"></asp:TextBox>
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
                                <asp:Button ID="btnCancel" runat="server" class="btn-warning btn-xs" Text="Cancel" OnClick="btnCancel_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>














        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="GridView1" EventName="RowDataBound" />
            <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnExport" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnSync" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>

</asp:Content>
