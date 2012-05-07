using System;
using System.Windows.Threading;

namespace Silverlight.Controls
{
    internal sealed class ToolTipTimer : DispatcherTimer
    {
 
        /// <summary>
        /// This event occurs when the timer has stopped.
        /// </summary>
        public event EventHandler Stopped;

        public ToolTipTimer(int maximumTicks, int initialDelay)
        {
            InitialDelay = initialDelay;
            MaximumTicks = maximumTicks < 1 ? 1 : maximumTicks;
            Interval = new TimeSpan(0, 0, 1);
            Tick += OnTick;
        }

        /// <summary>
        /// Stops the ToolTipTimer.
        /// </summary>
        public new void Stop()
        {
            base.Stop();
            if (Stopped != null)
                Stopped(this, System.EventArgs.Empty);
        }

        /// <summary>
        /// Resets the ToolTipTimer and starts it.
        /// </summary>
        public void StartAndReset()
        {
            CurrentTick = 0;
            Start();
        }
        /// <summary>
        /// Stops the ToolTipTimer and resets its tick count.
        /// </summary>
        public void StopAndReset()
        {
            Stop();
            CurrentTick = 0;
        }
        /// <summary>
        /// Resets the tick count of the ToolTipTimer.
        /// </summary>
        public void Reset()
        {
            if(IsEnabled)
                Stop();
            CurrentTick = 0;
        }

        /// <summary>
        /// Gets the maximum number of seconds for this timer.
        /// When the maximum number of ticks is hit, the timer will stop itself.
        /// </summary>
        /// <remarks>The minimum number of seconds is 1.</remarks>
        public int MaximumTicks { get; private set; }
        /// <summary>
        /// Gets the initial delay for this timer in seconds.
        /// When the maximum number of ticks is hit, the timer will stop itself.
        /// </summary>
        /// <remarks>The default delay is 0 seconds.</remarks>
        public int InitialDelay { get; private set; }
        /// <summary>
        /// Gets the current tick of the ToolTipTimer.
        /// </summary>
        public int CurrentTick { get; private set; }

        private void OnTick(object sender, System.EventArgs e)
        {
            CurrentTick++;
            if(CurrentTick == MaximumTicks + InitialDelay)
                Stop();
        }

    }
}