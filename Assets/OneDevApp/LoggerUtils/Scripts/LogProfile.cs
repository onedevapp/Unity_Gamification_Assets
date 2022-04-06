using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwipeWire
{
    /// <summary>
    /// Contains info regarding how the Logger
    /// should operate. Contains some predefined
    /// profiles to simplify the process.
    /// </summary>
    public struct LogProfile
    {

        #region Static Declaration
        /// <summary>
        /// The default logging profile for when running in unity debug.
        /// </summary>
        public static readonly LogProfile UnityDebug = new LogProfile(LogLevel.Verbose, LogOutput.Unity);

        /// <summary>
        /// Used for debugging the server.
        /// </summary>
        public static readonly LogProfile ConsoleDebug = new LogProfile(LogLevel.Verbose, LogOutput.Console);

        /// <summary>
        /// Used for release builds.
        /// </summary>
        public static readonly LogProfile Release = new LogProfile(LogLevel.None, LogOutput.FileOnly, true);
        #endregion

        #region Variables Declaration

        /// <summary>
        /// Allows for different levels of log statements
        /// to be picked. If verbose, then every log statement
        /// is printed.
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// What method should be used for outputting
        /// log statements.
        /// </summary>
        public LogOutput Output { get; set; }

        /// <summary>
        /// If the log history should be saved to file when
        /// the application closes.
        /// </summary>
        public bool SaveToFile { get; set; }
        #endregion

        #region Constructor(s)
        /// <summary>
        /// Create a new log profile that doesn't
        /// save to file.
        /// </summary>
        public LogProfile(LogLevel level, LogOutput output)
        {
            Level = level;
            Output = output;
            SaveToFile = false;
        }

        /// <summary>
        /// Create a new log profile that can save to file.
        /// </summary>
        public LogProfile(LogLevel level, LogOutput output, bool saveFile)
        {
            Level = level;
            Output = output;
            SaveToFile = saveFile;
        }
        #endregion

    }
}