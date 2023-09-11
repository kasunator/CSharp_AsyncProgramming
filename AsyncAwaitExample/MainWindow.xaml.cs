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
            startExceptionTaskAndTryToCathcIt();
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

             ****EXCEPTION handling in tasks*****
            If the code inside the task throws an unhandled exception, that exception is automatically re-thrown to whoever calls wait() or accesses the
            result property of a Task<TResult>.
            When this exception is trhown to the waiter or the accessor of the result, the CLR wrpas the exception in an AggregatedException.

            Example:
            // Start a Task that throws a NullReferenceException:
            Task task = Task.Run (() => { throw null; });
            try
            {
                task.Wait();
            }
            catch (AggregateException aex)
            {
                if (aex.InnerException is NullReferenceException)
                Console.WriteLine ("Null!");
                else
                throw;
            }


            You can test for a faulted task without making it throw by calling wait or result. The exception can be tested via the IsFaul
                IsFaulted and IsCanceled properties of the Task. If both properties return false, no error
            occurred; if IsCanceled is true, an OperationCanceledOperation was thrown for that
            task ; if IsFaulted is true, another type of exception was thrown and the Exception property will indicate the error.

            **Autonomous task exceptions**
            With autonomous “set-and-forget” tasks (those for which you don’t rendezvous via
            Wait() or Result, or a continuation that does the same), 
            it’s good practice to explicitly exception-handle the task code to avoid silent failure, just as you would with a
            thread.

            You can subscribe to unobserved exceptions at a global level via the static event
            TaskScheduler.UnobservedTaskException; handling this event and logging the error
            can make good sense.
            There are a couple of interesting nuances on what counts as unobserved:
            • Tasks waited upon with a timeout will generate an unobserved exception if the
                faults occurs after the timeout interval.
            • The act of checking a task’s Exception property after it has faulted makes the exception “observed.”

            *** Task countinuations exception ***
            * If an antecedent task faults, the exception is re-thrown when the continuation code
            calls awaiter.GetResult(). Rather than calling GetResult, we could simply access
            the Result property of the antecedent. The benefit of calling GetResult is that if the
            antecedent faults, the exception is thrown directly without being wrapped in Aggre
            gateException, allowing for simpler and cleaner catch blocks.

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

        /* this compute bound task will throw and exception */
        Task<int> throwTestTaskException()
        {
            return Task.Run(() => { Thread.Sleep(2000); throw new Exception( "Will this be ignored"); return -1; });
        }

        /* This async function will await the throwTestTaskException() method and try t catch its exception*/
        async void startExceptionTaskAndTryToCathcIt()
        {
           try
           {
            int result = await throwTestTaskException();

           }
           catch(Exception e)
           {
              Console.WriteLine("Exception caught:" + e.ToString());
           }
        }

    }
}
