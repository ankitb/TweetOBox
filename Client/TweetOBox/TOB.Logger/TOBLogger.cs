using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace TOB.Logger
{
    public class TOBLogger
    {
        private static TextWriter LogStream = null;
        private const string TOBLOGPREFIX = "TOBLog_";
        private const int TOBLOGGERFRAMESKIP = 3;
        private static string LogDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Logs\\";
        private static string InfoFlagFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "verbose.flag";
        private static bool LogInfo = false;

        private enum TOBLogMessageType
        {
            Info,
            Debug,
            Assert
        };

        static TOBLogger()
        {
            // Create TextStream object for logging and use AutoFlush?? option to avoid losing info during crashes.
            // TODO - Add better performance handling around cache size when executing for extended periods
            if (LogStream == null)
            {
                if (!Directory.Exists(LogDir))
                {
                    Directory.CreateDirectory(LogDir);
                }

                if (!File.Exists(InfoFlagFile))
                    LogInfo = true;

                LogStream = (TextWriter) File.CreateText(LogDir + TOBLOGPREFIX + DateTime.Now.ToFileTime().ToString() + ".txt");
            }

        }
        // function to find object exist or not
        public static bool Assert(Object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else
                return true;
        }

        public static void Assert(bool isAssert, string assertionText, params object[] args)
        {
            if (isAssert)
            {
                InternalWrite(TOBLogMessageType.Assert, string.Format(assertionText, args));
            }

            return;
        }

        public static void WriteInfo(string info, params object[] args)
        {
            InternalWrite(TOBLogMessageType.Info, string.Format(info, args));

            return;
        }

        public static void WriteDebugInfo(string info, params object[] args)
        {
            InternalWrite(TOBLogMessageType.Debug, string.Format(info, args));

            return;
        }

        private static void InternalWrite(TOBLogMessageType messageType, string info)
        {
            try
            {
                if (LogStream != null)
                {
                    switch (messageType)
                    {
                        case TOBLogMessageType.Info:
                            {
                                if(LogInfo)
                                    LogStream.WriteLine(DateTime.Now.TimeOfDay.ToString() + " " + GetCaller() + " " + info);
                                Console.WriteLine(DateTime.Now.TimeOfDay.ToString() + " " + GetCaller() + " " + info);
                                break;
                            }

                        case TOBLogMessageType.Debug:
                            {
                                LogStream.WriteLine(DateTime.Now.TimeOfDay.ToString() + " DEBUG - " + GetCaller() + " " + info);
                                Console.WriteLine(DateTime.Now.TimeOfDay.ToString() + " DEBUG - " + GetCaller() + " " + info);
                                break;
                            }

                        case TOBLogMessageType.Assert:
                            {
                                LogStream.WriteLine(DateTime.Now.TimeOfDay.ToString() + " *** ASSERTING - " + GetCaller() + " " + info);
                                Console.WriteLine(DateTime.Now.TimeOfDay.ToString() + " *** ASSERTING - " + GetCaller() + " " + info);
                                
                                Exception ex = new Exception(info);
                                LogStream.WriteLine(ex.ToString());
                                Console.WriteLine(DateTime.Now.TimeOfDay.ToString() + ex.ToString());
                                
                                break;
                            }
                    }

                    LogStream.Flush();

                }
            }
            catch (IOException ioEx)
            {
                Console.WriteLine("*** PANIC - IOException thrown in TOBLogger.InternalWrite. Dumping Exceptionn ...");
                Console.WriteLine(ioEx.ToString());
            }
            catch (Exception ex)
            {
                LogStream.WriteLine("*** EXCEPTION IN TOBLOGGER.InternalWrite");
                LogStream.WriteLine(ex.ToString());
            }

            return;
        }

        private static string GetCaller()
        {
            // Get the realtime Stack Trace
            StackFrame currentStack = new StackFrame(TOBLOGGERFRAMESKIP, true);
            if (currentStack != null)
            {
                // Get caller in form of [ClassName.MethodName]
                return "[" + currentStack.GetMethod().DeclaringType.Name + "." + currentStack.GetMethod().Name + "] ";
            }
            else
            {
                return string.Empty;
            }
        }

    }
}
