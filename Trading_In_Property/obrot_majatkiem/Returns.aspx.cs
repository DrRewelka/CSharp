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
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace obrot_majatkiem
{
    public partial class Returns : System.Web.UI.Page
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
                assetsConn.Close();
                usersConn.Close();
            }

            if (!this.Page.User.Identity.IsAuthenticated || perm == "admin")
            {
                FormsAuthentication.RedirectToLoginPage();
            }
        }

        private void ShowDataGrid(string itemStatus)
        {
            if (perm == "user")
            {
                try
                {
                    string userID = String.Empty;
                    
                    assetsConn.Open();

                    assetsCmd.Connection = assetsConn;

                    assetsCmd.CommandText = "SELECT ID FROM People WHERE Nazwa = @userName";
                    assetsCmd.Parameters.AddWithValue("@userName", HttpContext.Current.User.Identity.Name);

                    userID = assetsCmd.ExecuteScalar().ToString();

                    assetsConn.Close();
                    assetsCmd.Parameters.Clear();

                    OleDbDataAdapter dAdapter = new OleDbDataAdapter("SELECT * FROM Borrows WHERE ID_Użytkownika = " + userID + " AND " + itemStatus + " = 'Tak'", assetsConnParam);
                    OleDbCommandBuilder cBuilder = new OleDbCommandBuilder(dAdapter);

                    DataTable dataTable = new DataTable();

                    dAdapter.Fill(dataTable);

                    returnsGrid.DataSource = dataTable;
                    returnsGrid.DataBind();
                }
                catch(Exception exc)
                {
                    Scripts.ErrorLogCreator err = new Scripts.ErrorLogCreator();
                    err.ErrorLog(exc.Message);
                    errorLabel.Text = "Błąd! " + exc.Message;
                }
            }
        }

        protected void returnButton_Click(object sender, EventArgs e)
        {
            if (perm == "user")
            {
                try
                {
                    List<string> columnNames = new List<string>();
                    List<string> values = new List<string>();
                    string userID = String.Empty;
                    string itemID = String.Empty;
                    string date = String.Empty;
                    string borrowDateString = String.Empty;
                    DateTime dateNow, borrowDate;

                    itemID = returnsGrid.SelectedRow.Cells[2].Text;
                    borrowDateString = returnsGrid.SelectedRow.Cells[6].Text;
                    date = System.DateTime.Now.ToString();
                    System.Globalization.DateTimeFormatInfo dateInfo = new System.Globalization.DateTimeFormatInfo();
                    dateInfo.FullDateTimePattern = "dd/MM/yyyy HH:mm:ss tt";
                    dateNow = Convert.ToDateTime(date, dateInfo);
                    borrowDate = Convert.ToDateTime(borrowDateString, dateInfo);

                    assetsConn.Open();

                    assetsCmd.Connection = assetsConn;

                    assetsCmd.CommandText = "SELECT ID FROM People WHERE Nazwa = @userName";
                    assetsCmd.Parameters.AddWithValue("@userName", HttpContext.Current.User.Identity.Name);

                    userID = assetsCmd.ExecuteScalar().ToString();
                    assetsCmd.Parameters.Clear();

                    assetsCmd.CommandText = "UPDATE Borrows SET Wypożyczony = 'Nie', Zwrócony = 'Tak', Data_zwrócenia = @dateNow WHERE ID_przedmiotu = @itemID AND ID_użytkownika = @userID AND Data_wypożyczenia = @borrowDate";
                    assetsCmd.Parameters.AddWithValue("@dateNow", dateNow);
                    assetsCmd.Parameters.AddWithValue("@itemID", itemID);
                    assetsCmd.Parameters.AddWithValue("@userID", userID);
                    assetsCmd.Parameters.AddWithValue("@borrowDate", borrowDate);

                    assetsCmd.ExecuteNonQuery();
                    assetsCmd.Parameters.Clear();

                    assetsCmd.CommandText = "UPDATE Assets SET Wypożyczony = 'Nie' WHERE ID = @itemID";
                    assetsCmd.Parameters.AddWithValue("@itemID", itemID);
                    assetsCmd.ExecuteNonQuery();
                    assetsCmd.Parameters.Clear();

                    assetsConn.Close();

                    if (createPDFCheckBox.Checked)
                    {
                        assetsConn.Open();
                        assetsCmd.CommandText = "SELECT * FROM Borrows WHERE ID = " + itemID;

                        OleDbDataReader dr = assetsCmd.ExecuteReader();
                        for (int i = 0; i < dr.FieldCount; i++)
                            columnNames.Add(dr.GetName(i));

                        dr.Read();

                        for (int i = 0; i < dr.FieldCount; i++)
                            values.Add(dr.GetValue(i).ToString());

                        assetsConn.Close();

                        var doc = new Document();
                        PdfWriter.GetInstance(doc, new FileStream(AppDomain.CurrentDomain.BaseDirectory + "PDF\\Zwroty\\" + System.DateTime.Now.ToString("yyyy_MM_dd-HH_mm") + ".pdf", FileMode.Create));

                        doc.Open();

                        Font titleFont = FontFactory.GetFont(BaseFont.TIMES_ROMAN, BaseFont.CP1257, 14f, Font.BOLD);
                        Font dataFont = FontFactory.GetFont(BaseFont.TIMES_ROMAN, BaseFont.CP1257, 12f);
                        Phrase title = new Phrase("Potwierdzenie zwrotu", titleFont);
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
                        op1.OperationLog("UserID: " + userID + " || Operacja wykonana: wygenerowanie wydruku potwierdzającego zwrot przedmiotu o ID: " + itemID);
                    }
                    Scripts.OperationLogCreator op = new Scripts.OperationLogCreator();
                    op.OperationLog("UserID: " + userID + " || Operacja wykonana: zwrot przedmiotu o ID: " + itemID);
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

        protected void showBorrowed_Click(object sender, EventArgs e)
        {
            if(perm == "user")
                ShowDataGrid("Wypożyczony");
        }

        protected void showReturned_Click(object sender, EventArgs e)
        {
            if (perm == "user")
                ShowDataGrid("Zwrócony");
        }
    }
}