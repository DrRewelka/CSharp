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
    public partial class UsersManagement : Page
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

                usersConn.Close();
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

            if (!this.Page.User.Identity.IsAuthenticated || perm == "user")
            {
                FormsAuthentication.RedirectToLoginPage();
            }
        }

        private void ShowDataGrid()
        {
            if (perm == "admin")
            {
                try
                {
                    OleDbDataAdapter dAdapter = new OleDbDataAdapter("SELECT * FROM People", assetsConnParam);
                    OleDbCommandBuilder cBuilder = new OleDbCommandBuilder(dAdapter);

                    DataTable dataTable = new DataTable();

                    dAdapter.Fill(dataTable);

                    peopleGrid.DataSource = dataTable;
                    peopleGrid.DataBind();
                }
                catch (Exception exc)
                {
                    Scripts.ErrorLogCreator err = new Scripts.ErrorLogCreator();
                    err.ErrorLog(exc.Message);
                    errorLabel.Text = "Błąd! " + exc.Message;
                }
            }
        }

        protected void updateButton_Click(object sender, EventArgs e)
        {
            if (perm == "admin")
            {
                try
                {
                    List<string> columnNames = new List<string>();
                    String[] values;
                    string tempAdder = String.Empty;
                    string userID = String.Empty;
                    int counter = 2;
                    int maxCounter = 0;

                    assetsConn.Open();

                    assetsCmd.Connection = assetsConn;

                    assetsCmd.CommandText = "SELECT * FROM People";

                    OleDbDataReader dr = assetsCmd.ExecuteReader();

                    for (int i = 0; i < dr.FieldCount; i++)
                        columnNames.Add(dr.GetName(i));

                    values = DescriptionBox.Text.Split(';');
                    maxCounter = columnNames.Count;

                    dr.Close();
                    assetsConn.Close();

                    for (int i = 2; i < maxCounter; i++)
                    {
                        if (i == maxCounter - 1)
                            tempAdder += columnNames[i] + " = @v" + (i - 1) + " ";
                        else
                            tempAdder += columnNames[i] + " = @v" + (i - 1) + ", ";
                    }

                    assetsConn.Open();

                    if (UserNameBox.Text == String.Empty && DescriptionBox.Text != String.Empty)
                        assetsCmd.CommandText = "UPDATE People SET " + tempAdder + "WHERE ID = @id";
                    else
                        assetsCmd.CommandText = "UPDATE People SET " + columnNames[1] + " =  @v0, " + tempAdder + "WHERE ID = @id";

                    assetsCmd.Parameters.AddWithValue("@v0", UserNameBox.Text);
                    for (int i = 1; i < (maxCounter - 1); i++)
                    {
                        string v = "@v" + i;
                        if (counter < maxCounter && values.Length == columnNames.Count - 2)
                        {
                            assetsCmd.Parameters.AddWithValue(v, values[counter - 2]);
                            counter++;
                        }
                        else if (counter < maxCounter && values.Length < columnNames.Count - 2)
                        {
                            if (counter - 2 < values.Length)
                                assetsCmd.Parameters.AddWithValue(v, values[counter - 2]);
                            else
                                assetsCmd.Parameters.AddWithValue(v, String.Empty);
                            counter++;
                        }
                        else if (counter < maxCounter && values.Length > columnNames.Count - 2)
                        {
                            if (counter - 2 < columnNames.Count - 2)
                                assetsCmd.Parameters.AddWithValue(v, values[counter - 2]);
                            counter++;
                        }
                    }
                    assetsCmd.Parameters.AddWithValue("@id", IDBox.Text);

                    assetsCmd.ExecuteNonQuery();

                    assetsConn.Close();
                    assetsCmd.Parameters.Clear();

                    usersConn.Open();

                    usersCmd.CommandText = "SELECT ID FROM Users WHERE Login = @login";
                    usersCmd.Parameters.AddWithValue("@login", HttpContext.Current.User.Identity.Name);

                    userID = usersCmd.ExecuteScalar().ToString();
                    usersCmd.Parameters.Clear();

                    usersConn.Close();

                    Scripts.OperationLogCreator op = new Scripts.OperationLogCreator();
                    op.OperationLog("UserID: " + userID + " || Operacja wykonana: aktualizacja danych o użytkowniku z ID: " + IDBox.Text + " w tabeli People");

                    ClearTextBoxes();
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

        protected void updateFromFileButton_Click(object sender, EventArgs e)
        {
            if(perm == "admin")
            {
                try
                {
                    if (descriptionFileUpload.HasFile)
                    {
                        string filePath = AppDomain.CurrentDomain.BaseDirectory + "Uploads\\" + descriptionFileUpload.FileName;
                        descriptionFileUpload.SaveAs(filePath);
                        StreamReader sr = new StreamReader(filePath, Encoding.Default, true);
                        string line = sr.ReadLine();
                        string[] values = line.Split('\t');

                        List<string> columnNames = new List<string>();
                        string tempAdder = String.Empty;
                        string userID = String.Empty;
                        int counter = 2;
                        int maxCounter = 0;

                        assetsConn.Open();

                        assetsCmd.Connection = assetsConn;

                        assetsCmd.CommandText = "SELECT * FROM People";

                        OleDbDataReader dr = assetsCmd.ExecuteReader();

                        for (int i = 0; i < dr.FieldCount; i++)
                            columnNames.Add(dr.GetName(i));

                        maxCounter = columnNames.Count;

                        dr.Close();
                        assetsConn.Close();

                        for (int i = 2; i < maxCounter; i++)
                        {
                            if (i == maxCounter - 1)
                                tempAdder += columnNames[i] + " = @v" + (i - 1) + " ";
                            else
                                tempAdder += columnNames[i] + " = @v" + (i - 1) + ", ";
                        }

                        assetsConn.Open();

                        if (UserNameBox.Text == String.Empty && DescriptionBox.Text != String.Empty)
                            assetsCmd.CommandText = "UPDATE People SET " + tempAdder + "WHERE ID = @id";
                        else
                            assetsCmd.CommandText = "UPDATE People SET " + columnNames[1] + " =  @v0, " + tempAdder + "WHERE ID = @id";

                        assetsCmd.Parameters.AddWithValue("@v0", UserNameBox.Text);
                        for (int i = 1; i < (maxCounter - 1); i++)
                        {
                            string v = "@v" + i;
                            if (counter < maxCounter && values.Length == columnNames.Count - 2)
                            {
                                assetsCmd.Parameters.AddWithValue(v, values[counter - 2]);
                                counter++;
                            }
                            else if (counter < maxCounter && values.Length < columnNames.Count - 2)
                            {
                                if (counter - 2 < values.Length)
                                    assetsCmd.Parameters.AddWithValue(v, values[counter - 2]);
                                else
                                    assetsCmd.Parameters.AddWithValue(v, String.Empty);
                                counter++;
                            }
                            else if (counter < maxCounter && values.Length > columnNames.Count - 2)
                            {
                                if (counter - 2 < columnNames.Count - 2)
                                    assetsCmd.Parameters.AddWithValue(v, values[counter - 2]);
                                counter++;
                            }
                        }
                        assetsCmd.Parameters.AddWithValue("@id", IDBox.Text);

                        assetsCmd.ExecuteNonQuery();

                        assetsConn.Close();
                        assetsCmd.Parameters.Clear();

                        usersConn.Open();

                        usersCmd.CommandText = "SELECT ID FROM Users WHERE Login = @login";
                        usersCmd.Parameters.AddWithValue("@login", HttpContext.Current.User.Identity.Name);

                        userID = usersCmd.ExecuteScalar().ToString();
                        usersCmd.Parameters.Clear();

                        usersConn.Close();

                        Scripts.OperationLogCreator op = new Scripts.OperationLogCreator();
                        op.OperationLog("UserID: " + userID + " || Operacja wykonana: aktualizacja danych o użytkowniku z ID: " + IDBox.Text + " w tabeli People");

                        ClearTextBoxes();
                    }
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
        
        protected void refreshButton_Click(object sender, EventArgs e)
        {
            if (perm == "admin")
            {
                ShowDataGrid();
                ClearTextBoxes();
            }
        }

        protected void addColumnButton_Click(object sender, EventArgs e)
        {
            if (perm == "admin")
            {
                try
                {
                    List<string> columnNames = new List<string>();
                    string tempAdder = String.Empty;
                    string userID = String.Empty;

                    assetsConn.Open();

                    assetsCmd.Connection = assetsConn;

                    assetsCmd.CommandText = "SELECT * FROM People";

                    OleDbDataReader dr = assetsCmd.ExecuteReader();

                    for (int i = 0; i < dr.FieldCount; i++)
                        columnNames.Add(dr.GetName(i));

                    dr.Close();
                    assetsConn.Close();

                    if (columnNames.Count <= 12)
                    {
                        assetsConn.Open();

                        assetsCmd.Connection = assetsConn;

                        assetsCmd.CommandText = "ALTER TABLE People ADD " + columnBox.Text + " string";
                        assetsCmd.ExecuteNonQuery();

                        assetsConn.Close();
                        assetsCmd.Parameters.Clear();
                    }

                    usersConn.Open();

                    usersCmd.CommandText = "SELECT ID FROM Users WHERE Login = @login";
                    usersCmd.Parameters.AddWithValue("@login", HttpContext.Current.User.Identity.Name);

                    userID = usersCmd.ExecuteScalar().ToString();
                    usersCmd.Parameters.Clear();

                    usersConn.Close();

                    Scripts.OperationLogCreator op = new Scripts.OperationLogCreator();
                    op.OperationLog("UserID: " + userID + " || Operacja wykonana: dodanie pola opisowego: " + columnBox.Text + " do tabeli People");

                    ClearTextBoxes();
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

        protected void removeColumnButton_Click(object sender, EventArgs e)
        {
            if (perm == "admin")
            {
                try
                {
                    string userID = String.Empty;

                    assetsConn.Open();

                    assetsCmd.Connection = assetsConn;

                    assetsCmd.CommandText = "ALTER TABLE People DROP COLUMN " + columnBox.Text;

                    assetsCmd.ExecuteNonQuery();

                    assetsConn.Close();
                    assetsCmd.Parameters.Clear();

                    usersConn.Open();

                    usersCmd.CommandText = "SELECT ID FROM Users WHERE Login = @login";
                    usersCmd.Parameters.AddWithValue("@login", HttpContext.Current.User.Identity.Name);

                    userID = usersCmd.ExecuteScalar().ToString();
                    usersCmd.Parameters.Clear();

                    usersConn.Close();

                    Scripts.OperationLogCreator op = new Scripts.OperationLogCreator();
                    op.OperationLog("UserID: " + userID + " || Operacja wykonana: usunięcie pola opisowego: " + columnBox.Text + " z tabeli People");

                    ClearTextBoxes();
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

        private void ClearTextBoxes()
        {
            IDBox.Text = String.Empty;
            UserNameBox.Text = String.Empty;
            DescriptionBox.Text = String.Empty;
            columnBox.Text = String.Empty;
        }
    }
}