using System;
using System.Collections.Generic;
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
    }

    public static class Log
    {
        private static ILogger _logger = new DefaultLogger();

        public static void SetLogger(ILogger provider)
        {
            _logger = provider;
        }

        public static void Info(string text)
        {
            _logger.LogInfo(text);
        }

        public static void Warning(string text)
        {
            _logger.LogWarning(text);
        }

        public static void Error(string text)
        {
            _logger.LogError(text);
        }

        public static void Graph(string chart, float value, Colour color)
        {
            _logger.LogGraph(chart, value, color);
        }

        public static void Break(string text)
        {
            _logger.LogBreak(text);
        }
    }
}
