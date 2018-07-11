using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace obrot_majatkiem.Scripts
{
    public class MakeDocuments
    {
        public void CreatePDF(string _fileName)
        {
            var doc = new Document();

            string path = AppDomain.CurrentDomain.BaseDirectory + "PDF\\";
            PdfWriter.GetInstance(doc, new FileStream(path + _fileName, FileMode.Create));

            doc.Open();
            


            doc.Close();
        }
    }
}