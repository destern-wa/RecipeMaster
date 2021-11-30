using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using RecipeMaster.Model;
using RecipeMaster.Interface;

namespace RecipeMaster.Database
{
    /// <summary>
    /// The data context for a MSSQL database of recipes
    /// </summary>
    [Database]
    public class MsSqlDatabase : DataContext, IRecipeDatabase
    {
        /// <summary>
        /// Private field for singleton instance
        /// </summary>
        private static MsSqlDatabase instance;

        /// <summary>
        /// Static constructor
        /// </summary>
        static MsSqlDatabase() { }

        /// <summary>
        /// Database instance
        /// </summary>
        public static MsSqlDatabase Instance
        {
            get
            {
                if (instance == null) instance = new MsSqlDatabase();
                return instance;
            }
        }

        /// <summary>
        /// Disconnects from the database
        /// </summary>
        public void Disconnect()
        {
            this.Connection.Close();
            instance = null;
        }

        /// <summary>
        /// Constructs the database data context using the app's static connection string
        /// </summary>
        protected MsSqlDatabase() : base(App.msSqlConnStr)
        {
            /* Uncomment to refresh the database structure and seed with sample data */
            // InitialiseDatabase();

            /* Uncomment to log all queries to the console */
            // this.Log = System.Console.Out;
        }

        /// <summary>
        /// Constructs the database data context using a custom connection string
        /// </summary>
        /// <param name="connection"></param>
        public MsSqlDatabase(string connection) : base(connection) {
            /* Uncomment to refresh the database structure and seed with sample data */
            // InitialiseDatabase();

            /* Uncomment to log all queries to the console */
            // this.Log = System.Console.Out;
        }

        /// <summary>
        /// Refreshes the database structure and seeds it with sample data
        /// </summary>
        private void InitialiseDatabase()
        {
            if (DatabaseExists())
            {
                // Close any lingering / left-over connections that prevent deletion of the database.
                // Based on: https://stackoverflow.com/questions/11620/how-do-you-kill-all-current-connections-to-a-sql-server-2005-database#answer-11536813
                ExecuteQuery<object>(
                    "use master " +
                    "ALTER DATABASE Recipes SET OFFLINE WITH ROLLBACK IMMEDIATE " +
                    "ALTER DATABASE Recipes SET ONLINE",
                    new string[] { }
                );
            }
            DeleteDatabase();
            CreateDatabase();
            Seed.SeedDatabase(this);
            // Also initialise the list of favourites
            Favourites.Initialise();
        }

        /// <summary>
        /// Recipes table
        /// </summary>
        public Table<Recipe> Recipes;

        /// <summary>
        /// Ingredients table
        /// </summary>
        public Table<Ingredient> Ingredients;

        /// <summary>
        /// RecipeIngredients join table
        /// </summary>
        public Table<RecipeIngredient> RecipeIngredients;

        /// <summary>
        /// Categories table
        /// </summary>
        public Table<Category> Categories;

        /// <summary>
        /// Measures table
        /// </summary>
        public Table<Measure> Measures;

        /// <summary>
        /// Checks if there are any unsumbitted changes.
        /// 
        /// Based on: https://social.msdn.microsoft.com/Forums/en-US/dc06b365-eac3-4115-9e95-a24b9f5ec083/detect-whether-datacontext-has-changes
        /// </summary>
        public bool HasChanges()
        {
            ChangeSet changeSet = GetChangeSet();
            return (changeSet.Deletes.Count != 0 || changeSet.Inserts.Count != 0 || changeSet.Updates.Count != 0);
        }


        /// <summary>
        /// Discards changes (inserts, deletes, and updates) that have not yet been submited.
        /// 
        /// Based on http://graemehill.ca/discard-changes-in-linq-to-sql-datacontext/
        /// </summary>
        public void DiscardChanges()
        {
            // Get the changes
            ChangeSet changes = this.GetChangeSet();

            // Delete any insertions
            foreach (var insertion in changes.Inserts)
            {
                this.GetTable(insertion.GetType()).DeleteOnSubmit(insertion);
            }

            // Insert any deletions
            foreach (var deletion in changes.Deletes)
            {
                this.GetTable(deletion.GetType()).InsertOnSubmit(deletion);
            }

            // Refresh any tables with updates
            List<Type> refreshed = new List<Type>();
            foreach (var updated in changes.Updates)
            {
                Type tableType = updated.GetType();
                // Make sure not to do the same table twice
                if (refreshed.Contains(tableType)) continue;

                this.Refresh(RefreshMode.OverwriteCurrentValues, this.GetTable(tableType));
                refreshed.Add(tableType);
            }
        }


        /// <inheritdoc/>
        public void Add(Type T, object item)
        {
            GetTable(T).InsertOnSubmit(item);
            SubmitChanges();
        }

        /// <inheritdoc/>
        public void AddAll(Type T, IEnumerable<object> items)
        {
            GetTable(T).InsertAllOnSubmit(items);
            SubmitChanges();
        }

        /// <inheritdoc/>
        public void Edit(Type T, object item)
        {
            // Linq-to-sql mapping tracks changes automatically, so all we need to do is submit them
            SubmitChanges();
        }
        
        /// <inheritdoc/>
        public void Delete(Type T, int id)
        {
            // Check that the item exists in the database
            object itemToDelete = Get(T, id);
            if (itemToDelete == null) throw new Exception($"{T} with id {id} not found in database");

            // First delete join table entries, if applicable
            if (T == typeof(Ingredient))
            {
                RecipeIngredients.DeleteAllOnSubmit((itemToDelete as Ingredient).RecipeIngredients);
            }
            else if (T == typeof(Recipe))
            {
                RecipeIngredients.DeleteAllOnSubmit((itemToDelete as Recipe).RecipeIngredients);
            }

            // Then delete the item from its own table 
            GetTable(T).DeleteOnSubmit(itemToDelete);
            SubmitChanges();
        }


        /// <inheritdoc/>
        public IDatabaseRecord Get(Type T, int id)
        {
            return GetTable(T).Cast<IDatabaseRecord>().FirstOrDefault(t => t.Id == id);
        }

        /// <inheritdoc/>
        public List<IDatabaseRecord> GetAll(Type T)
        {
            return GetTable(T).Cast<IDatabaseRecord>().ToList();
        }

        /// <inheritdoc/>
        public List<IDatabaseRecord> GetWhere(Type T, Func<IDatabaseRecord, bool> predicate)
        {
            return GetTable(T).Cast<IDatabaseRecord>().Where(predicate).ToList();
        }

        /// <inheritdoc/>
        public int Count(Type T)
        {
            return GetTable(T).Cast<IDatabaseRecord>().Count();
        }
    }


}
