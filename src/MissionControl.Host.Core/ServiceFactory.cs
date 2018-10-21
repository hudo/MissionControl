using System;
using System.Collections.Generic;

namespace MissionControl.Host.Core
{
    public delegate object ServiceFactory(Type type);

    internal static class ServiceFactoryExtensions
    {
        public static T GetInstance<T>(this ServiceFactory factory, Type type) => (T) factory(type);

        public static IEnumerable<T> GetIntances<T>(this ServiceFactory factory, Type type) => (IEnumerable<T>) factory(typeof(IEnumerable<T>));
    }
}
