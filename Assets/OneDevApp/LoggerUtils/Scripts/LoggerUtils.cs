using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace OneDevApp
{
    /// <summary>
    /// Logger utility for logging custom error or log messages to
    /// console and file.
    /// </summary>
    public static class LoggerUtils
    {
        /// <summary>
        /// Controls the logs
        /// at one time.
        /// </summary>
        private static bool EnableLogginOnDevice = false;

        /// <summary>
        /// The file extension to use for log files.
        /// </summary>
        private const string LogFileExtension = "txt";

        /// <summary>
        /// The folder to save logs in.
        /// </summary>
        private const string LogFileDirectory = "AppLogs";

        /// <summary>
        /// The max number of log files allowed in the folder
        /// at one time.
        /// </summary>
        private const int MaxLogCount = 8;

        /// <summary>
        /// The collection of log messages to store.
        /// </summary>
        public static List<string> Messages { get; private set; }

        /// <summary>
        /// Controls how the logger operates.
        /// </summary>
        private static LogProfile profile;

       /* /// <summary>
        /// Controls how the logger operates.
        /// </summary>
        private static Destructor destructor;
        /// <summary>
        /// This is a little rigged contraption to allow for
        /// saving of the log file when the app is closed.
        /// Source: https://stackoverflow.com/questions/4364665/static-destructor
        /// </summary>
        private sealed class Destructor
        {
            ~Destructor()
            {
                if (profile.SaveToFile)
                {
                    SaveLogFile();
                }
            }
        }*/

        static LoggerUtils()
        {
            EnableLogginOnDevice = false;
            profile = LogProfile.Release;

            Messages = new List<string>();
           // destructor = new Destructor();
        }

        /// <summary>
        /// Set what profile the logger should follow.
        /// </summary>
        public static void SetLogProfile(LogProfile logProfile)
        {
            profile = logProfile;
        }

        /// <summary>
        /// Set what profile the logger should follow.
        /// </summary>
        public static void ToogleLogOnDevice(bool isEnable)
        {
            EnableLogginOnDevice = isEnable;
        }

        /// <summary>
        /// Log an error to console and to the log file.
        /// </summary>
        public static void LogError(string message)
        {
            if (!EnableLogginOnDevice) return;

            string fullError = DateTime.Now.ToString("h:mm:ss tt") + ": ERROR: " + message;
            Messages.Add(fullError);

            switch (profile.Output)
            {
                case LogOutput.Unity:
                    Debug.LogError(fullError);
                    break;
                case LogOutput.Console:
                    Console.WriteLine(fullError);
                    break;
            }
        }

        /// <summary>
        /// Log a warning to the console and add to the log file.
        /// </summary>
        public static void LogWarning(string message, LogLevel level = LogLevel.Normal)
        {
            if (!EnableLogginOnDevice) return;

            string fullWarn = DateTime.Now.ToString("h:mm:ss tt") + ": WARNING: " + message;
            Messages.Add(fullWarn);

            if (level <= profile.Level && level > 0)
            {
                switch (profile.Output)
                {
                    case LogOutput.Unity:
                        Debug.LogWarning(fullWarn);
                        break;
                    case LogOutput.Console:
                        Console.WriteLine(fullWarn);
                        break;
                }
            }
        }

        /// <summary>
        /// Log a message to console and to the log file.
        /// </summary>
        public static void Log(string message, LogLevel level = LogLevel.Normal)
        {
            return;
            //Debug.Log(message);
            if (!EnableLogginOnDevice) return;

            string fullLog = DateTime.Now.ToString("h:mm:ss tt") + ": " + message;
            Messages.Add(fullLog);

            if (level <= profile.Level && level > 0)
            {
                switch (profile.Output)
                {
                    case LogOutput.Unity:
                        Debug.Log(fullLog);
                        break;
                    case LogOutput.Console:
                        Console.WriteLine(fullLog);
                        break;
                }
            }
        }

        /// <summary>
        /// Store all of the log calls made to file.
        /// </summary>
        public static void SaveLogFile()
        {
            if (!profile.SaveToFile) return;

            Log("Saving log file");

            List<byte> logBytes = new List<byte>();

            //Add each message to the byte array and add a new line after each.
            foreach (string msg in Messages)
            {
                logBytes.AddRange(Encoding.ASCII.GetBytes(msg));
                logBytes.AddRange(Encoding.ASCII.GetBytes(Environment.NewLine));
            }

            string logFullDirectory = Application.persistentDataPath + Path.DirectorySeparatorChar + LogFileDirectory;

            //Don't save more than 8 files at any time.
            while (FileUtils.GetFileCount(logFullDirectory) >= MaxLogCount)
            {
                FileUtils.DeleteFileAtIndex(logFullDirectory, 0);
            }

            //Save the file.
            string fullLogFileName = "AppLogFile" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "." + LogFileExtension;


            Debug.Log("fullLogFileName::"+ fullLogFileName);
            FileUtils.SaveFile(logFullDirectory, fullLogFileName, logBytes.ToArray(), false);
        }
    }
}