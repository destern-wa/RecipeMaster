using System.Threading.Tasks;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;

namespace RecipeMaster.ViewModel
{
    [POCOViewModel] public class MessageBarViewModel
    {
        /* POCO View Models makes MVVM easier! https://docs.devexpress.com/WPF/17352/mvvm-framework/viewmodels/poco-viewmodels
         *  
         * A property automagically becomes bindable when:
         * (1) The property is auto-implemented﻿; AND
         * (2) The property has the virtual modifier; AND
         * (3) The property has a public getter, and a protected or public setter.
         * https://docs.devexpress.com/WPF/17352/mvvm-framework/viewmodels/poco-viewmodels#bindableproperties
         * 
         * A command is automagically generated when:
         * (1) There is a public void method (with zero/one parameters); AND
         * (2) There is a public bool method (same zero/one parameters), with same method name
         *     as the method in (1) but prefixed with "Can"
         * The resulting command is the first method name, suffixed with "Command".
         * https://docs.devexpress.com/WPF/17353/mvvm-framework/commands/delegate-commands#poco
         */

        /// <summary>
        /// Instantiates a new instance
        /// </summary>
        public static MessageBarViewModel Create()
        {
            return ViewModelSource.Create(() => new MessageBarViewModel());
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>Protected to prevent creating the View Model without the ViewModelSource</remarks>
        protected MessageBarViewModel() { }

        /// <summary>
        /// Current error message
        /// </summary>
        public virtual string ErrorMessage { get; set; } = string.Empty;
        /// <summary>
        /// Current success message
        /// </summary>
        public virtual string SuccessMessage { get; set; } = string.Empty;

        /// <summary>
        /// Displays an error message for the specified time, then clears it 
        /// </summary>
        /// <param name="message">Message to display</param>
        /// <param name="displayTime">Duration to display for, in milliseconds</param>
        /// <returns>Task that completes once the message is cleared</returns>
        public Task SetTemporaryErrorMessage(string message, int displayTime)
        {
            ErrorMessage = message;
            return Task.Delay(displayTime).ContinueWith(
                _ => ErrorMessage = string.Empty,
                TaskScheduler.FromCurrentSynchronizationContext()
            );
        }

        /// <summary>
        /// Displays a success message for the specified time, then clears it 
        /// </summary>
        /// <param name="message">Message to display</param>
        /// <param name="displayTime">Duration to display for, in milliseconds</param>
        /// <returns>Task that completes once the message is cleared</returns>
        public Task SetTemporarySuccessMessage(string message, int displayTime)
        {
            SuccessMessage = message;
            return Task.Delay(displayTime).ContinueWith(
                _ => SuccessMessage = string.Empty,
                TaskScheduler.FromCurrentSynchronizationContext()
            );
        }

        /// <summary>
        /// Number of seconds corresponding to a short display time
        /// </summary>
        public int ShortDisplayTime { get; } = 1200;

        /// <summary>
        /// Number of seconds corresponding to a medium display time
        /// </summary>
        public int MediumDisplayTime { get; } = 2500;

        /// <summary>
        /// Number of seconds corresponding to a long display time
        /// </summary>
        public int LongDisplayTime { get; } = 3800;
    }
}
