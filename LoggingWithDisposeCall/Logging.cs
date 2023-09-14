using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace DisposeCall
{
    class Logging : IDisposable
    {
        enum LogCat
        {
            VERBOSE,
            DEBUG,
            WARN,
            ERROR,
        }

        public enum LogLevel
        {
            DEBUG = 4,
            INFO = 3,
            WARN = 2,
            ERROR = 1,
            NONE = 0,
        }

        private string log_file = Environment.CurrentDirectory;
        private StreamWriter writer = null;

        private string logTag = "";
        public LogLevel logLevel;
        private ConcurrentQueue<string> writeQueue;
        private Task<bool> task;
        private CancellationTokenSource cancelSource;

        public Logging(string tag = "", LogLevel level = LogLevel.DEBUG, bool save2File = false)
        {
            logTag = tag;
            logLevel = level;

            if (log_file == "" && save2File == true)
            {
                DateTime now = DateTime.Now;
                //this.log_file = "UFWU_Log_Tag_"+ tag + now.Year + '_' + now.Month + '_' + now.Day + '_' + now.Hour + '_' + now.Minute + '_' + now.Second + ".txt";
                log_file = Environment.CurrentDirectory + "\\UFWU_Log_Tag.txt";
                writer = new StreamWriter(log_file, true);
                writeQueue = new ConcurrentQueue<string>();
                cancelSource = new CancellationTokenSource();
                //task = Task.Run(writeFileTask(cancelSource.Token));
                //task = Task.Run(writeFileTask());
                task = Task.Factory.StartNew(() => writeFileTask(cancelSource.Token), cancelSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            }
   

        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                // task.     
            }
        }

        private bool writeFileTask(CancellationToken cancellationToken)
        {
            bool retVal = true;
            while (writeQueue.TryDequeue(out string data) || cancellationToken.IsCancellationRequested)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
                else
                {
                    writer.WriteLine(data);
                }
            }
            return retVal;

        }

        private bool writeFileTask()
        {
            bool retVal = true;
            while (writeQueue.TryDequeue(out string data))
            {

                writer.WriteLine(data);

            }
            return retVal;

        }


        public Logging(bool noPrint, string tag = "")
        {
            logTag = tag;
            logLevel = noPrint ? LogLevel.NONE : LogLevel.DEBUG;
        }

        public void stopLogPrint()
        {
            logLevel = LogLevel.NONE;
        }

        public void debug(object l)
        {
            if (logLevel >= LogLevel.DEBUG)
            {
                setConsoleColor(LogCat.VERBOSE);
                WriteLog(logTag, l);
            }
        }

        public void log(object l)
        {
            if (logLevel >= LogLevel.INFO)
            {
                setConsoleColor(LogCat.VERBOSE);
                WriteLog(logTag, l);
            }
        }

        public void warn(object l)
        {
            if (logLevel >= LogLevel.WARN)
            {
                setConsoleColor(LogCat.WARN);
                WriteLog(logTag, l);
            }
        }

        public void error(object l)
        {
            if (logLevel >= LogLevel.ERROR)
            {
                setConsoleColor(LogCat.ERROR);
                WriteLog(logTag, l);
            }
        }

        public static void SLog(string t, object l)
        {
            SWriteLog(t, l);
        }

        private void setConsoleColor(LogCat cat)
        {
            switch (cat)
            {
                case LogCat.VERBOSE:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogCat.DEBUG:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogCat.WARN:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogCat.ERROR:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
        }


        private void WriteLog(string s, object o)
        {

            writer?.WriteLine(s + ": " + o);

            Console.Write("[" + DateTime.Now.ToString("s", DateTimeFormatInfo.InvariantInfo) + "] " + s + ": ");
            Console.WriteLine(o);
            Console.ResetColor();
        }

        private static void SWriteLog(string s, object o)
        {
            Console.Write("[" + DateTime.Now.ToString("s", DateTimeFormatInfo.InvariantInfo) + "] " + s + ": ");
            Console.WriteLine(o);
            Console.ResetColor();
        }
    }



}
