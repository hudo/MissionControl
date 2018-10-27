using System;

namespace MissionControl.Host.Core.Utilities
{
    internal static class Extensions
    {
        public static Exception Unwrap(this Exception e)
        {
            while (e.InnerException != null)
            {
                e = e.InnerException;
            }

            return e;
        }

        public static bool HasContent(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }
    }
}