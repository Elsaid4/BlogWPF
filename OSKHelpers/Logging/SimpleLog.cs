using OSKHelpers.INIFile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace OSKHelpers.Logging
{
    public class SimpleLog
    {
        #region Membri

        private static readonly SimpleLogger _logger;

        #endregion

        #region Proprietà

        /// <inheritdoc cref="SimpleLogger.DefaultLogPath"/>
        public static string DefaultLogPath => SimpleLogger.DefaultLogPath;

        /// <inheritdoc cref="SimpleLogger.DisableDebugLog"/>
        public static bool DisableDebugLog { get => _logger.DisableDebugLog; set => _logger.DisableDebugLog = value; }

        /// <inheritdoc cref="SimpleLogger.LogLevel"/>
        public static LogLevel LogLevel { get => _logger.LogLevel; set => _logger.LogLevel = value; }

        /// <inheritdoc cref="SimpleLogger.LogPath"/>
        public static string LogPath { get => _logger.LogPath; set => _logger.LogPath = value; }

        /// <inheritdoc cref="SimpleLogger.Prefix"/>
        public static string Prefix { get => _logger.Prefix; set => _logger.Prefix = value; }

        /// <inheritdoc cref="SimpleLogger.LogFile"/>
        public static string LogFile => _logger.LogFile;

        /// <inheritdoc cref="SimpleLogger.ForceDebug"/>
        public static bool ForceDebug { get => _logger.ForceDebug; set => _logger.ForceDebug = value; }

        /// <inheritdoc cref="SimpleLogger.ForceProtocol"/>
        public static bool ForceProtocol { get => _logger.ForceProtocol; set => _logger.ForceProtocol = value; }

        /// <inheritdoc cref="SimpleLogger.LogLevelDebug"/>
        public static bool LogLevelDebug => _logger.LogLevelDebug;

        /// <inheritdoc cref="SimpleLogger.LogLevelProtocol"/>
        public static bool LogLevelProtocol => _logger.LogLevelProtocol;

        #endregion

        #region Costruttore

        static SimpleLog()
        {
            _logger         = new SimpleLogger();
            _logger.LogPath = DefaultLogPath;
            LogLevel        = LogLevel.Error;
#if DEBUG
            LogLevel        = LogLevel.Debug;
#endif
        }

        #endregion

        #region Metodi

        /// <inheritdoc cref="SimpleLogger.Write(string, LogLevel)"/>
        public static void Write(string line, LogLevel logLevel = LogLevel.None) 
            => _logger.Write(line, LogLevel);

        /// <inheritdoc cref="SimpleLogger.Write(LogLevel, string, object, bool)"/>
        public static void Write(LogLevel logLevel, string line, object obj = null, bool inline = false) 
            => _logger.Write(logLevel, line, obj, inline);

        /// <inheritdoc cref="SimpleLogger.Write(LogLevel, object)"/>
        public static void Write(LogLevel logLevel, object obj)
            => _logger.Write(logLevel, obj);

        ///<inheritdoc cref="SimpleLogger.WriteLine(string)"/>
        public static void WriteLine(string line) 
            => _logger.WriteLine(line);

        /// <inheritdoc cref="SimpleLogger.DebugWrite(string, object, bool)"/>
        public static void DebugWrite(string line, object obj = null, bool inline = false) 
            => _logger.DebugWrite(line, obj, inline);

        /// <inheritdoc cref="SimpleLogger.DebugWrite(object)"/>
        public static void DebugWrite(object obj) 
            => _logger.DebugWrite(obj);

        /// <inheritdoc cref="SimpleLogger.ProtocolWrite(string, object, bool)"/>
        public static void ProtocolWrite(string line, object obj = null, bool inline = false) =>
            _logger.ProtocolWrite(line, obj, inline);

        /// <inheritdoc cref="SimpleLogger.ProtocolWrite(object)"/>
        public static void ProtocolWrite(object obj) 
            => _logger.ProtocolWrite(obj);

        /// <inheritdoc cref="SimpleLogger.LogLines(IEnumerable{string})"/>
        public static void LogLines(IEnumerable<string> lines)
            => _logger.LogLines(lines);

        /// <inheritdoc cref="SimpleLogger.LogLines(LogLevel, IEnumerable{string})"/>
        public static void LogLines(LogLevel logLevel, IEnumerable<string> lines)
            => _logger.LogLines(logLevel, lines);

        /// <inheritdoc cref="SimpleLogger.LogArray(LogLevel, string, string, IEnumerable{object}, bool)"/>
        public static void LogArray(LogLevel logLevel, string message, string arrayName, IEnumerable<object> array, bool serialize = false)
            => _logger.LogArray(logLevel, message, logLevel, arrayName, array, serialize);

        /// <inheritdoc cref="SimpleLogger.LogArray(LogLevel, string, LogLevel, string, IEnumerable{object}, bool)"/>
        public static void LogArray(LogLevel logLevel, string message, LogLevel detailLogLevel, string arrayName, IEnumerable<object> array, bool serialize = false)
            => _logger.LogArray(logLevel, message, detailLogLevel, arrayName, array, serialize);

        /// <inheritdoc cref="SimpleLogger.FormattedException(Exception, bool)"/>
        public static string FormattedException(Exception ex, bool includeStackTrace = false) 
            => _logger.FormattedException(ex, includeStackTrace);

        /// <inheritdoc cref="SimpleLogger.LogError(Exception, bool, string)"/>
        public static void LogError(Exception ex, bool includeStackTrace = false, [CallerMemberName] string callerMethodName = "")
        {
            var callerTypeName = new StackFrame(1).GetMethod().DeclaringType.Name;
            _logger.LogError(ex, callerTypeName, callerMethodName, includeStackTrace);
        }

        /// <inheritdoc cref="SimpleLogger.LogError(Exception, string, string, bool)"/>
        public static void LogError(Exception ex, string callerTypeName, string callerMethodName, bool includeStackTrace = false)
            => _logger.LogError(ex, callerTypeName, callerMethodName, includeStackTrace);

        /// <inheritdoc cref="SimpleLogger.GetCallerTypeName"/>
        public static string GetCallerTypeName() => new StackFrame(1).GetMethod().DeclaringType.Name;

        /// <inheritdoc cref="SimpleLogger.GetCallerMethodName(string)"/>
        public static string GetCallerMethodName([CallerMemberName] string callerMethodName = "") => callerMethodName;

        /// <inheritdoc cref="SimpleLogger.GetCallerTypeMethodName(string)"/>
        public static string GetCallerTypeMethodName([CallerMemberName] string callerMethodName = "") => $"{new StackFrame(1).GetMethod().DeclaringType.Name}.{callerMethodName}";

        ///<inheritdoc cref="SimpleLogger.LogCallerTypeMethodName(LogLevel, string)"/>
        public static void LogCallerTypeMethodName(LogLevel logLevel = LogLevel.Debug, [CallerMemberName] string callerMethodName = "") => Write(logLevel, $"{new StackFrame(1).GetMethod().DeclaringType.Name}.{callerMethodName}");

        ///<inheritdoc cref="SimpleLogger.DebugConsoleLog(string)"/>
        public static void DebugConsoleLog(string line)
            => _logger?.DebugConsoleLog(line);

        /// <inheritdoc cref="SimpleLogger.SetLogLevel(IniFileHelper)"/>
        public static void SetLogLevel(IniFileHelper iniFile)
            => _logger?.SetLogLevel(iniFile);

        #endregion

    }

}
