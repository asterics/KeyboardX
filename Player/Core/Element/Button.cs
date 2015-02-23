using NLog;
using Player.Core.Action;
using Player.Model;
using Player.Model.Action;
using System;
using System.Collections.Generic;

namespace Player.Core.Element
{
    /// <summary>
    /// Represents a button element in <c>Player.Core</c>. Wraps a <see cref="ButtonModel"/> instance.
    /// </summary>
    public class Button
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        // if needed, provide access to model per getter
        private ButtonModel model;

        public string Id
        {
            get { return model.Id; }
        }

        private IAction[] actions;

        public IAction[] Actions
        {
            get { return actions; }
        }


        public Button(ButtonModel model)
        {
            this.model = model;
            InitActions(model.ActionParams);
        }


        private void InitActions(ActionParameter[] actionParams)
        {
            if (actionParams == null || actionParams.Length == 0)
            {
                actions = new IAction[0];
                return;
            }

            actions = new IAction[actionParams.Length];
            for (int i = 0; i < actions.Length; i++)
            {
                try { actions[i] = ActionFactory.CreateAction(actionParams[i]); }
                catch (Exception e)
                {
                    logger.Error("Error while initializing actions for button '{0}'. {1}", Id, e.Message);
                }
                
            }
        }
    }
}
