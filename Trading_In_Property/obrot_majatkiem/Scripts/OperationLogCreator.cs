using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace obrot_majatkiem.Scripts
{
    public class OperationLogCreator
    {
        private string opLogFormat = System.DateTime.Now.ToString() + " ==> ";
        private string opLogPath = AppDomain.CurrentDomain.BaseDirectory + "Logs\\operationsLog.txt";

        public void OperationLog(string _opString)
        {
            StreamWriter sw = new StreamWriter(opLogPath, true);
            sw.WriteLine(opLogFormat + _opString);
            sw.Flush();
            sw.Close();
        }
    }
}