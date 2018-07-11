<%@ Page Title="Zwroty" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Returns.aspx.cs" Inherits="obrot_majatkiem.Returns" %>

<asp:Content ID="Content1" runat="server" contentplaceholderid="MainContent">
    <asp:Panel ID="Panel1" runat="server" HorizontalAlign="Center">
        <asp:Label ID="currentUserLabel" runat="server" Font-Bold="true" Font-Size="Large"></asp:Label>
    </asp:Panel>
    <asp:CheckBox ID="createPDFCheckBox" runat="server" Text="Wygenerować potwierdzenie zwrotu?" TextAlign="Left" style="margin-left:10px"/>
    <br />
    <asp:Panel ID="Panel2" runat="server" HorizontalAlign="Left">
        <asp:Button ID="returnButton" runat="server" Text="Zwróć zaznaczony" OnClick="returnButton_Click" />
        <asp:Button ID="showBorrowedButton" runat="server" Text="Pokaż wypożyczone pozycje" style="margin-left: 20px" OnClick="showBorrowed_Click"/>
        <asp:Button ID="showReturnedButton" runat="server" Text="Pokaż zwrócone pozycje" style="margin-left: 20px" OnClick="showReturned_Click"/>
        <br />
        <asp:Label ID="errorLabel" runat="server" Font-Bold="true" Height="25" style="margin-left:20px" ForeColor="Red"></asp:Label>
    </asp:Panel>
    <br />
    <asp:Panel ID="Panel3" runat="server" HorizontalAlign="Center">
        <asp:GridView ID="returnsGrid" runat="server" HorizontalAlign="Center">
            <Columns>
                <asp:CommandField SelectText="Zaznacz" ShowSelectButton="True" />
            </Columns>
            <SelectedRowStyle BackColor="Aqua" BorderColor="Lime" BorderWidth="1px" />
        </asp:GridView>
    </asp:Panel>
</asp:Content>