using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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

namespace AwiatOnPeriodicTimerAndEventWaitHandle
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


        private readonly EventWaitHandle _signal = new AutoResetEvent(false);
        private  CancellationTokenSource _cts = new CancellationTokenSource();
        private Task _signalAndPeriodWorkerTask;
        void DoWork()
        {
            Console.WriteLine($"DoWork @ {DateTime.Now}");
        }
        /* What chatGPT suggested was PeriodicTimer.
         * Ex: var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
         * But that is not avaialble in .NET framwork 4.8. So
         we are going to use Task Loop with Task.Delay*/
        public async Task RunAsync(CancellationToken token)
        {

            while (token.IsCancellationRequested == false)
            {
                DoWork();
                await Task.Delay(1000);
            }
        }


        /* This will start the  Task Loop with Task.Delay*/

        void startTaskLoopWithTaskDelay()
        {
            this._cts = new CancellationTokenSource();

            Task.Run(() => RunAsync(_cts.Token));
        }

        /* This will stop the Task Loop with Task.Delay */
        void stopTaskLoopWithTaskDelay()
        {
            this._cts.Cancel();
        }


        /* Convert WaitHandle to an awaitable Task 
         * It uses RegistedWaitHanlde to achieve this */

        private static Task WaitHandleAsync(WaitHandle handle, CancellationToken token = default)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            CancellationTokenRegistration ctr = default(CancellationTokenRegistration);
            RegisteredWaitHandle registration = null;

            /*RegisterWaitForSingleObject(WaitHandle waitObject, WaitOrTimerCallback callBack, object state, int millisecondsTimeOutInterval, bool executeOnlyOnce)*/
            registration = ThreadPool.RegisterWaitForSingleObject(
                handle,
                (state, timedOut) =>
                {
                    /* Wait or Timer Callback:
                     * This basically unregisters the WaitHandle after the callback. 
                     * Either if the wait signal is received or timeout(if any) has happened 
                     */
                    
                    //tcs.TrySetResult(true);         //set the result of the task completion source, which will be received by the "awaiting task" 
                    var localTcs = (TaskCompletionSource<object>)state;
                    //localTcs.TrySetResult(null);
                    registration?.Unregister(null); //unregister the callback
                    localTcs.SetResult(null);
                },
                tcs, //I gues this is the object that will be passed when above callback is called 
                Timeout.Infinite, //int millisecondsTimeOutInterval : Timeout.Infinite no timeout
                true); //wait only once

            if (token.CanBeCanceled)
            {
               ctr = token.Register(() =>
                    {
                        registration?.Unregister(null);
                        tcs.TrySetCanceled(token); //set the result of the task completion source as canceld, which will be received byt the aiaiting task.
                    });
            }

            tcs.Task.ContinueWith(_ =>
            {
                //registration?.Unregister(null);
                //ctr.Dispose();
            }, TaskScheduler.Default);

            return tcs.Task; //retrun the 
        }

        private void DoPeriodicWork()
        {
            Console.WriteLine("Periodic work: " + DateTime.Now);
        }

        private void HandleSignal()
        {
            Console.WriteLine("Signal received: " + DateTime.Now);
        }




        /* Async task that awaits on either on the async loop withe Task.Delay or the
         EventHandle - > awaitable task */
        private async Task RunAndAwaitPeriodOrSignal(EventWaitHandle signal, CancellationToken token)
        {
            TimeSpan interval = TimeSpan.FromSeconds(10);
            Task delayTask;
            Task signalTask = WaitHandleAsync(signal, token);

            while (!token.IsCancellationRequested)
            {
                delayTask = Task.Delay(interval, token);

                Task completed = await Task.WhenAny(delayTask, signalTask);
                //await signalTask.ConfigureAwait(false);
                //HandleSignal();
                if (completed == delayTask)
                 {
                     DoPeriodicWork();
                    //signalTask = WaitHandleAsync(signal, token);
                }
                 else if (completed == signalTask)
                 {
                     HandleSignal();
                    //re arm
                    signalTask = WaitHandleAsync(signal, token);
                 }
            }
        }


        public void StartSignalOrPeriodTask()
        {
            _signalAndPeriodWorkerTask = Task.Run(() => RunAndAwaitPeriodOrSignal(_signal, _cts.Token));
            Console.WriteLine("Signal Or Periodic Task Started");
        }

        public void TriggerSignalToTask()
        {
            _signal.Set();
            //Console.WriteLine("Signal set");
        }

        public async void StopSignalOrPeriodTask()
        {
            _cts.Cancel();

            try
            {
                await _signalAndPeriodWorkerTask.ConfigureAwait(false);
                //await _signalAndPeriodWorkerTask;
                Console.WriteLine("Signal Or Periodic Task stoped");
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //startTaskLoopWithTaskDelay();
            StartSignalOrPeriodTask();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // stopTaskLoopWithTaskDelay();
            StopSignalOrPeriodTask();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            TriggerSignalToTask();
        }
    }
}
