using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using RecipeMaster.Model;

namespace RecipeMaster.ViewModel
{
    /// <summary>
    /// View Model for the dashboard view
    /// </summary>
    [POCOViewModel] public class DashboardViewModel
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
        public static DashboardViewModel Create()
        {
            return ViewModelSource.Create(() => new DashboardViewModel());
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>Protected to prevent creating the View Model without the ViewModelSource</remarks>
        protected DashboardViewModel() {
            UpdateCounts();
        }

        /// <summary>
        /// Updates the counts of recipes, ingredients, and favourites
        /// </summary>
        public void UpdateCounts()
        {
            try
            {
                NumberOfRecipes = Recipe.DatabaseCollection.Count();
                NumberOfIngredients = Ingredient.DatabaseCollection.Count();
                NumberOfFavourites = Favourites.GetFavourites().Length;
            }
            catch (Database.RecipeDatabaseException e)
            {
                MessageViewModel.ErrorMessage = e.Message;
            }
        }

        /// <summary>
        /// Number of recipes in the database
        /// </summary>
        public virtual int NumberOfRecipes { get; set; } = 0;

        /// <summary>
        /// Number of ingredients in the datanbase
        /// </summary>
        public virtual int NumberOfIngredients { get; set; } = 0;

        /// <summary>
        /// Number of favourite recipes
        /// </summary>
        public virtual int NumberOfFavourites { get; set; } = 0;

        /// <summary>
        /// View Model for the message bar
        /// </summary>
        public virtual MessageBarViewModel MessageViewModel { get; protected set; } = MessageBarViewModel.Create();
    }
}
