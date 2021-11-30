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
    /// A unit of measurement
    /// </summary>
    [Table(Name = "Measures")]
    public class Measure : IDatabaseStorable<Measure>, IDatabaseRecord
    {
        /// <summary>
        /// Empty constructor
        /// </summary>
        public Measure() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Measurement unit name</param>
        public Measure(string name)
        {
            Name = name;
        }

        /// <inheritdoc/>
        [Column(IsPrimaryKey = true, IsDbGenerated = true)] public int Id { get; set; }

        /// <summary>
        /// Measurement unit name
        /// </summary>
        [Column] public string Name { get; set; }

        /* Many-to-one association with RecipeIngredients */
        /// <summary>
        /// Storage for association with RecipeIngredients
        /// </summary>
        private EntitySet<RecipeIngredient> _recipeIngredients = new EntitySet<RecipeIngredient>();
        /// <summary>
        /// RecipeIngredients assocaited with this measure
        /// </summary>
        /// <remarks>For internal use only</remarks>
        [Association(Name = "FK_Ingredients_Measures", Storage = "_recipeIngredients", OtherKey = "measureId", ThisKey = "Id")]
        private ICollection<RecipeIngredient> recipeIngredients
        {
            get { return _recipeIngredients; }
            set { _recipeIngredients.Assign(value); }
                
        }
        /// <summary>
        /// RecipeIngredients assocaited with this measure
        /// </summary>
        public JoinCollection<RecipeIngredient, Measure> RecipeIngredients
        {
            get => new JoinCollection<RecipeIngredient, Measure>(this,
                recipeIngredients,
                (recipeIngredient, measure) => recipeIngredient.Measure = measure,
                recipeIngredient => recipeIngredient.Measure = null 
            );
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Static instance for querying the database
        /// </summary>
        public static IDatabaseStorable<Measure> DatabaseCollection { get; private set; } = new Measure();

        /// <inheritdoc/>
        List<Measure> IDatabaseStorable<Measure>.GetAll()
        {
            try
            {
                return App.Database.GetAll(typeof(Measure)).Cast<Measure>().ToList();
            }
            catch (Exception e) { throw new RecipeDatabaseException("Database Error: Could not get measures", e); }
        }

        /// <inheritdoc/>
        List<Measure> IDatabaseStorable<Measure>.GetWhere(Func<Measure, bool> predicate)
        {
            try
            {
                return App.Database.GetWhere(typeof(Measure), o => predicate(o as Measure)).Cast<Measure>().ToList();
            }
            catch (Exception e) { throw new RecipeDatabaseException("Database Error: Could not get measures", e); }
        }

        /// <inheritdoc/>
        Measure IDatabaseStorable<Measure>.Get(int id)
        {
            try
            {
                return (Measure)App.Database.Get(typeof(Measure), id);
            }
            catch (Exception e) { throw new RecipeDatabaseException("Database Error: Could not get measure", e); }
        }

        /// <inheritdoc/>
        void IDatabaseStorable<Measure>.Update(Measure item)
        {
            try
            {
                App.Database.Edit(typeof(Measure), item);
            }
            catch (Exception e) { throw new RecipeDatabaseException("Database Error: Could not update measure", e); }
        }

        /// <inheritdoc/>
        void IDatabaseStorable<Measure>.Add(Measure item)
        {
            try
            {
                App.Database.Add(typeof(Measure), item);
            }
            catch (Exception e) { throw new RecipeDatabaseException("Database Error: Could not add measure", e); }
        }

        /// <inheritdoc/>
        void IDatabaseStorable<Measure>.Delete(int id)
        {
            try
            {
                App.Database.Delete(typeof(Measure), id);
            }
            catch (Exception e) { throw new RecipeDatabaseException("Database Error: Could not delete measure", e); }
        }

        /// <inheritdoc/>
        int IDatabaseStorable<Measure>.Count()
        {
            try
            {
                return App.Database.Count(typeof(Measure));
            }
            catch (Exception e) { throw new RecipeDatabaseException("Database Error: Could not get count of measures", e); }
        }
    }
}
