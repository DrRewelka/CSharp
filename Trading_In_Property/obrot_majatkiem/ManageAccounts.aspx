<%@ Page Title="Zarządzanie kontami" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageAccounts.aspx.cs" Inherits="obrot_majatkiem.About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="Panel1" runat="server" HorizontalAlign="Center">
        <asp:Label ID="currentUserLabel" runat="server" Font-Bold="true" Font-Size="Large"></asp:Label>
    </asp:Panel>
    <br />
    <asp:Label ID="IDLabel" runat="server" Text="ID:" Font-Bold="true" Font-Size="Large" Height="30" style="margin-left: 10px"></asp:Label>
    <asp:TextBox ID="IDBox" runat="server" Width="50px" Height="20px"></asp:TextBox>
    <asp:Label ID="LoginLabel" runat="server" Text="Login:" Font-Bold="true" Font-Size="Large" Height="30" style="margin-left: 20px"></asp:Label>
    <asp:TextBox ID="LoginBox" runat="server" Height="20"></asp:TextBox>
    <asp:Label ID="PasswordLabel" runat="server" Text="Hasło:" Font-Bold="true" Font-Size="Large" Height="30" style="margin-left: 20px"></asp:Label>
    <asp:TextBox ID="PasswordBox" runat="server" TextMode="Password" Height="20"></asp:TextBox>
    <asp:Label ID="PermissionLabel" runat="server" Text="Uprawnienia:" Font-Bold="true" Font-Size="Large" Height="30" style="margin-left: 20px"></asp:Label>
    <asp:DropDownList ID="PermissionList" runat="server">
        <asp:ListItem Text="admin" Value="0"></asp:ListItem>
        <asp:ListItem Text="user" Value="1"></asp:ListItem>
    </asp:DropDownList>
    <br />
    <asp:Panel ID="Panel2" runat="server" HorizontalAlign="Center">
        <asp:Button ID="updateButton" runat="server" Text="Aktualizuj" OnClick="updateButton_Click" style="margin-left:auto"/>
        <asp:Button ID="addButton" runat="server" OnClick="addButton_Click" Text="Dodaj" style="margin-left:auto" />
        <asp:Button ID="removeButton" runat="server" Text="Usuń" OnClick="removeButton_Click" />
        <asp:Button ID="refreshButton" runat="server" Text="Pokaż/Odśwież bazę" OnClick="refreshButton_Click" />
        <br /><br />
        <asp:Label ID="errorLabel" runat="server" Font-Bold="true" Height="25" style="margin-left:20px" ForeColor="Red"></asp:Label>
    </asp:Panel>
    <br />
    <asp:GridView ID="usersGrid" runat="server" AutoGenerateColumns="true" HorizontalAlign="Center" HeaderStyle-Font-Size="Large" Font-Size="Large" HeaderStyle-VerticalAlign="Middle" HeaderStyle-HorizontalAlign="Center" RowStyle-HorizontalAlign="Center" RowStyle-VerticalAlign="Middle">
        <AlternatingRowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
    </asp:GridView>
</asp:Content>