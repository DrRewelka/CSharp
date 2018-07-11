<%@ Page Title="Logowanie" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="obrot_majatkiem.Contact" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="Panel1" runat="server" HorizontalAlign="Center">
        <asp:Label ID="loggedInLabel" runat="server" Font-Bold="true"></asp:Label>
    </asp:Panel>
    <br />
    <asp:Panel ID="Panel2" runat="server" HorizontalAlign="Left">
        <asp:Label ID="login" runat="server" Text="Login:" Height="26px" style="margin-left:10px"></asp:Label>
        <asp:TextBox ID="loginBox" runat="server" Height="24px"></asp:TextBox>
        <br />
        <asp:Label ID="password" runat="server" Text="Hasło:" Height="26px" style="margin-left:10px"></asp:Label>
        <asp:TextBox ID="passwordBox" runat="server" Height="24px" TextMode="Password"></asp:TextBox>
        <br />
        <asp:Label ID="error" runat="server" ForeColor="Red" style="margin-left:10px"></asp:Label>
    </asp:Panel>
    <br />
    <asp:Panel ID="Panel3" runat="server" HorizontalAlign="Left">
        <asp:Button ID="loginButton" runat="server" Text="Zaloguj" OnClick="loginButton_Click" style="margin-left:10px"/>
        <asp:Button ID="logoutButton" runat="server" Text="Wyloguj" OnClick="logoutButton_Click" style="margin-left:10px"/>
    </asp:Panel>
</asp:Content>