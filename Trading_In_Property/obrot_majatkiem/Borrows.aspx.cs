using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace obrot_majatkiem
{
    public partial class Borrows : System.Web.UI.Page
    {
        private OleDbConnection assetsConn;
        private OleDbCommand assetsCmd = new OleDbCommand();

        private OleDbConnection usersConn;
        private OleDbCommand usersCmd = new OleDbCommand();

        private string assetsConnParam = ConfigurationManager.ConnectionStrings["assetsConnection"].ConnectionString;
        private string usersConnParam = ConfigurationManager.ConnectionStrings["usersConnection"].ConnectionString;

        private string perm;

        protected void Page_Load(object sender, EventArgs e)
        {
            assetsConn = new OleDbConnection(assetsConnParam);
            usersConn = new OleDbConnection(usersConnParam);

            try
            {
                usersConn.Open();

                usersCmd.Connection = usersConn;
                usersCmd.CommandText = "SELECT Permission FROM Users WHERE Login = @login";
                usersCmd.Parameters.AddWithValue("@login", HttpContext.Current.User.Identity.Name);

                currentUserLabel.Text = "Zalogowany jako: " + HttpContext.Current.User.Identity.Name;

                perm = (string)usersCmd.ExecuteScalar();

                usersCmd.Parameters.Clear();
            }
            catch (Exception exc)
            {
                Scripts.ErrorLogCreator err = new Scripts.ErrorLogCreator();
                err.ErrorLog(exc.Message);
                errorLabel.Text = "Błąd! " + exc.Message;
            }
            finally
            {
                usersConn.Close();
            }

            if (!this.Page.User.Identity.IsAuthenticated || perm == "admin")
            {
                FormsAuthentication.RedirectToLoginPage();
            }
        }

        private void ShowDataGrid()
        {
            if (perm == "user")
            {
                try
                {
                    List<string> columnNames = new List<string>();
                    string tempAdder = String.Empty;

                    assetsConn.Open();

                    assetsCmd.Connection = assetsConn;

                    assetsCmd.CommandText = "SELECT * FROM Assets";

                    OleDbDataReader dr = assetsCmd.ExecuteReader();

                    for (int i = 0; i < dr.FieldCount; i++)
                        columnNames.Add(dr.GetName(i));

                    dr.Close();
                    assetsConn.Close();

                    for (int i = 0; i < columnNames.Count; i++)
                    {
                        if (i != 2 && i != 3 && i < columnNames.Count - 1)
                            tempAdder += columnNames[i] + ", ";
                        else if (i != 2 && i != 3 && i == columnNames.Count - 1)
                            tempAdder += columnNames[i];
                    }

                    OleDbDataAdapter dAdapter = new OleDbDataAdapter("SELECT " + tempAdder + " FROM Assets WHERE Wycofany = 'Nie' AND Skasowany = 'Nie' AND Wypożyczony = 'Nie'", assetsConnParam);
                    OleDbCommandBuilder cBuilder = new OleDbCommandBuilder(dAdapter);

                    DataTable dataTable = new DataTable();

                    dAdapter.Fill(dataTable);

                    borrowsGrid.DataSource = dataTable;
                    borrowsGrid.DataBind();
                }
                catch (Exception exc)
                {
                    Scripts.ErrorLogCreator err = new Scripts.ErrorLogCreator();
                    err.ErrorLog(exc.Message);
                    errorLabel.Text = "Błąd! " + exc.Message;
                }
            }
        }

        protected void borrowButton_Click(object sender, EventArgs e)
        {
            if (perm == "user")
            {
                try
                {
                    List<string> columnNames = new List<string>();
                    List<string> values = new List<string>();
                    string itemID = String.Empty;
                    string userID = String.Empty;
                    string date = String.Empty;
                    string itemColumns = String.Empty;
                    string userColumns = String.Empty;
                    string itemValues = String.Empty;
                    string userValues = String.Empty;
                    string addColumns = String.Empty;
                    DateTime dateNow;

                    assetsConn.Open();

                    assetsCmd.Connection = assetsConn;

                    assetsCmd.CommandText = "SELECT * FROM Assets";

                    OleDbDataReader dr = assetsCmd.ExecuteReader();

                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        if (i > 4)
                            itemColumns += dr.GetName(i) + ", ";
                    }
                    dr.Close();

                    assetsCmd.CommandText = "SELECT * FROM People";

                    dr = assetsCmd.ExecuteReader();

                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        if (i > 1 && i != dr.FieldCount - 1)
                            userColumns += dr.GetName(i) + ", ";
                        if (i > 1 && i == dr.FieldCount - 1)
                            userColumns += dr.GetName(i);
                    }
                    dr.Close();

                    itemID = borrowsGrid.SelectedRow.Cells[1].Text;

                    assetsCmd.CommandText = "SELECT ID FROM People WHERE Nazwa = @userName";
                    assetsCmd.Parameters.AddWithValue("@userName", HttpContext.Current.User.Identity.Name);

                    userID = assetsCmd.ExecuteScalar().ToString();
                    date = System.DateTime.Now.ToString();
                    System.Globalization.DateTimeFormatInfo dateInfo = new System.Globalization.DateTimeFormatInfo();
                    dateInfo.FullDateTimePattern = "dd/MM/yyyy HH:mm:ss tt";
                    dateNow = Convert.ToDateTime(date, dateInfo);

                    assetsCmd.Parameters.Clear();

                    itemColumns = itemColumns.Remove(itemColumns.LastIndexOf(','));
                    assetsCmd.CommandText = "SELECT " + itemColumns + " FROM Assets WHERE ID = " + itemID;

                    dr = assetsCmd.ExecuteReader();
                    dr.Read();
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        itemValues += "'" + dr.GetString(i) + "'" + ", ";
                    }
                    dr.Close();

                    assetsCmd.CommandText = "SELECT " + userColumns + " FROM People WHERE ID = " + userID;
                    dr = assetsCmd.ExecuteReader();
                    dr.Read();
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        if (i < dr.FieldCount - 1)
                            userValues += "'" + dr.GetString(i) + "'" + ", ";
                        if (i == dr.FieldCount - 1)
                            userValues += "'" + dr.GetString(i) + "'";
                    }
                    dr.Close();

                    itemColumns += ", ";

                    assetsCmd.CommandText = "SELECT * FROM Assets";
                    dr = assetsCmd.ExecuteReader();

                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        if (i > 3)
                            addColumns += dr.GetName(i) + " string" + ", ";
                    }
                    dr.Close();

                    assetsCmd.CommandText = "SELECT * FROM People";
                    dr = assetsCmd.ExecuteReader();

                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        if (i > 1 && i < dr.FieldCount - 1)
                            addColumns += dr.GetName(i) + " string" + ", ";
                        if (i == dr.FieldCount - 1)
                            addColumns += dr.GetName(i) + " string";
                    }
                    dr.Close();

                    if (columnNames.Count == 7)
                    {
                        assetsCmd.CommandText = "ALTER TABLE Borrows ADD " + addColumns;
                        assetsCmd.ExecuteNonQuery();
                    }

                    assetsCmd.CommandText = "INSERT INTO Borrows (ID_przedmiotu, ID_użytkownika, Wypożyczony, Data_wypożyczenia, " + itemColumns + userColumns + ") VALUES (@itemID, @userID, 'Tak', @dateNow, " + itemValues + userValues + ")";
                    assetsCmd.Parameters.AddWithValue("@itemID", itemID);
                    assetsCmd.Parameters.AddWithValue("@userID", userID);
                    assetsCmd.Parameters.AddWithValue("@dateNow", dateNow);
                    assetsCmd.ExecuteNonQuery();
                    assetsCmd.Parameters.Clear();

                    assetsCmd.CommandText = "UPDATE Assets SET Wypożyczony = 'Tak' WHERE ID = @itemID";
                    assetsCmd.Parameters.AddWithValue("@itemID", itemID);
                    assetsCmd.ExecuteNonQuery();
                    assetsCmd.Parameters.Clear();
                    assetsConn.Close();

                    if (createPDFCheckBox.Checked)
                    {
                        assetsConn.Open();
                        assetsCmd.CommandText = "SELECT * FROM Borrows WHERE ID = (SELECT MAX(ID) FROM Borrows)";

                        dr = assetsCmd.ExecuteReader();
                        for (int i = 0; i < dr.FieldCount; i++)
                            columnNames.Add(dr.GetName(i));

                        dr.Read();

                        for (int i = 0; i < dr.FieldCount; i++)
                            values.Add(dr.GetValue(i).ToString());

                        assetsConn.Close();

                        var doc = new Document();
                        PdfWriter.GetInstance(doc, new FileStream(AppDomain.CurrentDomain.BaseDirectory + "PDF\\Wypożyczenia\\" + System.DateTime.Now.ToString("yyyy_MM_dd-HH_mm") + ".pdf", FileMode.Create));

                        doc.Open();

                        Font titleFont = FontFactory.GetFont(BaseFont.TIMES_ROMAN, BaseFont.CP1257, 14f, Font.BOLD);
                        Font dataFont = FontFactory.GetFont(BaseFont.TIMES_ROMAN, BaseFont.CP1257, 12f);
                        Phrase title = new Phrase("Potwierdzenie wypożyczenia", titleFont);
                        Paragraph titlePar = new Paragraph(title);
                        Paragraph dataPar = new Paragraph();

                        for (int i = 0; i < columnNames.Count; i++)
                        {
                            Phrase p = new Phrase(columnNames[i] + ": " + values[i] + "\n", dataFont);
                            dataPar.Add(p);
                        }
                        titlePar.SetAlignment("Center");
                        doc.Add(titlePar);
                        doc.Add(dataPar);
                        doc.Close();

                        Scripts.OperationLogCreator op1 = new Scripts.OperationLogCreator();
                        op1.OperationLog("UserID: " + userID + " || Operacja wykonana: wygenerowanie wydruku potwierdzającego wypożyczenie przedmiotu o ID: " + itemID);
                    }
                    Scripts.OperationLogCreator op = new Scripts.OperationLogCreator();
                    op.OperationLog("UserID: " + userID + " || Operacja wykonana: wypożyczenie przedmiotu o ID: " + itemID);

                    ClearTextBoxes();
                }
                catch(Exception exc)
                {
                    Scripts.ErrorLogCreator err = new Scripts.ErrorLogCreator();
                    err.ErrorLog(exc.Message);
                    errorLabel.Text = "Błąd! " + exc.Message;
                }
                finally
                {
                    assetsConn.Close();
                    usersConn.Close();
                }
            }
        }

        protected void searchButton_Click(object sender, EventArgs e)
        {
            if (perm == "user")
            {
                try
                {
                    if (searchValueBox.Text.Length < 3 && !searchValueBox.Text.Contains('*'))
                    {
                        errorLabel.ForeColor = System.Drawing.Color.Red;
                        errorLabel.Text = "Za mało znaków wyszukiwania!";
                    }
                    else if (searchValueBox.Text.Length >= 3 || searchValueBox.Text.Contains('*'))
                    {
                        List<string> columnNames = new List<string>();
                        string tempAdder = String.Empty;
                        string searchValue = String.Empty;

                        searchValue = searchValueBox.Text.Replace('*', '%');
                        searchValue = searchValue.Replace('?', '_');

                        assetsConn.Open();

                        assetsCmd.Connection = assetsConn;

                        assetsCmd.CommandText = "SELECT * FROM Assets";

                        OleDbDataReader dr = assetsCmd.ExecuteReader();

                        for (int i = 0; i < dr.FieldCount; i++)
                            columnNames.Add(dr.GetName(i));

                        dr.Close();
                        assetsConn.Close();

                        for (int i = 0; i < columnNames.Count; i++)
                        {
                            if (i != 2 && i != 3 && i < columnNames.Count - 1)
                                tempAdder += columnNames[i] + ", ";
                            else if (i != 2 && i != 3 && i == columnNames.Count - 1)
                                tempAdder += columnNames[i];
                        }

                        assetsCmd.Connection = assetsConn;

                        OleDbDataAdapter dAdapter = new OleDbDataAdapter("SELECT " + tempAdder + " FROM Assets WHERE " + columnNameBox.Text + " LIKE '" + searchValue + "' AND Wycofany = 'Nie' AND Skasowany = 'Nie' ORDER BY ID", assetsConnParam);
                        OleDbCommandBuilder cBuilder = new OleDbCommandBuilder(dAdapter);

                        DataTable dataTable = new DataTable();

                        dAdapter.Fill(dataTable);

                        borrowsGrid.DataSource = dataTable;
                        borrowsGrid.DataBind();

                        ClearTextBoxes();
                    }
                }
                catch (Exception exc)
                {
                    Scripts.ErrorLogCreator err = new Scripts.ErrorLogCreator();
                    err.ErrorLog(exc.Message);
                    errorLabel.Text = "Błąd! " + exc.Message;
                }
                finally
                {
                    assetsConn.Close();
                    usersConn.Close();
                }
            }
        }

        protected void showAllButton_Click(object sender, EventArgs e)
        {
            if (perm == "user")
            {
                ShowDataGrid();
                ClearTextBoxes();
            }
        }

        private void ClearTextBoxes()
        {
            columnNameBox.Text = String.Empty;
            searchValueBox.Text = String.Empty;
        }
    }
}