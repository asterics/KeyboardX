using NLog;
using Player.Conf;
using Player.Core.Element;
using Player.Core.Gui;
using Player.Core.Scan;
using Player.Model;
using System;
using StatusFlags = Player.Core.Status.ButtonStatus.Flags;

namespace Player.Core.Status
{
    /// <summary>
    /// Status related controller stuff goes here.
    /// </summary>
    class StatusController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        private event EventHandler<StatusChangeEventArgs> StatusChange;

        private GridStatusImpl Status { get; set; }


        public StatusController()
        {
            // NOP
        }


        public void Wire(Grid grid)
        {
            Status = grid.Status;
            Status.ResetButtons();

            StatusChange += grid.Drawer.OnStatusChange;

            if (Config.Global.ShowMouseChanges)
                grid.Drawer.MouseChange += OnMouseChange;

            if (grid.Scanner != null)
                grid.Scanner.Selection += OnSelection;
        }

        public void Unwire(Grid grid)
        {
            Status = null;
            StatusChange -= grid.Drawer.OnStatusChange;

            grid.Drawer.MouseChange -= OnMouseChange;

            if (grid.Scanner != null)
                grid.Scanner.Selection -= OnSelection;
        }

        public void ClearSelection()
        {
            CheckStatus();

            ButtonGroup changes = Status.ClearSelection();

            if (!changes.IsEmpty())
                RaiseStatusChangeEvent(new StatusChangeEventArgs(changes));
        }

        public void OnSelection(Object sender, SelectionEventArgs e)
        {
            logger.Trace("OnSelection() received...");
            CheckStatus();

            ButtonGroup changes = new ButtonGroup();

            // handle unselected first, because of selection race, or in other words: selection is trump
            Status.UpdateStatus(e.Unselected, StatusFlags.Selected, false);
            changes.Add(e.Unselected);

            Status.UpdateStatus(e.Selected, StatusFlags.Selected, true);
            changes.Add(e.Selected);

            if (!changes.IsEmpty())
            {
                changes.Seal();
                RaiseStatusChangeEvent(new StatusChangeEventArgs(changes));
            }
        }

        protected void OnMouseChange(Object sender, MouseChangeEventArgs e)
        {
            logger.Trace("OnMouseChange() received...");
            CheckStatus();
            CheckMouseChangeEvent(e);

            ButtonGroup changes = new ButtonGroup();

            // for this event it's pretty likely that one argument is empty, so check first

            if (!e.MouseOver.IsEmpty())
            {
                Status.UpdateStatus(e.MouseOver, StatusFlags.MouseOver, true);
                changes.Add(e.MouseOver);
            }

            if (!e.MouseOut.IsEmpty())
            {
                Status.UpdateStatus(e.MouseOut, StatusFlags.MouseOver, false);
                changes.Add(e.MouseOut);
            }

            if (!changes.IsEmpty())
            {
                changes.Seal();
                RaiseStatusChangeEvent(new StatusChangeEventArgs(changes));
            }
        }

        private void CheckStatus()
        {
            if (Status == null)
            {
                string msg = String.Format("{0} may only be used when Status is set!", GetType().Name);
                logger.Error(msg);
                throw new InvalidOperationException(msg);
            }
        }

        private void CheckMouseChangeEvent(MouseChangeEventArgs e)
        {
            if (e.MouseOver.IsEmpty() && e.MouseOut.IsEmpty())
                logger.Warn("Received an empty {0} object, which should not happen!", e.GetType().Name);
            else
            {
                if (e.MouseOver.Equals(e.MouseOut))
                    logger.Warn("Received a {0} object where MouseOver equals MouseOut, which should not happen!", e.GetType().Name);
            }
        }

        private void RaiseStatusChangeEvent(StatusChangeEventArgs e)
        {
            logger.Trace("Raising status change event...");

            EventHandler<StatusChangeEventArgs> handler = StatusChange;
            if (handler != null)
                handler(this, e);
        }
    }
}
