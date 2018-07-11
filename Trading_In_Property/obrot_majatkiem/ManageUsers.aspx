<%@ Page Title="Zarządzanie użytkownikami" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageUsers.aspx.cs" Inherits="obrot_majatkiem.UsersManagement" %>

<asp:Content ID="Content1" runat="server" contentplaceholderid="MainContent">
    <asp:Panel ID="Panel1" runat="server" HorizontalAlign="Center">
        <asp:Label ID="currentUserLabel" runat="server" Font-Bold="true" Font-Size="Large"></asp:Label>
    </asp:Panel>
    <br />

    <asp:Panel ID="Panel2" runat="server" HorizontalAlign="Left">
        <asp:Label ID="IDLabel" runat="server" Text="ID:" Font-Bold="true" Height="25" style="margin-left: 10px"></asp:Label>
        <asp:TextBox ID="IDBox" runat="server" Width="40" Height="20"></asp:TextBox>
        <asp:Label ID="UserNameLabel" runat="server" Text="Nazwa użytkownika:" Font-Bold="true" Height="25" style="margin-left: 10px"></asp:Label>
        <asp:TextBox ID="UserNameBox" runat="server" Height="20"></asp:TextBox>
        <br />
        <asp:Label ID="DescriptionLabel" runat="server" Text="Opis użytkownika (kolumny rozdzielać znakiem ';'):" Font-Bold="true" Height="25" style="margin-left: 10px"></asp:Label>
        <asp:TextBox ID="DescriptionBox" runat="server" Height="20" Width="400"></asp:TextBox>
        <br />
        <asp:Label ID="fileLabel" runat="server" Text="Wybierz plik .csv z opisem użytkownika (poszczególne pola opisowe rozdzielone tabulatorami): " Font-Bold="true" Height="25" style="margin-left:10px"></asp:Label>
        <asp:FileUpload ID="descriptionFileUpload" runat="server" />
    </asp:Panel>

    <asp:Panel ID="Panel3" runat="server" HorizontalAlign="Left">
        <asp:Button ID="updateButton" runat="server" Text="Aktualizuj z pola tekstowego" OnClick="updateButton_Click"/>
        <asp:Button ID="updateFromFileButton" runat="server" Text="Aktualizuj z pliku" OnClick="updateFromFileButton_Click" />
        <asp:Button ID="refreshButton" runat="server" Text="Pokaż/Odśwież bazę" OnClick="refreshButton_Click" />
    </asp:Panel>
    <br />
    <asp:Panel ID="Panel4" runat="server" HorizontalAlign="Left">
        <asp:Label ID="columnLabel" runat="server" Text="Nazwa pola opisowego:" Font-Bold="true" Height="25" style="margin-left: 10px"></asp:Label>
        <asp:TextBox ID="columnBox" runat="server" Height="20"></asp:TextBox>
        <asp:Button ID="addColumnButton" runat="server" Text="Dodaj pole opisowe" OnClick="addColumnButton_Click" />
        <asp:Button ID="removeColumnButton" runat="server" Text="Usuń pole opisowe" OnClick="removeColumnButton_Click" />
        <br />
        <asp:Label ID="errorLabel" runat="server" Font-Bold="true" Height="25" style="margin-left:20px" ForeColor="Red"></asp:Label>
    </asp:Panel>
    <br />
    <asp:Panel ID="Panel5" runat="server" HorizontalAlign="Center">
        <asp:GridView ID="peopleGrid" runat="server" HorizontalAlign="Center">
        </asp:GridView>
    </asp:Panel>
</asp:Content>