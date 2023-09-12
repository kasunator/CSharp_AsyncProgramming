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

namespace AsyncProgramming
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
        /*
         *      TaskCompletionSource lets you create a task out of any operation that completes
            in the future. It works by giving you a “slave” task that you manually drive—by
            indicating when the operation finishes or faults. This is ideal for I/O-bound work:
            you get all the benefits of tasks (with their ability to propagate return values,
            exceptions, and continuations) without blocking a thread for the duration of the
            operation.

                To use TaskCompletionSource, you simply instantiate the class. It exposes a Task
            property that returns a task upon which you can wait and attach continuations—
            just as with any other task. The task, however, is controlled entirely by the Task
            CompletionSource object via the following methods:
            public class TaskCompletionSource<TResult>
            {
                public void SetResult (TResult result);
                public void SetException (Exception exception);
                public void SetCanceled();
                public bool TrySetResult (TResult result);
                public bool TrySetException (Exception exception);
                public bool TrySetCanceled();
                public bool TrySetCanceled (CancellationToken cancellationToken);

            }
            Calling any of these methods signals the task, putting it into a completed, faulted,
            or canceled state (we cover the latter in the section “Cancellation” on page 659).
            You’re supposed to call one of these methods exactly once: if called again, Set
            Result, SetException, or SetCanceled will throw an exception, whereas the Try*
            methods return false.

            By attaching a continuation to the task, we can write its result without blocking
            any thread:
              Ex:  System.Runtime.CompilerServices.TaskAwaiter<int> awaiter = tcsTaskName().GetAwaiter();
                    awaiter.OnCompleted (() => Console.WriteLine (awaiter.GetResult()));

         */

         /* this function will block the UI thread for the result */
        private void _start_thread_with_tcs_oldFasioned_blocking()
        {
            // TaskCompletionSource<int> tcs new TaskCompletionSource<TResult>
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            /* 
             *  Thread thread = new Thread(() => { work code };
                Ex: Thread thread = new Thread(() => { Thread.Sleep(5000); tcs.SetResult(42); };
                thread.IsBackground = true;
                thread.Start();
             * 
             */

            new Thread(() => { Thread.Sleep(5000); tcs.SetResult(42); })
            { IsBackground = true }
            .Start();
            Task<int> task = tcs.Task; // Our "slave" task.
            Console.WriteLine(" now we start bloking");
            Console.WriteLine(task.Result); // 42 This is going to print aprx after 5 seconds
            Console.WriteLine(" Done Blocking");
        }

        /* this function will not block the UI thread for the result */
        private void _start_thread_with_tcs_oldFasioned_NonBlocking()
        {
            // TaskCompletionSource<int> tcs new TaskCompletionSource<TResult>
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            /* 
             *  Thread thread = new Thread(() => { work code };
                Ex: Thread thread = new Thread(() => { Thread.Sleep(5000); tcs.SetResult(42); };
                thread.IsBackground = true;
                thread.Start();
             * 
             */

            new Thread(() => { Thread.Sleep(5000); tcs.SetResult(42); })
            { IsBackground = true }
            .Start();
            Task<int> task = tcs.Task; // Our "slave" task.
            System.Runtime.CompilerServices.TaskAwaiter<int> awaiter = task.GetAwaiter();
            awaiter.OnCompleted(() => Console.WriteLine(awaiter.GetResult()));
        }

        /* this functions starts a task with tcs using a more portable Run method we created
         and will not block the UI*/
        private void _star_thread_with_tcs_uing_run_method()
        {
            // Task<TResult> task = Run(() => { any function that returns TResult });

            Task<int> task = Run(() => { Thread.Sleep(5000); return 42; });
            //By attaching a continuation to the task, we can write its result without blocking
            //any thread:
            System.Runtime.CompilerServices.TaskAwaiter<int> awaiter = task.GetAwaiter();
            awaiter.OnCompleted(() => Console.WriteLine(awaiter.GetResult()));
        }

        /*  A run method using TaskCompletionSource 
            Task<int> task = Run(() => Func<TResult> function);
            Ex:- Task<int> task = Run(() => { Thread.Sleep(5000); return 42; });
            Calling this method is equivalent to calling Task.Factory.StartNew with the Task
            CreationOptions.LongRunning option to request a nonpooled thread.*/
        Task<TResult> Run<TResult>(Func<TResult> function)
        {
            var tcs = new TaskCompletionSource<TResult>();
            new Thread(() =>
            {
                try { tcs.SetResult(function()); }
                catch (Exception ex) { tcs.SetException(ex); }
            }).Start();
            return tcs.Task;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Button Click Start");
            // _start_thread_with_tcs_oldFasioned();
            //_star_thread_with_tcs_uing_run_method();
            _start_thread_with_tcs_oldFasioned_NonBlocking();
            Console.WriteLine("Button Click End");
        }
    }
}
