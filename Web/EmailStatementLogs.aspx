<%@ Page Title="Statement Sent Logs" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EmailStatementLogs.aspx.cs" Inherits="Web.EmailStatementLogs" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .loading {
            position: fixed;
            top: 50%;
            left: 50%;
            padding: 0px;
            width: 5%;
            height: 5%;
            z-index: 1001;
        }
    </style>

    <h1 class="font_bold wrap">Statement Sent Logs</h1>
    <h3 class="font_medium wrap">[<asp:Label ID="LabelCompany" runat="server" Text=""></asp:Label>]
    Statement of Account 
        <asp:Label ID="LabelStmtDate" runat="server" Text=""></asp:Label>
    </h3>

    <asp:SqlDataSource CancelSelectOnNullParameter="true" ID="SqlDataSource" runat="server"
        SelectCommand="SELECT * FROM [FTS_fn_StatementLogs] (@senddatefrStr, @senddatetoStr, @strFr, @strTo, @sendResult) T0 ORDER BY T0.SendDate DESC, T0.CardCode "
        OnSelected="SqlDataSource_Selected"
        FilterExpression="CardCode LIKE '%{0}%' OR CardName LIKE '%{0}%' OR EmailTo LIKE '%{0}%' OR EmailCC LIKE '%{0}%' ">
        <FilterParameters>
            <asp:ControlParameter Name="Search" ControlID="txtSearch" PropertyName="Text" Type="String" />
        </FilterParameters>
        <SelectParameters>
            <asp:Parameter Name="senddatefrStr" Type="DateTime" />
            <asp:Parameter Name="senddatetoStr" Type="DateTime" />
            <asp:Parameter Name="strFr" Type="String" />
            <asp:Parameter Name="strTo" Type="String" />
            <asp:Parameter Name="sendResult" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>

    <asp:Timer runat="server" ID="Timer1" Interval="500" Enabled="false" OnTick="Timer1_Tick" />


    <asp:UpdatePanel ID="UpdatePanel1" EnablePartialRendering="true" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
        <ContentTemplate>
            <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                <ProgressTemplate>
                    <div class="loading">
                        <asp:Image ID="processMessage" runat="server" ImageUrl="Styles/images/loading/loading.gif" alt="loading" />
                        Loading...                            
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <div class="loading">
                <asp:Image ID="imgLoading" runat="server" ImageUrl="Styles/images/loading/loading.gif" alt="loading" Visible="false" />
            </div>
            <asp:Button ID="btnSearch" runat="server" CssClass="submit btn-xs btn-default" Text="Search" OnClick="btnSearch_Click" Width="120px" />
            <asp:Button ID="btnSend" runat="server" Text="Send Selected" CssClass="submit btn-xs btn-default" Style="position: relative; top: 0px; left: 0px; width: 160px;" OnClick="btnSend_Click" OnClientClick="return confirm('Are you sure want to send email for the record(s) selected?')" Visible="true" />
            <asp:Button ID="btnRefresh" runat="server" CssClass="submit btn-xs btn-default" Text="Refresh" OnClick="btnRefresh_Click" Width="120px" Visible="false" />


            <%--            <asp:Button ID="btnExport" runat="server" class="btn btn-success btn-xs pull-right" Text="Excel" OnClick="btnExport_Click" PostBackUrl="~/EmailSend.aspx" Visible="true" />
            <asp:Button ID="btnExportZIP" runat="server" class="btn btn-warning btn-xs pull-right" Text="ZIP" OnClick="btnExportZIP_Click" PostBackUrl="~/EmailSend.aspx" Visible="true" />
            <asp:Button ID="btnExportPDF" runat="server" class="btn btn-danger btn-xs pull-right" Text="PDF" OnClick="btnExportPDF_Click" PostBackUrl="~/EmailSend.aspx" Visible="true" />
            <asp:Button ID="btnSelect" runat="server" class="btn btn-success btn-xs pull-right" Text="Select Current" OnClick="btnSelect_Click" PostBackUrl="~/EmailSend.aspx" Visible="true" />--%>

            <hr class="wrap" />
            <div id="searchPanel">
                <div class="row">
                    <div class="col-md-2">
                        <asp:Label runat="server">Send Date From :</asp:Label>
                        <asp:TextBox ID="txtSendDateFr" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        <ajax:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtSendDateFr" DaysModeTitleFormat="dd/MM/yyyy" Format="dd/MM/yyyy" TodaysDateFormat="dd/MM/yyyy"></ajax:CalendarExtender>
                    </div>
                    <div class="col-md-2">
                        <asp:Label runat="server">Send Date To :</asp:Label>
                        <asp:TextBox ID="txtSendDateTo" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        <ajax:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="txtSendDateTo" DaysModeTitleFormat="dd/MM/yyyy" Format="dd/MM/yyyy" TodaysDateFormat="dd/MM/yyyy"></ajax:CalendarExtender>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <asp:Label runat="server">&nbsp;</asp:Label>
                        <asp:TextBox ID="txtCardCodeFr" runat="server" CssClass="form-control input-sm" placeholder="Customer Code From"></asp:TextBox>
                    </div>
                    <div class="col-md-2">
                        <asp:Label runat="server">&nbsp;</asp:Label>
                        <asp:TextBox ID="txtCardCodeTo" runat="server" CssClass="form-control input-sm" placeholder="Customer Code To"></asp:TextBox>
                    </div>
                    <div class="col-md-2">
                        <asp:Label runat="server">&nbsp;</asp:Label>
                        <asp:DropDownList ID="ddlSendResult" runat="server" CssClass="form-control input-sm" Visible="true">
                            <asp:ListItem Selected="True" Value="*">All</asp:ListItem>
                            <asp:ListItem Value="Success">Success</asp:ListItem>
                            <asp:ListItem Value="Failed">Failed</asp:ListItem>
                        </asp:DropDownList>
                        <asp:DropDownList ID="ddlCurrency" runat="server" CssClass="form-control input-sm" Visible="false">
                            <asp:ListItem Value="L">Local Currency</asp:ListItem>
                            <asp:ListItem Selected="True" Value="C">BP Currency</asp:ListItem>
                        </asp:DropDownList>
                        <asp:DropDownList ID="ddlPreviousTrans" runat="server" CssClass="form-control input-sm" Visible="false">
                            <asp:ListItem Selected="True" Value="Y">Yes</asp:ListItem>
                            <asp:ListItem Value="N">No</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-2">
                        <div class="has-feedback">
                            <asp:Label runat="server">&nbsp;</asp:Label>
                            <asp:TextBox ID="txtSearch" runat="server" class="form-control input-sm" placeholder="Press Enter to Search" alt="General Search"></asp:TextBox>
                            <%-- <span class="glyphicon glyphicon-search form-control-feedback"></span>--%>
                        </div>
                    </div>
                </div>
            </div>
            <hr class="wrap" />
            <asp:Label ID="LabelDefault" runat="server" Text="" class="label label-default"></asp:Label>
            <asp:Label ID="LabelPrimary" runat="server" Text="" class="label label-primary"></asp:Label>
            <asp:Label ID="LabelSuccess" runat="server" Text="" class="label label-success"></asp:Label>
            <asp:Label ID="LabelInfo" runat="server" Text="" class="label label-info"></asp:Label>
            <asp:Label ID="LabelWarning" runat="server" Text="" class="label label-warning"></asp:Label>
            <asp:Label ID="LabelDanger" runat="server" Text="" class="label label-danger"></asp:Label>
            <asp:Label ID="LabelLoading" runat="server" Text="" class="label label-success"></asp:Label>
            <asp:Label ID="lblUser" runat="server" Visible="False"></asp:Label>
            <asp:Label ID="lblScreen" runat="server" Visible="False"></asp:Label>
            <asp:Label ID="lbl_msg" Text="" runat="server"></asp:Label>

            <div class="table-responsive">
                <asp:GridView ID="GridView1" runat="server" class="table table-striped table-bordered table-hover"
                    DataKeyNames="ID"
                    AutoGenerateColumns="False"
                    AllowPaging="True"
                    AllowSorting="True"
                    DataSourceID="SqlDataSource"
                    OnRowDataBound="GridView1_RowDataBound"
                    OnPreRender="GridView1_PreRender"
                    OnRowCreated="GridView1_RowCreated" OnDataBound="GridView1_DataBound">
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkAll" runat="server" AutoPostBack="true" OnCheckedChanged="chkAll_CheckedChanged" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox
                                    ID="chkBox" runat="server" OnCheckedChanged="chkBox_CheckedChanged" AutoPostBack="false"
                                    CommandArgument='<%# ((GridViewRow) Container).RowIndex %>'
                                    CommandName="chkboxCheck" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ID" SortExpression="ID" ItemStyle-Width="150px" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblID" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CardCode" SortExpression="CardCode">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("CardCode") %>' ItemStyle-Width="20px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelCardCode" runat="server" Text='<%# Bind("CardCode") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="CardName" SortExpression="CardName" ItemStyle-Width="400px">
                            <ItemTemplate>
                                <asp:Label ID="CardName" runat="server" Text='<%# Bind("CardName") %>'></asp:Label>
                                <asp:Label ID="Label1" runat="server" Text="" class="label label-danger wrap font_light"></asp:Label>
                                <asp:Label ID="Label2" runat="server" Text="" class="label label-warning wrap font_light"></asp:Label>
                                <asp:Label ID="Label3" runat="server" Text="" class="label label-default wrap font_light"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Email To" SortExpression="EmailTo" ItemStyle-Width="150px">
                            <ItemTemplate>
                                <asp:Label ID="lblEmailTo" runat="server" Text='<%# Bind("EmailTo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CC" SortExpression="EmailCC" ItemStyle-Width="150px">
                            <ItemTemplate>
                                <asp:Label ID="lblEmailCC" runat="server" Text='<%# Bind("EmailCC") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Subject" SortExpression="EmailSubject" ItemStyle-Width="150px" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblEmailSubject" runat="server" Text='<%# Bind("EmailSubject") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Content" SortExpression="EmailContent" ItemStyle-Width="150px" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblEmailContent" runat="server" Text='<%# Bind("EmailContent") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Statement Date" SortExpression="StatementDate" ItemStyle-Width="150px">
                            <ItemTemplate>
                                <asp:Label ID="lblStatementDate" runat="server" Text='<%# Bind("StatementDate") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Send Date" SortExpression="SendDate" ItemStyle-Width="210px">
                            <ItemTemplate>
                                <asp:Label ID="lblSendDate" runat="server" Text='<%# Bind("SendDate") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Send By" SortExpression="SendBy" ItemStyle-Width="100px">
                            <ItemTemplate>
                                <asp:Label ID="lblSendBy" runat="server" Text='<%# Bind("SendBy") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Result" SortExpression="SendResult" ItemStyle-Width="100px">
                            <ItemTemplate>
                                <asp:Label ID="lblSendResult" runat="server" Text='<%# Bind("SendResult") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Error" SortExpression="ErrorDesc" ItemStyle-Width="400px">
                            <ItemTemplate>
                                <asp:Label ID="lblErrorDesc" runat="server" Text='<%# Bind("ErrorDesc") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="View" HeaderStyle-CssClass="text-center">
                            <ItemTemplate>
                                <asp:HyperLink ID="LinkView" runat="server" CssClass="btn btn-primary btn-xs"
                                    Target="_blank"
                                    NavigateUrl='<%# string.Format("~/Preview.aspx?CardCode={0}&StmtDate={1}&Currency={2}&Mode=View",HttpUtility.UrlEncode(Eval("CardCode").ToString()), HttpUtility.UrlEncode(Eval("StatementDate").ToString()), ddlCurrency.SelectedValue) %>'
                                    Text="Preview" />
                                <asp:HyperLink ID="LinkDownload" runat="server" CssClass="btn btn-primary btn-xs"
                                    NavigateUrl='<%# string.Format("~/Preview.aspx?CardCode={0}&StmtDate={1}&Currency={2}&Mode=Download",HttpUtility.UrlEncode(Eval("CardCode").ToString()), HttpUtility.UrlEncode(Eval("StatementDate").ToString()), ddlCurrency.SelectedValue) %>'
                                    Text="Download" />
                            </ItemTemplate>
                            <HeaderStyle CssClass="text-center"></HeaderStyle>
                        </asp:TemplateField>
                    </Columns>
                    <RowStyle CssClass="wrap" />
                    <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" Visible="true" />

                    <SortedAscendingHeaderStyle CssClass="sorting_ASC"></SortedAscendingHeaderStyle>
                    <SortedDescendingHeaderStyle CssClass="sorting_DESC"></SortedDescendingHeaderStyle>

                </asp:GridView>

            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="GridView1" EventName="DataBound" />
            <asp:AsyncPostBackTrigger ControlID="GridView1" EventName="RowDataBound" />
            <asp:AsyncPostBackTrigger ControlID="Timer1" EventName="Tick" />
            <asp:AsyncPostBackTrigger ControlID="btnRefresh" EventName="Click" />

            <%--            <asp:AsyncPostBackTrigger ControlID="btnExport" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnExportPDF" EventName="Click" />--%>
            <asp:AsyncPostBackTrigger ControlID="btnSend" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
