using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines = new string[50];//at least has Request line, Host Header, Blank line
        string[] requestLine = new string[50];//Method URI HTTP 

        RequestMethod method;
        public string relativeURI;
        HTTPVersion httpVersion;
        Dictionary<string, string> headerLines = new Dictionary<string, string>();

        string requestString;//Request line, Header line, Blank line
        string[] contentLines;//Used in Post

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            //TODO: parse the receivedRequest using the \r\n delimeter   
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)

            // Parse Request line
            bool ret;
            ret = ParseRequestLine();
            if (!ret)
                return false;

            // Validate blank line exists
            ret = ValidateBlankLine();
            if (!ret)
                return false;

            //CHECK method
            if (requestLine[0] == "GET")
                method = RequestMethod.GET;
            else
                return false;

            //CHECK HTTP version
            if (requestLine[2] == "HTTP/0.9")
                httpVersion = HTTPVersion.HTTP09;
            else if (requestLine[2] == "HTTP/1.0")
                httpVersion = HTTPVersion.HTTP10;
            else if (requestLine[2] == "HTTP/1.1")
                httpVersion = HTTPVersion.HTTP11;
            else
                return false;

            //VALIDATE URI
            ret = ValidateIsURI(requestLine[1]);
            if (!ret)
                return false;

            // Load header lines into HeaderLines dictionary
            ret = LoadHeaderLines();
            if (!ret)
                return false;

            return true;
        }

        private bool ParseRequestLine()
        {
            string[] spearator = { "\r\n" };
            requestLines = requestString.Split(spearator, StringSplitOptions.None);
            if (requestLines.Length < 3)
                return false;

            string[] delimiter = { " " };
            requestLine = requestLines[0].Split(delimiter, StringSplitOptions.None);
            if (requestLine.Length != 3)
                return false;

            return true;
        }

        private bool ValidateIsURI(string uri)
        {
            //return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
            if (uri == "")
                return false;

            string[] spearator = { "/" };
            string[] absoluteURI = uri.Split(spearator, StringSplitOptions.None);
            relativeURI = absoluteURI[absoluteURI.Length - 1];

            return true;
        }

        private bool LoadHeaderLines()
        {
            for (int i = 1; i < requestLines.Length - 2; i++)
            {
                string[] spearator = { ":" };
                string[] key_val = requestLines[i].Split(spearator, StringSplitOptions.None);
                if (key_val.Length < 2)
                    return false;
                headerLines.Add(key_val[0], key_val[1]);
            }

            return true;
        }

        private bool ValidateBlankLine()
        {
            if (requestLines[requestLines.Length - 1] != "")
                return false;
            return true;
        }

    }
}
