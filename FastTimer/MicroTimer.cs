using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace FastTimer
{
    public class MicroTimer
    {
        //The stopwatch uses the system's high performance counter
        Stopwatch sw = new Stopwatch();
        //The delegate used to call an external function
        public delegate void ExternalCode();
        public ExternalCode OnTimeout;
        //Store the timeout period in microseconds
        private int microSeconds =  100;
        //and the equivalent CPU ticks (1000000/100=10000)
        private long microSecondsInCPUTicks = Stopwatch.Frequency / 10000;
        //Accessors for the microseconds property
        //and conversion to the CPU ticks equivalent
        public int MicroSeconds
        {
            get
            {
                return microSeconds;
            }
            set
            {
                microSeconds = value;
                microSecondsInCPUTicks = value * Stopwatch.Frequency / 1000000;
            }
        }
        //Progress reporter
        public IProgress<int> Updater { get; set; }
        //Default feedback rate to a 10th of a second
        private int feedbackMilliseconds = 100;
        //in the equivalent CPU ticks
        private long feedbackSecondsInCPUTicks = Stopwatch.Frequency / 100;
        //Accessors for the feedbackMilliseconds property
        //and conversion to the CPU ticks equivalent
        public int FeedbackMilliseconds
        {
            get
            {
                return feedbackMilliseconds;
            }
            set
            {
                feedbackMilliseconds = value;
                feedbackSecondsInCPUTicks = value * Stopwatch.Frequency / 1000;
            }
        }
        //Support cancelling to stop the timer
        CancellationTokenSource stopTimer;
        //Stop the timer
        public void Stop()
        {
            if(stopTimer != null)
                stopTimer.Cancel();
        }
        //Start the MicroTimer (async for new thread)
        public async void Start()
        {
            //Start if delegate set
            if (stopTimer == null && OnTimeout != null)
            {
                //Need to be able to cancel the MicroTimer
                stopTimer = new CancellationTokenSource();
                //CPU bound task, hence Task.Run
                await Task.Run(() => Timing(Updater, stopTimer.Token));
                //When Timing function returns, finished with CancellationTokenSource
                stopTimer.Dispose();
                stopTimer = null;
            }
        }
        //Run the timer
        void Timing(IProgress<int> progress, CancellationToken cancelTimimg)
        {
            //Calculates the timing difference
            long interval;
            //Counter for progress timing
            int progressCounter = 0;
            //Start the stopwatch
            sw.Start();
            //Loop until stopped
            while (sw.IsRunning)
            {
                //Get interval to wait
                interval = sw.ElapsedTicks + microSecondsInCPUTicks;
                //Loop until interval has passed
                while (interval - sw.ElapsedTicks > 0)
                {
                    //Chance for kernel to yield the tight CPU loop
                    Thread.Sleep(0);
                }
                //Run the external code
                OnTimeout();
                //see if a progress report is required
                if (++progressCounter >= feedbackSecondsInCPUTicks / microSecondsInCPUTicks)
                {
                    progress.Report(progressCounter);
                    progressCounter = 0;
                }
                if (cancelTimimg.IsCancellationRequested)
                    sw.Stop();
            }
        }
        //Stopwatch IsRunning used for a MicroTimer IsRunning
        public bool IsRunning => sw.IsRunning;
    }
}
