using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using RecipeMaster.Model;

namespace RecipeMaster.ViewModel
{
    /// <summary>
    /// View model for the recipe browser view
    /// </summary>
    [POCOViewModel] public class RecipeBrowserViewModel
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
        /// Instantiates a new instance and populates with all recipes from the database.
        /// </summary>
        /// <returns>Instantiated instance</returns>
        public static RecipeBrowserViewModel Create()
        {
            return ViewModelSource.Create(() => new RecipeBrowserViewModel());
        }

        /// <summary>
        /// Instantiates a new instance and populates with a specific list of recipes.
        /// </summary>
        /// <param name="recipes">List of recipes, or null for an empty list</param>
        /// <returns>Instantiated instance</returns>
        public static RecipeBrowserViewModel Create(List<Recipe> recipes)
        {
            return ViewModelSource.Create(() => new RecipeBrowserViewModel(recipes));
        }
        /// <summary>
        /// Constructor that populates with all recipes from the database.
        /// </summary>
        /// <remarks>Protected to prevent creating the View Model without the ViewModelSource</remarks>
        protected RecipeBrowserViewModel()
        {  
            try
            {
                List<Recipe> allRecipes = Recipe.DatabaseCollection.GetAll();
                SetRecipes(allRecipes);
                SelectedSortOption = SortOption.Name.ToString();
                Sort();
            }
            catch (Database.RecipeDatabaseException e)
            {
                MessageBar.ErrorMessage = e.Message;
            }
        }
        /// <summary>
        /// Constructor that populates with specified recipes.
        /// </summary>
        /// <remarks>Protected to prevent creating the View Model without the ViewModelSource</remarks>
        protected RecipeBrowserViewModel(List<Recipe> recipes)
        {
            // Setup and sort the collection of recipes, if any were passed in
            if (recipes != null)
            {
                SetRecipes(recipes);
                SelectedSortOption = SortOption.Name.ToString();
                Sort();
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
        /// Collection of recipes to display in the browser
        /// </summary>
        private readonly ObservableCollection<Recipe> recipes = new ObservableCollection<Recipe>();
        /// <summary>
        /// Collection of recipes to display in the browser
        /// </summary>
        public virtual ObservableCollection<Recipe> Recipes
        {
            get => recipes;
            set => SetRecipes(value.ToList());
        }

        /// <summary>
        /// Sets the collection of recipes to display in the browser
        /// </summary>
        /// <param name="value">Recipes to display</param>
        public void SetRecipes(List<Recipe> value)
        {
            recipes.Clear();
            for (int i = 0; i < value.Count(); i++) recipes.Add(value.ElementAt(i));
        }

        /// <summary>
        /// Option for how to sort recipes
        /// </summary>
        enum SortOption
        {
            Name,
            Category,
            Time
        };
        /// <summary>
        /// Options for how to sort recipes
        /// </summary>
        public virtual Array SortOptions
        {
            get => Enum.GetValues(typeof(SortOption));
            protected set => Console.WriteLine(value);
        }

        /// <summary>
        /// Selected <see cref="SortOption"/>
        /// </summary>
        private SortOption selectedSortOption = SortOption.Name;
        /// <summary>
        /// Selected sort option
        /// </summary>
        public virtual string SelectedSortOption
        {
            get => selectedSortOption.ToString();
            set
            {
                selectedSortOption = (SortOption)Enum.Parse(typeof(SortOption), value);
                Sort();
            }
        }

        /// <summary>
        /// Sort is in ascending order
        /// </summary>
        private bool sortAscending = true;
        /// <summary>
        /// Sort is in descending order
        /// </summary>
        public virtual bool SortDescending
        {
            get => !sortAscending;
            set
            {
                sortAscending = !value;
                Sort();
            }
        }

        /// <summary>
        /// View model to help give the user success/failure messages
        /// </summary>
        public virtual MessageBarViewModel MessageBar { get; protected set; } = MessageBarViewModel.Create();

        /// <summary>
        /// View model for editing a recipe
        /// </summary>
        public virtual RecipeEditorViewModel EditorViewModel { get; protected set; } = RecipeEditorViewModel.Create();

        /// <summary>
        /// A recipe is being edited
        /// </summary>
        public virtual bool IsEditing { get; protected set; } = false;

        // ToggleFavCommand
        /// <summary>
        /// Toggles the favourite status of the selected recipe
        /// </summary>
        /// <param name="selected">selected recipe</param>
        public void ToggleFav(object selected)
        {
            Recipe recipe = selected as Recipe;
            (bool saved, string message, Exception e) = recipe.IsFavourite ? Favourites.RemoveFavourite(recipe.Id) : Favourites.AddFavourite(recipe.Id);
            if (saved)
            {
                string action = recipe.IsFavourite ? "added to" : "removed from";
                MessageBar.SetTemporarySuccessMessage($"Recipe successfully {action} favourites", MessageBar.ShortDisplayTime);
                Sort(); // refreshes the view
            }
            else
            {
                string action = !recipe.IsFavourite ? "added to" : "removed from";
                string errorMsg = $"Recipe could not be {action} favourites";
                if (message != null) errorMsg += $"\n{message}";
                if (e != null) errorMsg += $"\n{e.Message}";
                MessageBar.SetTemporaryErrorMessage(errorMsg, MessageBar.LongDisplayTime);
            }
        }
        /// <summary>
        /// Checks if it is possible to toggle the favourite status of the selected recipe
        /// </summary>
        /// <param name="selected">Selected recipe</param>
        /// <returns>Can toggle the favourite status</returns>
        public bool CanToggleFav(object selected)
        {
            return selected != null;
        }

        // EditRecipeCommand
        /// <summary>
        /// Presents the selected recipe for editing
        /// </summary>
        /// <param name="selected">selected recipe</param>
        public void EditRecipe(object selected)
        {
            // Instantiate a new editor view model for the selected recipe
            EditorViewModel = RecipeEditorViewModel.Create((Recipe)selected);
            // When done, turn off editing mode and re-sort the collection
            EditorViewModel.SetCloseAction(() =>
            {
                IsEditing = false;
                Sort(); // refreshes the view
            });
            // Turn on editiing mode
            IsEditing = true;
        }
        /// <summary>
        /// Checks if it is possible to edit the selected recipe
        /// </summary>
        /// <param name="selected">Selected recipe</param>
        /// <returns>Can edit the selected recipes</returns>
        public bool CanEditRecipe(object selected)
        {
            return selected != null;
        }

        // DeleteRecipeCommand
        /// <summary>
        /// Deletes the selected recipe
        /// </summary>
        /// <param name="selected">selected recipe</param>
        public void DeleteRecipe(object selected)
        {
            Recipe recipeToDelete = (Recipe)selected;
            try
            {
                // Remove from favourites, if it was a favourites
                if (Favourites.IsFavourite(recipeToDelete.Id))
                {
                    Favourites.RemoveFavourite(recipeToDelete.Id);
                }
                // Delete from databse
                Recipe.DatabaseCollection.Delete(recipeToDelete.Id);
                // Remove from Observable collection
                SetRecipes(Recipes.Where(recipe => recipe != recipeToDelete).ToList());
                MessageBar.SetTemporarySuccessMessage("Recipe deleted", MessageBar.ShortDisplayTime);
            }
            catch (Database.RecipeDatabaseException e)
            {
                MessageBar.SetTemporaryErrorMessage(e.Message, MessageBar.LongDisplayTime);
            }
        }
        /// <summary>
        /// Checks if it is possible to delete the selected recipe
        /// </summary>
        /// <param name="selected">Selected recipe</param>
        /// <returns>Can delete the selected recipes</returns>
        public bool CanDeleteRecipe(object selected)
        {
            return selected != null;
        }
        
        /// <summary>
        /// Sorts the recipes collection according to the selected sort option and ordering
        /// </summary>
        private void Sort() => Sort(selectedSortOption, sortAscending);
        /// <summary>
        /// Sorts the recipe collection
        /// </summary>
        /// <param name="by">Sort option</param>
        /// <param name="isAscending">Sort in ascending order</param>
        private void Sort(SortOption by, bool isAscending)
        {

            List<Recipe> sorted;
            switch (by)
            {
                case SortOption.Name:
                    sorted = Recipes.OrderBy(r => r.Name).ToList();
                    break;
                case SortOption.Category:
                    sorted = Recipes.OrderBy(r => r.Category?.Name).ToList();
                    break;
                case SortOption.Time:
                    sorted = Recipes.OrderBy(r => r.PrepTime).ToList();
                    break;
                default: // only needed so that compiler knows the variable sorted has been assiged
                    sorted = Recipes.ToList();
                    break;
            }
            if (!isAscending) sorted.Reverse();
            SetRecipes(sorted);
        }
    }
}
