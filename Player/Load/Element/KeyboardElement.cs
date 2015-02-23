using System;

namespace Player.Load.Element
{
    /// <summary>
    /// Base type for all elements of a keyboard.
    /// </summary>
    public interface KeyboardElement
    {
        /// <remarks>
        /// There is a contract between the Loader and it's client. The Loader should only return valid keyboard models. Because validation of the 
        /// keyboard file via XSD is not powerful enough to detect all possible problems, this function is added to every keyboard element.
        /// 
        /// All in all there are 3 stages of validation:
        ///   1. XSD validation
        ///   2. validation checks while parsing
        ///   3. validation checks on the keyboard elements (this function)
        ///   
        /// After the 3rd stage (i.e. calling this function on keyboard model) it should be guaranteed that a valid keyboard model was loaded.
        /// 
        /// If we have several grids in one keyboard and want to use lazy loading mechanism, the question is when a logical error in a grid should 
        /// be reported. Either on loading time of the keyboard file, in other words here at this function, or when the grid is really loaded.
        /// </remarks>
        void Validate();
    }
}
