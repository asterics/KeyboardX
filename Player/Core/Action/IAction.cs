using Player.Model.Action;
using System;

namespace Player.Core.Action
{
    /// <remarks>
    /// Actions are different to model. The possible actions will be very different in implementation, the only certain thing is, 
    /// that there will be "something to do" (<see cref="DoAction()"/>).
    /// 
    /// At model load time just action parameter objects are created.
    /// Objects inheriting from this interface are not created till grid is loaded (lazy loading). 
    /// 
    /// This interface is called 'IAction' instead of simply 'Action', because otherwise there would be naming conflict.
    /// </remarks>
    public interface IAction
    {
        bool RunSyncronous { get; }

        // No additional parameter should be necessary when executing the action. Parameters are already set.
        void DoAction();
    }
}
