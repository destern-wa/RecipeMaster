using RecipeMaster.Database;
using RecipeMaster.Interface;
using RecipeMaster.Util;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;

namespace RecipeMaster.Model
{
    /// <summary>
    /// Class that represents a recipe
    /// </summary>
    [Table(Name = "Recipes")]
    public class Recipe : IDatabaseStorable<Recipe>, IDatabaseRecord
    {
        /// <summary>
        /// Empty constructor
        /// </summary>
        public Recipe() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the recipe</param>
        /// <param name="prepTime">Preparation time in minutes</param>
        /// <param name="method">Method for making the recipe</param>
        /// <param name="numberOfServes">Number of serves made by following the method. Optional.</param>
        /// <param name="energyPerServe">Energy per serve. Optional.</param>
        /// <param name="category">Category the recipe is classified in. Optional</param>
        /// <param name="recipeIngredients">Ingredients (with measurement amounts) for this recipe</param>
        public Recipe(string name, int prepTime, string method, int? numberOfServes, string energyPerServe, Category category, List<RecipeIngredient> recipeIngredients)
        {
            Name = name;
            PrepTime = prepTime;
            Method = method;
            NumberOfServes = numberOfServes;
            EnergyPerServe = energyPerServe;
            Category = category;
            foreach (RecipeIngredient ri in recipeIngredients)
            {
                ri.Recipe = this;
            }
            RecipeIngredients = recipeIngredients;
        }

        /// <inheritdoc/>
        [Column(IsPrimaryKey = true, IsDbGenerated = true)] public int Id { get; set; }

        /// <summary>
        /// Name of the recipe
        /// </summary>
        [Column] public string Name { get; set; }

        /// <summary>
        /// Preparation time in minutes
        /// </summary>
        [Column] public int PrepTime { get; set; }

        /// <summary>
        /// Method for making the recipe
        /// </summary>
        [Column] public string Method { get; set; }

        /// <summary>
        /// Number of serves made by following the method
        /// </summary>
        [Column(CanBeNull = true)] public int? NumberOfServes { get; set; }

        /// <summary>
        /// Energy per serve (as a number plus a unit such as calories or kilojoules)
        /// </summary>
        [Column(CanBeNull = true)] public string EnergyPerServe { get; set; }

        /* One-to-many association with categories */
#pragma warning disable 0169 // Suppress "never used" warning: is used by LINQ-to-SQL 
        /// <summary>
        /// ID of category this recipe is associated with
        /// </summary>
        [Column(CanBeNull = true)] private int? categoryId;
#pragma warning restore 0169
        /// <summary>
        /// Storage for association with Categories
        /// </summary>
        private EntityRef<Category> _category = new EntityRef<Category>();
        /// <summary>
        /// Category this recipe is associated with 
        /// </summary>
        [Association(Name = "FK_Categories_Recipes", Storage = "_category", IsForeignKey = true, OtherKey = "Id", ThisKey = "categoryId")]
        public Category Category
        {
            get => _category.Entity;
            set => RecipricallySetCategory(value);
        }
        /// <summary>
        /// Sets the category to a new value, and reciprical updates the list of
        /// recipes for the old category as well as the new category.
        /// 
        /// Those reciprical updates aren't strictly neccesary to update the
        /// database, but are needed to ensure the LINQ-to-SQL objects and
        /// associations stay in-sync with the database. Which is weird and
        /// annoying. Hence this work-around is used rather than simply having
        /// `set => _category.Entity = value` as the setter.
        /// </summary>
        /// <param name="category"></param>
        private void RecipricallySetCategory(Category newCategory)
        {
            // If category hasn't changed, do nothing
            if (newCategory == _category.Entity) return;

            // Remove this from the old category's list of recipes
            _category.Entity?.Recipes.Remove(this);

            // Set the entity ref's entity to the new category
            _category.Entity = newCategory;

            // Add this to the new category's list of recipes, if not already present
            if (newCategory != null && !newCategory.Recipes.Contains(this))
            {
                newCategory.Recipes.Add(this);
            }
        }

        /* Many-to-many association with ingredients (via RecipeIngredients join table) */
        /// <summary>
        /// Storage for association with RecipeIngredients
        /// </summary>
        private EntitySet<RecipeIngredient> _recipeIngredients = new EntitySet<RecipeIngredient>();
        /// <summary>
        /// RecipeIngredients this recipe is associated with
        /// </summary>
        /// <remarks>For internal use only</remarks>
        [Association(Name = "FK_RecipeIngredients_Recipes", Storage = "_recipeIngredients", OtherKey = "recipeId", ThisKey = "Id", DeleteRule = "SET NULL")]
        internal ICollection<RecipeIngredient> RecipeIngredients
        {
            get => _recipeIngredients;
            private set => _recipeIngredients.Assign(value);
        }
        /// <summary>
        /// Ingredients this recipe is assocaited with
        /// </summary>
        public JoinCollection<Ingredient, Recipe, RecipeIngredient> Ingredients
        {
            get => new JoinCollection<Ingredient, Recipe, RecipeIngredient>(this,
                RecipeIngredients,
                recipeingredient => recipeingredient.Ingredient,
                (ingredient, recipe) => new RecipeIngredient(recipe, ingredient)
            );
        }

        /// <summary>
        /// Whether this recipe is a favourite recipe
        /// </summary>
        public bool IsFavourite
        {
            get => Favourites.IsFavourite(this.Id);
            protected set => Favourites.AddFavourite(this.Id);
        }

        /// <summary>
        /// Checks if this instance is currently a valid recipe 
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid()
        {
            // Name is requireed
            if (string.IsNullOrWhiteSpace(Name)) return false;

            // No time travel allowed
            if (PrepTime < 0) return false;
           
            // Method is required
            if (string.IsNullOrWhiteSpace(Method)) return false;

            // Must have at least 1 ingredient
            if (RecipeIngredients.Count == 0) return false;

            // Each ingredient for the recipe must be valid
            foreach (RecipeIngredient ri in RecipeIngredients)
            {
                if (!ri.IsValid()) return false;
            }

            // Otherwise it is valid
            return true;
        }

        /// <summary>
        /// Static instance for querying the database
        /// </summary>
        public static IDatabaseStorable<Recipe> DatabaseCollection { get; private set; } = new Recipe();

        /// <inheritdoc/>
        List<Recipe> IDatabaseStorable<Recipe>.GetAll()
        {
            try
            {
                return App.Database.GetAll(typeof(Recipe)).Cast<Recipe>().ToList();
            }
            catch (Exception e) { throw new RecipeDatabaseException("Database Error: Could not get recipes", e); }

        }

        /// <inheritdoc/>
        List<Recipe> IDatabaseStorable<Recipe>.GetWhere(Func<Recipe, bool> predicate)
        {
            try
            {
                return App.Database.GetWhere(typeof(Recipe), o => predicate(o as Recipe)).Cast<Recipe>().ToList();
            }
            catch (Exception e) { throw new RecipeDatabaseException("Database Error: Could not get recipes", e); }
        }

        /// <inheritdoc/>
        Recipe IDatabaseStorable<Recipe>.Get(int id)
        {
            try
            {
                return (Recipe)App.Database.Get(typeof(Recipe), id);
            }
            catch (Exception e)
            {
                throw new RecipeDatabaseException("Database Error: Could not get recipe", e);
            }
        }

        /// <inheritdoc/>
        void IDatabaseStorable<Recipe>.Update(Recipe item)
        {
            try
            {
                App.Database.Edit(typeof(Recipe), item);
            }
            catch (Exception e) { throw new RecipeDatabaseException("Database Error: Could not update recipe", e); }
        }

        /// <inheritdoc/>
        void IDatabaseStorable<Recipe>.Add(Recipe item)
        {
            try
            {
                App.Database.Add(typeof(Recipe), item);
            }
            catch (Exception e) { throw new RecipeDatabaseException("Database Error: Could not add recipe", e); }
        }

        /// <inheritdoc/>
        void IDatabaseStorable<Recipe>.Delete(int id)
        {
            try
            {
                App.Database.Delete(typeof(Recipe), id);
            }
            catch (Exception e) { throw new RecipeDatabaseException("Database Error: Could not delete recipe", e); }
        }

        /// <inheritdoc/>
        int IDatabaseStorable<Recipe>.Count()
        {
            try
            {
                return App.Database.Count(typeof(Recipe));
            }
            catch (Exception e) { throw new RecipeDatabaseException("Database Error: Could not get count of recipes", e); }
        }
    }
}
