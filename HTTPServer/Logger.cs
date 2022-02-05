using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        static StreamWriter s ;
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            //Datetime:
              s = new StreamWriter("log.txt");
            string now = DateTime.Now.ToString();
            s.WriteLine(now);
            //message:
            string exception = ex.Message;
            s.WriteLine(exception);
            // for each exception write its details associated with datetime 
            s.Close();
        }
    }
}
