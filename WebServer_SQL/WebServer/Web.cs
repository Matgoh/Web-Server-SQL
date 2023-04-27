﻿using Communications;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Text.Encodings.Web;

namespace WebServer
{

    /// <summary>
    /// Author:   H. James de St. Germain
    /// Date:     Spring 2020
    /// Updated:  Spring 2023
    /// 
    /// Code for a simple web server
    /// </summary>
    class Web
    {
        /// <summary>
        /// keep track of how many requests have come in.  Just used
        /// for display purposes.
        /// </summary>
        static private int counter = 1;

        /// <summary>
        /// Basic connect handler - i.e., a browser has connected!
        /// Print an information message
        /// </summary>
        /// <param name="channel"> the Networking connection</param>       
        internal static void OnClientConnect(Networking channel)
        {
            Console.WriteLine("Connected to browser");
        }

        /// <summary>
        /// Create the HTTP response header, containing items such as
        /// the "HTTP/1.1 200 OK" line.
        /// 
        /// See: https://www.tutorialspoint.com/http/http_responses.htm
        /// 
        /// Warning, don't forget that there have to be new lines at the
        /// end of this message!
        /// </summary>
        /// <param name="length"> how big a message are we sending</param>
        /// <param name="type"> usually html, but could be css</param>
        /// <returns>returns a string with the response header</returns>
        private static string BuildHTTPResponseHeader(int length, string type = "text/html")
        {
            string header = $@"
HTTP/1.1 200 OK
Date: {DateTime.Now}
Server: Matthew's Server
Last-Modified: Wed, 22 Jul 2009 19:15:56 GMT
Content-Length: {length}
Content-Type: text/html
Connection: Closed";

            return header;
        }

        /// <summary>
        ///   Create a web page!  The body of the returned message is the web page
        ///   "code" itself. Usually this would start with the doctype tag followed by the HTML element.  Take a look at:
        ///   https://www.sitepoint.com/a-basic-html5-template/
        /// </summary>
        /// <returns> A string the represents a web page.</returns>
        private static string BuildHTTPBody()
        {
            // FIXME: this should be a complete web page.

            return $@"
<html>
<head>
<style>
{{background - color: blue;}}
h1 {{color: maroon;}}
p {{color: LightSeaGreen;}}
a {{color: Olive;}}
</style>
</head>
<h1>Agari.o Website!{counter}</h1>
<a href='http://localhost:11001/Reload'>Reload Page</a> 
<br>
<a href='http://localhost:11001/Highscores'>High Scores</a> 
<p>how are you...</p>
<html>";
        }


        /// <summary>
        /// Create a response message string to send back to the connecting
        /// program (i.e., the web browser).  The string is of the form:
        /// 
        ///   HTTP Header
        ///   [new line]
        ///   HTTP Body
        ///  
        ///  The Header must follow the header protocol.
        ///  The body should follow the HTML doc protocol.
        /// </summary>
        /// <returns> the complete HTTP response</returns>
        private static string BuildMainPage()
        {
            string message = BuildHTTPBody();
            string header = BuildHTTPResponseHeader(message.Length);

            return header + message;
        }


        /// <summary>
        ///   <para>
        ///     When a request comes in (from a browser) this method will
        ///     be called by the Networking code.  Each line of the HTTP request
        ///     will come as a separate message.  The "line" we are interested in
        ///     is a PUT or GET request.  
        ///   </para>
        ///   <para>
        ///     The following messages are actionable:
        ///   </para>
        ///   <para>
        ///      get highscore - respond with a highscore page
        ///   </para>
        ///   <para>
        ///      get favicon - don't do anything (we don't support this)
        ///   </para>
        ///   <para>
        ///      get scores/name - along with a name, respond with a list of scores for the particular user
        ///   <para>
        ///      get scores/name/highmass/highrank/startime/endtime - insert the appropriate data
        ///      into the database.
        ///   </para>
        ///   </para>
        ///   <para>
        ///     create - contact the DB and create the required tables and seed them with some dummy data
        ///   </para>
        ///   <para>
        ///     get index (or "", or "/") - send a happy home page back
        ///   </para>
        ///   <para>
        ///     get css/styles.css?v=1.0  - send your sites css file data back
        ///   </para>
        ///   <para>
        ///     otherwise send a page not found error
        ///   </para>
        ///   <para>
        ///     Warning: when you send a response, the web browser is going to expect the message to
        ///     be line by line (new line separated) but we use new line as a special character in our
        ///     networking object.  Thus, you have to send _every line of your response_ as a new Send message.
        ///   </para>
        /// </summary>
        /// <param name="network_message_state"> provided by the Networking code, contains socket and message</param>
        public static void onMessage(Networking channel, string message)
        {
            //var game_list = Lab_Starter_Code.GetGames();       

            // If reload message is sent, just send header and body again
            if (message.StartsWith("GET/Reload"))
            {
               // reload web page
            }

            // Get highscores page
            if (message.StartsWith("GET/Highscores"))
            {
                
            }

            string body = BuildHTTPBody();
            int bodyLength = body.Length;
            string header = BuildHTTPResponseHeader(bodyLength);

            channel.Send(header);
            channel.Send("");
            channel.Send(body);
            Console.WriteLine(message);
        }

        /// <summary>
        /// Handle some CSS to make our pages beautiful
        /// </summary>
        /// <returns>HTTP Response Header with CSS file contents added</returns>
        private static string SendCSSResponse()
        {
            throw new NotSupportedException("read the css file from the solution folder, build an http response, and return this string");
            //Note: for starters, simply return a static hand written css string from right here (don't do file reading)
        }


        /// <summary>
        ///    (1) Instruct the DB to seed itself (build tables, add data)
        ///    (2) Report to the web browser on the success
        /// </summary>
        /// <returns> the HTTP response header followed by some informative information</returns>
        private static string CreateDBTablesPage()
        {
            return $@"CREATE TABLE Persons(
    PersonID int,
    LastName varchar(255),
    FirstName varchar(255),
    Address varchar(255),
    City varchar(255)
)";
          //  throw new NotImplementedException("create the database tables by 'talking' with the DB server and then return an informative web page");
        }

        internal static void OnDisconnect(Networking channel)
        {
            Debug.WriteLine($"Goodbye {channel.RemoteAddressPort}");
        }

        public static SqlConnection SqlConnection()
        {
            var builder = new ConfigurationBuilder();

            builder.AddUserSecrets<SqlConnection>();
            IConfigurationRoot Configuration = builder.Build();
            var SelectedSecrets = Configuration.GetSection("WebServerSecrets");

            string connectionString = new SqlConnectionStringBuilder()
            {
                DataSource = SelectedSecrets["server_name"],
                InitialCatalog = SelectedSecrets["database_name"],
                UserID = SelectedSecrets["userID"],
                Password = SelectedSecrets["LabDBPassword"],
                ConnectTimeout = 15, // if the server doesn't connect in X seconds, give up
                Encrypt = false
            }.ConnectionString;

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
       

    }
}