using System;

namespace Player.Core.Scan
{
    /// <summary>
    /// Responsible for scanning a grid, so the user is able to choose a button.
    /// </summary>
    /// <remarks>
    /// When does a scanner need to know about the state of a grid?
    /// => A scanner has to know which element is selected and which comes next.
    /// 
    /// A scanner needs a possibility to get stopped and restarted. This is needed for grid switch.
    /// => This is done by stopping scanner thread. The scanner object has not necessarily to deal with it.
    /// 
    /// What should happen if a trigger happens while the scanner is currently working?
    /// => Either the triggers are kept (via integer counter or via blocking, yields the same result) or the triggers are refused.
    ///    The first trigger could be easily kept and later consumed by the scanner, but the 2nd and forth triggers that arrive while scanner is 
    ///    busy, what sense does it make to keep them. If scanner gets ready again it would do a lot of crazy uncontrolled work then. But at least 
    ///    there should be some log message that trigger is discarded.
    /// </remarks>
    /// TODOs:
    ///  TODO 5: [scan] refactor whole scanning process, see https://github.com/s3huber/KeyboardX/issues/1
    ///  TODO 4: [scan] show error message when exception occurs while scanning (and scanner is broken)
    interface Scanner
    {
        event EventHandler<ButtonPressEventArgs> ButtonPress;

        event EventHandler<SelectionEventArgs> Selection;

        /// <summary>
        /// A <see cref="Scanner"/> should be run by another thread. This is the method that could be used therefore.
        /// </summary>
        void DoScan();

        /// <summary>
        /// Logically we have only one kind of a trigger, a binary trigger. So a scanner doesn't need to know anything about a trigger event, only 
        /// that it has happened. Therefore there is no argument.
        /// </summary>
        /// <remarks>
        /// Maybe it was not the best idea to keep this so simple. First, there is indeed the need for more than one switch (e.g. manual scanning).
        /// Second, to build more powerful features into scanner (e.g. timing) we need more details about trigger.
        /// </remarks>
        void OnTrigger();
    }
}
