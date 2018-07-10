using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace TestDB
{
    public partial class Form1 : Form
    {
        private OleDbConnection bookConn;
        private OleDbConnection userConn;
        private OleDbCommand oleDbCmd = new OleDbCommand();
        private string connParam = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\TestDB.mdb";
        private string userConnParam = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users.mdb";
        private bool loggedIn = false;

        public Form1()
        {
            bookConn = new OleDbConnection(connParam);
            userConn = new OleDbConnection(userConnParam);
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'testDBDataSet.Book' table. You can move, or remove it, as needed.
            this.bookTableAdapter.Fill(this.testDBDataSet.Book);
            label7.Text = "Guest";
        }

        private void insert_Click_1(object sender, EventArgs e)
        {
            bookConn.Open();
            oleDbCmd.Connection = bookConn;
            oleDbCmd.CommandText = "insert into Book (BookName, BookDescription) values ('" + this.textBox1.Text + "','" + this.textBox2.Text + "');";
            oleDbCmd.ExecuteNonQuery();
            bookConn.Close();

            textBox1.Text = null;
            textBox2.Text = null;

            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            OleDbDataAdapter dAdapter = new OleDbDataAdapter("select * from Book where ID=(select max(ID) from Book)", connParam);
            OleDbCommandBuilder cBuilder = new OleDbCommandBuilder(dAdapter);

            DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();

            dAdapter.Fill(dataTable);

            dataGridView1.Rows.Add(dataTable.Rows[0][0], dataTable.Rows[0][1], dataTable.Rows[0][2]);
        }

        private void showAll_Click_1(object sender, EventArgs e)
        {
            if (loggedIn)
            {
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                dataGridView1.Refresh();

                OleDbDataAdapter dAdapter = new OleDbDataAdapter("select * from Book", connParam);
                OleDbCommandBuilder cBuilder = new OleDbCommandBuilder(dAdapter);

                DataTable dataTable = new DataTable();
                DataSet ds = new DataSet();

                dAdapter.Fill(dataTable);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    dataGridView1.Rows.Add(dataTable.Rows[i][0], dataTable.Rows[i][1], dataTable.Rows[i][2]);
                }
            }
            else
                MessageBox.Show("You are not logged in!");
        }

        private void remove_Click(object sender, EventArgs e)
        {
            if (loggedIn)
            {
                bookConn.Open();

                oleDbCmd.Connection = bookConn;
                oleDbCmd.CommandText = "delete from Book where ID = " + textBox3.Text;
                textBox3.Text = null;
                oleDbCmd.ExecuteNonQuery();

                bookConn.Close();

                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                dataGridView1.Refresh();

                OleDbDataAdapter dAdapter = new OleDbDataAdapter("select * from Book", connParam);
                OleDbCommandBuilder cBuilder = new OleDbCommandBuilder(dAdapter);

                DataTable dataTable = new DataTable();
                DataSet ds = new DataSet();

                dAdapter.Fill(dataTable);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    dataGridView1.Rows.Add(dataTable.Rows[i][0], dataTable.Rows[i][1], dataTable.Rows[i][2]);
                }
            }
            else
                MessageBox.Show("You are not logged in!");
        }

        private void login_Click(object sender, EventArgs e)
        {
            int temp;
            userConn.Open();
            oleDbCmd.Connection = userConn;
            oleDbCmd.CommandText = "select ID from Users where Login='" + textBox4.Text + "' and ID=(select ID from Users where Password='" + textBox5.Text + "');";
            try
            {
                temp = (int)oleDbCmd.ExecuteScalar();
            }
            catch(Exception)
            {
                temp = -1;
            }
            if (temp > 0)
            {
                MessageBox.Show("You are logged in!");
                switch(temp)
                {
                    case 1: label7.Text = "Admin";
                            break;
                    case 2: label7.Text = "Kwiatkowski";
                            break;
                    case 3: label7.Text = "Chrumek";
                            break;
                }
                textBox4.Text = null;
                textBox5.Text = null;
                loggedIn = true;
            }
            else
            {
                MessageBox.Show("Invalid username or password!");
                textBox4.Text = null;
                textBox5.Text = null;
            }
            userConn.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (loggedIn)
            {
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                dataGridView1.Refresh();

                loggedIn = false;
                label7.Text = "Guest";
                MessageBox.Show("You have logged out!");
            }
        }
    }
}
