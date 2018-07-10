using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.Xml.Serialization;

namespace DataBaseAccess
{
    class Program
    {
        static void Main(string[] args)
        {
            string strAccessConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\\DataBase.mdb";
            string strAccessSelect = "SELECT * FROM Categories";

            DataSet myDataSet = new DataSet();
            OleDbConnection myAccessConnection = null;

            try
            {
                myAccessConnection = new OleDbConnection(strAccessConn);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error: Failed to create a database connection. \n{0}", ex.Message);
                Console.ReadKey();
                return;
            }

            try
            {
                OleDbCommand myAccessCommand = new OleDbCommand(strAccessSelect, myAccessConnection);
                OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(myAccessCommand);

                myAccessConnection.Open();
                myDataAdapter.Fill(myDataSet, "Categories");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error: Failed to retrieve required data from database. \n{0}", ex.Message);
                Console.ReadKey();
                return;
            }
            finally
            {
                myAccessConnection.Close();
            }

            DataTableCollection dataColl = myDataSet.Tables;
            foreach (DataTable dt in dataColl)
            {
                Console.WriteLine("Found data table {0}", dt.TableName);
            }

            Console.WriteLine("{0} tables in data set", myDataSet.Tables.Count); //moze byc dataColl.Count;
            Console.WriteLine("{0} rows in Categories table", myDataSet.Tables["Categories"].Columns.Count);
            DataColumnCollection columnColl = myDataSet.Tables["Categories"].Columns;

            int i = 0;
            foreach (DataColumn dc in columnColl)
            {
                Console.WriteLine("Column name[{0}] is {1}, of type {2}", i++, dc.ColumnName, dc.DataType);
            }

            DataRowCollection rowColl = myDataSet.Tables["Categories"].Rows;
            foreach (DataRow dr in rowColl)
            {
                Console.WriteLine("CategoryName[{0}] is {1}", dr[0], dr[1]);
            }
            Console.ReadKey();
        }
    }
}
