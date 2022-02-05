using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            //throw new NotImplementedException();
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])

            //DateTime now1 = DateTime.Now;
            //DateTime now2 = DateTime.Now;

            //headerLines.Add("Date: " + now1.AddDays(0).ToLongDateString()+ " "+now2.ToLongTimeString());

            string now = DateTime.Now.ToString();
            headerLines.Add("Date: " + now);
            headerLines.Add("\r\n");

            headerLines.Add("Content-Type: " + contentType);
            headerLines.Add("\r\n");
            headerLines.Add("Content-Length: " + content.Length);
            headerLines.Add("\r\n");

            if (redirectoinPath.Length != 0)
            {
                headerLines.Add("Location:" + redirectoinPath);
                headerLines.Add("\r\n");
            }

            // TODO: Create the response string
            responseString = GetStatusLine(code);
            for (int i = 0; i < headerLines.Count; i++)
            {
                responseString += headerLines[i];
            }
            responseString += "\r\n"+ content;
            Console.Write(responseString);

            //responseString = GetStatusLine(code) + headerLines.GetRange(0, count) + "\r\n" + content + "\r\n";
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = string.Empty;
            string[] names = Enum.GetNames(typeof(StatusCode));
            int[] values = (int[])Enum.GetValues(typeof(StatusCode));
            string name = string.Empty;
            for (int i = 0; i < values.Length; i++)
            {
                if ((int)code == values[i])
                {
                    name = names[i];
                    break;
                }
            }
            statusLine = "HTTP/1.1 " + (int)code + " " + name + "\r\n";
            return statusLine;
        }
    }
}