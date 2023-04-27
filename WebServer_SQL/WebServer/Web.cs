using Communications;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Text.Encodings.Web;

namespace WebServer
{

    /// <summary>
    /// Author:    Matthew Goh
    /// Partner:   Alex Qi
    /// Date:      26-April-2023
    /// Course:    CS 3500, University of Utah, School of Computing
    /// Copyright: CS 3500, Matthew Goh, and Alex Qi - This work may not 
    ///            be copied for use in Academic Coursework.
    ///
    /// I, Matthew Goh and Alex Qi, certify that I wrote this code from scratch and
    /// did not copy it in part or whole from another source.  All 
    /// references used in the completion of the assignments are cited 
    /// in my README file.
    ///
    /// File Contents
    ///
    ///    This class represents a web server that handles HTTP requests and responses.
    /// </summary>
    class Web
    {
        static Database database;

        /// <summary>
        /// Main method that creates a database and networking object to use and wait for clients message. 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            database = new Database();
            Console.WriteLine("Web Server Running");
            Networking server = new(NullLogger.Instance, OnClientConnect, OnDisconnect, onMessage, '\n');
            server.WaitForClients(11001, true);
            Console.ReadLine();
        }

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
<h1>Agar.io Website!</h1>
<a href='http://localhost:11001/Reload'>Reload Page</a> 
<br>
<a href='http://localhost:11001/Highscores'>High Scores</a> 
<br>
<a href='http://localhost:11001/PlayerList'>Player List</a> 
<br>
<a href='http://localhost:11001/Create'>Create New Table</a> 
<p>INFO: This database has various links that should portray certain aspects of the agari.o game. If you would like to see specifc player stats, follow the url with the player name. </p>
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
            // If reload message is sent, just send header and body again
            if (message.Contains("Reload"))
            {
               // reload web page
            }

            if (message.Contains("PlayerList"))
            {
                var playerList = database.GetPlayers();
                string bo = $@"
<html>
<head>
<style>
{{background - color: blue;}}
h1 {{color: GoldenRod;}}
p {{color: LightSeaGreen;}}
a {{color: Olive;}}
</style>
</head>
<hr/>
<h1>Player List:</h1>
<h2> Click on a player to see their stats: </h2>
{playerList}
<html>";
                string highScoreHeader = BuildHTTPResponseHeader(bo.Length);
                channel.Send(highScoreHeader);
                channel.Send("");
                channel.Send(bo);
                Console.WriteLine(message);
            }

            // Create body of highscores page
            if (message.Contains("Highscores"))
            {
                var HighScores = database.GetHighScore();
                string bo = $@"
<html>
<head>
<style>
{{background - color: blue;}}
h1 {{color: GoldenRod;}}
p {{color: LightSeaGreen;}}
h2{{color: Olive;}}
</style>
</head>
<hr/>
<h1>High Scores Page</h1>
<h2> Players With Highest Mass:</h2>
{HighScores}
<html>";
                string highScoreHeader = BuildHTTPResponseHeader(bo.Length);
                channel.Send(highScoreHeader);
                channel.Send("");
                channel.Send(bo);
                Console.WriteLine(message);
            }

            /// This method will handle the names of specific people. 
            if (message.Contains("scores"))
            {
                string toBeSearched = "scores";
                string name = message.Substring(message.IndexOf(toBeSearched) + toBeSearched.Length);
                database.GetPlayerScore(name);

                var playerData = database.GetPlayerData(name);

                string bo = $@"
<html>
<head>
<style>
{{background - color: blue;}}
h1 {{color: GoldenRod;}}
p {{color: LightSeaGreen;}}
a {{color: Olive;}}
</style>
</head>
<hr/>
<h1>Player List:</h1>
<h2> Click on a player to see their stats: </h2>
{playerData}
<html>";

                string highScoreHeader = BuildHTTPResponseHeader(bo.Length);
                channel.Send(highScoreHeader);
                channel.Send("");
                channel.Send(bo);
                Console.WriteLine(message);
            }
            if (message.Contains("Fancy"))
                {
                var fancy = database.GetFancy();
                    string bo = $@"
<html>
<head>
<style>
{{background - color: blue;}}
h1 {{color: blue;}}
p {{color: LightSeaGreen;}}
a {{color: red;}}
</style>
</head>
<hr/>
<h1>Fancy List:</h1>
<h2> Click on a player to see their stats: </h2>
{fancy}
<html>";
                string highScoreHeader = BuildHTTPResponseHeader(bo.Length);
                channel.Send(highScoreHeader);
                channel.Send("");
                channel.Send(bo);
                Console.WriteLine(message);
            }
            else
            {
                string body = BuildHTTPBody();
                int bodyLength = body.Length;
                string header = BuildHTTPResponseHeader(bodyLength);

                channel.Send(header);
                channel.Send("");
                channel.Send(body);
                Console.WriteLine(message);
            }
        }


        /// <summary>
        ///    (1) Instruct the DB to seed itself (build tables, add data)
        ///    (2) Report to the web browser on the success
        /// </summary>
        /// <returns> the HTTP response header followed by some informative information</returns>
        private static string CreateDBTablesPage(string name)
        {
            throw new NotImplementedException();
        }

        internal static void OnDisconnect(Networking channel)
        {
            Debug.WriteLine($"Goodbye {channel.RemoteAddressPort}");
        }
      
    }
}
