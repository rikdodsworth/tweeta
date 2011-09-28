using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WP7HelperFX.Common.Logging
{
    public enum Verbosity
    {
        _Disabled,
        ERROR,
        WARNING,
        INFO,
    }

    public static class Log
    {
        public static Verbosity Verbosity { get; set; }

        private static List<ILogger> subscribers = new List<ILogger>();

        static Log()
        {
            Verbosity = Verbosity.INFO;

#if DEBUG
            subscribers.Add(new DefaultLogger());
#endif
        }

        [Conditional("DEBUG")]
        public static void Initialize()
        {
            foreach (ILogger i in subscribers)
                i.Initialize();
        }

        [Conditional("DEBUG")]
        public static void DeInitialize()
        {
            foreach (ILogger i in subscribers)
                i.DeInitialize();
        }

        [Conditional("DEBUG")]
        public static void Error(string format, params object[] args)
        {
            Write(Verbosity.ERROR, format, args);
        }

        [Conditional("DEBUG")]
        public static void Warning(string format, params object[] args)
        {
            Write(Verbosity.WARNING, format, args);
        }

        [Conditional("DEBUG")]
        public static void Info(string format, params object[] args)
        {
            Write(Verbosity.INFO, format, args);
        }

        [Conditional("DEBUG")]
        private static void Write(Verbosity level, string format, params object[] args)
        {
            if (level > Verbosity)
                return;

            string msg = string.Format("{0}, {1,-8}:", DateTime.Now.ToLongTimeString(), level.ToString());

            if (args != null && args.Length > 0)
                msg += string.Format(format, args);
            else
                msg += format;

            foreach (ILogger i in subscribers)
                i.Write(msg);
        }
    }
}
