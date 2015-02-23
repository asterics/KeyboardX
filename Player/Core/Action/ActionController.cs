using NLog;
using Player.Conf;
using Player.Core.Element;
using Player.Core.Scan;
using Player.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Player.Core.Action
{
    /// <summary>
    /// Action related controller stuff goes here.
    /// </summary>
    /// <remarks>
    /// An explicit trigger event pressing a button can happen every time. What if a button is pressed twice at the same time?
    /// => As long as all actions are thread safe this shouldn't cause problems.
    /// 
    /// If actions that fire events need feedback about how things went, we need to add a callback to event args.
    /// 
    /// Check this out:
    /// http://msdn.microsoft.com/en-us/library/hh156548%28v=vs.110%29.aspx (index)
    /// http://msdn.microsoft.com/en-us/library/dd997364%28v=vs.110%29.aspx (cancellation)
    /// </remarks>
    class ActionController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private Grid Grid { get; set; }


        public ActionController()
        {
            // NOP
        }


        public void Wire(Grid grid)
        {
            Grid = grid;

            if (grid.Scanner != null)
                grid.Scanner.ButtonPress += OnButtonPress;
        }

        public void Unwire(Grid grid)
        {
            Grid = null;

            if (grid.Scanner != null)
                grid.Scanner.ButtonPress -= OnButtonPress;
        }

        protected void OnButtonPress(Object sender, ButtonPressEventArgs e)
        {
            logger.Trace("OnButtonPress('{0}') received...", e.ButtonId);

            CheckGrid();

            Task t = RunActionsAsync(Grid[e.ButtonId]);
            if (Config.Global.ButtonPressSynchronous)
                t.Wait();
        }

        // TODO 1: remove this check sometime when everything seems fine
        private void CheckGrid()
        {
            if (Grid == null)
            {
                string msg = String.Format("{0} may only be used when a grid is set!", GetType().Name);
                throw new InvalidOperationException(msg);
            }
        }

        /// <summary>
        /// Runs all actions of a button as own <see cref="Task"/>. If the actions itself are run synchronous or asynchronous depends on the actions 
        /// itself (<see cref="BaseAction.RunSyncronous"/>).
        /// <para />
        /// If this method should be synchronous simply use <c>Task.Wait()</c> method. Take care if UI thread is calling, it should better call 
        /// method from a new task.
        /// </summary>
        public async Task RunActionsAsync(Button btn)
        {
            if (btn.Actions.Length < 1)
            {
                logger.Info("No actions to run for button '{0}'.", btn.Id);
                return;
            }

            await Task.Yield();

            try
            {
                List<Task> tasks = new List<Task>();
                foreach (var act in btn.Actions)
                {
                    logger.Debug("Starting {0} {1}...", act.GetType().Name, (act.RunSyncronous) ? "synchronous" : "asynchronous");
                    Task t = Task.Factory.StartNew(act.DoAction);

                    if (act.RunSyncronous)
                        t.Wait();

                    tasks.Add(t);
                }

                Task.WaitAll(tasks.ToArray());
                logger.Trace("Actions for button '{0}' done!", btn.Id);
            }
            catch (Exception e)
            {
                string msg = String.Format("Exception(s) occured while running actions for button '{0}'!", btn.Id);
                logger.Error(ExceptionUtil.Format(msg, e));
            }
        }
    }
}
