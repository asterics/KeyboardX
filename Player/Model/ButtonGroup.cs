using System;
using System.Collections;
using System.Collections.Generic;

namespace Player.Model
{
    /// <summary>
    /// Represents a group of buttons by their id.
    /// Implements <see cref="IEnumerable"/>, so buttons in group can be comfortably enumerated.
    /// </summary>
    /// <remarks>
    /// Put in <c>Player.Model</c> cause is needed by nearly every namespace.
    /// </remarks>
    public class ButtonGroup : IEnumerable<string>
    {
        public static readonly ButtonGroup Empty = new ButtonGroup(new string[0]);


        private HashSet<string> buttonIds;

        public int Count
        {
            get { return buttonIds.Count; }
        }

        public bool Sealed { get; private set; }


        /// <summary>
        /// Creates a <see cref="ButtonGroup"/> object which is not sealed and button ids can be added.
        /// </summary>
        public ButtonGroup()
        {
            buttonIds = new HashSet<string>();
            Sealed = false;
        }

        /// <summary>
        /// Creates a <see cref="ButtonGroup"/> object with given button id, which is sealed, no more button ids can be added.
        /// </summary>
        /// <param name="btnId">The one and only button id for this group.</param>
        public ButtonGroup(string btnId)
        {
            buttonIds = new HashSet<string>();
            buttonIds.Add(btnId);
            Sealed = true;
        }

        /// <summary>
        /// Creates a <see cref="ButtonGroup"/> object with given button ids, which is sealed, no more button ids can be added.
        /// </summary>
        /// <param name="btnIds">The button ids that should be contained in this group.</param>
        public ButtonGroup(IEnumerable<string> btnIds)
        {
            buttonIds = new HashSet<string>(btnIds);
            buttonIds.Remove(null);
            Sealed = true;
        }


        public void Add(string btnId)
        {
            CheckSealed();

            buttonIds.Add(btnId);
        }

        public void Add(IEnumerable<string> btnIds)
        {
            CheckSealed();

            if (btnIds == Empty)
                return;

            buttonIds.UnionWith(btnIds);
            buttonIds.Remove(null);
        }

        private void CheckSealed()
        {
            if (Sealed)
            {
                string msg = String.Format("{0} is sealed! No button ids are allowed to be added after it's sealed!", GetType().Name);
                throw new InvalidOperationException(msg);
            }
        }

        public bool Equals(ButtonGroup bg)
        {
            return buttonIds.SetEquals(bg.buttonIds);
        }

        // explicitly define interface implementation
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return buttonIds.GetEnumerator();
        }

        public bool IsEmpty()
        {
            return (Count == 0);
        }

        public void Seal()
        {
            Sealed = true;
        }
    }
}
