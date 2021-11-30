using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using RecipeMaster.Model;

namespace RecipeMaster.ViewModel
{
    /// <summary>
    /// View model for the recipe searcher view
    /// </summary>
    [POCOViewModel] public class RecipeSearcherViewModel
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
        public static RecipeSearcherViewModel Create()
        {
            return ViewModelSource.Create(() => new RecipeSearcherViewModel());
        }

        /// <summary>
        /// Constructor that populates with all ingredients from the database.
        /// </summary>
        /// <remarks>Protected to prevent creating the View Model without the ViewModelSource</remarks>
        protected RecipeSearcherViewModel()
        {
            try
            {
                Ingredients = Ingredient.DatabaseCollection.GetAll();
            }
            catch (Database.RecipeDatabaseException e)
            {
                MessageBar.ErrorMessage = e.Message;
            }
        }

        /// <summary>
        /// Action to be invoked prior to the view closing
        /// </summary>
        private Action CloseAction { get; set; }
        /// <summary>
        /// Sets the <see cref="CloseAction"/>
        /// </summary>
        /// <param name="closeAction">Action to be invoked prior to the view closing</param>
        internal void SetCloseAction(Action closeAction)
        {
            CloseAction = closeAction;
        }

        /// <summary>
        /// Gets the preparation time limit, converetd from hours if required
        /// </summary>
        /// <returns>Preparation time limit in minutes</returns>
        private int GetPrepTimeLimitInMinutes() => TimeIsInHours ? PrepTimeLimit * 60 : PrepTimeLimit;

        /// <summary>
        /// Checks if a recipe is match for the simple search text
        /// </summary>
        /// <param name="r">Recipe to check</param>
        /// <returns>Recipe is a match</returns>
        private bool SimpleSearchMatch(Recipe r)
        {
            string searchTerm = SimpleSearchText?.Trim().ToLower();
            // Test for a matching name
            bool NameMatch = r.Name.ToLower().Contains(searchTerm);
            if (NameMatch) return true;

            // Otherwise, see if any ingredients match
            return r.Ingredients.Any(
                ingredient => ingredient.Name.ToLower().Contains(searchTerm)
            );
        }

        /// <summary>
        /// Returns a function that will peform an advanced search
        /// </summary>
        /// <param name="matchAll">If all search fields must match</param>
        /// <param name="searchName">Search term for the name</param>
        /// <param name="searchIngredient">Ingredient to search for</param>
        /// <param name="minTimeLimit">Miminmum prep time, in minutes</param>
        /// <param name="maxTimeLimit">Maximum prep time, in minutes</param>
        /// <returns>function that takes in a Recipe and returns true if it is a match, false otherwise</returns>
        private static Func<Recipe, bool> AdvancedSearchMatch(bool matchAll, string searchName, Ingredient searchIngredient, int minTimeLimit, int maxTimeLimit = int.MaxValue)
        {
            // Function to check if the name is a match
            Func<Recipe, bool> nameMatch = r => r.Name.ToLower().Contains(searchName?.Trim().ToLower());

            // Function to check if the ingredient is a match
            Func<Recipe, bool> ingredientMatch = r => r.Ingredients.Any(i => i == searchIngredient);

            // Function to check if the time is a match
            Func<Recipe, bool> timeMatch = r => minTimeLimit <= r.PrepTime && r.PrepTime <= maxTimeLimit;

            if (matchAll)
            {
                // Must match all non-null criteria
                return r => (searchName == null || nameMatch(r)) &&
                    (searchIngredient == null || ingredientMatch(r)) &&
                    timeMatch(r);
            }
            else
            {
                // Can match any of the non-null criteria
                return r => (searchName != null && nameMatch(r)) ||
                    (searchIngredient != null && ingredientMatch(r)) ||
                    ((minTimeLimit > 0 || maxTimeLimit < int.MaxValue) && timeMatch(r));
            }
        }

        /// <summary>
        /// View model to help give the user success/failure messages
        /// </summary>
        public virtual MessageBarViewModel MessageBar { get; set; } = MessageBarViewModel.Create();
        /// <summary>
        /// In advanced search mode
        /// </summary>
        public virtual bool IsAdvancedSearch { get; set; }
        /// <summary>
        /// Require all search queries to match, for advanced search mode
        /// </summary>
        public virtual bool MatchAll { get; set; } = false;
        /// <summary>
        /// Search text for simple search mode
        /// </summary>
        public virtual string SimpleSearchText { get; set; }
        /// <summary>
        /// Serach text for name
        /// </summary>
        public virtual string NameSearchText { get; set; }
        /// <summary>
        /// All known ingredients
        /// </summary>
        public virtual List<Ingredient> Ingredients { get; set; }
        /// <summary>
        /// Selected ingredient to search for
        /// </summary>
        public virtual Ingredient SelectedIngredient { get; set; }
        /// <summary>
        /// Preparation time limit
        /// </summary>
        public virtual int PrepTimeLimit { get; set; }
        /// <summary>
        /// Limit is a mimimum if true, or a maximum if false
        /// </summary>
        public virtual bool LimitIsMinimum { get; set; }
        /// <summary>
        /// Time unit for PrepTimeLimit is hours if true, or minutes if false 
        /// </summary>
        public virtual bool TimeIsInHours { get; set; }
        /// <summary>
        /// Number of results for search criteria
        /// </summary>
        public virtual int NumberOfResults { get; set; }
        /// <summary>
        /// View model for browsing the results
        /// </summary>
        public virtual RecipeBrowserViewModel ResultsViewDataContext { get; set; } = RecipeBrowserViewModel.Create(new List<Recipe>());

        // CancelCommand
        /// <summary>
        /// Clears all search criteria and hides results
        /// </summary>
        public void Cancel()
        {
            SimpleSearchText = string.Empty;
            NameSearchText = null;
            SelectedIngredient = null;
            PrepTimeLimit = 0;
            NumberOfResults = 0;
        }
        // Can always cancel
        public bool CanCancel() => true;

        // SearchCommand
        /// <summary>
        /// Peforms a search and displays the results, or a message if there are no results
        /// </summary>
        public void Search()
        {
            List<Recipe> results;
            try
            {
                if (IsAdvancedSearch)
                {
                    results = Recipe.DatabaseCollection.GetWhere(AdvancedSearchMatch(
                        MatchAll,
                        NameSearchText,
                        SelectedIngredient,
                        LimitIsMinimum ? GetPrepTimeLimitInMinutes() : int.MinValue,
                        LimitIsMinimum ? int.MaxValue : GetPrepTimeLimitInMinutes()
                    ));
                }
                else
                {
                    results = Recipe.DatabaseCollection.GetWhere(SimpleSearchMatch);

                }
                NumberOfResults = results.Count;
                ResultsViewDataContext.SetRecipes(results);
                if (NumberOfResults == 0)
                {
                    MessageBar.SetTemporaryErrorMessage("There are no matching recipes. Try a different search.", MessageBar.LongDisplayTime);
                }
            } catch (Database.RecipeDatabaseException e)
            {
                MessageBar.SetTemporaryErrorMessage(e.Message, MessageBar.LongDisplayTime);
                NumberOfResults = 0;
            }
        }
        /// <summary>
        /// Checks if search can be performed
        /// </summary>
        /// <returns>If search can be performed</returns>
        public bool CanSearch()
        {
            if (IsAdvancedSearch)
            {
                // At least one search field must be set
                return !string.IsNullOrWhiteSpace(NameSearchText) ||
                     SelectedIngredient != null ||
                     PrepTimeLimit > 0;
            }
            else
            {
                // Must have some text to search for
                return !string.IsNullOrWhiteSpace(SimpleSearchText);
            }
        }

        // ClearNameCommand
        /// <summary>
        /// Clears the name search text
        /// </summary>
        public void ClearName() => NameSearchText = null;
        // Can always clear the text
        public bool CanClearName() => true;

        // ClearIngredientCommand
        /// <summary>
        /// Clears the selected ingredient
        /// </summary>
        public void ClearIngredient() => SelectedIngredient = null;
        // Can always clear the selected ingredient
        public bool CanClearIngredient() => true;

        // ClearTimeLimitCommand
        /// <summary>
        /// Clears the time limit
        /// </summary>
        public void ClearTimeLimit()
        {
            PrepTimeLimit = 0;
            LimitIsMinimum = true;
        }
        // Can always clear the time limit
        public bool CanClearTimeLimit() => true;
    }
}
