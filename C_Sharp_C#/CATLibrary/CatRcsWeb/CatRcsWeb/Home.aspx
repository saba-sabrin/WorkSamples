<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="CatRcsWeb.Home" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>:: CAT Tools ::</title>
</head>
<body style="background-color:#008282" >
    <form id="frmMain" runat="server" style="background-color: #B5CECE; height: 150px; border:double;">
        <div style="margin: auto; padding: inherit; background-position: center center; background-color: #E7F5F0; border: medium double #336600; width: 250px; position: relative; z-index: inherit; left: 0px; right: 0px; top: 1px; vertical-align: middle; text-align: center; font-family: Arial, Helvetica, sans-serif; height: 100px;">
            <div>
            <asp:Label ID="lblTitle" runat="server" Text="Select CAT Tools" Font-Bold="True" Font-Size="Large" Font-Names="Tahoma" ForeColor="DarkGreen" BackColor="WhiteSmoke" BorderStyle="Groove" BorderColor="Silver"></asp:Label>
            </div>
            <br />
            <div>
                <asp:LinkButton ID="btnTheta" runat="server" Text="Theta Estimator" Font-Bold="True" Font-Size="Medium" PostBackUrl="~/ThetaWeb.aspx" ForeColor="SeaGreen">Theta Estimator</asp:LinkButton>
            </div>
            <br />
            <div>
                <asp:LinkButton ID="btnLP" runat="server" Text="Theta Estimator" Font-Bold="True" Font-Size="Medium" PostBackUrl="~/LPWeb.aspx" ForeColor="SeaGreen">LP Solver</asp:LinkButton>
            </div>
        </div>
    </form>
</body>
</html>
