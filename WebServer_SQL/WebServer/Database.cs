namespace WebServer;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Author:  H. James de St. Germain
/// Date:    Spring 2020
/// Updated: Spring 2022
///          Spring 2023
/// 
/// Coding examples for connecting to and querying an SQL Database
/// 
/// https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=windows
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
        //connectionString = new SqlConnectionStringBuilder()
        //{
        //    DataSource = "cs3500.eng.utah.edu,14330",
        //    InitialCatalog = "cs3500",
        //    UserID = "lab",
        //    Password = "lab123456",
        //    ConnectTimeout = 15, // if the server doesn't connect in X seconds, give up
        //    Encrypt = false
        //}.ConnectionString;

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
}
