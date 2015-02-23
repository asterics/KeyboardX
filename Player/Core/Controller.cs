using NLog;
using Player.Conf;
using Player.Core.Action;
using Player.Core.Element;
using Player.Core.Gui;
using Player.Core.Net;
using Player.Core.Status;
using Player.Core.Trigger;
using Player.Load;
using Player.Model;
using Player.Util;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Button = Player.Core.Element.Button;

namespace Player.Core
{
    /*
     * NOTES:
     * 
     * About dependency injection:
     *  http://msdn.microsoft.com/en-us/library/dn178469%28v=pandp.30%29.aspx
     *  http://msdn.microsoft.com/en-us/magazine/cc163739.aspx
     *  http://martinfowler.com/articles/injection.html
     */

    /// <summary>
    /// The central piece of the application.
    /// </summary>
    /// TODOs:
    ///  TODO 3: build in i18n framework which David Thaller suggested
    class Controller
    {
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();


        private readonly object inputLock = new object();

        public PlayerForm Form { get; private set; }

        protected StatusController statusController;
        protected ActionController actionController;
        protected TcpConnectionPool connectionPool;

        protected TcpSink tcpSink;

        protected Keyboard keyboard;
        protected Grid activeGrid;

        protected Thread scanThread;

        protected KeyTrigger keyTrigger;
        protected ClickTrigger clickTrigger;


        public Controller()
        {
            logger.Info("Starting Player...");

            Init();
        }


        #region Init

        private void Init()
        {
            BuildConfig();

            InitComponents();
            WireComponents();

            // open keyboard
            Form.Load += ((Object sender, EventArgs e) => OpenKeyboard(synchronous: false));  // either when form loads, asynchronously
            //OpenKeyboard(synchronous: true);  // or directly here, synchronous, before form is loaded
        }

        private void BuildConfig()
        {
            try { Config.Build(); }
            catch (ConfigException e)  // should be safe to go on
            {
                string msg = "Exception occurred while building config!";
                LogAndShowWarning(msg, e);
            }
            catch (Exception e)  // probably not safe to go on
            {
                string msg = "Unrecognized exception occurred while building config!";
                LogAndShowError(msg, e);
                throw new Exception(msg, e);
            }
        }

        private void InitComponents()
        {
            Form = new PlayerForm();
            statusController = new StatusController();
            actionController = new ActionController();

            InitConnectionPool();

            if (Config.Global.TcpSinkActive)
            {
                tcpSink = new TcpSink(Config.Global.TcpSinkPort); //, 3000);  // TODO B4RELEASE: remove timeout from TcpSink
                tcpSink.Activate();
            }
        }

        private void InitConnectionPool()
        {
            connectionPool = new TcpConnectionPool();

            foreach (var dest in Config.Global.TcpDests)
            {
                try { connectionPool.Add(dest); }
                catch (Exception e)
                {
                    logger.Error("Adding TCP destination '{0}' to connection pool failed!\n{1}", dest.Id, e);
                }
            }
        }

        private void WireComponents()
        {
            ActionFactory.SelectionHandler += statusController.OnSelection;
            ActionFactory.GridSwitchHandler += OnGridSwitch;
            ActionFactory.ConnectionPool = connectionPool;
            ActionFactory.StartScanner = StartScannerByScannerAction;
            ActionFactory.StopScanner = StopScannerByScannerAction;

            Form.FormClosing += OnFormClosing;

            keyTrigger = KeyTrigger.CreateSpaceBarTrigger();
            Form.KeyDown += keyTrigger.OnKeyDown;
            Form.KeyUp += keyTrigger.OnKeyUp;
            keyTrigger.Trigger += OnTrigger;

            if (Config.Global.ClickTriggerActive)
            {
                clickTrigger = new ClickTrigger();
                clickTrigger.Trigger += OnTrigger;
            }
        }

        private void OnFormClosing(Object sender, FormClosingEventArgs e)
        {
            logger.Info("Player is closing...");
            connectionPool.Dispose();

            if (tcpSink != null)
                tcpSink.Deactivate();
        }

        /// <summary>
        /// If opening keyboard before <see cref="Form"/> is loaded, call this synchronous, otherwise asynchronous.
        /// </summary>
        private void OpenKeyboard(bool synchronous)
        {
            if (synchronous)
                OpenKeyboardAndLoadDefaultGrid();
            else
            {
                Task.Factory.StartNew(() =>
                {
                    while (!Form.IsHandleCreated)
                        Thread.Yield();

                    OpenKeyboardAndLoadDefaultGrid();
                });
            }
        }

        private void OpenKeyboardAndLoadDefaultGrid()
        {
            try
            {
                KeyboardBuilder kbb = KeyboardBuilderFactory.CreateKeyboardBuilder(Form);
                keyboard = kbb.BuildKeyboard();

                Grid defGrid = keyboard.DefaultGrid;
                lock (inputLock)
                {
                    ActivateGrid(defGrid);
                }
            }
            catch (Exception e)
            {
                string msg = String.Format("Keyboard couldn't be opened!\n\n{0}\n\nFor more details look at log.", e.Message);
                ShowError(msg);
                Application.Exit();
            }
        }

        #endregion

        #region GridSwitch

        protected void OnGridSwitch(Object sender, GridSwitchEventArgs e)
        {
            logger.Debug("OnGridSwitch({0}) received... (from {1})", e.GridId, sender.GetType().Name);

            if (Monitor.TryEnter(inputLock))
            {
                try
                {
                    if (keyboard == null)
                        throw new Exception("Grid switch is not possible, there is no keyboard opened!");

                    Grid grid = keyboard[e.GridId];  // loads grid if not already cached

                    if (AlreadyActive(grid))
                        return;
                    else
                        DeactivateGrid();

                    ActivateGrid(grid);
                }
                catch (Exception ex)
                {
                    string msg = String.Format("Exception occurred while switching to grid '{0}'!", e.GridId);
                    LogAndShowError(msg, ex);
                }
                finally { Monitor.Exit(inputLock); }
            }
            else
                logger.Warn("Discarded grid switch event, because application is busy!");
        }

        private bool AlreadyActive(Grid grid)
        {
            if (grid == activeGrid)
            {
                logger.Warn("Grid switch aborted, cause given grid ('{0}') is already active!", grid.Id);
                return true;
            }
            else
                return false;
        }

        private void DeactivateGrid()
        {
            if (activeGrid == null)
                throw new InvalidOperationException("Grid cannot be deactivated, because there is no active grid!");

            if (scanThread != null)
                StopScanner();

            statusController.Unwire(activeGrid);
            actionController.Unwire(activeGrid);

            if (clickTrigger != null)
                activeGrid.Drawer.Click -= clickTrigger.OnClick;

            activeGrid = null;
        }

        private void ActivateGrid(Grid grid)
        {
            if (activeGrid != null)
                throw new InvalidOperationException("Grid cannot be activated, because there is already an active grid!");

            statusController.Wire(grid);
            actionController.Wire(grid);

            if (clickTrigger != null)
                grid.Drawer.Click += clickTrigger.OnClick;

            Form.SwitchDrawer(grid.Drawer, keyboard.Name, grid.Id);

            activeGrid = grid;

            if (activeGrid.Scanner != null)
                StartScanner();
        }

        #endregion

        #region Scanner

        protected void StartScannerByScannerAction()
        {
            if (Monitor.TryEnter(inputLock))
            {
                try
                {
                    if (CanStartScanner())
                        StartScanner();
                }
                catch (Exception e)
                {
                    string msg = "Exception occurred while starting scanner!";
                    LogAndShowError(msg, e);
                }
                finally { Monitor.Exit(inputLock); }
            }
            else
                logger.Warn("Discarded start scanner action, because application is busy!");
        }

        private bool CanStartScanner()
        {
            string msg = null;

            if (activeGrid.Scanner == null)  // possible when no scanner is defined or it's deactivated and called from ScannerAction
                msg = "Can't start scanner, cause there is none defined!";

            if (scanThread != null)
                msg = "Can't start scanner, cause it seems already running!";

            if (msg == null)
                return true;
            else
            {
                logger.Warn(msg);
                ShowError(msg);
                return false;
            }
        }

        protected void StopScannerByScannerAction()
        {
            if (Monitor.TryEnter(inputLock))
            {
                try
                {
                    if (CanStopScanner())
                    {
                        StopScanner();
                        statusController.ClearSelection();
                    }
                }
                catch (Exception e)
                {
                    string msg = "Exception occurred while stopping scanner!";
                    LogAndShowError(msg, e);
                }
                finally { Monitor.Exit(inputLock); }
            }
            else
                logger.Warn("Discarded stop scanner action, because application is busy!");
        }

        private bool CanStopScanner()
        {
            if (scanThread == null)
            {
                string msg = "Can't stop scanner, cause there is none active!";
                logger.Warn(msg);
                ShowError(msg);
                return false;
            }
            else
                return true;
        }

        private void StartScanner()
        {
            logger.Trace("Starting scanner thread...");

            scanThread = new Thread(activeGrid.Scanner.DoScan);
            scanThread.Name = "Scanner";
            scanThread.IsBackground = true;
            scanThread.Start();
        }

        /// <remarks>
        /// About interrupting and aborting: http://msdn.microsoft.com/en-us/library/tttdef8x%28v=vs.110%29.aspx
        /// How to handle abortion: http://msdn.microsoft.com/en-us/library/cyayh29d%28v=vs.110%29.aspx
        /// </remarks>
        private void StopScanner()
        {
            logger.Trace("Stopping scanner thread...");

            if (scanThread.IsAlive)
                scanThread.Abort();

            scanThread.Join();
            scanThread = null;
        }

        #endregion

        #region Trigger

        /// <summary>
        /// Let the controller handle all trigger events in the first place. This makes it easier to swap the scanner on grid change.
        /// If the trigger event contains a specific button as argument, execute it's actions. Otherwise notify scanner about trigger event.
        /// </summary>
        protected void OnTrigger(Object sender, TriggerEventArgs e)
        {
            logger.Debug("OnTrigger({1})... (received from {0})", sender.GetType().Name, e.ButtonId);

            if (activeGrid == null)
            {
                logger.Warn("Discarded trigger event, because no grid is active!");
                return;
            }

            if (Monitor.TryEnter(inputLock))
            {
                try
                {
                    if (e.ContainsButtonId())
                        ExplicitButtonTrigger(e.ButtonId);
                    else
                        Trigger();
                }
                catch (Exception ex)
                {
                    string msg = "Unrecognized exception occurred in OnTrigger()!";
                    logger.Error(ExceptionUtil.Format(msg, ex));
                }
                finally { Monitor.Exit(inputLock); }
            }
            else
                logger.Warn("Discarded trigger event, because application is busy!");
        }

        /// <summary>
        /// Caused e.g. by <see cref="ClickTrigger"/> or via network command.
        /// </summary>
        private void ExplicitButtonTrigger(string buttonId)
        {
            Button btn = activeGrid[buttonId];

            // run asynchronous, don't wait
            // use own task because if UI thread, it crashes otherwise
            Task.Run(() => actionController.RunActionsAsync(btn));
        }

        private void Trigger()
        {
            if (scanThread != null && activeGrid.Scanner != null)
                activeGrid.Scanner.OnTrigger();
            else
                logger.Warn("Discarded trigger event, because no scanner is active!");
        }

        #endregion

        #region Messages

        private void LogAndShowError(string title, Exception e)
        {
            logger.Error(ExceptionUtil.Format(title, e));
            ShowError(String.Format("{0}\n\n{1}", title, e.Message));
        }

        private void LogAndShowWarning(string msg, Exception e = null)
        {
            if (e == null)
            {
                logger.Warn(msg);
                ShowWarning(msg);
            }
            else
            {
                logger.Warn("{0}\n{1}", msg, e);
                ShowWarning(String.Format("{0}\n\n{1}", msg, e.Message));
            }
        }

        private void ShowError(string msg)
        {
            ShowMessage(msg, "Error", MessageBoxIcon.Error);
        }

        private void ShowWarning(string msg)
        {
            ShowMessage(msg, "Warning", MessageBoxIcon.Warning);
        }

        private void ShowMessage(string msg, string title = "Info", MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            MethodInvoker showMsg = delegate()
            {
                // workaround for situation where message is shown while key down
                if (keyTrigger != null)
                    keyTrigger.Cancel();

                MessageBox.Show(msg, title, MessageBoxButtons.OK, icon);
            };

            if (Form == null)
                showMsg.Invoke();
            else
                Form.Invoke(showMsg);
        }

        #endregion
    }
}
