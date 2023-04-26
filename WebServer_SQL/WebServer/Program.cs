using Communications;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServer;

Console.WriteLine("Web Server Running");
Networking server = new(NullLogger.Instance, OnClientConnect, OnDisconnect, onMessage, '\n');
server.WaitForClients(11001, true);
Console.ReadLine();

void onMessage(Networking channel, string message)
{
    Web.onMessage(channel, message);
}

void OnDisconnect(Networking channel)
{
    //Disconnect
}

void OnClientConnect(Networking channel)
{
    Web.OnClientConnect(channel);
}

