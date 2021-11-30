using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using RecipeMaster.Model;

namespace RecipeMaster.ViewModel
{
    /// <summary>
    /// View model for the recipe editor view
    /// </summary>
    [POCOViewModel(ImplementIDataErrorInfo = true)]
    public class RecipeEditorViewModel
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
        /// Instantiates an instance for adding a new recipe
        /// </summary>
        /// <remarks>Protected to prevent creating the View Model without the ViewModelSource</remarks>
        public static RecipeEditorViewModel Create()
        {
            return ViewModelSource.Create(() => new RecipeEditorViewModel(new Recipe(), true));
        }

        /// <summary>
        /// Instantiates an instance for editing a recipe
        /// </summary>
        /// <remarks>Protected to prevent creating the View Model without the ViewModelSource</remarks>
        /// <param name="recipe">Recipe to edit</param>
        public static RecipeEditorViewModel Create(Recipe recipe)
        {
            return ViewModelSource.Create(() => new RecipeEditorViewModel(recipe, false));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="recipe">Recipe object to edit</param>
        /// <param name="creatingNewRecipe">A new recipe is being created</param>
        protected RecipeEditorViewModel(Recipe recipe, bool creatingNewRecipe)
        {
            CreatingNewRecipe = creatingNewRecipe;
            RecipeModel = recipe;

            // Get all ingredients, categories, measures. Database access could fail, so
            // wrap in try-catch
            try
            {
                AllIngredients = Ingredient.DatabaseCollection.GetAll().Where(i => i != null).ToList();
                Categories = Category.DatabaseCollection.GetAll();
                Measures = Measure.DatabaseCollection.GetAll();
            }
            catch (Database.RecipeDatabaseException e)
            {
                MessageBar.ErrorMessage = e.Message;
            }
        }

        /// <summary>
        /// A new recipe is being created
        /// </summary>
        private bool CreatingNewRecipe { get; set; }

        /// <summary>
        /// Recipe object to edit
        /// </summary>
        private Recipe recipeModel = new Recipe();
        /// <summary>
        /// Recipe object to edit
        /// </summary>
        public virtual Recipe RecipeModel
        {
            get => recipeModel;
            set => recipeModel = value;
        }





        /// <summary>
        /// Action to invoke prior to closing the view
        /// </summary>
        private Action CloseAction { get; set; }
        /// <summary>
        /// Sets the action to invoke prior to closing the view
        /// </summary>
        /// <param name="closeAction">action to invoke prior to closing</param>
        internal void SetCloseAction(Action closeAction)
        {
            CloseAction = closeAction;
        }

        /// <summary>
        /// View model to help give the user success/failure messages
        /// </summary>
        public virtual MessageBarViewModel MessageBar { get; set; } = MessageBarViewModel.Create();

        // Props for category

        /// <summary>
        /// All exisitng categories
        /// </summary>
        public virtual List<Category> Categories { get; set; }
        /// <summary>
        /// Selected category for the recipe
        /// </summary>
        public virtual Category SelectedCategory
        {
            get => recipeModel.Category;
            set => recipeModel.Category = value;
        }
        /// <summary>
        /// If true: User is choosing an existing category. If false: user is entering a new category
        /// </summary>
        public virtual bool ChooseCategoryFromExisting { get; set; } = true;
        /// <summary>
        /// Name for a new category
        /// </summary>
        public virtual string NewCategoryName
        {
            get => RecipeModel.Category?.Name;
            set
            {
                // Empty means no category for this recipe
                if (string.IsNullOrEmpty(value))
                {
                    RecipeModel.Category = null;
                }
                else
                {
                    // Make sure a duplicate category isn't created
                    Category existing = Categories.FirstOrDefault(c => c.Name == value.Trim());
                    RecipeModel.Category = existing ?? new Category(value.Trim());
                }
            }
        }

        // Fields and props for adding ingredient
        /// <summary>
        /// All exisitng ingredients
        /// </summary>
        private List<Ingredient> AllIngredients;
        /// <summary>
        /// List of available ingredients, all those that the recipe does not already have.
        /// </summary>
        public virtual List<Ingredient> AvailableIngredients
        {
            get => AllIngredients
                ?.Where(ing => !RecipeModel.Ingredients.Any(i => i?.Name == ing.Name))
                .OrderBy(ing => ing.Name)
                .ToList();
        }
        /// <summary>
        /// Ingredient object to edit, for when the user wants to add a new ingredient
        /// </summary>
        private Ingredient _newIngredient = new Ingredient();
        /// <summary>
        /// New ingredient to be added
        /// </summary>
        public virtual Ingredient NewIngredient
        {
            get => _newIngredient;
            set => _newIngredient = new Ingredient(value?.Name); // always stored as a new instance so we can make changes without linq-to-sql tracking those changes
        }
        /// <summary>
        /// If true: User is choosing an existing ingredient. If false: user is entering a new ingredient
        /// </summary>
        public virtual bool ChooseIngredientFromExisting { get; set; } = true;
        /// <summary>
        /// Quantity of the new ingredient
        /// </summary>
        public virtual int? NewIngredientQuantity { get; set; } = null;
        /// <summary>
        /// All exisitng measures
        /// </summary>
        public virtual List<Measure> Measures { get; protected set; }
        /// <summary>
        /// Selected measure for the ingredient quantity
        /// </summary>
        public virtual Measure SelectedMeasure { get; set; }

        /// <summary>
        /// RecipeIngredients for the <see cref="RecipeModel"/>
        /// </summary>
        public virtual List<RecipeIngredient> RecipeIngredients
        {
            get => RecipeModel.RecipeIngredients.Where(ri => ri.IsValid()).ToList();
        }

        /// <summary>
        /// Hours component of the preparation time
        /// </summary>
        public virtual int PrepTimeHours
        {
            get => RecipeModel.PrepTime / 60;
            set => RecipeModel.PrepTime = value * 60 + PrepTimeMinutes;
        }
        /// <summary>
        /// Minutes component of the preparation time
        /// </summary>
        public virtual int PrepTimeMinutes
        {
            get => RecipeModel.PrepTime % 60;
            set => RecipeModel.PrepTime = PrepTimeHours * 60 + value;
        }
        /// <summary>
        /// If the recipe has been saved
        /// </summary>
        public virtual bool IsSaved { get; protected set; } = false;

        // ToggleCategorySelectionModeCommand
        /// <summary>
        /// Toggles the selection mode for the category
        /// </summary>
        public void ToggleCategorySelectionMode()
        {
            ChooseCategoryFromExisting = !ChooseCategoryFromExisting;
            SelectedCategory = null;
        }
        // Can always toggle the selection mode
        public bool CanToggleCategorySelectionMode() => true;


        // ToggleIngredientSelectionModeCommand
        /// <summary>
        /// Toggles the selection mode for a new ingredient
        /// </summary>
        public void ToggleIngredientSelectionMode()
        {
            ChooseIngredientFromExisting = !ChooseIngredientFromExisting;
            NewIngredient = null;
        }
        // Can always toggle the selection mode
        public bool CanToggleIngredientSelectionMode() => true;


        // AddIngredientCommand
        /// <summary>
        /// Adds the new ingredient to the recipe
        /// </summary>
        public void AddIngredient()
        {
            // (1) Get the ingredient to be added
            Ingredient ingredient;
            if (ChooseIngredientFromExisting)
            {
                // Use the existing instance so a duplicate entry is not (attempted to be) added to the database
                ingredient = AvailableIngredients.Find(i => i.Name == NewIngredient.Name);
            }
            else
            {
                // Create a complete new instance that can be added to the database (without any further
                // modifications when NewIngredient is changed)
                ingredient = new Ingredient(NewIngredient.Name);
            }
            if (ingredient == null) throw new Exception("Something went wrong: adding a null ingredient should not be possible.");

            // (2) Add ingredient to recipe model; add quantity and meaure to the returned join-class item
            RecipeIngredient recipeIngredient = RecipeModel.Ingredients.Add(ingredient);
            recipeIngredient.Quantity = NewIngredientQuantity;
            recipeIngredient.Measure = SelectedMeasure;

            // (3) Add the ingredient to the list of all ingredients, to
            // prevent a duplicate being added to the recipe later
            AllIngredients.Add(ingredient);

            // (4) Notify view that the lists will have changed. Because these props don't 
            // have setters, the POCOViewModel magic doesn't apply, so this has to be done
            // manually
            this.RaisePropertyChanged(vm => vm.RecipeIngredients);
            this.RaisePropertyChanged(vm => vm.AvailableIngredients);

            // (5) Reset properties related to adding a new ingredient
            NewIngredient = null;
            NewIngredientQuantity = null;
            SelectedMeasure = null;
        }
        /// <summary>
        /// Checks if the new ingredient can be added
        /// </summary>
        /// <returns>If the new ingredient can be added</returns>
        public bool CanAddIngredient()
        {
            // Needs valid quantity
            if (NewIngredientQuantity == null || NewIngredientQuantity <= 0) return false;

            // Needs valid ingredient
            if (NewIngredient == null || !NewIngredient.IsValid()) return false;

            // Needs valid measure
            if (SelectedMeasure == null) return false;

            // Needs to not already be an ingredient for this recipe
            if (RecipeModel.Ingredients
                .Where(ing => ing != null)
                .Any(ing => ing.Name == NewIngredient.Name.Trim())) return false;

            return true;
        }

        // RemoveIngredientCommand
        /// <summary>
        /// Removes an ingredient from the recipe
        /// </summary>
        /// <param name="index">Index of ingrediemt to be removed</param>
        public void RemoveIngredient(int index)
        {
            // Remove ingredient at the specified index
            RecipeModel.Ingredients.Remove(
                RecipeModel.Ingredients.ElementAt(index)
            );

            // Notify view that the list will have changed. Because the prop doesn't 
            // have a setter, the POCOViewModel magic doesn't apply, so this has to be 
            // done manually
            this.RaisePropertyChanged(vm => vm.RecipeIngredients);
        }
        /// <summary>
        /// Checks if an ingredient can be removed
        /// </summary>
        /// <param name="index">Index of ingredient to be removed</param>
        /// <returns>If the ingredient can be removed</returns>
        public bool CanRemoveIngredient(int index)
        {
            return RecipeModel.Ingredients.Count > 0 && index >= 0;
        }

        // CancelCommand
        /// <summary>
        /// Cancels the changes
        /// </summary>
        public void Cancel()
        {
            // Invoke the close action
            CloseAction?.Invoke();
        }
        // Can always cancel
        public bool CanCancel() => true;

        // SaveCommand
        /// <summary>
        /// Saves the recipe
        /// </summary>
        public void Save()
        {
            try
            {
                if (CreatingNewRecipe)
                {
                    Recipe.DatabaseCollection.Add(RecipeModel);
                }
                else
                {
                    Recipe.DatabaseCollection.Update(RecipeModel);
                }
                IsSaved = true;
                MessageBar.SuccessMessage = "Recipe saved!";
                // Wait a second before invoking the close action
                Task.Delay(1000).ContinueWith(_ => CloseAction?.Invoke(), TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Database.RecipeDatabaseException e)
            {
                // If there was an error, show an error message
                MessageBar.SetTemporaryErrorMessage(e.Message, MessageBar.LongDisplayTime);
            }
        }
        /// <summary>
        /// Checks if the recipe can be saved
        /// </summary>
        /// <returns>Recipe can be saved</returns>
        public bool CanSave()
        {
            // Require the model to be in a valid state
            return RecipeModel.IsValid();
        }
    }
}
