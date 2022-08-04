using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace TaskExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Button Click start");
            //startSleepAndReturnWithCount(5000);
            //startSleepAndReturn(5000);
            Task_await_await_wait();
            Console.WriteLine("Button Click stop");
        }
        /* The async modifier can be applied only to methods (and lambda
            expressions) that return void or  a Task or Task<TResult>. 
            
             Task<int>  SleepTaskAsynRetCount(int count)
            {
                return Task.Run (() => { Thread.Sleep(count); return count; });

            }

            async void startSleepAndReturnWithCount(int count)
            {
              int sleepTime = await  SleepTaskAsynRetCount(count)
              Console.Wrtieline("sleep Completed:" + sleepTime);
            }
             
            All methods that use await inside must use the async modifier.

            The async modifier can be applied only to methods (and lambda
            expressions) that return void or a Task or Task<TResult>.

            The expression upon which you await is typically a task; however, any object
            with a GetAwaiter method that returns an awaiter (implementing INotifyComple
            tion.OnCompleted and with an appropriately typed GetResult method and a bool
            IsCompleted property) will satisfy the compiler. You cant await on void functions.

            Upon encountering an awit expression, the execution returns to the caller.
            But before returning the compiler(runtime) attaches continuation to the awaited task.
            When the eaited task compeletes execution jums back into the method and executes the rest for the method
            follwing the await statement. 

            The complier and runtime attaching/expanding a continuation to the awaited task is similar to re-writing the above startSleepAndReturn in the following manner.
             void startSleepAndReturnWithCount(int count)
             {
               System.Runtime.CompilerServices.TaskAwaiter<int> awaiter = SleepTaskAsynRetCount(count).GetAwaiter();
               awaiter.OnCompleted ( () = > 
               {    
                    int sleepTime = awaiter.GetResult();
                    Console.Wrtieline("sleep Completed:" + sleepTime);
               });
             }
            The compiler relies on continuations (via the awaiter pattern) to resume execution
            after an await expression. This means that if running on the UI thread 
            the synchronization context ensures continuation execution resumes on the same thread.
            The execution of the UI thread happens till the wait expression. And then returns. After this
            return if there are pending work on the UI message queue then executes them, such as UI events. Once awaited 
            task completes the continuation also executes as an event on the UI message queue, leading to the execution of what is inside  
            awaiter.OnCompleted ( () = > {} ) This executes the what is remaining after the await expression.
         */


        /* this demonstrates await on a void function */
        void countAsync(int count)
        {
            Task.Run( () => { Thread.Sleep(count); } );
        }

        /* we can of course await on a function that returns task*/
        Task SleepTaskAsyn(int count)
        {
            return Task.Run( () => { Thread.Sleep(count); } );
        }

        /* we can ofcourse await on a function that returns Task<TResult> 
         In this casae we retun the count (time we slept)*/ 
        Task<int> SleepTaskAsynRetCount(int count)
        {
            return Task.Run(() => { Thread.Sleep(count); return count; });
        }

        async void startSleepAndReturn(int count)
        {
            await SleepTaskAsyn(count);
            Console.WriteLine("sleep Completed:");
        }

        async void startSleepAndReturnWithCount(int count)
        {
            int sleepTime = await SleepTaskAsynRetCount(count);
            Console.WriteLine("sleep Completed:" + sleepTime);
        }

        async Task<bool> outerMethod()
        {
            bool retVal = true;
            int count = 0;
            Console.WriteLine("outerMethod started");
            count = await innerMethod();
            Console.WriteLine("outerMethod end");
            if (count == 5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        Task<int> innerMethod()
        {
            Console.WriteLine("innerMethod started");
            return Task.Factory.StartNew(() => { Thread.Sleep(5000); return 5; });
        }

        void Task_await_await_wait()
        {
            bool final_result = false;
            Task.Factory.StartNew(async () =>
            {
                final_result = await outerMethod();
                Console.WriteLine("Task 1 finsihed result:" + final_result.ToString());
            }).Wait();
            Console.WriteLine("Task_await_await_wait exited ");
        }

    }
}
