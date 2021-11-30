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
    /// An ingredient that can be used in a recipe
    /// </summary>
    [Table(Name = "Ingredients")]
    public class Ingredient : IDatabaseStorable<Ingredient>, IDatabaseRecord
    {
        /// <summary>
        /// Empty constructor
        /// </summary>
        public Ingredient() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the ingredient</param>
        public Ingredient(string name)
        {
            Name = name;
        }

        /// <inheritdoc/>
        [Column(IsPrimaryKey = true, IsDbGenerated = true)] public int Id { get; set; }

        /// <summary>
        /// Name of the ingredient
        /// </summary>
        [Column] public string Name { get; set; }


        /* Many-to-many association with recipes (via RecipeIngredients join table) */
#pragma warning disable 0169 // Suppress "never used" warning: is used by LINQ-to-SQL 
        /// <summary>
        /// Storage for association with RecipeIngredients
        /// </summary>
        private EntitySet<RecipeIngredient> _recipeIngredients = new EntitySet<RecipeIngredient>();
#pragma warning restore 0169
        /// <summary>
        /// RecipeIngredients assocaited with this ingredient
        /// </summary>
        /// <remarks>For internal use only</remarks>
        [Association(Name = "FK_RecipeIngredients_Ingredients", Storage = "_recipeIngredients", OtherKey = "ingredientId", ThisKey = "Id")]
        internal ICollection<RecipeIngredient> RecipeIngredients
        {
            get => _recipeIngredients;
            private set => _recipeIngredients.Assign(value);
        }
        /// <summary>
        /// Recipes associated with this ingredient
        /// </summary>
        public JoinCollection<Recipe, Ingredient, RecipeIngredient> Recipes
        {
            get => new JoinCollection<Recipe, Ingredient, RecipeIngredient>(this,
                RecipeIngredients,
                recipeingredient => recipeingredient.Recipe,
                (recipe, Ingredient) => new RecipeIngredient(recipe, Ingredient)
            );
        }

        /// <summary>
        /// Checks if this instance is valid
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid() => !string.IsNullOrWhiteSpace(Name);

        /// <inheritdoc/>
        public override string ToString() => Name;

        /// <summary>
        /// Static instance for querying the database
        /// </summary>
        public static IDatabaseStorable<Ingredient> DatabaseCollection { get; private set; } = new Ingredient();

        /// <inheritdoc/>
        List<Ingredient> IDatabaseStorable<Ingredient>.GetAll()
        {
            try
            {
                return App.Database.GetAll(typeof(Ingredient)).Cast<Ingredient>().ToList();

            }
            catch (Exception e)
            {
                throw new RecipeDatabaseException("Database Error: Could not get ingredients", e);
            }
        }

        /// <inheritdoc/>
        List<Ingredient> IDatabaseStorable<Ingredient>.GetWhere(Func<Ingredient, bool> predicate)
        {
            try
            {
                return App.Database.GetWhere(typeof(Ingredient), o => predicate(o as Ingredient)).Cast<Ingredient>().ToList();
            }
            catch (Exception e) { throw new RecipeDatabaseException("Database Error: Could not get ingredients", e); }
        }

        /// <inheritdoc/>
        Ingredient IDatabaseStorable<Ingredient>.Get(int id)
        {
            try {
                return (Ingredient)App.Database.Get(typeof(Ingredient), id);
            }
            catch (Exception e) { throw new RecipeDatabaseException("Database Error: Could not get ingredient", e); }

        }

        /// <inheritdoc/>
        void IDatabaseStorable<Ingredient>.Update(Ingredient item)
        {
            try
            {
                App.Database.Edit(typeof(Ingredient), item);
            }
            catch (Exception e) { throw new RecipeDatabaseException("Database Error: Could not update ingredient", e); }
        }

        /// <inheritdoc/>
        void IDatabaseStorable<Ingredient>.Add(Ingredient item)
        {
            try
            {
                App.Database.Add(typeof(Ingredient), item);
            }
            catch (Exception e) { throw new RecipeDatabaseException("Database Error: Could not add ingredient", e); }
        }

        /// <inheritdoc/>
        void IDatabaseStorable<Ingredient>.Delete(int id)
        {
            try
            {
                App.Database.Delete(typeof(Ingredient), id);
            }
            catch (Exception e) { throw new RecipeDatabaseException("Database Error: Could not delete ingredient", e); }
        }

        /// <inheritdoc/>
        int IDatabaseStorable<Ingredient>.Count()
        {
            try
            {
                return App.Database.Count(typeof(Ingredient));
            }
            catch (Exception e) { throw new RecipeDatabaseException("Database Error: Could not get count of ingredients", e); }
        }
    }
}
