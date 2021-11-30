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
    /// A category that a recipe can be classified as.
    /// </summary>
    [Table(Name = "Categories")]
    public class Category : IDatabaseStorable<Category>, IDatabaseRecord
    {
        /// <summary>
        /// Empty constructor
        /// </summary>
        public Category() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the category</param>
        public Category(string name)
        {
            Name = name;
        }

        /// <inheritdoc/>
        [Column(IsPrimaryKey = true, IsDbGenerated = true)] public int Id { get; set; }

        /// <summary>
        /// Name of the category
        /// </summary>
        [Column] public string Name { get; set; }

        /* Many-to-one association with Recipes */
        /// <summary>
        /// Storage for the many-to-one association with Recipes
        /// </summary>
        private EntitySet<Recipe> _recipes = new EntitySet<Recipe>();
        /// <summary>
        /// Recipes assocaited with this category
        /// </summary>
        /// <remarks>For internal use only</remarks>
        [Association(Name = "FK_Recipes_Categories", Storage = "_recipes", OtherKey = "categoryId", ThisKey = "Id")]
        private ICollection<Recipe> recipes
        {
            get { return _recipes; }
            set { _recipes.Assign(value); }
        }
        /// <summary>
        /// Recipes assocaited with this category
        /// </summary>
        public JoinCollection<Recipe, Category> Recipes
        {
            get => new JoinCollection<Recipe, Category>(this,
                recipes,
                (recipe, cat) => recipe.Category = cat,
                recipe => recipe.Category = null
            );
        }

        /// <inheritdoc/>
        public override string ToString() => this.Name;


        /// <summary>
        /// Static instance for querying the database.
        /// </summary>
        public static IDatabaseStorable<Category> DatabaseCollection { get; private set; } = new Category();

        /// <inheritdoc/>
        List<Category> IDatabaseStorable<Category>.GetAll()
        {
            try
            {
                return App.Database.GetAll(typeof(Category)).Cast<Category>().ToList();
            }
            catch (Exception e)
            {
                throw new RecipeDatabaseException("Database Error: Could not get categories", e);
            }
        }

        /// <inheritdoc/>
        List<Category> IDatabaseStorable<Category>.GetWhere(Func<Category, bool> predicate)
        {
            try
            {
                return App.Database.GetWhere(typeof(Category), o => predicate(o as Category)).Cast<Category>().ToList();
            }
            catch (Exception e)
            {
                throw new RecipeDatabaseException("Database Error: Could not get categories", e);
            }
        }

        /// <inheritdoc/>
        Category IDatabaseStorable<Category>.Get(int id)
        {
            try
            {
                return (Category)App.Database.Get(typeof(Category), id);
            }
            catch (Exception e)
            {
                throw new RecipeDatabaseException("Database Error: Could not get category", e);
            }
        }

        /// <inheritdoc/>
        void IDatabaseStorable<Category>.Update(Category item)
        {
            try
            {
                App.Database.Edit(typeof(Category), item);
            }
            catch (Exception e)
            {
                throw new RecipeDatabaseException("Database Error: Could not update category", e);
            }
        }

        /// <inheritdoc/>
        void IDatabaseStorable<Category>.Add(Category item)
        {
            try
            {
                App.Database.Add(typeof(Category), item);
            }
            catch (Exception e)
            {
                throw new RecipeDatabaseException("Database Error: Could not add category", e);
            }
        }

        /// <inheritdoc/>
        void IDatabaseStorable<Category>.Delete(int id)
        {
            try
            {
                App.Database.Delete(typeof(Category), id);
            }
            catch (Exception e)
            {
                throw new RecipeDatabaseException("Database Error: Could not delete category", e);
            }
        }

        /// <inheritdoc/>
        int IDatabaseStorable<Category>.Count()
        {
            try
            {
                return App.Database.Count(typeof(Category));
            }
            catch (Exception e)
            {
                throw new RecipeDatabaseException("Database Error: Could not get categories count", e);
            }
        }
    }
}
