using Report_Printer_Desktop.Helpers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;

namespace Report_Printer_Desktop.DSL
{

    public sealed class ProcessingTimerDSL
    {
        public static ProcessingTimerDSL Instance { get; } = new ProcessingTimerDSL();

        private ProcessingTimerDSL()
        {
            _logger = Helper_Logger.Create("TimerService");
            _timersList = new List<TimerElapsed>();
        }

        private readonly Helper_Logger _logger;
        private readonly List<TimerElapsed> _timersList;


        public bool IsRunning { get; private set; }

        public bool StartTimers()
        {
            try
            {
                if (IsRunning) return true;

                // Create file download timer
                var fileDownloadDSL = new FileDownloadDSL(Helper_Logger.Create("FileDownload"));
                _timersList.Add(new TimerElapsed(fileDownloadDSL, 0, () => 5 * 60 * 1000));

                IsRunning = true;
                _logger.LogInfo("Timers started successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, "Failed to start timers");
                return false;
            }
        }

      
        public bool StopTimers()
        {
            try
            {
                foreach (var timer in _timersList)
                    timer?.StopTimer();

                _timersList.Clear();
                IsRunning = false;
                _logger.LogInfo("Timers stopped successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, "Failed to stop timers");
                return false;
            }
        }

        private class TimerElapsed
        {
            private readonly System.Timers.Timer _timer;
            private readonly FileDownloadDSL _fileDownloadDSL;
            private readonly Func<double> _timerInterval;

            public TimerElapsed(FileDownloadDSL fileDownloadDSL, double startAfter, Func<double> timerInterval)
            {
                _fileDownloadDSL = fileDownloadDSL;
                _timerInterval = timerInterval;

                _timer = new System.Timers.Timer();
                _timer.Elapsed += OnTimerElapsed;
                _timer.Enabled = true;
                _timer.Interval = startAfter;
            }

            
            private void OnTimerElapsed(object sender, ElapsedEventArgs e)
            {
                var timer = (System.Timers.Timer)sender;
                try
                {
                    // Disable timer while processing
                    timer.Enabled = false;

                    // Process downloads on thread pool to avoid blocking timer thread
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        _fileDownloadDSL.ProcessDownloads();
                    });
                }
                catch (Exception ex)
                {
                    _fileDownloadDSL.GetLogger().LogException(ex);
                }
                finally
                {
                    // Reset timer interval and re-enable
                    timer.Interval = _timerInterval();
                    timer.Enabled = true;
                }
            }

     
            public void StopTimer()
            {
                _timer.Stop();
                _timer.Dispose();
            }
        }
    }
}