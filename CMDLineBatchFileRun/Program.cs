using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static System.Net.Mime.MediaTypeNames;

namespace CMDLineBatchFileRun
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //start_GetDotNetVersion();
            start_RunInfoBat();


            Console.ReadLine();
        }

        public static  void start_GetDotNetVersion()
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
            StartInfo.Arguments = "--Version";
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

        public static void start_DotNetInvalidCmd()
        {
            Task<(String, String)> myTask = Task.Factory.StartNew(() => { return GetDotNetInvalidCmd(); }, TaskCreationOptions.LongRunning);
            try
            {

                myTask.Wait();
                (String result, String error) = myTask.Result;
                Console.WriteLine("DotNetInvalidCmd result :" + result + "error:" + error);

            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception :" + ex.ToString());
            }

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

        public static void start_RunInfoBat()
        {
            Task<(String, String)> myTask = Task.Factory.StartNew(() => { return RunInfoBat(); }, TaskCreationOptions.LongRunning);
            try
            {

                myTask.Wait();
                (String result, String error) = myTask.Result;
                Console.WriteLine("RunInfoBat result :" + result + "error:" + error);

            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception :" + ex.ToString());
            }

        } 

        public static (String result, String error) RunInfoBat()
        {
            ProcessStartInfo StartInfo = new ProcessStartInfo();
            String binaryPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string subString = "bin\\Debug";
            int index = binaryPath.IndexOf(subString);
            string batPath = binaryPath.Substring(0, index);

            StartInfo.FileName = "cmd";
            StartInfo.Arguments = "/c"  + " \""  +batPath + "info.bat\"";
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


            proc.WaitForExit();
            string output = proc.StandardOutput.ReadToEnd();
            string error = proc.StandardError.ReadToEnd();

            return (output, error);
        }

    }
}
