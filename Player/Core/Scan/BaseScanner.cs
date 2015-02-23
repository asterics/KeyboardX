using NLog;
using Player.Model;
using System;
using System.Diagnostics;
using System.Threading;

namespace Player.Core.Scan
{
    /// <summary>The base of all scanners.</summary>
    /// <remarks>
    /// This implementation currently uses <see cref="Monitor"/> for the scanning logic.
    /// 
    /// Another possibility would have been <see cref="AutoResetEvent"/>. But then it wouldn't be possible to check if a trigger is already queued 
    /// and the current one has to be discarded.
    /// 
    /// About AutoResetEvent: http://msdn.microsoft.com/en-us/library/system.threading.autoresetevent%28v=vs.110%29.aspx
    /// </remarks>
    abstract class BaseScanner : Scanner
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        public event EventHandler<ButtonPressEventArgs> ButtonPress;

        public event EventHandler<SelectionEventArgs> Selection;

        protected readonly GridModel grid;

        // A scanner is basically a state machine. The states are modeled through methods that fit this delegate type.
        protected delegate void ScanStateDelegate(bool trigger);
        protected ScanStateDelegate scanState;

        // For description of scan times see XML Schema for application config file.
        private readonly int initialScanDelay;
        private readonly int postAcceptanceDelay;
        private readonly int postInputAcceptanceTime;
        private readonly int scanTime;

        private volatile bool readyForTrigger;

        private readonly object triggerLock;
        private readonly Stopwatch triggerWatch;
        private bool triggerReceived;

        private ButtonGroup prevSelected;


        public BaseScanner(GridModel grid, int initialScanDelay, int postAcceptanceDelay, int postInputAcceptanceTime, int scanTime)
        {
            this.grid = grid;
            this.initialScanDelay = initialScanDelay;
            this.postAcceptanceDelay = postAcceptanceDelay;
            this.postInputAcceptanceTime = postInputAcceptanceTime;
            this.scanTime = scanTime;

            triggerLock = new Object();
            triggerWatch = new Stopwatch();
            Reset();
        }


        public void DoScan()
        {
            try
            {
                logger.Debug("Scanner thread for grid '{0}' started.", grid.Id);
                Reset();  // reset on every start, cause restarts are possible

                Thread.Sleep(initialScanDelay);
                logger.Debug("Initial scan delay of {0}ms is over - let's get this party started!", initialScanDelay);

                scanState(false);  // initial scan, i.e. initial select
                readyForTrigger = true;

                ScanForever();
            }
            catch (Exception e)
            {
                if (!(e is ThreadAbortException))
                    logger.Error("Unrecognized exception occurred in scanner thread!\n{0}", e);

                logger.Debug("Scanner thread for grid '{0}' stopped.", grid.Id);
            }
            finally
            {
                // Is there anything to clean up after stopping scanner?
            }
        }

        private void ScanForever()
        {
            while (true)
            {
                bool trigger;

                lock (triggerLock)
                {
                    Monitor.Wait(triggerLock, scanTime);
                    trigger = triggerReceived;
                    triggerReceived = false;
                }

                scanState(trigger);
            }
        }

        #region OnTrigger

        /// <summary>
        /// This method should return as soon as possible. Calling thread should not be blocked longer than necessary.
        /// </summary>
        /// <seealso cref="Scanner.OnTrigger()"/>
        public virtual void OnTrigger()
        {
            if (!IsReady())
                return;

            lock (triggerLock)
            {
                if (!CheckPostAcceptanceDelay() || IsTriggerQueued())
                    return;

                Trigger();
            }
        }

        private bool IsReady()
        {
            if (readyForTrigger)
                return true;
            else
            {
                logger.Debug("Discarding trigger, because scanner is not ready currently!");
                return false;
            }
        }

        private bool CheckPostAcceptanceDelay()
        {
            if (triggerWatch.IsRunning)
            {
                if (triggerWatch.ElapsedMilliseconds < postAcceptanceDelay)
                {
                    logger.Debug("Discarding trigger, because of Post Acceptance Delay!");
                    return false;
                }
            }

            triggerWatch.Restart();
            return true;
        }

        private bool IsTriggerQueued()
        {
            if (triggerReceived)
            {
                logger.Debug("Discarding trigger, because there is already one queued!");
                return true;
            }
            else
                return false;
        }

        private void Trigger()
        {
            triggerReceived = true;
            Monitor.Pulse(triggerLock);
        }

        #endregion

        /// <summary>
        /// Here all variables that represent some kind of state should be reseted. After calling this a scanner should be save to be restarted.
        /// Extending classes can/should override this method, but DON'T FORGET to call base.
        /// </summary>
        protected virtual void Reset()
        {
            readyForTrigger = false;
            triggerWatch.Reset();
            triggerReceived = false;
            prevSelected = ButtonGroup.Empty;
        }

        #region PressButton

        protected void PressButton(ButtonModel btn)
        {
            ButtonPressEventArgs args = new ButtonPressEventArgs(btn.Id);
            RaiseButtonPressEvent(args);

            while (TriggerAgain())
                RaiseButtonPressEvent(args);
        }

        private bool TriggerAgain()
        {
            lock (triggerLock)
            {
                if (triggerReceived)
                    logger.Debug("Trigger received while executing button (before PostInputAcceptanceTime).");
                else
                {
                    Monitor.Wait(triggerLock, postInputAcceptanceTime);

                    if (triggerReceived)
                        logger.Debug("Trigger received during PostInputAcceptanceTime.");
                }

                bool trigger = triggerReceived;
                triggerReceived = false;
                return trigger;
            }
        }

        private void RaiseButtonPressEvent(ButtonPressEventArgs e)
        {
            logger.Trace("Raising button press event for button '{0}'...", e.ButtonId);

            EventHandler<ButtonPressEventArgs> handler = ButtonPress;
            if (handler != null)
                handler(this, e);
        }

        # endregion

        protected void SelectButtons(ButtonGroup selection)
        {
            SelectionEventArgs args = new SelectionEventArgs(selection, prevSelected);
            prevSelected = selection;
            RaiseSelectionEvent(args);
        }

        private void RaiseSelectionEvent(SelectionEventArgs e)
        {
            logger.Trace("Raising selection event...");

            EventHandler<SelectionEventArgs> handler = Selection;
            if (handler != null)
                handler(this, e);
        }
    }
}
