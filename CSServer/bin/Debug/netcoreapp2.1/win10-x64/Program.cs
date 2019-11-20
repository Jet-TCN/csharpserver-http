
using System;   
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using Newtonsoft.Json;
using Microsoft.VisualBasic;

namespace CSServer
{
    
   public  class Program
    {

       
       
        static HttpListener _httpListener = new HttpListener();
        static string newLog = DateTime.Now.Date.ToString("dd-MM-yyyy") + ".txt";
        static string path = @"log-"+newLog;
        static string compPass = string.Empty;
        static void Main(string[] args)
        {

        //    Console.WriteLine(Utilities.getData(Utilities.EncodeMD5("thisuserisnotonfire").ToLower())) ;
        //  Console.WriteLine(Password.getData(Password.EncodeMD5("qwerty4231").ToLower()));


            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("<p style='color: green'> SYSTEM LOG FIRST CREATED ON " + DateTime.Now.Date.ToString("dd/MM/yyyy") + " @ " + DateTime.Now.TimeOfDay.ToString() + "</p>");
                }
            }
            bool bDebug = true;
            char vInput;
            WebClient client = new WebClient();
            // client.DownloadFile("https://google.co.uk/", @"C:\Users\Jet\source\repos\CSServer\CSServer\localfile.html");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Starting server...");
            Stopwatch _stopWatch = new Stopwatch();
           
           
            _stopWatch.Start();

            // _httpListener.Prefixes.Add("http://172.168.99.210:5000/");
            try
            {

                _httpListener.Prefixes.Add("http://localhost:5000/"); // add prefix "http://localhost:5000/"
            } catch (UnauthorizedAccessException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Program not run in administrator!");
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("<p style='color: red'> [" + DateTime.Now.ToString() + "] WARNING! POTENTIAL UNAUTHORIZED ACCESS!! BY "+  Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString() + "@" + Dns.GetHostName() +"  </p>");
                }
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            _httpListener.Start(); // start server (Run application as Administrator!)
            _stopWatch.Stop();
            // Console.
            Console.WriteLine("Server started @ {0} | Time taken: {1}ms", DateTime.Now.ToString(), _stopWatch.Elapsed.Milliseconds.ToString());
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine("<p style='color: green'> [" + DateTime.Now.ToString() + "] Server initiated by " + Dns.GetHostByName(Dns.GetHostName()).AddressList[1].ToString() + "@" + Dns.GetHostName() + "  </p>");
            }
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            Thread _responseThread = new Thread(ResponseThread);
            int iCursorPosX = Console.CursorLeft; int iCursorPosY = Console.CursorTop;
            Console.WriteLine(Console.CursorSize);
            _responseThread.Start(); // start the response thread

            while (bDebug == true)
            {
               
               vInput = Console.ReadKey(true).KeyChar;
                if (vInput == 'w' | vInput == 'W')
                {
                    
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("HTML");
                    Console.WriteLine(System.IO.File.ReadAllText(@"main.html"));
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("CSS");
                   
                        Console.WriteLine(
                  System.IO.File.ReadAllText(@"styles.css"));
                }
            }
        }

   /*     static void print(string msgText)
        {
                // read 10 lines from the top of the console buffer

        }*/
        static void padding(int px)
        {
            for (int x = 0; x < px; x++)
            {
                Console.Write(" ");
            }
        }
        static void ResponseThread()
        {
            while (true)
            {

                HttpListenerContext context = _httpListener.GetContext(); // get a context
                                                                          // Now, you'll find the request URL in context.Request.Url
                Console.WriteLine(context.Request.Url.ToString());
                if (context.Request.Url.ToString().EndsWith("/log") || context.Request.Url.ToString().EndsWith("/log/"))
                {
                    byte[] _responseArray = System.Text.Encoding.UTF8.GetBytes("<style> body { color: green; } </style>" + System.IO.File.ReadAllText(
                        @"log.html") +
                                         "<head><style>" + System.IO.File.ReadAllText(@"styles.css") + "</style></head>"); // get the bytes to response
                    try
                    {
                        context.Response.OutputStream.Write(_responseArray, 0, _responseArray.Length); // write bytes to the output stream
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Connection interrupt!");
                        ResponseThread();
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                else if (context.Request.Url.ToString().EndsWith("/LOGIN"))
                {
                    try
                    {
                        var logUp = "";
                        string receivedURL = context.Request.Url.ToString();
                        string receivedHash = Utilities.EncodeSHA512((receivedURL.Substring(0, receivedURL.Length - 6)).Substring("http://localhost:5000/".Length)).ToUpper();
                        string receivedPassword = (receivedURL.Substring(0, receivedURL.Length - 6)).Substring("http://localhost:5000/".Length);
                        Console.WriteLine(receivedHash);
                        var lookUp = Utilities.getData(receivedHash);
                         logUp = lookUp;
                        Console.WriteLine(lookUp);
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write(compPass);
                        Console.WriteLine(" | " + receivedHash);
                        if (compPass == receivedHash)
                        {
                            lookUp = "Success!";
                            byte[] _responseArray = System.Text.Encoding.UTF8.GetBytes(logUp);
                            context.Response.OutputStream.Write(_responseArray, 0, _responseArray.Length);
                        } else
                        {
                            byte[] _responseArray = System.Text.Encoding.UTF8.GetBytes("FAILURE");
                            context.Response.OutputStream.Write(_responseArray, 0, _responseArray.Length);
                        }
                        compPass = string.Empty;
                        Console.WriteLine(lookUp);
                        
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        
                        //   Console.WriteLine("Potential admin activity, see logs");
                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.WriteLine("<p style='color: green'> [" + DateTime.Now.ToString() + "] Authorized " + logUp + " access by " + Dns.GetHostByName(Dns.GetHostName()).AddressList[1].ToString() + "@" + Dns.GetHostName() + "  </p>");
                        }
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Connection interrupt!");
                        ResponseThread();
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                else if (context.Request.Url.ToString().EndsWith("/PASSWORD"))
                {
                    try
                    {
                        string receivedURL = context.Request.Url.ToString();
                        string receivedPassword = (receivedURL.Substring(0, receivedURL.Length - 9)).Substring("http://localhost:5000/".Length);
                        Console.WriteLine(receivedPassword);
                        var lookUp = Password.getData(Password.EncodeSHA512(receivedPassword).ToUpper());
                        if (lookUp != "")
                        {
                            Console.WriteLine(lookUp);
                            compPass = lookUp;
                        } else
                        {
                          
                        }
                       // byte[] _responseArray = System.Text.Encoding.UTF8.GetBytes(lookUp);
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        //context.Response.OutputStream.Write(_responseArray, 0, _responseArray.Length);
                        //   Console.WriteLine("Potential admin activity, see logs");
                  //      using (StreamWriter sw = File.AppendText(path))
                    //    {
                    //        sw.WriteLine("<p style='color: green'> [" + DateTime.Now.ToString() + "] Authorized " + lookUp + " access by " + Dns.GetHostByName(Dns.GetHostName()).AddressList[1].ToString() + "@" + Dns.GetHostName() + "  </p>");
                      //  }
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Connection interrupt!");
                        ResponseThread();
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                else if (context.Request.Url.ToString().EndsWith("/log.txt") || context.Request.Url.ToString().EndsWith("/log.txt/"))
                {
                    byte[] _responseArray = System.Text.Encoding.UTF8.GetBytes(/*"   <style> body { margin: 0; line-height: 16px; color: green; font-size: 16px; font-weight: bold; font-family: Arial; } </style>" +*/ System.IO.File.ReadAllText(path)); // get the bytes to response
                    try
                    {
                        context.Response.OutputStream.Write(_responseArray, 0, _responseArray.Length); // write bytes to the output stream
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Connection interrupt!");
                        ResponseThread();
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                else if (context.Request.Url.ToString().EndsWith("/ADMIN") || context.Request.Url.ToString().EndsWith("/ADMIN/"))
                {
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Potential admin activity, see logs");
                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.WriteLine("<p style='color: yellow'> [" + DateTime.Now.ToString() + "] Attention! ROOT Admin account sign-in utilized for previous admin account authorization. </p>");
                        }
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Connection interrupt!");
                        ResponseThread();
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                } else if (context.Request.Url.ToString().EndsWith("/info") || context.Request.Url.ToString().EndsWith("/info/")) {
                    byte[] _responseArray = System.Text.Encoding.UTF8.GetBytes(System.IO.File.ReadAllText(@"info.html") +
"<head><style>" + System.IO.File.ReadAllText(@"styles.css") + "</style></head>" +
"<script type='text/javascript' src='script.js'> </script>"); // get the bytes to response   
                    try
                    {
                        context.Response.OutputStream.Write(_responseArray, 0, _responseArray.Length); // write bytes to the output stream
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Connection interrupt!");
                        ResponseThread();
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                else
                {
                    byte[] _responseArray = System.Text.Encoding.UTF8.GetBytes(System.IO.File.ReadAllText(@"main.html") +
"<head><style>" + System.IO.File.ReadAllText(@"styles.css") + "</style></head>" +
"<script type='text/javascript' src='script.js'> </script>"); // get the bytes to response   
                    try
                    {
                        context.Response.OutputStream.Write(_responseArray, 0, _responseArray.Length); // write bytes to the output stream
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Connection interrupt!");
                        ResponseThread();
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
               

                
                    context.Response.KeepAlive = false; 
                string reqTest1 = context.Request.Url.ToString();
                string reqTest2 = context.Request.RawUrl.ToString();

                if (reqTest1.Contains(".") | (reqTest2.Contains(".")))
                {
                    padding(4);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("User requested {0} file", reqTest2.Substring(1));
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Response given to request:");
                    padding(4);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(context.Request.UserHostAddress);
                    padding(4);
                    Console.WriteLine(context.Request.UserHostName);
                    padding(4);
                    Console.WriteLine(context.Request.UserAgent);
                    padding(4);
                    Console.WriteLine(context.Request.Url);
                    padding(4);
                    Console.WriteLine(context.Request.ServiceName);
                    padding(4);
                    Console.WriteLine(context.Request.ProtocolVersion);
                }
                try
                {
                    context.Response.Close(); // close the connection

                } catch
                {

                }

                
                ResponseThread();
            }
            
        }
        static void OnProcessExit(object sender, EventArgs e)
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine("<p style='color: green'> [" + DateTime.Now.ToString() + "] Server closed by " + Dns.GetHostByName(Dns.GetHostName()).AddressList[1].ToString() + "@" + Dns.GetHostName() + "  </p>");
            }
        }
    }
}
