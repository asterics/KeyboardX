using System;

namespace Player.Model.Action
{
    /// <summary>
    /// Shared interface for all action parameter classes.<para/>
    /// An action parameter object should contain all information that is necessary for executing the action.
    /// </summary>
    public interface ActionParameter
    {
        bool RunSynchronous { get; }
    }
}
