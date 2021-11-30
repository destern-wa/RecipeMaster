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
    /// Junction table that correlates recipes with ingredients (a many-to-many association)
    /// </summary>
    [Table(Name = "RecipeIngredients")]
    public class RecipeIngredient : Nullifyable, IDatabaseStorable<RecipeIngredient>, IDatabaseRecord
    {
        /// <summary>
        /// Empty constructor
        /// </summary>
        public RecipeIngredient() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="recipe">Recipe</param>
        /// <param name="ingredient">Ingredient</param>
        public RecipeIngredient(Recipe recipe, Ingredient ingredient)
        {
            Recipe = recipe;
            Ingredient = ingredient;
        }

        /* One-to-many association with recipes */
#pragma warning disable 0169 // Suppress "never used" warning: is used by LINQ-to-SQL
        /// <summary>
        /// ID of associated recipe 
        /// </summary>
        [Column(IsPrimaryKey = true)] private int recipeId;
#pragma warning restore 0169
        /// <summary>
        /// Storage for assocated recipe
        /// </summary>
        private EntityRef<Recipe> _recipe = new EntityRef<Recipe>();
        /// <summary>
        /// Associated recipe
        /// </summary>
        [Association(Name = "FK_Recipes_RecipeIngredients", IsForeignKey = true, Storage = "_recipe", ThisKey = "recipeId", DeleteOnNull = true)]
        public Recipe Recipe
        {
            get => _recipe.Entity;
            set => _recipe.Entity = value;
        }

        /* One-to-many association with ingredients */
#pragma warning disable 0169 // Suppress "never used" warning: is used by LINQ-to-SQL
        /// <summary>
        /// ID of associated ingredient
        /// </summary>
        [Column(IsPrimaryKey = true)] private int ingredientId;
#pragma warning restore 0169
        /// <summary>
        /// Storage for associated ingredient
        /// </summary>
        private EntityRef<Ingredient> _ingredient = new EntityRef<Ingredient>();
        /// <summary>
        /// Associate ingredient
        /// </summary>
        [Association(Name = "FK_Ingredients_RecipeIngredients", IsForeignKey = true, Storage = "_ingredient", ThisKey = "ingredientId", DeleteOnNull = true)]
        public Ingredient Ingredient
        {
            get => _ingredient.Entity;
            set => _ingredient.Entity = value;
        }
        /// <summary>
        /// Quantity of the ingredient for the recipe (in units of <see cref="Measure"/>)
        /// </summary>
        [Column(CanBeNull = false)] public double? Quantity { get; set; }

        /* One-to-many association with measures */
#pragma warning disable 0169 // Suppress "never used" warning: is used by LINQ-to-SQL
        /// <summary>
        /// ID of associated measure
        /// </summary>
        [Column(IsPrimaryKey = true)] private int measureId;
#pragma warning restore 0169
        /// <summary>
        /// Storage for associated measure
        /// </summary>
        private EntityRef<Measure> _measure = new EntityRef<Measure>();
        /// <summary>
        /// Associate measure
        /// </summary>
        [Association(Name = "FK_Measures_RecipeIngredients", IsForeignKey = true, Storage = "_measure", ThisKey = "measureId", DeleteOnNull = true)]
        public Measure Measure
        {
            get => _measure.Entity;
            set => RecipricallySetMeasure(value);
        }
        /// <summary>
        /// Sets the measure to a new value, and reciprically updates the list of
        /// recipeIngredients for the old measure as well as the new measure.
        /// 
        /// Those reciprical updates aren't strictly neccesary to update the
        /// database, but are needed to ensure the LINQ-to-SQL objects and
        /// associations stay in-sync with the database. Which is weird and
        /// annoying. Hence this work-around is used rather than simply having
        /// `set => _measure.Entity = value` as the setter.
        /// </summary>
        /// <param name="newMeasure"></param>
        private void RecipricallySetMeasure(Measure newMeasure)
        {
            // If measure hasn't changed, do nothing
            if (newMeasure == _measure.Entity) return;
            // Remove this from the old measure's list of recipeIngredients
            _measure.Entity?.RecipeIngredients.Remove(this);
            // Set the entity ref's entity to the new measure
            _measure.Entity = newMeasure;
            // Add this to the new measure's list of recipeIngredients, if not already present
            if (newMeasure != null && !newMeasure.RecipeIngredients.Contains(this))
            {
                newMeasure.RecipeIngredients.Add(this);
            }
        }

        /// <summary>
        /// Marks this instance as awaiting deletion by setting all relavent properties to null (or similar)
        /// </summary>
        public override void Nullify()
        {
            Recipe?.RecipeIngredients.Remove(this);
            Ingredient?.RecipeIngredients.Remove(this);
            Measure?.RecipeIngredients.Remove(this);
            Recipe = null;
            Ingredient = null;
            Quantity = 0;
        }

        /// <summary>
        /// Checks if this instance is valid (has assocated recipe, ingredient, measure, and quantity is specified)
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid() => Recipe != null && Ingredient != null && Quantity != null && Measure != null;

        /// <inheritdoc/>
        public override string ToString()
        {
            if (!IsValid()) return "(to be deleted)";
            return $"{Quantity} {Measure}  x  {Ingredient.Name}";
        }

        /// <summary>
        /// Static instance for querying the database
        /// </summary>
        public static IDatabaseStorable<RecipeIngredient> DatabaseCollection { get; private set; } = new RecipeIngredient();

        /// <summary>
        /// NOT IMPLEMENTED. DO NOT USE.
        /// </summary>
        /// <remarks>RecipeIngredients is a junction table, so does not have an ID column.</remarks>
        public int Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        List<RecipeIngredient> IDatabaseStorable<RecipeIngredient>.GetAll()
        {
            try
            {
                return App.Database.GetAll(typeof(RecipeIngredient)).Cast<RecipeIngredient>().ToList();
            }
            catch (Exception e) { throw new RecipeDatabaseException("Database Error: Could not get recipe ingredients", e); }
        }

        /// <inheritdoc/>
        List<RecipeIngredient> IDatabaseStorable<RecipeIngredient>.GetWhere(Func<RecipeIngredient, bool> predicate)
        {
            try
            {
                return App.Database.GetWhere(typeof(RecipeIngredient), o => predicate(o as RecipeIngredient)).Cast<RecipeIngredient>().ToList();
            }
            catch (Exception e) { throw new RecipeDatabaseException("Database Error: Could not get recipe ingredients", e); }
        }

        /// <summary>
        /// NOT IMPLEMENTED. DO NOT USE.
        /// </summary>
        /// <remarks>RecipeIngredients is a junction table, so does not have an ID column,
        /// and hence an instance can not be reteived by ID</remarks>
        RecipeIngredient IDatabaseStorable<RecipeIngredient>.Get(int id)
        {
            throw new RecipeDatabaseException("RecipeIngredients can not be fetched by id", new NotImplementedException());
        }

        /// <inheritdoc/>
        void IDatabaseStorable<RecipeIngredient>.Update(RecipeIngredient item)
        {
            try
            {
                App.Database.Edit(typeof(RecipeIngredient), item);
            }
            catch (Exception e) { throw new RecipeDatabaseException("Database Error: Could not update recipe ingredient", e); }
        }

        /// <inheritdoc/>
        void IDatabaseStorable<RecipeIngredient>.Add(RecipeIngredient item)
        {
            try
            {
                App.Database.Add(typeof(RecipeIngredient), item);
            }
            catch (Exception e) { throw new RecipeDatabaseException("Database Error: Could not add recipe ingredient", e); }
        }

        /// <inheritdoc/>
        void IDatabaseStorable<RecipeIngredient>.Delete(int id)
        {
            try
            {
                App.Database.Delete(typeof(RecipeIngredient), id);
            }
            catch (Exception e) { throw new RecipeDatabaseException("Database Error: Could not delete recipe ingredient", e); }
        }

        /// <inheritdoc/>
        int IDatabaseStorable<RecipeIngredient>.Count()
        {
            try
            {
                return App.Database.Count(typeof(RecipeIngredient));
            }
            catch (Exception e) { throw new RecipeDatabaseException("Database Error: Could not get count of recipe ingredients", e); }
        }
    }
}
