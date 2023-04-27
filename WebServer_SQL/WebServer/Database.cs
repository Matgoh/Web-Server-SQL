namespace WebServer;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Reflection.PortableExecutable;

/// <summary>
/// Author:  H. James de St. Germain, Matthew Goh, and Alex Qi
/// Date:    Spring 2023
/// 
/// 
/// This database class will establish a connection to the sql database with secure login as well as provide
/// methods to obtain data from the database. 
/// 
/// 
/// </summary>
public class Database
{
    /// <summary>
    /// The information necessary for the program to connect to the Database
    /// </summary>
    public readonly string connectionString;

    /// <summary>
    /// Upon construction of this static class, build the connection string
    /// </summary>
    public Database()
    {
        var builder = new ConfigurationBuilder();

        builder.AddUserSecrets<Database>();
        IConfigurationRoot Configuration = builder.Build();
        var SelectedSecrets = Configuration.GetSection("WebServerSecrets");

        connectionString = new SqlConnectionStringBuilder()
        {
            DataSource = SelectedSecrets["server_name"],
            InitialCatalog = SelectedSecrets["database_name"],
            UserID = SelectedSecrets["userID"],
            Password = SelectedSecrets["LabDBPassword"],
            ConnectTimeout = 15, // if the server doesn't connect in X seconds, give up
            Encrypt = false
        }.ConnectionString;
    }

    /// <summary>
    /// This method will return the name and the score of each person in html from the database ordered from highest mass eaten.
    /// </summary>
    /// <returns></returns>
    public string GetHighScore()
    {
        Console.WriteLine("Getting Connection ...");

        try
        {
            //create instance of database connection
            using SqlConnection con = new(connectionString);

            //
            // Open the SqlConnection.
            //
            con.Open();


            // Here we join the playerlist and the highscore list to get the names and the associated mass ordered by highest mass. 
            using SqlCommand command = new SqlCommand("SELECT HighScore.HighestMass, PlayerList.Name FROM HighScore" +
                                                       "\r\nINNER JOIN PlayerList ON HighScore.PlayerId=PlayerList." +
                                                       "PlayerId\r\nORDER BY HighestMass DESC;", con);
            using SqlDataReader reader = command.ExecuteReader();

            string result = "<ol>";

            while (reader.Read())
            {
                result += $@"<li>
                             {reader.GetInt32(0)}
                             {reader.GetString(1)}, 
                             </li>";
            }

            result += "</ol>";
            return result;
        }
        catch (SqlException exception)
        {
            Console.WriteLine($"Error in SQL connection: {exception.Message}");
            return "";
        }
    }

    /// <summary>
    /// This method will return the name of each player from the database. 
    /// </summary>
    /// <returns></returns>
    public string GetPlayers()
    {
        Console.WriteLine("Getting Connection ...");

        try
        {
            //create instance of database connection
            using SqlConnection con = new(connectionString);

            //
            // Open the SqlConnection.
            //
            con.Open();


            // Get the list of players
            using SqlCommand command = new SqlCommand("SELECT PlayerList.Name FROM PlayerList", con);
            using SqlDataReader reader = command.ExecuteReader();

            string result = "<ol>";

            while (reader.Read())
            {
                result += $@"<li>
                             {reader.GetString(0)}, 
                             </li>";
            }

            result += "</ol>";
            return result;
        }
        catch (SqlException exception)
        {
            Console.WriteLine($"Error in SQL connection: {exception.Message}");
            return "";
        }
    }

    /// <summary>
    /// Determine if specified player exists.
    /// </summary>
    /// <param name="name"></param>
    public void GetPlayerScore(string name)
    {
        Console.WriteLine("Getting Connection ...");

        try
        {
            //create instance of database connection
            using SqlConnection con = new(connectionString);

            //
            // Open the SqlConnection.
            //
            con.Open();


            // Get the list of players
            using SqlCommand command = new SqlCommand($"SELECT PlayerList.PlayerId IN PlayerList WHERE PLayerList.Name = '{name}'", con);
            command.ExecuteNonQuery();
        }
        catch (SqlException exception)
        {
            Console.WriteLine($"Player Not Found: {exception.Message}");            
        }
    }

    /// <summary>
    /// Determine if specified player exists.
    /// </summary>
    /// <param name="name"></param>
    public string GetPlayerData(string name)
    {
        Console.WriteLine("Getting Connection ...");

        try
        {
            //create instance of database connection
            using SqlConnection con = new(connectionString);

            //
            // Open the SqlConnection.
            //
            con.Open();


            // Get the list of players
            using SqlCommand command = new SqlCommand($"SELECT HighScore.HighestMass, IN PlayerList WHERE PLayerList.Name = '{name}'", con);
            using SqlDataReader reader = command.ExecuteReader();


            string result = "<ol>";

            while (reader.Read())
            {
                result += $@"<li>
                             {reader.GetString(0)}, 
                             </li>";
            }

            result += "</ol>";
            return result;
        }
        catch (SqlException exception)
        {
            Console.WriteLine($"error: {exception.Message}");
            return "";
        }
    }
}
