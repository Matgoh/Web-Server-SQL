﻿namespace WebServer;

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
    public void getPlayers()
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

            //
            // This code uses an SqlCommand based on the SqlConnection.
            //
            using SqlCommand command = new SqlCommand("SELECT * FROM Player", con);
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine("{0} {1}",
                    reader.GetInt32(0), reader.GetString(1));
            }

            Console.WriteLine($"Successful SQL connection");
        }
        catch (SqlException exception)
        {
            Console.WriteLine($"Error in SQL connection: {exception.Message}");
        }
    }
}
