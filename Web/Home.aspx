<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Web.Home" %>




<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript" src="https://www.google.com/jsapi"></script>
    <style>
        .jumbotron {
            background-color: #ffffff;
            color: #000;
            padding: 0px 5px;
        }

        .container-table {
            display: table;
        }

        .vertical-center-row {
            display: table-cell;
            vertical-align: middle;
        }

        .carousel-control .glyphicon-chevron-left,
        .carousel-control .glyphicon-chevron-right,
        .carousel-control .icon-prev,
        .carousel-control .icon-next {
            position: absolute;
            top: 45%;
            z-index: 5;
            display: inline-block;
        }

        /*.home {
            padding: 60px 50px;
            height:500px;
        }*/

        .bg-grey {
            background-color: #fb7e7e;
        }

        .logo-small {
            color: #ED3024;
            font-size: 50px;
        }

        .logo {
            color: #ED3024;
            font-size: 200px;
        }

        .slideanim {
            /*visibility: hidden;*/
        }

        .slide {
            animation-name: slide;
            -webkit-animation-name: slide;
            animation-duration: 1s;
            -webkit-animation-duration: 1s;
            visibility: visible;
        }
    </style>


    <div class="jumbotron home text-center">
        <h1 class="">WELCOME</h1>
        <h2 class="font_bold">
            <asp:Label ID="LblUser" runat="server" Text="Label"></asp:Label>
        </h2>
        <asp:Label ID="lblLive" runat="server" CssClass="font_bold text-warning"></asp:Label>
        <br>
    </div>

<%--    <div class="row">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="col-lg-3 col-md-3 col-sm-12 col-xs-12">
                <div class="info-box blue-bg">
                    <i class="fa fa-envelope"></i>
                    <div class="count">
                        <asp:Label ID="labelSent" runat="server"></asp:Label>
                    </div>
                    <div class="title">Sent</div>
                </div>
            </div>
            <div class="col-lg-3 col-md-3 col-sm-12 col-xs-12">
                <div class="info-box brown-bg">
                    <i class="fa fa-user"></i>
                    <div class="count">
                        <asp:Label ID="labelCardCode" runat="server"></asp:Label>
                    </div>
                    <div class="title">BP</div>
                </div>
            </div>
            <div class="col-lg-3 col-md-3 col-sm-12 col-xs-12">
                <div class="info-box green-bg">
                    <i class="fa fa-cubes"></i>
                    <div class="count">
                        <asp:Label ID="labelCurSent" runat="server"></asp:Label>
                    </div>
                    <div class="title">Cur Month</div>
                </div>

            </div>
            <div class="col-lg-3 col-md-3 col-sm-12 col-xs-12">
                <div class="info-box dark-bg">
                    <i class="fa fa-thumbs-o-up"></i>
                    <div class="count"></div>
                    <div class="title">
                        Last Sent
                        <asp:Label ID="labelLastSent" runat="server"></asp:Label>
                    </div>
                </div>
                <!--/.info-box-->
            </div>
        </div>
    </div>--%>
    <%--        <div class="row">

        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="col-lg-3 col-md-3 col-sm-12 col-xs-12">
                <div class="info-box blue-bg">
                    <i class="fa fa-envelope"></i>
                    <div class="count">6.674</div>
                    <div class="title">Download</div>
                </div>
                <!--/.info-box-->
            </div>
            <!--/.col-->

            <div class="col-lg-3 col-md-3 col-sm-12 col-xs-12">
                <div class="info-box brown-bg">
                    <i class="fa fa-shopping-cart"></i>
                    <div class="count">7.538</div>
                    <div class="title">Purchased</div>
                </div>
                <!--/.info-box-->
            </div>
            <!--/.col-->

            <div class="col-lg-3 col-md-3 col-sm-12 col-xs-12">
                <div class="info-box dark-bg">
                    <i class="fa fa-thumbs-o-up"></i>
                    <div class="count">4.362</div>
                    <div class="title">Order</div>
                </div>
                <!--/.info-box-->
            </div>
            <!--/.col-->

            <div class="col-lg-3 col-md-3 col-sm-12 col-xs-12">
                <div class="info-box green-bg">
                    <i class="fa fa-cubes"></i>
                    <div class="count">1.426</div>
                    <div class="title">Stock</div>
                </div>
                <!--/.info-box-->
            </div>
            <!--/.col-->

        </div>
        <!--/.row-->
    </div>--%>



    <div>
        <asp:Literal ID="lt" runat="server"></asp:Literal>
        <div id="chart_div"></div>
    </div>



    <hr />

    <!-- Container (About Section) -->
    <div id="about" class="container-fluid home">
        <div class="row">
            <div class="col-sm-8">
                <div class="content-2">


                    <h1 class="title-1">Ajiya</h1>

                    <p>
                        Ajiya Berhad (“Ajiya”) and its group of companies (“the Group”) started as a metal roll forming company in 1990 using the brand name “AJIYA”. 
                        In 1996, the Group ventured into production of high value-added safety glass products. 
                        Ajiya products cater to a wide variety of users, from industrial commercial buildings to common residential houses.
                    </p>
                    <p>
                        Over 31 years since its establishment, Ajiya has expanded its geographical strategic locations from Segamat, Johor, to Northern, Southern, Central and Eastern regions of Malaysia. 
                        In response to growing competition, Ajiya had, in 2007 established its overseas presence in Thailand. 
                        To date, Ajiya has a network of 18 factories or warehouses with offices throughout Malaysia and Thailand.
                    </p>
                    <p>
                        Combining the key strengths of our Metal Division and Safety Glass Division, Ajiya proceeded to develop business to be a one-stop manufacturer of IBS (Industrialised Building System), with our very own Ajiya Green Integrated Building Solutions (“AGiBS”). 
                        AGiBS is a modern method of construction to increase productivity and quality at construction sites, align with CIDB’s initiatives.
                    </p>
                    <p>
                        Ajiya was listed on the Second Board of Bursa Malaysia Securities Berhad in 1996 and in 2003, was transferred to the Main Market.
                    </p>
                    <p>
                        Since its inception in 1990, Ajiya had committed to creating long-term shareholders’ value by delivering sustainable and profitable growth. 
                        Our corporate journey over the past years has been quite an adventure, some time smooth sailing, other time stormy. 
                        Each challenge was a lesson learnt and each success a reason for celebration.
                    </p>
                    <p>
                        <span style="line-height: 1.5;"></span><span style="line-height: 1.5;">—————————————<br>
                            <a href="https://www.ajiya.com/company-group/#ajiya-metal-group" target="_blank">Metal Group</a><br>
                            <a href="https://www.ajiya.com/company-group/#ajiya-glass-group" target="_blank">Glass Group</a>
                        </span>
                    </p>

                </div>
            </div>
<%--            <div class="col-sm-4">
                <span class="glyphicon glyphicon-signal logo"></span>
            </div>--%>
        </div>
    </div>

    <hr />

    <!-- Container (Contact Section) -->
    <div id="contact" class="container-fluid bg-grey home">
        <h2 class="text-center">CONTACT</h2>
        <div class="row">
            <div class="col-sm-5 clearfix align_left" style="background-color: #fb7e7e; padding: 5px 5px 5px 5px; opacity: 0.7;">
                <h3 style="color: #ffffff;">Fast Track SBOI Sdn Bhd<span style="font-size: 11px; color: #ffffff;">(776340)</span></h3>
                <b style="color: #ffffff;">Business Address:</b><br />
                <p style="color: #ffffff;">
                    <span class="glyphicon glyphicon-map-marker"></span>
                    SetiaWalk, Unit H-05-01,<br />
                    Block H, Persiaran Wawasan,<br />
                    Pusat Bandar Puchong,<br />
                    47160 Puchong, Selangor<br />
                    <br />

                </p>
                <p style="color: #ffffff;">
                    <span class="glyphicon glyphicon-phone">+603-78800871</span>
                    <br />
                    <span class="glyphicon glyphicon-phone">+603-78800873</span>
                    <br />
                    <span class="glyphicon glyphicon-envelope">hong@myfastrack.net</span>
                </p>
            </div>
            <div class="col-sm-7 slideanim">
<%--                <div class="row">
                    <div class="col-sm-6 form-group">
                        <input class="form-control" id="name" name="name" placeholder="Name" type="text" required>
                    </div>
                    <div class="col-sm-6 form-group">
                        <input class="form-control" id="email" name="email" placeholder="Email" type="email" required>
                    </div>
                </div>
                <textarea class="form-control" id="comments" name="comments" placeholder="Comment" rows="5"></textarea><br>
                <div class="row">
                    <div class="col-sm-12 form-group">
                        <button class="btn btn-default pull-right" type="submit">Send</button>
                    </div>
                </div>--%>
            </div>
        </div>
    </div>

    <footer class="container-fluid text-center home">
        <a href="#myPage" title="To Top">
            <span class="glyphicon glyphicon-chevron-up"></span>
        </a>
        <p>Email Portal Made By <a href="#" title="">Fast Track SBOi Sdn. Bhd.</a></p>
    </footer>






</asp:Content>


