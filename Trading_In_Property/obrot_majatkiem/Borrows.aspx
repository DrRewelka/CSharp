<%@ Page Title="Wypożyczenia" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Borrows.aspx.cs" Inherits="obrot_majatkiem.Borrows" %>

<asp:Content ID="Content1" runat="server" contentplaceholderid="MainContent">
    <asp:Panel ID="Panel1" runat="server" HorizontalAlign="Center">
        <asp:Label ID="currentUserLabel" runat="server" Font-Bold="true" Font-Size="Large"></asp:Label>
    </asp:Panel>
    <br />

    <asp:Panel ID="Panel2" runat="server" HorizontalAlign="Left">
        <asp:Label ID="columnNameLabel" runat="server" Text="Nazwa kolumny do wyszukiwania:" Font-Bold="true" Height="25" style="margin-left: 10px"></asp:Label>
        <asp:TextBox ID="columnNameBox" runat="server" Height="20"></asp:TextBox>
        <asp:Label ID="searchValueLabel" runat="server" Text="Wartość wyszukiwana:" Font-Bold="true" Height="25" style="margin-left: 20px"></asp:Label>
        <asp:TextBox ID="searchValueBox" runat="server" Height="20"></asp:TextBox>
        <asp:Label ID="errorLabel" runat="server" Text="" Font-Bold="true" Height="25" style="margin-left: 10px" ForeColor="Red"></asp:Label>
    <br />
        <asp:CheckBox ID="createPDFCheckBox" runat="server" Text="Wygenerować potwierdzenie wypożyczenia?" TextAlign="Left" style="margin-left:10px"/>
    </asp:Panel>

    <asp:Panel ID="Panel3" runat="server" HorizontalAlign="Left">
        <asp:Button ID="borrowButton" runat="server" Text="Wypożycz zaznaczony" OnClick="borrowButton_Click" />
        <asp:Button ID="searchButton" runat="server" Text="Wyszukaj" style="margin-left: 20px" OnClick="searchButton_Click"/>
        <asp:Button ID="showAllButton" runat="server" Text="Pokaż wszystkie pozycje" style="margin-left: 20px" OnClick="showAllButton_Click"/>
    </asp:Panel>
    <br />
    <asp:Panel ID="Panel4" runat="server" HorizontalAlign="Center">
        <asp:GridView ID="borrowsGrid" runat="server" HorizontalAlign="Center">
            <Columns>
                <asp:CommandField SelectText="Zaznacz" ShowSelectButton="True" />
            </Columns>
            <SelectedRowStyle BackColor="Aqua" BorderColor="Lime" BorderWidth="1px" />
        </asp:GridView>
    </asp:Panel>
</asp:Content>