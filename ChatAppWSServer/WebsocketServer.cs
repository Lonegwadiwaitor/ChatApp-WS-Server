using System;
using System.Net;
using ChatAppWSServer.Services;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace ChatAppWSServer
{
    public class WebsocketServer
    {
        private WebSocketServer _server;
        public WebsocketServer()
        {
            Console.WriteLine("Starting server...");
            
            _server = new WebSocketServer(IPAddress.Any, 5000, false);
            
            _server.AddWebSocketService<Connection>("/gateway");
            
            _server.Log.Level = LogLevel.Debug;

            _server.Start();

        }
    }
}