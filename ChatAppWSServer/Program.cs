using System;
using System.ComponentModel.Design;
using System.Threading;
using ChatAppWSServer;
using ChatAppWSServer.PrivateServices;
using Microsoft.Extensions.DependencyInjection;
using WebSocketSharp;
using WebSocketSharp.Server;

static void Main()
{
    Globals._provider = new ServiceCollection()
        .AddSingleton(new DatabaseService())
        .AddSingleton(new RandomService())
        .AddSingleton(new WebsocketServer())
        .BuildServiceProvider();

    Thread.Sleep(-1);
}

Main();