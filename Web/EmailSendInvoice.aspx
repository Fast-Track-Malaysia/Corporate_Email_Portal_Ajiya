<%@ Page Title="E-Mail Invoice" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EmailSendInvoice.aspx.cs" Inherits="Web.EmailSendInvoice" MaintainScrollPositionOnPostback="true" %>

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

        .fixedWitdhS {
            overflow-wrap: break-word;
            text-align: left;
        }

        .fixedWitdhL {
            column-width: 150px;
            max-width: 150px;
            overflow-wrap: break-word;
            text-align: left;
        }
    </style>

    <h1 class="font_bold wrap">E-Mail Invoice</h1>
    <h3 class="font_medium wrap">[<asp:Label ID="LabelCompany" runat="server" Text=""></asp:Label>]
    Invoice </h3>

    <asp:SqlDataSource CancelSelectOnNullParameter="true" ID="SqlDataSource" runat="server"
        SelectCommand="SELECT * FROM [FTS_fn_GetBPList_Invoice] (@DateFromStr, @DateToStr, @DocDateFromStr, @DocDateToStr, @LastSentFromStr, @LastSentToStr, @strFr, @strTo, @strLastSent) T0 ORDER BY T0.PortalDocNo "
        OnSelected="SqlDataSource_Selected"
        FilterExpression="CardCode LIKE '%{0}%' OR CardName LIKE '%{0}%' OR EmailTo LIKE '%{0}%' OR EmailCC LIKE '%{0}%' OR CONVERT(SAPDocNo,'System.String') = '{0}' 
        OR PortalDocNo = '{0}' ">
        <FilterParameters>
            <asp:ControlParameter Name="Search" ControlID="txtSearch" PropertyName="Text" />
        </FilterParameters>
        <SelectParameters>
            <asp:Parameter Name="DateFromStr" Type="DateTime" />
            <asp:Parameter Name="DateToStr" Type="DateTime" />
            <asp:Parameter Name="DocDateFromStr" Type="DateTime" />
            <asp:Parameter Name="DocDateToStr" Type="DateTime" />
            <asp:Parameter Name="LastSentFromStr" Type="DateTime" />
            <asp:Parameter Name="LastSentToStr" Type="DateTime" />
            <asp:Parameter Name="strFr" Type="String" />
            <asp:Parameter Name="strTo" Type="String" />
            <asp:Parameter Name="strLastSent" Type="String" />
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
                        <asp:Label runat="server">Date From :</asp:Label>
                        <asp:TextBox ID="txtDateFrom" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        <ajax:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtDateFrom" DaysModeTitleFormat="dd/MM/yyyy" Format="dd/MM/yyyy" TodaysDateFormat="dd/MM/yyyy"></ajax:CalendarExtender>
                    </div>
                    <div class="col-md-2">
                        <asp:Label runat="server">Date To :</asp:Label>
                        <asp:TextBox ID="txtDateTo" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        <ajax:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="txtDateTo" DaysModeTitleFormat="dd/MM/yyyy" Format="dd/MM/yyyy" TodaysDateFormat="dd/MM/yyyy"></ajax:CalendarExtender>
                    </div>
                    <div class="col-md-2">
                        <asp:Label runat="server">Doc Date From :</asp:Label>
                        <asp:TextBox ID="txtDocDateFrom" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        <ajax:CalendarExtender ID="CalendarExtender3" runat="server" TargetControlID="txtDocDateFrom" DaysModeTitleFormat="dd/MM/yyyy" Format="dd/MM/yyyy" TodaysDateFormat="dd/MM/yyyy"></ajax:CalendarExtender>
                    </div>
                    <div class="col-md-2">
                        <asp:Label runat="server">Doc Date To :</asp:Label>
                        <asp:TextBox ID="txtDocDateTo" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        <ajax:CalendarExtender ID="CalendarExtender4" runat="server" TargetControlID="txtDocDateTo" DaysModeTitleFormat="dd/MM/yyyy" Format="dd/MM/yyyy" TodaysDateFormat="dd/MM/yyyy"></ajax:CalendarExtender>
                    </div>
                    <div class="col-md-2">
                        <asp:Label runat="server">Sent Date From :</asp:Label>
                        <asp:TextBox ID="txtLastSentFrom" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        <ajax:CalendarExtender ID="CalendarExtender5" runat="server" TargetControlID="txtLastSentFrom" DaysModeTitleFormat="dd/MM/yyyy" Format="dd/MM/yyyy" TodaysDateFormat="dd/MM/yyyy"></ajax:CalendarExtender>
                    </div>
                    <div class="col-md-2">
                        <asp:Label runat="server">Sent Date To :</asp:Label>
                        <asp:TextBox ID="txtLastSentTo" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        <ajax:CalendarExtender ID="CalendarExtender6" runat="server" TargetControlID="txtLastSentTo" DaysModeTitleFormat="dd/MM/yyyy" Format="dd/MM/yyyy" TodaysDateFormat="dd/MM/yyyy"></ajax:CalendarExtender>
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
                    <div class="col-md-4">
                        <div class="has-feedback">
                            <asp:Label runat="server">&nbsp;</asp:Label>
                            <asp:TextBox ID="txtSearch" runat="server" class="form-control input-sm" placeholder="Press Enter to Search" alt="General Search"></asp:TextBox>
                            <%-- <span class="glyphicon glyphicon-search form-control-feedback"></span>--%>
                        </div>
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
                            <asp:DropDownList ID="ddlLastSent" runat="server" CssClass="form-control input-sm" Visible="true">
                                <asp:ListItem Value="A">All Sent/Not Sent</asp:ListItem>
                                <asp:ListItem Value="Y">Sent</asp:ListItem>
                                <asp:ListItem Value="N">Not Sent</asp:ListItem>
                            </asp:DropDownList>
                        </div>
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
                    DataKeyNames="DocEntry"
                    AutoGenerateColumns="False"
                    AllowPaging="True"
                    AllowSorting="True"
                    DataSourceID="SqlDataSource"
                    OnRowDataBound="GridView1_RowDataBound"
                    OnPreRender="GridView1_PreRender"
                    OnRowCreated="GridView1_RowCreated" OnDataBound="GridView1_DataBound" RowStyle-Wrap="true">
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
                        <asp:TemplateField HeaderText="Doc Entry" SortExpression="DocEntry" ItemStyle-CssClass="fixedWitdhS" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblDocEntry" runat="server" Text='<%# Bind("DocEntry") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="SAP Doc No." SortExpression="SAPDocNo" ItemStyle-CssClass="fixedWitdhS">
                            <ItemTemplate>
                                <asp:Label ID="lblSAPDocNo" runat="server" Text='<%# Bind("SAPDocNo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Portal Oid" SortExpression="PortalOid" ItemStyle-CssClass="fixedWitdhS" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblPortalOid" runat="server" Text='<%# Bind("PortalOid") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Portal Doc No." SortExpression="PortalDocNo" ItemStyle-CssClass="fixedWitdhS">
                            <ItemTemplate>
                                <asp:Label ID="lblPortalDocNo" runat="server" Text='<%# Bind("PortalDocNo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Posting Date" SortExpression="PostingDate" ItemStyle-CssClass="fixedWitdhS">
                            <ItemTemplate>
                                <asp:Label ID="lblPostingDate" runat="server" Text='<%# Bind("PostingDate") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Document Date" SortExpression="DocDate" ItemStyle-CssClass="fixedWitdhS">
                            <ItemTemplate>
                                <asp:Label ID="lblDocDate" runat="server" Text='<%# Bind("DocDate") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CardCode" SortExpression="CardCode" ItemStyle-CssClass="fixedWitdhS">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCardCode" runat="server" Text='<%# Bind("CardCode") %>' ItemStyle-Width="20px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelCardCode" runat="server" Text='<%# Bind("CardCode") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="CardName" SortExpression="CardName" ItemStyle-CssClass="fixedWitdhL">
                            <ItemTemplate>
                                <asp:Label ID="CardName" runat="server" Text='<%# Bind("CardName") %>'></asp:Label>
                                <asp:Label ID="LabelDanger" runat="server" Text="" class="label label-danger wrap font_light"></asp:Label>
                                <asp:Label ID="LabelWarning" runat="server" Text="" class="label label-warning wrap font_light"></asp:Label>
                                <asp:Label ID="LabelInfo" runat="server" Text="" class="label label-default wrap font_light"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Email To" SortExpression="EmailTo" ItemStyle-CssClass="fixedWitdhL">
                            <ItemTemplate>
                                <asp:Label ID="lblEmailTo" runat="server" Text='<%# Bind("EmailTo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="CC" SortExpression="EmailCC" ItemStyle-CssClass="fixedWitdhL">
                            <ItemTemplate>
                                <asp:Label ID="lblEmailCC" runat="server" Text='<%# Bind("EmailCC") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Last Sent" SortExpression="LastSent" ItemStyle-CssClass="fixedWitdhS">
                            <ItemTemplate>
                                <asp:Label ID="lblLastSent" runat="server" Text='<%# Bind("LastSent") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="Currency" HeaderText="Cur" SortExpression="Currency" />

                        <asp:BoundField DataField="DocTotal" HeaderText="Doc. Total" SortExpression="DocTotal" DataFormatString="{0:#,##0.00;(#,##0.00);-}">
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>

                        <asp:TemplateField HeaderText="View" HeaderStyle-CssClass="text-center" ItemStyle-CssClass="fixedWitdhS">
                            <ItemTemplate>
                                <asp:HyperLink ID="LinkView" runat="server" CssClass="btn btn-primary btn-xs"
                                    Target="_blank"
                                    NavigateUrl='<%# string.Format("~/PreviewInvoice.aspx?DocEntry={0}&SAPDocNo={1}&PortalDocNo={2}&CardCode={3}&Mode=View", HttpUtility.UrlEncode(Eval("DocEntry").ToString()), HttpUtility.UrlEncode(Eval("SAPDocNo").ToString()), HttpUtility.UrlEncode(Eval("PortalDocNo").ToString()), HttpUtility.UrlEncode(Eval("CardCode").ToString())) %>'
                                    Text="Preview" />
                                <asp:HyperLink ID="LinkDownload" runat="server" CssClass="btn btn-primary btn-xs"
                                    NavigateUrl='<%# string.Format("~/PreviewInvoice.aspx?DocEntry={0}&SAPDocNo={1}&PortalDocNo={2}&CardCode={3}&Mode=Download", HttpUtility.UrlEncode(Eval("DocEntry").ToString()), HttpUtility.UrlEncode(Eval("SAPDocNo").ToString()), HttpUtility.UrlEncode(Eval("PortalDocNo").ToString()), HttpUtility.UrlEncode(Eval("CardCode").ToString())) %>'
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
