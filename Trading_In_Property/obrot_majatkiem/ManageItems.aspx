<%@ Page Title="Zarządzanie majątkiem" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageItems.aspx.cs" Inherits="obrot_majatkiem._Default" %>

<asp:Content ID="Content1" runat="server" contentplaceholderid="MainContent">
    <asp:Panel ID="Panel1" runat="server" HorizontalAlign="Center">
        <asp:Label ID="currentUserLabel" runat="server" Font-Bold="true" Font-Size="Large"></asp:Label>
    </asp:Panel>
    <br />

    <asp:Panel ID="Panel2" runat="server" HorizontalAlign="Left">
        <asp:Label ID="IDLabel" runat="server" Text="ID:" Font-Bold="true" Height="25" style="margin-left: 10px"></asp:Label>
        <asp:TextBox ID="IDBox" runat="server" Width="40" Height="20"></asp:TextBox>
        <asp:Label ID="ItemNameLabel" runat="server" Text="Nazwa przedmiotu:" Font-Bold="true" Height="25" style="margin-left: 10px"></asp:Label>
        <asp:TextBox ID="ItemNameBox" runat="server" Height="20"></asp:TextBox>
        <br />
        <asp:Label ID="DescriptionLabel" runat="server" Text="Opis przedmiotu (kolumny rozdzielać znakiem ';'):" Font-Bold="true" Height="25" style="margin-left: 10px"></asp:Label>
        <asp:TextBox ID="DescriptionBox" runat="server" Height="20" Width="400"></asp:TextBox>
        <br />
        <asp:Label ID="Label1" runat="server" Text="Dodatkowe pola statusu:" Font-Bold="true" Height="25" style="margin-left: 10px"></asp:Label>
        <asp:CheckBox ID="cancelledCheckBox" runat="server" Text="Wycofany z wypożyczania" Height="25" TextAlign="Left" style="margin-left: 20px"/>
        <asp:CheckBox ID="removedCheckBox" runat="server" Text="Sprzęt skasowany" Height="25" TextAlign="Left" style="margin-left: 20px"/>
        <asp:Label ID="FileLabel" runat="server" Text="Wybierz plik .csv z opisem przedmiotu (poszczególne pola opisowe rozdzielone tabulatorami): " Font-Bold="true" Height="25" style="margin-left: 10px"></asp:Label>
        <asp:FileUpload ID="descriptionFileUpload" runat="server" />
    </asp:Panel>

    <asp:Panel ID="Panel3" runat="server" HorizontalAlign="Left">
        <asp:Button ID="updateButton" runat="server" Text="Aktualizuj z pola tekstowego" OnClick="updateButton_Click" style="margin-left:15px"/>
        <asp:Button ID="updateFromFileButton" runat="server" Text="Aktualizuj z pliku" OnClick="updateFromFileButton_Click" style="margin-left:15px"/>
        <asp:Button ID="addButton" runat="server" Text="Dodaj" OnClick="addButton_Click" style="margin-left:15px"/>
        <asp:Button ID="removeButton" runat="server" Text="Usuń" OnClick="removeButton_Click" style="margin-left:15px"/>
        <asp:Button ID="refreshButton" runat="server" Text="Pokaż/Odśwież bazę" OnClick="refreshButton_Click" style="margin-left:15px"/>
        <br />
        <asp:Button ID="borrowedButton" runat="server" Text="Raport z wypożyczonym sprzętem" OnClick="borrowedButton_Click" style="margin-left:15px"/>
        <asp:Button ID="availableButton" runat="server" Text="Raport z dostępnym sprzętem" OnClick="availableButton_Click" style="margin-left:15px"/>
        <br />
        <asp:Button ID="oneHistoryButton" runat="server" Text="Historia przedmiotu o podanym ID" OnClick="oneHistoryButton_Click" style="margin-left:15px"/>
        <br />
        <asp:Label ID="fromDateLabel" runat="server" Text="Data, od:" Font-Bold="true" Height="25" style="margin-left:10px"></asp:Label>
        <asp:TextBox ID="fromDateTextBox" runat="server" Height="20"></asp:TextBox>
        <asp:Label ID="toDateLabel" runat="server" Text=" do:" Font-Bold="true" Height="25" style="margin-left:20px"></asp:Label>
        <asp:TextBox ID="toDateTextBox" runat="server" Height="20"></asp:TextBox>
        <asp:Label ID="sortColumnLabel" runat="server" Text="Nazwa kolumny do sortowania:" Font-Bold="true" Height="25" style="margin-left:20px"></asp:Label>
        <asp:TextBox ID="sortColumnBox" runat="server" Height="20"></asp:TextBox>
        <br />
        <asp:Button ID="allHistoryButton" runat="server" Text="Historia wypożyczeń i zwrotów dla zakresu dat" OnClick="allHistoryButton_Click" style="margin-left:20px" />
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
        <asp:GridView ID="assetsGrid" runat="server" HorizontalAlign="Center">
        </asp:GridView>
    </asp:Panel>
</asp:Content>