using OSKHelpers.INIFile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OSKHelpers.Logging
{
    public class SimpleLogger
    {
        #region Membri

        private DateTime? _lastDate;

        private static readonly string _defaultLogPath;
        private string _logPath;
        private string _logFilename;

        private LogLevel _logLevel;
        private bool _forceDebug;
        private bool _forceProtocol;
        
        private string _prefix;

        private readonly object _lock = new object();

        #endregion

        #region Proprietà

        /// <summary>
        /// Cartella log di default
        /// </summary>
        public static string DefaultLogPath { get => _defaultLogPath; }

        /// <summary>
        /// Utilizzato per disabilitare le chiamate in fase di debug, non tocca quel che 
        /// è il normale funzionamento del livello LogLevel.Debug
        /// </summary>
        public bool DisableDebugLog { get; set; }

        /// <summary>
        /// Livello minimo di log per i messaggi che saranno registrati.<br/>
        /// Valore di default: Loglevel.Error
        /// </summary>
        public LogLevel LogLevel 
        { 
            get => _logLevel; 
            set
            {
                if (_forceProtocol || value == LogLevel.Protocol)
                    _logLevel = LogLevel.Protocol;
                else if (_forceDebug || value == LogLevel.Debug)
                    _logLevel = LogLevel.Debug;
                else
                    _logLevel = value;
            }
        }

        /// <summary>
        /// Percorso del file di log.<br/>
        /// La presenza della cartella viene verificato al momento della scrittura di un messaggio, se il percorso non viene trovato se ne tenta la creazione.<br />
        /// Di default viene utilizzata la cartella "Log" all'interno del percorso dell'applicativo.
        /// </summary>
        public string LogPath
        {
            get => _logPath;
            set
            {
                _logPath = value;
                // _lastDate viene posto a null per forzare la rigenerazione del nome file
                _lastDate = null;
            }
        }

        /// <summary>
        /// Prefisso da aggiungere al nome del file log.<br/>
        /// Valore di default: Null.
        /// </summary>
        public string Prefix
        {
            get => _prefix;
            set
            {
                _prefix = value;
                // _lastDate viene posto a null per forzare la rigenerazione del nome file
                _lastDate = null;
            }
        }

        /// <summary>
        /// Nome del file di log.<br/>
        /// Il nome è composto secondo il pattern PREFIXLogYYYYMMDD.txt, è possibile differenziare i vari log all'interno della stessa cartella modificando Prefix
        /// </summary>
        public string LogFile
        {
            get
            {
                if (DateTime.Now.Date != (_lastDate?.Date ?? DateTime.MinValue))
                {
                    _logFilename = Path.Combine(_logPath, $"{_prefix}Log{DateTime.Now:yyyyMMdd}.txt");
                    _lastDate = DateTime.Now.Date;
                }

                return _logFilename;
            }
        }

        /// <summary>
        /// Forza il livello di log a Debug.<br/> 
        /// Utile per attivare il debug già in fase di inizializzazione del programma (quando ancora non è stato caricato il file delle impostazioni),<br/>
        /// viene ignorato nel caso in cui sia True anche ForceProtocol.
        /// </summary>
        public bool ForceDebug
        {
            get => _forceDebug;
            set
            {
                _forceDebug = value;
                if (_forceDebug)
                {
                    LogLevel = LogLevel.Debug;
                }
            }
        }

        /// <summary>
        /// Forza il livello di log a Debug.<br/> 
        /// Utile per attivare il debug già in fase di inizializzazione del programma (quando ancora non è stato caricato il file delle impostazioni),<br/>
        /// se True il valore di ForceDebug viene ignorato.
        /// </summary>
        public bool ForceProtocol
        {
            get => _forceProtocol;
            set
            {
                _forceProtocol = value;
                if (_forceProtocol)
                {
                    LogLevel = LogLevel.Protocol;
                }
            }
        }

        /// <summary>
        /// True se il livello di log è uguale o superiore a Debug
        /// </summary>
        public bool LogLevelDebug => LogLevel >= LogLevel.Debug;

        /// <summary>
        /// True se il livello di log è uguale o superiore a Protocol
        /// </summary>
        public bool LogLevelProtocol => LogLevel >= LogLevel.Protocol;

        #endregion

        #region Costruttore

        static SimpleLogger()
        {
            _defaultLogPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Logs");
        }

        public SimpleLogger()
        {
            DisableDebugLog = false;
            _lastDate       = null;
            LogPath         = _defaultLogPath;
            _logFilename    = null;
            _forceDebug     = false;
            _forceProtocol  = false;

            LogLevel        = LogLevel.Error;
#if DEBUG
            LogLevel        = LogLevel.Debug;
#endif
        }

        #endregion

        #region Metodi

        /// <summary>
        /// Scrive la linea passata come parametro nel file di log e, di default, la visualizza a video quando in fase di debug
        /// </summary>
        /// <param name="line">Linea da scrivere nel log</param>
        /// <param name="logLevel">Utilizzato per formattare correttamente il prefisso nel caso in cui il livello di log sia DEBUG o PROTOCOL,<br/>
        /// non impedisce in alcun caso la scrittura della riga. Se omesso viene utilizzato il prefisso standard (data/ora)</param>
        public void Write(string line, LogLevel logLevel = LogLevel.None)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                lock (_lock)
                {
                    var prefix = GetLogLinePrefix(LogLevel);
                    try
                    {
#if DEBUG
                        DebugConsoleLog(line);
#endif
                        if (!Directory.Exists(_logPath))
                        {
                            Directory.CreateDirectory(_logPath);
                        }
                        File.AppendAllText(LogFile, $"{prefix} {line}\r\n");
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            // Se si verificano errori in fase di scrittura viene creato un log all'interno della cartella dell'applicazione, se possibile.
                            line = $"Errore in fase di scrittura del log.\r\n  Nome del file di log: {LogFile}\r\n  Errore: {FormattedException(ex, true)}\r\n  Messaggio originale: {line}";
                            File.AppendAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ERRORSLOG.txt"), $"{prefix} {line}\r\n");
                        }
                        catch { }
                    }
                }
            }
        }

        /// <summary>
        /// Effettua il log della linea con eventiale serializzazione di un oggetto se il livello di log è compatibile
        /// </summary>
        /// <param name="logLevel">Livello di log minimo</param>
        /// <param name="line">Testo del log</param>
        /// <param name="obj">Eventuale oggetto da serializzare</param>
        /// <param name="inline">Se True la serializzazione dell'oggetto viene salvata di seguito al testo, altrimenti viene salvata in una nuova riga.</param>
        public void Write(LogLevel logLevel, string line, object obj = null, bool inline = false)
        {
            if (logLevel <= LogLevel)
            {
                if (obj == null)
                {
                    Write(line, logLevel);
                }
                else
                {
                    if (!inline)
                    {
                        Write(line, logLevel);
                        Write(JsonSerializer.Serialize(obj));
                    }
                    else
                    {
                        try
                        {
                            Write($"{line} {JsonSerializer.Serialize(obj)}".Trim(), logLevel);
                        }
                        catch (Exception ex)
                        {
                            Write($"Errore in {nameof(SimpleLogger)}.{nameof(Write)}(LogLevel.{logLevel},'{line}', obj, {inline}): {SimpleLog.FormattedException(ex, true)}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Effettua il log della serializzazione dell'oggetto passato come parametro.
        /// </summary>
        public void Write(LogLevel logLevel, object obj) 
            => Write(logLevel, string.Empty, obj, true);

        /// <summary>
        /// Scrive una riga.<br/>
        /// Metodo creato per poter essere utilizzato, ad esempio, per il logging di EF.<br/>
        /// Scrive la linea passata come parametro nel file di log e, di default, la visualizza a video quando in fase di debug
        /// </summary>
        /// <param name="line"></param>
        public void WriteLine(string line) => Write(line);

        /// <summary>
        /// Effettua il log della linea con eventiale serializzazione di un oggetto se il livello di log è almeno Debug
        /// </summary>
        /// <param name="line">Testo del log</param>
        /// <param name="obj">Eventuale oggetto da serializzare</param>
        /// <param name="inline">Se True la serializzazione dell'oggetto viene salvata di seguito al testo, altrimenti viene salvata in una nuova riga.</param>
        public void DebugWrite(string line, object obj = null, bool inline = false) 
            => Write(LogLevel.Debug, line, obj, inline);

        ///<inheritdoc cref="DebugWrite(string, object, bool)"/>
        public void DebugWrite(object obj)
            => DebugWrite(string.Empty, obj, true);

        /// <summary>
        /// Effettua il log della linea con eventiale serializzazione di un oggetto se il livello di log è almeno Protocol
        /// </summary>
        /// <param name="line">Testo del log</param>
        /// <param name="obj">Eventuale oggetto da serializzare</param>
        /// <param name="inline">Se True la serializzazione dell'oggetto viene salvata di seguito al testo, altrimenti viene salvata in una nuova riga.</param>
        public void ProtocolWrite(string line, object obj = null, bool inline = false) 
            => Write(LogLevel.Protocol, line, obj, inline);

        /// <summary>
        /// Effettua il log della serializzazione dell'oggetto passato come parametro.
        /// </summary>
        public void ProtocolWrite(object obj) 
            => ProtocolWrite(string.Empty, obj, true);

        ///<inheritdoc cref="LogLines(LogLevel, IEnumerable{string})"/>
        public void LogLines(IEnumerable<string> lines)
            => LogLines(LogLevel.None, lines);

        /// <summary>
        /// Scrive sul log le stringhe passate come parametro se il livello di log è compatibile.
        /// </summary>
        /// <param name="logLevel">Livello di log minimo</param>
        /// <param name="lines">Linee di cui effettuare il log</param>
        public void LogLines(LogLevel logLevel, IEnumerable<string> lines)
        {
            try
            {
                if (logLevel < LogLevel && (lines?.Any() ?? false))
                {
                    string prefix = GetLogLinePrefix(logLevel);

                    lock (_lock)
                    {
                        if (!Directory.Exists(_logPath))
                        {
                            Directory.CreateDirectory(_logPath);
                        }
                        File.AppendAllLines(LogFile, lines.Select(l => $"{prefix} {l}"));
                    }
                }
            }
            catch (Exception ex)
            {
                Write($"Errore in {nameof(SimpleLogger)}.{nameof(LogArray)}: {FormattedException(ex)}");
            }
        }

        ///<inheritdoc cref="LogArray(LogLevel, string, LogLevel, string, IEnumerable{object}, bool)"/>
        public void LogArray(LogLevel logLevel, string message, string arrayName, IEnumerable<object> array, bool serialize = false)
            => LogArray(logLevel, message, logLevel, arrayName, array, serialize);

        /// <summary>
        /// Effettua il log dell'array opzionalmente registrando il contenuto di tutti gli elementi se il livello di log è compatibile con detailLogLevel.
        /// </summary>
        /// <param name="logLevel">Livello di log minimo</param>
        /// <param name="message">Testo del log</param>
        /// <param name="detailLogLevel">Livello di log minimo per registrare il contenuto dell'array.</param>
        /// <param name="arrayName">Nome dell'array</param>
        /// <param name="array">Array di cui effettuare il log</param>
        /// <param name="serialize">Richiede la serializzazione di ogni elemento</param>
        public void LogArray(LogLevel logLevel, string message, LogLevel detailLogLevel, string arrayName, IEnumerable<object> array, bool serialize = false)
        {
            if (logLevel <= LogLevel && array != null && (array?.Any() ?? false))
            {
                var prefix = GetLogLinePrefix(logLevel);

                var lines = new List<string>();
                try
                {
                    if (!string.IsNullOrWhiteSpace(message))
                    {
#if DEBUG
                        DebugConsoleLog(message);
#endif
                        lines.Add($"{prefix} {message}");
                    }
                    if (detailLogLevel <= LogLevel)
                    {
                        prefix = GetLogLinePrefix(detailLogLevel);

                        var i = 0;
                        foreach (var element in array)
                        {
                            lines.Add($"{prefix} {arrayName}[{i}]: {(serialize ? JsonSerializer.Serialize(element) : element)}");
                            i++;
                        }
                    }
                    lock (_lock)
                    {
                        if (!Directory.Exists(_logPath))
                        {
                            Directory.CreateDirectory(_logPath);
                        }
                        File.AppendAllLines(LogFile, lines);
                    }
                }
                catch (Exception ex)
                {
                    Write($"Errore in {nameof(SimpleLogger)}.{nameof(LogArray)}: {FormattedException(ex)}");
                }
            }
        }

        /// <summary>
        /// Restituisce una stringa standard per la visualizzazione ed il logging delle eccezioni e dell inner exception
        /// </summary>
        /// <param name="ex">Eccezione di cui effettuare il log</param>
        /// <param name="includeStackTrace">Indica se forzare l'inclusione del trace dello stack (con livello log Debug o superiore lo stack viene sempre incluso)</param>
        public string FormattedException(Exception ex, bool includeStackTrace = false)
        {
            includeStackTrace = includeStackTrace || LogLevelDebug;
            string formattedException = string.Empty;
            if (ex != null)
            {
                formattedException = $"{ex.GetType()} {ex.Message}"
                    + (ex.InnerException != null ? $"\r\n{ex.InnerException.GetType()} {ex.InnerException.Message}" : string.Empty)
                    + (includeStackTrace ? $"\r\nStack: {ex.StackTrace}" : string.Empty);
            }
            return formattedException;
        }

        /// <summary>
        /// Effettua il log dell'eccezione passata come parametro con un messaggio tipo "Errore in Classe.Metodo: Eccezione".<br/>
        /// Se viene passata un'eccezione nulla viene comunque registrato il messaggio d'errore.<br/>
        /// Il log viene effettuato per un livello di logging LogLevel.Error o superiore
        /// </summary>
        /// <param name="ex">Eccezione di cui effettuare il log</param>
        /// <param name="includeStackTrace">Indica se forzare l'inclusione del trace dello stack (con livello log Debug o superiore lo stack viene sempre incluso)</param>
        public void LogError(Exception ex, bool includeStackTrace = false, [CallerMemberName] string callerMethodName = "")
        {
            var callerTypeName = new StackFrame(1).GetMethod().DeclaringType.Name;
            LogError(ex, callerTypeName, callerMethodName, includeStackTrace);
        }

        /// <summary>
        /// Effettua il log dell'eccezione passata come parametro con un messaggio tipo "Errore in Classe.Metodo: Eccezione".<br/>
        /// Se viene passata un'eccezione nulla viene comunque registrato il messaggio d'errore.<br/>
        /// Il log viene effettuato per un livello di logging LogLevel.Error o superiore
        /// </summary>
        /// <param name="ex">Eccezione di cui effettuare il log</param>
        /// <param name="callerTypeName">Nome del tipo chiamante</param>
        /// <param name="callerMethodName">Nome del metodo chiamante</param>
        /// <param name="includeStackTrace">Indica se forzare l'inclusione del trace dello stack (con livello log Debug o superiore lo stack viene sempre incluso)</param>
        public void LogError(Exception ex, string callerTypeName, string callerMethodName , bool includeStackTrace = false)
        {
            Write(LogLevel.Error, $"Errore in {callerTypeName}.{callerMethodName}: {FormattedException(ex, includeStackTrace)}");
        }

        /// <summary>
        /// Restituisce il nome del tipo chiamante
        /// </summary>
        public string GetCallerTypeName() => new StackFrame(1).GetMethod().DeclaringType.Name;

        /// <summary>
        /// Restituisce il nome del metodo chiamante
        /// </summary>
        public string GetCallerMethodName([CallerMemberName] string callerMethodName = "") => callerMethodName;

        /// <summary>
        /// Restituisce il nome del tipo e metodo chiamanti nella forma ClassName.MethodName
        /// </summary>
        public string GetCallerTypeMethodName([CallerMemberName] string callerMethodName = "") => $"{new StackFrame(1).GetMethod().DeclaringType.Name}.{callerMethodName}";

        /// <summary>
        /// Effettua il log della classe e metodo chiamanti se il livello di log è adeguato.
        /// </summary>
        public void LogCallerTypeMethodName(LogLevel logLevel = LogLevel.Debug, [CallerMemberName] string callerMethodName = "") => Write(logLevel, $"{new StackFrame(1).GetMethod().DeclaringType.Name}.{callerMethodName}");

        /// <summary>
        /// Visualizza a video la lina passata come parametro in fase di debug, altrimenti non fa niente
        /// </summary>
        /// <param name="line"></param>
        public void DebugConsoleLog(string line)
        {
#if DEBUG
            if (!DisableDebugLog && Environment.UserInteractive)
            {
                Console.WriteLine($"{line}");
            }
#endif
        }

        /// <summary>
        /// Recupera il livello di log dalla chiave "LogLevel" nell'oggetto IniFileHelper passato come parametro .
        /// Se la chiave non esiste o ha un valore on valido viene impostato il valore di default (Error).
        /// In fase di Debug il livello sarà sempre impostato minimo a Debug
        /// </summary>
        /// <param name="iniFile">Oggetto IniFile da cui recuperare LogLevel</param>
        public void SetLogLevel(IniFileHelper iniFile)
        {
            var logLevel = LogLevel.Error;

            if (iniFile != null)
            {
                if (iniFile.HasKey(nameof(LogLevel)))
                {
                    var iniLogLevel = iniFile.GetInt(nameof(LogLevel));
                    if (Enum.IsDefined(typeof(LogLevel), iniLogLevel))
                    {
                        logLevel = (LogLevel)iniLogLevel;
                    }
                }
            }
#if DEBUG
            if (logLevel != LogLevel.Debug && logLevel != LogLevel.Protocol)
            {
                logLevel = LogLevel.Debug;
            }
#endif
            LogLevel = logLevel;
        }

        /// <summary>
        /// Restituisce il prefisso per le stringhe di log:<br/>
        /// - Data in formato dd/MM/yyyy<br/>
        /// - Eventuale descrizione per i livelli di log DEBUG o PROTOCOL<br/>
        /// Il prefisso non include lo spazio finale
        /// </summary>
        /// <param name="logLevel">LogLevel per generare il prefisso</param>
        private string GetLogLinePrefix(LogLevel logLevel = LogLevel.None) 
        {
            string prefix = $"{DateTime.Now:dd/MM/yyyy HH:mm:ss}";
            if (logLevel == LogLevel.Debug)
                prefix = $"{prefix} DEBUG";
            else if (logLevel == LogLevel.Protocol)
                prefix = $"{prefix} PROTOCOL";
            return prefix;
        }

        #endregion

    }

}
