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

namespace obrot_majatkiem
{
    public partial class About : Page
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

                usersConn.Close();

                usersCmd.Parameters.Clear();

                if (perm == "admin")
                {
                    ShowDataGrid();
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
                    OleDbDataAdapter dAdapter = new OleDbDataAdapter("SELECT * FROM Users", usersConnParam);
                    OleDbCommandBuilder cBuilder = new OleDbCommandBuilder(dAdapter);

                    DataTable dataTable = new DataTable();

                    dAdapter.Fill(dataTable);

                    usersGrid.DataSource = dataTable;
                    usersGrid.DataBind();
                }
                catch(Exception exc)
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
                    string userID = String.Empty;

                    assetsConn.Open();
                    usersConn.Open();

                    usersCmd.Connection = usersConn;
                    usersCmd.CommandText = "UPDATE Users SET Login = @login, [Password] = @password, Permission = @permission WHERE ID = @id";
                    usersCmd.Parameters.AddWithValue("@login", LoginBox.Text);
                    usersCmd.Parameters.AddWithValue("@password", Scripts.SecurePasswordHasher.Hash(PasswordBox.Text));
                    usersCmd.Parameters.AddWithValue("@permission", PermissionList.SelectedItem.Text);
                    usersCmd.Parameters.AddWithValue("@id", IDBox.Text);

                    assetsCmd.Connection = assetsConn;
                    assetsCmd.CommandText = "UPDATE People SET Nazwa = @login WHERE ID = @id";
                    assetsCmd.Parameters.AddWithValue("@login", LoginBox.Text);
                    assetsCmd.Parameters.AddWithValue("@id", IDBox.Text);

                    usersCmd.ExecuteNonQuery();
                    assetsCmd.ExecuteNonQuery();
                    usersConn.Close();
                    assetsConn.Close();

                    usersCmd.Parameters.Clear();

                    usersConn.Open();

                    usersCmd.CommandText = "SELECT ID FROM Users WHERE Login = @login";
                    usersCmd.Parameters.AddWithValue("@login", HttpContext.Current.User.Identity.Name);

                    userID = usersCmd.ExecuteScalar().ToString();
                    usersCmd.Parameters.Clear();

                    usersConn.Close();

                    Scripts.OperationLogCreator op = new Scripts.OperationLogCreator();
                    op.OperationLog("UserID: " + userID + " || Operacja wykonana: aktualizacja danych o użytkowniku z ID: " + IDBox.Text + " w tabelach Users i People");

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

        protected void addButton_Click(object sender, EventArgs e)
        {
            if (perm == "admin")
            {
                try
                {
                    string userID = String.Empty;

                    assetsConn.Open();
                    usersConn.Open();

                    usersCmd.Connection = usersConn;
                    usersCmd.CommandText = "INSERT INTO Users (Login, [Password], Permission) VALUES (@login, @password, @permission)";
                    usersCmd.Parameters.AddWithValue("@login", LoginBox.Text);
                    usersCmd.Parameters.AddWithValue("@password", Scripts.SecurePasswordHasher.Hash(PasswordBox.Text));
                    usersCmd.Parameters.AddWithValue("@permission", PermissionList.SelectedItem.Text);

                    assetsCmd.Connection = assetsConn;
                    assetsCmd.CommandText = "INSERT INTO People (Nazwa) VALUES (@login)";
                    assetsCmd.Parameters.AddWithValue("@login", LoginBox.Text);

                    usersCmd.ExecuteNonQuery();
                    assetsCmd.ExecuteNonQuery();
                    usersConn.Close();
                    assetsConn.Close();

                    assetsCmd.Parameters.Clear();
                    usersCmd.Parameters.Clear();

                    usersConn.Open();

                    usersCmd.CommandText = "SELECT ID FROM Users WHERE Login = @login";
                    usersCmd.Parameters.AddWithValue("@login", HttpContext.Current.User.Identity.Name);

                    userID = usersCmd.ExecuteScalar().ToString();
                    usersCmd.Parameters.Clear();

                    usersConn.Close();

                    Scripts.OperationLogCreator op = new Scripts.OperationLogCreator();
                    op.OperationLog("UserID: " + userID + " || Operacja wykonana: dodanie użytkownika z ID: " + IDBox.Text + " do tabel People i Users");

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

        protected void removeButton_Click(object sender, EventArgs e)
        {
            if (perm == "admin")
            {
                try
                {
                    string userID = String.Empty;

                    assetsConn.Open();
                    usersConn.Open();

                    usersCmd.Connection = usersConn;
                    usersCmd.CommandText = "DELETE FROM Users WHERE ID = @id";
                    usersCmd.Parameters.AddWithValue("@id", IDBox.Text);

                    assetsCmd.Connection = assetsConn;
                    assetsCmd.CommandText = "DELETE FROM People WHERE ID = @id";
                    assetsCmd.Parameters.AddWithValue("@id", IDBox.Text);

                    usersCmd.ExecuteNonQuery();
                    assetsCmd.ExecuteNonQuery();
                    usersConn.Close();
                    assetsConn.Close();

                    assetsCmd.Parameters.Clear();
                    usersCmd.Parameters.Clear();

                    usersConn.Open();

                    usersCmd.CommandText = "SELECT ID FROM Users WHERE Login = @login";
                    usersCmd.Parameters.AddWithValue("@login", HttpContext.Current.User.Identity.Name);

                    userID = usersCmd.ExecuteScalar().ToString();
                    usersCmd.Parameters.Clear();

                    usersConn.Close();

                    Scripts.OperationLogCreator op = new Scripts.OperationLogCreator();
                    op.OperationLog("UserID: " + userID + " || Operacja wykonana: usunięcie użytkownika z ID: " + IDBox.Text + " z tabel Users i People");

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

        protected void refreshButton_Click(object sender, EventArgs e)
        {
            if (perm == "admin")
            {
                ShowDataGrid();
            }
        }

        private void ClearTextBoxes()
        {
            IDBox.Text = String.Empty;
            LoginBox.Text = String.Empty;
            PasswordBox.Text = String.Empty;
        }
    }
}