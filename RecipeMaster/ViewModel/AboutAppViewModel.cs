using System;
using System.Diagnostics;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;

namespace RecipeMaster.ViewModel
{
    /// <summary>
    /// ViewModel for the AboutAppView 
    /// </summary>
    [POCOViewModel] public class AboutAppViewModel
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
        public static AboutAppViewModel Create()
        {
            return ViewModelSource.Create(() => new AboutAppViewModel());
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>Protected to prevent creating the View Model without the ViewModelSource</remarks>
        protected AboutAppViewModel() { }

        /// <summary>
        /// Opens a URI in the system's default browser
        /// </summary>
        /// <param name="uri">URI to visit</param>
        public void VisitHyperlink(Uri uri)
        {
            Process.Start(new ProcessStartInfo(uri.AbsoluteUri));
        }
        // Can... function so that a command is automagically generated
        public bool CanVisitHyperlink(Uri _uri) => true;
    }
}
