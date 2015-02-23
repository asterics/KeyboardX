using NLog;
using Player.Model;
using System;
using System.Collections.Generic;

namespace Player.Core.Status
{
    class GridStatusImpl : GridStatus
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        private Dictionary<string, ButtonStatusImpl> statusDict;

        public ButtonStatus this[string buttonId]
        {
            get { return statusDict[buttonId]; }
        }


        public GridStatusImpl(GridModel grid)
        {
            statusDict = new Dictionary<string, ButtonStatusImpl>();
            foreach (var btn in grid)
                statusDict[btn.Id] = new ButtonStatusImpl();
        }


        /// <summary>
        /// Clears selection flag for all selected buttons and returns them.
        /// </summary>
        public ButtonGroup ClearSelection()
        {
            ButtonGroup changes = new ButtonGroup();

            foreach (var entry in statusDict)
            {
                if (entry.Value.Selected)
                {
                    entry.Value.Update(ButtonStatus.Flags.Selected, false);
                    changes.Add(entry.Key);
                }
            }

            changes.Seal();
            return changes;
        }

        public void UpdateStatus(ButtonGroup buttons, ButtonStatus.Flags flag, bool state)
        {
            foreach (var btn in buttons)
            {
                try { statusDict[btn].Update(flag, state); }
                catch (KeyNotFoundException)
                {
                    logger.Error(String.Format("Status couldn't be updated, cause button '{0}' doesn't exist!", btn));
                }
            }
        }

        public void ResetButtons()
        {
            foreach (var btnStatus in statusDict.Values)
                btnStatus.Reset();
        }
    }
}
