using System;

namespace MissionControl.Host.Core.Utilities
{
    internal static class Guard
    {
        public static T NotNull<T>(T source, string argumentName)
        {
            if (source == null)
                throw new ArgumentNullException(argumentName);

            return source;
        }
    }
}