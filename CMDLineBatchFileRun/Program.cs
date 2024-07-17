using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDLineBatchFileRun
{
    internal class Program
    {
        static void Main(string[] args)
        {
            start_GetDotNetVersion();

            Console.ReadLine();
        }

        public static async void start_GetDotNetVersion()
        {
            Task<(String,String)> myTask = Task.Factory.StartNew(() => { return GetDotNetVersion(); }, TaskCreationOptions.LongRunning);

            try
            {
                //string result = await myTask;
                myTask.Wait();
                (String result, String error) = myTask.Result;
                Console.WriteLine("GetDotNetVersion result :"+ result+ "error:" + error);
                //Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception :"+ ex.ToString());
            }


        }


        /*Ex:1 Execute the "dotnet --version" command from the application. dotnet should be in your path .Since this this a .Net core application 
         it does not support waitForExitAsync. It only supports */
        public static (string result, string error) GetDotNetVersion()
        {
            ProcessStartInfo StartInfo = new ProcessStartInfo();
            StartInfo.FileName = "dotnet";
            StartInfo.Arguments = "--version";
            StartInfo.CreateNoWindow = true;
            StartInfo.RedirectStandardOutput = true;
            StartInfo.RedirectStandardError = true;
            StartInfo.UseShellExecute = false;

            Process proc = Process.Start(StartInfo);
            if (proc == null)
            {
                NullReferenceException nullRefEx = new NullReferenceException();
                throw nullRefEx;
            }

            //string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            string output = proc.StandardOutput.ReadToEnd();
            string error = proc.StandardError.ReadToEnd();
            return (output, error);

        }

        public static (string result,string error) GetDotNetInvalidCmd()
        {
            ProcessStartInfo StartInfo = new ProcessStartInfo();
            StartInfo.FileName = "dotnet";
            StartInfo.Arguments = "invalid command";
            StartInfo.CreateNoWindow = true;
            StartInfo.RedirectStandardOutput = true;
            StartInfo.RedirectStandardError = true;

            Process proc = Process.Start(StartInfo);
            if (proc == null)
            {
                NullReferenceException nullRefEx = new NullReferenceException();
                throw nullRefEx;
            }

            
            proc.WaitForExit();
            string output = proc.StandardOutput.ReadToEnd();
            string error = proc.StandardError.ReadToEnd();

            return (output, error);

        }

    }
}
