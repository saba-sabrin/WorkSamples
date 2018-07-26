<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ThetaWeb.aspx.cs" Inherits="CatRcsWeb.ThetaWeb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Theta Estimation</title>
    <script src="Scripts/jquery-3.1.0.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="Scripts/jspdf.debug.js"></script>
    <script type="text/javascript" src="Scripts/jspdf.js"></script>
    <script type="text/javascript" src="Scripts/jspdf.min.js"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            //alert('Page has been loaded');
            BindButtonClickEvent();
        });

        function BindButtonClickEvent() {
            $("#<%= btnSave.ClientID %>").click(function () {

                var content = '<%=Session["Theta_Value"]%>';

                //$("#<%= lblTest.ClientID %>").text("");

                var doc = new jsPDF();
                doc.setLineWidth(0.5);
                doc.setFont("helvetica");
                doc.setFontType("bold");
                doc.text(20, 20, 'Theta Estimation');

                // Draw line
                doc.line(20, 30, 200, 30); // left, top, edge (length), bottom

                doc.setFont("times");
                doc.setFontType("normal");
                doc.text(20, 40, 'The estimated value of theta is, ' + content + ' .');
                doc.save('Theta_Result.pdf');

                return false;
            });
        }
    </script>
</head>
<body style="background-color: #CAEEFF">
    <form id="frmMain" runat="server" style="border: thin groove #62DFFF; padding: 200px; margin: auto; background-position: center center; background-color: #F2F2F2; text-align: center; vertical-align: middle; line-height: normal; height: 250px; width: 500px; text-indent: inherit; position: fixed; z-index: inherit; top: 200px; left: 200px; bottom: 250px; right: 200px;" aria-orientation="horizontal">
        <div>
            <asp:Label ID="lblTitle" runat="server" Text="Theta Estimation" Font-Bold="True" Font-Names="Bell MT" Font-Size="XX-Large" ForeColor="#006699"></asp:Label>
        </div>
        <br />
        <div>
            <asp:Panel ID="Panel1" runat="server" HorizontalAlign="Left" BackColor="White" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" Font-Names="Lucida Bright" Font-Size="Medium">
                <div id="main" runat="server" style="padding: 20px">
                    <asp:Label ID="lblItems" runat="server" Text="Item IDs" Width="180px"></asp:Label>
                    <asp:TextBox ID="txtItemID" runat="server" Width="200px" ToolTip="Enter Item IDs seperated with comma or space"></asp:TextBox>
                    <br />
                    <br />
                    <asp:Label ID="lblResponse" runat="server" Text="Response categories" Width="180px"></asp:Label>
                    <asp:TextBox ID="txtResponse" runat="server" Width="200px" ToolTip="Enter Responses seperated with comma or space"></asp:TextBox>
                    <br />
                    <br />
                    <asp:CheckBox ID="chkDicho" runat="server" Text="Apply Dichotomous Model" AutoPostBack="True" OnCheckedChanged="chkDicho_CheckedChanged" />
                    <br />
                    <asp:CheckBox ID="chkPoly" runat="server" Text="Apply Polytomous Model" AutoPostBack="True" OnCheckedChanged="chkPoly_CheckedChanged" />
                </div>
            </asp:Panel>
            <br />
            <asp:Label ID="lblTest" Text="" runat="server" ForeColor="#cc0000"></asp:Label>
            <br />
            <div style="padding-right: 10px">
                <asp:Button ID="btnSubmit" Text="Calculate Theta" runat="server" BackColor="#0077B0" Font-Bold="True" Font-Names="Calibri" ForeColor="#E8FAFF" Font-Size="X-Large" Height="34px" Width="230px" BorderStyle="None" OnClick="btnSubmit_Click" ToolTip="Click to Submit values"></asp:Button>
                <asp:Button ID="btnSave" OnClientClick="return false;" Text="Export as PDF" runat="server" BackColor="#BF3000" Font-Bold="True" Font-Names="Calibri" ForeColor="#FFF4FF" Font-Size="X-Large" Height="34px" Width="210px" BorderStyle="None" ToolTip="Export result as PDF"></asp:Button>
                <br />
                <asp:LinkButton ID="btnBack" runat="server" Text="Back to Homepage" ForeColor="DarkMagenta" PostBackUrl="~/Home.aspx"></asp:LinkButton>
            </div>
        </div>
    </form>
</body>
</html>
