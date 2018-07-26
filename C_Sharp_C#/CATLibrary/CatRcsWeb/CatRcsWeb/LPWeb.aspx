<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LPWeb.aspx.cs" Inherits="CatRcsWeb.LPWeb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>LP Solver</title>
</head>
<body style="background-color: #CAEEFF">
    <form id="frmMain" runat="server" style="border: thin groove #62DFFF; padding: 200px; margin: auto; background-position: center center; background-color: #F2F2F2; text-align: center; vertical-align: middle; line-height: normal; height: 250px; width: 500px; text-indent: inherit; position: fixed; z-index: inherit; top: 200px; left: 200px; bottom: 250px; right: 200px;" aria-orientation="horizontal">
        <div>
            <asp:Label ID="lblTitle" runat="server" Text="LP Solution Generator" Font-Bold="True" Font-Names="Bell MT" Font-Size="XX-Large" ForeColor="#006699"></asp:Label>
        </div>
        <br />
        <div>
            <asp:Panel ID="Panel1" runat="server" HorizontalAlign="Left" BackColor="White" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" Font-Names="Lucida Bright" Font-Size="Medium">
                <div id="main" runat="server" style="padding: 20px">
                    <asp:CheckBox ID="chkGlpk" runat="server" Text="GLPK" />
                    <br />
                    <asp:CheckBox ID="chkLP" runat="server" Text="LP-Solver" />
                </div>
            </asp:Panel>
            <br />
            <asp:Label ID="lblTest" Text="" runat="server" ForeColor="Red"></asp:Label>
            <br />
            <div style="padding-right: 10px">
                <asp:Button ID="btnProcess" Text="Process" runat="server" BackColor="#0077B0" Font-Bold="True" Font-Names="Calibri" ForeColor="#E8FAFF" Font-Size="X-Large" Height="34px" Width="230px" BorderStyle="None" ToolTip="Click to process.." OnClick="btnProcess_Click"></asp:Button>
                <br />
                <div style="align-items:baseline;" id="divDownloadFiles" runat="server" visible="false">
                    <br />
                    <asp:LinkButton ID="btnGLPK" runat="server" Text="Result of GLPK solution" ForeColor="DarkBlue" Visible="false" OnClick="btnGLPK_Click"></asp:LinkButton>
                    <br />
                    <asp:LinkButton ID="btnLP" runat="server" Text="Result of LP-Solve solution" ForeColor="DarkBlue" Visible="false" OnClick="btnLP_Click"></asp:LinkButton>
                </div>
                <br />
                <asp:LinkButton ID="btnBack" runat="server" Text="Back to Homepage" ForeColor="DarkMagenta" PostBackUrl="~/Home.aspx"></asp:LinkButton>
            </div>
        </div>
    </form>
</body>
</html>
