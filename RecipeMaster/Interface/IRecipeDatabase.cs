using RecipeMaster.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMaster.Interface
{
    /// <summary>
    /// Recipes a database context that can be used to store recipe data
    /// </summary>
    public interface IRecipeDatabase
    {
        /// <summary>
        /// Disconnects from the database
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Checks if there are pending changes yet to be submitted
        /// </summary>
        bool HasChanges();

        /// <summary>
        ///  Discards changes (inserts, deletes, and updates) that have not yet been submited.
        /// </summary>
        void DiscardChanges();

        /// <summary>
        /// Add a record to a table in the database
        /// </summary>
        /// <param name="T">Type of record</param>
        /// <param name="item">Record to be added</param>
        void Add(Type T, object item);

        /// <summary>
        /// Add all records in a collection to a table in the database
        /// </summary>
        /// <param name="T">Type of the records</param>
        /// <param name="items">Records to be added</param>
        void AddAll(Type T, IEnumerable<object> items);
        
        /// <summary>
        /// Edits an record that exists in the database
        /// </summary>
        /// <param name="T">Type of the record</param>
        /// <param name="item">Updated version of a record that exists in the database (same ID number)</param>
        void Edit(Type T, object item);

        /// <summary>
        /// Delete a record from a table in the database
        /// </summary>
        /// <param name="T">Type of the record</param>
        /// <param name="id">ID number of record to delete</param>
        void Delete(Type T, int id);

        /// <summary>
        /// Gets a record from a table in the database
        /// </summary>
        /// <param name="T">Type of the record</param>
        /// <param name="id">ID number of the record</param>
        /// <returns>Matching record</returns>
        IDatabaseRecord Get(Type T, int id);

        /// <summary>
        /// Gets all records from a table in the database
        /// </summary>
        /// <param name="T">Type of the records</param>
        /// <returns>Matching records</returns>
        List<IDatabaseRecord> GetAll(Type T);

        /// <summary>
        /// Gets records from a table in the database, filtering results based on a predicate
        /// </summary>
        /// <param name="T">Type of the records</param>
        /// <param name="predicate">Predicate function</param>
        /// <returns>Matching records</returns>
        List<IDatabaseRecord> GetWhere(Type T, Func<IDatabaseRecord, bool> predicate);
       
        /// <summary>
        /// Gets the number of records that a table in the database contains
        /// </summary>
        /// <param name="T">Type of the records</param>
        /// <returns>Number of records</returns>
        int Count(Type T);
    }
}
