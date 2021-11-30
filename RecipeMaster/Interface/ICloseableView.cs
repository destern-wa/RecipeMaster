using System;

namespace RecipeMaster.Interface
{
    /// <summary>
    /// Represents a view that can have a closing action assigned, that will be called
    /// when the view attempts to close itself
    /// </summary>
    public interface ICloseableView
    {
        /// <summary>
        /// Assigns a closing action
        /// </summary>
        /// <param name="closeAction">Action to be called when the view closes</param>
        void SetCloseAction(Action closeAction);
    }
}