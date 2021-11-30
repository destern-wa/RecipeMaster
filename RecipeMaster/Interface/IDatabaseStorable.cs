using System;
using System.Collections.Generic;

namespace RecipeMaster.Interface
{
    /// <summary>
    /// Interface for classes that correspond to a database table, with methods
    /// for the various database operations (browse, read, edit, add, delete).
    /// These methods should be implemented explicitly, with public access via
    /// a static instance variable
    /// </summary>
    public interface IDatabaseStorable<T>
    {
        /// <summary>
        /// Unique ID for the record
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Get all records
        /// </summary>
        /// <returns>All records</returns>
        /// <exception cref="Database.RecipeDatabaseException"></exception>
        List<T> GetAll();

        /// <summary>
        /// Get records that match a condition
        /// </summary>
        /// <param name="predicate">Function that takes a record as a parameter, and
        /// returns true if the record should be included, or false if the record
        /// should be excluded.</param>
        /// <returns>Matching records</returns>
        /// <exception cref="Database.RecipeDatabaseException"></exception>
        List<T> GetWhere(Func<T, bool> predicate);

        /// <summary>
        /// Get a record by ID number
        /// </summary>
        /// <param name="id">ID number of record</param>
        /// <returns>Specified record</returns>
        /// <exception cref="Database.RecipeDatabaseException"></exception>
        T Get(int id);

        /// <summary>
        /// Update a record
        /// </summary>
        /// <param name="item">Updated version of record</param>
        /// <exception cref="Database.RecipeDatabaseException"></exception>
        void Update(T item);

        /// <summary>
        /// Add a record
        /// </summary>
        /// <param name="item">Record to add</param>
        /// <exception cref="Database.RecipeDatabaseException"></exception>
        void Add(T item);

        /// <summary>
        /// Delete a record by ID number
        /// </summary>
        /// <param name="id">ID number of record to delete</param>
        /// <exception cref="Database.RecipeDatabaseException"></exception>
        void Delete(int id);

        /// <summary>
        /// Counts how many records exist in the database
        /// </summary>
        /// <returns>Number of records</returns>
        /// <exception cref="Database.RecipeDatabaseException"></exception>
        int Count();
    }
}
