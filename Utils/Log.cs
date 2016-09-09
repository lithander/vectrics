using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Vectrics.Utils
{
    public interface ILogger
    {
        void LogInfo(string text);
        void LogWarning(string text);
        void LogBreak(string text);
        void LogError(string text);
        void LogGraph(string chart, float value, Colour color);
        void LogException(Exception ex);
    }

    public class DefaultLogger : ILogger
    {
        public void LogInfo(string text)
        {
            Console.WriteLine("[INFO] " + text);
        }

        public void LogWarning(string text)
        {
            Console.WriteLine("[WARNING] " + text);
        }

        public void LogBreak(string text)
        {
            Console.WriteLine("[BREAK] " + text);
        }

        public void LogError(string text)
        {
            Console.WriteLine("[ERROR] " + text);
        }

        public void LogGraph(string chart, float value, Colour color)
        {
            Console.WriteLine("[GRAPH] " + chart + ": " + value);
        }

        public void LogException(Exception ex)
        {
            Console.WriteLine("[EXCEPTION] " + ex.Message + ": " + ex.ToString());
        }
    }

    public static class Log
    {
        private static ILogger _logger = new DefaultLogger();

        public static bool Enabled = true;

        public static void SetLogger(ILogger provider)
        {
            _logger = provider;
        }

        public static void Assert(bool condition, string message = "<?>")
        {
            if (!Enabled) return;
            if (!condition)
            {
                StackTrace trace = new StackTrace();
                _logger.LogError("ASSERTION FAILED: " + message + " : " + trace.ToString());
            }
        }

        public static void Info(string text)
        {
            if (!Enabled) return;
            _logger.LogInfo(text);
        }

        public static void Warning(string text)
        {
            if (!Enabled) return;
            _logger.LogWarning(text);
        }

        public static void Error(string text)
        {
            if (!Enabled) return;
            _logger.LogError(text);
        }

        public static void Graph(string chart, float value, Colour color)
        {
            if (!Enabled) return;
            _logger.LogGraph(chart, value, color);
        }

        public static void Break(string text)
        {
            if (!Enabled) return;
            _logger.LogBreak(text);
        }

        public static void Exception(Exception ex)
        {
            if (!Enabled) return;
            _logger.LogException(ex);
        }
    }
}
