<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Web.Default" %>

<!DOCTYPE html>

<html>
<head>
    <title></title>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" />
    <link rel="shortcut icon" type="Styles/images/x-icon" href="Styles/images/SSB/favicon.ico">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.2.0/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>

    <style>
        .login {
            background: #F7F7F7;
        }

        body {
            color: #73879C;
            background: #2A3F54;
            font-family: "Helvetica Neue",Roboto,Arial,"Droid Sans",sans-serif;
            font-size: 13px;
            font-weight: 400;
            line-height: 1.471;
        }

        .login_wrapper {
            right: 0;
            margin: 2% auto 0;
            max-width: 350px;
            position: relative;
        }

        .animate {
            animation-duration: .5s;
            animation-timing-function: ease;
            animation-fill-mode: both;
        }

        .login_form {
            z-index: 22;
        }

        .login_form, .registration_form {
            position: absolute;
            top: 0;
            width: 100%;
        }

        .login_content {
            margin: 0 auto;
            padding: 5px 0 0;
            position: relative;
            text-align: center;
            text-shadow: 0 1px 0 #fff;
            min-width: 280px;
        }

            .login_content form {
                margin: 20px 0;
                position: relative;
                color: inherit;
            }

                .login_content form input[type="text"], .login_content form input[type="email"], .login_content form input[type="password"] {
                    border-radius: 3px;
                    -ms-box-shadow: 0 1px 0 #fff,0 -2px 5px rgba(0,0,0,.08) inset;
                    -o-box-shadow: 0 1px 0 #fff,0 -2px 5px rgba(0,0,0,.08) inset;
                    box-shadow: 0 1px 0 #fff,0 -2px 5px rgba(0,0,0,.08) inset;
                    border: 1px solid #c8c8c8;
                    color: #777;
                    margin: 0 0 20px;
                    width: 100%;
                }

            .login_content h1 {
                font: 400 25px Helvetica,Arial,sans-serif;
                letter-spacing: -.05em;
                line-height: 20px;
                margin: 10px 0 30px;
            }

                .login_content h1::after, .login_content h1::before {
                    content: "";
                    height: 1px;
                    position: absolute;
                    top: 10px;
                    width: 20%;
                }

                .login_content h1::before {
                    background: #7e7e7e;
                    background: linear-gradient(right,#7e7e7e 0,#fff 100%);
                    left: 0;
                }

                .login_content h1::after {
                    background: #7e7e7e;
                    background: linear-gradient(left,#7e7e7e 0,#fff 100%);
                    right: 0;
                }
    </style>
</head>
<body class="login">

    <div class="login_wrapper">

        <div class="animate form login_form">

            <div>
                <h1 class="text-center">
                    <img src="Styles/images/Ajiya/Company.png" height="187" width="250" /></h1>
            </div>

            <section class="login_content">

                <div runat="server" id="loginErrorMsgWrong" visible="false">
                    <asp:Label ID="lblMsg" runat="server" CssClass="text-danger"></asp:Label>
                </div>
                <h5>E-mail Portal</h5>

                <asp:Label ID="lblLive" runat="server" CssClass="text-success"></asp:Label>

                <form id="login" runat="server">
                    <h1>Subscriber Login </h1>
                    <fieldset id="inputs">
                        <div>
                            <asp:DropDownList ID="ddlCompany" runat="server" CssClass="form-control" AutoPostBack="true" DataTextField="CompnyCode" DataValueField="CompnyName">
                            </asp:DropDownList>
                        </div>
                        <br />
                        <div>
                            <input class="form-control" id="UserId" type="text" placeholder="User ID" autofocus required
                                runat="server" maxlength="20" />
                        </div>
                        <div>
                            <input class="form-control" id="password" type="password" placeholder="Password" required
                                runat="server" maxlength="50" />
                        </div>

                    </fieldset>

                    <fieldset id="actions">
                        <asp:Button class="btn btn-default submit" ID="submit" runat="server" Text="Log In" CommandName="Login"
                            OnClick="submit_Click" />

                    </fieldset>
                    <div class="clearfix"></div>
                    <hr style="border-color: #D8D8D8;" />



                    <div class="clearfix"></div>
                    <br>

                    <div>
                        <p>© 2017 All Rights Reserved. </p>
                    </div>

                </form>
            </section>
        </div>

    </div>

</body>
</html>
