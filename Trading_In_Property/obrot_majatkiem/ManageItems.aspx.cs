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
    public partial class _Default : Page
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
                        if (i != 2 && i != 3 && i != 4)
                            tempAdder += columnNames[i] + ", ";
                    }
                    tempAdder += columnNames[2] + ", " + columnNames[3] + ", " + columnNames[4];

                    OleDbDataAdapter dAdapter = new OleDbDataAdapter("SELECT " + tempAdder + " FROM Assets", assetsConnParam);
                    OleDbCommandBuilder cBuilder = new OleDbCommandBuilder(dAdapter);

                    DataTable dataTable = new DataTable();

                    dAdapter.Fill(dataTable);

                    assetsGrid.DataSource = dataTable;
                    assetsGrid.DataBind();
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
                    string userID = String.Empty;
                    string tempAdder = String.Empty;
                    string cancelled = String.Empty;
                    string removed = String.Empty;
                    int counter = 5;
                    int maxCounter = 0;

                    assetsConn.Open();

                    assetsCmd.Connection = assetsConn;

                    assetsCmd.CommandText = "SELECT * FROM Assets";

                    OleDbDataReader dr = assetsCmd.ExecuteReader();

                    for (int i = 0; i < dr.FieldCount; i++)
                        columnNames.Add(dr.GetName(i));

                    values = DescriptionBox.Text.Split(';');
                    maxCounter = columnNames.Count;

                    dr.Close();
                    assetsConn.Close();

                    for (int i = 5; i < maxCounter; i++)
                    {
                        tempAdder += columnNames[i] + " = @v" + (i - 1) + ", ";
                    }

                    if (cancelledCheckBox.Checked)
                        cancelled = "Tak";
                    else
                        cancelled = "Nie";

                    if (removedCheckBox.Checked)
                        removed = "Tak";
                    else
                        removed = "Nie";

                    assetsConn.Open();

                    if (ItemNameBox.Text == String.Empty && DescriptionBox.Text != String.Empty)
                        assetsCmd.CommandText = "UPDATE Assets SET " + tempAdder + "Wycofany = @cancelled, Skasowany = @removed WHERE ID = @id";
                    else

                        assetsCmd.CommandText = "UPDATE Assets SET " + columnNames[1] + " =  @v0, " + tempAdder + "Wycofany = @cancelled, Skasowany = @removed WHERE ID = @id";

                    assetsCmd.Parameters.AddWithValue("@v0", ItemNameBox.Text);
                    for (int i = 1; i < (maxCounter - 1); i++)
                    {
                        string v = "@v" + i;
                        if (counter < maxCounter && values.Length == columnNames.Count - 5)
                        {
                            assetsCmd.Parameters.AddWithValue(v, values[counter - 5]);
                            counter++;
                        }
                        else if (counter < maxCounter && values.Length < columnNames.Count - 5)
                        {
                            if (counter - 5 < values.Length)
                                assetsCmd.Parameters.AddWithValue(v, values[counter - 5]);
                            else
                                assetsCmd.Parameters.AddWithValue(v, String.Empty);
                            counter++;
                        }
                        else if (counter < maxCounter && values.Length > columnNames.Count - 5)
                        {
                            if (counter - 5 < columnNames.Count - 5)
                                assetsCmd.Parameters.AddWithValue(v, values[counter - 5]);
                            counter++;
                        }
                    }

                    assetsCmd.Parameters.AddWithValue("@cancelled", cancelled);
                    assetsCmd.Parameters.AddWithValue("@removed", removed);
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
                    op.OperationLog("UserID: " + userID + " || Operacja wykonana: aktualizacja danych o przedmiocie z ID: " + IDBox.Text + " w tabeli Assets");

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
            if (perm == "admin")
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
                        string userID = String.Empty;
                        string tempAdder = String.Empty;
                        string cancelled = String.Empty;
                        string removed = String.Empty;
                        int counter = 5;
                        int maxCounter = 0;

                        assetsConn.Open();

                        assetsCmd.Connection = assetsConn;

                        assetsCmd.CommandText = "SELECT * FROM Assets";

                        OleDbDataReader dr = assetsCmd.ExecuteReader();

                        for (int i = 0; i < dr.FieldCount; i++)
                            columnNames.Add(dr.GetName(i));

                        maxCounter = columnNames.Count;

                        dr.Close();
                        assetsConn.Close();

                        for (int i = 5; i < maxCounter; i++)
                        {
                            tempAdder += columnNames[i] + " = @v" + (i - 1) + ", ";
                        }

                        if (cancelledCheckBox.Checked)
                            cancelled = "Tak";
                        else
                            cancelled = "Nie";

                        if (removedCheckBox.Checked)
                            removed = "Tak";
                        else
                            removed = "Nie";

                        assetsConn.Open();

                        if (ItemNameBox.Text == String.Empty && DescriptionBox.Text != String.Empty)
                            assetsCmd.CommandText = "UPDATE Assets SET " + tempAdder + "Wycofany = @cancelled, Skasowany = @removed WHERE ID = @id";
                        else
                            assetsCmd.CommandText = "UPDATE Assets SET " + columnNames[1] + " =  @v0, " + tempAdder + "Wycofany = @cancelled, Skasowany = @removed WHERE ID = @id";

                        assetsCmd.Parameters.AddWithValue("@v0", ItemNameBox.Text);
                        for (int i = 1; i < (maxCounter - 1); i++)
                        {
                            string v = "@v" + i;
                            if (counter < maxCounter && values.Length == columnNames.Count - 5)
                            {
                                assetsCmd.Parameters.AddWithValue(v, values[counter - 5]);
                                counter++;
                            }
                            else if (counter < maxCounter && values.Length < columnNames.Count - 5)
                            {
                                if (counter - 5 < values.Length)
                                    assetsCmd.Parameters.AddWithValue(v, values[counter - 5]);
                                else
                                    assetsCmd.Parameters.AddWithValue(v, String.Empty);
                                counter++;
                            }
                            else if (counter < maxCounter && values.Length > columnNames.Count - 5)
                            {
                                if (counter - 5 < columnNames.Count - 5)
                                    assetsCmd.Parameters.AddWithValue(v, values[counter - 5]);
                                counter++;
                            }
                        }

                        assetsCmd.Parameters.AddWithValue("@cancelled", cancelled);
                        assetsCmd.Parameters.AddWithValue("@removed", removed);
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
                        op.OperationLog("UserID: " + userID + " || Operacja wykonana: aktualizacja danych o przedmiocie z ID: " + IDBox.Text + " w tabeli Assets");

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

        protected void addButton_Click(object sender, EventArgs e)
        {
            if(perm == "admin")
            {
                try
                {
                    List<string> columnNames = new List<string>();
                    String[] values;
                    string userID = String.Empty;
                    string tempAdderC = String.Empty;
                    string tempAdderV = String.Empty;
                    string cancelled = String.Empty;
                    string removed = String.Empty;
                    int counter = 5;
                    int maxCounter = 0;

                    assetsConn.Open();

                    assetsCmd.Connection = assetsConn;

                    assetsCmd.CommandText = "SELECT * FROM Assets";

                    OleDbDataReader dr = assetsCmd.ExecuteReader();

                    for (int i = 0; i < dr.FieldCount; i++)
                        columnNames.Add(dr.GetName(i));

                    values = DescriptionBox.Text.Split(';');
                    maxCounter = columnNames.Count;

                    dr.Close();
                    assetsConn.Close();

                    if (columnNames.Count <= 19)
                    {
                        for (int i = 5; i < maxCounter; i++)
                        {
                            tempAdderC += columnNames[i] + ", ";
                            tempAdderV += "@v" + (i - 3) + ", ";
                        }

                        if (cancelledCheckBox.Checked)
                            cancelled = "Tak";
                        else
                            cancelled = "Nie";

                        if (removedCheckBox.Checked)
                            removed = "Tak";
                        else
                            removed = "Nie";

                        assetsConn.Open();

                        if (DescriptionBox.Text == String.Empty)
                        {
                            assetsCmd.CommandText = "INSERT INTO Assets (Nazwa, Wycofany, Skasowany) VALUES (@itemName, @cancelled, @removed)";
                            assetsCmd.Parameters.AddWithValue("@itemName", ItemNameBox.Text);
                        }
                        else
                        {
                            assetsCmd.CommandText = "INSERT INTO Assets (" + columnNames[1] + ", " + tempAdderC + "Wycofany, Skasowany) VALUES (@itemName, " + tempAdderV + "@cancelled, @removed)";
                            assetsCmd.Parameters.AddWithValue("@itemName", ItemNameBox.Text);

                            for (int i = 1; i < (maxCounter - 1); i++)
                            {
                                string v = "@v" + i;
                                if (counter < maxCounter && values.Length == columnNames.Count - 5)
                                {
                                    assetsCmd.Parameters.AddWithValue(v, values[counter - 5]);
                                    counter++;
                                }
                                else if (counter < maxCounter && values.Length < columnNames.Count - 5)
                                {
                                    if (counter - 5 < values.Length)
                                        assetsCmd.Parameters.AddWithValue(v, values[counter - 5]);
                                    else
                                        assetsCmd.Parameters.AddWithValue(v, String.Empty);
                                    counter++;
                                }
                                else if (counter < maxCounter && values.Length > columnNames.Count - 5)
                                {
                                    if (counter - 5 < columnNames.Count - 4)
                                        assetsCmd.Parameters.AddWithValue(v, values[counter - 5]);
                                    counter++;
                                }
                            }
                        }

                        assetsCmd.Parameters.AddWithValue("@cancelled", cancelled);
                        assetsCmd.Parameters.AddWithValue("@removed", removed);

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
                        op.OperationLog("UserID: " + userID + " || Operacja wykonana: dodanie przedmiotu o ID: " + IDBox.Text + " do tabeli Assets");
                    }
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
            if(perm == "admin")
            {
                try
                { 
                    List<string> columnNames = new List<string>();
                    string userID = String.Empty;

                    assetsConn.Open();

                    assetsCmd.Connection = assetsConn;

                    assetsCmd.CommandText = "SELECT * FROM Assets";

                    OleDbDataReader dr = assetsCmd.ExecuteReader();

                    for (int i = 0; i < dr.FieldCount; i++)
                        columnNames.Add(dr.GetName(i));

                    dr.Close();
                    assetsConn.Close();

                    assetsConn.Open();

                    assetsCmd.Connection = assetsConn;

                    if (IDBox.Text != String.Empty && ItemNameBox.Text == String.Empty)
                    {
                        assetsCmd.CommandText = "DELETE FROM Assets WHERE ID = @id";
                        assetsCmd.Parameters.AddWithValue("@id", IDBox.Text);
                    }
                    else if (IDBox.Text == String.Empty && ItemNameBox.Text != String.Empty)
                    {
                        assetsCmd.CommandText = "DELETE FROM Assets WHERE Nazwa = @itemName";
                        assetsCmd.Parameters.AddWithValue("@itemName", ItemNameBox.Text);
                    }
                    else if (IDBox.Text == String.Empty && ItemNameBox.Text == String.Empty)
                    {
                        assetsCmd.CommandText = "DELETE FROM Assets WHERE ID = @id AND Nazwa = @itemName";
                        assetsCmd.Parameters.AddWithValue("@id", IDBox.Text);
                        assetsCmd.Parameters.AddWithValue("@itemName", ItemNameBox.Text);
                    }
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
                    op.OperationLog("UserID: " + userID + " || Operacja wykonana: usunięcie przedmiotu o ID: " + IDBox.Text + " z tabeli Assets");

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
            if(perm == "admin")
            {
                ShowDataGrid();
                ClearTextBoxes();
            }
        }

        protected void addColumnButton_Click(object sender, EventArgs e)
        {
            if(perm == "admin")
            {
                try
                {
                    string userID = String.Empty;

                    assetsConn.Open();

                    assetsCmd.Connection = assetsConn;

                    assetsCmd.CommandText = "ALTER TABLE Assets ADD " + columnBox.Text + " string";
                    assetsCmd.ExecuteNonQuery();

                    assetsCmd.CommandText = "ALTER TABLE Borrows ADD " + columnBox.Text + " string";
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
                    op.OperationLog("UserID: " + userID + " || Operacja wykonana: dodanie pola opisowego: " + columnBox.Text);

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

                    assetsCmd.CommandText = "ALTER TABLE Assets DROP COLUMN " + columnBox.Text;
                    assetsCmd.ExecuteNonQuery();

                    assetsCmd.CommandText = "ALTER TABLE Borrows DROP COLUMN " + columnBox.Text;
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
                    op.OperationLog("UserID: " + userID + " || Operacja wykonana: usunięcie pola opisowego: " + columnBox.Text + " z tabeli Assets");

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

        protected void borrowedButton_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> columnNames = new List<string>();
                List<string> values = new List<string>();
                string userID = String.Empty;
                string date = System.DateTime.Now.ToString();
                int j = 0;

                assetsConn.Open();
                assetsCmd.Connection = assetsConn;
                assetsCmd.CommandText = "SELECT * FROM Assets WHERE Wypożyczony = 'Tak'";

                OleDbDataReader dr = assetsCmd.ExecuteReader();
                for (int i = 0; i < dr.FieldCount; i++)
                    columnNames.Add(dr.GetName(i));

                while (dr.Read())
                {
                    for (int i = 0; i < dr.FieldCount; i++)
                        values.Add(dr.GetValue(i).ToString());
                }

                assetsConn.Close();

                var doc = new Document();
                PdfWriter.GetInstance(doc, new FileStream(AppDomain.CurrentDomain.BaseDirectory + "PDF\\Raporty\\" + System.DateTime.Now.ToString("yyyy_MM_dd-HH_mm") + ".pdf", FileMode.Create));

                doc.Open();

                Font titleFont = FontFactory.GetFont(BaseFont.TIMES_ROMAN, BaseFont.CP1257, 14f, Font.BOLD);
                Font dataFont = FontFactory.GetFont(BaseFont.TIMES_ROMAN, BaseFont.CP1257, 12f);
                Phrase title = new Phrase("Raport wypożyczeń na dzień " + date, titleFont);
                Paragraph titlePar = new Paragraph(title);
                Paragraph dataPar = new Paragraph();

                for (int i = 0; i < columnNames.Count; i++)
                {
                    if (i == columnNames.Count - 1)
                    {
                        Phrase p = new Phrase(columnNames[i] + ": " + values[j] + "\n\n", dataFont);
                        dataPar.Add(p);
                        i = -1;
                    }
                    else
                    {
                        Phrase p = new Phrase(columnNames[i] + ": " + values[j] + "\n", dataFont);
                        dataPar.Add(p);
                    }
                    j++;
                    if (j == values.Count)
                        break;
                }
                titlePar.SetAlignment("Center");
                doc.Add(titlePar);
                doc.Add(dataPar);
                doc.Close();

                Scripts.OperationLogCreator op1 = new Scripts.OperationLogCreator();
                op1.OperationLog("UserID: " + userID + " || Operacja wykonana: wygenerowanie raportu z aktualnie wypożyczonym sprzętem");
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

        protected void availableButton_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> columnNames = new List<string>();
                List<string> values = new List<string>();
                string userID = String.Empty;
                string date = System.DateTime.Now.ToString();
                int j = 0;

                assetsConn.Open();
                assetsCmd.Connection = assetsConn;
                assetsCmd.CommandText = "SELECT * FROM Assets WHERE Wycofany = 'Nie' AND Skasowany = 'Nie' AND Wypożyczony = 'Nie'";

                OleDbDataReader dr = assetsCmd.ExecuteReader();
                for (int i = 0; i < dr.FieldCount; i++)
                    columnNames.Add(dr.GetName(i));

                while (dr.Read())
                {
                    for (int i = 0; i < dr.FieldCount; i++)
                        values.Add(dr.GetValue(i).ToString());
                }

                assetsConn.Close();

                var doc = new Document();
                PdfWriter.GetInstance(doc, new FileStream(AppDomain.CurrentDomain.BaseDirectory + "PDF\\Raporty\\" + System.DateTime.Now.ToString("yyyy_MM_dd-HH_mm") + ".pdf", FileMode.Create));

                doc.Open();

                Font titleFont = FontFactory.GetFont(BaseFont.TIMES_ROMAN, BaseFont.CP1257, 14f, Font.BOLD);
                Font dataFont = FontFactory.GetFont(BaseFont.TIMES_ROMAN, BaseFont.CP1257, 12f);
                Phrase title = new Phrase("Raport wypożyczeń na dzień " + date, titleFont);
                Paragraph titlePar = new Paragraph(title);
                Paragraph dataPar = new Paragraph();

                for (int i = 0; i < columnNames.Count; i++)
                {
                    if (i == columnNames.Count - 1)
                    {
                        Phrase p = new Phrase(columnNames[i] + ": " + values[j] + "\n\n", dataFont);
                        dataPar.Add(p);
                        i = -1;
                    }
                    else
                    {
                        Phrase p = new Phrase(columnNames[i] + ": " + values[j] + "\n", dataFont);
                        dataPar.Add(p);
                    }
                    j++;
                    if (j == values.Count)
                        break;
                }
                titlePar.SetAlignment("Center");
                doc.Add(titlePar);
                doc.Add(dataPar);
                doc.Close();

                Scripts.OperationLogCreator op1 = new Scripts.OperationLogCreator();
                op1.OperationLog("UserID: " + userID + " || Operacja wykonana: wygenerowanie raportu z aktualnie dostępnym sprzętem");
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

        protected void allHistoryButton_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> columnNames = new List<string>();
                List<string> values = new List<string>();
                string userID = String.Empty;
                string date = System.DateTime.Now.ToString();
                string fDate, tDate;
                DateTime fromDate, toDate;
                int j = 0;

                fDate = fromDateTextBox.Text;
                tDate = toDateTextBox.Text;
                System.Globalization.DateTimeFormatInfo dateInfo = new System.Globalization.DateTimeFormatInfo();
                dateInfo.FullDateTimePattern = "dd/MM/yyyy HH:mm:ss tt";
                fromDate = Convert.ToDateTime(fDate, dateInfo);
                toDate = Convert.ToDateTime(tDate, dateInfo);

                assetsConn.Open();
                assetsCmd.Connection = assetsConn;
                assetsCmd.CommandText = "SELECT * FROM Borrows WHERE Data_wypożyczenia BETWEEN @fromDate AND @toDate ORDER BY @sortColumn";
                assetsCmd.Parameters.AddWithValue("@fromDate", fromDate);
                assetsCmd.Parameters.AddWithValue("@toDate", toDate);
                if(sortColumnBox.Text == String.Empty)
                    assetsCmd.Parameters.AddWithValue("@sortColumn", "ID");
                else
                    assetsCmd.Parameters.AddWithValue("@sortColumn", sortColumnBox.Text);

                OleDbDataReader dr = assetsCmd.ExecuteReader();
                for (int i = 0; i < dr.FieldCount; i++)
                    columnNames.Add(dr.GetName(i));

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        for (int i = 0; i < dr.FieldCount; i++)
                            values.Add(dr.GetValue(i).ToString());
                    }
                }

                assetsConn.Close();
                assetsCmd.Parameters.Clear();

                var doc = new Document();
                PdfWriter.GetInstance(doc, new FileStream(AppDomain.CurrentDomain.BaseDirectory + "PDF\\Raporty\\" + System.DateTime.Now.ToString("yyyy_MM_dd-HH_mm") + ".pdf", FileMode.Create));

                doc.Open();

                Font titleFont = FontFactory.GetFont(BaseFont.TIMES_ROMAN, BaseFont.CP1257, 14f, Font.BOLD);
                Font dataFont = FontFactory.GetFont(BaseFont.TIMES_ROMAN, BaseFont.CP1257, 12f);
                Phrase title = new Phrase("Raport wypożyczeń na dzień " + date, titleFont);
                Paragraph titlePar = new Paragraph(title);
                Paragraph dataPar = new Paragraph();

                for (int i = 0; i < columnNames.Count; i++)
                {
                    if (i == columnNames.Count - 1)
                    {
                        Phrase p = new Phrase(columnNames[i] + ": " + values[j] + "\n\n", dataFont);
                        dataPar.Add(p);
                        i = -1;
                    }
                    else
                    {
                        Phrase p = new Phrase(columnNames[i] + ": " + values[j] + "\n", dataFont);
                        dataPar.Add(p);
                    }
                    j++;
                    if (j == values.Count)
                        break;
                }
                titlePar.SetAlignment("Center");
                doc.Add(titlePar);
                doc.Add(dataPar);
                doc.Close();

                Scripts.OperationLogCreator op1 = new Scripts.OperationLogCreator();
                op1.OperationLog("UserID: " + userID + " || Operacja wykonana: wygenerowanie historii operacji na sprzęcie");
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

        protected void oneHistoryButton_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> columnNames = new List<string>();
                List<string> values = new List<string>();
                string userID = String.Empty;
                string date = System.DateTime.Now.ToString();
                int j = 0;

                assetsConn.Open();
                assetsCmd.Connection = assetsConn;
                assetsCmd.CommandText = "SELECT * FROM Borrows WHERE ID_przedmiotu = @itemID";
                assetsCmd.Parameters.AddWithValue("@itemID", IDBox.Text);

                OleDbDataReader dr = assetsCmd.ExecuteReader();
                for (int i = 0; i < dr.FieldCount; i++)
                    columnNames.Add(dr.GetName(i));

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        for (int i = 0; i < dr.FieldCount; i++)
                            values.Add(dr.GetValue(i).ToString());
                    }
                }

                assetsConn.Close();
                assetsCmd.Parameters.Clear();

                var doc = new Document();
                PdfWriter.GetInstance(doc, new FileStream(AppDomain.CurrentDomain.BaseDirectory + "PDF\\Raporty\\" + System.DateTime.Now.ToString("yyyy_MM_dd-HH_mm") + ".pdf", FileMode.Create));

                doc.Open();

                Font titleFont = FontFactory.GetFont(BaseFont.TIMES_ROMAN, BaseFont.CP1257, 14f, Font.BOLD);
                Font dataFont = FontFactory.GetFont(BaseFont.TIMES_ROMAN, BaseFont.CP1257, 12f);
                Phrase title = new Phrase("Historia operacji dla sprzętu o ID: " + IDBox.Text, titleFont);
                Paragraph titlePar = new Paragraph(title);
                Paragraph dataPar = new Paragraph();

                for (int i = 0; i < columnNames.Count; i++)
                {
                    if (i == columnNames.Count - 1)
                    {
                        Phrase p = new Phrase(columnNames[i] + ": " + values[j] + "\n\n", dataFont);
                        dataPar.Add(p);
                        i = -1;
                    }
                    else
                    {
                        Phrase p = new Phrase(columnNames[i] + ": " + values[j] + "\n", dataFont);
                        dataPar.Add(p);
                    }
                    j++;
                    if (j == values.Count)
                        break;
                }
                titlePar.SetAlignment("Center");
                doc.Add(titlePar);
                doc.Add(dataPar);
                doc.Close();

                Scripts.OperationLogCreator op1 = new Scripts.OperationLogCreator();
                op1.OperationLog("UserID: " + userID + " || Operacja wykonana: wygenerowanie historii operacji dla sprzętu o ID: " + IDBox.Text);
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

        private void ClearTextBoxes()
        {
            IDBox.Text = String.Empty;
            ItemNameBox.Text = String.Empty;
            DescriptionBox.Text = String.Empty;
            columnBox.Text = String.Empty;
            sortColumnBox.Text = String.Empty;
            fromDateTextBox.Text = String.Empty;
            toDateTextBox.Text = String.Empty;
        }
    }
}