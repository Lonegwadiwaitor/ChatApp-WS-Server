using System;
using System.Collections.Generic;
using ChatAppWSServer.Models.Serverside;
using Microsoft.Extensions.DependencyInjection;

namespace ChatAppWSServer
{
    public class Globals
    {
        public static IServiceProvider _provider;

        public static T GetService<T>() =>
            _provider.GetService<T>();

        public static List<ConnectedUser> ConnectedUsers = new List<ConnectedUser>();

    }
}