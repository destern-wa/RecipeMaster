using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using RecipeMaster.Model;

namespace RecipeMaster.ViewModel
{
    /// <summary>
    /// Root view model for both the main window, as well as controlling child view models 
    /// </summary>
    [POCOViewModel] public class RootViewModel
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
        /// Instantiates a  new instance
        /// </summary>
        public static RootViewModel Create()
        {
            return ViewModelSource.Create(() => new RootViewModel());
        }
        /// <summary>
        /// Instantiates a  new instance
        /// </summary>
        /// <param name="discardUnsavedChanges">Action to discard unsaved changes that may be tracked by the database</param>
        /// <param name="refreshChildViews">Action to cause the child view to be renewed</param>
        /// <param name="databaseHasChanges">Function to check if there are unsaved (pending) changes for the database</param>
        /// <param name="prompt">Function to display a prompt to the user and get their response as a boolean</param>
        /// <returns></returns>
        public static RootViewModel Create(
            Action discardUnsavedChanges,
            Action refreshChildViews,
            Func<bool> databaseHasChanges,
            Func<string, bool> prompt
        )
        {
            return ViewModelSource.Create(() => new RootViewModel(discardUnsavedChanges, refreshChildViews, databaseHasChanges, prompt));
        }
        protected RootViewModel() { }
        protected RootViewModel(
            Action discardDatabaseChanges,
            Action refreshChildView,
            Func<bool> databaseHasChanges,
            Func<string, bool> prompt
        )
        {
            DiscardUnsavedChanges = discardDatabaseChanges;
            RefreshChildView = refreshChildView;
            DatabaseHasChanges = databaseHasChanges;
            Prompt = prompt;
        }
        /// <summary>
        /// Discards unsaved changes that may be tracked by the database
        /// </summary>
        Action DiscardUnsavedChanges;
        /// <summary>
        /// Refreshes the displayed child view
        /// </summary>
        /// <remarks>Needed because MVVM doesn't play nicely with a hamburger menu. See comments in MainWindow.xaml.cs</remarks>
        Action RefreshChildView;
        /// <summary>
        /// checks if there are unsaved (pending) changes for the database
        /// </summary>
        Func<bool> DatabaseHasChanges;
        /// <summary>
        /// Display a prompt to the user and returns their response as a boolean
        /// </summary>
        Func<string, bool> Prompt;
        /// <summary>
        /// Title for child view
        /// </summary>
        public enum ViewTitle
        {
            Dashboard,
            Editor,
            Browser,
            Searcher,
            Favourites,
            About
        }
        /// <summary>
        /// Title of current child view
        /// </summary>
        ViewTitle currentViewTitle = ViewTitle.Dashboard;
        /// <summary>
        /// Title of current child view
        /// </summary>
        public virtual ViewTitle CurrentViewTitle
        {
            get => currentViewTitle;
            protected set
            {
                currentViewTitle = value;
                // Force things in the view to actually update, since these props don't have setters
                this.RaisePropertyChanged(vm => vm.CurrentViewLabel);
                this.RaisePropertyChanged(vm => vm.CurrentViewIndex);
                RefreshChildView(); /* This gets the code-behind to create a new view */
            }
        }
        /// <summary>
        /// View model for the current child view
        /// </summary>
        public object CurrentViewModel
        {
            get
            {
                if (CurrentViewTitle == ViewTitle.Dashboard) return Dashboard;
                else if (CurrentViewTitle == ViewTitle.Editor) return Editor;
                else if (CurrentViewTitle == ViewTitle.Browser) return Browser;
                else if (CurrentViewTitle == ViewTitle.Searcher) return Searcher;
                else if (CurrentViewTitle == ViewTitle.Favourites) return FavouritesBrowser;
                else if (CurrentViewTitle == ViewTitle.About) return AboutApp;
                else return null;
            }
        }

        /* Child ViewModels */
        /// <summary>
        /// View model for dashbaord
        /// </summary>
        private DashboardViewModel dashboard = DashboardViewModel.Create();
        /// <summary>
        /// View model for editor
        /// </summary>
        private RecipeEditorViewModel editor = RecipeEditorViewModel.Create();
        /// <summary>
        /// View model for browser
        /// </summary>
        private RecipeBrowserViewModel browser = RecipeBrowserViewModel.Create();
        /// <summary>
        /// View model for favourites
        /// </summary>
        private RecipeBrowserViewModel favouritesBrowser = RecipeBrowserViewModel.Create();
        /// <summary>
        /// View model for searcher
        /// </summary>
        private RecipeSearcherViewModel searcher = RecipeSearcherViewModel.Create();
        /// <summary>
        /// View model for about
        /// </summary>
        private AboutAppViewModel aboutApp = AboutAppViewModel.Create();

        /// <summary>
        /// Checks if unsaved changes can be discarded
        /// </summary>
        /// <returns>Unsaved changes can be discarded</returns>
        private bool CanDiscardUnsavedChanges()
        {
            bool editorUnsaved = CurrentViewTitle == ViewTitle.Editor && editor.IsSaved==false;
            if (editorUnsaved || DatabaseHasChanges())
            {
                return Prompt("Discard unsaved changes?");
            }
            return true;
        }
        /// <summary>
        /// Handles keeping unsaved changes (instead of switching the current child view and losing changes)
        /// </summary>
        private void HandleKeepingUnsavedChanges()
        {
            // Get the view to resync with these properties (otherwsie the wrong item will 
            // be highlighted in the hamburger menu
            this.RaisePropertyChanged(vm => vm.CurrentViewTitle);
            this.RaisePropertyChanged(vm => vm.CurrentViewIndex);
        }

        /// <summary>
        /// Label for the current child view
        /// </summary>
        public virtual string CurrentViewLabel { get => CurrentViewTitle.ToString(); }

        /// <summary>
        /// Index of the current child view
        /// </summary>
        public virtual int CurrentViewIndex { get => (int)CurrentViewTitle; }

        /// <summary>
        /// View model to help give the user success/failure messages
        /// </summary>
        public virtual MessageBarViewModel MessageBar { get; protected set; } = MessageBarViewModel.Create();

        /// <summary>
        /// View model for the about page
        /// </summary>
        public virtual AboutAppViewModel AboutApp
        {
            get => aboutApp;
            protected set => aboutApp = value;
        }

        /// <summary>
        /// View model for the dashboard
        /// </summary>
        public virtual DashboardViewModel Dashboard
        {
            get => dashboard;
            protected set => dashboard = value;
        }

        /// <summary>
        /// View model for the editor
        /// </summary>
        public virtual RecipeEditorViewModel Editor
        {
            get => editor;
            protected set => editor = value;
        }

        /// <summary>
        /// View model for the browser
        /// </summary>
        public virtual RecipeBrowserViewModel Browser
        {
            get => browser;
            protected set => browser = value;
        }

        /// <summary>
        /// View model for the searcher
        /// </summary>
        public virtual RecipeSearcherViewModel Searcher
        {
            get => searcher;
            protected set => searcher = value;
        }

        /// <summary>
        /// View model for the favourites
        /// </summary>
        public virtual RecipeBrowserViewModel FavouritesBrowser
        {
            get => favouritesBrowser;
            protected set => favouritesBrowser = value;
        }


       // ShowDashboardCommand
       /// <summary>
       /// Shows the dashboard (unless the user wants to keep unsaved changes)
       /// </summary>
        public void ShowDashboard() {
            if (CanDiscardUnsavedChanges())
            {
                DiscardUnsavedChanges();
                Dashboard.UpdateCounts();
                CurrentViewTitle = ViewTitle.Dashboard;
            }
            else HandleKeepingUnsavedChanges();
        }
        // Can always show it
        public bool CanShowDashboard() => true;

        // AddRecipeCommand
        /// <summary>
        /// Shows the editor for adding a recipe (unless the user wants to keep unsaved changes)
        /// </summary>
        public void AddRecipe()
        {
            if (CanDiscardUnsavedChanges())
            {
                DiscardUnsavedChanges();
                //Editor.ResetWith(null);
                Editor = RecipeEditorViewModel.Create();
                Editor.SetCloseAction(() =>
                {
                    // Reset view model so another recipe can be added
                    AddRecipe();
                });
                CurrentViewTitle = ViewTitle.Editor;
            }
            else HandleKeepingUnsavedChanges();
        }
        // Can always show it
        public bool CanAddRecipe() => true;

        // BrowseRecipesCommand
        /// <summary>
        /// Shows the browser (unless the user wants to keep unsaved changes)
        /// </summary>
        public void BrowseRecipes()
        {

            if (CanDiscardUnsavedChanges())
            {
                DiscardUnsavedChanges();
                Browser = RecipeBrowserViewModel.Create();
                //Browser.Recipes = new ObservableCollection<Recipe>(Recipe.DatabaseCollection.GetAll());
                CurrentViewTitle = ViewTitle.Browser;
            }
            else HandleKeepingUnsavedChanges();
        }
        // Can always show it
        public bool CanBrowseRecipes() => true;

        // SearchCommand
        /// <summary>
        /// Shows the searcher (unless the user wants to keep unsaved changes)
        /// </summary>
        public void Search()
        {

            if (CanDiscardUnsavedChanges())
            {
                DiscardUnsavedChanges();
                Searcher = RecipeSearcherViewModel.Create();
                CurrentViewTitle = ViewTitle.Searcher;
            }
            else HandleKeepingUnsavedChanges();
        }
        // Can always show it
        public bool CanSearch() => true;

        // ViewFavouritesCommand
        /// <summary>
        /// Shows the favourites (unless the user wants to keep unsaved changes)
        /// </summary>
        public void ViewFavourites()
        {
            if (CanDiscardUnsavedChanges())
            {
                DiscardUnsavedChanges();
                try
                {
                    List<Recipe> favourites = Recipe.DatabaseCollection.GetWhere(r => Favourites.IsFavourite(r.Id)).ToList();
                    FavouritesBrowser = RecipeBrowserViewModel.Create(favourites);
                    CurrentViewTitle = ViewTitle.Favourites;
                }
                catch (Database.RecipeDatabaseException e)
                {
                    MessageBar.SetTemporaryErrorMessage(e.Message, MessageBar.LongDisplayTime);
                }
            }
            else HandleKeepingUnsavedChanges();
        }
        // Can always view favourites
        public bool CanViewFavourites() => true;

        /// <summary>
        /// Shows the about page (unless the user wants to keep unsaved changes)
        /// </summary>
        public void ShowAboutPage()
        {
            if (CanDiscardUnsavedChanges())
            {
                DiscardUnsavedChanges();
                CurrentViewTitle = ViewTitle.About;
            }
            else HandleKeepingUnsavedChanges();
        }
    }
}
