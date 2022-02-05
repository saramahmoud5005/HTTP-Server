using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;
        private int portNumber;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            //TODO: initialize this.serverSocket

            this.LoadRedirectionRules(redirectionMatrixPath);
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            this.portNumber = portNumber;
            IPEndPoint host_end_point = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket.Bind(host_end_point);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
          
            serverSocket.Listen(1000);
             // TODO: Accept connections in while loop and
            while (true)
            {
                //TODO: accept connections 
                Socket client_Socket = this.serverSocket.Accept();

                Console.WriteLine("New Client Accepted: {0}", client_Socket.RemoteEndPoint);
                 //start a thread for each connection on function "Handle Connection"
                Thread new_thread = new Thread(new ParameterizedThreadStart(HandleConnection));

                new_thread.Start(client_Socket);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period

            Socket client_Sock = (Socket)obj;
            client_Sock.ReceiveTimeout = 0;

            string welcome = "Welcome to my test server";
            byte[] data = Encoding.ASCII.GetBytes(welcome);
            //client_Sock.Send(data);
            int received_length;

            // TODO: receive requests in while true until remote client closes the socket.


            while (true)
            {
                try
                {
                    // TODO: Receive request
                    data = new byte[1024 * 1024];
                    received_length = client_Sock.Receive(data);


                    // TODO: break the while loop if receivedLen==0
                    // true -> means client has Closed the connection
                    // Close the connection with this client

                    if (received_length == 0)
                    {
                        Console.WriteLine("Client: {0} ended the connection", client_Sock.RemoteEndPoint);
                        break;
                    }
                    string message = Encoding.ASCII.GetString(data, 0, received_length);

                    // TODO: Create a Request object using received request string
                    Request req = new Request(message);

                    // TODO: Call HandleRequest Method that returns the response
                    Response res = HandleRequest(req);

                    // TODO: Send Response back to client
                    string response = res.ResponseString;
                    data = Encoding.ASCII.GetBytes(response);
                    client_Sock.Send(data, 0, data.Length, SocketFlags.None);
                }


                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            client_Sock.Close();
        }

        Response HandleRequest(Request request)
        {
            Response res;
            string content=string.Empty;
            string content_type = "text/html";
            string uri = "";
            try
            {
                //TODO: check for bad request 
                if (request.ParseRequest() == false)
                {
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    return res = new Response(StatusCode.BadRequest, content_type, content, string.Empty);
                    
                }
                //TODO: map the relativeURI in request to get the physical path of the resource.
                uri = request.relativeURI;
                //TODO: check for redirect
                string redirectedPage = GetRedirectionPagePathIFExist(uri);
                if(redirectedPage != string.Empty)
                {
                    content = LoadDefaultPage(Configuration.RedirectionDefaultPageName);
                    res = new Response(StatusCode.Redirect, content_type, content, redirectedPage);
                    return res;
                }
                //TODO: check file exists
                //TODO: read the physical file
                content = LoadDefaultPage(uri);
                if(content == string.Empty)
                {
                    content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    return res = new Response(StatusCode.NotFound, content_type, content, string.Empty);
        
                }

                // Create OK response
                return res = new Response(StatusCode.OK, content_type, content, string.Empty);
                
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
              return  res = new Response(StatusCode.InternalServerError, content_type, content,string.Empty);
            
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            string redirected_page = string.Empty;
            if (Configuration.RedirectionRules.ContainsKey(relativePath))
                redirected_page = Configuration.RedirectionRules[relativePath];

                return redirected_page;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            string loaded_content= string.Empty;
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            if (File.Exists(filePath) == false)
            {
                Logger.LogException(new FileNotFoundException());
            }
            // else read file and return its content
            else
            {
                loaded_content = File.ReadAllText(filePath);

            }
            return loaded_content;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file
                StreamReader sr = new StreamReader(filePath);
                string contentRules = sr.ReadToEnd();
                // then fill Configuration.RedirectionRules dictionary 

                string[] spearator = { "," };
                string[] pathes = contentRules.Split(spearator, StringSplitOptions.None);
                for (int i = 0; i < pathes.Length; ++i)
                {
                    Configuration.RedirectionRules[pathes[i]] = pathes[i + 1];
                    i++;
                }

            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        
        }
    }
}
