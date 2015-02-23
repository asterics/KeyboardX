using NLog;
using Player.Conf;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Player.Core.Trigger
{
    /// <summary>
    /// Triggers when a given key is pressed. Considers Input Acceptance Time (IAT), Repeat Acceptance Delay (RAD) and Repeat Time (RT) in the 
    /// following manner.
    /// <para/>
    /// If all times are greater 0 and key is pressed indefinitely long, the following pattern results:
    /// IAT TRIGGER RAD TRIGGER RT TRIGGER RT TRIGGER...
    /// </summary>
    /// <remarks>
    /// Reference:
    /// http://msdn.microsoft.com/en-us/library/hh873175%28v=vs.110%29.aspx (TAP)
    /// http://msdn.microsoft.com/en-us/library/dd537607%28v=vs.110%29.aspx (cancel tasks)
    /// 
    /// Global Hot Keys:
    /// http://stackoverflow.com/questions/11752254/global-windows-key-press
    /// http://www.pinvoke.net/default.aspx/user32/registerhotkey.html
    /// http://www.dreamincode.net/forums/topic/180436-global-hotkeys/
    /// </remarks>
    /// TODOs:
    ///  TODO 5: should be able to listen to several keys not just one
    ///  TODO 3: should be able to get key events even when not focused (see above, Global Hot Keys)
    ///  TODO 1: if times need to be more accurate, think about how it could be done
    class KeyTrigger : BaseTrigger
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static KeyTrigger CreateSpaceBarTrigger() { return new KeyTrigger(Keys.Space); }


        private readonly Keys triggerKey;

        private CancellationTokenSource triggerCancel;


        public KeyTrigger(Keys key)
        {
            triggerKey = key;
        }


        public void OnKeyDown(Object sender, KeyEventArgs e)
        {
            if (e.KeyCode != triggerKey)
                return;
            else
                e.Handled = true;

            if (triggerCancel == null)
            {
                logger.Trace("Starting trigger task...");
                triggerCancel = new CancellationTokenSource();
                Task.Factory.StartNew(() => DoTrigger(triggerCancel.Token), triggerCancel.Token);
            }
        }

        public void OnKeyUp(Object sender, KeyEventArgs e)
        {
            if (e.KeyCode != triggerKey)
                return;
            else
                e.Handled = true;

            if (triggerCancel != null)
                Cancel();
            else
                logger.Warn("Trigger task seems to have already been canceled!");
        }

        /// <summary>
        /// Used when GUI thread is blocked and no key up will be raised, e.g. when showing error message.
        /// <para>ATTENTION: Has to be called from the GUI thread, otherwise it's not thread safe!</para>
        /// </summary>
        public void Cancel()
        {
            if (triggerCancel != null)
            {
                logger.Trace("Canceling trigger task...");
                triggerCancel.Cancel();
                triggerCancel.Dispose();
                triggerCancel = null;
            }
        }

        private void DoTrigger(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            TriggerAfterInputAcceptanceTime(token);
            TriggerRepeatedly(token);
        }

        private void TriggerAfterInputAcceptanceTime(CancellationToken token)
        {
            if (Config.Scanner.InputAcceptanceTime > 0)
            {
                bool canceled = token.WaitHandle.WaitOne(Config.Scanner.InputAcceptanceTime);

                if (canceled)
                    logger.Debug("Key press was too short to be accepted. (Input Acceptance Time = {0})", Config.Scanner.InputAcceptanceTime);
            }
            
            TriggerOrCancel(token);
        }

        private void TriggerRepeatedly(CancellationToken token)
        {
            if (Config.Scanner.RepeatAcceptanceDelay > 0)
                token.WaitHandle.WaitOne(Config.Scanner.RepeatAcceptanceDelay);

            TriggerOrCancel(token);

            while (true)
            {
                token.WaitHandle.WaitOne(Config.Scanner.RepeatTime);
                TriggerOrCancel(token);
            }
        }

        private void TriggerOrCancel(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            RaiseTriggerEvent(TriggerEventArgs.Empty);
        }
    }
}
