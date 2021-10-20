using System;
using Microsoft.Extensions.DependencyInjection;

namespace ChatAppWSServer
{
    public class Globals
    {
        public static IServiceProvider _provider;

        public static T GetService<T>() =>
            _provider.GetService<T>();

    }
}