using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateRedirectionRulesFile();
            //Start server
            // 1) Make server object on port 1000
            Server s = new Server(1000, "redirectionRules.txt");
            // 2) Start Server
            s.StartServer();
        }

        static void CreateRedirectionRulesFile()
        {
            // TODO: Create file named redirectionRules.txt
            // each line in the file specify a redirection rule
            string path = "redirectionRules.txt";
            // example: "aboutus.html,aboutus2.html"
            string text = "aboutus.html,aboutus2.html";
            File.WriteAllText(path, text);
            
            // means that when making request to aboustus.html,, it redirects me to aboutus2
        }
         
    }
}
