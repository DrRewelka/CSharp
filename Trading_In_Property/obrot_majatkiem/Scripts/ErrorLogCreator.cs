using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace obrot_majatkiem.Scripts
{
    public class ErrorLogCreator
    {
        private string errorLogFormat = System.DateTime.Now.ToString() + " ==> ";
        private string errorLogPath = AppDomain.CurrentDomain.BaseDirectory + "Logs\\errorLog.txt";

        public void ErrorLog(string _errorMsg)
        {
            StreamWriter sw = new StreamWriter(errorLogPath, true);
            sw.WriteLine(errorLogFormat + _errorMsg);
            sw.Flush();
            sw.Close();
        }
    }
}