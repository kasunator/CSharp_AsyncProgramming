using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskExamples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// We discuss the details of  initiating long running tasks.
    /// Difference between Task.Factory.StartNew() and Task.Start()
    /// lambda functions used for initiating tasks.
    /// calling methods from lambda functions, and making those methods run on taks.
    /// Passing arguments through lambda functions to methods and making them run on task. 
    /// Cancelling running tasks.
    /// 
    /// 
    /// </summary>
    /// 
    /* The Run() method provides a simple way tp start a task using default values and without using required additional parameters.
         
        The Task.Run() is equivalen to Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);

        The default settings of Task.Factory.StartNew(action) is equivalent to  
        Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Current);

        The Task.Factory property returns a TaskFactory object. Overloads of the TaskFactory.StartNew method let you specify parameters
        to pass to the task creation options and a task scheduler.  
        Ex: Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);

        LONG running Taks:
        By default, the CLR runs tasks on pooled threads, which is ideal for short-running
        compute-bound work. For longer-running and blocking operations (such as our
        preceding example), you can prevent use of a pooled thread as follows:
        Task task = Task.Factory.StartNew (() => ...,TaskCreationOptions.LongRunning);

        IMPORTANT !! USE TaskScheduler.Default setting when using Task.Factory.StartNew() because the defualt Scheduler setting is TaskScheduler.Current);
        Task task = Task.Factory.StartNew (() => ...,TaskScheduler.Default);

    */


    public partial class MainWindow : Window
    {
        private Task currentTask;
        private CancellationTokenSource cancelSource; //= CancellationTokenSource();
        private CancellationToken cancellationToken;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //start_longRunning_Task();
            //start_longRunning_Task_fromFunction();
            //start_cancellable_infinit_task();
            start_and_await_for_infinite_task();
        }

        private void Button_stop_Click(object sender, RoutedEventArgs e)
        {
            cancelSource?.Cancel();
            
        }



        void start_longRunning_Task()
        {
            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Task Started ");
                Thread.Sleep(100);
                Console.WriteLine("Task Ended ");
            }, TaskCreationOptions.LongRunning);
        }

        void long_running_function(int param)
        {
            Console.WriteLine("Task Started from function {0}",param);
            Thread.Sleep(100);
            Console.WriteLine("Task Ended from function {0}", param);
        }

        void infinite_loop_function(CancellationToken cancellationToken)
        {
            Console.WriteLine("Infinite loop taks started");
            while(true)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

        }

        void start_longRunning_Task_fromFunction()
        {

            Task.Factory.StartNew( () => { long_running_function(10); }, TaskCreationOptions.LongRunning);
        }

        void start_longRunning_Task_fromFunction2()
        {
            Action<int> syncFunc = (int x) => { long_running_function(x); };
            //it does not work like this becuase the syncFunc(10) is passed as void to the .StartNew() 
            //Task.Factory.StartNew( syncFunc(10), TaskCreationOptions.LongRunning);

            //It does not work like this because .StartNew() only takes onl Action not Action<int>
            //Task.Factory.StartNew(Action<int> syncFunc = (int x) => { long_running_function(x); }, TaskCreationOptions.LongRunning);
        }


        async void start_cancellable_infinit_task()
        {
            cancelSource = new CancellationTokenSource();

            currentTask = Task.Factory.StartNew(() => { infinite_loop_function(cancelSource.Token); }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            try
            {
                await currentTask;
                Console.WriteLine("Infinite loop function task completed");


            }
            catch ( OperationCanceledException e)
            {
                //e.ToString();
                //Console.WriteLine("Infinite loop function task completed with exception: {0}", e.ToString());
                Console.WriteLine("Infinite loop function task completed with operation cancel exception");
            }

        }


        void start_and_await_for_infinite_task()
        {
            cancelSource = new CancellationTokenSource();
            CancellationToken myCancelToken = cancelSource.Token; 
            currentTask = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Infinite loop taks started", myCancelToken);
                while (true)
                {
                    //cancellationToken
                    if (myCancelToken.IsCancellationRequested == true)
                    {
                        Console.WriteLine("cancellation requested");
                        myCancelToken.ThrowIfCancellationRequested();
                    }
                    
                }
            }, myCancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            cancellationToken = Task.Factory.CancellationToken;
           
        }


    }
}
