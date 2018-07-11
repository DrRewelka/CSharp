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
    public partial class Contact : Page
    {
        private OleDbConnection usersConn;
        private OleDbCommand usersCmd = new OleDbCommand();

        private string usersConnParam = ConfigurationManager.ConnectionStrings["usersConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            usersConn = new OleDbConnection(usersConnParam);
            if (this.Page.User.Identity.IsAuthenticated)
                loggedInLabel.Text = "Zalogowany jako: " + HttpContext.Current.User.Identity.Name;
            else
                loggedInLabel.Text = "Zalogowany jako: gość";
        }

        protected void loginButton_Click(object sender, EventArgs e)
        {
            try
            {
                string userID = String.Empty;

                usersConn.Open();
                usersCmd.Connection = usersConn;

                usersCmd.CommandText = "SELECT [Password] FROM Users WHERE Login = @login";
                usersCmd.Parameters.AddWithValue("@login", loginBox.Text);

                string hashedPass = (string)usersCmd.ExecuteScalar();

                usersCmd.Parameters.Clear();

                usersCmd.CommandText = "SELECT ID FROM Users WHERE Login = @login";
                usersCmd.Parameters.AddWithValue("@login", loginBox.Text);

                userID = usersCmd.ExecuteScalar().ToString();
                usersCmd.Parameters.Clear();

                usersConn.Close();

                if (!Scripts.SecurePasswordHasher.Verify(passwordBox.Text, hashedPass))
                {
                    error.ForeColor = System.Drawing.Color.Red;
                    error.Text = "Niewłaściwe dane logowania!";
                }
                else
                {
                    FormsAuthentication.RedirectFromLoginPage(loginBox.Text, false);
                    loggedInLabel.Text = "Zalogowany jako: " + HttpContext.Current.User.Identity.Name;
                }

                Scripts.OperationLogCreator op = new Scripts.OperationLogCreator();
                op.OperationLog("UserID: " + userID + " || Operacja wykonana: zalogowanie.");
            }
            catch(Exception exc)
            {
                Scripts.ErrorLogCreator err = new Scripts.ErrorLogCreator();
                err.ErrorLog(exc.Message);
                error.ForeColor = System.Drawing.Color.Red;
                error.Text = exc.Message;
            }
            finally
            {
                usersConn.Close();
            }
        }

        protected void logoutButton_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
            HttpContext.Current.User = null;
        }
    }
}